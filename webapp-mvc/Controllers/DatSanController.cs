using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using webapp_mvc.Models;

namespace webapp_mvc.Controllers
{
    public class DatSanController : Controller
    {
        private readonly DatabaseHelper _db;

        public DatSanController(DatabaseHelper db)
        {
            _db = db;
        }

        public IActionResult Index(string? maCS, string? maLS, DateTime? ngayDat, TimeSpan? gioBatDau, TimeSpan? gioKetThuc)
        {
            var model = new DatSanViewModel();
            model.MaCS = maCS;
            model.MaLS = maLS;
            
            // Initial defaults if empty (GET request might create empty model or bind query params)
            model.NgayDat = ngayDat ?? DateTime.Today;
            model.GioBatDau = gioBatDau ?? new TimeSpan(7, 0, 0);
            model.GioKetThuc = gioKetThuc ?? new TimeSpan(22, 0, 0);
            
            // Load Dropdowns
            LoadDropdownData(model);
            
            // Load List (with Filtering)
            // Detect if user explicitly filtered time (check if form was submitted with time values)
            bool userHasFiltered = Request.Query.ContainsKey("gioBatDau") || Request.Query.ContainsKey("gioKetThuc") || Request.Query.ContainsKey("ngayDat");
            LoadDanhSachSan(model, userHasFiltered);

            // Handle message from redirect
            if (Request.Query["msg"].Count > 0)
            {
                ViewBag.Message = Request.Query["msg"];
            }

            return View(model);
        }

        [HttpPost]
        public IActionResult Index(DatSanViewModel model)
        {
            // Debug: Log if this is AJAX
            var isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            Console.WriteLine($"[DatSan] POST received. IsAjax={isAjax}");
            
            // Validations
            if (string.IsNullOrEmpty(model.SelectedMaSan))
            {
                // Check if AJAX request
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = false, message = "Vui lòng chọn sân!" });
                }
                
