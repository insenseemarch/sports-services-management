using System;

namespace webapp_mvc.Models
{
    public class LichLamViecViewModel
    {
        public List<CaTrucItem> DanhSachCaTruc { get; set; } = new List<CaTrucItem>();
        
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }

        public string TenNhanVien { get; set; }
        public List<NhanVienItem> DanhSachDongNghiep { get; set; } = new List<NhanVienItem>();
        
        // Thống kê
        public int TongSoCa { get; set; }
        public int SoCaDaLam { get; set; }
        public int SoCaSapToi { get; set; }
        
        // Nghỉ phép
        public List<DonNghiPhepItem> DanhSachDonNghi { get; set; } = new List<DonNghiPhepItem>();
        public List<CaTrucItem> CaTrucCoTheNghi { get; set; } = new List<CaTrucItem>();
    }


    public class CaTrucItem
    {
        public long MaCaTruc { get; set; }
        public DateTime NgayTruc { get; set; }
        public TimeSpan GioBatDau { get; set; }
        public TimeSpan GioKetThuc { get; set; }
        public decimal PhuCap { get; set; }
        public string TrangThai { get; set; } // "Đã làm", "Sắp tới", "Đang diễn ra"
        public string ThuTrongTuan { get; set; } // "Thứ 2", "Chủ nhật"...
        public bool CoTheNghi { get; set; } // Flag để biết ca này có thể xin nghỉ không (chưa diễn ra, chưa có đơn)
    }

    public class DonNghiPhepItem
    {
        public long MaDon { get; set; }
        public DateTime NgayNghi { get; set; }
        public string CaNghiInfo { get; set; } // Hiển thị: "10:00 - 12:00 (10/05/2026)"
        public string LyDo { get; set; }
        public string TrangThai { get; set; } // "Chờ duyệt", "Đã duyệt", "Từ chối"
        public DateTime? NgayDuyet { get; set; }
    }
}
