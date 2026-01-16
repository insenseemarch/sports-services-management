-- ==================================================================================
-- SETUP DEADLOCK DEMO: Cập nhật lương nhân viên
-- ==================================================================================

-- 1. Setup Data for Default DB
USE TRUNGTAMTHETHAO;
GO

-- Ensure NVKT002 exists
IF NOT EXISTS (SELECT 1 FROM NHANVIEN WHERE MaNV = 'NVKT002')
BEGIN
    INSERT INTO NHANVIEN (MaNV, HoTen, CMND_CCCD, LuongCoBan, TenDangNhap, MatKhau)
    VALUES ('NVKT002', N'Kỹ Thuật Viên Demo', '888888888', 5000000, 'nvkt002_demo', '123456');
END
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

-- Ensure NVKT002 exists
IF NOT EXISTS (SELECT 1 FROM NHANVIEN WHERE MaNV = 'NVKT002')
BEGIN
    INSERT INTO NHANVIEN (MaNV, HoTen, CMND_CCCD, LuongCoBan, TenDangNhap, MatKhau)
    VALUES ('NVKT002', N'Kỹ Thuật Viên Demo', '888888888', 5000000, 'nvkt002_demo', '123456');
END
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
