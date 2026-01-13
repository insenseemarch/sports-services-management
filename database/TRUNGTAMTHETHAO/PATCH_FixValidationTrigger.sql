-- =============================================
-- PATCH: Sửa trigger KiemTraThoiLuongDat để bỏ qua khi Hủy sân
-- Mục đích: Cho phép hủy sân mà không bị validate lại thời lượng (tránh lỗi với data cũ)
-- =============================================

USE TRUNGTAMTHETHAO;
GO

CREATE OR ALTER TRIGGER trg_KiemTraThoiLuongDat
ON PHIEUDATSAN
FOR INSERT, UPDATE
AS
BEGIN
    -- Nếu là UPDATE và KHÔNG đổi giờ thì bỏ qua (cho phép Hủy sân thoải mái)
    IF EXISTS (SELECT 1 FROM deleted) 
    BEGIN
        IF NOT UPDATE(GioBatDau) AND NOT UPDATE(GioKetThuc)
            RETURN;
    END

    IF NOT EXISTS (SELECT 1 FROM inserted) RETURN;

    DECLARE @GioBD TIME, @GioKT TIME, @LoaiSan NVARCHAR(50);
    DECLARE @ThoiLuong INT;
    DECLARE @GioMoCua TIME = '06:00:00'; -- Giả định giờ mở cửa
    DECLARE @GioDongCua TIME = '22:00:00'; -- Giả định giờ đóng cửa

    SELECT @GioBD = I.GioBatDau, @GioKT = I.GioKetThuc, @LoaiSan = LS.TenLS
    FROM inserted I
    JOIN DATSAN D ON I.MaDatSan = D.MaDatSan
    JOIN SAN S ON D.MaSan = S.MaSan
    JOIN LOAISAN LS ON S.MaLS = LS.MaLS;

    -- 1. Kiểm tra khung giờ hoạt động
    IF @GioBD < @GioMoCua OR @GioKT > @GioDongCua
    BEGIN
        RAISERROR (N'Lỗi: Thời gian đặt nằm ngoài khung giờ hoạt động của cơ sở!', 16, 1);
        ROLLBACK TRANSACTION;
        RETURN;
    END

    -- 2. Kiểm tra thời lượng theo loại sân
    SET @ThoiLuong = DATEDIFF(MINUTE, @GioBD, @GioKT);

    IF @LoaiSan = N'Bóng đá mini' AND (@ThoiLuong % 90 <> 0 OR @ThoiLuong < 90)
    BEGIN
        RAISERROR (N'Lỗi: Sân bóng đá mini phải đặt theo bội số của 90 phút (1 trận = 90 phút)!', 16, 1);
        ROLLBACK TRANSACTION;
        RETURN;
    END

    IF @LoaiSan = N'Tennis' AND (@ThoiLuong % 120 <> 0)
    BEGIN
        RAISERROR (N'Lỗi: Sân Tennis phải đặt theo bội số của 2 giờ (120 phút)!', 16, 1);
        ROLLBACK TRANSACTION;
        RETURN;
    END

    IF (@LoaiSan = N'Cầu lông' OR @LoaiSan = N'Bóng rổ') AND (@ThoiLuong % 60 <> 0)
    BEGIN
        RAISERROR (N'Lỗi: Sân Cầu lông/Bóng rổ phải đặt theo bội số của 1 giờ!', 16, 1);
        ROLLBACK TRANSACTION;
        RETURN;
    END
END;
GO

PRINT 'PATCH COMPLETED: Trigger trg_KiemTraThoiLuongDat đã được cập nhật';
