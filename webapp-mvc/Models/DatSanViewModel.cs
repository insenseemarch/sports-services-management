using System.ComponentModel.DataAnnotations;

namespace webapp_mvc.Models
{
    public class DatSanViewModel
    {
        public string MaCS { get; set; }
        public string MaLS { get; set; }
        
        [DataType(DataType.Date)]
        public DateTime NgayDat { get; set; } = DateTime.Today;
        
        public TimeSpan GioBatDau { get; set; } = new TimeSpan(7, 0, 0);
        public TimeSpan GioKetThuc { get; set; } = new TimeSpan(8, 0, 0);
        
        public string SelectedMaSan { get; set; }

        public List<CoSoItem> DanhSachCoSo { get; set; } = new List<CoSoItem>();
        public List<LoaiSanItem> DanhSachLoaiSan { get; set; } = new List<LoaiSanItem>();
        public List<SanItem> DanhSachSanTrong { get; set; } = new List<SanItem>();
    }

    public class CoSoItem { public string MaCS { get; set; } public string TenCS { get; set; } }
    public class LoaiSanItem { public string MaLS { get; set; } public string TenLS { get; set; } }
    public class SanItem { 
        public string MaSan { get; set; } 
        public string TenSan { get; set; }
        public string TenLoaiSan { get; set; }
        public string TenCoSo { get; set; }
        public int SucChua { get; set; }
        public decimal GiaGio { get; set; }
        public string TinhTrang { get; set; }
        
        // New Props for Better Pricing Display
        public string HienThiGia { get; set; }
        public string DonViTinh { get; set; }
        public decimal MinGia { get; set; }
        public decimal MaxGia { get; set; }
        
        // Trạng thái bảo trì
        public bool DangBaoTri { get; set; }
    }
}
