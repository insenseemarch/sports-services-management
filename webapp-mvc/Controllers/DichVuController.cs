using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using webapp_mvc.Models;

namespace webapp_mvc.Controllers
{
    public class DichVuController : Controller
    {
        private readonly DatabaseHelper _db;

        public DichVuController(DatabaseHelper db)
        {
            _db = db;
        }

        [HttpGet]
        public IActionResult Index(long maDatSan = 0, string filterMaCS = null, DateTime? ngayDat = null, TimeSpan? gioBatDau = null, TimeSpan? gioKetThuc = null)
        {
            var model = new DichVuViewModel();
            model.MaDatSan = maDatSan;
            
            // Auto-populate from booking if coming from DatSan
            if (maDatSan > 0 && !ngayDat.HasValue)
            {
                string sqlBooking = @"
                    SELECT p.NgayDat, p.GioBatDau, p.GioKetThuc, s.MaCS
                    FROM PHIEUDATSAN p
                    LEFT JOIN DATSAN d ON p.MaDatSan = d.MaDatSan
                    LEFT JOIN SAN s ON d.MaSan = s.MaSan
                    WHERE p.MaDatSan = @MaDatSan";
                
                var dtBooking = _db.ExecuteQuery(sqlBooking, new SqlParameter("@MaDatSan", maDatSan));
                if (dtBooking.Rows.Count > 0)
                {
                    var row = dtBooking.Rows[0];
                    model.NgayDat = Convert.ToDateTime(row["NgayDat"]);
                    model.GioBatDau = (TimeSpan)row["GioBatDau"];
                    model.GioKetThuc = (TimeSpan)row["GioKetThuc"];
                    if (row["MaCS"] != DBNull.Value && string.IsNullOrEmpty(filterMaCS))
                    {
                        filterMaCS = row["MaCS"].ToString();
                    }
                }
            }
            else
            {
                model.NgayDat = ngayDat ?? DateTime.Today;
                model.GioBatDau = gioBatDau ?? new TimeSpan(7, 0, 0);
                model.GioKetThuc = gioKetThuc ?? new TimeSpan(22, 0, 0);
            }
            
            // List Co So for Filter
            var listCS = new List<CoSoItem>();

            using (var conn = _db.GetConnection())
            {
                conn.Open();

                // 0. Get List Co So & LoaiDV
                using (var cmdCS = new SqlCommand("SELECT MaCS, TenCS FROM COSO", conn))
                using (var rCS = cmdCS.ExecuteReader())
                {
                    while (rCS.Read())
                    {
                        listCS.Add(new CoSoItem 
                        { 
                            MaCS = rCS["MaCS"].ToString(), 
                            TenCS = rCS["TenCS"].ToString() 
                        });
                    }
                }
                ViewBag.DanhSachCoSo = listCS;

                // Get List LoaiDV
                var listLoai = new List<string>();
                using (var cmdLoai = new SqlCommand("SELECT TenLoai FROM LOAIDV", conn))
                using (var rLoai = cmdLoai.ExecuteReader())
                {
                    while (rLoai.Read())
                    {
                        listLoai.Add(rLoai["TenLoai"].ToString());
                    }
                }
                ViewBag.DanhSachLoaiDV = listLoai;

                // 1. Get Info Phieu Dat & MaCS (Default from Booking)
                string dbMaCS = "";
                string dbTenCS = "";
                
                if (maDatSan > 0)
                {
                    string sqlInfo = @"
                        SELECT p.MaDatSan, s.MaSan as TenSan, s.MaCS, cs.TenCS,
                               p.NgayDat, p.GioBatDau, p.GioKetThuc,
                               dbo.f_TinhTienSan(p.MaDatSan) as TienSanTamTinh
                        FROM PHIEUDATSAN p
                        JOIN DATSAN d ON p.MaDatSan = d.MaDatSan
                        JOIN SAN s ON d.MaSan = s.MaSan
                        JOIN COSO cs ON s.MaCS = cs.MaCS
                        WHERE p.MaDatSan = @MaDatSan";
                    
                    var cmd = new SqlCommand(sqlInfo, conn);
                    cmd.Parameters.AddWithValue("@MaDatSan", maDatSan);
                    
                    using (var r = cmd.ExecuteReader())
                    {
                        if (r.Read())
                        {
                            dbMaCS = r["MaCS"].ToString();
                            dbTenCS = r["TenCS"].ToString();
                            
                            // Format Money
                            decimal tienSan = r["TienSanTamTinh"] != DBNull.Value ? Convert.ToDecimal(r["TienSanTamTinh"]) : 0;
                            ViewBag.TienSanTamTinh = tienSan.ToString("N0");
                            
                            model.ThongTinSan = $"Phiếu #{r["MaDatSan"]} - {r["TenSan"]} ({r["TenCS"]})";
                            
                            // Tự động gán bộ lọc theo khung giờ của phiếu đặt
                            if (r["NgayDat"] != DBNull.Value) model.NgayDat = Convert.ToDateTime(r["NgayDat"]);
                            if (r["GioBatDau"] != DBNull.Value) model.GioBatDau = (TimeSpan)r["GioBatDau"];
                            if (r["GioKetThuc"] != DBNull.Value) model.GioKetThuc = (TimeSpan)r["GioKetThuc"];
                        }
                    }
                }

                // Determine Active Filter
                string activeMaCS = "";
                if (filterMaCS == "ALL") 
                {
                    activeMaCS = ""; // Explicitly view all
                }
                else if (!string.IsNullOrEmpty(filterMaCS))
                {
                    activeMaCS = filterMaCS; // User selected specific
                }
                else
                {
                    activeMaCS = dbMaCS; // Default to Booking's Facility (if any)
                }

                // Populate ViewBag for View State
                ViewBag.CurrentMaCS = string.IsNullOrEmpty(activeMaCS) ? "ALL" : activeMaCS;
                
                // Update TenCS in model based on list
                if (!string.IsNullOrEmpty(activeMaCS))
                {
                    var found = listCS.FirstOrDefault(x => x.MaCS == activeMaCS);
                    if (found != null) model.TenCS = found.TenCS;
                    else if (!string.IsNullOrEmpty(filterMaCS)) model.TenCS = "Cơ sở " + filterMaCS;
                    else model.TenCS = dbTenCS;
                }
                
                // Set ViewBag for Filter Select
                ViewBag.CurrentMaCS = activeMaCS;

                // 2. Get Services
                // LOGIC: If activeMaCS is set, filter by it. If not, get ALL (Join to get Facility Name).
                
                string sqlDV;
                if (!string.IsNullOrEmpty(activeMaCS))
                {
                     // Case: Filter by Facility
                     sqlDV = @"
                        SELECT 
                            d.MaDV, d.DonGia, d.DVT, d.MaLoaiDV,
                            ISNULL(current_ct.SoLuong, 0) as SoLuongDaDat,
                            -- Logic SoLuongTon: HLV -> Available (100), Others -> From DV_COSO or 0
                            CASE 
                                WHEN hlv.MaNV IS NOT NULL THEN 100
                                ELSE ISNULL(dvcs.SoLuongTon, 0)
                            END as SoLuongTon, 
                            CASE 
                                WHEN hlv.MaNV IS NOT NULL THEN nv.HoTen 
                                ELSE ld.TenLoai
                            END as TenHienThi,
                            ld.TenLoai,
                            hlv.ChuyenMon,
                            @MaCS as TenCS_Item
                        FROM DICHVU d
                        JOIN LOAIDV ld ON d.MaLoaiDV = ld.MaLoaiDV
                        LEFT JOIN CT_DICHVUDAT current_ct ON d.MaDV = current_ct.MaDV AND current_ct.MaDatSan = @MaDatSan
                        LEFT JOIN DV_COSO dvcs ON d.MaDV = dvcs.MaDV AND dvcs.MaCS = @MaCS
                        LEFT JOIN HLV hlv ON d.MaDV = hlv.MaDV
                        LEFT JOIN NHANVIEN nv ON hlv.MaNV = nv.MaNV
                        WHERE (dvcs.MaDV IS NOT NULL OR (hlv.MaDV IS NOT NULL AND nv.MaCS = @MaCS))
                        -- Exclude HLV/services already booked in the selected time range
                        AND NOT EXISTS (
                            SELECT 1 FROM CT_DICHVUDAT ct
                            JOIN PHIEUDATSAN p ON ct.MaDatSan = p.MaDatSan
                            WHERE ct.MaDV = d.MaDV
                            AND p.NgayDat = @NgayDat
                            AND p.TrangThai NOT IN (N'Đã hủy', N'Nháp')
                            AND (
                                (@GioBatDau >= p.GioBatDau AND @GioBatDau < p.GioKetThuc) OR
                                (@GioKetThuc > p.GioBatDau AND @GioKetThuc <= p.GioKetThuc) OR
                                (p.GioBatDau >= @GioBatDau AND p.GioBatDau < @GioKetThuc)
                            )
                        )";
                }
                else
                {
                    // Case: View All
                    sqlDV = @"
                        SELECT 
                            d.MaDV, d.DonGia, d.DVT, d.MaLoaiDV,
                            ISNULL(current_ct.SoLuong, 0) as SoLuongDaDat,
                            CASE 
                                WHEN hlv.MaNV IS NOT NULL THEN 100
                                ELSE ISNULL(dvcs.SoLuongTon, 0)
                            END as SoLuongTon, 
                            CASE 
                                WHEN hlv.MaNV IS NOT NULL THEN nv.HoTen 
                                ELSE ld.TenLoai
                            END as TenHienThi,
                            ld.TenLoai,
                            hlv.ChuyenMon,
                            COALESCE(cs_nv.TenCS, cs_dv.TenCS, '') as TenCS_Item
                        FROM DICHVU d
                        JOIN LOAIDV ld ON d.MaLoaiDV = ld.MaLoaiDV
                        LEFT JOIN CT_DICHVUDAT current_ct ON d.MaDV = current_ct.MaDV AND current_ct.MaDatSan = @MaDatSan
                        LEFT JOIN HLV hlv ON d.MaDV = hlv.MaDV
                        LEFT JOIN NHANVIEN nv ON hlv.MaNV = nv.MaNV
                        LEFT JOIN COSO cs_nv ON nv.MaCS = cs_nv.MaCS
                        LEFT JOIN DV_COSO dvcs ON d.MaDV = dvcs.MaDV
                        LEFT JOIN COSO cs_dv ON dvcs.MaCS = cs_dv.MaCS
                        
                        -- Filter: Only show valid combos (HLV with Staff, or Item with Stock Entry)
                        WHERE (hlv.MaNV IS NOT NULL OR dvcs.MaDV IS NOT NULL)
                        -- Exclude HLV/services already booked in the selected time range
                        AND NOT EXISTS (
                            SELECT 1 FROM CT_DICHVUDAT ct
                            JOIN PHIEUDATSAN p ON ct.MaDatSan = p.MaDatSan
                            WHERE ct.MaDV = d.MaDV
                            AND p.NgayDat = @NgayDat
                            AND p.TrangThai NOT IN (N'Đã hủy', N'Nháp')
                            AND (
                                (@GioBatDau >= p.GioBatDau AND @GioBatDau < p.GioKetThuc) OR
                                (@GioKetThuc > p.GioBatDau AND @GioKetThuc <= p.GioKetThuc) OR
                                (p.GioBatDau >= @GioBatDau AND p.GioBatDau < @GioKetThuc)
                            )
                        )
                        ";
                }
                    
                using (var cmdDV = new SqlCommand(sqlDV, conn))
                {
                    if (!string.IsNullOrEmpty(activeMaCS)) cmdDV.Parameters.AddWithValue("@MaCS", activeMaCS);
                    cmdDV.Parameters.AddWithValue("@NgayDat", model.NgayDat);
                    cmdDV.Parameters.AddWithValue("@GioBatDau", model.GioBatDau);
                    cmdDV.Parameters.AddWithValue("@GioKetThuc", model.GioKetThuc);
                    cmdDV.Parameters.AddWithValue("@MaDatSan", model.MaDatSan); // Add missing param

                    using (var reader = cmdDV.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string tenHienThi = reader["TenHienThi"] != DBNull.Value ? reader["TenHienThi"].ToString() : "";
                            string loaiDV = reader["TenLoai"] != DBNull.Value ? reader["TenLoai"].ToString() : "";
                            string chuyenMon = reader["ChuyenMon"] != DBNull.Value ? reader["ChuyenMon"].ToString() : "";
                            
                            // Item Level Facility Name
                            string tenCSItem = "";
                            if (!string.IsNullOrEmpty(activeMaCS))
                            {
                                tenCSItem = model.TenCS; 
                            }
                            else
                            {
                                tenCSItem = reader["TenCS_Item"] != DBNull.Value ? reader["TenCS_Item"].ToString().Trim() : "";
                            }

                            // LOGIC TEN HIEN THI
                            string displayName = tenHienThi; 

                            if (loaiDV.ToLower().Contains("huấn luyện") || loaiDV.ToLower().Contains("hlv"))
                            {
                                if (!displayName.ToLower().Contains("hlv")) displayName = "HLV " + displayName;
                            }
                            else if (string.IsNullOrEmpty(displayName))
                            {
                                displayName = loaiDV; // Default fallback if no specific name
                            }

                            var item = new DichVuItem
                            {
                                MaDV = reader["MaDV"].ToString(),
                                TenDV = displayName,
                                SoLuong = reader["SoLuongDaDat"] != DBNull.Value ? Convert.ToInt32(reader["SoLuongDaDat"]) : 0,
                                DonGia = Convert.ToDecimal(reader["DonGia"]),
                                DVT = reader["DVT"] != DBNull.Value ? reader["DVT"].ToString() : "",
                                HinhAnh = "", // No DB Column, View handles Placeholder
                                LoaiDV = loaiDV,
                                // Remove duplicate SoLuong = 0
                                SoLuongTon = Convert.ToInt32(reader["SoLuongTon"]),
                                ChuyenMon = !string.IsNullOrEmpty(chuyenMon) ? chuyenMon : null,
                                TenCS = tenCSItem 
                            };
                            model.DanhSachDichVu.Add(item);
                        }
                    }
                }
            }
            
