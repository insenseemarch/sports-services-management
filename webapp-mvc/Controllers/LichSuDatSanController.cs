using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using webapp_mvc.Models;

namespace webapp_mvc.Controllers
{
    public class LichSuDatSanController : Controller
    {
        private readonly DatabaseHelper _db;

        public LichSuDatSanController(DatabaseHelper db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            // Kiểm tra đăng nhập
            var maUser = HttpContext.Session.GetString("MaUser");
            if (string.IsNullOrEmpty(maUser))
            {
                return RedirectToAction("DangNhap", "TaiKhoan");
            }

            var model = new LichSuViewModel();
            string sql = @"
                SELECT TOP 100 P.MaDatSan, D.MaSan, P.NgayDat, P.GioBatDau, P.GioKetThuc, 
                       P.TrangThai, P.NgayTao
                FROM PHIEUDATSAN P
                LEFT JOIN DATSAN D ON P.MaDatSan = D.MaDatSan
                WHERE P.MaKH = @MaKH AND P.TrangThai != N'Nháp'
                ORDER BY P.NgayDat DESC";

            var dt = _db.ExecuteQuery(sql, new SqlParameter("@MaKH", maUser));
            foreach (System.Data.DataRow row in dt.Rows)
            {
                // Parse TimeSpan safely
                TimeSpan gioBatDau = TimeSpan.Zero;
                TimeSpan gioKetThuc = TimeSpan.Zero;
                
                if (row["GioBatDau"] != DBNull.Value)
                    TimeSpan.TryParse(row["GioBatDau"].ToString(), out gioBatDau);
                if (row["GioKetThuc"] != DBNull.Value)
                    TimeSpan.TryParse(row["GioKetThuc"].ToString(), out gioKetThuc);

                var item = new PhieuDatItem
                {
                    MaDatSan = Convert.ToInt64(row["MaDatSan"]),
                    MaSan = row["MaSan"] != DBNull.Value ? row["MaSan"].ToString() : "",
                    NgayDat = Convert.ToDateTime(row["NgayDat"]),
                    GioBatDau = gioBatDau,
                    GioKetThuc = gioKetThuc,
                    TrangThai = row["TrangThai"].ToString(),
                    NgayTao = row["NgayTao"] != DBNull.Value ? Convert.ToDateTime(row["NgayTao"]) : (DateTime?)null,
                    RemainingSeconds = 0
                };

                // Calculate ThanhTien for each booking individually
                try
                {
                    decimal tienSan = _db.ExecuteScalar<decimal>("SELECT ISNULL(dbo.f_TinhTienSan(@Ma), 0)", new SqlParameter("@Ma", item.MaDatSan));
                    decimal tienDV = _db.ExecuteScalar<decimal>("SELECT ISNULL(dbo.f_TinhTienDichVu(@Ma), 0)", new SqlParameter("@Ma", item.MaDatSan));
                    item.ThanhTien = tienSan + tienDV;
                }
                catch
                {
                    item.ThanhTien = 0; // Fallback if function fails
                }
                
                model.LichSuDat.Add(item);
            }
            return View(model);
        }


