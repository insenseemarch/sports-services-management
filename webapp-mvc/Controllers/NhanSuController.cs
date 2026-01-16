using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using webapp_mvc.Models;

namespace webapp_mvc.Controllers
{
    public class NhanSuController : Controller
    {
        private readonly DatabaseHelper _db;

        public NhanSuController(DatabaseHelper db)
        {
            _db = db;
        }

        [HttpGet]
        public IActionResult Index(string? search)
        {
            var model = new NhanSuViewModel();
            model.SearchKeyword = search ?? "";

            using (var conn = _db.GetConnection())
            {
                conn.Open();
                
                // 1. Load Dropdown Co So
                string sqlCS = "SELECT MaCS, TenCS FROM COSO";
                using (var cmd = new SqlCommand(sqlCS, conn))
                using (var r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        model.DanhSachCoSo.Add(new CoSoItem
                        {
                            MaCS = r["MaCS"].ToString()!,
                            TenCS = r["TenCS"].ToString()!
                        });
                    }
                }

                // 2. Load Danh Sach NV
                string sqlNV = @"
                    SELECT nv.MaNV, nv.HoTen, nv.ChucVu, nv.SDT, nv.LuongCoBan, cs.TenCS, tk.TenDangNhap, nv.TrangThai
                    FROM NHANVIEN nv
                    LEFT JOIN COSO cs ON nv.MaCS = cs.MaCS
                    LEFT JOIN TAIKHOAN tk ON nv.MaTK = tk.MaTK
                    WHERE (@Search = '' OR nv.HoTen LIKE N'%' + @Search + '%')";

                using (var cmd = new SqlCommand(sqlNV, conn))
                {
                    cmd.Parameters.AddWithValue("@Search", model.SearchKeyword);
                    using (var r = cmd.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            model.DanhSachNhanVien.Add(new NhanVienItem
                            {
                                MaNV = r["MaNV"].ToString()!,
                                HoTen = r["HoTen"].ToString()!,
                                ChucVu = r["ChucVu"].ToString()!,
                                SDT = r["SDT"] != DBNull.Value ? r["SDT"].ToString()! : "",
                                LuongCoBan = r["LuongCoBan"] != DBNull.Value ? Convert.ToDecimal(r["LuongCoBan"]) : 0,
                                TenCoSo = r["TenCS"] != DBNull.Value ? r["TenCS"].ToString()! : "",
                                TenDangNhap = r["TenDangNhap"] != DBNull.Value ? r["TenDangNhap"].ToString()! : "",
                                TrangThai = r["TrangThai"] != DBNull.Value ? r["TrangThai"].ToString()! : "Đang làm"
                            });
                        }
                    }
                }
            }

