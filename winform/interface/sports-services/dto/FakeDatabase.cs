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
}