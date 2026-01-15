using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using webapp_mvc.Models;
using webapp_mvc.Models.ViewModels;

namespace webapp_mvc.Controllers
{
    public class BaoTriController : Controller
    {
        private readonly DatabaseHelper _db;

        public BaoTriController(DatabaseHelper db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            // Kiểm tra đăng nhập
            var maUser = HttpContext.Session.GetString("MaUser");
            var vaiTro = HttpContext.Session.GetString("VaiTro");
            
            if (string.IsNullOrEmpty(maUser) || vaiTro != "Kỹ thuật")
            {
                return RedirectToAction("DangNhap", "TaiKhoan");
            }

            // Lấy danh sách phiếu bảo trì của nhân viên hiện tại
            string sql = @"
                SELECT p.MaPhieu, p.MaSan, p.NgayBatDau, p.NgayKetThucDuKien, 
                       p.NgayKetThucThucTe, p.MoTaSuCo, p.ChiPhi, p.TrangThai,
                       ls.TenLS, cs.TenCS
                FROM PHIEUBAOTRI p
                JOIN SAN s ON p.MaSan = s.MaSan
                JOIN LOAISAN ls ON s.MaLS = ls.MaLS
                JOIN COSO cs ON s.MaCS = cs.MaCS
                WHERE p.MaNV = @MaNV
                ORDER BY p.NgayBatDau DESC";

            var dt = _db.ExecuteQuery(sql, new SqlParameter("@MaNV", maUser));
            ViewBag.DanhSachPhieu = dt;

            return View();
        }

        public IActionResult CapNhat(long maPhieu)
        {
            // Kiểm tra đăng nhập
            var maUser = HttpContext.Session.GetString("MaUser");
            var vaiTro = HttpContext.Session.GetString("VaiTro");
            
            if (string.IsNullOrEmpty(maUser) || vaiTro != "Kỹ thuật")
            {
                return RedirectToAction("DangNhap", "TaiKhoan");
            }

            // Lấy chi tiết phiếu bảo trì và thông tin sân hiện tại
            string sql = @"
                SELECT p.MaPhieu, p.MaSan, p.NgayBatDau, p.NgayKetThucDuKien, 
                       p.NgayKetThucThucTe, p.MoTaSuCo, p.ChiPhi, p.TrangThai,
                       ls.TenLS, cs.TenCS, cs.DiaChi,
                       nv.HoTen as TenNV,
                       s.TinhTrang as TinhTrangSan
                FROM PHIEUBAOTRI p
                JOIN SAN s ON p.MaSan = s.MaSan
                JOIN LOAISAN ls ON s.MaLS = ls.MaLS
                JOIN COSO cs ON s.MaCS = cs.MaCS
                JOIN NHANVIEN nv ON p.MaNV = nv.MaNV
                WHERE p.MaPhieu = @MaPhieu AND p.MaNV = @MaNV";

            var dt = _db.ExecuteQuery(sql, 
                new SqlParameter("@MaPhieu", maPhieu),
                new SqlParameter("@MaNV", maUser));

            if (dt.Rows.Count == 0)
            {
                return NotFound();
            }

            var row = dt.Rows[0];
            var model = new PhieuBaoTriViewModel
            {
                MaPhieu = Convert.ToInt64(row["MaPhieu"]),
                MaSan = row["MaSan"].ToString(),
                NgayBatDau = Convert.ToDateTime(row["NgayBatDau"]),
                NgayKetThucDuKien = row["NgayKetThucDuKien"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(row["NgayKetThucDuKien"]) : null,
                NgayKetThucThucTe = row["NgayKetThucThucTe"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(row["NgayKetThucThucTe"]) : null,
                MoTaSuCo = row["MoTaSuCo"] != DBNull.Value ? row["MoTaSuCo"].ToString() : "",
                ChiPhi = row["ChiPhi"] != DBNull.Value ? Convert.ToDecimal(row["ChiPhi"]) : 0,
                TrangThai = row["TrangThai"].ToString(),
                TenLS = row["TenLS"].ToString(),
                TenCS = row["TenCS"].ToString(),
                DiaChi = row["DiaChi"].ToString(),
                TenNV = row["TenNV"].ToString(),
                TinhTrangSan = row["TinhTrangSan"] != DBNull.Value ? row["TinhTrangSan"].ToString() : ""
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult CapNhat(long maPhieu, string maSan, string trangThaiPhieu, string tinhTrangSan, 
                                     DateTime? ngayKetThucThucTe, decimal chiPhi, string moTaSuCo)
        {
            var maUser = HttpContext.Session.GetString("MaUser");
            if (string.IsNullOrEmpty(maUser)) return RedirectToAction("DangNhap", "TaiKhoan");

            try 
            {
                // 1. Cập nhật phiếu bảo trì
                string updatePhieuSql = @"
                    UPDATE PHIEUBAOTRI 
                    SET TrangThai = @TrangThai,
                        NgayKetThucThucTe = @NgayKetThucThucTe,
                        ChiPhi = @ChiPhi,
                        MoTaSuCo = @MoTaSuCo
                    WHERE MaPhieu = @MaPhieu AND MaNV = @MaNV";

                _db.ExecuteNonQuery(updatePhieuSql,
                    new SqlParameter("@TrangThai", trangThaiPhieu),
                    new SqlParameter("@NgayKetThucThucTe", ngayKetThucThucTe ?? (object)DBNull.Value),
                    new SqlParameter("@ChiPhi", chiPhi),
                    new SqlParameter("@MoTaSuCo", moTaSuCo ?? (object)DBNull.Value),
                    new SqlParameter("@MaPhieu", maPhieu),
                    new SqlParameter("@MaNV", maUser));

                // 2. Cập nhật tình trạng sân
                string updateSanSql = "UPDATE SAN SET TinhTrang = @TinhTrangSan WHERE MaSan = @MaSan";
                _db.ExecuteNonQuery(updateSanSql,
                    new SqlParameter("@TinhTrangSan", tinhTrangSan),
                    new SqlParameter("@MaSan", maSan));

                TempData["Success"] = "Cập nhật phiếu bảo trì thành công!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi cập nhật: " + ex.Message;
                return RedirectToAction("CapNhat", new { maPhieu = maPhieu });
            }
        }
    }
}
