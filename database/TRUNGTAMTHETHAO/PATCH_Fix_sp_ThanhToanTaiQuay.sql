CREATE OR ALTER PROCEDURE sp_ThanhToanTaiQuay
    -- VERSION: FIX_PAYMENT_V3_NO_CHECK
    @MaDatSan BIGINT
AS
BEGIN
    SET NOCOUNT ON;
    SET TRAN ISOLATION LEVEL REPEATABLE READ;
    
    BEGIN TRY
        BEGIN TRAN;
        
        DECLARE @TrangThaiHienTai NVARCHAR(50);
        SELECT @TrangThaiHienTai = TrangThai FROM PHIEUDATSAN WHERE MaDatSan = @MaDatSan;

        -- [ EMERGENCY FIX ] BỎ QUA CHECK TRÙNG

        -- UPDATE STATUS
        IF @TrangThaiHienTai = N'Nháp'
             UPDATE PHIEUDATSAN SET TrangThai = N'Chờ thanh toán', NgayTao = GETDATE() WHERE MaDatSan = @MaDatSan;
        
        COMMIT TRAN;
        PRINT N'Chuyển sang trạng thái chờ thanh toán thành công!';
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRAN;
        DECLARE @Msg NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(@Msg, 16, 1);
    END CATCH
END
