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
        public string MaNV { get; set; }
        public string HoTen { get; set; }
        public string ChucVu { get; set; }
        public string TenCoSo { get; set; }
        public string SDT { get; set; }
        public decimal LuongCoBan { get; set; }
        public string TenDangNhap { get; set; }
    }
}
