using Microsoft.AspNetCore.Mvc;
using webapp_mvc.Models;
using System.Data;
using System.Data.SqlClient;

namespace webapp_mvc.Controllers
{
    public class DatSanTrucTiepController : Controller
    {
        private readonly DatabaseHelper _db;
        private readonly ILogger<DatSanTrucTiepController> _logger;

        public DatSanTrucTiepController(IConfiguration configuration, ILogger<DatSanTrucTiepController> logger)
        {
            _db = new DatabaseHelper(configuration);
            _logger = logger;
        }

        // GET: /DatSanTrucTiep/Index
        public IActionResult Index()
        {
            var vaiTro = HttpContext.Session.GetString("VaiTro");
            bool hasAccess = vaiTro?.Equals("Lễ tân", StringComparison.OrdinalIgnoreCase) == true ||
                           vaiTro?.Equals("Quản lý", StringComparison.OrdinalIgnoreCase) == true;
            if (!hasAccess)
            {
                return RedirectToAction("Index", "HomeStaff");
            }

            return View();
        }

        // POST: /DatSanTrucTiep/TimKhachHang
        [HttpPost]
        public IActionResult TimKhachHang(string tuKhoa)
        {
            try
            {
                var query = @"
                    SELECT TOP 10 KH.MaKH, KH.HoTen, KH.SDT, KH.Email, KH.CCCD
                    FROM KHACHHANG KH
                    WHERE KH.HoTen LIKE @TuKhoa OR KH.SDT LIKE @TuKhoa OR KH.CCCD LIKE @TuKhoa
                    ORDER BY KH.HoTen
                ";

                var result = _db.ExecuteQuery(query, new SqlParameter("@TuKhoa", "%" + tuKhoa + "%"));
                var customers = new List<dynamic>();

                foreach (DataRow row in result.Rows)
                {
                    customers.Add(new
                    {
                        MaKH = row["MaKH"].ToString(),
                        HoTen = row["HoTen"].ToString(),
                        SDT = row["SDT"].ToString(),
                        Email = row["Email"].ToString(),
                        CCCD = row["CCCD"].ToString()
                    });
                }

                return Json(new { success = true, data = customers });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching customers");
                return Json(new { success = false, message = ex.Message });
            }
        }

        // DTO for binding JSON body when creating customer
        public class KhachHangDto
        {
            public string HoTen { get; set; }
            public string Sdt { get; set; }
            public string Cccd { get; set; }
            public string Email { get; set; }
            public string DiaChi { get; set; }
        }

        // POST: /DatSanTrucTiep/TaoKhachHang
        [HttpPost]
        public IActionResult TaoKhachHang([FromBody] KhachHangDto dto)
        {
            try
            {
                var hoTen = dto?.HoTen?.Trim();
                var sdt = dto?.Sdt?.Trim();
                var cccd = dto?.Cccd;
                var email = dto?.Email;
                var diaChi = dto?.DiaChi;

                // Validate required fields
                if (string.IsNullOrWhiteSpace(hoTen) || string.IsNullOrWhiteSpace(sdt))
                {
                    return Json(new { success = false, message = "Họ tên và SDT là bắt buộc!" });
                }

                // Check if customer already exists
                var checkQuery = "SELECT COUNT(*) as cnt FROM KHACHHANG WHERE SDT = @SDT";
                var checkResult = _db.ExecuteQuery(checkQuery, new SqlParameter("@SDT", sdt));
                if (int.Parse(checkResult.Rows[0]["cnt"].ToString()) > 0)
                {
                    return Json(new { success = false, message = "Khách hàng này đã tồn tại!" });
                }

                // Generate new customer ID
                var idQuery = "SELECT ISNULL(MAX(CAST(SUBSTRING(MaKH, 3, LEN(MaKH)) AS INT)), 0) + 1 as MaxId FROM KHACHHANG WHERE MaKH LIKE 'KH%'";
                var idResult = _db.ExecuteQuery(idQuery);
                string maKH = "KH" + idResult.Rows[0]["MaxId"].ToString().PadLeft(5, '0');

                // Insert new customer
                var query = @"
                    INSERT INTO KHACHHANG (MaKH, HoTen, SDT, Email, DiaChi, CCCD, LaHSSV, DiemTichLuy)
                    VALUES (@MaKH, @HoTen, @SDT, @Email, @DiaChi, @CCCD, 0, 0)
                ";

                _db.ExecuteNonQuery(query,
                    new SqlParameter("@MaKH", maKH),
                    new SqlParameter("@HoTen", hoTen),
                    new SqlParameter("@SDT", sdt),
                    new SqlParameter("@Email", email ?? ""),
                    new SqlParameter("@DiaChi", diaChi ?? ""),
                    new SqlParameter("@CCCD", cccd ?? "")
                );

                return Json(new { success = true, message = "Tạo khách hàng thành công!", maKH = maKH, hoTen = hoTen, sdt = sdt });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating customer");
                return Json(new { success = false, message = "Lỗi: " + ex.Message });
            }
        }

