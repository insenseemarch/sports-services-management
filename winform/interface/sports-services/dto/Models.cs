using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportsServices.Dto
{
    // Cấu trúc cho Doanh Thu
    public class DoanhThuDTO
    {
        public string Thang { get; set; }
        public decimal SoTien { get; set; }
    }

    // Cấu trúc cho Tỷ lệ đặt sân (Online/Trực tiếp)
    public class NguonDatDTO
    {
        public string Nguon { get; set; } // "Online", "Trực tiếp", "Vãng lai"
        public int SoLuong { get; set; }
    }

    // Cấu trúc cho Nhân viên chấm công
    public class NhanVienDTO
    {
        public string MaNV { get; set; }
        public string TenNV { get; set; }
        public int TongGioLam { get; set; }
        public decimal LuongTamTinh { get; set; }
    }

    // Cấu trúc KPI tổng hợp (4 cái thẻ màu)
    public class KPIDTO
    {
        public decimal TongDoanhThu { get; set; }
        public double TyLeLapDay { get; set; }
        public int TongLuotDat { get; set; }
        public decimal TienMatDoHuy { get; set; }
    }
}
