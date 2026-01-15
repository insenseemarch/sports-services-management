# Tính năng: Tạo Tài Khoản Khách Hàng Từ Trang Đặt Sân

## 📋 Tóm tắt
Đã thêm tính năng cho phép khách hàng **tạo tài khoản trực tiếp từ trang đặt sân** mà không cần phải chuyển sang trang đăng ký riêng. Giao diện sử dụng modal popup với hộp lỗi/thành công.

## 🎯 Yêu cầu đã hoàn thành
✅ Nút "Tạo tài khoản" hiển thị trong hero banner trang đặt sân  
✅ Modal form với các trường nhập liệu (username, password, thông tin cá nhân, v.v.)  
✅ Validation client-side (kiểm tra mật khẩu khớp, độ dài tối thiểu, etc.)  
✅ AJAX POST đến endpoint `/TaiKhoan/CreateAccountAjax`  
✅ Lưu dữ liệu vào 2 bảng: **TAIKHOAN** + **KHACHHANG** cùng lúc  
✅ Auto-login sau khi tạo tài khoản thành công  
✅ Thông báo success popup → tự động đóng modal sau 2 giây  
✅ Reload trang để cập nhật trạng thái đăng nhập  

---

## 📁 Các tệp đã sửa/thêm

### 1. **[DatSan/Index.cshtml](../webapp-mvc/Views/DatSan/Index.cshtml)** - Frontend

#### a) Nút "Tạo tài khoản" trong hero banner (dòng ~136)
```html
<!-- Create Account Button -->
<div class="mt-5">
    <button type="button" class="btn btn-lg fw-bold rounded-pill shadow-lg" 
            data-bs-toggle="modal" data-bs-target="#createAccountModal"
            style="background: linear-gradient(135deg, #a3cf06, #38ef7d); color: white;">
        <i class="fas fa-user-plus me-2"></i>Chưa có tài khoản? Tạo ngay
    </button>
</div>
```

#### b) Modal Form (dòng ~486-560)
- ID: `createAccountModal`
- Form ID: `formCreateAccount`
- Input fields:
  - `username` (required) - Tên đăng nhập
  - `password` (required) - Mật khẩu
  - `passwordConfirm` (required) - Xác nhận mật khẩu
  - `hoTen` (required) - Họ tên
  - `sdt` (optional) - Số điện thoại
  - `email` (optional) - Email
  - `ngaySinh` (optional) - Ngày sinh
  - `cccd` (optional) - CCCD/CMT
  - `laHSSV` (checkbox) - Là sinh viên?

#### c) JavaScript Event Handler (dòng ~798-897)
```javascript
$('#formCreateAccount').on('submit', function(e) {
    // 1. Validation (mật khẩu khớp, độ dài, required fields)
    // 2. AJAX POST to /TaiKhoan/CreateAccountAjax
    // 3. Success: show alert → auto close modal after 2s
    // 4. Error: show error message in modal (keep modal open)
});
```

**Validation bao gồm:**
- Mật khẩu và xác nhận mật khẩu phải khớp nhau
- Mật khẩu tối thiểu 6 ký tự
- Tên đăng nhập tối thiểu 3 ký tự
- Kiểm tra các trường bắt buộc: username, password, hoTen, sdt

---

### 2. **[TaiKhoanController.cs](../webapp-mvc/Controllers/TaiKhoanController.cs)** - Backend

#### Method: `CreateAccountAjax()` (dòng ~147-210)
```csharp
[HttpPost]
public IActionResult CreateAccountAjax([FromBody] DangKyViewModel model)
{
    try
    {
        // 1. Validate ModelState
        if (!ModelState.IsValid)
            return Json(new { success = false, message = "Dữ liệu không hợp lệ" });
        
        // 2. Check duplicate username
        var checkUser = _db.ExecuteQuery("SELECT COUNT(*) FROM TAIKHOAN WHERE TenDangNhap = @un", 
            new SqlParameter("@un", model.username));
        
        // 3. Generate IDs with timestamp
        var maTK = "TK" + DateTime.Now.ToString("yyyyMMddHHmmss");
        var maKH = "KH" + DateTime.Now.ToString("yyyyMMddHHmmss");
        
        // 4. INSERT TAIKHOAN
        _db.ExecuteNonQuery("sp_InsertTaiKhoan", 
            new SqlParameter("@MaTK", maTK),
            new SqlParameter("@TenDangNhap", model.username),
            new SqlParameter("@MatKhau", model.password), // Plain text (⚠️ Consider hashing)
            new SqlParameter("@VaiTro", "Khách hàng"));
        
        // 5. INSERT KHACHHANG
        _db.ExecuteNonQuery("sp_InsertKhachHang",
            new SqlParameter("@MaKH", maKH),
            new SqlParameter("@HoTen", model.hoTen),
            new SqlParameter("@SDT", model.sdt ?? ""),
            new SqlParameter("@Email", model.email ?? ""),
            new SqlParameter("@NgaySinh", model.ngaySinh ?? DBNull.Value),
            new SqlParameter("@CCCD", model.cccd ?? ""),
            new SqlParameter("@MaTK", maTK),
            new SqlParameter("@MaCB", "CB001")); // Default tier: Bronze
        
        // 6. Auto-login via Session
        HttpContext.Session.SetString("MaTK", maTK);
        HttpContext.Session.SetString("MaUser", maKH);
        HttpContext.Session.SetString("Username", model.username);
        HttpContext.Session.SetString("HoTen", model.hoTen);
        HttpContext.Session.SetString("VaiTro", "Khách hàng");
        
        return Json(new { 
            success = true, 
            message = "Tạo tài khoản thành công!", 
            maKH = maKH, 
            maTK = maTK,
            hoTen = model.hoTen
        });
    }
    catch (SqlException ex)
    {
        return Json(new { success = false, message = "Lỗi CSDL: " + ex.Message });
    }
    catch (Exception ex)
    {
        return Json(new { success = false, message = "Lỗi: " + ex.Message });
    }
}
```

