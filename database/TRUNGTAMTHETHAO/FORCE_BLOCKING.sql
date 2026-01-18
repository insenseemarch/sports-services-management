-- ==================================================================================
-- PATCH V3: Sử dụng UPDLOCK để đảm bảo Chặn (Blocking) tuyệt đối cho Demo
-- ==================================================================================

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
