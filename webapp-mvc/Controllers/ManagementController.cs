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

        public ManagementController(IConfiguration configuration, ILogger<ManagementController> logger)
        {
            _db = new DatabaseHelper(configuration);
            _logger = logger;
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

        // POST: /Management/CreateCaTruc
        [HttpPost]
        public IActionResult CreateCaTruc(DateTime ngayTruc, string tenCa, string gioBatDau, string gioKetThuc, string? ghiChu, string[]? maNVs)
        {
            try
            {
                // CATRUC schema: MaCaTruc (IDENTITY), MaNV, NgayTruc, GioBatDau, GioKetThuc, PhuCap
                // Insert MaNV = current user (creator) when available
                var maNVCreator = HttpContext.Session.GetString("MaUser");
                var sql = @"INSERT INTO CATRUC (MaNV, NgayTruc, GioBatDau, GioKetThuc, PhuCap)
                            VALUES (@MaNV, @Ngay, @GioBD, @GioKT, @Phu);
                            SELECT SCOPE_IDENTITY();";

                var parameters = new[] {
                    new System.Data.SqlClient.SqlParameter("@MaNV", (object?)maNVCreator ?? DBNull.Value),
                    new System.Data.SqlClient.SqlParameter("@Ngay", System.Data.SqlDbType.Date) { Value = ngayTruc.Date },
                    new System.Data.SqlClient.SqlParameter("@GioBD", System.Data.SqlDbType.NVarChar, 5) { Value = (object?)gioBatDau ?? "" },
                    new System.Data.SqlClient.SqlParameter("@GioKT", System.Data.SqlDbType.NVarChar, 5) { Value = (object?)gioKetThuc ?? "" },
                    new System.Data.SqlClient.SqlParameter("@Phu", System.Data.SqlDbType.Decimal) { Value = 0m }
                };

                var idObj = _db.ExecuteScalar<object>(sql, parameters);

                long maCa = 0;
                if (idObj != null && long.TryParse(idObj.ToString(), out var v)) maCa = v;

                // If client supplied assigned employees, persist them to THAMGIACATRUC
                if (maCa > 0 && maNVs != null && maNVs.Length > 0)
                {
                    try
                    {
                        var insertTG = "INSERT INTO THAMGIACATRUC (MaCaTruc, MaNV) VALUES (@MaCa, @MaNV)";
                        foreach (var mnv in maNVs)
                        {
                            if (string.IsNullOrWhiteSpace(mnv)) continue;
                            _db.ExecuteNonQuery(insertTG,
                                new System.Data.SqlClient.SqlParameter("@MaCa", maCa),
                                new System.Data.SqlClient.SqlParameter("@MaNV", mnv));
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Assigning staff after CreateCaTruc failed");
                        // do not fail the whole request — shift was created; return info to client
                    }
                }

                return Json(new { success = true, maCa = maCa, message = "Tạo ca trực thành công" });
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "CreateCaTruc failed");
                return Json(new { success = false, message = ex.Message });
            }
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
}
