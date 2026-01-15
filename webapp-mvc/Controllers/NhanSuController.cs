using Microsoft.AspNetCore.Mvc;
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
                    SELECT nv.MaNV, nv.HoTen, nv.ChucVu, nv.SDT, nv.LuongCoBan, cs.TenCS, tk.TenDangNhap 
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
                                TenDangNhap = r["TenDangNhap"] != DBNull.Value ? r["TenDangNhap"].ToString()! : ""
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
        public IActionResult Delete(string id)
        {
            try
            {
                 // Delete NHANVIEN first. (Note: Will fail if FK exists)
                 _db.ExecuteNonQuery("DELETE FROM NHANVIEN WHERE MaNV = @ID", new SqlParameter("@ID", id));
                 // Optional: Delete TAIKHOAN associated? A bit complex if we need MaTK.
                 
                 return RedirectToAction("Index", new { msg = "Xóa nhân viên thành công!" });
            }
            catch (Exception ex)
            {
                // Likely FK constraint
                return RedirectToAction("Index", new { msg = "Lỗi: Không thể xóa nhân viên này (đang có dữ liệu liên quan)." });
            }
        }
    }
}
