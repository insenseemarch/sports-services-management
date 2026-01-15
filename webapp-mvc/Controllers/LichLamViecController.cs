using Microsoft.AspNetCore.Mvc;
using webapp_mvc.Models;
using System.Data;
using System.Data.SqlClient;

namespace webapp_mvc.Controllers
{
    public class LichLamViecController : Controller
    {
        private readonly DatabaseHelper _db;
        private readonly ILogger<LichLamViecController> _logger;

        public LichLamViecController(IConfiguration configuration, ILogger<LichLamViecController> logger)
        {
            _db = new DatabaseHelper(configuration);
            _logger = logger;
        }

        // GET: /LichLamViec/Index
        public IActionResult Index()
        {
            var maNV = HttpContext.Session.GetString("MaUser");
            if (string.IsNullOrEmpty(maNV))
            {
                return RedirectToAction("DangNhap", "TaiKhoan");
            }

            return View();
        }

        // GET: /LichLamViec/GetMySchedule
        [HttpGet]
        public IActionResult GetMySchedule(string? fromDate, string? toDate)
        {
            try
            {
                var maNV = HttpContext.Session.GetString("MaUser");
                if (string.IsNullOrEmpty(maNV))
                {
                    return Json(new { success = false, message = "Chưa đăng nhập!" });
                }

                var from = string.IsNullOrEmpty(fromDate) ? DateTime.Now : DateTime.Parse(fromDate);
                var to = string.IsNullOrEmpty(toDate) ? DateTime.Now.AddDays(30) : DateTime.Parse(toDate);

                var query = @"
                    SELECT 
                        CT.MaCaTruc,
                        CONVERT(VARCHAR(10), CT.NgayTruc, 103) as NgayTruc,
                        CONVERT(VARCHAR(5), CT.GioBatDau, 108) + ' - ' + CONVERT(VARCHAR(5), CT.GioKetThuc, 108) as GioTruc,
                        ISNULL(CS.TenCS, N'Chưa xác định') as TenCoSo,
                        ISNULL(CT.PhuCap, 0) as PhuCap,
                        ISNULL(NV.HoTen, N'N/A') as NguoiQuanLy
                    FROM THAMGIACATRUC TGCT
                    INNER JOIN CATRUC CT ON TGCT.MaCaTruc = CT.MaCaTruc
                    LEFT JOIN NHANVIEN NV ON CT.MaNV = NV.MaNV
                    LEFT JOIN COSO CS ON NV.MaCS = CS.MaCS
                    WHERE TGCT.MaNV = @MaNV
                    AND CT.NgayTruc BETWEEN @FromDate AND @ToDate
                    ORDER BY CT.NgayTruc DESC, CT.GioBatDau DESC
                ";

                var result = _db.ExecuteQuery(query, 
                    new SqlParameter("@MaNV", maNV),
                    new SqlParameter("@FromDate", from),
                    new SqlParameter("@ToDate", to));

                var schedule = new List<dynamic>();
                foreach (DataRow row in result.Rows)
                {
                    schedule.Add(new
                    {
                        maCaTruc = row["MaCaTruc"].ToString(),
                        ngayTruc = row["NgayTruc"].ToString(),
                        gioTruc = row["GioTruc"].ToString(),
                        tenCoSo = row["TenCoSo"].ToString(),
                        phuCap = decimal.Parse(row["PhuCap"].ToString()).ToString("N0"),
                        nguoiQuanLy = row["NguoiQuanLy"].ToString()
                    });
                }

                _logger.LogInformation("GetMySchedule: Found {Count} shifts for staff {MaNV}", schedule.Count, maNV);
                return Json(new { success = true, data = schedule });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading schedule");
                return Json(new { success = false, message = ex.Message });
            }
        }

