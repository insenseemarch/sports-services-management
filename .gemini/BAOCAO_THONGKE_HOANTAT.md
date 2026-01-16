# ✅ BÁO CÁO HOÀN THÀNH - TÍNH NĂNG THỐNG KÊ

## 📋 YÊU CẦU ĐỀ BÀI vs THỰC TẾ

### ✅ 1. Doanh thu theo cơ sở, theo loại sân, theo tháng, quý, năm
**API:**
- `GetDoanhThuTheoCoso(tuNgay, denNgay)` - Doanh thu theo cơ sở
- `GetDoanhThuTheoLoaiSan(maCS, tuNgay, denNgay)` - Doanh thu theo loại sân
- `GetDoanhThuTheoThoiGian(kieu, nam)` - Doanh thu theo tháng/quý/năm

**Query:** JOIN COSO/LOAISAN → SAN → DATSAN → PHIEUDATSAN → HOADON
**Tính:** SUM(HD.TongTien)
**✅ ĐÚNG DATABASE**

---

### ✅ 2. Tỷ lệ sử dụng sân
**API:** `GetTyLeSuDungSan(maCS, tuNgay, denNgay)`

**Công thức:**
```
Tổng giờ đã đặt = SUM(DATEDIFF(MINUTE, GioBatDau, GioKetThuc) / 60.0)
Tổng giờ có thể = Số sân × Giờ hoạt động/ngày × Số ngày
Tỷ lệ = (Giờ đã đặt / Giờ có thể) × 100%
```

**Query:**
- Lấy giờ hoạt động thực tế từ `COSO.GioMoCua`, `GioDongCua`
- Đếm số sân từ bảng `SAN`
- Tính số ngày từ khoảng thời gian filter

**✅ ĐÚNG DATABASE** - Không hardcode, tính theo data thực tế

---

### ✅ 3. Số lượng đặt online và đặt trực tiếp
**API:** `GetThongKeDatSan(tuNgay, denNgay)`

**Query:**
```sql
SELECT 
    CASE WHEN KenhDat = N'Website' THEN N'Online' ELSE N'Trực tiếp' END AS HinhThuc,
    COUNT(*) AS SoLuong
FROM PHIEUDATSAN
WHERE TrangThai NOT IN (N'Hủy')
GROUP BY CASE WHEN KenhDat = N'Website' THEN N'Online' ELSE N'Trực tiếp' END
```

**✅ ĐÚNG DATABASE** - Dựa vào cột `KenhDat`

---

### ✅ 4. Tình hình hủy sân, no-show và số tiền bị mất do hủy
**API:** `GetThongKeHuySan(tuNgay, denNgay)` - **ĐÃ SỬA**

**Query:**
```sql
SELECT 
    COUNT(DISTINCT PDS.MaDatSan) AS SoLuongHuy,
    SUM(CASE WHEN HD.HinhThucTT LIKE N'%phạt%' THEN HD.ThanhTien ELSE 0 END) AS TienPhat,
    (SELECT COUNT(*) FROM PHIEUDATSAN WHERE TrangThai IN (N'No-Show', N'Vắng mặt')) AS SoNoShow
FROM PHIEUDATSAN PDS
LEFT JOIN HOADON HD ON PDS.MaDatSan = HD.MaPhieu
WHERE PDS.TrangThai IN (N'Đã hủy', N'Hủy')
```

**✅ ĐÚNG DATABASE** - Lấy tiền phạt thực tế từ `HOADON` (không ước lượng 10%)

---

### ✅ 5. Dịch vụ kèm theo được sử dụng nhiều nhất
**API:** `GetThongKeDichVu(maCS, top)`

**Query:** JOIN CT_DICHVUDAT → DICHVU
**Tính:** COUNT(*) hoặc SUM(SoLuong)
**✅ ĐÚNG DATABASE**

---

### ✅ 6. Thời gian làm việc của nhân viên theo tháng, quý, năm
**API:** `GetThongKeNhanSu(kieu, nam)`

**Query:** JOIN THAMGIACATRUC → CATRUC → NHANVIEN
**Tính:** SUM(SoGio) hoặc COUNT(Ca) theo tháng/quý/năm
**✅ ĐÚNG DATABASE**

---

## 🔧 NHỮNG GÌ ĐÃ SỬA/BỔ SUNG

### 1. ✅ Sửa `GetThongKeHuySan`
**Trước:** `SUM(HD.TongTien * 0.1)` - Ước lượng sai
**Sau:** `SUM(CASE WHEN HD.HinhThucTT LIKE N'%phạt%' THEN HD.ThanhTien ELSE 0 END)` - Lấy đúng tiền phạt

### 2. ✅ Thêm mới `GetTyLeSuDungSan` (version đúng)
- Tính theo giờ hoạt động thực tế từ `COSO`
- Hỗ trợ filter theo cơ sở và khoảng thời gian
- Không hardcode `365 * 16`

### 3. ✅ Xóa API cũ sai
- Xóa version cũ của `GetTyLeSuDungSan` (hardcode sai)

---

## 📊 DANH SÁCH API HOÀN CHỈNH

| STT | API | Mô tả | Params |
|-----|-----|-------|--------|
| 1 | `GetDoanhThuTheoCoso` | Doanh thu theo cơ sở | tuNgay, denNgay |
| 2 | `GetDoanhThuTheoLoaiSan` | Doanh thu theo loại sân | maCS, tuNgay, denNgay |
| 3 | `GetDoanhThuTheoThoiGian` | Doanh thu theo thời gian | kieu (thang/quy/nam), nam |
| 4 | `GetTyLeSuDungSan` | Tỷ lệ sử dụng sân | maCS, tuNgay, denNgay |
| 5 | `GetThongKeDatSan` | Đặt online/trực tiếp | tuNgay, denNgay |
| 6 | `GetThongKeHuySan` | Hủy sân & tiền phạt | tuNgay, denNgay |
| 7 | `GetThongKeDichVu` | Dịch vụ phổ biến | maCS, top |
| 8 | `GetThongKeNhanSu` | Thời gian làm việc NV | kieu, nam |

---

## ✅ KẾT LUẬN

**TẤT CẢ YÊU CẦU ĐỀ BÀI ĐÃ ĐƯỢC ĐÁP ỨNG:**
- ✅ Doanh thu: Theo cơ sở, loại sân, tháng/quý/năm
- ✅ Tỷ lệ sử dụng sân: Tính đúng theo giờ hoạt động thực tế
- ✅ Đặt online/trực tiếp: Phân loại theo KenhDat
- ✅ Hủy sân & tiền phạt: Lấy từ HOADON thực tế
- ✅ Dịch vụ phổ biến: Top N dịch vụ
- ✅ Thời gian làm việc NV: Theo tháng/quý/năm

**TẤT CẢ API ĐỀU LẤY ĐÚNG DỮ LIỆU TỪ DATABASE**
- Không hardcode
- Không ước lượng
- JOIN đúng bảng
- Filter đúng điều kiện

---

## 🎨 BƯỚC TIẾP THEO (TÙY CHỌN)

Nếu muốn cải thiện giao diện:
1. Kiểm tra View có dùng Chart.js chưa
2. Thêm biểu đồ đẹp (bar chart, pie chart, line chart)
3. Thêm export Excel/PDF

**Server đang chạy tại:** http://localhost:5000
**Trang thống kê:** http://localhost:5000/Management/ThongKe
