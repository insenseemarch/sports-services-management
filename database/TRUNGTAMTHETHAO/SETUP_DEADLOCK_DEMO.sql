-- =============================================
-- Author:      AntiGravity
-- Create date: 2026-01-16
-- Description: Setup Deadlock Demo for Shift Assignment
--              Scenario: Two managers assign staff to shift simultaneously.
--              Mechanism: Conversion Deadlock (Read Shared -> Update Exclusive)
-- =============================================

USE TRUNGTAMTHETHAO
GO

--------------------------------------------------------------------------------
-- 1. STORED PROCEDURE: UNSAFE (REPEATABLE READ -> CONVERSION DEADLOCK)
--    Both Tx hold Shared (S) Lock on CATRUC, then both try to Upgrade to Exclusive (X).
--------------------------------------------------------------------------------
CREATE OR ALTER PROCEDURE sp_DeadlockDemo_Unsafe
    @MaCaTruc BIGINT = 1, -- Dummy target
    @MaNV VARCHAR(20) = 'NV001'
AS
BEGIN
    SET NOCOUNT ON
    SET TRAN ISOLATION LEVEL REPEATABLE READ
    
    BEGIN TRY
        BEGIN TRANSACTION

        -- 1. Read Capacity (Holds S-Lock)
        DECLARE @SoLuong INT
        SELECT @SoLuong = COUNT(*) FROM CATRUC WHERE MaCaTruc = @MaCaTruc

        -- 2. Simulate User Thinking / Form filling delay
        WAITFOR DELAY '00:00:10'

        -- 3. Perform Assignment (Requires X-Lock on CATRUC to update capacity or similar, 
        --    or just simulating the collision on the same resource)
        --    Here we update CATRUC to trigger lock conversion (THIS IS WHERE DEADLOCK OCCURS)
        UPDATE CATRUC 
        SET GioBatDau = GioBatDau -- No-op update to force X-Lock
        WHERE MaCaTruc = @MaCaTruc

        -- 4. Insert into THAMGIACATRUC (Just to match scenario)
        --    We use dynamic SQL or check existence to avoid PK errors during demo repeats
        IF NOT EXISTS (SELECT 1 FROM THAMGIACATRUC WHERE MaCaTruc = @MaCaTruc AND MaNV = @MaNV)
        BEGIN
            INSERT INTO THAMGIACATRUC (MaCaTruc, MaNV) VALUES (@MaCaTruc, @MaNV)
        END

        COMMIT TRANSACTION
        SELECT 1 AS Success, 'Transaction committed successfully' AS Message
    END TRY
    BEGIN CATCH
        -- Rollback if transaction is still open
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION
        
        -- Check if this is a deadlock error
        IF ERROR_NUMBER() = 1205
        BEGIN
            SELECT 0 AS Success, 
                   'DEADLOCK DETECTED: This transaction was chosen as the deadlock victim and has been rolled back. Chỉ có 1 transaction được lưu vào database.' AS Message,
                   1205 AS ErrorNumber
        END
        ELSE
        BEGIN
            SELECT 0 AS Success, 
                   ERROR_MESSAGE() AS Message,
                   ERROR_NUMBER() AS ErrorNumber
        END
    END CATCH
END
GO

--------------------------------------------------------------------------------
-- 2. STORED PROCEDURE: SAFE (SERIALIZABLE + UPDLOCK)
--    UPDLOCK forces Update Lock during Select, ensuring only one Tx can read-for-update at a time.
--    This effectively serializes the transactions, preventing the deadlock.
--------------------------------------------------------------------------------
CREATE OR ALTER PROCEDURE sp_DeadlockDemo_Safe
    @MaCaTruc BIGINT = 1,
    @MaNV VARCHAR(20) = 'NV001'
AS
BEGIN
    SET TRAN ISOLATION LEVEL SERIALIZABLE
    BEGIN TRANSACTION

    -- 1. Read Capacity WITH UPDLOCK (Holds U-Lock, incompatible with other U-Locks)
    --    Tx2 will wait here until Tx1 completes.
    DECLARE @SoLuong INT
    SELECT @SoLuong = COUNT(*) FROM CATRUC WITH (UPDLOCK) WHERE MaCaTruc = @MaCaTruc

    -- 2. Simulate Delay
    WAITFOR DELAY '00:00:10'

    -- 3. Update (Converts U-Lock to X-Lock, which is safe because no one else holds U/S)
    UPDATE CATRUC 
    SET GioBatDau = GioBatDau 
    WHERE MaCaTruc = @MaCaTruc

    IF NOT EXISTS (SELECT 1 FROM THAMGIACATRUC WHERE MaCaTruc = @MaCaTruc AND MaNV = @MaNV)
    BEGIN
        INSERT INTO THAMGIACATRUC (MaCaTruc, MaNV) VALUES (@MaCaTruc, @MaNV)
    END

    COMMIT TRANSACTION
    SELECT 1 AS Success
END
GO

-- REPEAT FOR FIXED DB
USE TRUNGTAMTHETHAO_FIXED
GO

CREATE OR ALTER PROCEDURE sp_DeadlockDemo_Unsafe
    @MaCaTruc BIGINT = 1, @MaNV VARCHAR(20) = 'NV001'
AS
BEGIN
    SET NOCOUNT ON
    SET TRAN ISOLATION LEVEL REPEATABLE READ
    
    BEGIN TRY
        BEGIN TRANSACTION
        DECLARE @SoLuong INT
        SELECT @SoLuong = COUNT(*) FROM CATRUC WHERE MaCaTruc = @MaCaTruc
        WAITFOR DELAY '00:00:10'
        UPDATE CATRUC SET GioBatDau = GioBatDau WHERE MaCaTruc = @MaCaTruc
        
        IF NOT EXISTS (SELECT 1 FROM THAMGIACATRUC WHERE MaCaTruc = @MaCaTruc AND MaNV = @MaNV)
        BEGIN
            INSERT INTO THAMGIACATRUC (MaCaTruc, MaNV) VALUES (@MaCaTruc, @MaNV)
        END
        
        COMMIT TRANSACTION
        SELECT 1 AS Success, 'Transaction committed successfully' AS Message
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION
        
        IF ERROR_NUMBER() = 1205
        BEGIN
            SELECT 0 AS Success, 
                   'DEADLOCK DETECTED: This transaction was chosen as the deadlock victim and has been rolled back.' AS Message,
                   1205 AS ErrorNumber
        END
        ELSE
        BEGIN
            SELECT 0 AS Success, 
                   ERROR_MESSAGE() AS Message,
                   ERROR_NUMBER() AS ErrorNumber
        END
    END CATCH
END
GO

CREATE OR ALTER PROCEDURE sp_DeadlockDemo_Safe
    @MaCaTruc BIGINT = 1, @MaNV VARCHAR(20) = 'NV001'
AS
BEGIN
    SET TRAN ISOLATION LEVEL SERIALIZABLE
    BEGIN TRANSACTION
    DECLARE @SoLuong INT
    SELECT @SoLuong = COUNT(*) FROM CATRUC WITH (UPDLOCK) WHERE MaCaTruc = @MaCaTruc
    WAITFOR DELAY '00:00:10'
    UPDATE CATRUC SET GioBatDau = GioBatDau WHERE MaCaTruc = @MaCaTruc
    COMMIT TRANSACTION
    SELECT 1 AS Success
END
GO
