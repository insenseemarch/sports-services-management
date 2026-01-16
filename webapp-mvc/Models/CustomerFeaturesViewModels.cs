using System.ComponentModel.DataAnnotations;

namespace webapp_mvc.Models
{
    public class DichVuViewModel
    {
        public long MaDatSan { get; set; }
        public string ThongTinSan { get; set; }
        public string? TenCS { get; set; }
        public string? FilterMaCS { get; set; } // Added for Service-Only Order Context
        public DateTime NgayDat { get; set; } = DateTime.Today;
        public TimeSpan GioBatDau { get; set; } = new TimeSpan(7, 0, 0);
        public TimeSpan GioKetThuc { get; set; } = new TimeSpan(22, 0, 0);
        public List<DichVuItem> DanhSachDichVu { get; set; } = new List<DichVuItem>();
    }
    public class DichVuItem
    {
        public string MaDV { get; set; } = string.Empty;
        public string TenDV { get; set; } = string.Empty;
        public decimal DonGia { get; set; }
        public string DVT { get; set; } = string.Empty;
        public string HinhAnh { get; set; } = string.Empty;
        public string LoaiDV { get; set; } = string.Empty; 
        public int SoLuong { get; set; }
        public int SoLuongTon { get; set; }

        public string? ChuyenMon { get; set; } // Cho HLV
        public string? TenCS { get; set; } // Facility Name per Item
    }

    public class LichSuViewModel
    {
        public List<PhieuDatItem> LichSuDat { get; set; } = new List<PhieuDatItem>();
    }
    public class PhieuDatItem
    {
        public long MaDatSan { get; set; }
        public string MaSan { get; set; }
        public string? LoaiSan { get; set; } // Court type for validation
        public DateTime NgayDat { get; set; }
        public TimeSpan GioBatDau { get; set; }
        public TimeSpan GioKetThuc { get; set; }
        public decimal ThanhTien { get; set; }
        public string TrangThai { get; set; }
        public DateTime? NgayTao { get; set; }
        public double RemainingSeconds { get; set; } // Server-calculated countdown
    }

    public class BaoCaoLoiViewModel
    {
        public List<string> DanhSachSan { get; set; } = new List<string>();
        public string SelectedSan { get; set; }
        [Required]
        public string MoTaSuCo { get; set; }
    }

    public class HoSoViewModel
    {
        public string HoTen { get; set; }
        public string SDT { get; set; }
        public string Email { get; set; }
        public string DiaChi { get; set; }
        
        [DataType(DataType.Date)]
        public DateTime? NgaySinh { get; set; }
        public string CCCD { get; set; }
        public bool LaHSSV { get; set; }
        public int DiemTichLuy { get; set; }
        public string MaCB { get; set; } // Mã Cấp Bậc
        public string TenCB { get; set; } // Tên Cấp Bậc (display only)
        
        // Staff Fields
        public string? ChucVu { get; set; }
        public string? TenCS { get; set; }
        public string? GioiTinh { get; set; }
        public decimal? LuongCoBan { get; set; }
    }
}
