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
                SELECT P.MaDatSan, D.MaSan, P.NgayDat, P.GioBatDau, P.GioKetThuc, 
                       (dbo.f_TinhTienSan(P.MaDatSan) + dbo.f_TinhTienDichVu(P.MaDatSan)) as ThanhTien,
                       P.TrangThai,
                       ISNULL(P.NgayTao, P.NgayDat) as NgayTao,
                       DATEDIFF(SECOND, GETDATE(), DATEADD(MINUTE, 30, ISNULL(P.NgayTao, P.NgayDat))) as RemainingSeconds
                FROM PHIEUDATSAN P
                JOIN DATSAN D ON P.MaDatSan = D.MaDatSan
                WHERE P.MaKH = @MaKH 
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
                
                if (row["TrangThai"].ToString() == "Nháp") continue; // Skip Drafts

                var item = new PhieuDatItem
                {
                    MaDatSan = Convert.ToInt64(row["MaDatSan"]),
                    MaSan = row["MaSan"].ToString(),
                    NgayDat = Convert.ToDateTime(row["NgayDat"]),
                    GioBatDau = gioBatDau,
                    GioKetThuc = gioKetThuc,
                    ThanhTien = Convert.ToDecimal(row["ThanhTien"]),
                    TrangThai = row["TrangThai"].ToString(),
                    NgayTao = row["NgayTao"] != DBNull.Value ? Convert.ToDateTime(row["NgayTao"]) : (DateTime?)null
                };

                // Use SQL-calculated Remaining Seconds to avoid Timezone issues
                if (row["RemainingSeconds"] != DBNull.Value && item.TrangThai == "Chờ thanh toán")
                {
                     item.RemainingSeconds = Convert.ToDouble(row["RemainingSeconds"]);
                }
                else
                {
                     item.RemainingSeconds = 0;
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
                // 1. Kiểm tra khung giờ hoạt động (6h-22h)
                if (gioBatDauMoi < new TimeSpan(6, 0, 0) || gioKetThucMoi > new TimeSpan(22, 0, 0))
                {
                    return Json(new { success = false, message = "Thời gian đặt phải trong khung giờ hoạt động (6h-22h)!" });
                }

                // 2. Lấy thông tin loại sân để kiểm tra thời lượng
                string getCourtTypeSql = @"
                    SELECT ls.TenLS
                    FROM PHIEUDATSAN p
                    JOIN DATSAN d ON p.MaDatSan = d.MaDatSan
                    JOIN SAN s ON d.MaSan = s.MaSan
                    JOIN LOAISAN ls ON s.MaLS = ls.MaLS
                    WHERE p.MaDatSan = @MaDatSan";
                
                var dtCourtType = _db.ExecuteQuery(getCourtTypeSql, new SqlParameter("@MaDatSan", maDatSan));
                if (dtCourtType.Rows.Count == 0)
                {
                    return Json(new { success = false, message = "Không tìm thấy thông tin phiếu đặt!" });
                }

                string tenLoaiSan = dtCourtType.Rows[0]["TenLS"].ToString().ToLower();
                int thoiLuongPhut = (int)(gioKetThucMoi - gioBatDauMoi).TotalMinutes;

                // 3. Kiểm tra thời lượng theo loại sân
                if (tenLoaiSan.Contains("bóng đá") || tenLoaiSan.Contains("mini"))
                {
                    if (thoiLuongPhut % 90 != 0 || thoiLuongPhut < 90)
                    {
                        return Json(new { success = false, message = "Sân bóng đá mini phải đặt theo bội số của 90 phút (1 trận = 90 phút)!" });
                    }
                }
                else if (tenLoaiSan.Contains("tennis"))
                {
                    if (thoiLuongPhut % 120 != 0)
                    {
                        return Json(new { success = false, message = "Sân Tennis phải đặt theo bội số của 2 giờ (120 phút)!" });
                    }
                }
                else if (tenLoaiSan.Contains("cầu lông") || tenLoaiSan.Contains("bóng rổ"))
                {
                    if (thoiLuongPhut % 60 != 0)
                    {
                        return Json(new { success = false, message = "Sân Cầu lông/Bóng rổ phải đặt theo bội số của 1 giờ!" });
                    }
                }

                // 4. Kiểm tra trùng giờ (Logic SQL thuần túy, không qua SP)
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
                
                return Json(new { success = true, message = "Đổi sân thành công!" });
            }
            catch (Exception ex)
            {
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
