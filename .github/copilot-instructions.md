# ViệtSport Sports Facility Booking System - Development Guide

## Architecture Overview

This is a **multi-tier system** for managing sports facility bookings and services across multiple locations.

### Key Components:
- **Database**: SQL Server (`TRUNGTAMTHETHAO`) with 23+ core tables, partitioned invoices by year
- **Web Application**: ASP.NET Core MVC (net10.0) - primary interface
- **Desktop Application**: WinForm (.NET) - staff and admin operations
- **Transaction-Heavy**: Handles concurrent bookings, payments, inventory (equipment) with strict isolation requirements

### Data Flow:
```
Customer Portal (Web) / Staff Portal (WinForm)
     ↓
Controllers (DatSanController, TaiKhoanController, etc.)
     ↓
DatabaseHelper (ADO.NET with SqlCommand)
     ↓
SQL Server (Stored Procedures, Triggers, Partitioned Tables)
```

---

## Critical Database Design Patterns

### 1. **Concurrency Control with Transactions**
All booking operations must use **SQL Server Transaction Isolation Levels** with explicit locking:

- **Repeatable Read + Update Locks** for double-booking prevention
  - `sp_DatSan` checks room availability with `UPDLOCK` and `XLOCK` hints
  - Prevents Race Condition: T1 and T2 reading same available slot simultaneously

- **Partition by Year** on `HOADON` table (range right on DATE column)
  - Invoices split into yearly partitions (2020-2026)
  - Query predicates MUST filter by date for partition elimination

### 2. **Key Tables & Relationships**:
| Table | Purpose | Critical Constraints |
|-------|---------|----------------------|
| `PHIEUDATSAN` | Booking records | PK: MaDatSan, FK: MaKH, MaSan; Status: 'Còn trống', 'Đã đặt', 'Đang sử dụng', 'Bảo trì' |
| `HOADON` | Invoices (partitioned) | Composite PK: (MaHD, NgayLap); Partition on NgayLap |
| `DATSAN` | Junction for booking→room | PK: (MaDatSan, MaSan); Prevents 1 booking, multiple rooms |
| `CT_DICHVUDAT` | Service line items | SoLuong checked against `DV_COSO.SoLuongTon` |
| `THAMGIACATRUC` | Staff shift assignments | Prevents double-booking staff |
| `DONNGHIPHEP` | Leave requests | Requires NguoiThayThe before approval |

### 3. **Inventory & Service Management**:
- `DV_COSO` tracks inventory per location (SoLuongTon)
- `CT_DICHVUDAT` subtracts qty when service booked
- **TRIGGER** auto-rejects if SoLuongTon < requested quantity
- Equipment like balls, rackets, bibs tracked by count; trainers by availability calendar

---

## Development Conventions

### Session & User Context
```csharp
// All controllers use HttpContext.Session for multi-user support
var maUser = HttpContext.Session.GetString("MaUser");
var vaiTro = HttpContext.Session.GetString("VaiTro"); // 'Khách hàng', 'Lễ tân', etc.
```

### Database Access Pattern
```csharp
// Use DatabaseHelper for ALL database operations
var _db = new DatabaseHelper(configuration);

// Always use parameterized queries (SqlParameter)
_db.ExecuteNonQuery("sp_DatSan", new SqlParameter("@MaKH", userId), ...);

// Detect SP vs SQL by presence of spaces
if (!query.Contains(" ")) cmd.CommandType = CommandType.StoredProcedure;
```

### Error Handling from SQL Stored Procedures
```csharp
try {
    _db.ExecuteNonQuery("sp_DatSan", parameters);
}
catch (SqlException ex) {
    // RAISERROR from triggers/SP becomes SqlException.Message
    // Example: "Lỗi: Sân này đã bị đặt trùng giờ trong khung giờ này"
    ModelState.AddModelError("", "Lỗi đặt sân: " + ex.Message);
}
```

---

## Critical Business Rules in Code