        // GET: /DatSanTrucTiep/LoadCoSo
        [HttpGet]
        public IActionResult LoadCoSo()
        {
            try
            {
                var query = "SELECT MaCS, TenCS FROM COSO ORDER BY TenCS";
                var result = _db.ExecuteQuery(query);
                var coSos = new List<dynamic>();

                foreach (DataRow row in result.Rows)
                {
                    coSos.Add(new
                    {
                        MaCS = row["MaCS"].ToString(),
                        TenCS = row["TenCS"].ToString()
                    });
                }

                return Json(new { success = true, data = coSos });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading facilities");
                return Json(new { success = false, message = ex.Message });
            }
        }

        // GET: /DatSanTrucTiep/LoadLoaiSan
        [HttpGet]
        public IActionResult LoadLoaiSan()
        {
            try
            {
                var query = "SELECT MaLS, TenLS FROM LOAISAN ORDER BY TenLS";
                var result = _db.ExecuteQuery(query);
                var loaiSans = new List<dynamic>();

                foreach (DataRow row in result.Rows)
                {
                    loaiSans.Add(new
                    {
                        MaLS = row["MaLS"].ToString(),
                        TenLS = row["TenLS"].ToString()
                    });
                }

                return Json(new { success = true, data = loaiSans });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading court types");
                return Json(new { success = false, message = ex.Message });
            }
        }

        // GET: /DatSanTrucTiep/LoadDichVu?maCS=&maLS=
        [HttpGet]
        public IActionResult LoadDichVu(string maCS = "", string maLS = "")
        {
            try
            {
                var query = @"
                    SELECT DV.MaDV, L.TenLoai as TenDV, DV.DonGia as Gia, DVC.SoLuongTon, DV.DVT
                    FROM DICHVU DV
                    INNER JOIN DV_COSO DVC ON DV.MaDV = DVC.MaDV
                    INNER JOIN LOAIDV L ON DV.MaLoaiDV = L.MaLoaiDV
                    WHERE 1=1
                ";
                
                var parameters = new List<SqlParameter>();
                
                if (!string.IsNullOrEmpty(maCS))
                {
                    query += " AND DVC.MaCS = @MaCS";
                    parameters.Add(new SqlParameter("@MaCS", maCS));
                }
                
                if (!string.IsNullOrEmpty(maLS))
                {
                    query += " AND DV.MaLS = @MaLS";
                    parameters.Add(new SqlParameter("@MaLS", maLS));
                }
                
                query += " ORDER BY L.TenLoai";
                
                var result = _db.ExecuteQuery(query, parameters.ToArray());
                var dichVus = new List<dynamic>();

                foreach (DataRow row in result.Rows)
                {
                    dichVus.Add(new
                    {
                        MaDV = row["MaDV"].ToString(),
                        TenDV = row["TenDV"].ToString(),
                        Gia = decimal.Parse(row["Gia"].ToString()),
                        SoLuongTon = int.Parse(row["SoLuongTon"].ToString()),
                        DVT = row["DVT"].ToString()
                    });
                }

                return Json(new { success = true, data = dichVus });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading services");
                return Json(new { success = false, message = ex.Message });
            }
        }

