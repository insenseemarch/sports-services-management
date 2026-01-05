using System;
using System.Collections.Generic;

namespace SportsServices.Dto
{
    // Class static này đóng vai trò như một Database tạm thời
    public static class FakeDatabase
    {
        // Các bảng dữ liệu (List)
        public static List<CoSo> CoSos { get; set; } = new List<CoSo>();
        public static List<NhanVien> NhanViens { get; set; } = new List<NhanVien>();

        // Hàm này chạy 1 lần lúc mở App để nạp dữ liệu mẫu
        public static void InitData()
        {
            // 1. Tạo dữ liệu Cơ sở
            CoSos = new List<CoSo>
            {
                new CoSo { MaCoSo = "CS1", TenCoSo = "Sân Cầu Giấy (Hà Nội)" },
                new CoSo { MaCoSo = "CS2", TenCoSo = "Sân Quận 7 (TP.HCM)" },
                new CoSo { MaCoSo = "CS3", TenCoSo = "Sân Hải Châu (Đà Nẵng)" },
                new CoSo { MaCoSo = "CS4", TenCoSo = "Sân Ninh Kiều (Cần Thơ)" }
            };

            // 2. Tạo dữ liệu Nhân viên (khoảng 10 người)
            NhanViens = new List<NhanVien>
            {
                new NhanVien { MaNV = "NV01", HoTen = "Nguyễn Văn A", NgaySinh = new DateTime(1990, 1, 1), GioiTinh = "Nam", ChucVu = "Quản lý", MaCoSo = "CS2", LuongCB = 15000000, SDT = "0901234567", CCCD = "001090000001", DiaChi = "TP.HCM" },
                new NhanVien { MaNV = "NV02", HoTen = "Trần Thị B", NgaySinh = new DateTime(1995, 5, 10), GioiTinh = "Nữ", ChucVu = "Thu ngân", MaCoSo = "CS2", LuongCB = 8000000, SDT = "0901234568", CCCD = "001095000002", DiaChi = "TP.HCM" },
                new NhanVien { MaNV = "NV03", HoTen = "Lê Văn C", NgaySinh = new DateTime(1998, 8, 20), GioiTinh = "Nam", ChucVu = "Bảo vệ", MaCoSo = "CS1", LuongCB = 7000000, SDT = "0901234569", CCCD = "001098000003", DiaChi = "Hà Nội" },
                new NhanVien { MaNV = "NV04", HoTen = "Phạm Thị D", NgaySinh = new DateTime(2000, 12, 12), GioiTinh = "Nữ", ChucVu = "Lễ tân", MaCoSo = "CS1", LuongCB = 7500000, SDT = "0901234570", CCCD = "001200000004", DiaChi = "Hà Nội" },
                new NhanVien { MaNV = "NV05", HoTen = "Hoàng Văn E", NgaySinh = new DateTime(1992, 3, 15), GioiTinh = "Nam", ChucVu = "Kỹ thuật", MaCoSo = "CS3", LuongCB = 10000000, SDT = "0901234571", CCCD = "001092000005", DiaChi = "Đà Nẵng" }
            };
        }
    }
    public static class KhoDuLieuBaoCao
    {
        // 1. Lấy số liệu KPI Tổng hợp
        public static KPIDTO GetSoLieuKPI()
        {
            return new KPIDTO
            {
                TongDoanhThu = 156000000,
                TyLeLapDay = 75.5, // 75.5%
                TongLuotDat = 420,
                TienMatDoHuy = 3500000 // Mất 3.5 triệu do hủy sân
            };
        }

        // 2. Lấy danh sách Doanh thu 3 tháng gần nhất
        public static List<DoanhThuDTO> GetDoanhThu()
        {
            return new List<DoanhThuDTO> {
                new DoanhThuDTO { Thang = "Tháng 9", SoTien = 45000000 },
                new DoanhThuDTO { Thang = "Tháng 10", SoTien = 52000000 },
                new DoanhThuDTO { Thang = "Tháng 11", SoTien = 59000000 }
            };
        }

