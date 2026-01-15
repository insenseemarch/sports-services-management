using System.ComponentModel.DataAnnotations;

namespace webapp_mvc.Models
{
    public class NhanSuViewModel
    {
        public List<NhanVienItem> DanhSachNhanVien { get; set; } = new List<NhanVienItem>();
        public string SearchKeyword { get; set; } = string.Empty;

        // Create
        [Required] public string HoTen { get; set; }
        public string GioiTinh { get; set; } = "Nam";
        public DateTime NgaySinh { get; set; } = DateTime.Now.AddYears(-20);
        public string SDT { get; set; }
        [Required] public string CCCD { get; set; } // Thay Email bằng CCCD (Bắt buộc trong DB)
        public string DiaChi { get; set; }
        public string ChucVu { get; set; }
        public decimal LuongCoBan { get; set; }
        public string MaCS { get; set; }
        
        // Account
        [Required] public string TenDangNhap { get; set; }
        [Required] public string MatKhau { get; set; }

        public List<CoSoItem> DanhSachCoSo { get; set; } = new List<CoSoItem>();
        public string? Message { get; set; }
        public bool IsSuccess { get; set; }
    }

    public class NhanVienItem
    {
        public string MaNV { get; set; } = string.Empty;
        public string HoTen { get; set; } = string.Empty;
        public string ChucVu { get; set; } = string.Empty;
        public string TenCoSo { get; set; } = string.Empty;
        public string SDT { get; set; } = string.Empty;
        public string CCCD { get; set; } = string.Empty;
        public string? Email { get; set; }
        public DateTime? NgaySinh { get; set; }
        public string? DiaChi { get; set; }
        public string MaCS { get; set; } = string.Empty;
        public decimal LuongCoBan { get; set; }
        public decimal Luong { get; set; }
        public string TenDangNhap { get; set; } = string.Empty;
        public string MatKhau { get; set; } = string.Empty;
        public string VaiTro { get; set; } = string.Empty;
        public string TrangThai { get; set; } = "Đang làm";
        public int SoCaTruc { get; set; }
    }
}
