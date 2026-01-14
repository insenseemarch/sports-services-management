# Fixes Summary - DatSanTrucTiep (Đặt Sân Trực Tiếp) Features

## Date: 2025
## Status: ✅ BUILD SUCCESSFUL (0 errors, 114 warnings)

---

## 🔧 Issues Fixed

### Issue #1: Customer Creation INSERT Column Mismatch ❌ → ✅

**Problem:**
- SQL Error: "Invalid column name 'TrangThai', 'NgayDangKy', 'CapBac'"
- Backend TaoKhachHang action tried to insert data into non-existent KHACHHANG table columns

**Root Cause:**
- KHACHHANG table schema doesn't have columns: TrangThai, NgayDangKy, CapBac
- These columns exist in TAIKHOAN (accounts), not KHACHHANG (customers)

**Solution Applied:**
- Read database schema from TongHop.sql
- Identified actual KHACHHANG columns: MaKH, HoTen, NgaySinh, CCCD, SDT, Email, DiaChi, LaHSSV (BIT), DiemTichLuy (INT), MaCB, MaTK
- Updated INSERT statement in DatSanTrucTiepController.cs (lines 105-115):
  ```csharp
  INSERT INTO KHACHHANG (MaKH, HoTen, SDT, Email, DiaChi, CCCD, LaHSSV, DiemTichLuy)
  VALUES (@MaKH, @HoTen, @SDT, @Email, @DiaChi, @CCCD, 0, 0)
  ```
- Removed references to TrangThai, NgayDangKy, CapBac

**File Changed:** `webapp-mvc/Controllers/DatSanTrucTiepController.cs`

---

### Issue #2: Customer Search Not Filtering ❌ → ⏳ (Fixed, needs verification)

**Problem:**
- Search by name (Tên), phone (SDT), or ID (CCCD) showed full customer list regardless of search keyword
- User typed keyword but results didn't filter

**Root Cause:**
- Frontend Fetch API sent keyword as JSON body: `body: JSON.stringify({tuKhoa: keyword})`
- Backend TimKhachHang method expected form parameter (not JSON): `public IActionResult TimKhachHang(string tuKhoa)`
- Query-string-binding in ASP.NET Core doesn't bind JSON body to simple string parameters

**Solution Applied:**
- Modified search event listener in Index.cshtml (lines 654-700)
- Changed from JSON body to query string parameter:
  ```javascript
  const url = new URL('@Url.Action("TimKhachHang", "DatSanTrucTiep")', window.location.origin);
  url.searchParams.append('tuKhoa', keyword);
  
  fetch(url, {
      method: 'POST',
      headers: {'Content-Type': 'application/x-www-form-urlencoded'}
  })
  ```
- Added console.log for debugging
- Added error handling with .catch()
- Added null checks for dropdown elements

**File Changed:** `webapp-mvc/Views/DatSanTrucTiep/Index.cshtml`

---

### Issue #3: Dropdowns Not Loading ❌ → ⏳ (Fixed, needs verification)

**Problem:**
- loadCoSo() (Facilities), loadLoaiSan() (Court Types), loadDichVu() (Services) functions not populating dropdowns
- User refreshes page but dropdowns remain empty

**Root Cause:**
- Fetch calls not handling potential errors (no .catch() handlers)
- No null checks before accessing DOM elements
- No logging to debug if functions are executing
- setTimeout 200ms might be too short or functions not executing at all

**Solutions Applied:**

1. **Added Console Logging** (all dropdown functions):
   - `console.log('LoadCoSo result:', d)`
   - `console.log('LoadLoaiSan result:', d)`
   - `console.log('LoadDichVu result:', d)`
   - `console.log('LoadSan result:', d)`
   - Allows DevTools debugging to see if functions execute and what data they receive

2. **Added Error Handlers**:
   - `.catch(err => console.error('LoadCoSo error:', err))`
   - Catches network/server errors
   - Logs error to console

3. **Added Null Checks**:
   - `if (d.success && d.data)` (instead of just `if (d.success)`)
   - `const el = document.getElementById('maSan'); if (el) el.innerHTML = html;`
   - Prevents errors when DOM elements don't exist

4. **Added JSON Key Casing Fallbacks**:
   - Support both PascalCase (MaCS, TenCoSo) and camelCase (maCS, tenCoSo)
   - Example: `${i.MaCS || i.maCS}` and `${i.TenCoSo || i.tenCoSo}`
   - Handles API responses with inconsistent casing

5. **Improved DOMContentLoaded**:
   - Added null checks before accessing elements
   - Wrapped loadCoSo/loadDichVu calls with better error handling

**Files Changed:** 
- `webapp-mvc/Views/DatSanTrucTiep/Index.cshtml` (lines 560-620 + 633-639)

---

## 📋 Summary of Backend Fixes

### DatSanTrucTiepController.cs Changes:

**TaoKhachHang Method (Lines 69-127):**
- Added KhachHangDto class for proper [FromBody] JSON binding
- Fixed INSERT statement to use valid KHACHHANG columns only
- Set LaHSSV = 0 (not HSSV/student by default)
- Set DiemTichLuy = 0 (start with 0 loyalty points)
- Removed non-existent columns: TrangThai, NgayDangKy, CapBac

