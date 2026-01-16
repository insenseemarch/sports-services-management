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


-- 1. Setup Data for Default DB
USE TRUNGTAMTHETHAO;
GO

-- PROCEDURE 1: Unsafe Update (Read Committed) -> Lost Update
CREATE OR ALTER PROCEDURE sp_UpdateSalary_Unsafe
    @MaNV VARCHAR(20),
    @LuongMoi DECIMAL(18,2)
AS
BEGIN
    SET NOCOUNT ON;
    SET TRANSACTION ISOLATION LEVEL READ COMMITTED; -- Mặc định
    
    BEGIN TRANSACTION;
    
    -- 1. Đọc lương cũ (Release S-Lock ngay)
    DECLARE @LuongCu DECIMAL(18,2);
    SELECT @LuongCu = LuongCoBan 
    FROM NHANVIEN 
    WHERE MaNV = @MaNV;
    
    -- 2. Delay 10s (Simulate User thinking or Verify time)
    WAITFOR DELAY '00:00:10';
    
    -- 3. Update (Ghi đè - Lost Update nếu có người khác update trong lúc delay)
    UPDATE NHANVIEN
    SET LuongCoBan = @LuongMoi
    WHERE MaNV = @MaNV;
    
    COMMIT TRANSACTION;
    
    SELECT 
        @LuongCu as LuongCu, 
        @LuongMoi as LuongMoi, 
        'Success (Unsafe)' as Status;
END
GO

-- 2. Setup Data for Fixed DB
USE TRUNGTAMTHETHAO_FIXED;
GO

-- PROCEDURE 2: Safe Update (Repeatable Read) -> Deadlock
CREATE OR ALTER PROCEDURE sp_UpdateSalary_Safe
    @MaNV VARCHAR(20),
    @LuongMoi DECIMAL(18,2)
AS
BEGIN
    SET NOCOUNT ON;
    -- *** KEY FIX: REPEATABLE READ ***
    -- Giữ Shared Lock khi đọc -> Cả 2 Transaction đều giữ S-Lock
    -- Khi cả 2 cố gắng Update (cần X-Lock) -> Deadlock
    SET TRANSACTION ISOLATION LEVEL REPEATABLE READ; 
    
    BEGIN TRANSACTION;
    
    -- 1. Đọc lương cũ (Giữ S-Lock cho đến khi Commit)
    DECLARE @LuongCu DECIMAL(18,2);
    SELECT @LuongCu = LuongCoBan 
    FROM NHANVIEN 
    WHERE MaNV = @MaNV;
    
    -- 2. Delay 10s
    WAITFOR DELAY '00:00:10';
    
    -- 3. Update (Cần X-Lock -> Conflict với S-Lock của Transaction kia -> Deadlock)
    UPDATE NHANVIEN
    SET LuongCoBan = @LuongMoi
    WHERE MaNV = @MaNV;
    
    COMMIT TRANSACTION;
    
    SELECT 
        @LuongCu as LuongCu, 
        @LuongMoi as LuongMoi, 
        'Success (Safe)' as Status;
END
GO

-- PROCEDURE 3: Real Fix (UPDLOCK) -> No Deadlock, converts to Blocking
CREATE OR ALTER PROCEDURE sp_UpdateSalary_RealFix
    @MaNV VARCHAR(20),
    @LuongMoi DECIMAL(18,2)
AS
BEGIN
    SET NOCOUNT ON;
    SET TRANSACTION ISOLATION LEVEL READ COMMITTED; 
    
    BEGIN TRANSACTION;
    
    -- 1. Đọc lương cũ VÀ giữ luôn Update Lock (UPDLOCK)
    -- UPDLOCK không chặn S-Lock (đọc), nhưng chặn U-Lock/X-Lock khác.
    -- Quan trọng: Chỉ 1 người được giữ U-Lock tại 1 thời điểm -> Người đến sau sẽ chờ ngay tại đây
    -- -> Tránh được việc 2 người cùng giữ S-Lock rồi cùng convert lên X-Lock (nguyên nhân Deadlock)
    DECLARE @LuongCu DECIMAL(18,2);
    SELECT @LuongCu = LuongCoBan 
    FROM NHANVIEN WITH (UPDLOCK) 
    WHERE MaNV = @MaNV;
    
    -- 2. Delay 10s (Người thứ 2 đang chờ ở step 1 nên không chạy được tới dòng waitfor này)
    WAITFOR DELAY '00:00:10';
    
    -- 3. Update (Chuyển U-Lock thành X-Lock dễ dàng vì mình đang giữ U-Lock độc quyền)
    UPDATE NHANVIEN
    SET LuongCoBan = @LuongMoi
    WHERE MaNV = @MaNV;
    
    COMMIT TRANSACTION;
    
    SELECT 
        @LuongCu as LuongCu, 
        @LuongMoi as LuongMoi, 
        'Success (Real Fix)' as Status;
