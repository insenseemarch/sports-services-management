-- ==================================================================================
-- PATCH V2: Sửa logic SP để Re-Query dữ liệu (Demo Unrepeatable Read chuẩn)
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


-- 2. Fixed DB (Repeatable Read) -> Sẽ thấy GIÁ CŨ (ĐÚNG)
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
    SET TRANSACTION ISOLATION LEVEL REPEATABLE READ; -- GIỮ KHÓA S
    
    BEGIN TRANSACTION;
    SET @TransactionId = NEWID();

    IF @NgayDat IS NULL SET @NgayDat = GETDATE();
    
    -- 1. Đọc lần 1 (Sẽ giữ Shared Lock trên dòng này)
    -- Dù chỉ gán vào biến nhưng SQL Server vẫn phải access data page -> LockRow
    DECLARE @GiaTam DECIMAL(18,2);
    SELECT TOP 1 @GiaTam = GiaApDung
    FROM KHUNGGIO 
    WHERE MaLS = @MaLS 
      AND @GioBatDau >= GioBatDau 
      AND @GioBatDau < GioKetThuc
      AND NgayApDung <= @NgayDat
    ORDER BY NgayApDung DESC;
    
    -- 2. Delay 10s
    -- Transaction Update bên kia sẽ bị TREO vì không xin được X-Lock
    WAITFOR DELAY '00:00:10';
    
    -- 3. Đọc lần 2 VÀ TRẢ VỀ
    -- Do Update chưa chạy xong (vì bị treo), nên giá trị vẫn CŨ -> ỔN ĐỊNH
    SELECT TOP 1 
        ISNULL(K.GiaApDung, 0) AS GiaApDung, -- Re-query trực tiếp
        ISNULL(K.GioBatDau, '00:00:00') AS GioBatDau,
        ISNULL(K.GioKetThuc, '23:59:59') AS GioKetThuc,
        ISNULL(LS.DVT, N'giờ') AS DVT,
        'REPEATABLE READ (Fixed)' AS IsolationLevel,
        @TransactionId AS TransactionId,
        GETDATE() AS QueryTime
    FROM KHUNGGIO K
    JOIN LOAISAN LS ON K.MaLS = LS.MaLS
    WHERE K.MaLS = @MaLS 
      AND @GioBatDau >= K.GioBatDau 
      AND @GioBatDau < K.GioKetThuc
      AND K.NgayApDung <= @NgayDat
    ORDER BY K.NgayApDung DESC;
    
    COMMIT TRANSACTION; -- Nhả Lock -> Manager mới update được
END
GO
