using Microsoft.AspNetCore.Mvc;
using webapp_mvc.Models;
using System.Data;
using System.Data.SqlClient;

namespace webapp_mvc.Controllers
{
    public class TaiKhoanController : Controller
    {
        private readonly DatabaseHelper _db;
        private readonly ILogger<TaiKhoanController> _logger;

        public TaiKhoanController(IConfiguration configuration, ILogger<TaiKhoanController> logger)
        {
            _db = new DatabaseHelper(configuration);
            _logger = logger;
        }

        // GET: /TaiKhoan/DangKy
        public IActionResult DangKy()
        {
            return View();
        }

        // POST: /TaiKhoan/DangKy
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DangKy(DangKyViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return Json(new { success = false, message = string.Join("<br>", errors) });
            }

            try
            {
                // check duplicate username
                var checkQuery = "SELECT COUNT(*) FROM TAIKHOAN WHERE TenDangNhap = @Username";
                var count = _db.ExecuteScalar<int>(checkQuery, new SqlParameter("@Username", model.Username));
                
                if (count > 0)
                {
                    return Json(new { success = false, field = "Username", message = "Tên đăng nhập đã tồn tại!" });
                }

                var maTK = "TK" + DateTime.Now.ToString("yyyyMMddHHmmss");
                var maKH = "KH" + DateTime.Now.ToString("yyyyMMddHHmmss");

                var insertTKQuery = @"INSERT INTO TAIKHOAN (MaTK, TenDangNhap, MatKhau, VaiTro, NgayDangKy) VALUES (@MaTK, @Username, @Password, N'Khách hàng', GETDATE())";
                _db.ExecuteNonQuery(insertTKQuery, new SqlParameter("@MaTK", maTK), new SqlParameter("@Username", model.Username), new SqlParameter("@Password", model.Password));

                var insertKHQuery = @"INSERT INTO KHACHHANG (MaKH, HoTen, NgaySinh, CCCD, SDT, Email, DiaChi, LaHSSV, DiemTichLuy, MaCB, MaTK) VALUES (@MaKH, @HoTen, @NgaySinh, @CCCD, @SDT, @Email, @DiaChi, @LaHSSV, 0, 'CB001', @MaTK)";
                _db.ExecuteNonQuery(insertKHQuery,
                    new SqlParameter("@MaKH", maKH), new SqlParameter("@HoTen", model.HoTen),
                    new SqlParameter("@NgaySinh", (object)model.NgaySinh ?? DBNull.Value), new SqlParameter("@CCCD", (object)model.CCCD ?? DBNull.Value),
                    new SqlParameter("@SDT", model.SDT), new SqlParameter("@Email", model.Email),
                    new SqlParameter("@DiaChi", (object)model.DiaChi ?? DBNull.Value), new SqlParameter("@LaHSSV", model.LaHSSV ? 1 : 0),
                    new SqlParameter("@MaTK", maTK)
                );

                return Json(new { success = true, message = "Đăng ký thành công! Vui lòng đăng nhập." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during registration");
                return Json(new { success = false, message = "Có lỗi xảy ra: " + ex.Message });
            }
        }

        // GET: /TaiKhoan/DangNhap
        public IActionResult DangNhap()
        {
            return RedirectToAction("Index", "Home");
        }

        // POST: /TaiKhoan/DangNhap
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DangNhap(DangNhapViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return Json(new { success = false, message = string.Join("<br>", errors) });
            }

            try
            {
                var query = @"
                    SELECT TK.MaTK, TK.TenDangNhap, TK.VaiTro, 
                           ISNULL(KH.HoTen, NV.HoTen) AS HoTen,
                           ISNULL(KH.MaKH, NV.MaNV) AS MaUser
                    FROM TAIKHOAN TK
                    LEFT JOIN KHACHHANG KH ON TK.MaTK = KH.MaTK
                    LEFT JOIN NHANVIEN NV ON TK.MaTK = NV.MaTK
                    WHERE TK.TenDangNhap = @Username AND TK.MatKhau = @Password
                ";
                
                var result = _db.ExecuteQuery(query, new SqlParameter("@Username", model.Username), new SqlParameter("@Password", model.Password));
                
                Console.WriteLine($"Login attempt: User={model.Username}, Pass={model.Password}, Found={result.Rows.Count}");

                if (result.Rows.Count == 0)
                {
                    return Json(new { success = false, message = "Tên đăng nhập hoặc mật khẩu không đúng!" });
                }

                var row = result.Rows[0];
                HttpContext.Session.SetString("MaTK", row["MaTK"].ToString() ?? "");
                HttpContext.Session.SetString("Username", row["TenDangNhap"].ToString() ?? "");
                HttpContext.Session.SetString("HoTen", row["HoTen"].ToString() ?? "");
                HttpContext.Session.SetString("VaiTro", row["VaiTro"].ToString() ?? "");
                HttpContext.Session.SetString("MaUser", row["MaUser"].ToString() ?? "");

                // Remember Me Logic
                if (model.RememberMe)
                {
                    var cookieOptions = new CookieOptions
                    {
                        Expires = DateTime.Now.AddDays(30),
                        HttpOnly = true,
                        Secure = true, // Ensure HTTPS
                        SameSite = SameSiteMode.Strict
                    };
                    Response.Cookies.Append("UserInfo", row["MaTK"].ToString(), cookieOptions);
                }

                // Determine redirect URL based on role
                var vaiTro = row["VaiTro"].ToString() ?? "Khách hàng";
                Console.WriteLine($"VaiTro from DB: [{vaiTro}]");
                
                string redirectUrl = vaiTro.ToLower() switch
                {
                    "khách hàng" => Url.Action("Index", "Home"),
                    "lễ tân" => Url.Action("Index", "HomeStaff"),
                    "thu ngân" => Url.Action("Index", "HomeStaff"),
                    "kỹ thuật" => Url.Action("Index", "HomeStaff"),
                    "quản lý" => Url.Action("Index", "HomeStaff"),
                    "it" => Url.Action("Index", "HomeStaff"),
                    _ => Url.Action("Index", "Home")
                };
                
                Console.WriteLine($"Redirect URL: {redirectUrl}");

                // If this is an AJAX request (fetch), return JSON with redirectUrl, else do a server-side redirect
                var isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";
                if (isAjax)
                {
                    return Json(new { success = true, message = "Đăng nhập thành công!", redirectUrl = redirectUrl });
                }
                else
                {
                    return Redirect(redirectUrl ?? Url.Action("Index", "Home"));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login");
                return Json(new { success = false, message = "Có lỗi xảy ra: " + ex.Message });
            }
        }

        // GET: /TaiKhoan/YeuCauDangNhap (Trang Landing Page khi chưa Login)
        [HttpGet]
        public IActionResult YeuCauDangNhap()
        {
            return View();
        }

        // POST: /TaiKhoan/CreateAccountAjax (Tạo tài khoản từ trang đặt sân)
        [HttpPost]
        public IActionResult CreateAccountAjax([FromBody] DangKyViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return Json(new { success = false, message = string.Join("<br>", errors) });
            }

            try
            {
                // Kiểm tra tên đăng nhập đã tồn tại
                var checkQuery = "SELECT COUNT(*) FROM TAIKHOAN WHERE TenDangNhap = @Username";
                var count = _db.ExecuteScalar<int>(checkQuery, new SqlParameter("@Username", model.Username));
                
                if (count > 0)
                {
                    return Json(new { success = false, message = "Tên đăng nhập đã tồn tại!" });
                }

                // Kiểm tra SĐT đã tồn tại
                var checkSDTQuery = "SELECT COUNT(*) FROM KHACHHANG WHERE SDT = @SDT";
                var sdtCount = _db.ExecuteScalar<int>(checkSDTQuery, new SqlParameter("@SDT", model.SDT));
                
                if (sdtCount > 0)
                {
                    return Json(new { success = false, message = "Số điện thoại đã được sử dụng!" });
                }

                // Kiểm tra Email đã tồn tại (nếu có nhập)
                if (!string.IsNullOrEmpty(model.Email))
                {
                    var checkEmailQuery = "SELECT COUNT(*) FROM KHACHHANG WHERE Email = @Email";
                    var emailCount = _db.ExecuteScalar<int>(checkEmailQuery, new SqlParameter("@Email", model.Email));
                    
                    if (emailCount > 0)
                    {
                        return Json(new { success = false, message = "Email đã được sử dụng!" });
                    }
                }

                // Kiểm tra CCCD đã tồn tại (nếu có nhập)
                if (!string.IsNullOrEmpty(model.CCCD))
                {
                    var checkCCCDQuery = "SELECT COUNT(*) FROM KHACHHANG WHERE CCCD = @CCCD";
                    var cccdCount = _db.ExecuteScalar<int>(checkCCCDQuery, new SqlParameter("@CCCD", model.CCCD));
                    
                    if (cccdCount > 0)
                    {
                        return Json(new { success = false, message = "CCCD/CMT đã được sử dụng!" });
                    }
                }

                var maTK = "TK" + DateTime.Now.ToString("yyyyMMddHHmmss");
                var maKH = "KH" + DateTime.Now.ToString("yyyyMMddHHmmss");

                // Tạo tài khoản
                var insertTKQuery = @"INSERT INTO TAIKHOAN (MaTK, TenDangNhap, MatKhau, VaiTro, NgayDangKy) VALUES (@MaTK, @Username, @Password, N'Khách hàng', GETDATE())";
                _db.ExecuteNonQuery(insertTKQuery, 
                    new SqlParameter("@MaTK", maTK), 
                    new SqlParameter("@Username", model.Username), 
                    new SqlParameter("@Password", model.Password));

                // Tạo khách hàng
                var insertKHQuery = @"INSERT INTO KHACHHANG (MaKH, HoTen, NgaySinh, CCCD, SDT, Email, DiaChi, LaHSSV, DiemTichLuy, MaCB, MaTK) VALUES (@MaKH, @HoTen, @NgaySinh, @CCCD, @SDT, @Email, @DiaChi, @LaHSSV, 0, 'CB001', @MaTK)";
                _db.ExecuteNonQuery(insertKHQuery,
                    new SqlParameter("@MaKH", maKH), 
                    new SqlParameter("@HoTen", model.HoTen ?? ""),
                    new SqlParameter("@NgaySinh", (object)model.NgaySinh ?? DBNull.Value), 
                    new SqlParameter("@CCCD", (object)model.CCCD ?? DBNull.Value),
                    new SqlParameter("@SDT", model.SDT ?? ""),
                    new SqlParameter("@Email", model.Email ?? ""),
                    new SqlParameter("@DiaChi", (object)model.DiaChi ?? DBNull.Value), 
                    new SqlParameter("@LaHSSV", model.LaHSSV ? 1 : 0),
                    new SqlParameter("@MaTK", maTK)
                );

                return Json(new { 
                    success = true,
                    message = "Tạo tài khoản thành công!",
                    maKH = maKH,
                    sdt = model.SDT,
                    hoTen = model.HoTen
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during account creation");
                return Json(new { success = false, message = "Lỗi: " + ex.Message });
            }
        }

        // GET: /TaiKhoan/DangXuat
        public IActionResult DangXuat()
        {
            HttpContext.Session.Clear();
            Response.Cookies.Delete("UserInfo"); // Clear persistent cookie
            return RedirectToAction("Index", "Home");
        }
    }
}
