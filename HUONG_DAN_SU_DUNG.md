# 📖 HƯỚNG DẪN SỬ DỤNG - ĐẶT SÂN TRỰC TIẾP (Lễ Tân)

## 🎯 TỔNG QUAN

Trang **Đặt Sân Trực Tiếp** cho phép lễ tân:
1. ✅ Tìm kiếm khách hàng hiện có
2. ✅ Tạo khách hàng mới nhanh chóng
3. ✅ Đặt sân cho khách
4. ✅ Đặt dịch vụ cho khách (bóng, dụng cụ, đồng phục, v.v.)

---

## 📋 TAB 1: ĐẶT SÂN

### Bước 1: Tìm Khách Hàng
1. Nhập **tên**, **SDT**, hoặc **CCCD** vào ô tìm kiếm
2. Danh sách khách hàng sẽ hiển thị tự động (sau 300ms)
3. Nhấn vào khách hàng cần đặt

**Nếu khách không có trong hệ thống:**
- Nhấn nút **"Tạo Khách Mới"**
- Điền: Họ tên (*), SDT (*), CCCD, Email, Địa chỉ
- Nhấn **"Tạo"** → Khách sẽ được thêm vào hệ thống tự động

### Bước 2: Chọn Sân
1. **Cơ Sở**: Chọn cơ sở thể thao (VD: Cơ sở A, Cơ sở B)
2. **Loại Sân**: Loại sân sẽ tự load (VD: Cầu lông, Bóng bàn, Tennis)
3. **Sân**: Danh sách sân sẽ hiển thị với giá cơ bản
4. Chọn sân cần đặt

### Bước 3: Chọn Thời Gian
1. **Ngày Đặt**: Chọn ngày (mặc định là hôm nay)
2. **Từ Giờ**: Nhập giờ bắt đầu (VD: 15:00)
3. **Đến Giờ**: Nhập giờ kết thúc (VD: 17:00)
4. *Lưu ý*: Nếu có trùng giờ, hệ thống sẽ báo lỗi

### Bước 4: Hoàn Tất Đặt Sân
1. *(Tùy chọn)* Thêm ghi chú
2. Nhấn **"Tạo Phiếu Đặt Sân"**
3. Sau khi thành công, phiếu sẽ hiển thị ở bảng dưới

---

## 🛒 TAB 2: ĐẶT DỊCH VỤ

### Bước 1: Tìm/Tạo Khách Hàng
*Tương tự như Tab 1 - Tìm kiếm và tạo khách hàng*

### Bước 2: Chọn Dịch Vụ
1. **Dịch Vụ**: Chọn dịch vụ từ dropdown
   - Bóng badminton
   - Vợt tennis
   - Đồng phục
   - Các dịch vụ khác...
2. Giá dịch vụ sẽ hiển thị tự động

### Bước 3: Nhập Số Lượng
1. **Số Lượng**: Nhập số lượng cần đặt
2. Giá sẽ tự động tính toán

### Bước 4: Hoàn Tất Đặt Dịch Vụ
1. *(Tùy chọn)* Thêm ghi chú
2. Nhấn **"Tạo Phiếu Dịch Vụ"**
3. Phiếu sẽ hiển thị ở bảng dưới

---

## 📊 BẢNG PHIẾU GẦN ĐÂY

### Tab Đặt Sân - Hiển Thị:
- **Mã Phiếu**: ID phiếu đặt
- **Khách Hàng**: Tên khách
- **Sân**: Tên sân được đặt
- **Ngày**: Ngày đặt
- **Khung Giờ**: Thời gian từ-đến
- **Trạng Thái**: Đã đặt, Đang sử dụng, v.v.
- **Thao Tác**: Xem chi tiết (nếu cần)

### Tab Dịch Vụ - Hiển Thị:
- **Mã Phiếu**: ID phiếu dịch vụ
- **Khách Hàng**: Tên khách
- **Dịch Vụ**: Tên dịch vụ
- **SL**: Số lượng
- **Giá**: Tổng tiền
- **Trạng Thái**: Chờ thanh toán, Đã thanh toán, v.v.

---

## ⚠️ LƯU Ý QUAN TRỌNG

### Kiểm Tra Trùng Giờ
- ❌ Không thể đặt sân trùng giờ
- Hệ thống tự động kiểm tra và báo lỗi nếu giờ trùng

### Tồn Kho Dịch Vụ
- Chỉ có thể đặt dịch vụ có tồn kho
- Số lượng tồn kho sẽ giảm khi đặt
- Nếu hết hàng: "Chọn dịch vụ khác hoặc liên hệ quán"

### Dữ Liệu Khách Hàng
- **Họ Tên** (*): Bắt buộc
- **SDT** (*): Bắt buộc (dùng để kiểm tra duplicate)
- CCCD, Email, Địa chỉ: Tùy chọn

### Thông Báo
- 🟢 **Xanh**: Đặt thành công → Bảng tự động cập nhật
- 🔴 **Đỏ**: Lỗi → Kiểm tra lại thông tin

---

## 🎨 GIAO DIỆN

### Màu Sắc
- **Xanh lá (Lime)**: #cce830 - Accent color
- **Đen**: #1a1a1a - Text, header
- **Xám sáng**: #f8f9fa - Background

### Các Nút
- **"Tạo Phiếu..."** (Xanh lá): Xác nhận hành động
- **"Làm Mới"** (Xám): Reset form
- **"Tạo Khách Mới"**: Mở modal tạo khách
- **"X"**: Xóa khách hàng đã chọn

---

## 🔍 TROUBLESHOOTING

### Vấn đề: Khách không xuất hiện
**Giải pháp**:
1. Kiểm tra họ tên/SDT/CCCD đúng
2. Nhấn vào danh sách nếu có
3. Nếu vẫn không có → "Tạo Khách Mới"

### Vấn đề: "Sân đã được đặt"
**Giải pháp**:
1. Chọn khung giờ khác
2. Chọn ngày khác
3. Chọn sân khác

### Vấn đề: Dịch vụ không đặt được
**Giải pháp**:
1. Kiểm tra số lượng tồn kho
2. Nếu hết → Thông báo khách
3. Chọn dịch vụ khác

---

## 📞 HỖ TRỢ

Nếu gặp vấn đề:
- Kiểm tra lại thông tin nhập
- Đảm bảo khách hàng đã được chọn
- Liên hệ IT nếu lỗi hệ thống

---

**Version**: 1.0
**Last Updated**: Tháng 1, 2025
