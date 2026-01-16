using Microsoft.AspNetCore.Mvc;
using webapp_mvc.Models;
using System.Data.SqlClient;
using System.Data;

namespace webapp_mvc.Controllers
{
    public class ManagementController : Controller
    {
        private readonly DatabaseHelper _db;
        private readonly ILogger<ManagementController> _logger;
        private readonly IConfiguration _configuration;

        public ManagementController(IConfiguration configuration, ILogger<ManagementController> logger)
        {
            _db = new DatabaseHelper(configuration);
            _logger = logger;
            _configuration = configuration;
        }

        // GET: /Management/Index - redirect to home (removed Management landing page)
        public IActionResult Index()
        {
            return RedirectToAction("Index", "Home");
        }

        // GET: /Management/NhanSu - Quản lý nhân sự
        public IActionResult NhanSu()
        {
            var vaiTro = HttpContext.Session.GetString("VaiTro");
            if (!vaiTro?.Equals("Quản lý", StringComparison.OrdinalIgnoreCase) == true)
            {
                return RedirectToAction("Index", "HomeStaff");
            }

            // Quản lý can access all facilities
            _logger.LogInformation("Loading employees for Quản lý - all facilities");
            var employees = new List<NhanVienItem>();

            try
            {
                var query = @"
                    SELECT NV.MaNV, NV.HoTen, NV.SDT, NV.CMND_CCCD, NV.NgaySinh, 
                           NV.DiaChi, NV.LuongCoBan, NV.ChucVu,
                           TK.VaiTro, TK.TenDangNhap, CS.TenCS,
                           (SELECT COUNT(*) 
                            FROM THAMGIACATRUC TG 
                            JOIN CATRUC CT ON TG.MaCaTruc = CT.MaCaTruc
                            WHERE TG.MaNV = NV.MaNV 
                            AND MONTH(CT.NgayTruc) = MONTH(GETDATE())
                            AND YEAR(CT.NgayTruc) = YEAR(GETDATE())) AS SoCaTruc
                    FROM NHANVIEN NV
                    LEFT JOIN TAIKHOAN TK ON NV.MaTK = TK.MaTK
                    LEFT JOIN COSO CS ON NV.MaCS = CS.MaCS
                    ORDER BY NV.HoTen
                ";

                var parameters = new SqlParameter[0];
                
                var dt = _db.ExecuteQuery(query, parameters);
                _logger.LogInformation($"Query returned {dt.Rows.Count} rows");
                foreach (DataRow row in dt.Rows)
                {
                    employees.Add(new NhanVienItem
                    {
                        MaNV = row["MaNV"]?.ToString() ?? "",
                        HoTen = row["HoTen"]?.ToString() ?? "",
                        SDT = row["SDT"]?.ToString() ?? "",
                        CCCD = row["CMND_CCCD"]?.ToString() ?? "",
                        NgaySinh = row["NgaySinh"] != DBNull.Value ? Convert.ToDateTime(row["NgaySinh"]) : null,
                        DiaChi = row["DiaChi"]?.ToString(),
                        ChucVu = row["ChucVu"]?.ToString() ?? "",
                        LuongCoBan = row["LuongCoBan"] != DBNull.Value ? Convert.ToDecimal(row["LuongCoBan"]) : 0,
                        Luong = row["LuongCoBan"] != DBNull.Value ? Convert.ToDecimal(row["LuongCoBan"]) : 0,
                        TrangThai = "Đang làm", // Default value since not in DB
                        VaiTro = row["VaiTro"]?.ToString() ?? "",
                        TenDangNhap = row["TenDangNhap"]?.ToString() ?? "",
                        TenCoSo = row["TenCS"]?.ToString() ?? "",
                        SoCaTruc = row["SoCaTruc"] != DBNull.Value ? Convert.ToInt32(row["SoCaTruc"]) : 0
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading employee list");
                TempData["Error"] = "Lỗi tải danh sách nhân viên: " + ex.Message;
            }

            return View(employees);
        }

        // GET: /Management/PhanCongCaTruc
        public IActionResult PhanCongCaTruc()
        {
            var vaiTro = HttpContext.Session.GetString("VaiTro");
            if (!vaiTro?.Equals("Quản lý", StringComparison.OrdinalIgnoreCase) == true)
            {
                return RedirectToAction("Index", "HomeStaff");
            }

            // Load staff list for the create-shift modal
            try
            {
                var maCS = HttpContext.Session.GetString("MaCS");
                var sql = @"SELECT MaNV, HoTen, ChucVu FROM NHANVIEN " + (string.IsNullOrEmpty(maCS) ? "" : "WHERE MaCS = @MaCS") + " ORDER BY HoTen";
                var parameters = string.IsNullOrEmpty(maCS) ? new System.Data.SqlClient.SqlParameter[0] : new[] { new System.Data.SqlClient.SqlParameter("@MaCS", maCS) };
                var dt = _db.ExecuteQuery(sql, parameters);
                var staffList = new List<dynamic>();
                foreach (System.Data.DataRow row in dt.Rows)
                {
                    staffList.Add(new { MaNV = row["MaNV"].ToString(), HoTen = row["HoTen"].ToString(), ChucVu = row["ChucVu"].ToString() });
                }
                ViewBag.StaffList = staffList;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading staff for PhanCongCaTruc");
                ViewBag.StaffList = new List<dynamic>();
            }

            return View();
        }

        // POST: /Management/AssignNhanVien
        [HttpPost]
        public IActionResult AssignNhanVien(string maCaTruc, string maNV)
        {
            try
            {
                var sql = @"INSERT INTO THAMGIACATRUC (MaCaTruc, MaNV)
                            VALUES (@MaCa, @MaNV)";
                _db.ExecuteNonQuery(sql,
                    new System.Data.SqlClient.SqlParameter("@MaCa", maCaTruc),
                    new System.Data.SqlClient.SqlParameter("@MaNV", maNV));
                return Json(new { success = true, message = "Phân công thành công" });
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "AssignNhanVien failed");
                return Json(new { success = false, message = ex.Message });
            }
        }

        // GET: /Management/GetLichCaTruc?view=week&from=2026-01-01
        [HttpGet]
        public IActionResult GetLichCaTruc(string view = "week", DateTime? from = null)
        {
            var maCS = HttpContext.Session.GetString("MaCS");
            try
            {
                var start = from ?? DateTime.Today;
                _logger.LogInformation($"GetLichCaTruc called with view={view}, from={from}");
                if (view == "week") start = start.AddDays(-(int)start.DayOfWeek + 1);
                var end = view == "month" ? new DateTime(start.Year, start.Month, DateTime.DaysInMonth(start.Year, start.Month)) : start.AddDays(view == "week" ? 6 : 0);
                _logger.LogInformation($"Computed range start={start:yyyy-MM-dd}, end={end:yyyy-MM-dd}");
                var query = @"SELECT C.MaCaTruc,
                                     CONVERT(VARCHAR(5), C.GioBatDau, 108) + ' - ' + CONVERT(VARCHAR(5), C.GioKetThuc, 108) AS TenCa,
                                     C.NgayTruc, C.GioBatDau, C.GioKetThuc, ISNULL(C.PhuCap,0) AS PhuCap,
                                     TG.MaNV, NV.HoTen, NV.ChucVu
                              FROM CATRUC C
                              LEFT JOIN THAMGIACATRUC TG ON TG.MaCaTruc = C.MaCaTruc
                              LEFT JOIN NHANVIEN NV ON NV.MaNV = TG.MaNV
                              WHERE CAST(C.NgayTruc AS DATE) BETWEEN @Start AND @End
                              ORDER BY C.NgayTruc, C.GioBatDau";

                var dt = _db.ExecuteQuery(query,
                    new System.Data.SqlClient.SqlParameter("@Start", start.Date),
                    new System.Data.SqlClient.SqlParameter("@End", end.Date));

                _logger.LogInformation($"GetLichCaTruc: query returned {dt.Rows.Count} rows");
                if (dt.Rows.Count > 0)
                {
                    var cols = string.Join(", ", dt.Columns.Cast<System.Data.DataColumn>().Select(c => c.ColumnName));
                    _logger.LogInformation($"Columns: {cols}");
                }

                var list = new List<dynamic>();
                foreach (System.Data.DataRow row in dt.Rows)
                {
                    try
                    {
                        var item = new
                        {
                            MaCaTruc = row["MaCaTruc"]?.ToString() ?? "",
                            TenCa = row["TenCa"]?.ToString() ?? "",
                            NgayTruc = row["NgayTruc"] != DBNull.Value ? Convert.ToDateTime(row["NgayTruc"]) : DateTime.MinValue,
                            GioBatDau = row["GioBatDau"]?.ToString() ?? "",
                            GioKetThuc = row["GioKetThuc"]?.ToString() ?? "",
                            PhuCap = row["PhuCap"] != DBNull.Value ? Convert.ToDecimal(row["PhuCap"]) : 0,
                            MaNV = row["MaNV"]?.ToString(),
                            HoTen = row["HoTen"]?.ToString(),
                            ChucVu = row["ChucVu"]?.ToString()
                        };
                        list.Add(item);
                        _logger.LogInformation($"Added shift: MaCaTruc={item.MaCaTruc}, TenCa={item.TenCa}, NgayTruc={item.NgayTruc:yyyy-MM-dd}, HoTen={item.HoTen}");
                    }
                    catch (System.Exception rowEx)
                    {
                        _logger.LogError(rowEx, $"Error processing row in GetLichCaTruc");
                    }
                }

                _logger.LogInformation($"Returning {list.Count} shifts in JSON response");
                return Json(new { success = true, data = list });
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "GetLichCaTruc failed");
                return Json(new { success = false, message = ex.Message });
            }
        }

        // POST: /Management/SuaCaTruc
        [HttpPost]
        public IActionResult SuaCaTruc(string maCaTruc, DateTime ngayTruc, string tenCa, string gioBatDau, string gioKetThuc, string? ghiChu)
        {
            try
            {
                // CATRUC has columns: MaNV, NgayTruc, GioBatDau, GioKetThuc, PhuCap
                var sql = @"UPDATE CATRUC SET NgayTruc = @Ngay, GioBatDau = @GioBD, GioKetThuc = @GioKT WHERE MaCaTruc = @Ma";
                _db.ExecuteNonQuery(sql,
                    new System.Data.SqlClient.SqlParameter("@Ngay", ngayTruc),
                    new System.Data.SqlClient.SqlParameter("@GioBD", gioBatDau),
                    new System.Data.SqlClient.SqlParameter("@GioKT", gioKetThuc),
                    new System.Data.SqlClient.SqlParameter("@Ma", maCaTruc));
                return Json(new { success = true, message = "Cập nhật ca trực thành công" });
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "SuaCaTruc failed");
                return Json(new { success = false, message = ex.Message });
            }
        }

