using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Collections.Generic;
using webapp_mvc.Models;

namespace webapp_mvc.Controllers
{
    public class NhanVienController : Controller
    {
        private readonly DatabaseHelper _db;

        public NhanVienController(DatabaseHelper db)
        {
            _db = db;
        }

        // GET: /NhanVien/Index - Danh sách phiếu đặt sân của nhân viên
        [HttpGet]
        public IActionResult Index(string fromDate = null, string toDate = null, string status = null)
        {
            var maUser = HttpContext.Session.GetString("MaUser");
            var vaiTro = HttpContext.Session.GetString("VaiTro");

            // Security check
            if (string.IsNullOrEmpty(maUser) || string.IsNullOrEmpty(vaiTro) || vaiTro == "Khách hàng")
            {
                return RedirectToAction("DangNhap", "TaiKhoan");
            }

            // Build SQL with filters
            string sql = @"
                SELECT p.MaDatSan, p.NgayDat, p.GioBatDau, p.GioKetThuc, p.TrangThai, p.NgayTao,
                       kh.HoTen as TenKhachHang, kh.SDT,
                       s.MaSan, ls.TenLS, cs.TenCS,
                       dbo.f_TinhTienSan(p.MaDatSan) as TienSan
                FROM PHIEUDATSAN p
                LEFT JOIN KHACHHANG kh ON p.MaKH = kh.MaKH
                LEFT JOIN DATSAN d ON p.MaDatSan = d.MaDatSan
                LEFT JOIN SAN s ON d.MaSan = s.MaSan
                LEFT JOIN LOAISAN ls ON s.MaLS = ls.MaLS
                LEFT JOIN COSO cs ON s.MaCS = cs.MaCS
                WHERE 1=1";

            var parameters = new List<SqlParameter>();

            // Check if NguoiLap column exists and filter by it
            try
            {
                var checkColumn = _db.ExecuteQuery("SELECT TOP 1 NguoiLap FROM PHIEUDATSAN");
                // Column exists, add filter
                sql += " AND p.NguoiLap = @MaNV";
                parameters.Add(new SqlParameter("@MaNV", maUser));
            }
            catch
            {
                // Column doesn't exist, show all bookings (or you can add different logic)
                // For now, just continue without this filter
            }

            sql += " AND p.TrangThai != N'Nháp'";

            // Add date filters
            if (!string.IsNullOrEmpty(fromDate))
            {
                sql += " AND p.NgayDat >= @FromDate";
                parameters.Add(new SqlParameter("@FromDate", DateTime.Parse(fromDate)));
                ViewBag.FromDate = fromDate;
            }

            if (!string.IsNullOrEmpty(toDate))
            {
                sql += " AND p.NgayDat <= @ToDate";
                parameters.Add(new SqlParameter("@ToDate", DateTime.Parse(toDate)));
                ViewBag.ToDate = toDate;
            }

            // Add status filter
            if (!string.IsNullOrEmpty(status))
            {
                sql += " AND p.TrangThai = @Status";
                parameters.Add(new SqlParameter("@Status", status));
                ViewBag.Status = status;
            }

            sql += " ORDER BY p.NgayTao DESC, p.MaDatSan DESC";

            var dt = _db.ExecuteQuery(sql, parameters.ToArray());
            
            ViewBag.DanhSachPhieu = dt;
            
            if (TempData["SuccessMessage"] != null)
            {
                ViewBag.SuccessMessage = TempData["SuccessMessage"];
            }

            return View();
        }

