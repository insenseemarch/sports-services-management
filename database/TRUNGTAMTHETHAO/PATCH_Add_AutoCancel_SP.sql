-- =============================================
-- Author:      Auto-generated
-- Create date: 2026-01-16
-- Description: Stored procedure to automatically cancel bookings that remain unpaid after 30 minutes
-- =============================================

USE TRUNGTAMTHETHAO;
GO

CREATE OR ALTER PROCEDURE sp_TuDongHuyDonQuaHan
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @SoPhutGioiHan INT = 30;
    
    -- Cancel bookings that are in "Chờ thanh toán" status and have exceeded the time limit
    UPDATE PHIEUDATSAN
    SET TrangThai = N'Đã hủy'
    WHERE TrangThai = N'Chờ thanh toán'
      AND DATEDIFF(MINUTE, NgayTao, GETDATE()) > @SoPhutGioiHan;
      
    -- Return the number of cancelled bookings
    SELECT @@ROWCOUNT AS SoLuongHuy;
END;
GO
