namespace webapp_mvc.Models
{
    public class ThanhToanKhachHangViewModel
    {
        public long MaDatSan { get; set; }
        public string? MaSan { get; set; }
        public string? TenSan { get; set; }
        public string? TenCoSo { get; set; }
        public DateTime NgayDat { get; set; }
        public TimeSpan GioBatDau { get; set; }
        public TimeSpan GioKetThuc { get; set; }
        
        // Detailed Pricing Info
        public string? DonViTinh { get; set; } // Giờ, Trận, Ca
        public decimal SoLuongSan { get; set; } // 1.5 giờ, 2 trận
        public decimal DonGiaSan { get; set; } // 400k/trận

        public decimal TienSan { get; set; }
        public decimal TienDichVu { get; set; }
        public decimal TongCong { get; set; }
        public decimal GiamGia { get; set; }
        public decimal ThanhTien { get; set; }
        
        public List<DichVuDatItem> DanhSachDichVu { get; set; } = new List<DichVuDatItem>();
    }

    public class DichVuDatItem
    {
        public string? TenDV { get; set; }
        public int SoLuong { get; set; }
        public int SoGioThue { get; set; }
        public decimal ThanhTien { get; set; }
    }
}
