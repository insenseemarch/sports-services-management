-- Kiểm tra xem stored procedures đã tồn tại chưa

USE TRUNGTAMTHETHAO;
GO

PRINT '=== CHECKING DEFAULT DATABASE ===';
IF EXISTS (SELECT 1 FROM sys.objects WHERE name = 'sp_GetCourtPrice_NoIsolation' AND type = 'P')
    PRINT '✅ sp_GetCourtPrice_NoIsolation EXISTS in TRUNGTAMTHETHAO';
ELSE
    PRINT '❌ sp_GetCourtPrice_NoIsolation NOT FOUND in TRUNGTAMTHETHAO';
GO

USE TRUNGTAMTHETHAO_FIXED;
GO

PRINT '=== CHECKING FIXED DATABASE ===';
IF EXISTS (SELECT 1 FROM sys.objects WHERE name = 'sp_GetCourtPrice_WithRepeatableRead' AND type = 'P')
    PRINT '✅ sp_GetCourtPrice_WithRepeatableRead EXISTS in TRUNGTAMTHETHAO_FIXED';
ELSE
    PRINT '❌ sp_GetCourtPrice_WithRepeatableRead NOT FOUND in TRUNGTAMTHETHAO_FIXED';
GO

-- Kiểm tra dữ liệu trong bảng KHUNGGIO
SELECT COUNT(*) as [Total Records in KHUNGGIO] FROM TRUNGTAMTHETHAO_FIXED.dbo.KHUNGGIO;
GO

-- Kiểm tra 1 record mẫu
SELECT TOP 3 * FROM TRUNGTAMTHETHAO_FIXED.dbo.KHUNGGIO;
GO