        // 3. Lấy tỷ lệ nguồn đặt (Vẽ biểu đồ tròn)
        public static List<NguonDatDTO> GetNguonDat()
        {
            return new List<NguonDatDTO> {
                new NguonDatDTO { Nguon = "App/Online", SoLuong = 300 },
                new NguonDatDTO { Nguon = "Trực tiếp", SoLuong = 100 },
                new NguonDatDTO { Nguon = "Điện thoại", SoLuong = 20 }
            };
        }

        // 4. Lấy danh sách Nhân viên (Hiện xuống bảng dưới cùng)
        public static List<NhanVienDTO> GetNhanVien()
        {
            return new List<NhanVienDTO> {
                new NhanVienDTO { MaNV="NV01", TenNV="Nguyễn Văn A", TongGioLam=160, LuongTamTinh=5000000 },
                new NhanVienDTO { MaNV="NV02", TenNV="Trần Thị B", TongGioLam=180, LuongTamTinh=6500000 },
                new NhanVienDTO { MaNV="NV03", TenNV="Lê C", TongGioLam=150, LuongTamTinh=4800000 },
            };
        }
    }
    public class DonNghiPhep
    {
        public string MaDon { get; set; }

        // Thông tin người nghỉ
        public string MaNV { get; set; }       // Hiện lên lưới
        public string TenNV { get; set; }      // Hiện xuống chi tiết

        public DateTime NgayNghi { get; set; }
        public string CaNghi { get; set; }

        // Thông tin người thay thế
        public string MaNVThayThe { get; set; }  // Hiện lên lưới
        public string TenNVThayThe { get; set; } // Hiện xuống chi tiết

        public string TrangThai { get; set; }
        public string Lydo { get; set; }
        public string ChiNhanh { get; set; }
    }

    // 2. Tạo kho dữ liệu giả (List)
    public static class KhoDonPhep
    {
        public static List<DonNghiPhep> DanhSachDon = new List<DonNghiPhep>();

        public static void TaoDuLieuMau()
        {
            if (DanhSachDon.Count > 0) return;

            // Dữ liệu mẫu khớp với 4 chi nhánh
            DanhSachDon.Add(new DonNghiPhep
            {
                MaDon = "DN01",
                MaNV = "NV001",
                TenNV = "Nguyễn Văn A",
                NgayNghi = DateTime.Now,
                CaNghi = "Sáng",
                MaNVThayThe = "NV002",
                TenNVThayThe = "Trần Văn B",
                Lydo = "Sốt cao",
                TrangThai = "Chờ duyệt",
                ChiNhanh = "Thành Phố Hồ Chí Minh"
            });

            DanhSachDon.Add(new DonNghiPhep
            {
                MaDon = "DN02",
                MaNV = "NV010",
                TenNV = "Lê Thị C",
                NgayNghi = DateTime.Now.AddDays(1),
                CaNghi = "Cả ngày",
                MaNVThayThe = "NV011",
                TenNVThayThe = "Phạm Văn D",
                Lydo = "Việc gia đình",
                TrangThai = "Chờ duyệt",
                ChiNhanh = "Hà Nội"
            });
        }
    }

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

    public class CoSo
    {
        public string MaCoSo { get; set; }
        public string TenCoSo { get; set; }
    }

    public class NhanVien
    {
        public string MaNV { get; set; }
        public string HoTen { get; set; }
        public DateTime NgaySinh { get; set; }
        public string GioiTinh { get; set; }
        public string ChucVu { get; set; }
        public string MaCoSo { get; set; } // Khoá ngoại liên kết với CoSo
        public decimal LuongCB { get; set; }
        public string SDT { get; set; }
        public string CCCD { get; set; }
        public string DiaChi { get; set; }
    }
}