### Booking Constraints (in DatSanController):
- **2-hour advance booking**: `GioBatDau` must be ≥ 2 hours from now (online only)
- **Max 2 bookings/day**: Query `PHIEUDATSAN WHERE MaKH = @MaKH AND NgayDat = @Date AND TrangThai != 'Hủy'`
- **No overlapping times**: Trigger checks for time conflict on same room
- **Auto-cancel**: 30-min counter if online payment pending; 15-min late check-in cancels booking

### Pricing & Discounts:
- Base price from `SAN` table; varies by `LOAISAN` (mini soccer, badminton, tennis, basketball)
- **Time-based pricing**: 5% discount day hrs (6-16), normal evenings, +20% weekends/holidays
- **Loyalty tiers**: Bạc (-5%), Vàng (-10%), Bạch Kim (-20%) from `CAPBAC` table
- **Student discount**: 10% if `KHACHHANG.LaHSSV = 1` (must have student ID photo)

### Cancellation Policy:
- **>24 hours**: 10% penalty
- **<24 hours**: 50% penalty  
- **No-show**: 100% loss
- Trigger `TRG_HUY_DATSAN` calculates refund

---

## Testing & Deployment Strategy

### Build & Run:
```bash
# WebApp (ASP.NET Core MVC)
cd webapp-mvc
dotnet build
dotnet run
# URL: https://localhost:5001

# WinForm (Desktop)
# Open winform/interface/interface.sln in Visual Studio
# Build → Run (F5)
```

### Database Setup:
```sql
-- Execute all SQL files in order:
-- 1. database/TRUNGTAMTHETHAO/TongHop.sql (schema + partitions)
-- 2. database/TRUNGTAMTHETHAO/Function+SP+Trigger.sql (stored procs + triggers)
-- 3. database/TRUNGTAMTHETHAO/Partition+IndexTable.sql (indexing)
-- 4. database/DATA/Data.sql (sample data)
```

### Connection String:
Located in `webapp-mvc/appsettings.json`:
```json
"DefaultConnection": "Server=localhost,1433;Database=TRUNGTAMTHETHAO;User Id=sa;Password=..."
```

---

## Common Pitfalls & Solutions

| Issue | Root Cause | Fix |
|-------|-----------|-----|
| Dirty reads after booking | Missing UPDLOCK on SAN table read | Use `SELECT... WITH (UPDLOCK)` in sp_DatSan |
| Double-booking same room, time | Transaction isolation too low | Enforce Repeatable Read + Update Locks |
| Invoice partition queries slow | Date filter in WHERE predicate missing | Ensure `WHERE NgayLap = @Date` for partition elimination |
| Staff assigned to overlapping shifts | No check in THAMGIACATRUC insert | Add trigger to prevent MaNV in 2 shifts same time |
| Service qty goes negative | No inventory check before CT_DICHVUDAT insert | TRIGGER validates `DV_COSO.SoLuongTon >= @SoLuong` |

---

## Key File Reference

- **Controllers**: `webapp-mvc/Controllers/` - MVC endpoints (DatSan, TaiKhoan, DichVu, BaoCaoLoi, LichSuDatSan, NhanSu, ThanhToan, HoSo)
- **Models**: `webapp-mvc/Models/` - ViewModels + DatabaseHelper
- **Database**: `database/TRUNGTAMTHETHAO/` - All SQL (schema, SP, triggers, indexes, sample data)
- **Views**: `webapp-mvc/Views/` - Razor templates for forms (DatSan, DichVu, TaiKhoan)
- **Desktop**: `winform/interface/sports-services/` - WinForm forms (FrmMainCustomer, FrmMainStaff, FrmDangNhap)
- **Config**: `webapp-mvc/appsettings.json` - Connection string

---

## Multi-User Concurrency Notes

1. **Session-per-user**: Each login creates session with `MaUser`, `VaiTro`, `MaCS` (branch)
2. **Stored Procedure Locking**: All business-critical operations use `WITH (UPDLOCK, HOLDLOCK)` in SQL
3. **Trigger Validation**: AFTER INSERT triggers on PHIEUDATSAN validate availability + max-bookings rule
4. **No pessimistic locking in C#**: ADO.NET doesn't maintain row locks; rely on SQL Server isolation level + locks

This is an **academic project** emphasizing transaction design. Prioritize ACID guarantees over read performance.
