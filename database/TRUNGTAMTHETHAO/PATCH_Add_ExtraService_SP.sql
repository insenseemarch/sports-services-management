-- PATCH: Stored Procedures for ordering extra services
USE TRUNGTAMTHETHAO
GO

-- 1. SP Get Invoice Info by Booking ID or Invoice ID
CREATE OR ALTER PROCEDURE sp_LayThongTinHoaDon
    @MaTraCuu NVARCHAR(50) -- Can be MaDatSan (DS...) or MaHD (Number)
AS
BEGIN
    DECLARE @MaDatSan BIGINT
    DECLARE @MaHD BIGINT

    -- Determine if input is MaDatSan or MaHD
    IF LEFT(@MaTraCuu, 2) = 'DS'
    BEGIN
        SET @MaDatSan = CAST(SUBSTRING(@MaTraCuu, 3, LEN(@MaTraCuu)) AS BIGINT)
        SELECT @MaHD = MaHD FROM HOADON WHERE MaPhieu = @MaDatSan
    END
    ELSE
    BEGIN
        SET @MaHD = CAST(@MaTraCuu AS BIGINT)
        SELECT @MaDatSan = MaPhieu FROM HOADON WHERE MaHD = @MaHD
    END

    IF @MaHD IS NULL
    BEGIN
        -- Check if Booking exists but no Invoice
        IF EXISTS (SELECT 1 FROM PHIEUDATSAN WHERE MaDatSan = @MaDatSan)
        BEGIN
             -- Case: Booking exists but NOT Paid yet
             SELECT 
                P.MaDatSan,
                CAST(NULL AS BIGINT) as MaHD,
                K.HoTen as TenKH,
                CS.TenCS,
                S.MaSan,
                P.NgayDat,
                P.GioBatDau,
                P.GioKetThuc,
                P.TrangThai as TrangThaiPhieu,
                CAST(0 AS BIT) as DaThanhToan,
                CAST(0 AS DECIMAL(18,2)) as TongTienDaTra
            FROM PHIEUDATSAN P
            JOIN KHACHHANG K ON P.MaKH = K.MaKH
            JOIN DATSAN D ON P.MaDatSan = D.MaDatSan
            JOIN SAN S ON D.MaSan = S.MaSan
            JOIN COSO CS ON S.MaCS = CS.MaCS
            WHERE P.MaDatSan = @MaDatSan
            RETURN
        END
        ELSE
        BEGIN
            RAISERROR(N'Không tìm thấy thông tin đặt sân hoặc hóa đơn!', 16, 1)
            RETURN
        END
    END

    -- Case: Invoice exists (Paid)
    SELECT 
        P.MaDatSan,
        H.MaHD,
        K.HoTen as TenKH,
        CS.TenCS,
        S.MaSan,
        P.NgayDat,
        P.GioBatDau,
        P.GioKetThuc,
        P.TrangThai as TrangThaiPhieu,
        CAST(1 AS BIT) as DaThanhToan,
        H.ThanhTien as TongTienDaTra
    FROM HOADON H
    JOIN PHIEUDATSAN P ON H.MaPhieu = P.MaDatSan
    JOIN KHACHHANG K ON P.MaKH = K.MaKH
    JOIN DATSAN D ON P.MaDatSan = D.MaDatSan
    JOIN SAN S ON D.MaSan = S.MaSan
    JOIN COSO CS ON S.MaCS = CS.MaCS
    WHERE H.MaHD = @MaHD
END
GO

