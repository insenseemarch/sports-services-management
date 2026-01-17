# DANH SÁCH CHỨC NĂNG HỆ THỐNG ĐẶT SÂN THỂ THAO - VIETSPORT

## I. PHÂN HỆ KHÁCH HÀNG (Customer Portal)

### 1. Đăng nhập / Đăng ký
- **Đăng nhập**: Form đăng nhập với username/password
- **Đăng ký**: Tạo tài khoản mới (Họ tên, CCCD, SDT, Email, Địa chỉ, Ngày sinh)
- **Quên mật khẩu**: Reset password qua email

### 2. Đặt Sân (FormDatSan)
- Chọn cơ sở
- Chọn loại sân (Bóng đá, Tennis, Cầu lông, Bóng rổ...)
- Chọn ngày đặt
- Chọn khung giờ (hiển thị giá theo khung giờ)
- Kiểm tra sân trống
- Xác nhận đặt sân
- Chọn hình thức thanh toán (Online/Tại quầy)

### 3. Đặt Dịch Vụ (FormDatDichVu)
- Thuê dụng cụ thể thao (Bóng, vợt, áo bib...)
- Thuê HLV cá nhân
- Đặt nước uống
- Thuê phòng VIP
- Thuê tủ đồ
- Các dịch vụ khác

### 4. Thay Đổi / Hủy Đặt Sân (FormDoiSan)
- Xem lịch sử đặt sân
- Đổi giờ (nếu còn sân trống)
- Hủy sân (tính phí phạt theo quy định)
- Xem trạng thái đặt sân

### 5. Hồ Sơ Cá Nhân (FrmXemProfileKhachHang)
- Xem thông tin cá nhân
- Cập nhật thông tin
- Đổi mật khẩu
- Xem điểm tích lũy
- Xem cấp bậc thành viên (Đồng, Bạc, Vàng, Bạch Kim...)

---

## II. PHÂN HỆ NHÂN VIÊN (Staff Portal)

### A. CHUNG (Tất cả nhân viên)

#### 1. Lịch Làm Việc (FormLichLamViec)
- Xem lịch ca trực của bản thân
- Đăng ký ca trực
- Xin nghỉ phép
- Xem lịch sử nghỉ phép

#### 2. Hồ Sơ Cá Nhân (FrmXemProfileNhanVien)
- Xem thông tin cá nhân
- Đổi mật khẩu
- Xem lương
- Xem tổng giờ làm việc

---

### B. LỄ TÂN (LE_TAN)

#### 1. Đặt Sân Trực Tiếp (FormDatSan)
- Tiếp nhận khách hàng đặt sân tại quầy
- Tìm kiếm khách hàng (theo SDT/CCCD)
- Tạo phiếu đặt sân mới
- In phiếu xác nhận
- Chuyển cho thu ngân thanh toán

---

### C. THU NGÂN (THU_NGAN)

#### 1. Thanh Toán & Hóa Đơn (FrmHoaDon)
- Xem danh sách phiếu đặt chờ thanh toán
- Tính tổng tiền (Sân + Dịch vụ)
- Áp dụng ưu đãi/giảm giá
- Thu tiền (Tiền mặt/Chuyển khoản/Thẻ)
- Xuất hóa đơn
- In hóa đơn
- Gửi hóa đơn qua email/SMS

---

### D. KỸ THUẬT (KY_THUAT)

#### 1. Quản Lý Bảo Trì (QuanLiBaoTri)
- Xem danh sách sân cần bảo trì
- Tạo phiếu bảo trì
- Cập nhật tiến độ bảo trì
- Ghi nhận chi phí bảo trì
- Cập nhật trạng thái sân (Bảo trì → Còn trống)
- Xem lịch sử bảo trì

---

### E. QUẢN LÝ (QUAN_LY)

#### 1. Quản Lý Nhân Sự (FrmDanhSachNhanVien)
- Xem danh sách nhân viên
- Thêm nhân viên mới (FrmThemNhanVien)
- Sửa thông tin nhân viên
- Xóa nhân viên
- Phân quyền
- Quản lý lương

#### 2. Phân Công Ca Trực (PhanCongCaTruc)
- Tạo ca trực mới
- Phân công nhân viên vào ca
- Xem lịch ca trực theo tuần/tháng
- Sửa/Xóa ca trực
- Xem chi tiết ca trực (frmChiTietCa)

