USE TRUNGTAMTHETHAO
GO

-- ============================================================================
-- PATCH: Thêm cột TrangThai vào bảng NHANVIEN để hỗ trợ Xóa mềm (Soft Delete)
-- ============================================================================

PRINT N'🔧 Bắt đầu cập nhật bảng NHANVIEN...';
GO

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'NHANVIEN' AND COLUMN_NAME = 'TrangThai')
BEGIN
    ALTER TABLE NHANVIEN ADD TrangThai NVARCHAR(50) DEFAULT N'Đang làm';
    PRINT N'✅ Đã thêm cột TrangThai vào bảng NHANVIEN';
    
    -- Cập nhật dữ liệu cũ
    EXEC('UPDATE NHANVIEN SET TrangThai = N''Đang làm'' WHERE TrangThai IS NULL');
    PRINT N'✅ Đã cập nhật trạng thái mặc định cho nhân viên cũ.';
END
ELSE
BEGIN
    PRINT N'ℹ️  Cột TrangThai đã tồn tại trong bảng NHANVIEN.';
END
GO
