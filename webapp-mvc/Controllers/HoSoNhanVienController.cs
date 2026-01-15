using Microsoft.AspNetCore.Mvc;
using webapp_mvc.Models;
using System.Data.SqlClient;

namespace webapp_mvc.Controllers
{
    public class HoSoNhanVienController : Controller
    {
        private readonly DatabaseHelper _db;
        private readonly ILogger<HoSoNhanVienController> _logger;

        public HoSoNhanVienController(IConfiguration configuration, ILogger<HoSoNhanVienController> logger)
        {
            _db = new DatabaseHelper(configuration);
            _logger = logger;
        }

        // GET: /HoSoNhanVien/Index
        public IActionResult Index()
        {
            var maNV = HttpContext.Session.GetString("MaUser");
            if (string.IsNullOrEmpty(maNV))
            {
                return RedirectToAction("DangNhap", "TaiKhoan");
            }

            try
            {
                var query = @"
                    SELECT NV.MaNV, NV.HoTen, NV.CCCD, NV.SDT, NV.Email, NV.DiaChi, 
                           NV.NgaySinh, NV.Luong, CS.TenCoSo, NV.ChucVu
                    FROM NHANVIEN NV
                    LEFT JOIN COSO CS ON NV.MaCS = CS.MaCS
                    WHERE NV.MaNV = @MaNV
                ";

                var result = _db.ExecuteQuery(query, new SqlParameter("@MaNV", maNV));

                if (result.Rows.Count > 0)
                {
                    var row = result.Rows[0];
                    ViewBag.HoTen = row["HoTen"];
                    ViewBag.CCCD = row["CCCD"];
                    ViewBag.SDT = row["SDT"];
                    ViewBag.Email = row["Email"];
                    ViewBag.DiaChi = row["DiaChi"];
                    ViewBag.NgaySinh = row["NgaySinh"];
                    ViewBag.Luong = row["Luong"];
                    ViewBag.TenCoSo = row["TenCoSo"];
                    ViewBag.ChucVu = row["ChucVu"];
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading employee profile");
            }

            return View();
        }

        // POST: /HoSoNhanVien/CaiDatTaiKhoan
        [HttpPost]
        public IActionResult CaiDatTaiKhoan(string email, string sdt, string diaChi)
        {
            var maNV = HttpContext.Session.GetString("MaUser");
            if (string.IsNullOrEmpty(maNV))
            {
                return Json(new { success = false, message = "Vui lòng đăng nhập!" });
            }

            try
            {
                var query = @"
                    UPDATE NHANVIEN 
                    SET Email = @Email, SDT = @SDT, DiaChi = @DiaChi
                    WHERE MaNV = @MaNV
                ";

                _db.ExecuteNonQuery(query, 
                    new SqlParameter("@Email", email ?? ""),
                    new SqlParameter("@SDT", sdt ?? ""),
                    new SqlParameter("@DiaChi", diaChi ?? ""),
                    new SqlParameter("@MaNV", maNV)
                );

                return Json(new { success = true, message = "Cập nhật thông tin thành công!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating profile");
                return Json(new { success = false, message = "Lỗi: " + ex.Message });
            }
        }
    }
}