        // GET: /NhanVien/Profile
        [HttpGet]
        public IActionResult Profile()
        {
            var maUser = HttpContext.Session.GetString("MaUser");
            var vaiTro = HttpContext.Session.GetString("VaiTro");

            // Security check
            if (string.IsNullOrEmpty(maUser) || string.IsNullOrEmpty(vaiTro) || vaiTro == "Khách hàng")
            {
                return RedirectToAction("DangNhap", "TaiKhoan");
            }

            var model = new HoSoViewModel();
            
            string query = @"
                SELECT NV.*, CS.TenCS 
                FROM NHANVIEN NV
                LEFT JOIN COSO CS ON NV.MaCS = CS.MaCS
                WHERE NV.MaNV = @MaNV";

            var dt = _db.ExecuteQuery(query, new SqlParameter("@MaNV", maUser));
            if (dt.Rows.Count > 0)
            {
                var row = dt.Rows[0];
                model.HoTen = row["HoTen"].ToString();
                model.SDT = row["SDT"].ToString();
                model.DiaChi = row["DiaChi"].ToString();
                model.NgaySinh = row["NgaySinh"] == DBNull.Value ? null : Convert.ToDateTime(row["NgaySinh"]);
                model.CCCD = row["CMND_CCCD"].ToString();
                model.ChucVu = row["ChucVu"].ToString();
                model.TenCS = row["TenCS"].ToString();
                model.GioiTinh = row["GioiTinh"].ToString();
                model.LuongCoBan = row["LuongCoBan"] != DBNull.Value ? Convert.ToDecimal(row["LuongCoBan"]) : 0;
            }

            if (TempData["SuccessMessage"] != null)
            {
                ViewBag.Message = TempData["SuccessMessage"];
            }

            return View(model);
        }

        // POST: /NhanVien/Profile
        [HttpPost]
        public IActionResult Profile(HoSoViewModel model)
        {
            var maUser = HttpContext.Session.GetString("MaUser");
            var vaiTro = HttpContext.Session.GetString("VaiTro");

            if (string.IsNullOrEmpty(maUser) || string.IsNullOrEmpty(vaiTro) || vaiTro == "Khách hàng")
            {
                return RedirectToAction("DangNhap", "TaiKhoan");
            }

            string sql = @"UPDATE NHANVIEN 
                           SET HoTen=@HoTen, SDT=@SDT, DiaChi=@DiaChi, 
                               NgaySinh=@NgaySinh, CMND_CCCD=@CCCD, GioiTinh=@GioiTinh
                           WHERE MaNV=@MaNV";
            
            _db.ExecuteNonQuery(sql,
                 new SqlParameter("@HoTen", model.HoTen),
                 new SqlParameter("@SDT", model.SDT),
                 new SqlParameter("@DiaChi", model.DiaChi ?? ""),
                 new SqlParameter("@NgaySinh", (object)model.NgaySinh ?? DBNull.Value),
                 new SqlParameter("@CCCD", model.CCCD ?? ""),
                 new SqlParameter("@GioiTinh", model.GioiTinh ?? ""),
                 new SqlParameter("@MaNV", maUser)
            );

            TempData["SuccessMessage"] = "Cập nhật hồ sơ thành công!";
            return RedirectToAction("Profile");
        }

        // GET: /NhanVien/LichLamViec (Placeholder for Menu Link)
        // GET: /NhanVien/LichLamViec
        [HttpGet]
        public IActionResult LichLamViec(string fromDate, string toDate)
        {
            var maUser = HttpContext.Session.GetString("MaUser");
            var vaiTro = HttpContext.Session.GetString("VaiTro");

            if (string.IsNullOrEmpty(maUser) || string.IsNullOrEmpty(vaiTro) || vaiTro == "Khách hàng")
            {
                return RedirectToAction("DangNhap", "TaiKhoan");
            }

            // Default: First day of current month to Last day of current month
            DateTime start = string.IsNullOrEmpty(fromDate) ? new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1) : DateTime.Parse(fromDate);
            DateTime end = string.IsNullOrEmpty(toDate) ? start.AddMonths(1).AddDays(-1) : DateTime.Parse(toDate);

            var model = new LichLamViecViewModel
            {
                FromDate = start,
                ToDate = end,
                DanhSachCaTruc = new List<CaTrucItem>(),
                DanhSachDonNghi = new List<DonNghiPhepItem>(),
                CaTrucCoTheNghi = new List<CaTrucItem>(),
                DanhSachDongNghiep = new List<NhanVienItem>()
            };

