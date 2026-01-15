# KẾ HOẠCH IMPLEMENTATION 12 CHỨC NĂNG

## PHASE 1: AUTHENTICATION & USER MANAGEMENT (Chức năng 1, 5)

### ✅ Chức năng 1: Đăng ký khách hàng
**Controller**: `TaiKhoanController.cs`
**Views**: 
- `Views/TaiKhoan/DangKy.cshtml`
- `Views/TaiKhoan/DangNhap.cshtml`

**Fields theo WinForm**:
- Họ tên (required)
- Username (required, unique)
- Password (required)
- Confirm Password (required, match)
- SDT (required)
- Email (required)
- CCCD (optional)
- Địa chỉ (optional)
- Ngày sinh (optional)
- Giới tính (optional)
- Checkbox: Là HSSV (nếu tick → upload ảnh thẻ SV)
- Upload ảnh thẻ SV (nếu HSSV = true)

**Validation**:
- Username chưa tồn tại
- Password match với Confirm Password
- Nếu tick HSSV → bắt buộc upload ảnh

**Database**:
```sql
INSERT INTO TAIKHOAN (MaTK, TenDangNhap, MatKhau, VaiTro, NgayDangKy)
INSERT INTO KHACHHANG (MaKH, HoTen, NgaySinh, CCCD, SDT, Email, DiaChi, LaHSSV, MaTK)
```

---

### ✅ Chức năng 5: Xem thông tin cá nhân
**Controller**: `HoSoController.cs`
**Views**:
- `Views/HoSo/Index.cshtml` (Khách hàng)
- `Views/HoSo/NhanVien.cshtml` (Nhân viên)

**Khách hàng**:
- Xem: Họ tên, CCCD, SDT, Email, Địa chỉ, Ngày sinh, Điểm tích lũy, Cấp bậc
- Sửa: Họ tên, SDT, Email, Địa chỉ
- Đổi mật khẩu

**Nhân viên**:
- Xem: Họ tên, Chức vụ, CCCD, SDT, Địa chỉ, Lương cơ bản, Tổng giờ làm
- Sửa: SDT, Địa chỉ
- Đổi mật khẩu

---

## PHASE 2: BOOKING SYSTEM (Chức năng 2, 3, 4)

### ✅ Chức năng 2: Đặt sân + Dịch vụ
**Controller**: `DatSanController.cs`
**Views**:
- `Views/DatSan/Index.cshtml` (Chọn sân)
- `Views/DatSan/ChonDichVu.cshtml` (Chọn dịch vụ)
- `Views/DatSan/XacNhan.cshtml` (Xác nhận đặt)

**Flow**:
1. Chọn cơ sở
2. Chọn loại sân
3. Chọn ngày đặt
4. Chọn khung giờ (hiển thị giá)
5. Kiểm tra sân trống (AJAX)
6. Chọn dịch vụ kèm theo (HLV, Dụng cụ, Nước uống...)
7. Chọn hình thức thanh toán (Online/Tại quầy)
8. Xác nhận đặt

**Validation**:
- Sân không trùng giờ
- Khách không đặt quá 2 sân/ngày
- Đặt trước ít nhất 2 giờ
- Thanh toán tại quầy: phải hoàn tất trong 30 phút

---

### ✅ Chức năng 3: Thay đổi/Hủy đặt sân
**Controller**: `LichSuDatSanController.cs`
**Views**:
- `Views/LichSuDatSan/Index.cshtml` (Danh sách)
- `Views/LichSuDatSan/ChiTiet.cshtml` (Chi tiết)

**Actions**:
- Xem lịch sử đặt sân
- Đổi giờ (nếu còn sân trống)
- Hủy sân (tính phí phạt):
  - Hủy trước 24h: phạt 10%
  - Hủy trong 24h: phạt 50%
  - No-show: mất 100%

---

### ✅ Chức năng 4: Report lỗi sân
**Controller**: `BaoCaoLoiController.cs`
**Views**:
- `Views/BaoCaoLoi/Index.cshtml` (Danh sách)
- `Views/BaoCaoLoi/TaoBaoCao.cshtml` (Tạo mới)

**Fields**:
- Chọn sân
- Mô tả sự cố
- Upload ảnh (optional)
- Gửi báo cáo

---

## PHASE 3: STAFF MANAGEMENT (Chức năng 6, 7, 8)

