using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using webapp_mvc.Models;

namespace webapp_mvc.Controllers
{
    public class DatSanController : Controller
    {
        private readonly DatabaseHelper _db;
        private readonly IConfiguration _configuration;

        public DatSanController(DatabaseHelper db, IConfiguration configuration)
        {
            _db = db;
            _configuration = configuration;
        }

        public IActionResult Index(string? maCS, string? maLS, DateTime? ngayDat, TimeSpan? gioBatDau, TimeSpan? gioKetThuc, string? demoMode)
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
            LoadDanhSachSan(model, userHasFiltered, demoMode);

            // Pass demoMode to View for UI state
            ViewBag.DemoMode = demoMode;

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
            var vaiTro = HttpContext.Session.GetString("VaiTro");
            
            // XỬ LÝ ĐẶT HỘ (CHO NHÂN VIÊN): Ưu tiên lấy MaKH từ form nếu có
            string maKHDuocDat = maUser;
            string bookingFor = Request.Form["BookingForMaKH"];
            
            if (!string.IsNullOrEmpty(vaiTro) && vaiTro != "Khách hàng" && !string.IsNullOrEmpty(bookingFor))
            {
                maKHDuocDat = bookingFor;
            }

            if (string.IsNullOrEmpty(maKHDuocDat))
            {
                // Check if AJAX request
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = false, message = "Vui lòng đăng nhập!", redirectToLogin = true });
                }
                
                // Chưa đăng nhập -> Chuyển hướng
                return RedirectToAction("DangNhap", "TaiKhoan");
            }

            // Kiểm tra giờ hoạt động của cơ sở
            string sqlCheckHours = @"
                SELECT cs.GioMoCua, cs.GioDongCua, cs.TenCS
                FROM SAN s
                JOIN COSO cs ON s.MaCS = cs.MaCS
                WHERE s.MaSan = @MaSan";

            var dtHours = _db.ExecuteQuery(sqlCheckHours, new SqlParameter("@MaSan", model.SelectedMaSan));

            if (dtHours.Rows.Count > 0)
            {
                var row = dtHours.Rows[0];
                TimeSpan gioMoCua = (TimeSpan)row["GioMoCua"];
                TimeSpan gioDongCua = (TimeSpan)row["GioDongCua"];
                string tenCS = row["TenCS"].ToString();
                
                if (model.GioBatDau < gioMoCua || model.GioKetThuc > gioDongCua)
                {
                    string errorMsg = $"Cơ sở {tenCS} chỉ hoạt động từ {gioMoCua:hh\\:mm} đến {gioDongCua:hh\\:mm}. Vui lòng chọn giờ trong khung giờ này!";
                    
                    if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    {
                        return Json(new { success = false, message = errorMsg });
                    }
                    
                    ModelState.AddModelError("", errorMsg);
                    LoadDropdownData(model);
                    LoadDanhSachSan(model);
                    return View(model);
                }
            }

            // TẠO PHIẾU NHÁP NGAY (để có maDatSan)
            // Phiếu nháp sẽ không bị tính vào trigger kiểm tra trùng lịch
            var p = new SqlParameter[] {
                new SqlParameter("@MaKH", maKHDuocDat), 
                new SqlParameter("@NguoiLap", maUser), // Người lập phiếu (có thể là NV hoặc KH)
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
                long maDatSan = _db.ExecuteScalar<long>(sqlGetId, new SqlParameter("@MaKH", maKHDuocDat));
                
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
                
                ModelState.AddModelError("", errorMessage);
                LoadDropdownData(model);
                LoadDanhSachSan(model);
                return View(model);
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



        private void LoadDanhSachSan(DatSanViewModel model, bool userHasFiltered = false, string? demoMode = null)
        {
             // Determine Connection & Isolation Level for Demo
             string connectionString = _configuration.GetConnectionString("DefaultConnection");
             string isolationQuery = ""; 
             
             // Map 'default' (from View) to 'unsafe' logic (Read Uncommitted for Dirty Read Demo)
             // Map 'fixed' (from View) to 'safe' logic (Read Committed for Dirty Read Demo)
             if (demoMode == "unsafe" || demoMode == "default") 
             {
                 // DIRTY READ DEMO: Force Read Uncommitted to see uncommitted 'Bảo trì' status
                 // UNREPEATABLE READ: Also works (Read Uncommitted is even worse than Read Committed)
                 isolationQuery = "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;";
             }
             if (demoMode == "safe" || demoMode == "fixed")
             {
                 // SAFE DEMO: Use Fixed DB + Read Committed (Default)
                 connectionString = _configuration.GetConnectionString("FixedConnection");
                 isolationQuery = "SET TRANSACTION ISOLATION LEVEL READ COMMITTED;";
             }

             // 1. Setup Filter Logic
             bool isFilteringByTime = userHasFiltered && (model.GioKetThuc > model.GioBatDau);
             List<SqlParameter> checkParams = new List<SqlParameter>();
             
             // 2. Dynamic Price Query
             string minGiaSql = "(SELECT MIN(GiaApDung) FROM KHUNGGIO k WHERE k.MaLS = s.MaLS) as MinGia";
             string maxGiaSql = "(SELECT MAX(GiaApDung) FROM KHUNGGIO k WHERE k.MaLS = s.MaLS) as MaxGia";

             if (isFilteringByTime)
             {
                 minGiaSql = @"(SELECT ISNULL(MIN(GiaApDung), (SELECT MIN(GiaApDung) FROM KHUNGGIO WHERE MaLS = s.MaLS)) 
                                FROM KHUNGGIO k 
                                WHERE k.MaLS = s.MaLS 
                                AND k.GioBatDau < @GioKetThuc AND k.GioKetThuc > @GioBatDau) as MinGia";
                                
                 maxGiaSql = @"(SELECT ISNULL(MAX(GiaApDung), (SELECT MAX(GiaApDung) FROM KHUNGGIO WHERE MaLS = s.MaLS)) 
                                FROM KHUNGGIO k 
                                WHERE k.MaLS = s.MaLS 
                                AND k.GioBatDau < @GioKetThuc AND k.GioKetThuc > @GioBatDau) as MaxGia";
             }

             string sql = isolationQuery + $@"
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
                WHERE 1=1 
                AND s.TinhTrang NOT IN (N'Bảo Trì', N'Đang Bảo Trì', N'Ngưng Hoạt Động', N'Không Hoạt Động', N'Không hoạt động', N'Ngưng hoạt động')";

             // 3. Apply Filters
             if (!string.IsNullOrEmpty(model.MaCS)) 
             {
                 sql += " AND s.MaCS = @MaCS";
                 checkParams.Add(new SqlParameter("@MaCS", model.MaCS));
             }
             if (!string.IsNullOrEmpty(model.MaLS)) 
             {
                 sql += " AND s.MaLS = @MaLS";
                 checkParams.Add(new SqlParameter("@MaLS", model.MaLS));
             }

             if (isFilteringByTime)
             {
                 sql += " AND dbo.f_KiemTraSanTrong(s.MaSan, @NgayDat, @GioBatDau, @GioKetThuc, NULL) = 1";
                 checkParams.Add(new SqlParameter("@NgayDat", model.NgayDat));
                 checkParams.Add(new SqlParameter("@GioBatDau", model.GioBatDau));
                 checkParams.Add(new SqlParameter("@GioKetThuc", model.GioKetThuc));
             }
             
             sql += " ORDER BY s.MaSan";

             // 4. Execute Query
             DataTable dt = new DataTable();
             if (!string.IsNullOrEmpty(demoMode))
             {
                 // Manual ADO.NET for Demo Mode
                 using (SqlConnection conn = new SqlConnection(connectionString))
                 {
                     conn.Open();
                     using (SqlCommand cmd = new SqlCommand(sql, conn))
                     {
                         if(checkParams.Count > 0) cmd.Parameters.AddRange(checkParams.ToArray());
                         using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                         {
                             da.Fill(dt);
                         }
                     }
                 }
                 
                 // Global Min/Max not fetched here for demo mode to simplify, or assume pre-calculated? 
                 // Actually they are used in mapping.
             }
             else 
             {
                 // Normal flow using existing Helper
                 // Note: Does not verify Global Min/Max for fallback here to keep it short, assumes DB values are fine.
                 dt = _db.ExecuteQuery(sql, checkParams.ToArray());
             }
             
             // 5. Map to Model
             MapDtToModel(dt, model);
        }

        private void MapDtToModel(DataTable dt, DatSanViewModel model)
        {
             // Need Global Price for fallback? 
             // Ideally this should be passed in or fetched. For now using 0 as fallback or hardcoded.
             // Re-fetching global purely for fallback if needed.
             // Ideally we shouldn't fetch it every time inside this helper.
             // We'll skip the Global Fallback for now to simplify or fetch it once.
             // To keep it 1:1, let's fetch it if needed or assume 0.
             decimal globalMin = 50000; // Approximate fallback
             decimal globalMax = 500000;

             foreach (DataRow row in dt.Rows)
             {
                var item = new SanItem
                {
                    MaSan = row["MaSan"].ToString(),
                    MaLS = row["MaLS"].ToString(),
                    TenSan = row["MaSan"].ToString(),
                    TenLoaiSan = row["TenLS"].ToString(),
                    TenCoSo = row["TenCS"].ToString(),
                    SucChua = Convert.ToInt32(row["SucChua"]),
                    TinhTrang = row["TinhTrang"].ToString(),
                    DangBaoTri = Convert.ToInt32(row["DangBaoTri"]) == 1,
                    MinGia = row["MinGia"] != DBNull.Value ? Convert.ToDecimal(row["MinGia"]) : 0,
                    MaxGia = row["MaxGia"] != DBNull.Value ? Convert.ToDecimal(row["MaxGia"]) : 0
                };
                
                 string dvt = row["DVT"] != DBNull.Value ? row["DVT"].ToString() : "Giờ";
                 
                 // Fallback
                 if (item.MinGia <= 0) item.MinGia = globalMin;
                 if (item.MaxGia <= 0) item.MaxGia = globalMax;

                 if (item.MinGia > 0 && item.MaxGia > 0 && item.MinGia == item.MaxGia)
                    item.HienThiGia = item.MinGia.ToString("N0");
                 else if (item.MinGia > 0 && item.MaxGia > 0)
                    item.HienThiGia = $"{item.MinGia:N0} - {item.MaxGia:N0}";
                 else
                    item.HienThiGia = "Liên hệ";
                
                item.DonViTinh = dvt;
                item.GiaGio = item.MinGia > 0 ? item.MinGia : item.MaxGia;
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

        [HttpGet]
        public IActionResult GetKhachHang(string sdt)
        {
            if (string.IsNullOrEmpty(sdt))
                return Json(new { success = false, message = "Vui lòng nhập SĐT" });
            
            var dt = _db.ExecuteQuery("SELECT MaKH, HoTen FROM KHACHHANG WHERE SDT = @SDT", 
                new SqlParameter("@SDT", sdt));
            
            if (dt.Rows.Count > 0)
            {
                return Json(new { 
                    success = true, 
                    makh = dt.Rows[0]["MaKH"].ToString(),
                    hoten = dt.Rows[0]["HoTen"].ToString()
                });
            }
            return Json(new { success = false, message = "Không tìm thấy khách hàng này!" });
        }

        /// <summary>
        /// API Endpoint: Lấy giá sân theo khung giờ
        /// Hỗ trợ demo Unrepeatable Read bằng cách chọn database mode
        /// </summary>
        [HttpGet]
        public IActionResult GetCourtPriceByTimeSlot(string maLS, string gioBatDau, bool useFixedDb = false, DateTime? ngayDat = null)
        {
            try
            {
                if (string.IsNullOrEmpty(maLS) || string.IsNullOrEmpty(gioBatDau))
                {
                    return Json(new { success = false, message = "Thiếu thông tin cần thiết" });
                }

                // Parse time
                if (!TimeSpan.TryParse(gioBatDau, out TimeSpan gioBD))
                {
                    return Json(new { success = false, message = "Giờ bắt đầu không hợp lệ" });
                }

                // Lấy connection string tương ứng với database mode
                var configuration = HttpContext.RequestServices.GetService(typeof(IConfiguration)) as IConfiguration;
                string connectionString = useFixedDb 
                    ? configuration.GetConnectionString("FixedConnection")
                    : configuration.GetConnectionString("DefaultConnection");

                // Chọn stored procedure tương ứng
                string spName = useFixedDb 
                    ? "sp_GetCourtPrice_WithRepeatableRead" 
                    : "sp_GetCourtPrice_NoIsolation";

                // Tạo connection riêng cho query này
                using (var conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (var cmd = new SqlCommand(spName, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@MaLS", maLS);
                        cmd.Parameters.AddWithValue("@GioBatDau", gioBD);
                        
                        if (ngayDat.HasValue)
                            cmd.Parameters.AddWithValue("@NgayDat", ngayDat.Value);

                        // Note: We don't strictly need the output parameter if we select it in the result set, 
                        // but keeping it for compatibility if needed. The SP returns it in result set now.
                        SqlParameter transactionIdParam = null;
                        if (useFixedDb)
                        {
                            transactionIdParam = new SqlParameter("@TransactionId", SqlDbType.UniqueIdentifier)
                            {
                                Direction = ParameterDirection.Output
                            };
                            cmd.Parameters.Add(transactionIdParam);
                        }

                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Safe retrieval helper
                                T GetValue<T>(string colName, T defaultValue = default)
                                {
                                    try 
                                    {
                                        int ord = reader.GetOrdinal(colName);
                                        if (reader.IsDBNull(ord)) return defaultValue;
                                        return (T)reader.GetValue(ord);
                                    }
                                    catch 
                                    { 
                                        return defaultValue; 
                                    }
                                }

                                decimal giaApDung = GetValue<decimal>("GiaApDung", 0);
                                string dvt = GetValue<string>("DVT", "giờ");
                                string isolationLevel = GetValue<string>("IsolationLevel", "Unknown");
                                DateTime queryTime = GetValue<DateTime>("QueryTime", DateTime.Now);
                                
                                // Try read TransactionId from result set first (more reliable in loop)
                                string txIdStr = null;
                                if (useFixedDb)
                                {
                                    try {
                                        var txObj = GetValue<object>("TransactionId");
                                        if (txObj != null) txIdStr = txObj.ToString();
                                    } catch { /* ignore */ }
                                }

                                var result = new
                                {
                                    success = true,
                                    price = giaApDung,
                                    formattedPrice = giaApDung.ToString("N0"),
                                    unit = dvt,
                                    isolationLevel = isolationLevel,
                                    databaseMode = useFixedDb ? "FIXED (REPEATABLE READ)" : "DEFAULT (READ COMMITTED)",
                                    queryTime = queryTime.ToString("HH:mm:ss"),
                                    transactionId = txIdStr // Use value from reader
                                };

                                return Json(result);
                            }
                        }
                    }
                }

                return Json(new { success = false, message = "Không tìm thấy giá cho khung giờ này" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi: " + ex.Message });
            }
        }
    }
}
