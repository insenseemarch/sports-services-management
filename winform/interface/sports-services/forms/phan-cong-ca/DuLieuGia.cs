using System;
using System.Collections.Generic;

public class ThongTinCa
{
    public string TenCa { get; set; }      // Ví dụ: "Ca Sáng"
    public DateTime Ngay { get; set; }     // Ngày làm việc
    public string NguoiQuanLy { get; set; } // Tên quản lý
    public string DanhSachNV { get; set; }  // Danh sách nhân viên (Lưu dạng chuỗi dài cho dễ)

    public string ChiNhanh { get; set; }
}

public static class KhoDuLieu
{
    // Đây là cái "Database tạm" của chúng ta
    public static List<ThongTinCa> DanhSachPhanCong = new List<ThongTinCa>();

    // Hàm tạo dữ liệu mẫu để khi bật lên nhìn cho đỡ trống
    public static void TaoDuLieuMau()
    {
        if (DanhSachPhanCong.Count > 0) return; // Đã có dữ liệu thì không tạo lại

        // Thêm mẫu 1 ca
        DanhSachPhanCong.Add(new ThongTinCa()
        {
            TenCa = "Ca Sáng (7h-11h)",
            Ngay = DateTime.Today, // Ngày hôm nay
            NguoiQuanLy = "NV001 - Nguyễn Văn A",
            DanhSachNV = "NV002 - Trần Thị B\nNV003 - Lê Văn C\nNV004 - Phạm Thị D"
        });
    }
}