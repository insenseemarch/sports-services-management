-- =============================================
-- PATCH: Fix sp_DeadlockDemo_Safe to return Message column
-- Run this in SQL Server Management Studio
-- =============================================

USE TRUNGTAMTHETHAO
GO

CREATE OR ALTER PROCEDURE sp_DeadlockDemo_Safe
    @MaCaTruc BIGINT = 1,
    @MaNV VARCHAR(20) = 'NV001'
AS
BEGIN
    SET TRAN ISOLATION LEVEL SERIALIZABLE
    BEGIN TRANSACTION

    -- 1. Read Capacity WITH UPDLOCK
    DECLARE @SoLuong INT
    SELECT @SoLuong = COUNT(*) FROM CATRUC WITH (UPDLOCK) WHERE MaCaTruc = @MaCaTruc

    -- 2. Simulate Delay
    WAITFOR DELAY '00:00:10'

    -- 3. Update
    UPDATE CATRUC 
    SET GioBatDau = GioBatDau 
    WHERE MaCaTruc = @MaCaTruc

    IF NOT EXISTS (SELECT 1 FROM THAMGIACATRUC WHERE MaCaTruc = @MaCaTruc AND MaNV = @MaNV)
    BEGIN
        INSERT INTO THAMGIACATRUC (MaCaTruc, MaNV) VALUES (@MaCaTruc, @MaNV)
    END

    COMMIT TRANSACTION
    
    -- FIX: Add Message column to match Unsafe procedure
    SELECT 1 AS Success, 'Transaction committed successfully (No deadlock with UPDLOCK)' AS Message
END
GO

USE TRUNGTAMTHETHAO_FIXED
GO

CREATE OR ALTER PROCEDURE sp_DeadlockDemo_Safe
    @MaCaTruc BIGINT = 1, @MaNV VARCHAR(20) = '001'
AS
BEGIN
    SET TRAN ISOLATION LEVEL SERIALIZABLE
    BEGIN TRANSACTION
    DECLARE @SoLuong INT
    SELECT @SoLuong = COUNT(*) FROM CATRUC WITH (UPDLOCK) WHERE MaCaTruc = @MaCaTruc
    WAITFOR DELAY '00:00:10'
    UPDATE CATRUC SET GioBatDau = GioBatDau WHERE MaCaTruc = @MaCaTruc
    
    IF NOT EXISTS (SELECT 1 FROM THAMGIACATRUC WHERE MaCaTruc = @MaCaTruc AND MaNV = @MaNV)
    BEGIN
        INSERT INTO THAMGIACATRUC (MaCaTruc, MaNV) VALUES (@MaCaTruc, @MaNV)
    END
    
    COMMIT TRANSACTION
    
    -- FIX: Add Message column to match Unsafe procedure
    SELECT 1 AS Success, 'Transaction committed successfully (No deadlock with UPDLOCK)' AS Message
END
GO

PRINT 'DONE! sp_DeadlockDemo_Safe updated for both databases.'