            return View(model);
        }

        [HttpPost]
        public IActionResult Create(NhanSuViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (var conn = _db.GetConnection())
                {
                    conn.Open();
                    using (var tran = conn.BeginTransaction())
                    {
                        try
                        {
                            // 1. Check duplicate user
                            string checkUser = "SELECT COUNT(*) FROM TAIKHOAN WHERE TenDangNhap = @User";
                            var cmdCheck = new SqlCommand(checkUser, conn, tran);
                            cmdCheck.Parameters.AddWithValue("@User", model.TenDangNhap);
                            int exist = (int)cmdCheck.ExecuteScalar();
                            if (exist > 0) throw new Exception("Tên đăng nhập đã tồn tại!");

                            // 2. Generate ID
                            string maTK = "TK" + DateTime.Now.ToString("ddHHmmss"); 
                            string maNV = "NV" + DateTime.Now.ToString("ddHHmmss");

                            // 3. Insert TAIKHOAN
                            string sqlTK = @"INSERT INTO TAIKHOAN (MaTK, TenDangNhap, MatKhau, VaiTro, NgayDangKy)
                                             VALUES (@MaTK, @User, @Pass, 'NhanVien', GETDATE())";
                            var cmdTK = new SqlCommand(sqlTK, conn, tran);
                            cmdTK.Parameters.AddWithValue("@MaTK", maTK);
                            cmdTK.Parameters.AddWithValue("@User", model.TenDangNhap);
                            cmdTK.Parameters.AddWithValue("@Pass", model.MatKhau); 
                            cmdTK.ExecuteNonQuery();

                            // 4. Insert NHANVIEN (Fix column names matching DB)
                            // Bỏ Email, NgayVaoLam. Thêm CMND_CCCD.
                            string sqlNV = @"INSERT INTO NHANVIEN (MaNV, HoTen, NgaySinh, GioiTinh, DiaChi, SDT, CMND_CCCD, LuongCoBan, ChucVu, MaTK, MaCS)
                                             VALUES (@MaNV, @HoTen, @NgaySinh, @GioiTinh, @DiaChi, @SDT, @CCCD, @Luong, @ChucVu, @MaTK, @MaCS)";
                            var cmdNV = new SqlCommand(sqlNV, conn, tran);
                            cmdNV.Parameters.AddWithValue("@MaNV", maNV);
                            cmdNV.Parameters.AddWithValue("@HoTen", model.HoTen);
                            cmdNV.Parameters.AddWithValue("@NgaySinh", model.NgaySinh);
                            cmdNV.Parameters.AddWithValue("@GioiTinh", model.GioiTinh);
                            cmdNV.Parameters.AddWithValue("@DiaChi", model.DiaChi ?? "");
                            cmdNV.Parameters.AddWithValue("@SDT", model.SDT ?? "");
                            cmdNV.Parameters.AddWithValue("@CCCD", model.CCCD); // New required param
                            // cmdNV.Parameters.AddWithValue("@Email", ... REMOVED);
                            cmdNV.Parameters.AddWithValue("@Luong", model.LuongCoBan);
                            cmdNV.Parameters.AddWithValue("@ChucVu", model.ChucVu);
                            cmdNV.Parameters.AddWithValue("@MaTK", maTK);
                            cmdNV.Parameters.AddWithValue("@MaCS", model.MaCS ?? (object)DBNull.Value);
                            
                            cmdNV.ExecuteNonQuery();

                            tran.Commit();
                            return RedirectToAction("Index", new { msg = "Thêm nhân viên thành công!" });
                        }
                        catch (Exception ex)
                        {
                            tran.Rollback();
                            model.Message = "Lỗi: " + ex.Message;
                            model.IsSuccess = false;
                        }
                    }
                }
            }
            return Index(""); 
        }
        [HttpPost]
        public IActionResult XoaNhanVien(string maNV)
        {
            try
            {
                // First, get MaTK for this employee
                string? maTK = null;
                using (var conn = _db.GetConnection())
                {
                    conn.Open();
                    string sqlGetMaTK = "SELECT MaTK FROM NHANVIEN WHERE MaNV = @MaNV";
                    using (var cmd = new SqlCommand(sqlGetMaTK, conn))
                    {
                        cmd.Parameters.AddWithValue("@MaNV", maNV);
                        var result = cmd.ExecuteScalar();
                        if (result != null)
                            maTK = result.ToString();
                    }
                }

                if (string.IsNullOrEmpty(maTK))
                {
                    return Json(new { success = false, message = "Không tìm thấy nhân viên" });
                }

                // Check if employee has related data in other tables
                bool hasRelatedData = false;
                using (var conn = _db.GetConnection())
                {
                    conn.Open();
                    
                        // Check multiple tables for references. Use per-table queries wrapped in try/catch
                        // to avoid failing when some optional tables do not exist in the current DB.
                        var checkQueries = new List<string>
                        {
                            "SELECT COUNT(*) FROM PHIEUDATSAN WHERE NguoiLap = @MaNV",
                            "SELECT COUNT(*) FROM THAMGIACATRUC WHERE MaNV = @MaNV",
                            "SELECT COUNT(*) FROM DONNGHIPHEP WHERE MaNV = @MaNV",
                            "SELECT COUNT(*) FROM PHIEUNHAPKHO WHERE NguoiNhap = @MaNV",
                            "SELECT COUNT(*) FROM PHIEUBAOTRI WHERE MaNV = @MaNV",
                            "SELECT COUNT(*) FROM BAOCAOTHONGKE WHERE NguoiLapPhieu = @MaNV"
                        };

                        foreach (var q in checkQueries)
                        {
                            try
                            {
                                using (var cmd = new SqlCommand(q, conn))
                                {
                                    cmd.Parameters.AddWithValue("@MaNV", maNV);
                                    var obj = cmd.ExecuteScalar();
                                    if (obj != null && Convert.ToInt32(obj) > 0)
                                    {
                                        hasRelatedData = true;
                                        break;
                                    }
                                }
                            }
                            catch (SqlException)
                            {
                                // Table or column may not exist in this DB snapshot; skip and continue checking others.
                                continue;
                            }
                        }
                }

                if (hasRelatedData)
                {
                    // Has related data - update to "Nghỉ việc" status instead of deleting
                    // Clear MaCS (workplace) and set LuongCoBan to NULL
                    string sqlUpdate = @"
                        UPDATE NHANVIEN 
                        SET MaCS = NULL, 
                            LuongCoBan = NULL,
                            TrangThai = N'Đã nghỉ việc'
                        WHERE MaNV = @MaNV";
                    
                    _db.ExecuteNonQuery(sqlUpdate, new SqlParameter("@MaNV", maNV));
                    
                    return Json(new { 
                        success = true, 
                        fullyDeleted = false,
                        message = "Nhân viên có dữ liệu liên quan. Đã chuyển sang trạng thái 'Nghỉ việc' và xóa thông tin cơ sở, lương." 
                    });
                }
                else
                {
                    // No related data - safe to delete completely
                    try
                    {
                        _db.ExecuteNonQuery("DELETE FROM NHANVIEN WHERE MaNV = @MaNV", 
                            new SqlParameter("@MaNV", maNV));
                        
                        // Also delete TAIKHOAN
                        _db.ExecuteNonQuery("DELETE FROM TAIKHOAN WHERE MaTK = @MaTK", 
                            new SqlParameter("@MaTK", maTK));
                        
                        return Json(new { 
                            success = true, 
                            fullyDeleted = true,
                            message = "Xóa nhân viên thành công!" 
                        });
                    }
                    catch (SqlException ex)
                    {
                        // If still fails due to other FK constraints, update instead
                        string sqlUpdate = @"
                            UPDATE NHANVIEN 
                            SET MaCS = NULL, 
                                LuongCoBan = NULL,
                                TrangThai = N'Đã nghỉ việc'
                            WHERE MaNV = @MaNV";
                        
                        _db.ExecuteNonQuery(sqlUpdate, new SqlParameter("@MaNV", maNV));
                        
                        return Json(new { 
                            success = true, 
                            fullyDeleted = false,
                            message = "Nhân viên có ràng buộc dữ liệu. Đã chuyển sang trạng thái 'Nghỉ việc'." 
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        [HttpPost]
        public IActionResult PhantomReadDemo(string demoMode)
        {
            // demoMode: "unsafe" (Repeatable Read) vs "safe" (Serializable)
            string spName = (demoMode == "safe") ? "sp_PhantomRead_Demo_Safe" : "sp_PhantomRead_Demo_Unsafe";
            
            // Log for debug
            Console.WriteLine($"[PHANTOM READ DEMO] Mode: {demoMode}, SP: {spName}");

            try 
            {
                using (var conn = _db.GetConnection())
                {
                    conn.Open();
                    // We need a long timeout because the SP waits 15s
                    using (var cmd = new SqlCommand(spName, conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.CommandTimeout = 60; 

                        using (var r = cmd.ExecuteReader())
                        {
                            if (r.Read())
                            {
                                int before = Convert.ToInt32(r["CountBefore"]);
                                int after = Convert.ToInt32(r["CountAfter"]);
                                
                                string msg = (before != after) 
                                    ? $"⚠ LỖI PHANTOM READ: Đọc lần 1 = {before}, Đọc lần 2 = {after} (Xuất hiện {after - before} nhân viên ma)."
                                    : $"✅ KẾT QUẢ NHẤT QUÁN: Đọc lần 1 = {before}, Đọc lần 2 = {after}.";

                                return Json(new { success = true, before = before, after = after, message = msg });
                            }
                        }
                    }
                }
                return Json(new { success = false, message = "Không có dữ liệu trả về từ SP." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi Demo: " + ex.Message });
            }
        }
    }
}