        // GET: /LichLamViec/GetAvailableShifts
        [HttpGet]
        public IActionResult GetAvailableShifts()
        {
            try
            {
                var maNV = HttpContext.Session.GetString("MaUser");
                if (string.IsNullOrEmpty(maNV))
                {
                    return Json(new { success = false, message = "Chưa đăng nhập!" });
                }

                // Get shifts that need more staff (future shifts only)
                var query = @"
                    SELECT 
                        CT.MaCaTruc,
                        CONVERT(VARCHAR(10), CT.NgayTruc, 103) as NgayTruc,
                        CONVERT(VARCHAR(5), CT.GioBatDau, 108) + ' - ' + CONVERT(VARCHAR(5), CT.GioKetThuc, 108) as GioTruc,
                        ISNULL(CS.TenCS, N'Chưa xác định') as TenCoSo,
                        3 as SoNhanVienCanThiet,
                        COUNT(TGCT.MaNV) as SoNhanVienHienTai
                    FROM CATRUC CT
                    LEFT JOIN THAMGIACATRUC TGCT ON CT.MaCaTruc = TGCT.MaCaTruc
                    LEFT JOIN NHANVIEN NV ON CT.MaNV = NV.MaNV
                    LEFT JOIN COSO CS ON NV.MaCS = CS.MaCS
                    WHERE CT.NgayTruc >= CAST(GETDATE() AS DATE)
                    AND CT.MaCaTruc NOT IN (
                        SELECT MaCaTruc FROM THAMGIACATRUC WHERE MaNV = @MaNV
                    )
                    GROUP BY CT.MaCaTruc, CT.NgayTruc, CT.GioBatDau, CT.GioKetThuc, CS.TenCS
                    HAVING COUNT(TGCT.MaNV) < 3
                    ORDER BY CT.NgayTruc, CT.GioBatDau
                ";

                var result = _db.ExecuteQuery(query, new SqlParameter("@MaNV", maNV));
                var shifts = new List<dynamic>();

                foreach (DataRow row in result.Rows)
                {
                    shifts.Add(new
                    {
                        maCaTruc = long.Parse(row["MaCaTruc"].ToString()),
                        ngayTruc = row["NgayTruc"].ToString(),
                        gioTruc = row["GioTruc"].ToString(),
                        tenCoSo = row["TenCoSo"].ToString(),
                        soNhanVienCanThiet = int.Parse(row["SoNhanVienCanThiet"].ToString()),
                        soNhanVienHienTai = int.Parse(row["SoNhanVienHienTai"].ToString())
                    });
                }

                _logger.LogInformation("GetAvailableShifts: Found {Count} available shifts", shifts.Count);
                return Json(new { success = true, data = shifts });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading available shifts");
                return Json(new { success = false, message = ex.Message });
            }
        }

