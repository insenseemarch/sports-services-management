using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using webapp_mvc.Models;

namespace webapp_mvc.Controllers
{
    public class DatSanThanhToanController : Controller
    {
        private readonly DatabaseHelper _db;
        private readonly IConfiguration _configuration;

        public DatSanThanhToanController(IConfiguration configuration)
        {
            _configuration = configuration;
            _db = new DatabaseHelper(configuration);
        }

        public IActionResult Index(long maDatSan)
        {
            var maUser = HttpContext.Session.GetString("MaUser");
            if (string.IsNullOrEmpty(maUser))
            {
                return RedirectToAction("DangNhap", "TaiKhoan");
            }

            var vaiTro = HttpContext.Session.GetString("VaiTro")?.Trim();
            // CHỐT CHẶN: Nếu là Nhân viên -> Đá sang StaffPreview ngay lập tức
            if (!string.IsNullOrEmpty(vaiTro) && !string.Equals(vaiTro, "Khách hàng", StringComparison.OrdinalIgnoreCase))
            {
                 return RedirectToAction("StaffPreview", new { maDatSan = maDatSan });
            }

            var model = new ThanhToanKhachHangViewModel
            {
                MaDatSan = maDatSan
            };

            // Lấy thông tin phiếu đặt sân
            string sqlPhieu = @"
                SELECT p.MaDatSan, p.MaKH, p.NgayDat, p.GioBatDau, p.GioKetThuc, p.TrangThai,
                       s.MaSan, ls.TenLS, ls.DVT, cs.TenCS
                FROM PHIEUDATSAN p
                LEFT JOIN DATSAN d ON p.MaDatSan = d.MaDatSan
                LEFT JOIN SAN s ON d.MaSan = s.MaSan
                LEFT JOIN LOAISAN ls ON s.MaLS = ls.MaLS
                LEFT JOIN COSO cs ON s.MaCS = cs.MaCS
                WHERE p.MaDatSan = @MaDatSan";

            // Nếu là Khách hàng thì mới bắt buộc check chính chủ. Nhân viên xem thoải mái.
            if (string.Equals(vaiTro, "Khách hàng", StringComparison.OrdinalIgnoreCase))
            {
                 sqlPhieu += " AND p.MaKH = @MaKH";
            }

            var dtPhieu = _db.ExecuteQuery(sqlPhieu, 
                new SqlParameter("@MaDatSan", maDatSan),
                new SqlParameter("@MaKH", maUser));

            if (dtPhieu.Rows.Count == 0)
            {
                TempData["ErrorMessage"] = "Không tìm thấy phiếu đặt!";
                return RedirectToAction("Index", "Home");
            }

            var row = dtPhieu.Rows[0];
            model.MaSan = row["MaSan"] != DBNull.Value ? row["MaSan"].ToString() : "";
            model.TenSan = row["MaSan"] != DBNull.Value ? $"{row["TenLS"]} - {row["MaSan"]}" : "Dịch vụ lẻ";
            model.TenCoSo = row["TenCS"].ToString();
            model.NgayDat = Convert.ToDateTime(row["NgayDat"]);
            model.GioBatDau = (TimeSpan)row["GioBatDau"];
            model.GioKetThuc = (TimeSpan)row["GioKetThuc"];

            // Lấy tên khách hàng từ phiếu đặt (p.MaKH)
            string bookingMaKH = row["MaKH"].ToString();
            string sqlInfoKH = "SELECT HoTen, SDT FROM KHACHHANG WHERE MaKH = @MaKH";
            var dtInfo = _db.ExecuteQuery(sqlInfoKH, new SqlParameter("@MaKH", bookingMaKH));
            
            if (dtInfo.Rows.Count > 0)
            {
                ViewBag.TenKhachHang = dtInfo.Rows[0]["HoTen"].ToString();
                ViewBag.SDTKhachHang = dtInfo.Rows[0]["SDT"].ToString();
            }
            else
            {
                ViewBag.TenKhachHang = "Khách vãng lai";
                ViewBag.SDTKhachHang = "";
            }

            
            // Calc detailed info
            model.DonViTinh = row["DVT"] != DBNull.Value ? row["DVT"].ToString() : "Giờ";
            
            // Calculate Duration & Quantity
            double durationMins = (model.GioKetThuc - model.GioBatDau).TotalMinutes;
            string courtType = row["TenLS"] != DBNull.Value ? row["TenLS"].ToString().ToLower() : "";
            
            if (courtType.Contains("bóng đá") || courtType.Contains("mini"))
            {
                model.SoLuongSan = (decimal)(durationMins / 90.0);
                model.DonViTinh = "Trận (90p)";
            } 
            else if (courtType.Contains("tennis") || courtType.Contains("ca"))
            {
                model.SoLuongSan = (decimal)(durationMins / 120.0);
                 model.DonViTinh = "Ca (2h)";
            }
            else
            {
                model.SoLuongSan = (decimal)(durationMins / 60.0);
                 model.DonViTinh = "Giờ";
            }
            // Round nicely logic handled in UI mostly, but here keep decimal precision
            
            // Tính tiền sân thực tế từ Database Function
            string sqlTienSan = "SELECT dbo.f_TinhTienSan(@MaDatSan)";
            model.TienSan = _db.ExecuteScalar<decimal>(sqlTienSan, new SqlParameter("@MaDatSan", maDatSan));
            
            // Calc Unit Price based on total
            if (model.SoLuongSan > 0)
                model.DonGiaSan = model.TienSan / model.SoLuongSan;
            else 
                model.DonGiaSan = 0;

            // Lấy danh sách dịch vụ đã chọn
            string sqlDichVu = @"
                SELECT ld.TenLoai as TenDV, ct.SoLuong, ct.SoGioThue, ct.ThanhTien
                FROM CT_DICHVUDAT ct
                JOIN DICHVU dv ON ct.MaDV = dv.MaDV
                JOIN LOAIDV ld ON dv.MaLoaiDV = ld.MaLoaiDV
                WHERE ct.MaDatSan = @MaDatSan";

            var dtDichVu = _db.ExecuteQuery(sqlDichVu, 
                new SqlParameter("@MaDatSan", maDatSan));

            model.DanhSachDichVu = new List<DichVuDatItem>();
            decimal tongTienDichVu = 0;

            foreach (DataRow dvRow in dtDichVu.Rows)
            {
                var thanhTien = Convert.ToDecimal(dvRow["ThanhTien"]);
                tongTienDichVu += thanhTien;

                model.DanhSachDichVu.Add(new DichVuDatItem
                {
                    TenDV = dvRow["TenDV"].ToString(), // Fixed column alias
                    SoLuong = Convert.ToInt32(dvRow["SoLuong"]),
                    SoGioThue = dvRow["SoGioThue"] != DBNull.Value ? Convert.ToInt32(dvRow["SoGioThue"]) : 0,
                    ThanhTien = thanhTien
                });
            }

            model.TienDichVu = tongTienDichVu;
            model.TongCong = model.TienSan + model.TienDichVu;

            // Áp dụng ưu đãi thành viên
            string sqlCapBac = @"
                SELECT cb.UuDai 
                FROM KHACHHANG kh 
                JOIN CAPBAC cb ON kh.MaCB = cb.MaCB 
                WHERE kh.MaKH = @MaKH";
            
            var dtCapBac = _db.ExecuteQuery(sqlCapBac, new SqlParameter("@MaKH", maUser));
            if (dtCapBac.Rows.Count > 0)
            {
                var tyLeUuDai = Convert.ToDecimal(dtCapBac.Rows[0]["UuDai"]);
                model.GiamGia = model.TongCong * tyLeUuDai;
            }

            model.ThanhTien = model.TongCong - model.GiamGia;

            return View(model);
        }

