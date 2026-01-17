USE TRUNGTAMTHETHAO
GO

-- 1. Create DANHGIA Table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DANHGIA]') AND type in (N'U'))
BEGIN
    CREATE TABLE DANHGIA (
        MaDanhGia BIGINT IDENTITY(1,1) PRIMARY KEY,
        MaDatSan BIGINT NOT NULL,
        Diem INT CHECK (Diem >= 1 AND Diem <= 5),
        NoiDung NVARCHAR(500),
        NgayDanhGia DATETIME DEFAULT GETDATE(),
        FOREIGN KEY (MaDatSan) REFERENCES PHIEUDATSAN(MaDatSan)
    );
    PRINT 'Created DANHGIA table.';
END
GO

-- 2. SP Cancel Booking
CREATE OR ALTER PROCEDURE sp_KhachHang_HuySan
    @MaDatSan BIGINT
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @TienPhat DECIMAL(18,2);
    
    -- Calculate penalty using existing function
    SET @TienPhat = dbo.f_TinhTienPhat(@MaDatSan, GETDATE());

    -- Update status
    UPDATE PHIEUDATSAN
    SET TrangThai = N'Đã hủy'
    WHERE MaDatSan = @MaDatSan;

    -- Return penalty amount
    SELECT @TienPhat AS TienPhat;
END
GO

-- 3. SP Review Booking
CREATE OR ALTER PROCEDURE sp_KhachHang_DanhGia
    @MaDatSan BIGINT,
    @Diem INT,
    @NoiDung NVARCHAR(500)
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO DANHGIA (MaDatSan, Diem, NoiDung)
    VALUES (@MaDatSan, @Diem, @NoiDung);
END
GO

-- 4. SP Change Booking (Simplified: Update Time if valid)
CREATE OR ALTER PROCEDURE sp_KhachHang_DoiSan
    @MaDatSan BIGINT,
    @NgayMoi DATE,
    @GioBatDauMoi TIME,
    @GioKetThucMoi TIME
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @MaSan VARCHAR(20);
    SELECT @MaSan = D.MaSan 
    FROM PHIEUDATSAN P JOIN DATSAN D ON P.MaDatSan = D.MaDatSan 
    WHERE P.MaDatSan = @MaDatSan;

    -- Check if new time is available
    IF dbo.f_KiemTraSanTrong(@MaSan, @NgayMoi, @GioBatDauMoi, @GioKetThucMoi) = 0
    BEGIN
        SELECT 0 AS Result, N'Sân đã bị đặt trong khung giờ này.' AS Message;
        RETURN;
    END

    -- Update
    UPDATE PHIEUDATSAN
    SET NgayDat = @NgayMoi, GioBatDau = @GioBatDauMoi, GioKetThuc = @GioKetThucMoi
    WHERE MaDatSan = @MaDatSan;

    SELECT 1 AS Result, N'Đổi giờ thành công.' AS Message;
END
GO