END
GO

USE TRUNGTAMTHETHAO
GO

--------------------------------------------------------------------------------
-- 1. STORED PROCEDURE: WRITER (Simulation)
--    Update Court Status to 'Đang bảo trì' -> Wait 15s -> Rollback
--------------------------------------------------------------------------------
CREATE OR ALTER PROCEDURE sp_MaintainCourt_Delay
    @MaSan VARCHAR(20),
    @TinhTrang NVARCHAR(50) -- e.g. N'Đang bảo trì'
AS
BEGIN
    BEGIN TRANSACTION
    
    -- Update Status
    UPDATE SAN
    SET TinhTrang = @TinhTrang
    WHERE MaSan = @MaSan

    -- Wait 15 seconds (Simulating long process or user delay)
    WAITFOR DELAY '00:00:15'
    
    -- Rollback (Simulating cancellation or error)
    ROLLBACK TRANSACTION
END
GO

--------------------------------------------------------------------------------
-- 2. STORED PROCEDURE: READER (DIRTY READ - Unsafe)
--    Uses READ UNCOMMITTED to see the uncommitted status
--------------------------------------------------------------------------------
CREATE OR ALTER PROCEDURE sp_GetSanList_Dirty
    @MaCS VARCHAR(20) = NULL,
    @MaLS VARCHAR(20) = NULL
AS
BEGIN
    SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED -- KEY POINT

    SELECT 
        s.MaSan, 
        s.MaSan as TenSan, -- Using MaSan as Name
        s.TinhTrang,
        ls.TenLS,
        cs.TenCS,
        s.SucChua,
        0 as GiaTien -- Dummy column, not used in demo view
    FROM SAN s
    JOIN LOAISAN ls ON s.MaLS = ls.MaLS
    JOIN COSO cs ON s.MaCS = cs.MaCS
    WHERE (@MaCS IS NULL OR s.MaCS = @MaCS)
      AND (@MaLS IS NULL OR s.MaLS = @MaLS)
END
GO


USE TRUNGTAMTHETHAO_FIXED
GO

--------------------------------------------------------------------------------
-- 1. STORED PROCEDURE: WRITER (Duplicate for Fixed DB)
--------------------------------------------------------------------------------
CREATE OR ALTER PROCEDURE sp_MaintainCourt_Delay
    @MaSan VARCHAR(20),
    @TinhTrang NVARCHAR(50)
AS
BEGIN
    BEGIN TRANSACTION
    
    UPDATE SAN
    SET TinhTrang = @TinhTrang
    WHERE MaSan = @MaSan

    WAITFOR DELAY '00:00:15'
    
    ROLLBACK TRANSACTION
END
GO

--------------------------------------------------------------------------------
-- 3. STORED PROCEDURE: READER (SAFE READ)
--    Uses READ COMMITTED (Default) to block until transaction finishes
--------------------------------------------------------------------------------
CREATE OR ALTER PROCEDURE sp_GetSanList_Safe
    @MaCS VARCHAR(20) = NULL,
    @MaLS VARCHAR(20) = NULL
AS
BEGIN
    SET TRANSACTION ISOLATION LEVEL READ COMMITTED -- KEY POINT

    SELECT 
        s.MaSan, 
        s.MaSan as TenSan,
        s.TinhTrang,
        ls.TenLS,
        cs.TenCS,
        s.SucChua,
        0 as GiaTien
    FROM SAN s
    JOIN LOAISAN ls ON s.MaLS = ls.MaLS
    JOIN COSO cs ON s.MaCS = cs.MaCS
    WHERE (@MaCS IS NULL OR s.MaCS = @MaCS)
      AND (@MaLS IS NULL OR s.MaLS = @MaLS)
END
GO

-- =============================================
-- Author:      AntiGravity
-- Create date: 2024-01-16
-- Description: Patch sp_DatSan to validate Court Status
--              Prevents booking if Status is 'Bảo Trì', 'Đang Bảo Trì', or 'Ngưng hoạt động'
-- =============================================

USE TRUNGTAMTHETHAO
GO

DROP PROCEDURE IF EXISTS sp_DatSan
GO