**TimKhachHang Method (Lines 33-68):**
- Already had correct WHERE clause with LIKE operators
- Query: `WHERE KH.HoTen LIKE @TuKhoa OR KH.SDT LIKE @TuKhoa OR KH.CCCD LIKE @TuKhoa`
- Returns TOP 10 results ordered by name
- Issue was frontend sending JSON instead of query string (now fixed in frontend)

---

## 📝 Summary of Frontend Fixes

### Index.cshtml Changes:

**Customer Search (Lines 654-700):**
- Changed from JSON body to query string parameter binding
- Added error handling with .catch()
- Added console logging for debugging
- Added null checks for DOM elements

**Dropdown Loading Functions (Lines 560-620):**
- loadCoSo() - Added console.log, error handler, null checks, fallback keys
- loadLoaiSanByCs() - Same improvements
- loadCourts() (loadSan) - Same improvements  
- loadDichVu() - Same improvements

**DOMContentLoaded (Lines 633-639):**
- Added null checks before accessing elements
- Maintained setTimeout 200ms for initialization delay

---

## ✅ Build Status

```
Build succeeded.
0 errors, 114 warnings (pre-existing)
Output: D:\HCMUS\HQT CSDL\sports-services-management\webapp-mvc\bin\Debug\net9.0\webapp-mvc.dll
```

All warnings are pre-existing nullable reference type warnings (not related to our fixes).

---

## 🧪 Testing Instructions

### Test #1: Create New Customer
1. Navigate to receptionist page: `/HomeStaff/Index`
2. Click "Đặt Sân Trực Tiếp" tab
3. Click "Thêm khách hàng mới" button
4. Fill modal form:
   - Họ tên: "Test Customer"
   - SDT: "0987654321"
   - CCCD: "123456789" (optional)
   - Email: "test@example.com" (optional)
   - Địa chỉ: "Test Address" (optional)
5. Click "Tạo"
6. **Expected:** Modal closes, customer appears in search results, no SQL errors

### Test #2: Search Customer by Name/SDT/CCCD
1. In customer search input, type: "Lê" (common Vietnamese name)
2. Open DevTools (F12) → Network tab
3. Watch for POST request to `/DatSanTrucTiep/TimKhachHang?tuKhoa=L%C3%AA`
4. Check Response tab - should show only customers with "Lê" in name/SDT/CCCD
5. **Expected:** Search results filter correctly, only matching customers show

### Test #3: Dropdowns Load
1. Refresh page
2. Open DevTools (F12) → Console tab
3. Look for messages: "LoadCoSo result:", "LoadLoaiSan result:", "LoadDichVu result:"
4. Facilities dropdown should populate with facility names
5. Select facility → Court Type dropdown should populate
6. Select facility + court type → Court dropdown should populate
7. **Expected:** All dropdowns load with data, no errors in console

---

## 🔍 Debugging Tips

### If Search Still Shows Full List:
1. Open DevTools → Network tab
2. Type keyword in search input
3. Look for POST request to TimKhachHang
4. Check Request tab: URL should have `?tuKhoa=keyword` (not JSON body)
5. Check Response tab: Should show filtered results (not full list)

### If Dropdowns Don't Populate:
1. Open DevTools → Console tab
2. Refresh page
3. Look for error messages starting with "LoadCoSo error:", "LoadLoaiSan error:", etc.
4. If no messages = functions not executing
5. If error = network issue or backend error

### Common Issues:
- **Issue:** "Không tìm thấy" (Not found) shows but you know customer exists
  - **Solution:** Make sure customer was actually created (check database)
  
- **Issue:** Dropdown empty after selecting facility
  - **Solution:** Check if LoadSan POST request includes both maCS and maLS parameters
  
- **Issue:** Customer creation "Lỗi: ..." alert appears
  - **Solution:** Check DevTools Network tab Response for exact SQL error message

---

## 📂 Files Modified

1. `webapp-mvc/Controllers/DatSanTrucTiepController.cs`
   - TaoKhachHang method (lines 69-127)
   - TimKhachHang method (already correct)
   - KhachHangDto class (lines 70-77)

2. `webapp-mvc/Views/DatSanTrucTiep/Index.cshtml`
   - loadCoSo() function (lines 563-574)
   - loadLoaiSanByCs() function (lines 576-588)
   - loadCourts() function (lines 590-605)
   - loadDichVu() function (lines 607-619)
   - CUSTOMER SEARCH event listener (lines 654-700)
   - DOMContentLoaded (lines 633-639)

---

## 🎯 Next Steps

1. **Run the application**: `dotnet run` in webapp-mvc directory
2. **Test all three issues** using instructions above
3. **Verify database**: Query KHACHHANG table to confirm new customers are created
4. **Check browser console**: DevTools → Console for any errors while using the page
5. **Report any remaining issues** with: Page, Action Taken, Error Message, DevTools Screenshots

---

## ℹ️ Additional Notes

- **JSON Binding in ASP.NET Core:** 
  - Simple type parameters (string, int) use query string/form data by default
  - Use `[FromBody]` to bind complex types (classes) from JSON
  - Use `[FromQuery]` or no attribute for query string parameters

- **Search Performance:**
  - Query uses TOP 10 to limit results
  - LIKE operator is case-insensitive in SQL Server by default
  - Consider adding SQL indexes on HoTen, SDT, CCCD for large datasets

- **Dropdown Data:**
  - All dropdowns use GET endpoints (simpler, cacheable)
  - No filtering needed for initial load (shows all items)
  - Consider adding empty select option by default (already done: "Chọn...")

---

**End of Summary**
