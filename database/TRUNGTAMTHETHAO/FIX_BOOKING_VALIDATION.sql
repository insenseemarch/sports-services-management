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