        // POST: /DatSanTrucTiep/LoadSan
        [HttpPost]
        public IActionResult LoadSan([FromBody] Dictionary<string, string> data)
        {
            try
            {
                var maCS = data.GetValueOrDefault("maCS", "");
                var maLS = data.GetValueOrDefault("maLS", "");
                
                var query = @"
                    SELECT S.MaSan, S.MaSan AS TenSan, S.TinhTrang, S.SucChua, L.TenLS
                    FROM SAN S
                    JOIN LOAISAN L ON S.MaLS = L.MaLS
                    WHERE S.MaCS = @MaCS AND S.MaLS = @MaLS
                    ORDER BY S.MaSan
                ";

                var result = _db.ExecuteQuery(query,
                    new SqlParameter("@MaCS", maCS ?? (object)DBNull.Value),
                    new SqlParameter("@MaLS", maLS ?? (object)DBNull.Value)
                );

                var sans = new List<dynamic>();
                foreach (DataRow row in result.Rows)
                {
                    sans.Add(new
                    {
                        MaSan = row["MaSan"].ToString(),
                        TenSan = row["TenSan"].ToString(),
                        TinhTrang = row["TinhTrang"].ToString(),
                        SucChua = row["SucChua"].ToString()
                    });
                }

                return Json(new { success = true, data = sans });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading courts");
                return Json(new { success = false, message = ex.Message });
            }
        }

