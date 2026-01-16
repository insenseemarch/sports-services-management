-- =============================================
-- Author:      AntiGravity
-- Create date: 2024-01-16
-- Description: Stored Procedures for DIRTY READ Demo
--              1. Writer: sp_MaintainCourt_Delay (Updates Status with Delay + Rollback)
--              2. Reader: sp_GetSanList_Dirty (Read Uncommitted)
--              3. Reader: sp_GetSanList_Safe (Read Committed)
-- =============================================

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