        // POST: Thanh toán Online
        [HttpPost]
        public IActionResult ThanhToanOnline(long maDatSan, string phuongThuc)
        {
            var maUser = HttpContext.Session.GetString("MaUser");
            if (string.IsNullOrEmpty(maUser))
            {
                return Json(new { success = false, message = "Vui lòng đăng nhập!" });
            }

            try
            {
                // Gọi SP thanh toán
                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@MaDatSan", maDatSan),
                    new SqlParameter("@NguoiLap", DBNull.Value),
                    new SqlParameter("@HinhThucTT", phuongThuc),
                    new SqlParameter("@MaUD", DBNull.Value)
                };

                _db.ExecuteNonQuery("sp_ThanhToanOnline", parameters);

                return Json(new { 
                    success = true, 
                    message = "Thanh toán thành công!",
                    redirectUrl = Url.Action("XacNhan", new { maDatSan = maDatSan, loai = "online" })
                });
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

        [HttpPost]
        public IActionResult ThanhToanTaiQuay(long maDatSan)
        {
            var maUser = HttpContext.Session.GetString("MaUser");
            if (string.IsNullOrEmpty(maUser))
            {
                return Json(new { success = false, message = "Vui lòng đăng nhập!" });
            }

            try
            {
                // Gọi SP xử lý (Update từ Nháp -> Chờ thanh toán + Double Check)
                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@MaDatSan", maDatSan)
                };

                _db.ExecuteNonQuery("sp_ThanhToanTaiQuay", parameters);
                
                return Json(new { 
                    success = true, 
                    message = "Đã đặt chỗ! Vui lòng thanh toán tại quầy trong vòng 30 phút.",
                    redirectUrl = Url.Action("XacNhan", new { maDatSan = maDatSan, loai = "taiquay" })
                });
            }
            catch (SqlException ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi: " + ex.Message });
            }
        }

