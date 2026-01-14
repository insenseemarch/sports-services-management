using Microsoft.AspNetCore.Mvc;

namespace webapp_mvc.Controllers
{
    public class BaoTriController : Controller
    {
        public IActionResult Index()
        {
            var vaiTro = HttpContext.Session.GetString("VaiTro");
            bool hasAccess = vaiTro?.Equals("Kỹ thuật", StringComparison.OrdinalIgnoreCase) == true ||
                           vaiTro?.Equals("Quản lý", StringComparison.OrdinalIgnoreCase) == true;
            if (!hasAccess)
            {
                return Unauthorized();
            }
            return View();
        }
    }

    public class QuanLyNhanSuController : Controller
    {
        public IActionResult Index()
        {
            var vaiTro = HttpContext.Session.GetString("VaiTro");
            if (!vaiTro?.Equals("Quản lý", StringComparison.OrdinalIgnoreCase) ?? true)
            {
                return Unauthorized();
            }
            return View();
        }
    }

    public class PhanCongCaTrucController : Controller
    {
        public IActionResult Index()
        {
            var vaiTro = HttpContext.Session.GetString("VaiTro");
            if (!vaiTro?.Equals("Quản lý", StringComparison.OrdinalIgnoreCase) ?? true)
            {
                return Unauthorized();
            }
            return View();
        }
    }

    public class PheDuyetNghiPhepController : Controller
    {
        public IActionResult Index()
        {
            var vaiTro = HttpContext.Session.GetString("VaiTro");
            if (!vaiTro?.Equals("Quản lý", StringComparison.OrdinalIgnoreCase) ?? true)
            {
                return Unauthorized();
            }
            return View();
        }
    }

    public class BaoCaoThongKeController : Controller
    {
        public IActionResult Index()
        {
            var vaiTro = HttpContext.Session.GetString("VaiTro");
            if (!vaiTro?.Equals("Quản lý", StringComparison.OrdinalIgnoreCase) ?? true)
            {
                return Unauthorized();
            }
            return View();
        }
    }

    public class CauHinhHeThongController : Controller
    {
        public IActionResult Index()
        {
            var vaiTro = HttpContext.Session.GetString("VaiTro");
            if (!vaiTro?.Equals("IT", StringComparison.OrdinalIgnoreCase) ?? true)
            {
                return Unauthorized();
            }
            return View();
        }
    }
}