            // 0. Get User Info & Colleagues
            string sqlInfo = "SELECT HoTen, ChucVu FROM NHANVIEN WHERE MaNV = @MaNV";
            var dtInfo = _db.ExecuteQuery(sqlInfo, new SqlParameter("@MaNV", maUser));
            if (dtInfo.Rows.Count > 0)
            {
                model.TenNhanVien = dtInfo.Rows[0]["HoTen"].ToString();
                string chucVu = dtInfo.Rows[0]["ChucVu"].ToString();
                
                // Get Colleagues same role
                string sqlColleagues = "SELECT MaNV, HoTen FROM NHANVIEN WHERE ChucVu = @ChucVu AND MaNV != @MaNV";
                var dtColleagues = _db.ExecuteQuery(sqlColleagues, new SqlParameter("@ChucVu", chucVu), new SqlParameter("@MaNV", maUser));
                foreach (System.Data.DataRow row in dtColleagues.Rows)
                {
                    model.DanhSachDongNghiep.Add(new NhanVienItem {
                        MaNV = row["MaNV"].ToString(),
                        HoTen = row["HoTen"].ToString()
                    });
                }
            }

            // 1. Get Schedule
            string sql = @"
                SELECT MaCaTruc, NgayTruc, GioBatDau, GioKetThuc, PhuCap
                FROM CATRUC 
                WHERE MaNV = @MaNV 
                AND NgayTruc >= @FromDate 
                AND NgayTruc <= @ToDate
                ORDER BY NgayTruc DESC, GioBatDau ASC";
            
            var parameters = new SqlParameter[] {
                new SqlParameter("@MaNV", maUser),
                new SqlParameter("@FromDate", start),
                new SqlParameter("@ToDate", end)
            };

            var dt = _db.ExecuteQuery(sql, parameters);
            
            foreach (System.Data.DataRow row in dt.Rows)
            {
                var ngayTruc = Convert.ToDateTime(row["NgayTruc"]);
                var gioBatDau = (TimeSpan)row["GioBatDau"];
                var gioKetThuc = (TimeSpan)row["GioKetThuc"];
                long maCa = Convert.ToInt64(row["MaCaTruc"]);

                var item = new CaTrucItem
                {
                    MaCaTruc = maCa,
                    NgayTruc = ngayTruc,
                    GioBatDau = gioBatDau,
                    GioKetThuc = gioKetThuc,
                    PhuCap = row["PhuCap"] != DBNull.Value ? Convert.ToDecimal(row["PhuCap"]) : 0,
                    ThuTrongTuan = ngayTruc.ToString("dddd", new System.Globalization.CultureInfo("vi-VN"))
                };

                // Logic Trang Thai & CoTheNghi
                if (ngayTruc.Date < DateTime.Now.Date) 
                {
                    item.TrangThai = "Đã làm";
                    item.CoTheNghi = false;
                }
                else if (ngayTruc.Date == DateTime.Now.Date) 
                {
                    var now = DateTime.Now.TimeOfDay;
                    if (now > gioKetThuc) { item.TrangThai = "Đã làm"; item.CoTheNghi = false; }
                    else if (now >= gioBatDau && now <= gioKetThuc) { item.TrangThai = "Đang trực"; item.CoTheNghi = false; }
                    else { item.TrangThai = "Sắp tới"; item.CoTheNghi = true; } // Today but haven't started (maybe too late to ask?) Let's allow for now.
                }
                else 
                {
                    item.TrangThai = "Sắp tới";
                    item.CoTheNghi = true;
                }

                model.DanhSachCaTruc.Add(item);
                
                // Add to eligibility list if applicable (Need to filter out ones already requested later)
                if (item.CoTheNghi) {
                    model.CaTrucCoTheNghi.Add(item);
                }
            }