        // POST: /LichLamViec/RegisterShift
        [HttpPost]
        public IActionResult RegisterShift([FromBody] RegisterShiftRequest request)
        {
            try
            {
                var maNV = HttpContext.Session.GetString("MaUser");
                if (string.IsNullOrEmpty(maNV))
                {
                    return Json(new { success = false, message = "Chưa đăng nhập!" });
                }

                // Check for conflicts
                var checkQuery = @"
                    SELECT COUNT(*) as Conflicts
                    FROM THAMGIACATRUC TGCT
                    INNER JOIN CATRUC CT1 ON TGCT.MaCaTruc = CT1.MaCaTruc
                    INNER JOIN CATRUC CT2 ON CT2.MaCaTruc = @MaCaTruc
                    WHERE TGCT.MaNV = @MaNV
                    AND CT1.NgayTruc = CT2.NgayTruc
                    AND (
                        (CT1.GioBatDau <= CT2.GioBatDau AND CT1.GioKetThuc > CT2.GioBatDau)
                        OR (CT1.GioBatDau < CT2.GioKetThuc AND CT1.GioKetThuc >= CT2.GioKetThuc)
                        OR (CT1.GioBatDau >= CT2.GioBatDau AND CT1.GioKetThuc <= CT2.GioKetThuc)
                    )
                ";

                var conflicts = _db.ExecuteScalar<int>(checkQuery, 
                    new SqlParameter("@MaNV", maNV),
                    new SqlParameter("@MaCaTruc", request.MaCaTruc));

                if (conflicts > 0)
                {
                    return Json(new { success = false, message = "Bạn đã có ca trực trùng giờ trong ngày này!" });
                }

                // Register shift
                var insertQuery = @"
                    INSERT INTO THAMGIACATRUC (MaCaTruc, MaNV)
                    VALUES (@MaCaTruc, @MaNV)
                ";

                _db.ExecuteNonQuery(insertQuery,
                    new SqlParameter("@MaCaTruc", request.MaCaTruc),
                    new SqlParameter("@MaNV", maNV));

                _logger.LogInformation("RegisterShift: Staff {MaNV} registered for shift {MaCaTruc}", maNV, request.MaCaTruc);
                return Json(new { success = true, message = "Đăng ký ca trực thành công!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error registering shift");
                return Json(new { success = false, message = "Lỗi: " + ex.Message });
            }
        }

        // GET: /LichLamViec/GetMyUpcomingShifts
        [HttpGet]
        public IActionResult GetMyUpcomingShifts()
        {
            try
            {
                var maNV = HttpContext.Session.GetString("MaUser");
                if (string.IsNullOrEmpty(maNV))
                {
                    return Json(new { success = false, message = "Chưa đăng nhập!" });
                }

                var query = @"
                    SELECT 
                        CT.MaCaTruc,
                        CONVERT(VARCHAR(10), CT.NgayTruc, 103) as NgayTruc,
                        CONVERT(VARCHAR(5), CT.GioBatDau, 108) + ' - ' + CONVERT(VARCHAR(5), CT.GioKetThuc, 108) as GioTruc,
                        ISNULL(CS.TenCS, N'Chưa xác định') as TenCoSo
                    FROM THAMGIACATRUC TGCT
                    INNER JOIN CATRUC CT ON TGCT.MaCaTruc = CT.MaCaTruc
                    LEFT JOIN NHANVIEN NV ON CT.MaNV = NV.MaNV
                    LEFT JOIN COSO CS ON NV.MaCS = CS.MaCS
                    WHERE TGCT.MaNV = @MaNV
                    AND CT.NgayTruc >= CAST(GETDATE() AS DATE)
                    ORDER BY CT.NgayTruc, CT.GioBatDau
                ";

                var result = _db.ExecuteQuery(query, new SqlParameter("@MaNV", maNV));
                var shifts = new List<dynamic>();

                foreach (DataRow row in result.Rows)
                {
                    shifts.Add(new
                    {
                        maCaTruc = long.Parse(row["MaCaTruc"].ToString()),
                        ngayTruc = row["NgayTruc"].ToString(),
                        gioTruc = row["GioTruc"].ToString(),
                        tenCoSo = row["TenCoSo"].ToString()
                    });
                }

                return Json(new { success = true, data = shifts });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading upcoming shifts");
                return Json(new { success = false, message = ex.Message });
            }
        }

        // GET: /LichLamViec/GetStaffForReplacement
        [HttpGet]
        public IActionResult GetStaffForReplacement(long? maCaTruc)
        {
            try
            {
                var maNV = HttpContext.Session.GetString("MaUser");
                var maCS = HttpContext.Session.GetString("MaCS");
                
                _logger.LogInformation("GetStaffForReplacement called with maCaTruc={MaCaTruc}, maNV={MaNV}, maCS={MaCS}", maCaTruc, maNV, maCS);
                
                if (string.IsNullOrEmpty(maNV))
                {
                    return Json(new { success = false, message = "Chưa đăng nhập!" });
                }

                // If maCS not in session, get it from NHANVIEN table
                if (string.IsNullOrEmpty(maCS))
                {
                    var csQuery = "SELECT MaCS FROM NHANVIEN WHERE MaNV = @MaNV";
                    var csResult = _db.ExecuteQuery(csQuery, new SqlParameter("@MaNV", maNV));
                    if (csResult.Rows.Count > 0)
                    {
                        maCS = csResult.Rows[0]["MaCS"].ToString();
                        _logger.LogInformation("Retrieved MaCS from NHANVIEN: {MaCS}", maCS);
                    }
                    else
                    {
                        return Json(new { success = false, message = "Không tìm thấy thông tin cơ sở của nhân viên!" });
                    }
                }

                string query;
                SqlParameter[] parameters;

                if (maCaTruc.HasValue)
                {
                    // Get staff not busy during the selected shift
                    query = @"
                        SELECT DISTINCT NV.MaNV, NV.HoTen, NV.ChucVu
                        FROM NHANVIEN NV
                        WHERE NV.MaCS = @MaCS
                        AND NV.MaNV != @MaNV
                        AND NV.ChucVu IN (N'Lễ tân', N'Nhân viên kỹ thuật', N'Nhân viên vệ sinh')
                        AND NV.MaNV NOT IN (
                            -- Exclude staff already assigned to overlapping shifts
                            SELECT TGC.MaNV
                            FROM THAMGIACATRUC TGC
                            INNER JOIN CATRUC CT ON TGC.MaCaTruc = CT.MaCaTruc
                            INNER JOIN CATRUC CT_Selected ON CT_Selected.MaCaTruc = @MaCaTruc
                            WHERE CT.NgayTruc = CT_Selected.NgayTruc
                            AND (
                                (CT.GioBatDau <= CT_Selected.GioBatDau AND CT.GioKetThuc > CT_Selected.GioBatDau)
                                OR (CT.GioBatDau < CT_Selected.GioKetThuc AND CT.GioKetThuc >= CT_Selected.GioKetThuc)
                                OR (CT.GioBatDau >= CT_Selected.GioBatDau AND CT.GioKetThuc <= CT_Selected.GioKetThuc)
                            )
                        )
                        AND NV.MaNV NOT IN (
                            -- Exclude staff with approved leave during this shift
                            SELECT DNP.MaNV
                            FROM DONNGHIPHEP DNP
                            INNER JOIN CATRUC CT_Selected ON CT_Selected.MaCaTruc = @MaCaTruc
                            WHERE DNP.TrangThai = N'Đã duyệt'
                            AND CT_Selected.NgayTruc = DNP.NgayNghi
                        )
                        ORDER BY NV.HoTen
                    ";
                    
                    parameters = new SqlParameter[] {
                        new SqlParameter("@MaCS", maCS),
                        new SqlParameter("@MaNV", maNV),
                        new SqlParameter("@MaCaTruc", maCaTruc.Value)
                    };
                }
                else
                {
                    // No shift selected, return all staff
                    query = @"
                        SELECT MaNV, HoTen, ChucVu
                        FROM NHANVIEN
                        WHERE MaCS = @MaCS
                        AND MaNV != @MaNV
                        AND ChucVu IN (N'Lễ tân', N'Nhân viên kỹ thuật', N'Nhân viên vệ sinh')
                        ORDER BY HoTen
                    ";
                    
                    parameters = new SqlParameter[] {
                        new SqlParameter("@MaCS", maCS),
                        new SqlParameter("@MaNV", maNV)
                    };
                }

                var result = _db.ExecuteQuery(query, parameters);
                
                _logger.LogInformation("GetStaffForReplacement: Query returned {Count} staff members", result.Rows.Count);
                
                var staff = new List<dynamic>();
                foreach (DataRow row in result.Rows)
                {
                    staff.Add(new
                    {
                        maNV = row["MaNV"].ToString(),
                        hoTen = row["HoTen"].ToString(),
                        chucVu = row["ChucVu"].ToString()
                    });
                }

                _logger.LogInformation("GetStaffForReplacement: Returning {Count} staff members", staff.Count);
                return Json(new { success = true, data = staff });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading staff for replacement");
                return Json(new { success = false, message = ex.Message });
            }
        }

        // POST: /LichLamViec/SubmitLeaveRequest
        [HttpPost]
        public IActionResult SubmitLeaveRequest([FromBody] LeaveRequest request)
        {
            try
            {
                var maNV = HttpContext.Session.GetString("MaUser");
                if (string.IsNullOrEmpty(maNV))
                {
                    return Json(new { success = false, message = "Chưa đăng nhập!" });
                }

                // Validate shift belongs to staff and get NgayTruc
                var validateQuery = @"
                    SELECT CT.NgayTruc
                    FROM THAMGIACATRUC TGCT
                    INNER JOIN CATRUC CT ON TGCT.MaCaTruc = CT.MaCaTruc
                    WHERE TGCT.MaCaTruc = @CaNghi AND TGCT.MaNV = @MaNV
                ";
                var result = _db.ExecuteQuery(validateQuery,
                    new SqlParameter("@CaNghi", request.CaNghi),
                    new SqlParameter("@MaNV", maNV));

                if (result.Rows.Count == 0)
                {
                    return Json(new { success = false, message = "Ca trực không hợp lệ hoặc bạn không tham gia ca này!" });
                }

                var ngayTruc = Convert.ToDateTime(result.Rows[0]["NgayTruc"]);

                // Insert leave request
                var insertQuery = @"
                    INSERT INTO DONNGHIPHEP (MaNV, NgayNghi, CaNghi, LyDo, TrangThai, NguoiThayThe)
                    VALUES (@MaNV, @NgayNghi, @CaNghi, @LyDo, N'Chờ duyệt', @NguoiThayThe)
                ";

                _db.ExecuteNonQuery(insertQuery,
                    new SqlParameter("@MaNV", maNV),
                    new SqlParameter("@NgayNghi", ngayTruc),
                    new SqlParameter("@CaNghi", request.CaNghi),
                    new SqlParameter("@LyDo", request.LyDo),
                    new SqlParameter("@NguoiThayThe", string.IsNullOrEmpty(request.NguoiThayThe) ? DBNull.Value : request.NguoiThayThe));

                _logger.LogInformation("SubmitLeaveRequest: Staff {MaNV} requested leave for shift {CaNghi} on {NgayTruc}", maNV, request.CaNghi, ngayTruc);
                return Json(new { success = true, message = "Gửi đơn nghỉ phép thành công!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error submitting leave request");
                return Json(new { success = false, message = "Lỗi: " + ex.Message });
            }
        }

        // GET: /LichLamViec/GetLeaveHistory
        [HttpGet]
        public IActionResult GetLeaveHistory()
        {
            try
            {
                var maNV = HttpContext.Session.GetString("MaUser");
                if (string.IsNullOrEmpty(maNV))
                {
                    return Json(new { success = false, message = "Chưa đăng nhập!" });
                }

                var query = @"
                    SELECT 
                        D.MaDon,
                        CONVERT(VARCHAR(10), D.NgayNghi, 103) as NgayNghi,
                        CONVERT(VARCHAR(5), CT.GioBatDau, 108) + ' - ' + CONVERT(VARCHAR(5), CT.GioKetThuc, 108) as GioTruc,
                        D.LyDo,
                        ISNULL(NV.HoTen, N'Chưa có') as NguoiThayThe,
                        D.TrangThai,
                        ISNULL(CONVERT(VARCHAR(10), D.NgayDuyet, 103), '') as NgayDuyet
                    FROM DONNGHIPHEP D
                    LEFT JOIN CATRUC CT ON D.CaNghi = CT.MaCaTruc
                    LEFT JOIN NHANVIEN NV ON D.NguoiThayThe = NV.MaNV
                    WHERE D.MaNV = @MaNV
                    ORDER BY D.NgayNghi DESC
                ";

                var result = _db.ExecuteQuery(query, new SqlParameter("@MaNV", maNV));
                var history = new List<dynamic>();

                foreach (DataRow row in result.Rows)
                {
                    history.Add(new
                    {
                        maDon = row["MaDon"].ToString(),
                        ngayNghi = row["NgayNghi"].ToString(),
                        gioTruc = row["GioTruc"].ToString(),
                        lyDo = row["LyDo"].ToString(),
                        nguoiThayThe = row["NguoiThayThe"].ToString(),
                        trangThai = row["TrangThai"].ToString(),
                        ngayDuyet = row["NgayDuyet"].ToString()
                    });
                }

                _logger.LogInformation("GetLeaveHistory: Found {Count} leave requests for staff {MaNV}", history.Count, maNV);
                return Json(new { success = true, data = history });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading leave history");
                return Json(new { success = false, message = ex.Message });
            }
        }
    }

    // Request models
    public class RegisterShiftRequest
    {
        public long MaCaTruc { get; set; }
    }

    public class LeaveRequest
    {
        public long CaNghi { get; set; }
        public string LyDo { get; set; } = string.Empty;
        public string? NguoiThayThe { get; set; }
    }
}
