USE TRUNGTAMTHETHAO
GO

-- ============================================================================
-- PATCH: Sửa lỗi thiếu cột trong Database
-- ============================================================================

PRINT N'🔧 Bắt đầu vá lỗi Database...';
GO

-- 1. Thêm cột NgayTao vào bảng PHIEUDATSAN (nếu chưa có)
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'PHIEUDATSAN' AND COLUMN_NAME = 'NgayTao')
BEGIN
    ALTER TABLE PHIEUDATSAN ADD NgayTao DATETIME DEFAULT GETDATE();
    PRINT N'✅ Đã thêm cột NgayTao vào bảng PHIEUDATSAN';
END
ELSE
BEGIN
    PRINT N'ℹ️  Cột NgayTao đã tồn tại.';
END
GO

-- 2. Cập nhật dữ liệu cũ: Gán NgayTao = NgayDat (tránh bị NULL)
UPDATE PHIEUDATSAN 
SET NgayTao = CAST(NgayDat AS DATETIME) 
WHERE NgayTao IS NULL;
PRINT N'✅ Đã điền dữ liệu cho các phiếu cũ bị trống NgayTao.';
GO

-- 3. Thêm cột GioMoCua vào bảng COSO (nếu chưa có)
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'COSO' AND COLUMN_NAME = 'GioMoCua')
BEGIN
    ALTER TABLE COSO ADD GioMoCua TIME;
    PRINT N'✅ Đã thêm cột GioMoCua vào bảng COSO';
END
ELSE
BEGIN
    PRINT N'ℹ️  Cột GioMoCua đã tồn tại.';
END
GO

-- 4. Thêm cột GioDongCua vào bảng COSO (nếu chưa có)
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'COSO' AND COLUMN_NAME = 'GioDongCua')
BEGIN
    ALTER TABLE COSO ADD GioDongCua TIME;
    PRINT N'✅ Đã thêm cột GioDongCua vào bảng COSO';
END
ELSE
BEGIN
    PRINT N'ℹ️  Cột GioDongCua đã tồn tại.';
END
GO

-- 5. Cập nhật giờ mở/đóng cửa mặc định cho các cơ sở cũ (nếu NULL)
UPDATE COSO 
SET GioMoCua = '06:00:00', GioDongCua = '22:00:00'
WHERE GioMoCua IS NULL OR GioDongCua IS NULL;
PRINT N'✅ Đã cập nhật giờ mở/đóng cửa mặc định cho các cơ sở.';
GO

PRINT N'';
PRINT N'🎉 Hoàn tất vá lỗi! Database đã sẵn sàng.';
GO
