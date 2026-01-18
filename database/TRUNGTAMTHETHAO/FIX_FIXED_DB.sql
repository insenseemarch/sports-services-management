-- ============================================================================
-- FIX: Thêm các cột thiếu vào database TRUNGTAMTHETHAO_FIXED
-- ============================================================================

USE TRUNGTAMTHETHAO_FIXED;
GO

PRINT N'Bắt đầu fix database TRUNGTAMTHETHAO_FIXED...';
GO

-- 1. Thêm cột TenKhungGio vào KHUNGGIO
IF NOT EXISTS (
    SELECT 1 
    FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE TABLE_NAME = 'KHUNGGIO' 
      AND COLUMN_NAME = 'TenKhungGio'
)
BEGIN
    ALTER TABLE KHUNGGIO ADD TenKhungGio NVARCHAR(100);
    PRINT N'✅ Đã thêm cột TenKhungGio';
END
ELSE
BEGIN
    PRINT N'ℹ️  Cột TenKhungGio đã tồn tại';
END
GO

-- 2. Thêm cột LoaiNgay vào KHUNGGIO
IF NOT EXISTS (
    SELECT 1 
    FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE TABLE_NAME = 'KHUNGGIO' 
      AND COLUMN_NAME = 'LoaiNgay'
)
BEGIN
    ALTER TABLE KHUNGGIO ADD LoaiNgay NVARCHAR(50);
    PRINT N'✅ Đã thêm cột LoaiNgay';
END
ELSE
BEGIN
    PRINT N'ℹ️  Cột LoaiNgay đã tồn tại';
END
GO

-- 3. Thêm cột GiaTriToiThieu vào KHUNGGIO
IF NOT EXISTS (
    SELECT 1 
    FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE TABLE_NAME = 'KHUNGGIO' 
      AND COLUMN_NAME = 'GiaTriToiThieu'
)
BEGIN
    ALTER TABLE KHUNGGIO ADD GiaTriToiThieu DECIMAL(18,2) DEFAULT 0;
    PRINT N'✅ Đã thêm cột GiaTriToiThieu';
END
ELSE
BEGIN
    PRINT N'ℹ️  Cột GiaTriToiThieu đã tồn tại';
END
GO

-- 4. Thêm cột SoGioToiThieu vào KHUNGGIO
IF NOT EXISTS (
    SELECT 1 
    FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE TABLE_NAME = 'KHUNGGIO' 
      AND COLUMN_NAME = 'SoGioToiThieu'
)
BEGIN
    ALTER TABLE KHUNGGIO ADD SoGioToiThieu INT DEFAULT 1;
    PRINT N'✅ Đã thêm cột SoGioToiThieu';
END
ELSE
BEGIN
    PRINT N'ℹ️  Cột SoGioToiThieu đã tồn tại';
END
GO

-- 5. Thêm cột TrangThai vào KHUNGGIO
IF NOT EXISTS (
    SELECT 1 
    FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE TABLE_NAME = 'KHUNGGIO' 
      AND COLUMN_NAME = 'TrangThai'
)
BEGIN
    ALTER TABLE KHUNGGIO ADD TrangThai BIT DEFAULT 1;
    PRINT N'✅ Đã thêm cột TrangThai';
END
ELSE
BEGIN
    PRINT N'ℹ️  Cột TrangThai đã tồn tại';
END
GO

-- 6. Thêm cột NgayTao vào KHUNGGIO
IF NOT EXISTS (
    SELECT 1 
    FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE TABLE_NAME = 'KHUNGGIO' 
      AND COLUMN_NAME = 'NgayTao'
)
BEGIN
    ALTER TABLE KHUNGGIO ADD NgayTao DATETIME DEFAULT GETDATE();
    PRINT N'✅ Đã thêm cột NgayTao';
END
ELSE
BEGIN
    PRINT N'ℹ️  Cột NgayTao đã tồn tại';
END
GO

PRINT N'';
PRINT N'🎉 Hoàn tất fix database TRUNGTAMTHETHAO_FIXED!';
GO