        // POST: /DatSanTrucTiep/TaoPhieuDat
        [HttpPost]
        public IActionResult TaoPhieuDat([FromBody] Dictionary<string, string> data)
        {
            try
            {
                var maKH = data.ContainsKey("maKH") ? data["maKH"] : null;
                var maSan = data.ContainsKey("maSan") ? data["maSan"] : null;
                var ngayDat = data.ContainsKey("ngayDat") ? data["ngayDat"] : null;
                var gioBatDau = data.ContainsKey("gioBatDau") ? data["gioBatDau"] : null;
                var gioKetThuc = data.ContainsKey("gioKetThuc") ? data["gioKetThuc"] : null;
                var ghiChu = data.ContainsKey("ghiChu") ? data["ghiChu"] : "";
                
                // Validate
                if (string.IsNullOrWhiteSpace(maKH) || string.IsNullOrWhiteSpace(maSan))
                {
                    return Json(new { success = false, message = "Vui lòng chọn khách hàng và sân!" });
                }

                // Generate booking ID
                var maDatSan = "DS" + DateTime.Now.ToString("yyyyMMddHHmmss");

                // Check availability
                var checkQuery = @"
                    SELECT COUNT(*) as cnt
                    FROM DATSAN DS
                    INNER JOIN PHIEUDATSAN P ON DS.MaDatSan = P.MaDatSan
                    WHERE DS.MaSan = @MaSan 
                    AND P.NgayDat = @NgayDat
                    AND P.TrangThai IN (N'Đã đặt', N'Đang sử dụng', N'Chờ thanh toán')
                    AND (
                        (CAST(@GioBatDau AS TIME) >= P.GioBatDau AND CAST(@GioBatDau AS TIME) < P.GioKetThuc)
                        OR (CAST(@GioKetThuc AS TIME) > P.GioBatDau AND CAST(@GioKetThuc AS TIME) <= P.GioKetThuc)
                        OR (CAST(@GioBatDau AS TIME) <= P.GioBatDau AND CAST(@GioKetThuc AS TIME) >= P.GioKetThuc)
                    )
                ";

                var checkResult = _db.ExecuteQuery(checkQuery,
                    new SqlParameter("@MaSan", maSan),
                    new SqlParameter("@NgayDat", ngayDat),
                    new SqlParameter("@GioBatDau", gioBatDau),
                    new SqlParameter("@GioKetThuc", gioKetThuc)
                );

                if (int.Parse(checkResult.Rows[0]["cnt"].ToString()) > 0)
                {
                    return Json(new { success = false, message = "Sân này đã bị đặt trong khung giờ này!" });
                }

                // Create booking
                var maNV = HttpContext.Session.GetString("MaUser");
                var query = @"
                    INSERT INTO PHIEUDATSAN (MaKH, NguoiLap, NgayDat, GioBatDau, GioKetThuc, KenhDat, TrangThai, NgayTao)
                    VALUES (@MaKH, @NguoiLap, @NgayDat, @GioBatDau, @GioKetThuc, N'Tại quầy', N'Chờ thanh toán', GETDATE())
                    SELECT CAST(SCOPE_IDENTITY() AS BIGINT) AS MaDatSan
                ";

                var result = _db.ExecuteQuery(query,
                    new SqlParameter("@MaKH", maKH),
                    new SqlParameter("@NguoiLap", maNV ?? ""),
                    new SqlParameter("@NgayDat", ngayDat),
                    new SqlParameter("@GioBatDau", gioBatDau),
                    new SqlParameter("@GioKetThuc", gioKetThuc)
                );

                // Get generated MaDatSan
                var generatedMaDatSan = long.Parse(result.Rows[0]["MaDatSan"].ToString());

                // Insert court booking details (DATSAN junction table)
                var detailQuery = @"
                    INSERT INTO DATSAN (MaDatSan, MaSan)
                    VALUES (@MaDatSan, @MaSan)
                ";

                _db.ExecuteNonQuery(detailQuery,
                    new SqlParameter("@MaDatSan", generatedMaDatSan),
                    new SqlParameter("@MaSan", maSan)
                );

                // Get customer info
                var customerQuery = "SELECT HoTen, SDT FROM KHACHHANG WHERE MaKH = @MaKH";
                var customerResult = _db.ExecuteQuery(customerQuery, new SqlParameter("@MaKH", maKH));
                var tenKH = customerResult.Rows.Count > 0 ? customerResult.Rows[0]["HoTen"].ToString() : "";
                var sdtKH = customerResult.Rows.Count > 0 ? customerResult.Rows[0]["SDT"].ToString() : "";

                // Get court type
                var courtQuery = "SELECT MaLS FROM SAN WHERE MaSan = @MaSan";
                var courtResult = _db.ExecuteQuery(courtQuery, new SqlParameter("@MaSan", maSan));
                var maLoaiSan = courtResult.Rows.Count > 0 ? courtResult.Rows[0]["MaLS"].ToString() : "";

                // Get facility from session or court
                var facilityQuery = "SELECT MaCS FROM SAN WHERE MaSan = @MaSan";
                var facilityResult = _db.ExecuteQuery(facilityQuery, new SqlParameter("@MaSan", maSan));
                var maCoSo = facilityResult.Rows.Count > 0 ? facilityResult.Rows[0]["MaCS"].ToString() : "";

                return Json(new { 
                    success = true, 
                    message = "Tạo phiếu đặt sân thành công!", 
                    data = new {
                        maDatSan = generatedMaDatSan,
                        maKH = maKH,
                        tenKH = tenKH,
                        sdtKH = sdtKH,
                        maSan = maSan,
                        maCoSo = maCoSo,
                        maLoaiSan = maLoaiSan,
                        ngayDat = ngayDat,
                        gioBatDau = gioBatDau,
                        gioKetThuc = gioKetThuc
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating booking");
                return Json(new { success = false, message = "Lỗi: " + ex.Message });
            }
        }

        // POST: /DatSanTrucTiep/TaoPhieuDichVu
        [HttpPost]
        public IActionResult TaoPhieuDichVu([FromBody] Dictionary<string, string> data)
        {
            try
            {
                var maDatSan = data.ContainsKey("maDatSan") ? data["maDatSan"] : null;
                var maDV = data.ContainsKey("maDV") ? data["maDV"] : null;
                var soLuongStr = data.ContainsKey("soLuong") ? data["soLuong"] : "1";
                var ghiChu = data.ContainsKey("ghiChu") ? data["ghiChu"] : "";

                if (!int.TryParse(soLuongStr, out int soLuong) || soLuong <= 0)
                {
                    return Json(new { success = false, message = "Số lượng không hợp lệ!" });
                }

                // Validate
                if (string.IsNullOrWhiteSpace(maDatSan) || string.IsNullOrWhiteSpace(maDV))
                {
                    return Json(new { success = false, message = "Vui lòng điền đầy đủ thông tin!" });
                }

                // Get facility from booking
                var facilityQuery = @"
                    SELECT S.MaCS 
                    FROM DATSAN DS
                    INNER JOIN SAN S ON DS.MaSan = S.MaSan
                    WHERE DS.MaDatSan = @MaDatSan
                ";
                var facilityResult = _db.ExecuteQuery(facilityQuery, 
                    new SqlParameter("@MaDatSan", long.Parse(maDatSan)));

                if (facilityResult.Rows.Count == 0)
                {
                    return Json(new { success = false, message = "Không tìm thấy thông tin đặt sân!" });
                }

                var maCS = facilityResult.Rows[0]["MaCS"].ToString();

                // Get service info and check availability at facility
                var serviceQuery = @"
                    SELECT DVC.GiaDV, DVC.SoLuongTon 
                    FROM DV_COSO DVC
                    WHERE DVC.MaDV = @MaDV AND DVC.MaCS = @MaCS
                ";
                var serviceResult = _db.ExecuteQuery(serviceQuery, 
                    new SqlParameter("@MaDV", maDV),
                    new SqlParameter("@MaCS", maCS));

                if (serviceResult.Rows.Count == 0)
                {
                    return Json(new { success = false, message = "Dịch vụ không tồn tại tại cơ sở này!" });
                }

                decimal gia = decimal.Parse(serviceResult.Rows[0]["GiaDV"].ToString());
                int soLuongTon = int.Parse(serviceResult.Rows[0]["SoLuongTon"].ToString());

                if (soLuongTon < soLuong)
                {
                    return Json(new { success = false, message = "Số lượng dịch vụ không đủ! Còn lại: " + soLuongTon });
                }

                // Insert into CT_DICHVUDAT (MaDV, MaDatSan, SoLuong)
                var query = @"
                    INSERT INTO CT_DICHVUDAT (MaDV, MaDatSan, SoLuong)
                    VALUES (@MaDV, @MaDatSan, @SoLuong)
                ";

                _db.ExecuteNonQuery(query,
                    new SqlParameter("@MaDV", maDV),
                    new SqlParameter("@MaDatSan", long.Parse(maDatSan)),
                    new SqlParameter("@SoLuong", soLuong)
                );

                // Update inventory
                var updateQuery = @"
                    UPDATE DV_COSO 
                    SET SoLuongTon = SoLuongTon - @SoLuong 
                    WHERE MaDV = @MaDV AND MaCS = @MaCS
                ";

                _db.ExecuteNonQuery(updateQuery,
                    new SqlParameter("@SoLuong", soLuong),
                    new SqlParameter("@MaDV", maDV),
                    new SqlParameter("@MaCS", maCS)
                );

                return Json(new { success = true, message = "Thêm dịch vụ thành công!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating service order");
                return Json(new { success = false, message = "Lỗi: " + ex.Message });
            }
        }

        // GET: /DatSanTrucTiep/LoadPhieuDatGanDay
        [HttpGet]
        public IActionResult LoadPhieuDatGanDay()
        {
            try
            {
                var maNV = HttpContext.Session.GetString("MaUser");
                var maCS = HttpContext.Session.GetString("MaCS");
                
                if (string.IsNullOrEmpty(maNV))
                {
                    return Json(new { success = false, message = "Chưa đăng nhập!" });
                }
                
                // Get recent bookings from staff's branch (not just created by this staff)
                // This allows viewing all bookings in the facility, not just self-created ones
                var query = @"
                    SELECT TOP 10
                        P.MaDatSan,
                        K.HoTen as TenKH,
                        ISNULL(S.MaSan, 'N/A') as DanhSachSan,
                        CONVERT(VARCHAR(10), P.NgayDat, 103) as NgayDat,
                        CONVERT(VARCHAR(5), P.GioBatDau, 108) as GioBatDau,
                        CONVERT(VARCHAR(5), P.GioKetThuc, 108) as GioKetThuc,
                        P.TrangThai,
                        ISNULL(P.NguoiLap, N'Online') as NguoiLap
                    FROM PHIEUDATSAN P
                    INNER JOIN KHACHHANG K ON P.MaKH = K.MaKH
                    LEFT JOIN DATSAN DS ON P.MaDatSan = DS.MaDatSan
                    LEFT JOIN SAN S ON DS.MaSan = S.MaSan
                    WHERE (S.MaCS = @MaCS OR @MaCS IS NULL)
                    AND P.TrangThai IN (N'Chờ thanh toán', N'Đã đặt', N'Đang sử dụng', N'Hoàn thành')
                    ORDER BY P.NgayTao DESC
                ";
                
                var result = _db.ExecuteQuery(query, new SqlParameter("@MaCS", (object)maCS ?? DBNull.Value));
                var bookings = new List<dynamic>();

                foreach (DataRow row in result.Rows)
                {
                    bookings.Add(new
                    {
                        MaDatSan = row["MaDatSan"].ToString(),
                        TenKH = row["TenKH"].ToString(),
                        DanhSachSan = row["DanhSachSan"].ToString(),
                        NgayDat = row["NgayDat"].ToString(),
                        GioBatDau = row["GioBatDau"].ToString(),
                        GioKetThuc = row["GioKetThuc"].ToString(),
                        TrangThai = row["TrangThai"].ToString()
                    });
                }

                _logger.LogInformation("LoadPhieuDatGanDay: Found {Count} bookings for staff {MaNV}", bookings.Count, maNV);
                return Json(new { success = true, data = bookings });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading recent bookings");
                return Json(new { success = false, message = ex.Message });
            }
        }

        // GET: /DatSanTrucTiep/LoadPhieuDichVuGanDay
        [HttpGet]
        public IActionResult LoadPhieuDichVuGanDay()
        {
            try
            {
                var maNV = HttpContext.Session.GetString("MaUser");
                
                if (string.IsNullOrEmpty(maNV))
                {
                    return Json(new { success = false, message = "Chưa đăng nhập!" });
                }
                
                // Get top 3 service bookings created by this staff
                var query = @"
                    SELECT TOP 3
                        C.MaDV + '_' + CAST(C.MaDatSan AS VARCHAR) as MaDatDichVu,
                        KH.HoTen as TenKhachHang,
                        L.TenLoai as TenDichVu,
                        C.SoLuong,
                        (C.SoLuong * DV.DonGia) as TongTien,
                        P.TrangThai
                    FROM CT_DICHVUDAT C
                    INNER JOIN PHIEUDATSAN P ON C.MaDatSan = P.MaDatSan
                    INNER JOIN KHACHHANG KH ON P.MaKH = KH.MaKH
                    INNER JOIN DICHVU DV ON C.MaDV = DV.MaDV
                    INNER JOIN LOAIDV L ON DV.MaLoaiDV = L.MaLoaiDV
                    INNER JOIN DV_COSO DVC ON C.MaDV = DVC.MaDV
                    INNER JOIN DATSAN DS ON P.MaDatSan = DS.MaDatSan
                    INNER JOIN SAN S ON DS.MaSan = S.MaSan AND DVC.MaCS = S.MaCS
                    WHERE P.NguoiLap = @MaNV
                    ORDER BY P.NgayTao DESC
                ";

                var result = _db.ExecuteQuery(query, new SqlParameter("@MaNV", maNV));
                var services = new List<dynamic>();

                foreach (DataRow row in result.Rows)
                {
                    services.Add(new
                    {
                        MaDatDichVu = row["MaDatDichVu"].ToString(),
                        TenKhachHang = row["TenKhachHang"].ToString(),
                        TenDichVu = row["TenDichVu"].ToString(),
                        SoLuong = int.Parse(row["SoLuong"].ToString()),
                        TongTien = decimal.Parse(row["TongTien"].ToString()).ToString("N0"),
                        TrangThai = row["TrangThai"].ToString()
                    });
                }

                _logger.LogInformation("LoadPhieuDichVuGanDay: Found {Count} service orders for staff {MaNV}", services.Count, maNV);
                return Json(new { success = true, data = services });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading recent service orders");
                return Json(new { success = false, message = ex.Message });
            }
        }

        // POST: /DatSanTrucTiep/ChuyenThuNgan
        [HttpPost]
        public IActionResult ChuyenThuNgan([FromBody] Dictionary<string, string> data)
        {
            try
            {
                var maDatSan = data["maDatSan"];
                
                if (string.IsNullOrWhiteSpace(maDatSan))
                {
                    return Json(new { success = false, message = "Mã đặt sân không hợp lệ!" });
                }

                // Update booking status to "Chờ thanh toán"
                var updateQuery = @"
                    UPDATE PHIEUDATSAN 
                    SET TrangThai = N'Chờ thanh toán'
                    WHERE MaDatSan = @MaDatSan
                ";

                _db.ExecuteNonQuery(updateQuery, new SqlParameter("@MaDatSan", long.Parse(maDatSan)));

                return Json(new { success = true, message = "Đã chuyển phiếu đặt sân cho thu ngân thanh toán!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending to cashier");
                return Json(new { success = false, message = "Lỗi: " + ex.Message });
            }
        }

        // GET: /DatSanTrucTiep/PrintConfirmation?maDatSan=xxx
        public IActionResult PrintConfirmation(string maDatSan)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(maDatSan))
                {
                    return Content("Mã đặt sân không hợp lệ!");
                }

                // Get booking details
                var query = @"
                    SELECT 
                        P.MaDatSan,
                        P.NgayDat,
                        P.GioBatDau,
                        P.GioKetThuc,
                        P.TrangThai,
                        P.NgayTao,
                        KH.HoTen as TenKH,
                        KH.SDT as SdtKH,
                        KH.Email as EmailKH,
                        STRING_AGG(S.MaSan, ', ') as DanhSachSan,
                        CS.TenCS,
                        LS.TenLS
                    FROM PHIEUDATSAN P
                    INNER JOIN KHACHHANG KH ON P.MaKH = KH.MaKH
                    LEFT JOIN DATSAN DS ON P.MaDatSan = DS.MaDatSan
                    LEFT JOIN SAN S ON DS.MaSan = S.MaSan
                    LEFT JOIN COSO CS ON S.MaCS = CS.MaCS
                    LEFT JOIN LOAISAN LS ON S.MaLS = LS.MaLS
                    WHERE P.MaDatSan = @MaDatSan
                    GROUP BY P.MaDatSan, P.NgayDat, P.GioBatDau, P.GioKetThuc, P.TrangThai, P.NgayTao,
                             KH.HoTen, KH.SDT, KH.Email, CS.TenCS, LS.TenLS
                ";

                var result = _db.ExecuteQuery(query, new SqlParameter("@MaDatSan", long.Parse(maDatSan)));

                if (result.Rows.Count == 0)
                {
                    return Content("Không tìm thấy phiếu đặt sân!");
                }

                var row = result.Rows[0];

                // Pass data to view
                ViewBag.MaDatSan = row["MaDatSan"].ToString();
                ViewBag.TenKH = row["TenKH"].ToString();
                ViewBag.SdtKH = row["SdtKH"].ToString();
                ViewBag.EmailKH = row["EmailKH"].ToString();
                ViewBag.TenCS = row["TenCS"].ToString();
                ViewBag.TenLS = row["TenLS"].ToString();
                ViewBag.DanhSachSan = row["DanhSachSan"].ToString();
                ViewBag.NgayDat = Convert.ToDateTime(row["NgayDat"]).ToString("dd/MM/yyyy");
                ViewBag.GioBatDau = row["GioBatDau"].ToString();
                ViewBag.GioKetThuc = row["GioKetThuc"].ToString();
                ViewBag.TrangThai = row["TrangThai"].ToString();
                ViewBag.NgayTao = Convert.ToDateTime(row["NgayTao"]).ToString("dd/MM/yyyy HH:mm");

                return View("PrintConfirmation");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error printing confirmation");
                return Content("Lỗi: " + ex.Message);
            }
        }
    }
}

