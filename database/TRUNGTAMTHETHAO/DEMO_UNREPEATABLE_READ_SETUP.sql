-- ==================================================================================
-- DEMO: UNREPEATABLE READ - XEM GIÁ SÂN
-- Mục đích: Tạo database thứ 2 và stored procedures để demo lỗi tranh chấp
-- ==================================================================================

-- ==================================================================================
-- 2. TẠO STORED PROCEDURES CHO DATABASE CHƯA FIX (READ COMMITTED - mặc định)
-- ==================================================================================
-- 1. Default DB (Read Committed) -> Sẽ thấy GIÁ MỚI sau khi chờ (LỖI)
USE TRUNGTAMTHETHAO;
GO

ALTER PROCEDURE sp_GetCourtPrice_NoIsolation
    @MaLS VARCHAR(20),
    @GioBatDau TIME,
    @NgayDat DATE = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SET TRANSACTION ISOLATION LEVEL READ COMMITTED; -- Mặc định

    DECLARE @GiaLuucHoiDau DECIMAL(18,2);
    
    IF @NgayDat IS NULL SET @NgayDat = GETDATE();
    
    -- 1. Đọc lần 1 (Chỉ để log hoặc internal check)
    SELECT TOP 1 @GiaLuucHoiDau = GiaApDung
    FROM KHUNGGIO 
    WHERE MaLS = @MaLS 
      AND @GioBatDau >= GioBatDau 
      AND @GioBatDau < GioKetThuc
      AND NgayApDung <= @NgayDat
    ORDER BY NgayApDung DESC;

    -- 2. Delay 10s (Trong lúc này Transaction khác update & commit)
    WAITFOR DELAY '00:00:10';
    
    -- 3. Đọc lần 2 VÀ TRẢ VỀ (Đây là giá trị user nhận được)
    -- Vì Read Committed không giữ Shared Lock, nên sẽ đọc được giá trị MỚI NHẤT
    SELECT TOP 1 
        ISNULL(K.GiaApDung, 0) AS GiaApDung, -- Re-query trực tiếp từ bảng
        ISNULL(K.GioBatDau, '00:00:00') AS GioBatDau,
        ISNULL(K.GioKetThuc, '23:59:59') AS GioKetThuc,
        ISNULL(LS.DVT, N'giờ') AS DVT,
        'READ COMMITTED (Unrepeatable Read happened)' AS IsolationLevel,
        GETDATE() AS QueryTime
    FROM KHUNGGIO K
    JOIN LOAISAN LS ON K.MaLS = LS.MaLS
    WHERE K.MaLS = @MaLS 
      AND @GioBatDau >= K.GioBatDau 
      AND @GioBatDau < K.GioKetThuc
      AND K.NgayApDung <= @NgayDat
    ORDER BY K.NgayApDung DESC;
END
GO


USE TRUNGTAMTHETHAO_FIXED;
GO

-- Sửa lại SP Fixed để dùng UPDLOCK (Update Lock)
-- UPDLOCK tương thích với Shared Lock nhưng CHẶN Update khác
-- Nó mạnh hơn Shared Lock thường và chắc chắn sẽ block Manager
ALTER PROCEDURE sp_GetCourtPrice_WithRepeatableRead
    @MaLS VARCHAR(20),
    @GioBatDau TIME,
    @NgayDat DATE = NULL,
    @TransactionId UNIQUEIDENTIFIER OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SET TRANSACTION ISOLATION LEVEL REPEATABLE READ; 
    
    BEGIN TRANSACTION;
    SET @TransactionId = NEWID();

    IF @NgayDat IS NULL SET @NgayDat = GETDATE();
    
    -- Đọc lần 1 VỚI UPDLOCK
    -- UPDLOCK: "Tôi đang đọc để chuẩn bị sửa, đừng ai đụng vào!"
    -- Điều này sẽ CHẶN mọi transaction muốn Update dòng này
    DECLARE @GiaTam DECIMAL(18,2);
    
    SELECT TOP 1 @GiaTam = GiaApDung
    FROM KHUNGGIO WITH (UPDLOCK) -- <=== KEY CHANGE: Force Lock mạnh hơn
    WHERE MaLS = @MaLS 
      AND @GioBatDau >= GioBatDau 
      AND @GioBatDau < GioKetThuc
      AND NgayApDung <= @NgayDat
    ORDER BY NgayApDung DESC;
    
    -- Delay 10s -> Manager Update sẽ KHÓC THÉT (Blocked)
    WAITFOR DELAY '00:00:10';
    
    -- Đọc lần 2 và Commit
    SELECT TOP 1 
        ISNULL(K.GiaApDung, 0) AS GiaApDung,
        ISNULL(K.GioBatDau, '00:00:00') AS GioBatDau,
        ISNULL(K.GioKetThuc, '23:59:59') AS GioKetThuc,
        ISNULL(LS.DVT, N'giờ') AS DVT,
        'REPEATABLE READ (Protected)' AS IsolationLevel,
        @TransactionId AS TransactionId,
        GETDATE() AS QueryTime
    FROM KHUNGGIO K
    JOIN LOAISAN LS ON K.MaLS = LS.MaLS
    WHERE K.MaLS = @MaLS 
      AND @GioBatDau >= K.GioBatDau 
      AND @GioBatDau < K.GioKetThuc
      AND K.NgayApDung <= @NgayDat
    ORDER BY K.NgayApDung DESC;
    
    COMMIT TRANSACTION;
END
GO
-- ==================================================================================
-- 4. TẠO PROCEDURE HỖ TRỢ: Commit Transaction
-- ==================================================================================
USE TRUNGTAMTHETHAO_FIXED;
GO

CREATE OR ALTER PROCEDURE sp_CommitPriceTransaction
    @TransactionId UNIQUEIDENTIFIER
AS
BEGIN
    -- Procedure này sẽ được gọi khi user submit form đặt sân
    -- Hoặc sau một khoảng timeout
    
    -- Trong demo này, transaction đã được commit trong sp_GetCourtPrice_WithRepeatableRead
    -- Nhưng trong production, bạn có thể dùng temp table hoặc session state
    
    PRINT 'Transaction committed: ' + CAST(@TransactionId AS NVARCHAR(50));
END
GO

-- ==================================================================================
-- 5. SCRIPT TEST DEMO
-- ==================================================================================

PRINT '============================================';
PRINT 'SETUP COMPLETED!';
PRINT '============================================';
PRINT '';
PRINT 'Hai database đã được tạo:';
PRINT '1. TRUNGTAMTHETHAO (READ COMMITTED - có lỗi Unrepeatable Read)';
PRINT '2. TRUNGTAMTHETHAO_FIXED (REPEATABLE READ - đã fix)';
PRINT '';
PRINT 'Stored Procedures:';
PRINT '- sp_GetCourtPrice_NoIsolation (Database 1)';
PRINT '- sp_GetCourtPrice_WithRepeatableRead (Database 2)';
PRINT '';
PRINT 'Để test, chạy:';
PRINT 'EXEC sp_GetCourtPrice_NoIsolation @MaLS=''LS001'', @GioBatDau=''08:00:00''';

GO