CREATE PROCEDURE sp_DatSan
    @MaKH VARCHAR(20),
    @NguoiLap VARCHAR(20), 
    @MaSan VARCHAR(20),
    @NgayDat DATE,
    @GioBatDau TIME,
    @GioKetThuc TIME,
    @KenhDat NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;
    SET TRAN ISOLATION LEVEL SERIALIZABLE; -- Khóa phạm vi thời gian
    
    BEGIN TRY
        BEGIN TRAN; 

        -- 1. VALIDATION: Check Court Status (NEW)
        IF EXISTS (SELECT 1 FROM SAN 
                   WHERE MaSan = @MaSan 
                   AND TinhTrang IN (N'Bảo Trì', N'Đang Bảo Trì', N'Ngưng Hoạt Động', N'Không Hoạt Động'))
        BEGIN
            ROLLBACK TRAN;
            RAISERROR(N'Lỗi: Sân đang bảo trì hoặc ngưng hoạt động!', 16, 1);
            RETURN;
        END

        -- 2. VALIDATION: Check Overlap (Existing)
        IF dbo.f_KiemTraSanTrong(@MaSan, @NgayDat, @GioBatDau, @GioKetThuc, NULL) = 0
        BEGIN
            ROLLBACK TRAN; 
            RAISERROR(N'Lỗi: Sân đã bị người khác đặt!', 16, 1);
            RETURN;
        END

        -- 3. VALIDATION: Online Booking Time (Existing)
        IF @KenhDat = 'Online' AND DATEDIFF(HOUR, GETDATE(), CAST(@NgayDat AS DATETIME) + CAST(@GioBatDau AS DATETIME)) < 2
        BEGIN
             ROLLBACK TRAN;
             RAISERROR(N'Lỗi: Đặt Online phải trước 2 tiếng!', 16, 1);
             RETURN;
        END

        -- 4. INSERT BOOKING
        INSERT INTO PHIEUDATSAN (MaKH, NguoiLap, NgayDat, NgayKetThuc, GioBatDau, GioKetThuc, KenhDat, TrangThai)
        VALUES (@MaKH, @NguoiLap, @NgayDat, @NgayDat, @GioBatDau, @GioKetThuc, @KenhDat, N'Nháp');
        
        DECLARE @MaDatSan BIGINT = SCOPE_IDENTITY();
        INSERT INTO DATSAN (MaDatSan, MaSan) VALUES (@MaDatSan, @MaSan);

        COMMIT TRAN; 
        PRINT N'Đặt sân thành công! Mã: ' + CAST(@MaDatSan AS VARCHAR(20));
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRAN;
        DECLARE @Msg NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(@Msg, 16, 1);
    END CATCH
END
GO

USE TRUNGTAMTHETHAO_FIXED
GO

DROP PROCEDURE IF EXISTS sp_DatSan
GO

CREATE PROCEDURE sp_DatSan
    @MaKH VARCHAR(20),
    @NguoiLap VARCHAR(20), 
    @MaSan VARCHAR(20),
    @NgayDat DATE,
    @GioBatDau TIME,
    @GioKetThuc TIME,
    @KenhDat NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;
    SET TRAN ISOLATION LEVEL SERIALIZABLE; -- Khóa phạm vi thời gian
    
    BEGIN TRY
        BEGIN TRAN; 

        -- 1. VALIDATION: Check Court Status (NEW)
        IF EXISTS (SELECT 1 FROM SAN 
                   WHERE MaSan = @MaSan 
                   AND TinhTrang IN (N'Bảo Trì', N'Đang Bảo Trì', N'Ngưng Hoạt Động', N'Không Hoạt Động'))
        BEGIN
            ROLLBACK TRAN;
            RAISERROR(N'Lỗi: Sân đang bảo trì hoặc ngưng hoạt động!', 16, 1);
            RETURN;
        END

        -- 2. VALIDATION: Check Overlap (Existing)
        IF dbo.f_KiemTraSanTrong(@MaSan, @NgayDat, @GioBatDau, @GioKetThuc, NULL) = 0
        BEGIN
            ROLLBACK TRAN; 
            RAISERROR(N'Lỗi: Sân đã bị người khác đặt!', 16, 1);
            RETURN;
        END

        -- 3. VALIDATION: Online Booking Time (Existing)
        IF @KenhDat = 'Online' AND DATEDIFF(HOUR, GETDATE(), CAST(@NgayDat AS DATETIME) + CAST(@GioBatDau AS DATETIME)) < 2
        BEGIN
             ROLLBACK TRAN;
             RAISERROR(N'Lỗi: Đặt Online phải trước 2 tiếng!', 16, 1);
             RETURN;
        END

        -- 4. INSERT BOOKING
        INSERT INTO PHIEUDATSAN (MaKH, NguoiLap, NgayDat, NgayKetThuc, GioBatDau, GioKetThuc, KenhDat, TrangThai)
        VALUES (@MaKH, @NguoiLap, @NgayDat, @NgayDat, @GioBatDau, @GioKetThuc, @KenhDat, N'Nháp');
        
        DECLARE @MaDatSan BIGINT = SCOPE_IDENTITY();
        INSERT INTO DATSAN (MaDatSan, MaSan) VALUES (@MaDatSan, @MaSan);

        COMMIT TRAN; 
        PRINT N'Đặt sân thành công! Mã: ' + CAST(@MaDatSan AS VARCHAR(20));
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRAN;
        DECLARE @Msg NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(@Msg, 16, 1);
    END CATCH
END
GO