            // 2. Get Leave Requests
            string sqlDon = @"SELECT d.*, c.GioBatDau, c.GioKetThuc 
                              FROM DONNGHIPHEP d 
                              LEFT JOIN CATRUC c ON d.CaNghi = c.MaCaTruc
                              WHERE d.MaNV = @MaNV 
                              ORDER BY d.MaDon DESC";
            
            var dtDon = _db.ExecuteQuery(sqlDon, new SqlParameter("@MaNV", maUser));
             foreach (System.Data.DataRow row in dtDon.Rows)
            {
                var ngayNghi = Convert.ToDateTime(row["NgayNghi"]);
                var gioBD = row["GioBatDau"] != DBNull.Value ? (TimeSpan)row["GioBatDau"] : TimeSpan.Zero;
                var gioKT = row["GioKetThuc"] != DBNull.Value ? (TimeSpan)row["GioKetThuc"] : TimeSpan.Zero;
                long caNghiId = Convert.ToInt64(row["CaNghi"]);

                model.DanhSachDonNghi.Add(new DonNghiPhepItem
                {
                    MaDon = Convert.ToInt64(row["MaDon"]),
                    NgayNghi = ngayNghi,
                    CaNghiInfo = $"{gioBD:hh\\:mm} - {gioKT:hh\\:mm} ({ngayNghi:dd/MM/yyyy})",
                    LyDo = row["LyDo"].ToString(),
                    TrangThai = row["TrangThai"].ToString(),
                    NgayDuyet = row["NgayDuyet"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(row["NgayDuyet"]) : null
                });

                // Remove from CoTheNghi if already has a request (Pending or Approved)
                if (row["TrangThai"].ToString() != "Từ chối") {
                    var existing = model.CaTrucCoTheNghi.FirstOrDefault(x => x.MaCaTruc == caNghiId);
                    if (existing != null) model.CaTrucCoTheNghi.Remove(existing);
                }
            }
            
            // 3. Populate CaTrucCoTheNghi (All Future Shifts)
            string sqlFuture = @"SELECT MaCaTruc, NgayTruc, GioBatDau, GioKetThuc 
                                 FROM CATRUC 
                                 WHERE MaNV = @MaNV AND NgayTruc >= @Today
                                 ORDER BY NgayTruc ASC, GioBatDau ASC";
            var dtFuture = _db.ExecuteQuery(sqlFuture, 
                new SqlParameter("@MaNV", maUser), 
                new SqlParameter("@Today", DateTime.Now.Date));
            
            model.CaTrucCoTheNghi.Clear();
            foreach(System.Data.DataRow row in dtFuture.Rows) {
                long maCa = Convert.ToInt64(row["MaCaTruc"]);
                DateTime ngayTruc = (DateTime)row["NgayTruc"];
                TimeSpan gioBatDau = (TimeSpan)row["GioBatDau"];
                TimeSpan gioKetThuc = (TimeSpan)row["GioKetThuc"];

                // Strict Future Check: If today, must not have started yet
                if (ngayTruc.Date == DateTime.Now.Date && gioBatDau <= DateTime.Now.TimeOfDay) continue;

                // Check if already requested
                bool exists = false;
                foreach(System.Data.DataRow rDon in dtDon.Rows) {
                     if (Convert.ToInt64(rDon["CaNghi"]) == maCa 
                         && rDon["TrangThai"].ToString() != "Từ chối"
                         && Convert.ToDateTime(rDon["NgayNghi"]).Date == ngayTruc.Date) {
                         exists = true; break;
                     }
                }
                
                if (!exists) {
                    model.CaTrucCoTheNghi.Add(new CaTrucItem {
                         MaCaTruc = maCa,
                         NgayTruc = ngayTruc,
                         GioBatDau = gioBatDau,
                         GioKetThuc = gioKetThuc,
                         ThuTrongTuan = ngayTruc.ToString("dddd", new System.Globalization.CultureInfo("vi-VN"))
                    });
                }
            }

            // Fallback: Ensure shifts capable of being applied for leave in the current view are included
            // This covers cases where Future Query might miss something due to Date checks, but Calendar View sees it.
            var existingIds = new HashSet<long>(model.CaTrucCoTheNghi.Select(x => x.MaCaTruc));
            
            foreach(var item in model.DanhSachCaTruc)
            {
                if (item.CoTheNghi && !existingIds.Contains(item.MaCaTruc))
                {
                    // Double check overlap with existing requests
                    bool exists = false;
                    foreach(System.Data.DataRow rDon in dtDon.Rows) {
                         // Fix: Also check if Date matches to avoid hiding recycled Shift IDs from past years
                         if (Convert.ToInt64(rDon["CaNghi"]) == item.MaCaTruc 
                             && rDon["TrangThai"].ToString() != "Từ chối"
                             && Convert.ToDateTime(rDon["NgayNghi"]).Date == item.NgayTruc.Date) {
                             exists = true; break;
                         }
                    }
                    
                    if (!exists) {
                        model.CaTrucCoTheNghi.Add(item);
                        existingIds.Add(item.MaCaTruc);
                    }
                }
            }
            // Sort by Date then Time
            model.CaTrucCoTheNghi = model.CaTrucCoTheNghi.OrderBy(x => x.NgayTruc).ThenBy(x => x.GioBatDau).ToList();


            // Stats
            model.TongSoCa = model.DanhSachCaTruc.Count;
            model.SoCaDaLam = model.DanhSachCaTruc.Count(x => x.TrangThai == "Đã làm");
            model.SoCaSapToi = model.TongSoCa - model.SoCaDaLam;
            
            if (TempData["SuccessMessage"] != null) ViewBag.SuccessMessage = TempData["SuccessMessage"];
            if (TempData["Error"] != null) ViewBag.Error = TempData["Error"];

            return View(model);
        }

