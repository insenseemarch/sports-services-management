-- ==================================================================================
-- PATCH: Thêm Stored Procedures cho Demo Unrepeatable Read
-- Chạy script này SAU KHI đã tạo database TRUNGTAMTHETHAO_FIXED
-- ==================================================================================

-- ==================================================================================
-- 1. TẠO SP CHO DATABASE MẶC ĐỊNH (READ COMMITTED)
-- ==================================================================================
USE TRUNGTAMTHETHAO;
GO

CREATE OR ALTER PROCEDURE sp_GetCourtPrice_NoIsolation
    @MaLS VARCHAR(20),
    @GioBatDau TIME,
    @NgayDat DATE = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Sử dụng READ COMMITTED mặc định (cho phép Unrepeatable Read)
    DECLARE @GiaApDung DECIMAL(18,2);
    DECLARE @GioBatDauKG TIME;
    DECLARE @GioKetThucKG TIME;
    DECLARE @DVT NVARCHAR(20);
    
    -- Kiểm tra ngày đặt
    IF @NgayDat IS NULL
        SET @NgayDat = GETDATE();
    
    -- Lấy giá theo khung giờ (READ COMMITTED - có thể đọc dữ liệu mới commit từ transaction khác)
    SELECT TOP 1 
        @GiaApDung = K.GiaApDung,
        @GioBatDauKG = K.GioBatDau,
        @GioKetThucKG = K.GioKetThuc,
        @DVT = LS.DVT
    FROM KHUNGGIO K
    JOIN LOAISAN LS ON K.MaLS = LS.MaLS
    WHERE K.MaLS = @MaLS 
      AND @GioBatDau >= K.GioBatDau 
      AND @GioBatDau < K.GioKetThuc
      AND K.NgayApDung <= @NgayDat
    ORDER BY K.NgayApDung DESC;
    
    -- Nếu không tìm thấy, lấy giá mặc định
    IF @GiaApDung IS NULL
    BEGIN
        SELECT TOP 1 
            @GiaApDung = K.GiaApDung,
            @GioBatDauKG = K.GioBatDau,
            @GioKetThucKG = K.GioKetThuc,
            @DVT = LS.DVT
        FROM KHUNGGIO K
        JOIN LOAISAN LS ON K.MaLS = LS.MaLS
        WHERE K.MaLS = @MaLS
        ORDER BY K.NgayApDung DESC;
    END
    
    -- Trả về kết quả
    SELECT 
        ISNULL(@GiaApDung, 0) AS GiaApDung,
        ISNULL(@GioBatDauKG, '00:00:00') AS GioBatDau,
        ISNULL(@GioKetThucKG, '23:59:59') AS GioKetThuc,
        ISNULL(@DVT, N'giờ') AS DVT,
        'READ COMMITTED' AS IsolationLevel,
        GETDATE() AS QueryTime;
END
GO

-- ==================================================================================
-- 2. TẠO SP CHO DATABASE ĐÃ FIX (REPEATABLE READ)
-- ==================================================================================
USE TRUNGTAMTHETHAO_FIXED;
GO

CREATE OR ALTER PROCEDURE sp_GetCourtPrice_WithRepeatableRead
    @MaLS VARCHAR(20),
    @GioBatDau TIME,
    @NgayDat DATE = NULL,
    @TransactionId UNIQUEIDENTIFIER OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    
    -- *** QUAN TRỌNG: Set REPEATABLE READ để ngăn chặn Unrepeatable Read ***
    SET TRANSACTION ISOLATION LEVEL REPEATABLE READ;
    
    -- Bắt đầu transaction
    BEGIN TRANSACTION;
    
    -- Tạo Transaction ID
    SET @TransactionId = NEWID();
    
    DECLARE @GiaApDung DECIMAL(18,2);
    DECLARE @GioBatDauKG TIME;
    DECLARE @GioKetThucKG TIME;
    DECLARE @DVT NVARCHAR(20);
    
    -- Kiểm tra ngày đặt
    IF @NgayDat IS NULL
        SET @NgayDat = GETDATE();
    
    -- Đọc giá lần đầu (và LOCK cho đến khi COMMIT)
    -- REPEATABLE READ sẽ giữ SHARED LOCK trên rows đã đọc
    SELECT TOP 1 
        @GiaApDung = K.GiaApDung,
        @GioBatDauKG = K.GioBatDau,
        @GioKetThucKG = K.GioKetThuc,
        @DVT = LS.DVT
    FROM KHUNGGIO K WITH (REPEATABLEREAD)
    JOIN LOAISAN LS ON K.MaLS = LS.MaLS
    WHERE K.MaLS = @MaLS 
      AND @GioBatDau >= K.GioBatDau 
      AND @GioBatDau < K.GioKetThuc
      AND K.NgayApDung <= @NgayDat
    ORDER BY K.NgayApDung DESC;
    
    -- Nếu không tìm thấy, lấy giá mặc định
    IF @GiaApDung IS NULL
    BEGIN
        SELECT TOP 1 
            @GiaApDung = K.GiaApDung,
            @GioBatDauKG = K.GioBatDau,
            @GioKetThucKG = K.GioKetThuc,
            @DVT = LS.DVT
        FROM KHUNGGIO K WITH (REPEATABLEREAD)
        JOIN LOAISAN LS ON K.MaLS = LS.MaLS
        WHERE K.MaLS = @MaLS
        ORDER BY K.NgayApDung DESC;
    END
    
    -- Trả về kết quả
    SELECT 
        ISNULL(@GiaApDung, 0) AS GiaApDung,
        ISNULL(@GioBatDauKG, '00:00:00') AS GioBatDau,
        ISNULL(@GioKetThucKG, '23:59:59') AS GioKetThuc,
        ISNULL(@DVT, N'giờ') AS DVT,
        'REPEATABLE READ' AS IsolationLevel,
        @TransactionId AS TransactionId,
        GETDATE() AS QueryTime;
    
    -- Commit transaction
    COMMIT TRANSACTION;
END
GO

PRINT '============================================';
PRINT 'SETUP COMPLETED!';
PRINT '============================================';
PRINT '';
PRINT 'Stored Procedures đã được tạo:';
PRINT '- TRUNGTAMTHETHAO.sp_GetCourtPrice_NoIsolation';
PRINT '- TRUNGTAMTHETHAO_FIXED.sp_GetCourtPrice_WithRepeatableRead';
PRINT '';
GO
