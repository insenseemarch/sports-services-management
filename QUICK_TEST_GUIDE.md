# Quick Testing Checklist - Receptionist Direct Booking Page

## ✅ Build Status
- **Result:** SUCCESS ✅
- **Errors:** 0
- **Warnings:** 114 (pre-existing, nullable reference warnings)
- **Time:** Ready to run

---

## 🚀 To Run the Application

```bash
cd "d:\HCMUS\HQT CSDL\sports-services-management\webapp-mvc"
dotnet run
```

Then navigate to: `https://localhost:5001/HomeStaff/Index`

Login as: **Receptionist Role** (Vai trò: Lễ tân)

---

## 📝 Test Checklist

### Test 1: Create New Customer ✓
- [ ] Click "Thêm khách hàng mới" button
- [ ] Fill in required fields (Họ tên, SDT)
- [ ] Click "Tạo"
- [ ] **Expected:** No SQL error, modal closes, customer in search results
- [ ] **Check:** Database has new customer (MaKH, HoTen, SDT)

### Test 2: Search Customer ✓
- [ ] Type customer name (e.g., "Lê")
- [ ] **Expected:** Only customers with "Lê" in name show
- [ ] Type phone number (e.g., "090")
- [ ] **Expected:** Only customers with "090" in phone show
- [ ] Type ID number (e.g., "123")
- [ ] **Expected:** Only customers with "123" in CCCD show

### Test 3: Dropdowns Load ✓
- [ ] Page loads and dropdowns populate automatically
- [ ] Facilities dropdown has items (Cơ sở)
- [ ] Court Types dropdown has items (Loại sân)
- [ ] Services dropdown has items (Dịch vụ)
- [ ] **Check:** Open DevTools Console → No error messages

### Test 4: Dependent Dropdowns ✓
- [ ] Select facility → Court types should update
- [ ] Select facility + type → Courts should load
- [ ] Select court + date/time → Can submit booking

### Test 5: Booking Creation ✓
- [ ] Select customer (or create new)
- [ ] Select facility, court type, court
- [ ] Select date and time
- [ ] Click "Đặt sân"
- [ ] **Expected:** Booking created successfully, appears in list below

---

## 🔍 DevTools Debugging

### Network Tab
1. **Customer Search:**
   - Look for: `POST .../DatSanTrucTiep/TimKhachHang?tuKhoa=<keyword>`
   - Check Response: Should have filtered customer list

2. **Dropdowns:**
   - Look for: `GET .../DatSanTrucTiep/LoadCoSo` (etc.)
   - Check Response: Should have JSON with MaCS, TenCoSo

3. **Booking:**
   - Look for: `POST .../DatSanTrucTiep/TaoPhieuDat`
   - Check Response: Should have `{"success":true}`

### Console Tab
1. **Expected Messages:**
   ```
   LoadCoSo result: {success: true, data: Array(3)}
   LoadLoaiSan result: {success: true, data: Array(4)}
   LoadDichVu result: {success: true, data: Array(5)}
   Search result: {success: true, data: Array(1)}
   ```

2. **Error Messages (None Expected):**
   - If you see errors, DevTools will show them here
   - Check "LoadCoSo error:", "LoadLoaiSan error:", "Search error:"

---

## 🐛 Troubleshooting

| Problem | Solution |
|---------|----------|
| Search shows full list | Check Network tab: Request URL should have `?tuKhoa=...` |
| Dropdowns empty | Open Console: Look for "LoadCoSo error:" messages |
| Customer creation fails | Check Network Response: Look for SQL error details |
| Booking won't submit | Check Console: Any validation errors? |
| Page loads slowly | Normal if first load; subsequent loads are fast |

---

## 📊 What Was Fixed

| Issue | Status | Fix |
|-------|--------|-----|
| Customer creation SQL error | ✅ FIXED | Updated INSERT to use valid KHACHHANG columns |
| Search not filtering | ✅ FIXED | Changed JSON body to query string parameter |
| Dropdowns not loading | ✅ FIXED | Added error handling and logging |

---

## 📂 Key Files

- **Controller:** `webapp-mvc/Controllers/DatSanTrucTiepController.cs`
- **View:** `webapp-mvc/Views/DatSanTrucTiep/Index.cshtml`
- **Database:** `TRUNGTAMTHETHAO` (SQL Server)
- **Build Config:** `webapp-mvc/webapp-mvc.csproj`

---

## ⏱️ Estimated Time

- Build: ~5 seconds
- Run: ~10 seconds
- Full test cycle: ~10 minutes

---

**Ready to test! 🚀**