        // POST: /NhanVien/XinNghiPhep
        [HttpPost]
        public IActionResult XinNghiPhep(long maCaTruc, string lyDo, string nguoiThayThe)
        {
            var maUser = HttpContext.Session.GetString("MaUser");
            if (string.IsNullOrEmpty(maUser)) return RedirectToAction("DangNhap", "TaiKhoan");

            // Get info about the shift
            string sqlCheck = "SELECT NgayTruc FROM CATRUC WHERE MaCaTruc = @MaCa AND MaNV = @MaNV";
            var dt = _db.ExecuteQuery(sqlCheck, 
                new SqlParameter("@MaCa", maCaTruc),
                new SqlParameter("@MaNV", maUser));
            
            if (dt.Rows.Count == 0) {
                 TempData["Error"] = "Ca trực không hợp lệ!";
                 return RedirectToAction("LichLamViec");
            }
            
            DateTime ngayNghi = Convert.ToDateTime(dt.Rows[0]["NgayTruc"]);

            // Insert
            string sql = @"INSERT INTO DONNGHIPHEP (MaNV, NgayNghi, CaNghi, LyDo, TrangThai, NguoiThayThe) 
                           VALUES (@MaNV, @NgayNghi, @CaNghi, @LyDo, N'Chờ duyệt', @NguoiThayThe)";
            
            _db.ExecuteNonQuery(sql,
                new SqlParameter("@MaNV", maUser),
                new SqlParameter("@NgayNghi", ngayNghi),
                new SqlParameter("@CaNghi", maCaTruc),
                new SqlParameter("@LyDo", lyDo),
                new SqlParameter("@NguoiThayThe", (object)nguoiThayThe ?? DBNull.Value)
            );

            TempData["SuccessMessage"] = "Đơn xin nghỉ phép của bạn đã được gửi thành công!";
            TempData["ShowSuccessModal"] = "true"; // Trigger modal in View
            return RedirectToAction("LichLamViec");
        }
    }
}