### ✅ Chức năng 6: Quản lý nhân viên
**Controller**: `NhanSuController.cs`
**Views**:
- `Views/NhanSu/Index.cshtml` (Danh sách)
- `Views/NhanSu/Them.cshtml` (Thêm mới)
- `Views/NhanSu/Sua.cshtml` (Sửa)

**Actions**:
- Xem danh sách nhân viên theo cơ sở
- Thêm nhân viên mới
- Sửa thông tin
- Xóa nhân viên
- Phân quyền

---

### ✅ Chức năng 7: Lịch làm việc & Nghỉ phép
**Controller**: `LichLamViecController.cs`
**Views**:
- `Views/LichLamViec/Index.cshtml` (Lịch tháng)
- `Views/LichLamViec/XinNghi.cshtml` (Xin nghỉ)

**Features**:
- Xem lịch ca trực trong tháng (Calendar view)
- Xin nghỉ phép (chọn ngày, ca, lý do)
- Xem lịch sử nghỉ phép

---

### ✅ Chức năng 8: Phân ca & Duyệt nghỉ phép
**Controller**: `QuanLyController.cs`
**Views**:
- `Views/QuanLy/PhanCa.cshtml`
- `Views/QuanLy/DuyetPhep.cshtml`

**Phân ca**:
- Tạo ca trực mới
- Phân công nhân viên
- Xem lịch ca theo tuần/tháng

**Duyệt phép**:
- Xem danh sách đơn chờ duyệt
- Duyệt/Từ chối
- Tìm người thay thế

---

## PHASE 4: MAINTENANCE & FINANCE (Chức năng 9, 10, 11)

### ✅ Chức năng 9: Bảo trì sân
**Controller**: `BaoTriController.cs`
**Views**:
- `Views/BaoTri/Index.cshtml`
- `Views/BaoTri/TaoPhieu.cshtml`

**Features**:
- Xem danh sách phiếu bảo trì
- Tạo phiếu bảo trì mới
- Cập nhật tiến độ
- Cập nhật trạng thái sân

---

### ✅ Chức năng 10: Hóa đơn
**Controller**: `HoaDonController.cs`
**Views**:
- `Views/HoaDon/Index.cshtml`
- `Views/HoaDon/ChiTiet.cshtml`

**Features**:
- Xem danh sách hóa đơn
- Xuất hóa đơn (PDF)
- In hóa đơn
- Gửi email hóa đơn

---

### ✅ Chức năng 11: Cấu hình hệ thống
**Controller**: `HeThongController.cs`
**Views**:
- `Views/HeThong/BangGia.cshtml`
- `Views/HeThong/ThamSo.cshtml`

**Settings**:
- Bảng giá theo khung giờ
- Thời gian đặt tối thiểu/tối đa
- Hạn mức đặt sân
- Chính sách hủy
- Chính sách ưu đãi

---

## PHASE 5: REPORTING (Chức năng 12)

### ✅ Chức năng 12: Báo cáo thống kê
**Controller**: `BaoCaoController.cs`
**Views**:
- `Views/BaoCao/DoanhThu.cshtml`
- `Views/BaoCao/TanSuat.cshtml`
- `Views/BaoCao/TonKho.cshtml`

**Reports**:
- Doanh thu theo cơ sở/tháng/quý/năm
- Tỷ lệ sử dụng sân
- Tần suất khách hàng
- Cảnh báo tồn kho
- Thống kê dịch vụ
- Hiệu suất nhân viên

---

## TIMELINE

**Week 1**: Phase 1 (Chức năng 1, 5)
**Week 2**: Phase 2 (Chức năng 2, 3, 4)
**Week 3**: Phase 3 (Chức năng 6, 7, 8)
**Week 4**: Phase 4 (Chức năng 9, 10, 11)
**Week 5**: Phase 5 (Chức năng 12)
**Week 6**: Testing & Bug fixes

---

## TECH STACK

- **Backend**: ASP.NET Core MVC (.NET 10)
- **Database**: SQL Server (existing schema)
- **Frontend**: Bootstrap 5 + jQuery
- **Authentication**: Session-based
- **File Upload**: IFormFile
- **PDF**: iTextSharp / DinkToPdf
- **Email**: MailKit
- **Charts**: Chart.js

---

## NEXT STEPS

1. ✅ Tạo TaiKhoanController
2. ✅ Tạo View Đăng ký
3. ✅ Tạo View Đăng nhập
4. ✅ Implement Session management
5. ✅ Test flow đăng ký → đăng nhập
