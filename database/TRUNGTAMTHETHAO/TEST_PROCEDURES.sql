-- Test stored procedure trực tiếp

USE TRUNGTAMTHETHAO_FIXED;
GO

PRINT '=== TESTING Fixed DB Stored Procedure ===';

DECLARE @TransactionId UNIQUEIDENTIFIER;

EXEC sp_GetCourtPrice_WithRepeatableRead 
    @MaLS = 'LS001',
    @GioBatDau = '04:00:00',
    @NgayDat = '2026-01-21',
    @TransactionId = @TransactionId OUTPUT;

PRINT 'TransactionId: ' + CAST(@TransactionId AS VARCHAR(50));
GO

PRINT '';
PRINT '=== TESTING Default DB Stored Procedure ===';

USE TRUNGTAMTHETHAO;
GO

EXEC sp_GetCourtPrice_NoIsolation 
    @MaLS = 'LS001',
    @GioBatDau = '04:00:00',
    @NgayDat = '2026-01-21';
GO
