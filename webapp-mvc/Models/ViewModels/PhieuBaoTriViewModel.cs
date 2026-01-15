using System;

namespace webapp_mvc.Models.ViewModels
{
    public class PhieuBaoTriViewModel
    {
        public long MaPhieu { get; set; }
        public string MaSan { get; set; }
        public DateTime NgayBatDau { get; set; }
        public DateTime? NgayKetThucDuKien { get; set; }
        public DateTime? NgayKetThucThucTe { get; set; }
        public string MoTaSuCo { get; set; }
        public decimal ChiPhi { get; set; }
        public string TrangThai { get; set; }
        public string TenLS { get; set; }
        public string TenCS { get; set; }
        public string DiaChi { get; set; }
        public string TenNV { get; set; }
        public string TinhTrangSan { get; set; }
    }
}