        // POST: /Management/XoaCaTruc
        [HttpPost]
        public IActionResult XoaCaTruc(string maCaTruc)
        {
            try
            {
                var sqlDelTG = "DELETE FROM THAMGIACATRUC WHERE MaCaTruc = @Ma";
                var sqlDel = "DELETE FROM CATRUC WHERE MaCaTruc = @Ma";
                _db.ExecuteNonQuery(sqlDelTG, new System.Data.SqlClient.SqlParameter("@Ma", maCaTruc));
                _db.ExecuteNonQuery(sqlDel, new System.Data.SqlClient.SqlParameter("@Ma", maCaTruc));
                return Json(new { success = true, message = "Xóa ca trực thành công" });
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "XoaCaTruc failed");
                return Json(new { success = false, message = ex.Message });
            }
        }

        // GET: /Management/GetChiTietCa?maCaTruc=CT...
        [HttpGet]
        public IActionResult GetChiTietCa(string maCaTruc)
        {
            try
            {
                var query = @"SELECT C.MaCaTruc,
                                     CONVERT(VARCHAR(5), C.GioBatDau, 108) + ' - ' + CONVERT(VARCHAR(5), C.GioKetThuc, 108) AS TenCa,
                                     C.NgayTruc, C.GioBatDau, C.GioKetThuc, ISNULL(C.PhuCap,0) AS PhuCap,
                                     TG.MaNV, NV.HoTen
                              FROM CATRUC C
                              LEFT JOIN THAMGIACATRUC TG ON TG.MaCaTruc = C.MaCaTruc
                              LEFT JOIN NHANVIEN NV ON NV.MaNV = TG.MaNV
                              WHERE C.MaCaTruc = @Ma";

                var dt = _db.ExecuteQuery(query, new System.Data.SqlClient.SqlParameter("@Ma", maCaTruc));
                var list = new List<dynamic>();
                foreach (System.Data.DataRow row in dt.Rows)
                {
                    list.Add(new
                    {
                        MaCaTruc = row["MaCaTruc"].ToString(),
                        TenCa = row["TenCa"].ToString(),
                        NgayTruc = Convert.ToDateTime(row["NgayTruc"]),
                        GioBatDau = row["GioBatDau"].ToString(),
                        GioKetThuc = row["GioKetThuc"].ToString(),
                        GhiChu = row["GhiChu"].ToString(),
                        MaNV = row["MaNV"]?.ToString(),
                        HoTen = row["HoTen"]?.ToString()
                    });
                }

                return Json(new { success = true, data = list });
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "GetChiTietCa failed");
                return Json(new { success = false, message = ex.Message });
            }
        }

        // POST: /Management/DuyetDon
        [HttpPost]
        public IActionResult DuyetDon(string maDon, string? nguoiThayThe)
        {
            try
            {
                _logger.LogInformation($"DuyetDon called: maDon={maDon}, nguoiThayThe={nguoiThayThe}");
                
                // Get leave request details
                var queryLeave = "SELECT MaNV, CaNghi, NguoiThayThe FROM DONNGHIPHEP WHERE MaDon = @MaDon";
                var dtLeave = _db.ExecuteQuery(queryLeave, new SqlParameter("@MaDon", maDon));
                
                if (dtLeave.Rows.Count == 0)
                {
                    return Json(new { success = false, message = "Không tìm thấy đơn nghỉ phép" });
                }
                
                var maNVNghi = dtLeave.Rows[0]["MaNV"].ToString();
                var caNghi = dtLeave.Rows[0]["CaNghi"] != DBNull.Value ? dtLeave.Rows[0]["CaNghi"].ToString() : null;
                var nguoiThayTheDB = dtLeave.Rows[0]["NguoiThayThe"]?.ToString();
                
                // Use replacement from parameter or from database
                var finalReplacement = !string.IsNullOrEmpty(nguoiThayThe) ? nguoiThayThe : nguoiThayTheDB;
                
                // Update leave request status and NgayDuyet
                var updateSql = "UPDATE DONNGHIPHEP SET TrangThai = N'Đã duyệt', NgayDuyet = GETDATE() WHERE MaDon = @Ma";
                _db.ExecuteNonQuery(updateSql, new SqlParameter("@Ma", maDon));
                
                // If there's a shift assigned and a replacement staff, update THAMGIACATRUC
                if (!string.IsNullOrEmpty(caNghi) && !string.IsNullOrEmpty(finalReplacement))
                {
                    _logger.LogInformation($"Updating THAMGIACATRUC: Replace {maNVNghi} with {finalReplacement} in shift {caNghi}");
                    
                    // Check if the original staff is assigned to this shift
                    var checkShift = "SELECT COUNT(*) FROM THAMGIACATRUC WHERE MaCaTruc = @MaCaTruc AND MaNV = @MaNV";
                    var dtCheck = _db.ExecuteQuery(checkShift, 
                        new SqlParameter("@MaCaTruc", long.Parse(caNghi)),
                        new SqlParameter("@MaNV", maNVNghi));
                    
                    if (dtCheck.Rows.Count > 0 && Convert.ToInt32(dtCheck.Rows[0][0]) > 0)
                    {
                        // Delete old assignment
                        var deleteShift = "DELETE FROM THAMGIACATRUC WHERE MaCaTruc = @MaCaTruc AND MaNV = @MaNV";
                        _db.ExecuteNonQuery(deleteShift,
                            new SqlParameter("@MaCaTruc", long.Parse(caNghi)),
                            new SqlParameter("@MaNV", maNVNghi));
                        
                        // Add new assignment for replacement staff
                        var insertShift = "INSERT INTO THAMGIACATRUC (MaCaTruc, MaNV) VALUES (@MaCaTruc, @MaNV)";
                        _db.ExecuteNonQuery(insertShift,
                            new SqlParameter("@MaCaTruc", long.Parse(caNghi)),
                            new SqlParameter("@MaNV", finalReplacement));
                        
                        _logger.LogInformation($"Successfully replaced {maNVNghi} with {finalReplacement} in shift {caNghi}");
                    }
                    else
                    {
                        _logger.LogWarning($"Staff {maNVNghi} is not assigned to shift {caNghi}, skipping THAMGIACATRUC update");
                    }
                }
                
                return Json(new { success = true, message = "Đã phê duyệt đơn nghỉ phép" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DuyetDon failed");
                return Json(new { success = false, message = "Lỗi phê duyệt: " + ex.Message });
            }
        }

        // POST: /Management/TuChoiDon
        [HttpPost]
        public IActionResult TuChoiDon(string maDon, string? lyDo)
        {
            try
            {
                var sql = "UPDATE DONNGHIPHEP SET TrangThai = N'Từ chối', LyDoTuChoi = @LyDo WHERE MaDon = @Ma";
                _db.ExecuteNonQuery(sql, new System.Data.SqlClient.SqlParameter("@LyDo", lyDo ?? ""), new System.Data.SqlClient.SqlParameter("@Ma", maDon));
                return Json(new { success = true, message = "Đã từ chối đơn" });
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "TuChoiDon failed");
                return Json(new { success = false, message = ex.Message });
            }
        }

        // GET: /Management/PheDuyetNghiPhep
        public IActionResult PheDuyetNghiPhep()
        {
            var vaiTro = HttpContext.Session.GetString("VaiTro");
            if (!vaiTro?.Equals("Quản lý", StringComparison.OrdinalIgnoreCase) == true)
            {
                return RedirectToAction("Index", "HomeStaff");
            }

            // Quản lý can access leave requests from all facilities
            var leaveRequests = new List<dynamic>();

            try
            {
                var query = @"
                    SELECT DNP.MaDon, DNP.MaNV, NV.HoTen, DNP.CaNghi,
                           CONVERT(VARCHAR(5), ISNULL(CT.GioBatDau, '00:00'), 108) + ' - ' + CONVERT(VARCHAR(5), ISNULL(CT.GioKetThuc, '00:00'), 108) AS LoaiNghi,
                           DNP.NgayNghi AS TuNgay, DNP.NgayNghi AS DenNgay, DNP.LyDo, DNP.TrangThai,
                           COALESCE(DNP.NgayDuyet, DNP.NgayNghi) AS NgayGui, DNP.NguoiThayThe, NV2.HoTen AS TenNguoiThayThe,
                           CS.TenCS
                    FROM DONNGHIPHEP DNP
                    JOIN NHANVIEN NV ON DNP.MaNV = NV.MaNV
                    LEFT JOIN NHANVIEN NV2 ON DNP.NguoiThayThe = NV2.MaNV
                    LEFT JOIN CATRUC CT ON DNP.CaNghi = CT.MaCaTruc
                    LEFT JOIN COSO CS ON NV.MaCS = CS.MaCS
                    ORDER BY 
                        CASE DNP.TrangThai 
                            WHEN N'Chờ duyệt' THEN 1 
                            WHEN N'Đã duyệt' THEN 2 
                            ELSE 3 
                        END,
                        COALESCE(DNP.NgayDuyet, DNP.NgayNghi) DESC
                ";

                var parameters = new SqlParameter[0];
                
                var dt = _db.ExecuteQuery(query, parameters);
                _logger.LogInformation($"PheDuyetNghiPhep: Found {dt.Rows.Count} leave requests");
                
                // Log column names for debugging
                if (dt.Columns.Count > 0)
                {
                    var columnNames = string.Join(", ", dt.Columns.Cast<DataColumn>().Select(c => c.ColumnName));
                    _logger.LogInformation($"Columns: {columnNames}");
                }
                
                foreach (DataRow row in dt.Rows)
                {
                    leaveRequests.Add(new
                    {
                        MaDon = row["MaDon"]?.ToString() ?? "",
                        MaNV = row["MaNV"]?.ToString() ?? "",
                        HoTen = row["HoTen"]?.ToString() ?? "",
                        CaNghi = row["CaNghi"] != DBNull.Value ? Convert.ToInt64(row["CaNghi"]) : (long?)null,
                        LoaiNghi = row["LoaiNghi"]?.ToString() ?? "",
                        TuNgay = row["TuNgay"] != DBNull.Value ? Convert.ToDateTime(row["TuNgay"]) : DateTime.Now,
                        DenNgay = row["DenNgay"] != DBNull.Value ? Convert.ToDateTime(row["DenNgay"]) : DateTime.Now,
                        LyDo = row["LyDo"]?.ToString() ?? "",
                        TrangThai = row["TrangThai"]?.ToString() ?? "Chờ duyệt",
                        NgayGui = row["NgayGui"] != DBNull.Value ? Convert.ToDateTime(row["NgayGui"]) : DateTime.Now,
                        NguoiThayThe = row["NguoiThayThe"]?.ToString(),
                        TenNguoiThayThe = row["TenNguoiThayThe"]?.ToString(),
                        TenCoSo = row["TenCS"]?.ToString()
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading leave requests");
                TempData["Error"] = "Lỗi tải danh sách nghỉ phép: " + ex.Message;
            }

            ViewBag.LeaveRequests = leaveRequests;
            return View();
        }

        // GET: /Management/BaoCaoThongKe
        public IActionResult BaoCaoThongKe()
        {
            var vaiTro = HttpContext.Session.GetString("VaiTro");
            if (!vaiTro?.Equals("Quản lý", StringComparison.OrdinalIgnoreCase) == true)
            {
                return RedirectToAction("Index", "HomeStaff");
            }

            return View();
        }

        // GET: /Management/ThemNhanVien
        public IActionResult ThemNhanVien()
        {
            var vaiTro = HttpContext.Session.GetString("VaiTro");
            if (!vaiTro?.Equals("Quản lý", StringComparison.OrdinalIgnoreCase) == true)
            {
                return RedirectToAction("Index", "HomeStaff");
            }

            // Load danh sách cơ sở
            try
            {
                var query = "SELECT MaCS, TenCS FROM COSO ORDER BY TenCS";
                var dt = _db.ExecuteQuery(query);
                var coSoList = new List<dynamic>();
                foreach (DataRow row in dt.Rows)
                {
                    coSoList.Add(new
                    {
                        MaCS = row["MaCS"].ToString(),
                        TenCoSo = row["TenCS"].ToString()
                    });
                }
                ViewBag.CoSoList = coSoList;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading facilities");
            }

            return View();
        }

        // POST: /Management/ThemNhanVien
        [HttpPost]
        public IActionResult ThemNhanVien(string hoTen, string sdt, string cccd, 
            DateTime? ngaySinh, string? diaChi, string chucVu, string maCS, 
            decimal luongCoBan, string tenDangNhap, string matKhau)
        {
            var currentVaiTro = HttpContext.Session.GetString("VaiTro");
            if (!currentVaiTro?.Equals("Quản lý", StringComparison.OrdinalIgnoreCase) == true)
            {
                if (Request.ContentType?.Contains("application/json") == true)
                    return Json(new { success = false, message = "Không có quyền!" });
                return RedirectToAction("Index", "HomeStaff");
            }

            try
            {
                // Map ChucVu to VaiTro
                string vaiTro = chucVu switch
                {
                    "Quản lý" => "Quản lý",
                    "Lễ tân" => "Lễ tân",
                    "Kỹ thuật" => "Nhân viên kỹ thuật",
                    "Thu ngân" => "Nhân viên kỹ thuật",
                    "HLV" => "Nhân viên kỹ thuật",
                    _ => "Nhân viên kỹ thuật"
                };

                // Generate MaNV
                var maxMaNV = _db.ExecuteScalar<string>("SELECT MAX(MaNV) FROM NHANVIEN") ?? "NV00000";
                var numPart = int.Parse(maxMaNV.Substring(2)) + 1;
                var maNV = "NV" + numPart.ToString("D5");

                // Generate MaTK
                var maxMaTK = _db.ExecuteScalar<string>("SELECT MAX(MaTK) FROM TAIKHOAN") ?? "TK00000";
                var tkNumPart = int.Parse(maxMaTK.Substring(2)) + 1;
                var maTK = "TK" + tkNumPart.ToString("D5");

                // Insert TAIKHOAN first
                var insertTK = @"
                    INSERT INTO TAIKHOAN (MaTK, TenDangNhap, MatKhau, VaiTro)
                    VALUES (@MaTK, @TenDangNhap, @MatKhau, @VaiTro)
                ";
                _db.ExecuteNonQuery(insertTK,
                    new SqlParameter("@MaTK", maTK),
                    new SqlParameter("@TenDangNhap", tenDangNhap),
                    new SqlParameter("@MatKhau", matKhau),
                    new SqlParameter("@VaiTro", vaiTro)
                );

                // Insert NHANVIEN
                var insertNV = @"
                    INSERT INTO NHANVIEN (MaNV, HoTen, SDT, CMND_CCCD, NgaySinh, DiaChi, ChucVu, MaCS, LuongCoBan, MaTK)
                    VALUES (@MaNV, @HoTen, @SDT, @CCCD, @NgaySinh, @DiaChi, @ChucVu, @MaCS, @LuongCoBan, @MaTK)
                ";
                _db.ExecuteNonQuery(insertNV,
                    new SqlParameter("@MaNV", maNV),
                    new SqlParameter("@HoTen", hoTen),
                    new SqlParameter("@SDT", sdt),
                    new SqlParameter("@CCCD", cccd),
                    new SqlParameter("@NgaySinh", (object?)ngaySinh ?? DBNull.Value),
                    new SqlParameter("@DiaChi", (object?)diaChi ?? DBNull.Value),
                    new SqlParameter("@ChucVu", chucVu),
                    new SqlParameter("@MaCS", maCS),
                    new SqlParameter("@LuongCoBan", luongCoBan),
                    new SqlParameter("@MaTK", maTK)
                );

                // Always return JSON for AJAX requests
                return Json(new { success = true, message = "Thêm nhân viên thành công!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding employee");
                return Json(new { success = false, message = ex.Message });
            }
        }

        // GET: /Management/SuaNhanVien
        public IActionResult SuaNhanVien(string id)
        {
            var vaiTro = HttpContext.Session.GetString("VaiTro");
            if (!vaiTro?.Equals("Quản lý", StringComparison.OrdinalIgnoreCase) == true)
            {
                return RedirectToAction("Index", "HomeStaff");
            }

            try
            {
                // Load thông tin nhân viên
                var query = @"
                    SELECT NV.*, TK.TenDangNhap, TK.VaiTro
                    FROM NHANVIEN NV
                    LEFT JOIN TAIKHOAN TK ON NV.MaTK = TK.MaTK
                    WHERE NV.MaNV = @MaNV
                ";
                var dt = _db.ExecuteQuery(query, new SqlParameter("@MaNV", id));
                if (dt.Rows.Count == 0)
                {
                    TempData["Error"] = "Không tìm thấy nhân viên!";
                    return RedirectToAction("NhanSu");
                }

                var row = dt.Rows[0];
                var employee = new NhanVienItem
                {
                    MaNV = row["MaNV"].ToString() ?? "",
                    HoTen = row["HoTen"].ToString() ?? "",
                    SDT = row["SDT"].ToString() ?? "",
                    CCCD = row["CMND_CCCD"].ToString() ?? "",
                    NgaySinh = row["NgaySinh"] != DBNull.Value ? Convert.ToDateTime(row["NgaySinh"]) : null,
                    DiaChi = row["DiaChi"]?.ToString(),
                    ChucVu = row["ChucVu"].ToString() ?? "",
                    MaCS = row["MaCS"].ToString() ?? "",
                    LuongCoBan = row["LuongCoBan"] != DBNull.Value ? Convert.ToDecimal(row["LuongCoBan"]) : 0,
                    Luong = row["LuongCoBan"] != DBNull.Value ? Convert.ToDecimal(row["LuongCoBan"]) : 0,
                    TenDangNhap = row["TenDangNhap"]?.ToString() ?? "",
                    VaiTro = row["VaiTro"]?.ToString() ?? "",
                    TrangThai = "Đang làm"
                };

                ViewBag.Employee = employee;

                // Load danh sách cơ sở
                var csQuery = "SELECT MaCS, TenCS FROM COSO ORDER BY TenCS";
                var csDt = _db.ExecuteQuery(csQuery);
                var coSoList = new List<dynamic>();
                foreach (DataRow csRow in csDt.Rows)
                {
                    coSoList.Add(new
                    {
                        MaCS = csRow["MaCS"].ToString(),
                        TenCoSo = csRow["TenCS"].ToString()
                    });
                }
                ViewBag.CoSoList = coSoList;

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading employee for edit");
                TempData["Error"] = "Lỗi tải thông tin nhân viên: " + ex.Message;
                return RedirectToAction("NhanSu");
            }
        }

        // POST: /Management/SuaNhanVien
        [HttpPost]
        public IActionResult SuaNhanVien(string maNV, string hoTen, string sdt, string cccd, 
            DateTime? ngaySinh, string? diaChi, string chucVu, string maCS, 
            decimal luongCoBan, string? matKhau)
        {
            var currentVaiTro = HttpContext.Session.GetString("VaiTro");
            if (!currentVaiTro?.Equals("Quản lý", StringComparison.OrdinalIgnoreCase) == true)
            {
                if (Request.ContentType?.Contains("application/json") == true)
                    return Json(new { success = false, message = "Không có quyền!" });
                return RedirectToAction("Index", "HomeStaff");
            }

            try
            {
                // Map ChucVu to VaiTro
                string vaiTro = chucVu switch
                {
                    "Quản lý" => "Quản lý",
                    "Lễ tân" => "Lễ tân",
                    "Kỹ thuật" => "Nhân viên kỹ thuật",
                    "Thu ngân" => "Nhân viên kỹ thuật",
                    "HLV" => "Nhân viên kỹ thuật",
                    _ => "Nhân viên kỹ thuật"
                };
                // Update NHANVIEN
                var updateNV = @"
                    UPDATE NHANVIEN 
                    SET HoTen = @HoTen, SDT = @SDT, CMND_CCCD = @CCCD, 
                        NgaySinh = @NgaySinh, DiaChi = @DiaChi, ChucVu = @ChucVu, 
                        MaCS = @MaCS, LuongCoBan = @LuongCoBan
                    WHERE MaNV = @MaNV
                ";
                _db.ExecuteNonQuery(updateNV,
                    new SqlParameter("@MaNV", maNV),
                    new SqlParameter("@HoTen", hoTen),
                    new SqlParameter("@SDT", sdt),
                    new SqlParameter("@CCCD", cccd),
                    new SqlParameter("@NgaySinh", (object?)ngaySinh ?? DBNull.Value),
                    new SqlParameter("@DiaChi", (object?)diaChi ?? DBNull.Value),
                    new SqlParameter("@ChucVu", chucVu),
                    new SqlParameter("@MaCS", maCS),
                    new SqlParameter("@LuongCoBan", luongCoBan)
                );

                // Get MaTK from NHANVIEN
                var getMaTK = "SELECT MaTK FROM NHANVIEN WHERE MaNV = @MaNV";
                var maTK = _db.ExecuteScalar<string>(getMaTK, new SqlParameter("@MaNV", maNV));

                // Update TAIKHOAN
                if (!string.IsNullOrEmpty(matKhau))
                {
                    var updateTKWithPassword = @"
                        UPDATE TAIKHOAN 
                        SET MatKhau = @MatKhau, VaiTro = @VaiTro
                        WHERE MaTK = @MaTK
                    ";
                    _db.ExecuteNonQuery(updateTKWithPassword,
                        new SqlParameter("@MatKhau", matKhau),
                        new SqlParameter("@VaiTro", vaiTro),
                        new SqlParameter("@MaTK", maTK)
                    );
                }
                else
                {
                    var updateTK = @"
                        UPDATE TAIKHOAN 
                        SET VaiTro = @VaiTro
                        WHERE MaTK = @MaTK
                    ";
                    _db.ExecuteNonQuery(updateTK,
                        new SqlParameter("@VaiTro", vaiTro),
                        new SqlParameter("@MaTK", maTK)
                    );
                }

                // Always return JSON for AJAX requests
                return Json(new { success = true, message = "Cập nhật nhân viên thành công!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating employee");
                return Json(new { success = false, message = ex.Message });
            }
        }

        // POST: /Management/XoaNhanVien
        [HttpPost]
        public IActionResult XoaNhanVien(string id)
        {
            var vaiTro = HttpContext.Session.GetString("VaiTro");
            if (!vaiTro?.Equals("Quản lý", StringComparison.OrdinalIgnoreCase) == true)
            {
                return Json(new { success = false, message = "Không có quyền!" });
            }

            try
            {
                // Note: Database doesn't have TrangThai column for soft delete
                // For now, we'll just return a message
                // TODO: Add TrangThai column to NHANVIEN and TAIKHOAN tables
                
                return Json(new { success = false, message = "Chức năng xóa nhân viên tạm thời chưa khả dụng. Vui lòng liên hệ quản trị viên." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting employee");
                return Json(new { success = false, message = "Lỗi: " + ex.Message });
            }
        }
        [HttpGet]
        public IActionResult GetCoSoList()
        {
            try
            {
                var query = "SELECT MaCS, TenCS FROM COSO ORDER BY TenCS";
                var dt = _db.ExecuteQuery(query);
                var coSoList = new List<object>();
                
                foreach (DataRow row in dt.Rows)
                {
                    coSoList.Add(new
                    {
                        maCS = row["MaCS"].ToString(),
                        tenCS = row["TenCS"].ToString()
                    });
                }
                
                return Json(coSoList);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading co so list");
                return Json(new List<object>());
            }
        }

        [HttpGet]
        public IActionResult GetEmployeeData(string id)
        {
            var vaiTro = HttpContext.Session.GetString("VaiTro");
            if (!vaiTro?.Equals("Quản lý", StringComparison.OrdinalIgnoreCase) == true)
            {
                return Json(new { success = false, message = "Không có quyền!" });
            }

            try
            {
                var query = @"
                    SELECT NV.MaNV, NV.HoTen, NV.SDT, NV.CMND_CCCD, NV.NgaySinh, 
                           NV.DiaChi, NV.ChucVu, NV.MaCS, NV.LuongCoBan,
                           TK.VaiTro, TK.TenDangNhap
                    FROM NHANVIEN NV
                    LEFT JOIN TAIKHOAN TK ON NV.MaTK = TK.MaTK
                    WHERE NV.MaNV = @MaNV
                ";

                var dt = _db.ExecuteQuery(query, new SqlParameter("@MaNV", id));
                if (dt.Rows.Count == 0)
                {
                    return Json(new { success = false, message = "Không tìm thấy nhân viên!" });
                }

                var row = dt.Rows[0];
                return Json(new
                {
                    success = true,
                    maNV = row["MaNV"].ToString(),
                    hoTen = row["HoTen"].ToString(),
                    sdt = row["SDT"].ToString(),
                    cccd = row["CMND_CCCD"].ToString(),
                    ngaySinh = row["NgaySinh"] != DBNull.Value ? Convert.ToDateTime(row["NgaySinh"]).ToString("yyyy-MM-dd") : null,
                    diaChi = row["DiaChi"]?.ToString(),
                    chucVu = row["ChucVu"].ToString(),
                    maCS = row["MaCS"].ToString(),
                    luongCoBan = row["LuongCoBan"] != DBNull.Value ? Convert.ToDecimal(row["LuongCoBan"]) : 0,
                    vaiTro = row["VaiTro"].ToString(),
                    tenDangNhap = row["TenDangNhap"].ToString()
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading employee data");
                return Json(new { success = false, message = ex.Message });
            }
        }

        // GET: /Management/BaoCaoLoiSan
        public IActionResult BaoCaoLoiSan()
        {
            var vaiTro = HttpContext.Session.GetString("VaiTro");
            if (!vaiTro?.Equals("Quản lý", StringComparison.OrdinalIgnoreCase) == true)
            {
                return RedirectToAction("Index", "HomeStaff");
            }

            return View();
        }

        // GET: /Management/GetBaoCaoLoi - API to get all maintenance reports
        [HttpGet]
        public JsonResult GetBaoCaoLoi()
        {
            try
            {
                // Quản lý can access maintenance reports from all facilities
                var query = @"
                    SELECT P.MaPhieu, P.MaSan, S.MaSan AS TenSan, LS.TenLS, CS.TenCS,
                           P.NgayBatDau, P.NgayKetThucDuKien, P.NgayKetThucThucTe,
                           P.MoTaSuCo, P.ChiPhi, P.TrangThai,
                           P.MaNV, NV.HoTen AS TenNhanVien
                    FROM PHIEUBAOTRI P
                    JOIN SAN S ON P.MaSan = S.MaSan
                    JOIN LOAISAN LS ON S.MaLS = LS.MaLS
                    JOIN COSO CS ON S.MaCS = CS.MaCS
                    LEFT JOIN NHANVIEN NV ON P.MaNV = NV.MaNV
                    ORDER BY 
                        CASE P.TrangThai 
                            WHEN N'Chờ xử lý' THEN 1 
                            WHEN N'Đang xử lý' THEN 2 
                            WHEN N'Hoàn thành' THEN 3 
                            ELSE 4 
                        END,
                        P.NgayBatDau DESC
                ";

                var dt = _db.ExecuteQuery(query);
                var reports = new List<object>();

                foreach (DataRow row in dt.Rows)
                {
                    reports.Add(new
                    {
                        maPhieu = row["MaPhieu"]?.ToString() ?? "",
                        maSan = row["MaSan"]?.ToString() ?? "",
                        tenSan = row["TenSan"]?.ToString() ?? "",
                        tenLoaiSan = row["TenLS"]?.ToString() ?? "",
                        tenCoSo = row["TenCS"]?.ToString() ?? "",
                        ngayBatDau = row["NgayBatDau"] != DBNull.Value ? Convert.ToDateTime(row["NgayBatDau"]).ToString("dd/MM/yyyy") : "",
                        ngayKetThucDuKien = row["NgayKetThucDuKien"] != DBNull.Value ? Convert.ToDateTime(row["NgayKetThucDuKien"]).ToString("dd/MM/yyyy") : "",
                        ngayKetThucThucTe = row["NgayKetThucThucTe"] != DBNull.Value ? Convert.ToDateTime(row["NgayKetThucThucTe"]).ToString("dd/MM/yyyy") : "",
                        moTaSuCo = row["MoTaSuCo"]?.ToString() ?? "",
                        chiPhi = row["ChiPhi"] != DBNull.Value ? Convert.ToDecimal(row["ChiPhi"]) : 0,
                        trangThai = row["TrangThai"]?.ToString() ?? "Chờ xử lý",
                        maNV = row["MaNV"]?.ToString() ?? "",
                        tenNhanVien = row["TenNhanVien"]?.ToString() ?? ""
                    });
                }

                return Json(new { success = true, data = reports });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading maintenance reports");
                return Json(new { success = false, message = ex.Message });
            }
        }

        // POST: /Management/TaoBaoCaoLoi - Create new maintenance report
        [HttpPost]
        public JsonResult TaoBaoCaoLoi([FromBody] TaoBaoCaoLoiRequest request)
        {
            try
            {
                var maNV = HttpContext.Session.GetString("MaUser");
                
                var sql = @"INSERT INTO PHIEUBAOTRI (MaSan, MaNV, NgayBatDau, MoTaSuCo, TrangThai, ChiPhi)
                           VALUES (@MaSan, @MaNV, GETDATE(), @MoTa, N'Chờ xử lý', 0)";
                
                _db.ExecuteNonQuery(sql, 
                    new SqlParameter("@MaSan", request.MaSan),
                    new SqlParameter("@MaNV", string.IsNullOrEmpty(maNV) ? DBNull.Value : (object)maNV),
                    new SqlParameter("@MoTa", request.MoTaSuCo)
                );

                return Json(new { success = true, message = "Tạo báo cáo lỗi thành công!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating maintenance report");
                return Json(new { success = false, message = ex.Message });
            }
        }

        // POST: /Management/HoanThanhBaoCao - Complete maintenance report
        [HttpPost]
        public JsonResult HoanThanhBaoCao([FromBody] HoanThanhBaoCaoRequest request)
        {
            try
            {
                var sql = @"UPDATE PHIEUBAOTRI 
                           SET TrangThai = N'Hoàn thành',
                               ChiPhi = @ChiPhi,
                               NgayKetThucThucTe = GETDATE()
                           WHERE MaPhieu = @MaPhieu";
                
                _db.ExecuteNonQuery(sql, 
                    new SqlParameter("@MaPhieu", request.MaPhieu),
                    new SqlParameter("@ChiPhi", request.ChiPhi)
                );

                return Json(new { success = true, message = "Hoàn thành báo cáo thành công!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error completing maintenance report");
                return Json(new { success = false, message = ex.Message });
            }
        }

        // POST: /Management/CapNhatTrangThai - Update report status
        [HttpPost]
        public JsonResult CapNhatTrangThai([FromBody] CapNhatTrangThaiRequest request)
        {
            try
            {
                var sql = @"UPDATE PHIEUBAOTRI 
                           SET TrangThai = @TrangThai,
                               NgayKetThucDuKien = @NgayKetThuc,
                               ChiPhi = @ChiPhi,
                               MaNV = @MaNV
                           WHERE MaPhieu = @MaPhieu";
                
                _db.ExecuteNonQuery(sql, 
                    new SqlParameter("@MaPhieu", request.MaPhieu),
                    new SqlParameter("@TrangThai", request.TrangThai),
                    new SqlParameter("@NgayKetThuc", string.IsNullOrEmpty(request.NgayKetThucDuKien) ? DBNull.Value : (object)DateTime.Parse(request.NgayKetThucDuKien)),
                    new SqlParameter("@ChiPhi", request.ChiPhi),
                    new SqlParameter("@MaNV", string.IsNullOrEmpty(request.MaNV) ? DBNull.Value : (object)request.MaNV)
                );

                // Nếu trạng thái là "Hoàn thành", cập nhật ngày kết thúc thực tế
                if (request.TrangThai == "Hoàn thành")
                {
                    var updateActualDate = "UPDATE PHIEUBAOTRI SET NgayKetThucThucTe = GETDATE() WHERE MaPhieu = @MaPhieu";
                    _db.ExecuteNonQuery(updateActualDate, new SqlParameter("@MaPhieu", request.MaPhieu));
                }

                return Json(new { success = true, message = "Cập nhật trạng thái thành công!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating report status");
                return Json(new { success = false, message = ex.Message });
            }
        }

        // GET: /Management/GetDanhSachSan - Get list of courts for dropdown
        [HttpGet]
        public JsonResult GetDanhSachSan()
        {
            try
            {
                // Quản lý can access courts from all facilities
                var query = @"
                    SELECT S.MaSan, S.MaSan AS TenSan, LS.TenLS, CS.TenCS
                    FROM SAN S
                    JOIN LOAISAN LS ON S.MaLS = LS.MaLS
                    JOIN COSO CS ON S.MaCS = CS.MaCS
                    ORDER BY CS.TenCS, LS.TenLS, S.MaSan
                ";

                var parameters = new SqlParameter[0];
                
                var dt = _db.ExecuteQuery(query, parameters);
                var courts = new List<object>();

                foreach (DataRow row in dt.Rows)
                {
                    courts.Add(new
                    {
                        maSan = row["MaSan"]?.ToString() ?? "",
                        tenSan = row["TenSan"]?.ToString() ?? "",
                        tenLoaiSan = row["TenLS"]?.ToString() ?? "",
                        tenCoSo = row["TenCS"]?.ToString() ?? "",
                        display = $"{row["TenSan"]} ({row["TenLS"]} - {row["TenCS"]})"
                    });
                }

                return Json(new { success = true, data = courts });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading courts list");
                return Json(new { success = false, message = ex.Message });
            }
        }

        // GET: /Management/GetDanhSachNhanVien - Get list of staff for assignment
        [HttpGet]
        public JsonResult GetDanhSachNhanVienKyThuat()
        {
            try
            {
                // Quản lý can access technical staff from all facilities
                var query = @"
                    SELECT NV.MaNV, NV.HoTen, NV.SDT, NV.ChucVu, CS.TenCS
                    FROM NHANVIEN NV
                    LEFT JOIN COSO CS ON NV.MaCS = CS.MaCS
                    WHERE NV.ChucVu = N'Kỹ thuật'
                    ORDER BY CS.TenCS, NV.HoTen
                ";

                var dt = _db.ExecuteQuery(query);
                var staff = new List<object>();

                foreach (DataRow row in dt.Rows)
                {
                    staff.Add(new
                    {
                        maNV = row["MaNV"]?.ToString() ?? "",
                        hoTen = row["HoTen"]?.ToString() ?? "",
                        sdt = row["SDT"]?.ToString() ?? "",
                        chucVu = row["ChucVu"]?.ToString() ?? "",
                        tenCoSo = row["TenCS"]?.ToString() ?? ""
                    });
                }

                return Json(new { success = true, data = staff });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading staff list");
                return Json(new { success = false, message = ex.Message });
            }
        }

        // GET: /Management/GetDanhSachCoSo - Get list of facilities
        [HttpGet]
        public JsonResult GetDanhSachCoSo()
        {
            try
            {
                var query = @"
                    SELECT MaCS, TenCS
                    FROM COSO
                    ORDER BY TenCS
                ";

                var dt = _db.ExecuteQuery(query);
                var facilities = new List<object>();

                foreach (DataRow row in dt.Rows)
                {
                    facilities.Add(new
                    {
                        MaCS = row["MaCS"]?.ToString() ?? "",
                        TenCS = row["TenCS"]?.ToString() ?? ""
                    });
                }

                return Json(new { success = true, data = facilities });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading facilities list");
                return Json(new { success = false, message = ex.Message });
            }
        }

        // GET: /Management/ThongKe - Trang thống kê báo cáo
        public IActionResult ThongKe()
        {
            var vaiTro = HttpContext.Session.GetString("VaiTro");
            if (!vaiTro?.Equals("Quản lý", StringComparison.OrdinalIgnoreCase) == true)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        // API: Get revenue by facility
        [HttpGet]
        public IActionResult GetDoanhThuTheoCoso(string? tuNgay = null, string? denNgay = null)
        {
            try
            {
                var query = @"
                    SELECT 
                        CS.MaCS,
                        CS.TenCS,
                        ISNULL(SUM(HD.TongTien), 0) AS DoanhThu
                    FROM COSO CS
                    LEFT JOIN SAN S ON CS.MaCS = S.MaCS
                    LEFT JOIN DATSAN DS ON S.MaSan = DS.MaSan
                    LEFT JOIN PHIEUDATSAN PDS ON DS.MaDatSan = PDS.MaDatSan
                    LEFT JOIN HOADON HD ON PDS.MaDatSan = HD.MaPhieu
                    WHERE 1=1";

                var parameters = new List<SqlParameter>();
                
                if (!string.IsNullOrEmpty(tuNgay))
                {
                    query += " AND HD.NgayLap >= @TuNgay";
                    parameters.Add(new SqlParameter("@TuNgay", DateTime.Parse(tuNgay)));
                }
                
                if (!string.IsNullOrEmpty(denNgay))
                {
                    query += " AND HD.NgayLap <= @DenNgay";
                    parameters.Add(new SqlParameter("@DenNgay", DateTime.Parse(denNgay)));
                }

                query += " GROUP BY CS.MaCS, CS.TenCS ORDER BY DoanhThu DESC";

                var dt = _db.ExecuteQuery(query, parameters.ToArray());
                var data = new List<object>();

                foreach (DataRow row in dt.Rows)
                {
                    data.Add(new
                    {
                        MaCS = row["MaCS"].ToString(),
                        TenCS = row["TenCS"].ToString(),
                        DoanhThu = Convert.ToDecimal(row["DoanhThu"])
                    });
                }

                return Json(new { success = true, data });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // API: Get revenue by court type
        [HttpGet]
        public IActionResult GetDoanhThuTheoLoaiSan(string? maCS = null, string? tuNgay = null, string? denNgay = null)
        {
            try
            {
                var query = @"
                    SELECT 
                        LS.TenLS,
                        ISNULL(SUM(HD.TongTien), 0) AS DoanhThu
                    FROM LOAISAN LS
                    LEFT JOIN SAN S ON LS.MaLS = S.MaLS
                    LEFT JOIN DATSAN DS ON S.MaSan = DS.MaSan
                    LEFT JOIN PHIEUDATSAN PDS ON DS.MaDatSan = PDS.MaDatSan
                    LEFT JOIN HOADON HD ON PDS.MaDatSan = HD.MaPhieu
                    WHERE 1=1";

                var parameters = new List<SqlParameter>();

                if (!string.IsNullOrEmpty(maCS))
                {
                    query += " AND S.MaCS = @MaCS";
                    parameters.Add(new SqlParameter("@MaCS", maCS));
                }

                if (!string.IsNullOrEmpty(tuNgay))
                {
                    query += " AND HD.NgayLap >= @TuNgay";
                    parameters.Add(new SqlParameter("@TuNgay", DateTime.Parse(tuNgay)));
                }
                
                if (!string.IsNullOrEmpty(denNgay))
                {
                    query += " AND HD.NgayLap <= @DenNgay";
                    parameters.Add(new SqlParameter("@DenNgay", DateTime.Parse(denNgay)));
                }

                query += " GROUP BY LS.TenLS ORDER BY DoanhThu DESC";

                var dt = _db.ExecuteQuery(query, parameters.ToArray());
                var data = new List<object>();

                foreach (DataRow row in dt.Rows)
                {
                    data.Add(new
                    {
                        TenLS = row["TenLS"].ToString(),
                        DoanhThu = Convert.ToDecimal(row["DoanhThu"])
                    });
                }

                return Json(new { success = true, data });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // API: Get revenue by time period
        [HttpGet]
        public IActionResult GetDoanhThuTheoThoiGian(string kieu = "thang", int nam = 2026)
        {
            try
            {
                string groupBy = kieu.ToLower() switch
                {
                    "thang" => "MONTH(HD.NgayLap)",
                    "quy" => "CEILING(CAST(MONTH(HD.NgayLap) AS FLOAT) / 3)",
                    _ => "YEAR(HD.NgayLap)"
                };

                var query = $@"
                    SELECT 
                        {groupBy} AS KyHan,
                        ISNULL(SUM(HD.TongTien), 0) AS DoanhThu
                    FROM HOADON HD
                    WHERE YEAR(HD.NgayLap) = @Nam
                    GROUP BY {groupBy}
                    ORDER BY KyHan";

                var dt = _db.ExecuteQuery(query, new SqlParameter("@Nam", nam));
                var data = new List<object>();

                foreach (DataRow row in dt.Rows)
                {
                    data.Add(new
                    {
                        KyHan = Convert.ToInt32(row["KyHan"]),
                        DoanhThu = Convert.ToDecimal(row["DoanhThu"])
                    });
                }

                return Json(new { success = true, data });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // API: Get court utilization rate
        [HttpGet]
        public IActionResult GetTyLeSuDungSan(string? maCS = null)
        {
            try
            {
                var query = @"
                    SELECT 
                        S.MaSan,
                        LS.TenLS + ' - ' + CS.TenCS AS TenSan,
                        365 * 16 AS TongGioKhaDung,
                        ISNULL(SUM(DATEDIFF(HOUR, PDS.GioBatDau, PDS.GioKetThuc)), 0) AS TongGioDat
                    FROM SAN S
                    JOIN LOAISAN LS ON S.MaLS = LS.MaLS
                    JOIN COSO CS ON S.MaCS = CS.MaCS
                    LEFT JOIN DATSAN DS ON S.MaSan = DS.MaSan
                    LEFT JOIN PHIEUDATSAN PDS ON DS.MaDatSan = PDS.MaDatSan AND PDS.TrangThai NOT IN (N'Hủy', N'Vắng mặt')
                    WHERE 1=1";

                var parameters = new List<SqlParameter>();

                if (!string.IsNullOrEmpty(maCS))
                {
                    query += " AND S.MaCS = @MaCS";
                    parameters.Add(new SqlParameter("@MaCS", maCS));
                }

                query += " GROUP BY S.MaSan, LS.TenLS, CS.TenCS";

                var dt = _db.ExecuteQuery(query, parameters.ToArray());
                var data = new List<object>();

                foreach (DataRow row in dt.Rows)
                {
                    var tongGioKhaDung = Convert.ToInt32(row["TongGioKhaDung"]);
                    var tongGioDat = Convert.ToInt32(row["TongGioDat"]);
                    var tyLe = tongGioKhaDung > 0 ? (double)tongGioDat / tongGioKhaDung * 100 : 0;

                    data.Add(new
                    {
                        MaSan = row["MaSan"].ToString(),
                        TenSan = row["TenSan"].ToString(),
                        TyLeSuDung = Math.Round(tyLe, 2)
                    });
                }

                return Json(new { success = true, data });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // API: Get booking statistics
        [HttpGet]
        public IActionResult GetThongKeDatSan(string? tuNgay = null, string? denNgay = null)
        {
            try
            {
                var query = @"
                    SELECT 
                        CASE WHEN PDS.KenhDat = N'Website' THEN N'Online' ELSE N'Trực tiếp' END AS HinhThuc,
                        COUNT(*) AS SoLuong
                    FROM PHIEUDATSAN PDS
                    WHERE PDS.TrangThai NOT IN (N'Hủy')";

                var parameters = new List<SqlParameter>();

                if (!string.IsNullOrEmpty(tuNgay))
                {
                    query += " AND PDS.NgayDat >= @TuNgay";
                    parameters.Add(new SqlParameter("@TuNgay", DateTime.Parse(tuNgay)));
                }
                
                if (!string.IsNullOrEmpty(denNgay))
                {
                    query += " AND PDS.NgayDat <= @DenNgay";
                    parameters.Add(new SqlParameter("@DenNgay", DateTime.Parse(denNgay)));
                }

                query += " GROUP BY CASE WHEN PDS.KenhDat = N'Website' THEN N'Online' ELSE N'Trực tiếp' END";

                var dt = _db.ExecuteQuery(query, parameters.ToArray());
                var data = new List<object>();

                foreach (DataRow row in dt.Rows)
                {
                    data.Add(new
                    {
                        HinhThuc = row["HinhThuc"].ToString(),
                        SoLuong = Convert.ToInt32(row["SoLuong"])
                    });
                }

                return Json(new { success = true, data });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // API: Get cancellation statistics
        [HttpGet]
        public IActionResult GetThongKeHuySan(string? tuNgay = null, string? denNgay = null)
        {
            try
            {
                var query = @"
                    SELECT 
                        COUNT(*) AS SoLuongHuy,
                        ISNULL(SUM(HD.TongTien * 0.1), 0) AS TienMat,
                        (SELECT COUNT(*) FROM PHIEUDATSAN WHERE TrangThai = N'Vắng mặt') AS SoNoShow
                    FROM PHIEUDATSAN PDS
                    LEFT JOIN HOADON HD ON PDS.MaDatSan = HD.MaPhieu
                    WHERE PDS.TrangThai = N'Hủy'";

                var parameters = new List<SqlParameter>();

                if (!string.IsNullOrEmpty(tuNgay))
                {
                    query += " AND PDS.NgayDat >= @TuNgay";
                    parameters.Add(new SqlParameter("@TuNgay", DateTime.Parse(tuNgay)));
                }
                
                if (!string.IsNullOrEmpty(denNgay))
                {
                    query += " AND PDS.NgayDat <= @DenNgay";
                    parameters.Add(new SqlParameter("@DenNgay", DateTime.Parse(denNgay)));
                }

                var dt = _db.ExecuteQuery(query, parameters.ToArray());
                var data = new
                {
                    SoLuongHuy = dt.Rows.Count > 0 ? Convert.ToInt32(dt.Rows[0]["SoLuongHuy"]) : 0,
                    TienMat = dt.Rows.Count > 0 ? Convert.ToDecimal(dt.Rows[0]["TienMat"]) : 0,
                    SoNoShow = dt.Rows.Count > 0 ? Convert.ToInt32(dt.Rows[0]["SoNoShow"]) : 0
                };

                return Json(new { success = true, data });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // API: Get service statistics
        [HttpGet]
        public IActionResult GetThongKeDichVu(string? maCS = null, int top = 10)
        {
            try
            {
                var query = $@"
                    SELECT TOP {top}
                        LDV.TenLoai AS TenDV,
                        SUM(CT.SoLuong) AS SoLanSuDung,
                        SUM(CT.ThanhTien) AS DoanhThu
                    FROM DICHVU DV
                    JOIN LOAIDV LDV ON DV.MaLoaiDV = LDV.MaLoaiDV
                    JOIN CT_DICHVUDAT CT ON DV.MaDV = CT.MaDV
                    JOIN PHIEUDATSAN PDS ON CT.MaDatSan = PDS.MaDatSan
                    JOIN DATSAN DS ON PDS.MaDatSan = DS.MaDatSan
                    JOIN SAN S ON DS.MaSan = S.MaSan
                    WHERE 1=1";

                var parameters = new List<SqlParameter>();

                if (!string.IsNullOrEmpty(maCS))
                {
                    query += " AND S.MaCS = @MaCS";
                    parameters.Add(new SqlParameter("@MaCS", maCS));
                }

                query += " GROUP BY LDV.TenLoai ORDER BY SoLanSuDung DESC";

                var dt = _db.ExecuteQuery(query, parameters.ToArray());
                var data = new List<object>();

                foreach (DataRow row in dt.Rows)
                {
                    data.Add(new
                    {
                        TenDV = row["TenDV"].ToString(),
                        SoLanSuDung = Convert.ToInt32(row["SoLanSuDung"]),
                        DoanhThu = Convert.ToDecimal(row["DoanhThu"])
                    });
                }

                return Json(new { success = true, data });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // API: Get staff statistics
        [HttpGet]
        public IActionResult GetThongKeNhanSu(string kieu = "thang", int nam = 2026)
        {
            try
            {
                string groupBy = kieu.ToLower() switch
                {
                    "thang" => "MONTH(CT.NgayTruc)",
                    "quy" => "CEILING(CAST(MONTH(CT.NgayTruc) AS FLOAT) / 3)",
                    _ => "YEAR(CT.NgayTruc)"
                };

                var query = $@"
                    SELECT 
                        {groupBy} AS KyHan,
                        SUM(DATEDIFF(HOUR, CT.GioBatDau, CT.GioKetThuc)) AS TongGioLamViec,
                        COUNT(DISTINCT CT.MaNV) AS SoNhanVien
                    FROM CATRUC CT
                    WHERE YEAR(CT.NgayTruc) = @Nam
                    GROUP BY {groupBy}
                    ORDER BY KyHan";

                var dt = _db.ExecuteQuery(query, new SqlParameter("@Nam", nam));
                var data = new List<object>();

                foreach (DataRow row in dt.Rows)
                {
                    data.Add(new
                    {
                        KyHan = Convert.ToInt32(row["KyHan"]),
                        TongGioLamViec = Convert.ToInt32(row["TongGioLamViec"]),
                        SoNhanVien = Convert.ToInt32(row["SoNhanVien"])
                    });
                }

                return Json(new { success = true, data });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // API: Get staff performance
        [HttpGet]
        public IActionResult GetHieuSuatNhanVien(string? maCS = null, int top = 10)
        {
            try
            {
                var query = $@"
                    SELECT TOP {top}
                        NV.HoTen,
                        COUNT(CT.MaCaTruc) AS SoCaTruc,
                        SUM(DATEDIFF(HOUR, CT.GioBatDau, CT.GioKetThuc)) AS TongGioLamViec
                    FROM NHANVIEN NV
                    JOIN CATRUC CT ON NV.MaNV = CT.MaNV
                    WHERE 1=1";

                var parameters = new List<SqlParameter>();

                if (!string.IsNullOrEmpty(maCS))
                {
                    query += " AND NV.MaCS = @MaCS";
                    parameters.Add(new SqlParameter("@MaCS", maCS));
                }

                query += " GROUP BY NV.HoTen ORDER BY TongGioLamViec DESC";

                var dt = _db.ExecuteQuery(query, parameters.ToArray());
                var data = new List<object>();

                foreach (DataRow row in dt.Rows)
                {
                    data.Add(new
                    {
                        HoTen = row["HoTen"].ToString(),
                        SoCaTruc = Convert.ToInt32(row["SoCaTruc"]),
                        TongGioLamViec = Convert.ToInt32(row["TongGioLamViec"])
                    });
                }

                return Json(new { success = true, data });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // =====================================================
        // QUẢN LÝ GIÁ & ƯU ĐÃI
        // =====================================================

        // GET: /Management/QuanLyGia
        public IActionResult QuanLyGia()
        {
            var vaiTro = HttpContext.Session.GetString("VaiTro");
            if (!vaiTro?.Equals("Quản lý", StringComparison.OrdinalIgnoreCase) == true)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        // ===== KHUNG GIỜ & GIÁ =====

        // GET: /Management/GetDanhSachKhungGio
        [HttpGet]
        public JsonResult GetDanhSachKhungGio(string? maLS = null, string? loaiNgay = null)
        {
            try
            {
                var query = @"
                SELECT KG.MaKG, KG.MaLS, LS.TenLS, KG.GioBatDau, KG.GioKetThuc, 
                       KG.NgayApDung, KG.GiaApDung, KG.LoaiNgay, KG.TenKhungGio,
                       KG.TrangThai, KG.NgayTao
                FROM KHUNGGIO KG
                JOIN LOAISAN LS ON KG.MaLS = LS.MaLS
                WHERE 1=1";

                var parameters = new List<SqlParameter>();

                if (!string.IsNullOrEmpty(maLS))
                {
                    query += " AND KG.MaLS = @MaLS";
                    parameters.Add(new SqlParameter("@MaLS", maLS));
                }

                if (!string.IsNullOrEmpty(loaiNgay))
                {
                    query += " AND KG.LoaiNgay = @LoaiNgay";
                    parameters.Add(new SqlParameter("@LoaiNgay", loaiNgay));
                }

                query += " ORDER BY KG.NgayApDung DESC, LS.TenLS, KG.GioBatDau";

                var dt = _db.ExecuteQuery(query, parameters.ToArray());
                var data = new List<object>();

                foreach (DataRow row in dt.Rows)
                {
                    data.Add(new
                    {
                        maKG = row["MaKG"]?.ToString(),
                        maLS = row["MaLS"]?.ToString(),
                        tenLS = row["TenLS"]?.ToString(),
                        gioBatDau = row["GioBatDau"] != DBNull.Value ? ((TimeSpan)row["GioBatDau"]).ToString(@"hh\:mm") : "",
                        gioKetThuc = row["GioKetThuc"] != DBNull.Value ? ((TimeSpan)row["GioKetThuc"]).ToString(@"hh\:mm") : "",
                        ngayApDung = row["NgayApDung"] != DBNull.Value ? Convert.ToDateTime(row["NgayApDung"]).ToString("dd/MM/yyyy") : "",
                        giaApDung = row["GiaApDung"] != DBNull.Value ? Convert.ToDecimal(row["GiaApDung"]) : 0,
                        loaiNgay = row["LoaiNgay"]?.ToString(),
                        tenKhungGio = row["TenKhungGio"]?.ToString(),
                        trangThai = row["TrangThai"] == DBNull.Value || Convert.ToBoolean(row["TrangThai"]),
                        ngayTao = row["NgayTao"] != DBNull.Value ? Convert.ToDateTime(row["NgayTao"]).ToString("dd/MM/yyyy HH:mm") : ""
                    });
                }

                return Json(new { success = true, data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting time slots");
                return Json(new { success = false, message = ex.Message });
            }
        }

        // POST: /Management/TaoKhungGio
        [HttpPost]
        public JsonResult TaoKhungGio([FromBody] TaoKhungGioRequest request)
        {
            try
            {
                // Generate MaKG
                var maxMaKG = _db.ExecuteScalar<string>("SELECT MAX(MaKG) FROM KHUNGGIO") ?? "KG00000";
                var numPart = int.Parse(maxMaKG.Substring(2)) + 1;
                var maKG = "KG" + numPart.ToString("D5");

                var query = @"
                INSERT INTO KHUNGGIO (MaKG, MaLS, GioBatDau, GioKetThuc, NgayApDung, 
                                      GiaApDung, LoaiNgay, TenKhungGio, TrangThai)
                VALUES (@MaKG, @MaLS, @GioBD, @GioKT, @NgayApDung, @GiaApDung, 
                        @LoaiNgay, @TenKhungGio, @TrangThai)";

                _db.ExecuteNonQuery(query,
                    new SqlParameter("@MaKG", maKG),
                    new SqlParameter("@MaLS", request.MaLS),
                    new SqlParameter("@GioBD", TimeSpan.Parse(request.GioBatDau)),
                    new SqlParameter("@GioKT", TimeSpan.Parse(request.GioKetThuc)),
                    new SqlParameter("@NgayApDung", DateTime.Parse(request.NgayApDung)),
                    new SqlParameter("@GiaApDung", request.GiaApDung),
                    new SqlParameter("@LoaiNgay", request.LoaiNgay),
                    new SqlParameter("@TenKhungGio", request.TenKhungGio),
                    new SqlParameter("@TrangThai", true)
                );

                return Json(new { success = true, message = "Tạo khung giờ thành công!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating time slot");
                return Json(new { success = false, message = ex.Message });
            }
        }

        // POST: /Management/CapNhatKhungGio
        [HttpPost]
        public JsonResult CapNhatKhungGio([FromBody] CapNhatKhungGioRequest request)
        {
            try
            {
                var query = @"
                UPDATE KHUNGGIO 
                SET MaLS = @MaLS, GioBatDau = @GioBD, GioKetThuc = @GioKT, 
                    NgayApDung = @NgayApDung, GiaApDung = @GiaApDung, 
                    LoaiNgay = @LoaiNgay, TenKhungGio = @TenKhungGio
                WHERE MaKG = @MaKG";

                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@MaKG", request.MaKG),
                    new SqlParameter("@MaLS", request.MaLS),
                    new SqlParameter("@GioBD", TimeSpan.Parse(request.GioBatDau)),
                    new SqlParameter("@GioKT", TimeSpan.Parse(request.GioKetThuc)),
                    new SqlParameter("@NgayApDung", DateTime.Parse(request.NgayApDung)),
                    new SqlParameter("@GiaApDung", request.GiaApDung),
                    new SqlParameter("@LoaiNgay", request.LoaiNgay),
                    new SqlParameter("@TenKhungGio", request.TenKhungGio)
                };

                // Update DEFAULT database
                _db.ExecuteNonQuery(query, parameters);

                // *** DEMO UNREPEATABLE READ: Also update FIXED database ***
                try
                {
                    var fixedConnectionString = _configuration.GetConnectionString("FixedConnection");
                    if (!string.IsNullOrEmpty(fixedConnectionString))
                    {
                        using (var conn = new SqlConnection(fixedConnectionString))
                        {
                            using (var cmd = new SqlCommand(query, conn))
                            {
                                // Clone parameters for second database
                                foreach (SqlParameter param in parameters)
                                {
                                    cmd.Parameters.Add(new SqlParameter(param.ParameterName, param.Value));
                                }
                                conn.Open();
                                cmd.ExecuteNonQuery();
                                _logger.LogInformation($"Updated price on BOTH databases: MaKG={request.MaKG}, GiaApDung={request.GiaApDung}");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    // If Fixed DB doesn't exist yet, just log warning and continue
                    _logger.LogWarning($"Could not update Fixed DB (this is OK if not yet created): {ex.Message}");
                }

                return Json(new { success = true, message = "Cập nhật khung giờ thành công!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating time slot");
                return Json(new { success = false, message = ex.Message });
            }
        }

        // POST: /Management/XoaKhungGio
        [HttpPost]
        public JsonResult XoaKhungGio(string maKG)
        {
            try
            {
                var query = "DELETE FROM KHUNGGIO WHERE MaKG = @MaKG";
                _db.ExecuteNonQuery(query, new SqlParameter("@MaKG", maKG));

                return Json(new { success = true, message = "Xóa khung giờ thành công!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting time slot");
                return Json(new { success = false, message = ex.Message });
            }
        }

        // ===== ƯU ĐÃI =====

        // GET: /Management/GetDanhSachUuDai
        [HttpGet]
        public JsonResult GetDanhSachUuDai(string? loaiUuDai = null, bool? trangThai = null)
        {
            try
            {
                var query = @"
                SELECT MaUD, TenCT, TyLeGiamGia, DieuKienApDung, LoaiUuDai,
                       NgayBatDau, NgayKetThuc, GiaTriToiThieu, SoGioToiThieu, 
                       TrangThai, NgayTao
                FROM UUDAI
                WHERE 1=1";

                var parameters = new List<SqlParameter>();

                if (!string.IsNullOrEmpty(loaiUuDai))
                {
                    query += " AND LoaiUuDai = @LoaiUuDai";
                    parameters.Add(new SqlParameter("@LoaiUuDai", loaiUuDai));
                }

                if (trangThai.HasValue)
                {
                    if (trangThai.Value) // Filter Active: (TrangThai = 1 OR NULL) AND (Not Expired)
                    {
                        query += " AND (TrangThai = 1 OR TrangThai IS NULL)";
                        // Optional: To strictly filter out expired items from "Active" list
                        // query += " AND (NgayKetThuc IS NULL OR NgayKetThuc >= CAST(GETDATE() AS DATE))";
                        // However, user requirement emphasizes UI display state. 
                        // Let's keep it simple: Active = Not Paused. Expired items are still "Active" in DB status, just expired.
                        // But logically, if I search "Active", I expect valid ones? 
                        // Let's add the date check for strict correctness if filtering Active.
                        query += " AND (NgayKetThuc IS NULL OR NgayKetThuc >= CAST(GETDATE() AS DATE))";
                    }
                    else // Filter Paused (or Expired? No, filter name is "Trạng thái").
                    {
                        // If user selects "Tạm dừng" (false), show explicitly paused items.
                        // What about "Đã kết thúc"? If UI has only 2 options (Active/Paused), 
                        // "Đã kết thúc" items (Active but Expired) won't show in "Active" (due to date check)
                        // and won't show in "Paused" (TrangThai=1). They disappear?
                        // We should probably allow Expired to show if we don't have a specific filter for it.
                        // OR, maybe the filter dropdown needs a 3rd option?
                        // For now, let's stick to standard behavior: 
                        // "Trạng thái" usually refers to the toggle switch (Active/Paused).
                        // So Active = (TrangThai=1 OR NULL). Paused = (TrangThai=0).
                        // I will NOT filter by date here to ensure expired items (which are technically 'Active' configuration-wise) still show up 
                        // so user can edit them (e.g. extend date). Hiding them makes them inaccessible.
                        
                        // REVERTING Date check decision: Use standard status column filtering to prevent data loss.
                        // User can see "Đã kết thúc" status in the row.
                        query += " AND TrangThai = 0";
                    }
                }
                else 
                {
                   // If trangThai is null (All), show everything
                }

                query += " ORDER BY NgayTao DESC";

                var dt = _db.ExecuteQuery(query, parameters.ToArray());
                var data = new List<object>();

                foreach (DataRow row in dt.Rows)
                {
                    data.Add(new
                    {
                        maUD = row["MaUD"]?.ToString(),
                        tenCT = row["TenCT"]?.ToString(),
                        tyLeGiamGia = row["TyLeGiamGia"] != DBNull.Value ? Convert.ToDecimal(row["TyLeGiamGia"]) : 0,
                        dieuKienApDung = row["DieuKienApDung"]?.ToString(),
                        loaiUuDai = row["LoaiUuDai"]?.ToString(),
                        ngayBatDau = row["NgayBatDau"] != DBNull.Value ? Convert.ToDateTime(row["NgayBatDau"]).ToString("dd/MM/yyyy") : "",
                        ngayKetThuc = row["NgayKetThuc"] != DBNull.Value ? Convert.ToDateTime(row["NgayKetThuc"]).ToString("dd/MM/yyyy") : "",
                        giaTriToiThieu = row["GiaTriToiThieu"] != DBNull.Value ? Convert.ToDecimal(row["GiaTriToiThieu"]) : 0,
                        soGioToiThieu = row["SoGioToiThieu"] != DBNull.Value ? Convert.ToInt32(row["SoGioToiThieu"]) : 0,
                        // Fix: DBNull -> true (Active)
                        trangThai = row["TrangThai"] == DBNull.Value || Convert.ToBoolean(row["TrangThai"]),
                        ngayTao = row["NgayTao"] != DBNull.Value ? Convert.ToDateTime(row["NgayTao"]).ToString("dd/MM/yyyy HH:mm") : ""
                    });
                }

                return Json(new { success = true, data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting promotions");
                return Json(new { success = false, message = ex.Message });
            }
        }

        // POST: /Management/TaoUuDai
        [HttpPost]
        public JsonResult TaoUuDai([FromBody] TaoUuDaiRequest request)
        {
            try
            {
                // Generate MaUD
                var maxMaUD = _db.ExecuteScalar<string>("SELECT MAX(MaUD) FROM UUDAI") ?? "UD00000";
                var numPart = int.Parse(maxMaUD.Substring(2)) + 1;
                var maUD = "UD" + numPart.ToString("D5");

                var query = @"
                INSERT INTO UUDAI (MaUD, TenCT, TyLeGiamGia, DieuKienApDung, LoaiUuDai,
                                   NgayBatDau, NgayKetThuc, GiaTriToiThieu, SoGioToiThieu, TrangThai)
                VALUES (@MaUD, @TenCT, @TyLe, @DieuKien, @LoaiUD, @NgayBD, @NgayKT, 
                        @GiaTriMin, @SoGioMin, @TrangThai)";

                _db.ExecuteNonQuery(query,
                    new SqlParameter("@MaUD", maUD),
                    new SqlParameter("@TenCT", request.TenCT),
                    new SqlParameter("@TyLe", request.TyLeGiamGia),
                    new SqlParameter("@DieuKien", request.DieuKienApDung ?? ""),
                    new SqlParameter("@LoaiUD", request.LoaiUuDai),
                    new SqlParameter("@NgayBD", DateTime.Parse(request.NgayBatDau)),
                    new SqlParameter("@NgayKT", DateTime.Parse(request.NgayKetThuc)),
                    new SqlParameter("@GiaTriMin", request.GiaTriToiThieu),
                    new SqlParameter("@SoGioMin", request.SoGioToiThieu),
                    new SqlParameter("@TrangThai", true)
                );

                return Json(new { success = true, message = "Tạo ưu đãi thành công!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating promotion");
                return Json(new { success = false, message = ex.Message });
            }
        }

        // POST: /Management/CapNhatUuDai
        [HttpPost]
        public JsonResult CapNhatUuDai([FromBody] CapNhatUuDaiRequest request)
        {
            try
            {
                var query = @"
                UPDATE UUDAI 
                SET TenCT = @TenCT, TyLeGiamGia = @TyLe, DieuKienApDung = @DieuKien,
                    LoaiUuDai = @LoaiUD, NgayBatDau = @NgayBD, NgayKetThuc = @NgayKT,
                    GiaTriToiThieu = @GiaTriMin, SoGioToiThieu = @SoGioMin
                WHERE MaUD = @MaUD";

                _db.ExecuteNonQuery(query,
                    new SqlParameter("@MaUD", request.MaUD),
                    new SqlParameter("@TenCT", request.TenCT),
                    new SqlParameter("@TyLe", request.TyLeGiamGia),
                    new SqlParameter("@DieuKien", request.DieuKienApDung ?? ""),
                    new SqlParameter("@LoaiUD", request.LoaiUuDai),
                    new SqlParameter("@NgayBD", DateTime.Parse(request.NgayBatDau)),
                    new SqlParameter("@NgayKT", DateTime.Parse(request.NgayKetThuc)),
                    new SqlParameter("@GiaTriMin", request.GiaTriToiThieu),
                    new SqlParameter("@SoGioMin", request.SoGioToiThieu)
                );

                return Json(new { success = true, message = "Cập nhật ưu đãi thành công!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating promotion");
                return Json(new { success = false, message = ex.Message });
            }
        }

        // POST: /Management/BatTatUuDai
        [HttpPost]
        public JsonResult BatTatUuDai(string maUD, bool trangThai)
        {
            try
            {
                var query = "UPDATE UUDAI SET TrangThai = @TrangThai WHERE MaUD = @MaUD";
                _db.ExecuteNonQuery(query, 
                    new SqlParameter("@TrangThai", trangThai),
                    new SqlParameter("@MaUD", maUD)
                );

                return Json(new { success = true, message = trangThai ? "Đã bật ưu đãi!" : "Đã tắt ưu đãi!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error toggling promotion status");
                return Json(new { success = false, message = ex.Message });
            }
        }

        // POST: /Management/XoaUuDai
        [HttpPost]
        public JsonResult XoaUuDai(string maUD)
        {
            try
            {
                // Check if promotion is used in any orders
                var checkQuery = "SELECT COUNT(*) FROM UUDAI_APDUNG WHERE MaUD = @MaUD";
                var dt = _db.ExecuteQuery(checkQuery, new SqlParameter("@MaUD", maUD));
                var count = Convert.ToInt32(dt.Rows[0][0]);

                if (count > 0)
                {
                    return Json(new { success = false, message = "Không thể xóa ưu đãi đã được sử dụng!" });
                }

                var query = "DELETE FROM UUDAI WHERE MaUD = @MaUD";
                _db.ExecuteNonQuery(query, new SqlParameter("@MaUD", maUD));

                return Json(new { success = true, message = "Xóa ưu đãi thành công!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting promotion");
                return Json(new { success = false, message = ex.Message });
            }
        }

        // ===== THAM SỐ HỆ THỐNG =====

        // GET: /Management/GetThamSoHeThong
        [HttpGet]
        public JsonResult GetThamSoHeThong()
        {
            try
            {
                var query = "SELECT * FROM THAMSO_HETHONG ORDER BY MaThamSo";
                var dt = _db.ExecuteQuery(query);
                var data = new List<object>();

                foreach (DataRow row in dt.Rows)
                {
                    data.Add(new
                    {
                        maThamSo = row["MaThamSo"]?.ToString(),
                        tenThamSo = row["TenThamSo"]?.ToString(),
                        giaTri = row["GiaTri"]?.ToString(),
                        donVi = row["DonVi"]?.ToString(),
                        moTa = row["MoTa"]?.ToString(),
                        ngayCapNhat = row["NgayCapNhat"] != DBNull.Value ? Convert.ToDateTime(row["NgayCapNhat"]).ToString("dd/MM/yyyy HH:mm") : ""
                    });
                }

                return Json(new { success = true, data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting system parameters");
                return Json(new { success = false, message = ex.Message });
            }
        }

        // POST: /Management/CapNhatThamSo
        [HttpPost]
        public JsonResult CapNhatThamSo([FromBody] List<CapNhatThamSoRequest> parameters)
        {
            try
            {
                foreach (var param in parameters)
                {
                    var query = @"
                    UPDATE THAMSO_HETHONG 
                    SET GiaTri = @GiaTri, NgayCapNhat = GETDATE()
                    WHERE MaThamSo = @MaThamSo";

                    _db.ExecuteNonQuery(query,
                        new SqlParameter("@MaThamSo", param.MaThamSo),
                        new SqlParameter("@GiaTri", param.GiaTri)
                    );
                }

                return Json(new { success = true, message = "Cập nhật tham số thành công!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating system parameters");
                return Json(new { success = false, message = ex.Message });
            }
        }

        // GET: /Management/GetLoaiSan - Get list of court types
        [HttpGet]
        public JsonResult GetLoaiSan()
        {
            try
            {
                var query = "SELECT MaLS, TenLS FROM LOAISAN ORDER BY TenLS";
                var dt = _db.ExecuteQuery(query);
                var data = new List<object>();

                foreach (DataRow row in dt.Rows)
                {
                    data.Add(new
                    {
                        maLS = row["MaLS"]?.ToString(),
                        tenLS = row["TenLS"]?.ToString()
                    });
                }

                return Json(new { success = true, data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting court types");
                return Json(new { success = false, message = ex.Message });
            }
        }

        // POST: /Management/CreateCaTruc - Create shift and assign employees
        [HttpPost]
        public JsonResult CreateCaTruc(string ngayTruc, string tenCa, string gioBatDau, string gioKetThuc, string? ghiChu, List<string>? maNVs)
        {
            try
            {
                // Get current logged-in manager
                var maNguoiTaoCA = HttpContext.Session.GetString("MaUser");
                if (string.IsNullOrEmpty(maNguoiTaoCA))
                {
                    return Json(new { success = false, message = "Phiên đăng nhập hết hạn!" });
                }

                // Generate MaCaTruc (Removed - using IDENTITY)

                // Calculate PhuCap based on shift time
                decimal phuCap = 0;
                if (gioBatDau == "06:00") phuCap = 50000; // Morning shift
                else if (gioBatDau == "18:00") phuCap = 100000; // Evening shift

                // 1. INSERT into CATRUC (Schema assumes MaCaTruc is IDENTITY)
                // Use SCOPE_IDENTITY() to get the generated ID
                var insertCaTructQuery = @"
                    INSERT INTO CATRUC (MaNV, NgayTruc, GioBatDau, GioKetThuc, PhuCap)
                    VALUES (@MaNV, @NgayTruc, @GioBatDau, @GioKetThuc, @PhuCap);
                    SELECT CAST(SCOPE_IDENTITY() AS INT);";

                var newMaCaTruc = _db.ExecuteScalar<int>(insertCaTructQuery,
                    new SqlParameter("@MaNV", maNguoiTaoCA),
                    new SqlParameter("@NgayTruc", DateTime.Parse(ngayTruc)),
                    new SqlParameter("@GioBatDau", TimeSpan.Parse(gioBatDau)),
                    new SqlParameter("@GioKetThuc", TimeSpan.Parse(gioKetThuc)),
                    new SqlParameter("@PhuCap", phuCap)
                );
                
                var maCaTruc = newMaCaTruc.ToString();

                // 2. INSERT into THAMGIACATRUC for each selected employee
                if (maNVs != null && maNVs.Count > 0)
                {
                    var insertThamGiaQuery = @"
                        INSERT INTO THAMGIACATRUC (MaCaTruc, MaNV)
                        VALUES (@MaCaTruc, @MaNV)";

                    foreach (var maNV in maNVs)
                    {
                        if (!string.IsNullOrWhiteSpace(maNV))
                        {
                            _db.ExecuteNonQuery(insertThamGiaQuery,
                                new SqlParameter("@MaCaTruc", maCaTruc),
                                new SqlParameter("@MaNV", maNV.Trim())
                            );
                        }
                    }
                }

                _logger.LogInformation($"Created shift {maCaTruc} with {maNVs?.Count ?? 0} employees");
                return Json(new { success = true, message = "Tạo ca trực thành công!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating shift");
                return Json(new { success = false, message = "Lỗi: " + ex.Message });
            }
        }
    }

    // Request models
    public class TaoBaoCaoLoiRequest
    {
        public string MaSan { get; set; } = "";
        public string MoTaSuCo { get; set; } = "";
    }

    public class CapNhatTrangThaiRequest
    {
        public string MaPhieu { get; set; } = "";
        public string TrangThai { get; set; } = "";
        public string NgayKetThucDuKien { get; set; } = "";
        public decimal ChiPhi { get; set; }
        public string MaNV { get; set; } = "";
    }

    public class HoanThanhBaoCaoRequest
    {
        public string MaPhieu { get; set; } = "";
        public decimal ChiPhi { get; set; }
    }

    // Request models for Quản Lý Giá
    public class TaoKhungGioRequest
    {
        public string MaLS { get; set; } = "";
        public string GioBatDau { get; set; } = "";
        public string GioKetThuc { get; set; } = "";
        public string NgayApDung { get; set; } = "";
        public decimal GiaApDung { get; set; }
        public string LoaiNgay { get; set; } = "Thường";
        public string TenKhungGio { get; set; } = "";
    }

    public class CapNhatKhungGioRequest
    {
        public string MaKG { get; set; } = "";
        public string MaLS { get; set; } = "";
        public string GioBatDau { get; set; } = "";
        public string GioKetThuc { get; set; } = "";
        public string NgayApDung { get; set; } = "";
        public decimal GiaApDung { get; set; }
        public string LoaiNgay { get; set; } = "Thường";
        public string TenKhungGio { get; set; } = "";
    }

    public class TaoUuDaiRequest
    {
        public string TenCT { get; set; } = "";
        public decimal TyLeGiamGia { get; set; }
        public string? DieuKienApDung { get; set; }
        public string LoaiUuDai { get; set; } = "";
        public string NgayBatDau { get; set; } = "";
        public string NgayKetThuc { get; set; } = "";
        public decimal GiaTriToiThieu { get; set; }
        public int SoGioToiThieu { get; set; }
    }

    public class CapNhatUuDaiRequest
    {
        public string MaUD { get; set; } = "";
        public string TenCT { get; set; } = "";
        public decimal TyLeGiamGia { get; set; }
        public string? DieuKienApDung { get; set; }
        public string LoaiUuDai { get; set; } = "";
        public string NgayBatDau { get; set; } = "";
        public string NgayKetThuc { get; set; } = "";
        public decimal GiaTriToiThieu { get; set; }
        public int SoGioToiThieu { get; set; }
    }

    public class CapNhatThamSoRequest
    {
        public string MaThamSo { get; set; } = "";
        public string GiaTri { get; set; } = "";
    }
}