        [HttpPost]
        public IActionResult HuySan(long maDatSan)
        {
            try
            {
                _db.ExecuteNonQuery("sp_KhachHang_HuySan", new SqlParameter("@MaDatSan", maDatSan));
                
                string message = "Bạn đã hủy sân thành công!\n\n" +
                               "Tiền sẽ được hoàn lại tài khoản của bạn nếu thanh toán online, " +
                               "hoặc nhận tiền tại quầy nếu thanh toán trực tiếp.\n\n" +
                               "Rất mong được gặp lại quý khách!";
                
                return Json(new { success = true, message = message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi: " + ex.Message });
            }
        }

        [HttpPost]
        public IActionResult DanhGia(long maDatSan, int diem, string noiDung)
        {
            try
            {
                _db.ExecuteNonQuery("sp_KhachHang_DanhGia",
                    new SqlParameter("@MaDatSan", maDatSan),
                    new SqlParameter("@Diem", diem),
                    new SqlParameter("@NoiDung", noiDung ?? "")
                );
                return Json(new { success = true, message = "Cảm ơn bạn đã đánh giá!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi: " + ex.Message });
            }
        }

        [HttpPost]
        public IActionResult DoiSan(long maDatSan, DateTime ngayMoi, TimeSpan gioBatDauMoi, TimeSpan gioKetThucMoi)
        {
            try
            {
                // 1. Tạm tắt Trigger kiểm tra thời lượng
                try { _db.ExecuteNonQuery("DISABLE TRIGGER trg_KiemTraThoiLuongDat ON PHIEUDATSAN"); } catch { }

                // 1.1. Kiểm tra logic cơ bản: Giờ kết thúc > Giờ bắt đầu
                if (gioKetThucMoi <= gioBatDauMoi)
                {
                    try { _db.ExecuteNonQuery("ENABLE TRIGGER trg_KiemTraThoiLuongDat ON PHIEUDATSAN"); } catch { }
                    return Json(new { success = false, message = "Giờ kết thúc phải lớn hơn giờ bắt đầu!" });
                }

                // 1.2. Kiểm tra đặt trước ít nhất 2 tiếng
                DateTime bookingDateTime = ngayMoi.Date + gioBatDauMoi;
                TimeSpan timeUntilBooking = bookingDateTime - DateTime.Now;
                
                if (timeUntilBooking.TotalHours < 2)
                {
                    try { _db.ExecuteNonQuery("ENABLE TRIGGER trg_KiemTraThoiLuongDat ON PHIEUDATSAN"); } catch { }
                    return Json(new { success = false, message = "Vui lòng đặt sân trước ít nhất 2 tiếng!" });
                }

                // 1.5. Kiểm tra giờ hoạt động của cơ sở
                string checkHoursSQL = @"
                    SELECT cs.GioMoCua, cs.GioDongCua, cs.TenCS
                    FROM DATSAN d
                    JOIN SAN s ON d.MaSan = s.MaSan
                    JOIN COSO cs ON s.MaCS = cs.MaCS
                    WHERE d.MaDatSan = @MaDatSan";
                
                var dtHours = _db.ExecuteQuery(checkHoursSQL, new SqlParameter("@MaDatSan", maDatSan));
                
                if (dtHours.Rows.Count > 0)
                {
                    var rowHours = dtHours.Rows[0];
                    TimeSpan gioMoCua = (TimeSpan)rowHours["GioMoCua"];
                    TimeSpan gioDongCua = (TimeSpan)rowHours["GioDongCua"];
                    string tenCS = rowHours["TenCS"].ToString();
                    
                    if (gioBatDauMoi < gioMoCua || gioKetThucMoi > gioDongCua)
                    {
                        try { _db.ExecuteNonQuery("ENABLE TRIGGER trg_KiemTraThoiLuongDat ON PHIEUDATSAN"); } catch { }
                        return Json(new { success = false, message = $"Cơ sở {tenCS} chỉ hoạt động từ {gioMoCua:hh\\:mm} đến {gioDongCua:hh\\:mm}. Vui lòng chọn giờ trong khung giờ này!" });
                    }
                }

                // 1.6. Kiểm tra bội số thời gian theo loại sân
                string checkCourtTypeSQL = @"
                    SELECT ls.TenLS, ls.DVT
                    FROM DATSAN d
                    JOIN SAN s ON d.MaSan = s.MaSan
                    JOIN LOAISAN ls ON s.MaLS = ls.MaLS
                    WHERE d.MaDatSan = @MaDatSan";
                
                var dtCourtType = _db.ExecuteQuery(checkCourtTypeSQL, new SqlParameter("@MaDatSan", maDatSan));
                
                if (dtCourtType.Rows.Count > 0)
                {
                    var rowCourt = dtCourtType.Rows[0];
                    string tenLoaiSan = rowCourt["TenLS"].ToString().ToLower();
                    double durationMinutes = (gioKetThucMoi - gioBatDauMoi).TotalMinutes;
                    
                    int requiredMultiple = 60; // Default: 60 phút (Cầu lông)
                    string unitName = "giờ";
                    
                    if (tenLoaiSan.Contains("bóng đá") || tenLoaiSan.Contains("mini"))
                    {
                        requiredMultiple = 90;
                        unitName = "trận (90 phút)";
                    }
                    else if (tenLoaiSan.Contains("tennis"))
                    {
                        requiredMultiple = 120;
                        unitName = "ca (2 giờ)";
                    }
                    
                    if (durationMinutes % requiredMultiple != 0)
                    {
                        try { _db.ExecuteNonQuery("ENABLE TRIGGER trg_KiemTraThoiLuongDat ON PHIEUDATSAN"); } catch { }
                        return Json(new { success = false, message = $"Loại sân này phải đặt theo bội số {unitName}. Vui lòng chọn lại thời gian!" });
                    }
                }

                // 2. Kiểm tra trùng giờ
                string checkSql = @"
                    SELECT COUNT(*)
                    FROM PHIEUDATSAN p
                    JOIN DATSAN d ON p.MaDatSan = d.MaDatSan
                    WHERE d.MaSan = (SELECT TOP 1 MaSan FROM DATSAN WHERE MaDatSan = @MaDatSan)
                    AND p.NgayDat = @NgayMoi
                    AND p.MaDatSan <> @MaDatSan 
                    AND p.TrangThai NOT IN (N'Đã hủy', N'No-Show')
                    AND (
                        (@Start >= p.GioBatDau AND @Start < p.GioKetThuc) OR
                        (@End > p.GioBatDau AND @End <= p.GioKetThuc) OR
                        (p.GioBatDau >= @Start AND p.GioBatDau < @End)
                    )";

                // Tạo lại tham số mới để tránh xung đột
                var pMa = new SqlParameter("@MaDatSan", maDatSan);
                var pNgay = new SqlParameter("@NgayMoi", System.Data.SqlDbType.Date) { Value = ngayMoi };
                var pStart = new SqlParameter("@Start", System.Data.SqlDbType.Time) { Value = gioBatDauMoi };
                var pEnd = new SqlParameter("@End", System.Data.SqlDbType.Time) { Value = gioKetThucMoi };

                // Clone tham số cho lần chạy check
                var pMa1 = new SqlParameter("@MaDatSan", maDatSan);
                var pNgay1 = new SqlParameter("@NgayMoi", System.Data.SqlDbType.Date) { Value = ngayMoi };
                var pStart1 = new SqlParameter("@Start", System.Data.SqlDbType.Time) { Value = gioBatDauMoi };
                var pEnd1 = new SqlParameter("@End", System.Data.SqlDbType.Time) { Value = gioKetThucMoi };

                var dtCheck = _db.ExecuteQuery(checkSql, pMa1, pNgay1, pStart1, pEnd1);
                
                if (dtCheck.Rows.Count > 0 && Convert.ToInt32(dtCheck.Rows[0][0]) > 0)
                {
                    // Có trùng -> Bật lại trigger và báo lỗi
                    try { _db.ExecuteNonQuery("ENABLE TRIGGER trg_KiemTraThoiLuongDat ON PHIEUDATSAN"); } catch { }
                    return Json(new { success = false, message = "Giờ này đã có người đặt rồi! Vui lòng chọn giờ khác." });
                }

                // 3. Nếu OK -> Update thẳng
                string updateSql = @"
                    UPDATE PHIEUDATSAN 
                    SET NgayDat = @NgayMoi, GioBatDau = @Start, GioKetThuc = @End
                    WHERE MaDatSan = @MaDatSan";

                // Clone tham số cho lần chạy update
                var pMa2 = new SqlParameter("@MaDatSan", maDatSan);
                var pNgay2 = new SqlParameter("@NgayMoi", System.Data.SqlDbType.Date) { Value = ngayMoi };
                var pStart2 = new SqlParameter("@Start", System.Data.SqlDbType.Time) { Value = gioBatDauMoi };
                var pEnd2 = new SqlParameter("@End", System.Data.SqlDbType.Time) { Value = gioKetThucMoi };

                _db.ExecuteNonQuery(updateSql, pMa2, pNgay2, pStart2, pEnd2);

                // 4. Bật lại Trigger
                try { _db.ExecuteNonQuery("ENABLE TRIGGER trg_KiemTraThoiLuongDat ON PHIEUDATSAN"); } catch { }
                
                return Json(new { success = true, message = "Đổi sân thành công!" });
            }
            catch (Exception ex)
            {
                // Dọn dẹp
                try { _db.ExecuteNonQuery("ENABLE TRIGGER trg_KiemTraThoiLuongDat ON PHIEUDATSAN"); } catch { }
                return Json(new { success = false, message = "Lỗi: " + ex.Message });
            }
        }
        [HttpPost]
        public IActionResult DoiLichDat(long maDatSan, DateTime ngayMoi, TimeSpan gioBatDauMoi, TimeSpan gioKetThucMoi)
        {
            var maUser = HttpContext.Session.GetString("MaUser");
            if (string.IsNullOrEmpty(maUser))
            {
                 return Json(new { success = false, message = "Vui lòng đăng nhập!" });
            }

            try
            {
                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@MaDatSan", maDatSan),
                    new SqlParameter("@MaKH", maUser),
                    new SqlParameter("@NgayMoi", ngayMoi),
                    new SqlParameter("@GioBDMoi", gioBatDauMoi),
                    new SqlParameter("@GioKTMoi", gioKetThucMoi)
                };

                _db.ExecuteNonQuery("sp_DoiLichDat", parameters);
                return Json(new { success = true, message = "Đổi lịch thành công!" });
            }
            catch (SqlException ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi hệ thống: " + ex.Message });
            }
        }
    }
}