        // GET: Trang xác nhận
        public IActionResult XacNhan(long maDatSan, string loai = "")
        {
            var maUser = HttpContext.Session.GetString("MaUser");
            if (string.IsNullOrEmpty(maUser))
            {
                return RedirectToAction("DangNhap", "TaiKhoan");
            }

            ViewBag.MaDatSan = maDatSan;
            ViewBag.LoaiThanhToan = loai; // "online" hoặc "taiquay"
            
           // Lấy thông tin phiếu để hiển thị
            string sql = @"
                SELECT p.TrangThai, p.NgayDat, p.GioBatDau, s.MaSan, cs.TenCS
                FROM PHIEUDATSAN p
                JOIN DATSAN d ON p.MaDatSan = d.MaDatSan
                JOIN SAN s ON d.MaSan = s.MaSan
                JOIN COSO cs ON s.MaCS = cs.MaCS
                WHERE p.MaDatSan = @MaDatSan AND p.MaKH = @MaKH";
            
            var dt = _db.ExecuteQuery(sql,
                new SqlParameter("@MaDatSan", maDatSan),
                new SqlParameter("@MaKH", maUser));
            
            if (dt.Rows.Count > 0)
            {
                ViewBag.TrangThai = dt.Rows[0]["TrangThai"].ToString();
                ViewBag.NgayDat = Convert.ToDateTime(dt.Rows[0]["NgayDat"]);
                ViewBag.GioBatDau = (TimeSpan)dt.Rows[0]["GioBatDau"];
                ViewBag.TenSan = dt.Rows[0]["MaSan"].ToString();
                ViewBag.TenCoSo = dt.Rows[0]["TenCS"].ToString();
            }
            
            return View();
        }
        // GET: Xem trước phiếu đặt sân (Giao diện dành cho Nhân viên)
        // GET: Xem trước phiếu đặt sân (Giao diện dành cho Nhân viên)
        public IActionResult StaffPreview(long maDatSan)
        {
            try 
            {
                var maUser = HttpContext.Session.GetString("MaUser");
                if (string.IsNullOrEmpty(maUser)) return RedirectToAction("DangNhap", "TaiKhoan");

                var model = new ThanhToanKhachHangViewModel { MaDatSan = maDatSan };

                // Dùng SQL cơ bản nhất để tránh lỗi thiếu cột (NguoiLap, NgayTao)
                string sqlPhieu = @"
                    SELECT p.MaDatSan, p.MaKH, p.NgayDat, p.GioBatDau, p.GioKetThuc, p.TrangThai,
                           s.MaSan, ls.TenLS, ls.DVT, cs.TenCS
                    FROM PHIEUDATSAN p
                    LEFT JOIN DATSAN d ON p.MaDatSan = d.MaDatSan
                    LEFT JOIN SAN s ON d.MaSan = s.MaSan
                    LEFT JOIN LOAISAN ls ON s.MaLS = ls.MaLS
                    LEFT JOIN COSO cs ON s.MaCS = cs.MaCS
                    WHERE p.MaDatSan = @MaDatSan";

                var dtPhieu = _db.ExecuteQuery(sqlPhieu, new SqlParameter("@MaDatSan", maDatSan));
                if (dtPhieu.Rows.Count == 0) 
                {
                    return Content($"LỖI CRITICAL: Không tìm thấy phiếu đặt sân với ID = {maDatSan} trong cơ sở dữ liệu.\nNgười dùng hiện tại: {maUser}.\nHãy kiểm tra lại bảng PHIEUDATSAN xem có dòng nào có MaDatSan = {maDatSan} không.");
                }

                var row = dtPhieu.Rows[0];
                model.MaSan = row["MaSan"]?.ToString() ?? "";
                model.TenSan = row["MaSan"] != DBNull.Value ? $"{row["TenLS"]} - {row["MaSan"]}" : "Dịch vụ lẻ";
                model.TenCoSo = row["TenCS"]?.ToString() ?? "";
                model.NgayDat = Convert.ToDateTime(row["NgayDat"]);
                model.GioBatDau = (TimeSpan)row["GioBatDau"];
                model.GioKetThuc = (TimeSpan)row["GioKetThuc"];

                // Lấy tên Khách Hàng
                string maKH = row["MaKH"].ToString();
                var dtKH = _db.ExecuteQuery("SELECT HoTen, SDT FROM KHACHHANG WHERE MaKH = @MaKH", new SqlParameter("@MaKH", maKH));
                if (dtKH.Rows.Count > 0)
                {
                    ViewBag.TenKhachHang = dtKH.Rows[0]["HoTen"].ToString();
                    ViewBag.SDTKhachHang = dtKH.Rows[0]["SDT"].ToString();
                }
                else
                {
                    ViewBag.TenKhachHang = "Khách vãng lai";
                    ViewBag.SDTKhachHang = "---";
                }

                // Lấy tên Người Lập (Tạm thời lấy currentUser nếu DB thiếu cột NguoiLap)
                ViewBag.TenNguoiLap = "Nhân viên"; // Default
                if (row.Table.Columns.Contains("NguoiLap") && row["NguoiLap"] != DBNull.Value)
                {
                   string maNguoiLap = row["NguoiLap"].ToString();
                   var dtNV = _db.ExecuteQuery("SELECT HoTen FROM NHANVIEN WHERE MaNV = @MaNV", new SqlParameter("@MaNV", maNguoiLap));
                   if(dtNV.Rows.Count > 0) ViewBag.TenNguoiLap = dtNV.Rows[0]["HoTen"].ToString();
                }
                else
                {
                    // Fallback: Tìm tên nhân viên hiện tại
                     var dtNV = _db.ExecuteQuery("SELECT HoTen FROM NHANVIEN WHERE MaNV = @MaNV", new SqlParameter("@MaNV", maUser));
                     if(dtNV.Rows.Count > 0) ViewBag.TenNguoiLap = dtNV.Rows[0]["HoTen"].ToString();
                }

                // Ngày lập phiếu (Default Now nếu thiếu cột)
                ViewBag.NgayLap = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
                if (row.Table.Columns.Contains("NgayTao") && row["NgayTao"] != DBNull.Value)
                {
                    ViewBag.NgayLap = Convert.ToDateTime(row["NgayTao"]).ToString("dd/MM/yyyy HH:mm");
                }

            // TÍNH TOÁN TIỀN
            // 1. Tiền Sân
            double durationMins = (model.GioKetThuc - model.GioBatDau).TotalMinutes;
            string courtType = row["TenLS"]?.ToString().ToLower() ?? "";
            
            if (courtType.Contains("bóng đá") || courtType.Contains("mini"))
            {
                model.SoLuongSan = (decimal)(durationMins / 90.0);
                model.DonViTinh = "Trận (90p)";
            } 
            else if (courtType.Contains("tennis"))
            {
                model.SoLuongSan = (decimal)(durationMins / 120.0);
                 model.DonViTinh = "Ca (2h)";
            }
            else
            {
                model.SoLuongSan = (decimal)(durationMins / 60.0);
                 model.DonViTinh = "Giờ";
            }

            string sqlTienSan = "SELECT dbo.f_TinhTienSan(@MaDatSan)";
            model.TienSan = _db.ExecuteScalar<decimal>(sqlTienSan, new SqlParameter("@MaDatSan", maDatSan));
            if (model.SoLuongSan > 0) model.DonGiaSan = model.TienSan / model.SoLuongSan;

            // 2. Tiền Dịch Vụ
            string sqlDichVu = @"
                SELECT ld.TenLoai as TenDV, ct.SoLuong, ct.ThanhTien
                FROM CT_DICHVUDAT ct
                JOIN DICHVU dv ON ct.MaDV = dv.MaDV
                JOIN LOAIDV ld ON dv.MaLoaiDV = ld.MaLoaiDV
                WHERE ct.MaDatSan = @MaDatSan";

            var dtDichVu = _db.ExecuteQuery(sqlDichVu, new SqlParameter("@MaDatSan", maDatSan));
            model.DanhSachDichVu = new List<DichVuDatItem>();
            foreach (DataRow r in dtDichVu.Rows)
            {
                model.DanhSachDichVu.Add(new DichVuDatItem
                {
                    TenDV = r["TenDV"].ToString(),
                    SoLuong = Convert.ToInt32(r["SoLuong"]),
                    ThanhTien = Convert.ToDecimal(r["ThanhTien"])
                });
            }
             model.TienDichVu = model.DanhSachDichVu.Sum(x => x.ThanhTien);
             model.TongCong = model.TienSan + model.TienDichVu;
             model.ThanhTien = model.TongCong; // Nhân viên thì hiện tổng trước, chưa trừ ưu đãi (hoặc có thể trừ luôn tùy logic) - Hiện tại giữ nguyên tổng

             return View(model);
            }
            catch (Exception ex)
            {
                return Content("Error loading staff preview: " + ex.Message);
            }
        }