#### 3. Phê Duyệt Nghỉ Phép (FrmPheDuyetDonNghiPhep)
- Xem danh sách đơn nghỉ phép chờ duyệt
- Duyệt/Từ chối đơn
- Tìm người thay thế
- Xem lịch sử nghỉ phép

#### 4. Báo Cáo Lỗi Sân (FormReportLoiSan)
- Xem danh sách báo cáo lỗi
- Tạo báo cáo lỗi mới
- Chuyển cho kỹ thuật xử lý
- Theo dõi tiến độ xử lý

#### 5. Báo Cáo Thống Kê (FrmBaoCao)
- **Doanh thu**:
  - Theo cơ sở
  - Theo loại sân
  - Theo tháng/quý/năm
- **Tỷ lệ sử dụng sân**:
  - Tổng giờ đặt / Tổng giờ khả dụng
- **Thống kê đặt sân**:
  - Số lượng đặt online
  - Số lượng đặt trực tiếp
- **Tình hình hủy sân**:
  - Số lượng hủy
  - Số tiền mất do hủy
  - Tỷ lệ No-show
- **Dịch vụ**:
  - Dịch vụ được sử dụng nhiều nhất
  - Doanh thu từ dịch vụ
- **Nhân sự**:
  - Tổng giờ làm việc theo tháng/quý/năm
  - Hiệu suất nhân viên

---

### F. QUẢN TRỊ HỆ THỐNG (IT)

#### 1. Thay Đổi Hệ Thống (FrmThayDoiHeThong)
- Quản lý tài khoản
- Thiết lập tham số hệ thống:
  - Bảng giá theo khung giờ
  - Thời gian đặt tối thiểu/tối đa
  - Hạn mức số sân mỗi khách được đặt
  - Chính sách hủy sân
  - Chính sách ưu đãi
- Sao lưu/Phục hồi dữ liệu
- Xem log hệ thống

---

## III. CHỨC NĂNG BỔ SUNG

### 1. Quản Lý Ưu Đãi
- Tạo chương trình khuyến mãi
- Áp dụng ưu đãi theo:
  - Cấp bậc thành viên
  - Học sinh/Sinh viên
  - Khung giờ
  - Ngày trong tuần
  - Combo nhiều giờ

### 2. Quản Lý Khung Giờ & Giá
- Thiết lập giá theo:
  - Loại sân
  - Khung giờ (Sáng/Chiều/Tối)
  - Ngày thường/Cuối tuần/Lễ
- Cập nhật giá theo thời gian

### 3. Quản Lý Cơ Sở & Sân
- Thêm/Sửa/Xóa cơ sở
- Thêm/Sửa/Xóa sân
- Cập nhật trạng thái sân
- Quản lý sức chứa

### 4. Quản Lý Dịch Vụ
- Thêm/Sửa/Xóa dịch vụ
- Quản lý tồn kho dịch vụ
- Cảnh báo tồn kho thấp
- Phân phối dịch vụ theo cơ sở

---

## IV. PHÂN QUYỀN HỆ THỐNG

| Chức năng | Khách hàng | Lễ tân | Thu ngân | Kỹ thuật | Quản lý | IT |
|-----------|------------|--------|----------|----------|---------|-----|
| Đặt sân online | ✅ | ❌ | ❌ | ❌ | ❌ | ❌ |
| Đặt sân trực tiếp | ❌ | ✅ | ❌ | ❌ | ✅ | ❌ |
| Thanh toán | ❌ | ❌ | ✅ | ❌ | ✅ | ❌ |
| Bảo trì sân | ❌ | ❌ | ❌ | ✅ | ✅ | ❌ |
| Quản lý nhân sự | ❌ | ❌ | ❌ | ❌ | ✅ | ❌ |
| Phân ca trực | ❌ | ❌ | ❌ | ❌ | ✅ | ❌ |
| Duyệt nghỉ phép | ❌ | ❌ | ❌ | ❌ | ✅ | ❌ |
| Báo cáo thống kê | ❌ | ❌ | ❌ | ❌ | ✅ | ❌ |
| Cấu hình hệ thống | ❌ | ❌ | ❌ | ❌ | ❌ | ✅ |
| Xem lịch làm việc | ❌ | ✅ | ✅ | ✅ | ✅ | ✅ |
| Hồ sơ cá nhân | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ |

