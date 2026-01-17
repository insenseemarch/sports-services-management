# 🔧 FIX: Service Booking Inventory Bug

## Problem
When booking trainer (HLV) services, users got error:
```
Lỗi đặt HLV Trần Thị Bình: Lỗi: Số lượng tồn kho không được nhỏ hơn 0!
```
(Error booking trainer: Error: Inventory quantity cannot be less than 0!)

## Root Cause
The system treated **all services identically**, but trainers and services have different inventory models:

| Service Type | Inventory Model | Should Track Stock |
|---|---|---|
| **Equipment** (balls, rackets, nets, bibs) | Physical items with `DV_COSO.SoLuongTon` | ✅ Yes |
| **Trainers (HLV)** | Staff availability in `NHANVIEN` | ❌ No |
| **VIP Rooms** | Facility resource | ❌ No |
| **Lockers** | Facility resource | ❌ No |

**The bug:** `sp_ThemDichVu` and trigger `trg_CapNhatTonKhoDichVu` attempted to:
1. Check `DV_COSO.SoLuongTon < requested quantity` for trainers (which may not exist or be 0)
2. Decrement inventory for non-physical services

## Solution
Modified **two files** to distinguish between **stock items** vs **non-stock services**:

### 1. `sp_ThemDichVu` Stored Procedure
**Change:** Added check for service type using `LOAIDV` (service category)

```sql
-- Check if service is HLV (LDV001), VIP (LDV004), or Locker (LDV005)
DECLARE @IsStockItem BIT = 1;
IF EXISTS (
    SELECT 1 FROM DICHVU DV 
    JOIN LOAIDV L ON DV.MaLoaiDV = L.MaLoaiDV
    WHERE DV.MaDV = @MaDV 
    AND (
        L.MaLoaiDV IN ('LDV001', 'LDV004', 'LDV005') 
        OR L.TenLoai LIKE N'%Huấn luyện viên%' 
        OR L.TenLoai LIKE N'%VIP%' 
        OR L.TenLoai LIKE N'%Tủ đồ%'
    )
)
BEGIN
    SET @IsStockItem = 0;  -- Don't track inventory
END
```

**Then only check & deduct inventory if `@IsStockItem = 1`**

### 2. `trg_CapNhatTonKhoDichVu` Trigger
**Change:** Applied same service type check before updating `DV_COSO`

```sql
IF @IsStockItem = 1
BEGIN
    -- Only check inventory and deduct for physical items
    UPDATE DV_COSO SET SoLuongTon = SoLuongTon - @SoLuongDat ...
END
-- Non-stock services (HLV, VIP, Locker) bypass inventory tracking
```

## Files Modified
1. `/database/TRUNGTAMTHETHAO/Function+SP+Trigger.sql`
   - Updated `sp_ThemDichVu` (lines ~1047)
   - Updated `trg_CapNhatTonKhoDichVu` (lines ~1313)

## Testing Steps
1. **Trainer Booking:** Customer books trainer → Should NOT error
2. **Equipment Booking:** Customer books ball (stock=5) → Should deduct from `DV_COSO.SoLuongTon`
3. **Equipment Shortage:** Customer books ball (stock=2) when requesting 5 → Should error ✅
4. **VIP Room:** Customer books VIP shower → Should NOT error

## Design Notes
✅ **Your design is CORRECT** - You properly separated:
- Physical inventory (equipment) → Requires stock tracking
- Service/Staff resources (trainers, VIP rooms) → No inventory tracking needed

The implementation just needed this distinction in the database logic. The fix ensures trainers can be booked without inventory constraints while still protecting equipment stock.