        // GET: Partial View cho Modal (Phiếu đặt sân)
        public IActionResult StaffPreviewPartial(long maDatSan)
        {
            try 
            {
                var maUser = HttpContext.Session.GetString("MaUser");
                // Không redirect ở đây, nếu lỗi return Empty partial hoặc Content lỗi
                if (string.IsNullOrEmpty(maUser)) return Content("Session Expired");

                var model = new ThanhToanKhachHangViewModel { MaDatSan = maDatSan };

                string sqlPhieu = @"
                    SELECT p.MaDatSan, p.MaKH, p.NgayDat, p.GioBatDau, p.GioKetThuc, p.TrangThai,
                           s.MaSan, ls.TenLS, ls.DVT, cs.TenCS
                    FROM PHIEUDATSAN p
                    LEFT JOIN DATSAN d ON p.MaDatSan = d.MaDatSan
                    LEFT JOIN SAN s ON d.MaSan = s.MaSan
                    LEFT JOIN LOAISAN ls ON s.MaLS = ls.MaLS
                    LEFT JOIN COSO cs ON s.MaCS = cs.MaCS
                    WHERE p.MaDatSan = @MaDatSan";

                var dtPhieu = _db.ExecuteQuery(sqlPhieu, new SqlParameter("@MaDatSan", maDatSan));
                if (dtPhieu.Rows.Count == 0) return Content("Booking Not Found");

                var row = dtPhieu.Rows[0];
                model.MaSan = row["MaSan"]?.ToString() ?? "";
                model.TenSan = row["MaSan"] != DBNull.Value ? $"{row["TenLS"]} - {row["MaSan"]}" : "Dịch vụ lẻ";
                model.TenCoSo = row["TenCS"]?.ToString() ?? "";
                model.NgayDat = Convert.ToDateTime(row["NgayDat"]);
                model.GioBatDau = (TimeSpan)row["GioBatDau"];
                model.GioKetThuc = (TimeSpan)row["GioKetThuc"];

                string maKH = row["MaKH"].ToString();
                var dtKH = _db.ExecuteQuery("SELECT HoTen, SDT FROM KHACHHANG WHERE MaKH = @MaKH", new SqlParameter("@MaKH", maKH));
                if (dtKH.Rows.Count > 0)
                {
                    ViewBag.TenKhachHang = dtKH.Rows[0]["HoTen"].ToString();
                    ViewBag.SDTKhachHang = dtKH.Rows[0]["SDT"].ToString();
                }
                else
                {
                    ViewBag.TenKhachHang = "Khách vãng lai";
                    ViewBag.SDTKhachHang = "---";
                }

                // Lấy tên Người Lập (Tạm thời lấy currentUser nếu DB thiếu cột NguoiLap)
                ViewBag.TenNguoiLap = "Nhân viên"; 
                if (row.Table.Columns.Contains("NguoiLap") && row["NguoiLap"] != DBNull.Value)
                {
                   string maNguoiLap = row["NguoiLap"].ToString();
                   var dtNV = _db.ExecuteQuery("SELECT HoTen FROM NHANVIEN WHERE MaNV = @MaNV", new SqlParameter("@MaNV", maNguoiLap));
                   if(dtNV.Rows.Count > 0) ViewBag.TenNguoiLap = dtNV.Rows[0]["HoTen"].ToString();
                }
                else
                {
                     var dtNV = _db.ExecuteQuery("SELECT HoTen FROM NHANVIEN WHERE MaNV = @MaNV", new SqlParameter("@MaNV", maUser));
                     if(dtNV.Rows.Count > 0) ViewBag.TenNguoiLap = dtNV.Rows[0]["HoTen"].ToString();
                }

                ViewBag.NgayLap = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
                if (row.Table.Columns.Contains("NgayTao") && row["NgayTao"] != DBNull.Value)
                {
                    ViewBag.NgayLap = Convert.ToDateTime(row["NgayTao"]).ToString("dd/MM/yyyy HH:mm");
                }

                // Tính tiền (Logic y hệt hàm trên)
                double durationMins = (model.GioKetThuc - model.GioBatDau).TotalMinutes;
                string courtType = row["TenLS"]?.ToString().ToLower() ?? "";
            
                if (courtType.Contains("bóng đá") || courtType.Contains("mini")) { model.SoLuongSan = (decimal)(durationMins / 90.0); model.DonViTinh = "Trận (90p)"; } 
                else if (courtType.Contains("tennis")) { model.SoLuongSan = (decimal)(durationMins / 120.0); model.DonViTinh = "Ca (2h)"; }
                else { model.SoLuongSan = (decimal)(durationMins / 60.0); model.DonViTinh = "Giờ"; }

                string sqlTienSan = "SELECT dbo.f_TinhTienSan(@MaDatSan)";
                model.TienSan = _db.ExecuteScalar<decimal>(sqlTienSan, new SqlParameter("@MaDatSan", maDatSan));
                if (model.SoLuongSan > 0) model.DonGiaSan = model.TienSan / model.SoLuongSan;

                string sqlDichVu = @"
                    SELECT ld.TenLoai as TenDV, ct.SoLuong, ct.ThanhTien
                    FROM CT_DICHVUDAT ct
                    JOIN DICHVU dv ON ct.MaDV = dv.MaDV
                    JOIN LOAIDV ld ON dv.MaLoaiDV = ld.MaLoaiDV
                    WHERE ct.MaDatSan = @MaDatSan";

                var dtDichVu = _db.ExecuteQuery(sqlDichVu, new SqlParameter("@MaDatSan", maDatSan));
                model.DanhSachDichVu = new List<DichVuDatItem>();
                foreach (DataRow r in dtDichVu.Rows)
                {
                    model.DanhSachDichVu.Add(new DichVuDatItem {
                        TenDV = r["TenDV"].ToString(),
                        SoLuong = Convert.ToInt32(r["SoLuong"]),
                        ThanhTien = Convert.ToDecimal(r["ThanhTien"])
                    });
                }
                model.TienDichVu = model.DanhSachDichVu.Sum(x => x.ThanhTien);
                model.TongCong = model.TienSan + model.TienDichVu;
                model.ThanhTien = model.TongCong;

                return PartialView("_StaffReceipt", model);
            }
            catch (Exception ex)
            {
                return Content("Error loading receipt: " + ex.Message);
            }
        }