-- 2. SP Add Service and Update Invoice (Handles both Paid and Unpaid)
CREATE OR ALTER PROCEDURE sp_ThemDichVuVaCapNhatHoaDon
    @MaDatSan BIGINT,
    @MaDV VARCHAR(20),
    @SoLuong INT,
    @MaNV VARCHAR(20) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;

        -- Check if Invoice exists
        DECLARE @MaHD BIGINT
        DECLARE @TongTienCu DECIMAL(18,2)
        DECLARE @GiamGia DECIMAL(18,2)
        DECLARE @DaThanhToan BIT = 0
        
        SELECT @MaHD = MaHD, @TongTienCu = TongTien, @GiamGia = ISNULL(GiamGia, 0)
        FROM HOADON WHERE MaPhieu = @MaDatSan

        IF @MaHD IS NOT NULL
            SET @DaThanhToan = 1

        -- Get Facility of the Booking
        DECLARE @MaCS VARCHAR(20)
        SELECT @MaCS = S.MaCS 
        FROM DATSAN D JOIN SAN S ON D.MaSan = S.MaSan 
        WHERE D.MaDatSan = @MaDatSan

        IF @MaCS IS NULL
        BEGIN
            RAISERROR(N'Không tìm thấy thông tin đặt sân!', 16, 1);
        END

        -- Get Service Price and Check Inventory
        DECLARE @DonGia DECIMAL(18,2)
        DECLARE @SoLuongTon INT
        
        SELECT @DonGia = DV.DonGia, @SoLuongTon = DVC.SoLuongTon
        FROM DICHVU DV
        JOIN DV_COSO DVC ON DV.MaDV = DVC.MaDV
        WHERE DV.MaDV = @MaDV AND DVC.MaCS = @MaCS

        IF @DonGia IS NULL
        BEGIN
            RAISERROR(N'Dịch vụ không tồn tại tại cơ sở này!', 16, 1);
        END

        IF @SoLuongTon < @SoLuong
        BEGIN
            RAISERROR(N'Số lượng tồn kho không đủ!', 16, 1);
        END

        -- Update Inventory
        UPDATE DV_COSO SET SoLuongTon = SoLuongTon - @SoLuong WHERE MaDV = @MaDV AND MaCS = @MaCS;

        -- Insert or Update CT_DICHVUDAT
        DECLARE @TienThem DECIMAL(18,2) = @SoLuong * @DonGia
        
        IF EXISTS (SELECT 1 FROM CT_DICHVUDAT WHERE MaDatSan = @MaDatSan AND MaDV = @MaDV)
        BEGIN
            UPDATE CT_DICHVUDAT
            SET SoLuong = SoLuong + @SoLuong,
                ThanhTien = ThanhTien + @TienThem
            WHERE MaDatSan = @MaDatSan AND MaDV = @MaDV
        END
        ELSE
        BEGIN
            DECLARE @TrangThaiDV NVARCHAR(50) = CASE WHEN @DaThanhToan = 1 THEN N'Chưa thanh toán' ELSE N'Chờ thanh toán' END
            INSERT INTO CT_DICHVUDAT (MaDV, MaDatSan, SoLuong, ThanhTien, TrangThaiSuDung, GhiChu)
            VALUES (@MaDV, @MaDatSan, @SoLuong, @TienThem, @TrangThaiDV, N'Đặt thêm')
        END

        DECLARE @TongTienMoi DECIMAL(18,2)
        DECLARE @TienDaTra DECIMAL(18,2) = 0
        DECLARE @CanThanhToanThem DECIMAL(18,2)

        IF @DaThanhToan = 1
        BEGIN
            -- CASE 1: Already Paid - Update existing invoice
            SET @TienDaTra = @TongTienCu - @GiamGia
            
            UPDATE HOADON
            SET TongTien = TongTien + @TienThem,
                ThanhTien = ThanhTien + @TienThem
            WHERE MaHD = @MaHD

            SET @TongTienMoi = @TongTienCu + @TienThem
            SET @CanThanhToanThem = @TienThem
        END
        ELSE
        BEGIN
            -- CASE 2: Not Yet Paid - Calculate total for future payment
            DECLARE @TienSan DECIMAL(18,2)
            SELECT @TienSan = dbo.f_TinhTienSan(@MaDatSan)
            
            DECLARE @TongDichVu DECIMAL(18,2)
            SELECT @TongDichVu = ISNULL(SUM(ThanhTien), 0)
            FROM CT_DICHVUDAT
            WHERE MaDatSan = @MaDatSan
            
            SET @TongTienMoi = @TienSan + @TongDichVu
            SET @CanThanhToanThem = @TongTienMoi -- For unpaid, "additional" is the whole amount
        END

        COMMIT TRANSACTION;

        -- Return Info
        SELECT 
            @DaThanhToan as DaThanhToan,
            @MaHD as MaHD,
            @TongTienMoi as TongTienMoi,
            @TienDaTra as TienDaTra,
            @CanThanhToanThem as CanThanhToanThem,
            @TienThem as GiaTriDichVu,
            N'Thêm dịch vụ thành công!' as Message
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(@ErrorMessage, 16, 1);
    END CATCH
END
GO
