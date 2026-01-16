using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using webapp_mvc.Models;

namespace webapp_mvc.Controllers
{
    public class ThanhToanController : Controller
    {
        private readonly DatabaseHelper _db;

        public ThanhToanController(DatabaseHelper db)
        {
            _db = db;
        }

        // Trang danh sách các phiếu cần thanh toán
        [HttpGet]
        public IActionResult Index(string? search)
        {
            Console.WriteLine("DEBUG CHECK: Truy cap trang Thanh Toan (Code Moi)");
            // Check quyền: Phải là Nhân viên (Thu ngân/Quản lý)
            var vaiTro = HttpContext.Session.GetString("VaiTro");
            if (vaiTro == "Khách hàng" || string.IsNullOrEmpty(vaiTro))
            {
                return RedirectToAction("Index", "Home");
            }

            var list = new List<ThanhToanItem>();
            string sql = @"
                SELECT P.MaDatSan, P.MaKH, K.HoTen, P.NgayDat, S.TenCS, 
                       P.TrangThai,
                       dbo.f_TinhTienSan(P.MaDatSan) as TienSan,
                       -- TINH LAI TIEN DICH VU TU GOC (SO LUONG * DON GIA)
                       ISNULL(SUM(CT.SoLuong * DV.DonGia), 0) as TienDichVu,
                       COUNT(CT.MaDV) as SoLuongMon
                FROM PHIEUDATSAN P
                JOIN KHACHHANG K ON P.MaKH = K.MaKH
                JOIN DATSAN D ON P.MaDatSan = D.MaDatSan
                JOIN SAN SCN ON D.MaSan = SCN.MaSan
                JOIN COSO S ON SCN.MaCS = S.MaCS
                LEFT JOIN CT_DICHVUDAT CT ON P.MaDatSan = CT.MaDatSan
                LEFT JOIN DICHVU DV ON CT.MaDV = DV.MaDV -- JOIN THEM BANG DICH VU
                WHERE P.TrangThai IN (N'Đã đặt', N'Đang sử dụng')
                AND (@Search IS NULL OR CAST(P.MaDatSan AS NVARCHAR) LIKE '%' + @Search + '%' OR K.SDT LIKE '%' + @Search + '%')
                GROUP BY P.MaDatSan, P.MaKH, K.HoTen, P.NgayDat, S.TenCS, P.TrangThai
                ORDER BY P.NgayDat DESC";
            
            var dt = _db.ExecuteQuery(sql, new SqlParameter("@Search", search ?? (object)DBNull.Value));
            foreach (System.Data.DataRow r in dt.Rows)
            {
                var item = new ThanhToanItem
                {
                    MaDatSan = Convert.ToInt64(r["MaDatSan"]),
                    TenKhachHang = r["HoTen"].ToString()!,
                    NgayDat = Convert.ToDateTime(r["NgayDat"]),
                    TenCoSo = r["TenCS"].ToString()!,
                    TrangThai = r["TrangThai"].ToString()!,
                    TienSan = Convert.ToDecimal(r["TienSan"]),
                    TienDichVu = Convert.ToDecimal(r["TienDichVu"]),
                    SoLuongMon = Convert.ToInt32(r["SoLuongMon"])
                };
                Console.WriteLine($"DEBUG ROW: ID={item.MaDatSan}, TienSan={item.TienSan}, DV={item.TienDichVu}, SLMon={item.SoLuongMon}");
                item.TongTien = item.TienSan + item.TienDichVu;
                list.Add(item);
            }

            return View(list);
        }

        // Trang xác nhận thanh toán cho 1 phiếu
        [HttpGet]
        public IActionResult Confirm(long id)
        {
            // Load chi tiết phiếu + Dịch vụ đã dùng
            var model = new HoaDonViewModel();
            model.MaDatSan = id;

            // 1. Lấy thông tin phiếu
            string sqlInfo = @"SELECT P.MaDatSan, K.HoTen, dbo.f_TinhTienSan(P.MaDatSan) as TienSan 
                               FROM PHIEUDATSAN P JOIN KHACHHANG K ON P.MaKH = K.MaKH WHERE P.MaDatSan = @ID";
            var dtInfo = _db.ExecuteQuery(sqlInfo, new SqlParameter("@ID", id));
            if(dtInfo.Rows.Count > 0)
            {
                model.TenKhachHang = dtInfo.Rows[0]["HoTen"].ToString()!;
                model.TienSan = Convert.ToDecimal(dtInfo.Rows[0]["TienSan"]);
            }

            // 2. Lấy tiền dịch vụ
            string sqlDV = "SELECT ISNULL(SUM(ThanhTien), 0) FROM CT_DICHVUDAT WHERE MaDatSan = @ID";
            model.TienDichVu = _db.ExecuteScalar<decimal>(sqlDV, new SqlParameter("@ID", id));

            model.TongTien = model.TienSan + model.TienDichVu;
            
            return View(model);
        }

        [HttpPost]
        public IActionResult Process(long MaDatSan, string HinhThucTT, decimal GiamGia)
        {
            var maUser = HttpContext.Session.GetString("MaUser"); // Thu ngân

            var p = new SqlParameter[] {
                new SqlParameter("@MaDatSan", MaDatSan),
                new SqlParameter("@NguoiLap", maUser), // Param name match SP
                new SqlParameter("@HinhThucTT", HinhThucTT),
                new SqlParameter("@GiamGia", GiamGia) // Optional Param? Check SP signature later.
                                                      // Assuming SP handles discount logic internally or via param?
                                                      // Let's assume standard params.
            };

            // Gọi SP: sp_ThanhToanVaXuatHoaDon
            // Signature: @MaDatSan, @NguoiLap, @HinhThucTT
            // GiamGia is handled inside SP (via promotion) or param?
            // Re-read SP: It calculates discount based on Customer Rank automatically.
            // So we don't pass GiamGia manually unless SP supports it.
            // Let's pass basic params first.
            
            try 
            {
                 // Gọi SP chỉ với 3 tham số cơ bản
                 var pSimple = new SqlParameter[] {
                    new SqlParameter("@MaDatSan", MaDatSan),
                    new SqlParameter("@NguoiLap", maUser),
                    new SqlParameter("@HinhThucTT", HinhThucTT)
                 };
                _db.ExecuteNonQuery("sp_ThanhToanVaXuatHoaDon", pSimple);
                
                return RedirectToAction("Index", new { msg = "Thanh toán thành công!" });
            }
            catch(SqlException ex)
            {
                return RedirectToAction("Confirm", new { id = MaDatSan, error = ex.Message });
            }
        }
    }

    public class ThanhToanItem {
        public long MaDatSan { get; set; }
        public string TenKhachHang { get; set; }
        public DateTime NgayDat { get; set; }
        public string TenCoSo { get; set; }
        public string TrangThai { get; set; }
        public decimal TienSan { get; set; }
        public decimal TienDichVu { get; set; } // Debug property
        public int SoLuongMon { get; set; } // Debug property
        public decimal TongTien { get; set; } // Total including services
    }

    public class HoaDonViewModel {
        public long MaDatSan { get; set; }
        public string TenKhachHang { get; set; }
        public decimal TienSan { get; set; }
        public decimal TienDichVu { get; set; }
        public decimal TongTien { get; set; }
    }
}
