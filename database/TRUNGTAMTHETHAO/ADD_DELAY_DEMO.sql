-- ==================================================================================
-- PATCH: Thêm WAITFOR DELAY để demo Blocking behavior
-- ==================================================================================

-- 1. Default DB (Read Committed) - Sẽ KHÔNG chặn update
USE TRUNGTAMTHETHAO;
GO

ALTER PROCEDURE sp_GetCourtPrice_NoIsolation
    @MaLS VARCHAR(20),
    @GioBatDau TIME,
    @NgayDat DATE = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @GiaApDung DECIMAL(18,2);
    DECLARE @GioBatDauKG TIME;
    DECLARE @GioKetThucKG TIME;
    DECLARE @DVT NVARCHAR(20);
    
    IF @NgayDat IS NULL SET @NgayDat = GETDATE();
    
    -- SELECT xong sẽ nhả Shared Lock ngay
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

    -- Delay 10s giả lập xử lý
    -- Trong thời gian này, Quản lý vẫn UPDATE được vì không còn lock
    WAITFOR DELAY '00:00:10';
    
    SELECT 
        ISNULL(@GiaApDung, 0) AS GiaApDung,
        ISNULL(@GioBatDauKG, '00:00:00') AS GioBatDau,
        ISNULL(@GioKetThucKG, '23:59:59') AS GioKetThuc,
        ISNULL(@DVT, N'giờ') AS DVT,
        'READ COMMITTED (Delayed)' AS IsolationLevel,
        GETDATE() AS QueryTime;
END
GO


-- 2. Fixed DB (Repeatable Read) - Sẽ CHẶN update
USE TRUNGTAMTHETHAO_FIXED;
GO

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
    
    DECLARE @GiaApDung DECIMAL(18,2);
    DECLARE @GioBatDauKG TIME;
    DECLARE @GioKetThucKG TIME;
    DECLARE @DVT NVARCHAR(20);
    
    IF @NgayDat IS NULL SET @NgayDat = GETDATE();
    
    -- SELECT này sẽ giữ Shared Lock
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
    
    -- Delay 10s VÀ VẪN GIỮ LOCK
    -- Trong thời gian này, Manager Update sẽ bị BLOCKED
    WAITFOR DELAY '00:00:10';
    
    SELECT 
        ISNULL(@GiaApDung, 0) AS GiaApDung,
        ISNULL(@GioBatDauKG, '00:00:00') AS GioBatDau,
        ISNULL(@GioKetThucKG, '23:59:59') AS GioKetThuc,
        ISNULL(@DVT, N'giờ') AS DVT,
        'REPEATABLE READ (Delayed)' AS IsolationLevel,
        @TransactionId AS TransactionId,
        GETDATE() AS QueryTime;
    
    COMMIT TRANSACTION; -- Chỉ nhả lock tại đây
END
GO
