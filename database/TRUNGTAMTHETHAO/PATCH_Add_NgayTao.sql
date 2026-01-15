USE TRUNGTAMTHETHAO
GO

-- 1. Thêm cột NgayTao vào bảng PHIEUDATSAN (nếu chưa có)
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'PHIEUDATSAN' AND COLUMN_NAME = 'NgayTao')
BEGIN
    ALTER TABLE PHIEUDATSAN ADD NgayTao DATETIME DEFAULT GETDATE();
    PRINT N'✅ Đã thêm cột NgayTao vào bảng PHIEUDATSAN';
END
ELSE
BEGIN
    PRINT N'ℹ️ Cột NgayTao đã tồn tại, không cần thêm.';
END
GO

-- 2. Cập nhật dữ liệu cũ: Gán NgayTao = NgayDat (tránh bị NULL gây lỗi web)
UPDATE PHIEUDATSAN 
SET NgayTao = CAST(NgayDat AS DATETIME) 
WHERE NgayTao IS NULL;

PRINT N'✅ Đã điền dữ liệu cho các phiếu cũ bị trống NgayTao.';
GO
