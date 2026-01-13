using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using webapp_mvc.Models;

namespace webapp_mvc.Controllers
{
    public class BaoCaoLoiController : Controller
    {
        private readonly DatabaseHelper _db;

        public BaoCaoLoiController(DatabaseHelper db)
        {
            _db = db;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var model = new BaoCaoLoiViewModel();
            var dt = _db.ExecuteQuery("SELECT MaSan, TenCS FROM SAN s JOIN COSO c ON s.MaCS = c.MaCS");
            foreach (System.Data.DataRow row in dt.Rows)
            {
                model.DanhSachSan.Add(row["MaSan"].ToString() + " - " + row["TenCS"].ToString());
            }
            return View(model);
        }

        [HttpPost]
        public IActionResult Index(BaoCaoLoiViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Lấy User hiện tại
                var maUser = HttpContext.Session.GetString("MaUser");
                var vaiTro = HttpContext.Session.GetString("VaiTro"); // "Khách hàng" hoặc "Nhân viên"

                // Nếu là khách hàng -> MaNV = NULL. Nếu là nhân viên -> MaNV = maUser
                object maNVParams = DBNull.Value;
                if (vaiTro != "Khách hàng" && !string.IsNullOrEmpty(maUser))
                {
                     maNVParams = maUser; 
                }

                string maSan = model.SelectedSan.Split(" - ")[0];
                string sql = @"INSERT INTO PHIEUBAOTRI (MaSan, MaNV, NgayBatDau, MoTaSuCo, TrangThai, ChiPhi)
                               VALUES (@MaSan, @MaNV, GETDATE(), @MoTa, N'Chờ xử lý', 0)";
                
                _db.ExecuteNonQuery(sql, 
                    new SqlParameter("@MaSan", maSan),
                    new SqlParameter("@MaNV", maNVParams),
                    new SqlParameter("@MoTa", model.MoTaSuCo)
                );
                return RedirectToAction("Index"); // Success
            }
            // Reload list
            var dt = _db.ExecuteQuery("SELECT MaSan, TenCS FROM SAN s JOIN COSO c ON s.MaCS = c.MaCS");
            foreach (System.Data.DataRow row in dt.Rows)
            {
                model.DanhSachSan.Add(row["MaSan"].ToString() + " - " + row["TenCS"].ToString());
            }
            return View(model);
        }
    }
}