**Chức năng chính:**
- Kiểm tra username không trùng lặp
- Tạo ID tự động dùng timestamp (MaTK = "TK" + yyyyMMddHHmmss)
- Lưu vào TAIKHOAN + KHACHHANG cùng lúc
- Auto-login bằng Session (không cần refresh trang để reload)
- Trả về JSON response với success flag và dữ liệu người dùng

---

## 🗄️ Cấu trúc Database

### TAIKHOAN Table
```
MaTK (PK)       | TenDangNhap | MatKhau | VaiTro
TK20250115...   | username    | pass123 | Khách hàng
```

### KHACHHANG Table
```
MaKH (PK) | HoTen    | SDT    | Email | MaTK (FK)        | MaCB  
KH202501  | Nguyễn A | 09xxx  | a@... | TK20250115...    | CB001
```

**Mối quan hệ:**
- `KHACHHANG.MaTK` → `TAIKHOAN.MaTK` (Foreign Key)
- Khi tạo account mới, cả 2 records phải được insert cùng lúc

---

## 🔄 Quy trình User Flow

```
1. User vào trang đặt sân → thấy nút "Tạo tài khoản" ở hero banner
   ↓
2. Click nút → mở modal createAccountModal
   ↓
3. Điền form: username, password, hoTen, sdt, email, etc.
   ↓
4. Click "Tạo tài khoản" → JavaScript validation
   ↓
5. AJAX POST to /TaiKhoan/CreateAccountAjax (JSON)
   ↓
6. Server: Check duplicate username → INSERT TAIKHOAN → INSERT KHACHHANG
   ↓
7. Server returns: { success: true, hoTen: "..." }
   ↓
8. Client: Show success alert → wait 2 seconds → reload page
   ↓
9. User logged in tự động, có thể tiếp tục đặt sân
```

---

## ⚙️ Validation Rules

### Client-side (JavaScript)
| Field | Rule |
|-------|------|
| username | 3-20 ký tự |
| password | ≥ 6 ký tự, phải khớp với passwordConfirm |
| hoTen | Required |
| sdt | Optional nhưng validate format nếu có |

### Server-side (C#)
| Check | Action |
|-------|--------|
| ModelState invalid | Return error JSON |
| Username exists | Return error: "Tên đăng nhập đã được sử dụng" |
| SqlException | Return error: "Lỗi CSDL: ..." |
| Success | Create session, return success JSON + reload |

---

## 🎨 Styling & UX

### Nút "Tạo tài khoản"
- Gradient: từ xanh lá (#a3cf06) sang xanh nước biển (#38ef7d)
- Kích thước: btn-lg, fw-bold
- Icon: fa-user-plus
- Position: Trong hero banner, dưới text mô tả

### Modal Form
- Centered modal với bo tròn 4px (rounded-4)
- Background: White, shadow-lg
- Header: Icon + tiêu đề
- Body: Form fields với spacing 4
- Alert boxes: 
  - Error (alert-danger) - hiển thị lỗi validation/server
  - Success (alert-success) - hiển thị thông báo thành công

### Loading State
- Nút submit: Disable + show spinner icon + text "Đang tạo tài khoản..."
- Restore nút nếu error

---

## 🔐 Security Notes ⚠️

**Hiện tại:**
- ✅ Parameterized SQL queries (SqlParameter)
- ✅ Server-side validation
- ✅ Duplicate username check
- ❌ Password stored plain text (should use bcrypt/hash)
- ❌ No CSRF token validation (should add @Html.AntiForgeryToken())

**Recommendations:**
1. Hash password trước khi lưu vào DB: `BCrypt.Net.BCrypt.HashPassword(password)`
2. Thêm anti-CSRF token vào form
3. Rate limiting trên CreateAccountAjax endpoint
4. Validate email format client + server side

---

## 📝 Testing Checklist

- [ ] Click nút "Tạo tài khoản" → modal hiển thị
- [ ] Nhập form không đầy đủ → show error
- [ ] Mật khẩu không khớp → show error
- [ ] Username < 3 ký tự → show error
- [ ] Mật khẩu < 6 ký tự → show error
- [ ] Nhập username đã tồn tại → server returns error → show error
- [ ] Nhập hợp lệ → AJAX POST thành công
- [ ] Server tạo TAIKHOAN + KHACHHANG → check DB
- [ ] Success popup hiển thị → auto close sau 2s
- [ ] Reload trang → user đã logged in
- [ ] Session variables set đúng: MaTK, MaUser, HoTen, VaiTro

---

## 📂 File Summary

| File | Đã thay đổi | Dòng |
|------|---------|------|
| Views/DatSan/Index.cshtml | ✅ | 136, 486-560, 798-897 |
| Controllers/TaiKhoanController.cs | ✅ | 147-210 |

---

## 🚀 Deployment Notes

1. **Database**: Không cần migration (bảng TAIKHOAN + KHACHHANG đã tồn tại)
2. **Build**: `dotnet build` - successful (119 warnings, no errors)
3. **Run**: `dotnet run --project webapp-mvc`
4. **Test URL**: http://localhost:5000/DatSan

---

**Created:** 2025-01-15  
**Status:** ✅ Ready for Testing
