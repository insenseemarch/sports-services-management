using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using webapp_mvc.Models;

namespace webapp_mvc.Controllers
{
    public class HoSoController : Controller
    {
        private readonly DatabaseHelper _db;

        public HoSoController(DatabaseHelper db)
        {
            _db = db;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var maUser = HttpContext.Session.GetString("MaUser");
            if (string.IsNullOrEmpty(maUser)) return RedirectToAction("DangNhap", "TaiKhoan");

            var model = GetHoSoModel(maUser);
            // Check for success message from redirect
            if (TempData["SuccessMessage"] != null)
            {
                ViewBag.Message = TempData["SuccessMessage"];
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Edit()
        {
            var maUser = HttpContext.Session.GetString("MaUser");
            if (string.IsNullOrEmpty(maUser)) return RedirectToAction("DangNhap", "TaiKhoan");

            var model = GetHoSoModel(maUser);
            return View(model);
        }

        [HttpPost]
        public IActionResult Edit(HoSoViewModel model)
        {
            var maUser = HttpContext.Session.GetString("MaUser");
            if (string.IsNullOrEmpty(maUser)) return RedirectToAction("DangNhap", "TaiKhoan");

            // Chỉ cho phép cập nhật thông tin cá nhân cơ bản
            string sql = @"UPDATE KHACHHANG 
                           SET HoTen=@HoTen, SDT=@SDT, Email=@Email, DiaChi=@DiaChi, 
                               NgaySinh=@NgaySinh, CCCD=@CCCD 
                           WHERE MaKH=@MaKH";
            
            _db.ExecuteNonQuery(sql,
                new SqlParameter("@HoTen", model.HoTen),
                new SqlParameter("@SDT", model.SDT),
                new SqlParameter("@Email", model.Email),
                new SqlParameter("@DiaChi", model.DiaChi ?? ""),
                new SqlParameter("@NgaySinh", (object)model.NgaySinh ?? DBNull.Value),
                new SqlParameter("@CCCD", model.CCCD ?? ""),
                new SqlParameter("@MaKH", maUser)
            );
            
            TempData["SuccessMessage"] = "Cập nhật hồ sơ thành công!";
            return RedirectToAction("Index");
        }

        private HoSoViewModel GetHoSoModel(string maUser)
        {
            var model = new HoSoViewModel();
            // JOIN CAPBAC để lấy tên cấp bậc
            string query = @"
                SELECT K.*, C.TenCB 
                FROM KHACHHANG K
                LEFT JOIN CAPBAC C ON K.MaCB = C.MaCB
                WHERE K.MaKH = @MaKH";

            var dt = _db.ExecuteQuery(query, new SqlParameter("@MaKH", maUser));
            if (dt.Rows.Count > 0)
            {
                var row = dt.Rows[0];
                model.HoTen = row["HoTen"].ToString();
                model.SDT = row["SDT"].ToString();
                model.Email = row["Email"].ToString();
                model.DiaChi = row["DiaChi"].ToString();
                
                model.NgaySinh = row["NgaySinh"] == DBNull.Value ? null : Convert.ToDateTime(row["NgaySinh"]);
                model.CCCD = row["CCCD"].ToString();
                model.LaHSSV = row["LaHSSV"] != DBNull.Value && Convert.ToBoolean(row["LaHSSV"]);
                model.DiemTichLuy = row["DiemTichLuy"] != DBNull.Value ? Convert.ToInt32(row["DiemTichLuy"]) : 0;
                model.MaCB = row["MaCB"].ToString();
                model.TenCB = row["TenCB"].ToString(); // Lấy từ bảng CAPBAC
            }
            return model;
        }
    }
}