                ModelState.AddModelError("", "Vui lòng chọn sân!");
                LoadDropdownData(model);
                LoadDanhSachSan(model);
                return View(model);
            }

            // Lấy User hiện tại từ Session (Hỗ trợ nhiều người dùng)
            var maUser = HttpContext.Session.GetString("MaUser");
            if (string.IsNullOrEmpty(maUser))
            {
                // Check if AJAX request
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = false, message = "Vui lòng đăng nhập!", redirectToLogin = true });
                }
                
                // Chưa đăng nhập -> Chuyển hướng
                return RedirectToAction("DangNhap", "TaiKhoan");
            }

            // Gọi SP đặt sân theo đúng chuẩn Transaction
            var p = new SqlParameter[] {
                new SqlParameter("@MaKH", maUser), 
                new SqlParameter("@NguoiLap", maUser), // Khách tự đặt
                new SqlParameter("@MaSan", model.SelectedMaSan),
                new SqlParameter("@NgayDat", model.NgayDat),
                new SqlParameter("@GioBatDau", model.GioBatDau),
                new SqlParameter("@GioKetThuc", model.GioKetThuc),
                new SqlParameter("@KenhDat", "Online")
            };

            try 
            {
                _db.ExecuteNonQuery("sp_DatSan", p);
                
                // Lấy MaDatSan vừa tạo
                string sqlGetId = "SELECT TOP 1 MaDatSan FROM PHIEUDATSAN WHERE MaKH = @MaKH ORDER BY MaDatSan DESC";
                long maDatSan = _db.ExecuteScalar<long>(sqlGetId, new SqlParameter("@MaKH", maUser));
                
                // Check if AJAX request
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { 
                        success = true, 
                        message = "Đặt sân thành công!",
                        redirectUrl = Url.Action("Index", "DichVu", new { maDatSan = maDatSan })
                    });
                }
                
                // Chuyển sang bước chọn Dịch vụ
                return RedirectToAction("Index", "DichVu", new { maDatSan = maDatSan });
            }
            catch (SqlException ex)
            {
                // Bắt các lỗi từ TRIGGER và Stored Procedure (RAISERROR)
                string errorMessage = ex.Message;
                if (errorMessage.StartsWith("Lỗi: "))
                {
                    errorMessage = errorMessage.Substring(5);
                }
                
                // Check if AJAX request
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = false, message = errorMessage });
                }
                
                // Regular request - use TempData
                TempData["ErrorMessage"] = errorMessage;
                TempData["ToastType"] = "error";
                TempData["BookingMaSan"] = model.SelectedMaSan;
                TempData["BookingNgayDat"] = model.NgayDat.ToString("yyyy-MM-dd");
                TempData["BookingGioBatDau"] = model.GioBatDau.ToString(@"hh\:mm");
                TempData["BookingGioKetThuc"] = model.GioKetThuc.ToString(@"hh\:mm");
                
                return RedirectToAction("Index", new { maCS = model.MaCS, maLS = model.MaLS });
            }
            catch (Exception ex)
            {
                // Check if AJAX request
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = false, message = "Lỗi hệ thống: " + ex.Message });
                }
                
                TempData["ErrorMessage"] = "Lỗi hệ thống: " + ex.Message;
                TempData["ToastType"] = "error";
                TempData["BookingMaSan"] = model.SelectedMaSan;
                TempData["BookingNgayDat"] = model.NgayDat.ToString("yyyy-MM-dd");
                TempData["BookingGioBatDau"] = model.GioBatDau.ToString(@"hh\:mm");
                TempData["BookingGioKetThuc"] = model.GioKetThuc.ToString(@"hh\:mm");
                
                return RedirectToAction("Index", new { maCS = model.MaCS, maLS = model.MaLS });
            }
        }


        private void LoadDropdownData(DatSanViewModel model)
        {
            using (var conn = _db.GetConnection())
            {
                conn.Open();
                // Load CoSo
                using (var cmd = new SqlCommand("SELECT MaCS, TenCS FROM COSO", conn))
                using (var r = cmd.ExecuteReader())
                {
                    while (r.Read()) model.DanhSachCoSo.Add(new CoSoItem { MaCS = r["MaCS"].ToString(), TenCS = r["TenCS"].ToString() });
                }
                // Load LoaiSan
                using (var cmd = new SqlCommand("SELECT MaLS, TenLS FROM LOAISAN", conn))
                using (var r = cmd.ExecuteReader())
                {
                    while (r.Read()) model.DanhSachLoaiSan.Add(new LoaiSanItem { MaLS = r["MaLS"].ToString(), TenLS = r["TenLS"].ToString() });
                }
            }
        }



        private void LoadDanhSachSan(DatSanViewModel model, bool userHasFiltered = false)
        {
            // 1. Setup Filter Logic
            bool isFilteringByTime = userHasFiltered && (model.GioKetThuc > model.GioBatDau);
            var parameters = new List<SqlParameter>();
            
            // 2. Dynamic Price Query
            // Default: Price range across all hours
            string minGiaSql = "(SELECT MIN(GiaApDung) FROM KHUNGGIO k WHERE k.MaLS = s.MaLS) as MinGia";
            string maxGiaSql = "(SELECT MAX(GiaApDung) FROM KHUNGGIO k WHERE k.MaLS = s.MaLS) as MaxGia";

            if (isFilteringByTime)
            {
                // Filtered: Price range strictly within filtered hours
                minGiaSql = @"(SELECT ISNULL(MIN(GiaApDung), (SELECT MIN(GiaApDung) FROM KHUNGGIO WHERE MaLS = s.MaLS)) 
                               FROM KHUNGGIO k 
                               WHERE k.MaLS = s.MaLS 
                               AND k.GioBatDau < @GioKetThuc AND k.GioKetThuc > @GioBatDau) as MinGia";
                               
                maxGiaSql = @"(SELECT ISNULL(MAX(GiaApDung), (SELECT MAX(GiaApDung) FROM KHUNGGIO WHERE MaLS = s.MaLS)) 
                               FROM KHUNGGIO k 
                               WHERE k.MaLS = s.MaLS 
                               AND k.GioBatDau < @GioKetThuc AND k.GioKetThuc > @GioBatDau) as MaxGia";
            }

            // 3. Construct Main Query
            string sql = $@"
                SELECT s.MaSan, s.MaLS, ls.TenLS, ls.DVT, cs.TenCS, s.SucChua, s.TinhTrang,
                       {minGiaSql},
                       {maxGiaSql},
                       CASE WHEN EXISTS (
                           SELECT 1 FROM PHIEUBAOTRI pb 
                           WHERE pb.MaSan = s.MaSan 
                             AND pb.TrangThai = N'Đang bảo trì'
                             AND GETDATE() BETWEEN pb.NgayBatDau AND ISNULL(pb.NgayKetThucThucTe, pb.NgayKetThucDuKien)
                       ) THEN 1 ELSE 0 END as DangBaoTri
                FROM SAN s
                JOIN LOAISAN ls ON s.MaLS = ls.MaLS
                JOIN COSO cs ON s.MaCS = cs.MaCS
                WHERE 1=1";

            // 4. Apply Filters
            if (!string.IsNullOrEmpty(model.MaCS)) 
            {
                sql += " AND s.MaCS = @MaCS";
                parameters.Add(new SqlParameter("@MaCS", model.MaCS));
            }
            if (!string.IsNullOrEmpty(model.MaLS)) 
            {
                sql += " AND s.MaLS = @MaLS";
                parameters.Add(new SqlParameter("@MaLS", model.MaLS));
            }

            if (isFilteringByTime)
            {
                // Check availability
                sql += " AND dbo.f_KiemTraSanTrong(s.MaSan, @NgayDat, @GioBatDau, @GioKetThuc, NULL) = 1";
                parameters.Add(new SqlParameter("@NgayDat", model.NgayDat));
                parameters.Add(new SqlParameter("@GioBatDau", model.GioBatDau));
                parameters.Add(new SqlParameter("@GioKetThuc", model.GioKetThuc));
            }
            
            sql += " ORDER BY s.MaSan";

            // 5. Execute & Bind
            var dt = _db.ExecuteQuery(sql, parameters.ToArray());
            
            // 6. Pricing Rules (Optional fallback needed?) - Not needed if subquery handles it
            
            foreach (DataRow row in dt.Rows)
            {
                var item = new SanItem
                {
                    MaSan = row["MaSan"].ToString(),
                    TenSan = row["MaSan"].ToString(),
                    TenLoaiSan = row["TenLS"].ToString(),
                    TenCoSo = row["TenCS"].ToString(),
                    SucChua = Convert.ToInt32(row["SucChua"]),
                    TinhTrang = row["TinhTrang"].ToString(),
                    DangBaoTri = Convert.ToInt32(row["DangBaoTri"]) == 1,
                    MinGia = row["MinGia"] != DBNull.Value ? Convert.ToDecimal(row["MinGia"]) : 0,
                    MaxGia = row["MaxGia"] != DBNull.Value ? Convert.ToDecimal(row["MaxGia"]) : 0
                };

                // Hiển thị giá: Luôn là giá/giờ hoặc giá/trận (min-max nếu có nhiều khung giờ)
                string dvt = row["DVT"] != DBNull.Value ? row["DVT"].ToString() : "Giờ";
                
                if (item.MinGia == item.MaxGia)
                {
                    item.HienThiGia = item.MinGia.ToString("N0");
                }
                else
                {
                    item.HienThiGia = $"{item.MinGia:N0} - {item.MaxGia:N0}";
                }
                
                item.DonViTinh = dvt;
                item.GiaGio = item.MinGia; // Dùng giá min làm giá đại diện

                model.DanhSachSanTrong.Add(item);
            }
        }

        private decimal CalculatePrice(DataTable dtKG, string maLS, string tenLS, TimeSpan start, TimeSpan end)
        {
             if (dtKG == null) return 0;

             // Find applicable rule containing Start Time
             DataRow bestRow = null;
             foreach(DataRow r in dtKG.Select($"MaLS = '{maLS}'"))
             {
                 TimeSpan kStart = (TimeSpan)r["GioBatDau"];
                 TimeSpan kEnd = (TimeSpan)r["GioKetThuc"];
                 // Check if Start Time is within this bucket
                 if (start >= kStart && start < kEnd)
                 {
                     bestRow = r;
                     break; 
                 }
             }
             
             // Fallback: If no bucket covers this hour, take the LAST/DEFAULT rule (often the one ending at 23:00 or standard)
             if (bestRow == null)
             {
                 var rows = dtKG.Select($"MaLS = '{maLS}'");
                 if (rows.Length > 0) bestRow = rows[0]; // Take random valid rule to avoid 0
             }
             
             if (bestRow == null) return 0;

             decimal price = Convert.ToDecimal(bestRow["GiaApDung"]);
             double minutes = (end - start).TotalMinutes;
             
             // Logic matching DB function
             if (tenLS.Contains("Bóng đá") || tenLS.Contains("Futsal"))
                return price * (decimal)(minutes / 90.0); // 90 mins standard
             else
                return price * (decimal)(minutes / 60.0); // 60 mins standard
        }
    }
}
