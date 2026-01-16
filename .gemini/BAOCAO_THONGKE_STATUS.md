# BÁO CÁO KIỂM TRA TÍNH NĂNG THỐNG KÊ

## YÊU CẦU ĐỀ BÀI:

1. ✅ Doanh thu theo cơ sở, theo loại sân, theo tháng, quý, năm
2. ❓ Tỷ lệ sử dụng sân (tổng số giờ được đặt / tổng số giờ có thể cho thuê)
3. ✅ Số lượng đặt online và đặt trực tiếp
4. ❓ Tình hình hủy sân, no-show và số tiền bị mất do hủy
5. ✅ Dịch vụ kèm theo được sử dụng nhiều nhất
6. ✅ Thời gian làm việc của nhân viên theo tháng, quý, năm

## TRẠNG THÁI HIỆN TẠI:

### ✅ ĐÃ CÓ (cần kiểm tra logic):

1. **GetDoanhThuTheoCoso** (dòng 1122)
   - Query: JOIN COSO -> SAN -> DATSAN -> PHIEUDATSAN -> HOADON
   - Tính: SUM(HD.TongTien)
   - Filter: Theo ngày
   - ⚠️ THIẾU: Group theo tháng/quý/năm

2. **GetDoanhThuTheoLoaiSan** (dòng 1177)
   - Query: JOIN LOAISAN -> SAN -> DATSAN -> PHIEUDATSAN -> HOADON
   - Tính: SUM(HD.TongTien)
   - Filter: Theo cơ sở, theo ngày
   - ⚠️ THIẾU: Group theo tháng/quý/năm

3. **GetThongKeDatSan** (dòng 1332)
   - Cần kiểm tra xem có đếm online/trực tiếp không

4. **GetThongKeHuySan** (dòng 1381)
   - Cần kiểm tra xem có tính tiền mất không

5. **GetThongKeDichVu** (dòng 1426)
   - Top dịch vụ được dùng nhiều nhất

6. **GetThongKeNhanSu** (dòng 1476)
   - Thời gian làm việc theo tháng/quý/năm

### ❌ THIẾU:

1. **Tỷ lệ sử dụng sân**
   - Cần tính: (Tổng giờ đã đặt) / (Tổng giờ có thể cho thuê)
   - Công thức: Số giờ hoạt động × Số sân × Số ngày

2. **Số tiền bị mất do hủy**
   - Cần query bảng HOADON với điều kiện là hóa đơn phạt hủy

3. **Biểu đồ đẹp**
   - Cần kiểm tra View có dùng Chart.js chưa

## KẾ HOẠCH HÀNH ĐỘNG:

1. Kiểm tra từng API xem query có đúng không
2. Bổ sung API thiếu
3. Thêm group theo tháng/quý/năm
4. Kiểm tra và cải thiện biểu đồ