---

## V. QUY TRÌNH NGHIỆP VỤ CHÍNH

### 1. Quy trình đặt sân online
1. Khách đăng nhập
2. Chọn cơ sở, loại sân, ngày, giờ
3. Hệ thống kiểm tra sân trống
4. Khách chọn dịch vụ kèm theo (nếu có)
5. Chọn hình thức thanh toán
6. Xác nhận đặt
7. Nhận phiếu xác nhận (Email/SMS)

### 2. Quy trình đặt sân trực tiếp
1. Khách đến quầy lễ tân
2. Lễ tân tìm kiếm/tạo mới khách hàng
3. Lễ tân tạo phiếu đặt sân
4. Chuyển cho thu ngân
5. Thu ngân thu tiền & xuất hóa đơn
6. In phiếu cho khách

### 3. Quy trình hủy/đổi sân
1. Khách yêu cầu hủy/đổi
2. Hệ thống kiểm tra thời gian
3. Tính phí phạt (nếu có)
4. Cập nhật trạng thái
5. Hoàn tiền (nếu đủ điều kiện)

### 4. Quy trình bảo trì sân
1. Phát hiện sự cố (Khách báo hoặc Kỹ thuật phát hiện)
2. Tạo phiếu bảo trì
3. Cập nhật trạng thái sân → "Bảo trì"
4. Kỹ thuật xử lý
5. Ghi nhận chi phí
6. Hoàn thành → Cập nhật trạng thái → "Còn trống"

---

## VI. CÔNG NGHỆ SỬ DỤNG

### WinForm (Hiện tại)
- .NET Framework
- SQL Server
- ADO.NET

### Web MVC (Cần implement)
- ASP.NET Core MVC
- SQL Server
- Entity Framework Core / ADO.NET
- Bootstrap 5
- JavaScript/jQuery

---

## VII. CẤU TRÚC DATABASE

### Bảng chính:
1. **TAIKHOAN** - Tài khoản đăng nhập
2. **KHACHHANG** - Thông tin khách hàng
3. **NHANVIEN** - Thông tin nhân viên
4. **COSO** - Cơ sở thể thao
5. **SAN** - Sân bãi
6. **LOAISAN** - Loại sân
7. **PHIEUDATSAN** - Phiếu đặt sân
8. **DATSAN** - Chi tiết sân được đặt
9. **DICHVU** - Dịch vụ
10. **CT_DICHVUDAT** - Chi tiết dịch vụ đặt
11. **HOADON** - Hóa đơn
12. **CATRUC** - Ca trực
13. **DONNGHIPHEP** - Đơn nghỉ phép
14. **PHIEUBAOTRI** - Phiếu bảo trì
15. **BAOCAOTHONGKE** - Báo cáo thống kê
16. **KHUNGGIO** - Khung giờ & giá
17. **UUDAI** - Ưu đãi
18. **CAPBAC** - Cấp bậc thành viên

---

## VIII. CHECKLIST IMPLEMENT WEB MVC

### ✅ Đã hoàn thành:
- [x] Thiết kế giao diện trang chủ
- [x] Hiển thị dữ liệu thực từ database
- [x] Navbar & Layout
- [x] HomeViewModel & Controller

### ⏳ Cần làm tiếp:
- [ ] Đăng nhập / Đăng ký
- [ ] Phân quyền (Customer/Staff)
- [ ] Đặt sân (Customer)
- [ ] Đặt dịch vụ (Customer)
- [ ] Lịch sử đặt sân (Customer)
- [ ] Hồ sơ cá nhân (Customer)
- [ ] Đặt sân trực tiếp (Lễ tân)
- [ ] Thanh toán & Hóa đơn (Thu ngân)
- [ ] Quản lý bảo trì (Kỹ thuật)
- [ ] Quản lý nhân sự (Quản lý)
- [ ] Phân ca trực (Quản lý)
- [ ] Duyệt nghỉ phép (Quản lý)
- [ ] Báo cáo thống kê (Quản lý)
- [ ] Cấu hình hệ thống (IT)
