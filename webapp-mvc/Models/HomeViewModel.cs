namespace webapp_mvc.Models
{
    public class HomeViewModel
    {
        // Stats
        public int TongSan { get; set; }
        public int TongKhachHang { get; set; }
        public int TongCoSo { get; set; }
        public decimal TyLeDungSan { get; set; }

        // Popular Courts
        public List<SanNoiBat> DanhSachSanNoiBat { get; set; } = new List<SanNoiBat>();
    }

    public class SanNoiBat
    {
        public string MaSan { get; set; } = string.Empty;
        public string TenLoaiSan { get; set; } = string.Empty;
        public string TenCoSo { get; set; } = string.Empty;
        public string DiaChi { get; set; } = string.Empty;
        public decimal GiaThue { get; set; }
        public string HinhAnh { get; set; } = string.Empty;
    }
}
