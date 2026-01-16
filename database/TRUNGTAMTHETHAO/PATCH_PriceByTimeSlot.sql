-- =============================================
-- PATCH: Sửa function f_TinhTienSan tính giá theo TỪNG KHUNG GIỜ
-- Mục đích: Tính chính xác giá sân khi đặt qua nhiều khung giờ khác nhau
-- =============================================

USE TRUNGTAMTHETHAO;
GO

CREATE OR ALTER FUNCTION f_TinhTienSan (@MaDatSan BIGINT)
RETURNS DECIMAL(18,2)
AS
BEGIN
    DECLARE @TienSan DECIMAL(18,2) = 0;
    DECLARE @MaLS VARCHAR(20);
    DECLARE @TenLS NVARCHAR(50);
    DECLARE @GioBatDau TIME;
    DECLARE @GioKetThuc TIME;
    DECLARE @NgayDat DATE;
    DECLARE @CurrentHour TIME;
    DECLARE @NextHour TIME;
    DECLARE @GiaKhung DECIMAL(18,2);
    DECLARE @DurationMinutes INT;

    -- Lấy thông tin phiếu đặt và loại sân
    SELECT @MaLS = S.MaLS, @TenLS = LS.TenLS, 
           @GioBatDau = P.GioBatDau, @GioKetThuc = P.GioKetThuc,
           @NgayDat = P.NgayDat
    FROM PHIEUDATSAN P
    JOIN DATSAN D ON P.MaDatSan = D.MaDatSan
    JOIN SAN S ON D.MaSan = S.MaSan
    JOIN LOAISAN LS ON S.MaLS = LS.MaLS
    WHERE P.MaDatSan = @MaDatSan;

    -- Tính thời lượng (phút)
    SET @DurationMinutes = DATEDIFF(MINUTE, @GioBatDau, @GioKetThuc);

    -- TÍNH TIỀN THEO TỪNG KHUNG GIỜ
    -- Duyệt từng giờ từ GioBatDau đến GioKetThuc
    SET @CurrentHour = @GioBatDau;
    
    WHILE @CurrentHour < @GioKetThuc
    BEGIN
        -- Tính giờ tiếp theo (mỗi lần tăng 1 giờ)
        SET @NextHour = DATEADD(HOUR, 1, @CurrentHour);
        IF @NextHour > @GioKetThuc
            SET @NextHour = @GioKetThuc;
        
        -- Tìm giá khung giờ chứa @CurrentHour
        SELECT TOP 1 @GiaKhung = K.GiaApDung
        FROM KHUNGGIO K
        WHERE K.MaLS = @MaLS 
          AND @CurrentHour >= K.GioBatDau 
          AND @CurrentHour < K.GioKetThuc
          AND K.NgayApDung <= @NgayDat
        ORDER BY K.NgayApDung DESC;
        
        -- Nếu không tìm thấy khung giờ phù hợp, lấy khung GẦN NHẤT
        IF @GiaKhung IS NULL
        BEGIN
            -- Tìm khung giờ gần nhất (trước hoặc sau)
            SELECT TOP 1 @GiaKhung = K.GiaApDung
            FROM KHUNGGIO K
            WHERE K.MaLS = @MaLS 
              AND K.NgayApDung <= @NgayDat
            ORDER BY 
                ABS(DATEDIFF(MINUTE, @CurrentHour, K.GioBatDau)),
                K.NgayApDung DESC;
        END
        
        -- Cộng giá vào tổng (tính theo tỷ lệ thời gian thực tế)
        DECLARE @ActualMinutes INT = DATEDIFF(MINUTE, @CurrentHour, @NextHour);
        SET @TienSan = @TienSan + (ISNULL(@GiaKhung, 0) * @ActualMinutes / 60.0);
        
        -- Reset giá cho vòng lặp tiếp theo
        SET @GiaKhung = NULL;
        
        -- Tăng giờ
        SET @CurrentHour = @NextHour;
    END

    RETURN @TienSan;
END
GO

PRINT 'PATCH COMPLETED: Function f_TinhTienSan đã được cập nhật để tính giá theo từng khung giờ';