            if (Request.Query["msg"].Count > 0) ViewBag.Message = Request.Query["msg"];
            
            return View(model);
        }

        [HttpPost]
        public IActionResult Index(DichVuViewModel model)
        {
            var maUser = HttpContext.Session.GetString("MaUser");
            if (string.IsNullOrEmpty(maUser))
            {
                return RedirectToAction("DangNhap", "TaiKhoan");
            }

            // 1. Handle Service-Only Order (New Booking Header)
            if (model.MaDatSan <= 0)
            {
                if (string.IsNullOrEmpty(model.FilterMaCS) || model.FilterMaCS == "ALL")
                {
                    return RedirectToAction("Index", new { maDatSan = 0, msg = "Vui lòng chọn một CƠ SỞ cụ thể để đặt dịch vụ!" });
                }

                try 
                {
                    // Create Draft Booking with user-selected date/time
                    string sqlCreate = @"
                        INSERT INTO PHIEUDATSAN (MaKH, NguoiLap, NgayDat, GioBatDau, GioKetThuc, KenhDat, TrangThai, NgayTao) 
                        VALUES (@MaKH, @MaKH, @NgayDat, @GioBatDau, @GioKetThuc, N'Online', N'Nháp', GETDATE());
                        SELECT CAST(SCOPE_IDENTITY() AS BIGINT);";
                    
                    long newId = _db.ExecuteScalar<long>(sqlCreate, 
                        new SqlParameter("@MaKH", maUser),
                        new SqlParameter("@NgayDat", model.NgayDat),
                        new SqlParameter("@GioBatDau", model.GioBatDau),
                        new SqlParameter("@GioKetThuc", model.GioKetThuc));
                    
                    if (newId > 0)
                    {
                        model.MaDatSan = newId;
                    }
                }
                catch (Exception ex)
                {
                    return RedirectToAction("Index", new { maDatSan = 0, msg = "Lỗi tạo đơn hàng: " + ex.Message });
                }
            }

            var errors = new List<string>();

            // 1. Get Current Quantities to calculate Delta
            var currentQuantities = new Dictionary<string, int>();
            var sqlCurrent = "SELECT MaDV, SoLuong FROM CT_DICHVUDAT WHERE MaDatSan = @MaDatSan";
            var dtCurrent = _db.ExecuteQuery(sqlCurrent, new SqlParameter("@MaDatSan", model.MaDatSan));
            foreach (System.Data.DataRow row in dtCurrent.Rows)
            {
                 currentQuantities[row["MaDV"].ToString()] = Convert.ToInt32(row["SoLuong"]);
            }

            foreach (var item in model.DanhSachDichVu)
            {
                // Logic: Set Mode -> Delta Mode
                int oldQty = currentQuantities.ContainsKey(item.MaDV) ? currentQuantities[item.MaDV] : 0;
                int newQty = item.SoLuong;
                int delta = newQty - oldQty;

                if (delta != 0)
                {
                    var p = new SqlParameter[] {
                        new SqlParameter("@MaDatSan", model.MaDatSan),
                        new SqlParameter("@MaDV", item.MaDV),
                        new SqlParameter("@SoLuong", delta),
                        new SqlParameter("@MaCSContext", string.IsNullOrEmpty(model.FilterMaCS) ? (object)DBNull.Value : model.FilterMaCS)
                    };
                    try 
                    {
                        // Call SP with Delta
                        _db.ExecuteNonQuery("sp_ThemDichVu", p);
                    } 
                    catch (SqlException ex)
                    {
                        errors.Add($"Lỗi {item.TenDV}: {ex.Message}");
                    }
                    catch (Exception ex)
                    {
                        errors.Add($"Lỗi hệ thống {item.TenDV}: {ex.Message}");
                    }
                }
            }

            if (errors.Count > 0)
            {
                // Nếu có lỗi, stay at page và hiện lỗi
                foreach (var err in errors) ModelState.AddModelError("", err);
                // Load lại data hiển thị vì model trả về có thể thiếu info
                // (Tạm redirect về GET với Msg lỗi nếu việc reload phức tạp, hoặc reload tại chỗ)
                // Ở đây reload nhẹ:
                // Cần load lại info vì Model binding chỉ có list input
                // Logic load lại giống HttpGet (Refactor later)
                // Để đơn giản tôi return Redirect với Error Query
                return RedirectToAction("Index", new { maDatSan = model.MaDatSan, msg = string.Join("; ", errors) });
            }

            return RedirectToAction("Index", "DatSanThanhToan", new { maDatSan = model.MaDatSan });
        }
    }
}
