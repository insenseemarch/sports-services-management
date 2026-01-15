using Microsoft.AspNetCore.Mvc;
using webapp_mvc.Models;
using System.Data.SqlClient;

namespace webapp_mvc.Controllers
{
    public class HomeStaffController : Controller
    {
        private readonly DatabaseHelper _db;
        private readonly ILogger<HomeStaffController> _logger;

        public HomeStaffController(IConfiguration configuration, ILogger<HomeStaffController> logger)
        {
            _db = new DatabaseHelper(configuration);
            _logger = logger;
        }

        // GET: /HomeStaff/Index
        public IActionResult Index()
        {
            // Check if user is logged in
            var vaiTro = HttpContext.Session.GetString("VaiTro");
            var hoTen = HttpContext.Session.GetString("HoTen");
            var maNV = HttpContext.Session.GetString("MaUser");

            if (string.IsNullOrEmpty(vaiTro))
            {
                return RedirectToAction("DangNhap", "TaiKhoan");
            }

            // Only staff can access this area
            if (vaiTro.Equals("Khách hàng", StringComparison.OrdinalIgnoreCase))
            {
                return RedirectToAction("Index", "Home");
            }

            ViewBag.HoTen = hoTen;
            ViewBag.VaiTro = vaiTro;
            ViewBag.MaNV = maNV;

            // Get employee info
            try
            {
                var query = @"
                    SELECT NV.MaNV, NV.HoTen, NV.SDT, NV.Email, NV.DiaChi, 
                           NV.NgaySinh, CS.TenCoSo, NV.Luong
                    FROM NHANVIEN NV
                    LEFT JOIN COSO CS ON NV.MaCS = CS.MaCS
                    WHERE NV.MaNV = @MaNV
                ";
                var result = _db.ExecuteQuery(query, new SqlParameter("@MaNV", maNV));
                if (result.Rows.Count > 0)
                {
                    var row = result.Rows[0];
                    ViewBag.DiaChi = row["DiaChi"];
                    ViewBag.SDT = row["SDT"];
                    ViewBag.CoSo = row["TenCoSo"];
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading employee info");
            }

            return View();
        }

        // GET: /HomeStaff/DatSanTrucTiep (Lễ Tân + Quản Lý)
        public IActionResult DatSanTrucTiep()
        {
            var vaiTro = HttpContext.Session.GetString("VaiTro");
            bool hasAccess = vaiTro?.Equals("Lễ tân", StringComparison.OrdinalIgnoreCase) == true ||
                           vaiTro?.Equals("Quản lý", StringComparison.OrdinalIgnoreCase) == true;
            if (!hasAccess)
            {
                return Unauthorized();
            }
            return RedirectToAction("Index", "DatSanTrucTiep");
        }

        // GET: /HomeStaff/ThanhToan (Thu Ngân + Quản Lý)
        public IActionResult ThanhToan()
        {
            var vaiTro = HttpContext.Session.GetString("VaiTro");
            bool hasAccess = vaiTro?.Equals("Thu ngân", StringComparison.OrdinalIgnoreCase) == true ||
                           vaiTro?.Equals("Quản lý", StringComparison.OrdinalIgnoreCase) == true;
            if (!hasAccess)
            {
                return Unauthorized();
            }
            return RedirectToAction("Index", "ThanhToan");
        }

        // GET: /HomeStaff/BaoTri (Kỹ Thuật + Quản Lý)
        public IActionResult BaoTri()
        {
            var vaiTro = HttpContext.Session.GetString("VaiTro");
            bool hasAccess = vaiTro?.Equals("Kỹ thuật", StringComparison.OrdinalIgnoreCase) == true ||
                           vaiTro?.Equals("Quản lý", StringComparison.OrdinalIgnoreCase) == true;
            if (!hasAccess)
            {
                return Unauthorized();
            }
            return RedirectToAction("Index", "BaoTri");
        }

        // GET: /HomeStaff/QuanLy (Quản Lý)
        public IActionResult QuanLy()
        {
            var vaiTro = HttpContext.Session.GetString("VaiTro");
            if (!vaiTro?.Equals("Quản lý", StringComparison.OrdinalIgnoreCase) ?? true)
            {
                return Unauthorized();
            }
            return View();
        }

        // GET: /HomeStaff/CauHinh (IT Admin)
        public IActionResult CauHinh()
        {
            var vaiTro = HttpContext.Session.GetString("VaiTro");
            if (!vaiTro?.Equals("IT", StringComparison.OrdinalIgnoreCase) ?? true)
            {
                return Unauthorized();
            }
            return RedirectToAction("Index", "CauHinhHeThong");
        }
    }
}
