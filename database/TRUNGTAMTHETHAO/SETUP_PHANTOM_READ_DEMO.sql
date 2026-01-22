-- =============================================
-- Author:      AntiGravity
-- Create date: 2026-01-16
-- Description: Setup Phantom Read Demo
--              Scenario: Manager A counts employees -> Manager B inserts -> Manager A counts again (Phantom Row)
-- =============================================

USE TRUNGTAMTHETHAO
GO

--------------------------------------------------------------------------------
-- 1. STORED PROCEDURE: READER (UNSAFE - REPEATABLE READ / READ COMMITTED)
--    Note: REPEATABLE READ allows Phantom Rows (Inserts) in standard SQL definition.
--    However, in SQL Server, REPEATABLE READ prevents updates/deletes but NOT inserts 
--    that fall into range unless range locks are taken.
--    We will use explicit delay to demonstrate the race condition.
--------------------------------------------------------------------------------
CREATE OR ALTER PROCEDURE sp_PhantomRead_Demo_Unsafe
AS
BEGIN
    SET TRAN ISOLATION LEVEL REPEATABLE READ -- Allows Phantoms
    BEGIN TRANSACTION

    -- 1. First Reading
    DECLARE @Count1 INT
    SELECT @Count1 = COUNT(*) FROM NHANVIEN

    -- 2. Simulate User Thinking / Processing Report
    WAITFOR DELAY '00:00:15'

    -- 3. Second Reading
    DECLARE @Count2 INT
    SELECT @Count2 = COUNT(*) FROM NHANVIEN

    COMMIT TRANSACTION

    -- Return Result
    SELECT @Count1 AS CountBefore, @Count2 AS CountAfter
END
GO

--------------------------------------------------------------------------------
-- 2. STORED PROCEDURE: READER (SAFE - SERIALIZABLE)
--    SERIALIZABLE takes Range Locks, preventing Inserts into the set.
--------------------------------------------------------------------------------
CREATE OR ALTER PROCEDURE sp_PhantomRead_Demo_Safe
AS
BEGIN
    SET TRAN ISOLATION LEVEL SERIALIZABLE -- Prevents Phantoms (Locks Range)
    BEGIN TRANSACTION

    -- 1. First Reading
    DECLARE @Count1 INT
    SELECT @Count1 = COUNT(*) FROM NHANVIEN

    -- 2. Simulate User Thinking
    WAITFOR DELAY '00:00:15'

    -- 3. Second Reading
    DECLARE @Count2 INT
    SELECT @Count2 = COUNT(*) FROM NHANVIEN

    COMMIT TRANSACTION

    -- Return Result
    SELECT @Count1 AS CountBefore, @Count2 AS CountAfter
END
GO


USE TRUNGTAMTHETHAO_FIXED
GO

--------------------------------------------------------------------------------
-- DUPLICATES FOR FIXED DB (Ensure consistent procedures exist)
--------------------------------------------------------------------------------

CREATE OR ALTER PROCEDURE sp_PhantomRead_Demo_Unsafe
AS
BEGIN
    SET TRAN ISOLATION LEVEL REPEATABLE READ
    BEGIN TRANSACTION
    DECLARE @Count1 INT
    SELECT @Count1 = COUNT(*) FROM NHANVIEN
    WAITFOR DELAY '00:00:15'
    DECLARE @Count2 INT
    SELECT @Count2 = COUNT(*) FROM NHANVIEN
    COMMIT TRANSACTION
    SELECT @Count1 AS CountBefore, @Count2 AS CountAfter
END
GO

CREATE OR ALTER PROCEDURE sp_PhantomRead_Demo_Safe
AS
BEGIN
    SET TRAN ISOLATION LEVEL SERIALIZABLE
    BEGIN TRANSACTION
    DECLARE @Count1 INT
    SELECT @Count1 = COUNT(*) FROM NHANVIEN
    WAITFOR DELAY '00:00:15'
    DECLARE @Count2 INT
    SELECT @Count2 = COUNT(*) FROM NHANVIEN
    COMMIT TRANSACTION
    SELECT @Count1 AS CountBefore, @Count2 AS CountAfter
END
GO