        // POST: Xử lý nút bấm của Nhân viên
        [HttpPost]
        public IActionResult StaffAction(long maDatSan, string actionType)
        {
            try
            {
                if (actionType == "confirm")
                {
                    // Nút "Tạo phiếu đặt sân": Xác nhận phiếu (Chuyển trạng thái Nháp -> Đã đặt/Chờ thanh toán)
                    _db.ExecuteNonQuery("UPDATE PHIEUDATSAN SET TrangThai = N'Chờ thanh toán' WHERE MaDatSan = @Id", new SqlParameter("@Id", maDatSan));
                    return Json(new { success = true, message = "Đã tạo phiếu thành công!", redirectUrl = Url.Action("Index", "NhanVien") }); // Về trang chủ NV hoặc Lịch làm việc
                }
                else if (actionType == "send_cashier")
                {
                    // Nút "Chuyển sang thu ngân": Có thể giống confirm nhưng có thông báo khác, hoặc update trạng thái đặc biệt
                    _db.ExecuteNonQuery("UPDATE PHIEUDATSAN SET TrangThai = N'Chờ thanh toán' WHERE MaDatSan = @Id", new SqlParameter("@Id", maDatSan));
                    // Trong thực tế có thể bắn noti cho thu ngân, ở đây tạm update DB
                    return Json(new { success = true, message = "Đã chuyển phiếu sang bộ phận thu ngân!", redirectUrl = Url.Action("Index", "NhanVien") });
                }
            }
            catch(Exception ex)
            {
                return Json(new { success = false, message = "Lỗi: " + ex.Message });
            }
             return Json(new { success = false, message = "Hành động không hợp lệ" });
        }
    }
}
