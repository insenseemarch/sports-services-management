USE master
GO

USE TRUNGTAMTHETHAO
GO

-- ===================================================================================
-- ==																				==
-- ==									FUNCTION 									==
-- ==																				==
-- ===================================================================================

-- 1. Hàm tính tiền sân (Dựa vào khung giờ và loại sân)
-- Input: Mã Phiếu Đặt
-- Output: Tiền sân 
CREATE OR ALTER FUNCTION f_TinhTienSan (@MaDatSan BIGINT)
RETURNS DECIMAL(18,2)
AS
BEGIN
    DECLARE @TienSan DECIMAL(18,2) = 0;
    DECLARE @MaLS VARCHAR(20);
    DECLARE @TenLS NVARCHAR(50);
    DECLARE @GioBatDau TIME;
    DECLARE @GioKetThuc TIME;
    DECLARE @GiaApDung DECIMAL(18,2);

    -- Lấy thông tin phiếu đặt và loại sân
    SELECT @MaLS = S.MaLS, @TenLS = LS.TenLS, @GioBatDau = P.GioBatDau, @GioKetThuc = P.GioKetThuc
    FROM PHIEUDATSAN P
    JOIN DATSAN D ON P.MaDatSan = D.MaDatSan
    JOIN SAN S ON D.MaSan = S.MaSan
    JOIN LOAISAN LS ON S.MaLS = LS.MaLS
    WHERE P.MaDatSan = @MaDatSan;

    -- Lấy giá theo khung giờ
    SELECT TOP 1 @GiaApDung = K.GiaApDung
    FROM KHUNGGIO K
    WHERE K.MaLS = @MaLS 
      AND @GioBatDau >= K.GioBatDau 
      AND @GioBatDau < K.GioKetThuc
      AND K.NgayApDung <= (SELECT NgayDat FROM PHIEUDATSAN WHERE MaDatSan = @MaDatSan)
    ORDER BY K.NgayApDung DESC;

    -- TÍNH TIỀN THEO QUY TẮC
    IF @TenLS IN (N'Bóng đá mini', N'Sân Tennis')
    BEGIN
        -- Tính theo trận (90p) hoặc ca (2h) -> GIÁ TRỌN GÓI (Fixed Price)
        SET @TienSan = ISNULL(@GiaApDung, 0);
    END
    ELSE
    BEGIN
        -- Tính theo giờ (Cầu lông, Bóng rổ...) -> GIÁ x SỐ GIỜ
        SET @TienSan = ISNULL(@GiaApDung, 0) * (DATEDIFF(MINUTE, @GioBatDau, @GioKetThuc) / 60.0);
    END

    RETURN @TienSan;
END
GO

-- 2. Hàm tính tổng tiền dịch vụ của một phiếu đặt
-- Input: Mã Phiếu Đặt
-- Output: Tổng tiền các dịch vụ đã gọi
CREATE OR ALTER FUNCTION f_TinhTienDichVu (@MaDatSan BIGINT)
RETURNS DECIMAL(18,2)
AS
BEGIN
    DECLARE @TongTienDV DECIMAL(18,2) = 0;
    
    SELECT @TongTienDV = SUM(ThanhTien)
    FROM CT_DICHVUDAT
    WHERE MaDatSan = @MaDatSan;
    
    RETURN ISNULL(@TongTienDV, 0);
END
GO

-- 3. Hàm kiểm tra sân trống
-- Input: Mã Sân, Ngày, Giờ BĐ, Giờ KT
-- Output: 1 (Trống), 0 (Bận)
CREATE OR ALTER FUNCTION f_KiemTraSanTrong 
(
    @MaSan VARCHAR(20), 
    @NgayDat DATE, 
    @GioBD TIME, 
    @GioKT TIME,
    @MaDatSanExclude BIGINT = NULL -- Tham số tùy chọn: Mã phiếu cần bỏ qua (để đổi lịch)
)
RETURNS BIT
AS
BEGIN
    DECLARE @KetQua BIT = 1; -- Mặc định là trống (1)

    IF EXISTS (
        SELECT 1
        FROM PHIEUDATSAN P
        JOIN DATSAN D ON P.MaDatSan = D.MaDatSan
        WHERE D.MaSan = @MaSan
          AND P.NgayDat = @NgayDat
          AND P.TrangThai <> N'Đã hủy' AND P.TrangThai <> N'Nháp'
          AND (@MaDatSanExclude IS NULL OR P.MaDatSan <> @MaDatSanExclude) -- Bỏ qua chính nó
          AND (
              (@GioBD >= P.GioBatDau AND @GioBD < P.GioKetThuc) OR 
              (@GioKT > P.GioBatDau AND @GioKT <= P.GioKetThuc) OR 
              (P.GioBatDau >= @GioBD AND P.GioBatDau < @GioKT)      
          )
    )
    BEGIN
        SET @KetQua = 0; -- Đã bị trùng (0)
    END

    RETURN @KetQua;
END
GO

GO

-- 11. Đổi lịch đặt sân (Reschedule)
CREATE OR ALTER PROCEDURE sp_DoiLichDat
    @MaDatSan BIGINT,
    @MaKH VARCHAR(20),
    @NgayMoi DATE,
    @GioBDMoi TIME,
    @GioKTMoi TIME
AS
BEGIN
    SET NOCOUNT ON;
    SET TRAN ISOLATION LEVEL REPEATABLE READ;

    BEGIN TRY
        BEGIN TRAN;

        -- 1. Kiểm tra tồn tại và quyền sở hữu
        DECLARE @MaSan VARCHAR(20), @NgayCu DATE, @GioBDCu TIME, @TrangThai NVARCHAR(50);
        
        SELECT @MaSan = D.MaSan, @NgayCu = P.NgayDat, @GioBDCu = P.GioBatDau, @TrangThai = P.TrangThai
        FROM PHIEUDATSAN P
        JOIN DATSAN D ON P.MaDatSan = D.MaDatSan
        WHERE P.MaDatSan = @MaDatSan AND P.MaKH = @MaKH;

        IF @MaSan IS NULL
        BEGIN
            ROLLBACK TRAN;
            RAISERROR(N'Không tìm thấy phiếu đặt hoặc bạn không có quyền thay đổi!', 16, 1);
            RETURN;
        END

        -- 2. Kiểm tra trạng thái (Chỉ được đổi khi chưa hoàn thành/hủy)
        IF @TrangThai IN (N'Đã hủy', N'Hoàn thành', N'No-Show')
        BEGIN
            ROLLBACK TRAN;
            RAISERROR(N'Không thể đổi lịch cho đơn đã hủy hoặc đã hoàn thành!', 16, 1);
            RETURN;
        END
        
        -- 3. Kiểm tra hạn đổi (Ví dụ: Phải trước giờ đá cũ 2 tiếng) - Tạm thời disable hoặc set 0
        -- IF DATEDIFF(HOUR, GETDATE(), CAST(@NgayCu AS DATETIME) + CAST(@GioBDCu AS DATETIME)) < 2 ...

        -- 4. Kiểm tra Sân Trống cho Giờ Mới (Trừ chính đơn này ra)
        -- Lưu ý: f_KiemTraSanTrong đã sửa để nhận tham số @MaDatSanExclude
        IF dbo.f_KiemTraSanTrong(@MaSan, @NgayMoi, @GioBDMoi, @GioKTMoi, @MaDatSan) = 0
        BEGIN
            ROLLBACK TRAN;
            RAISERROR(N'Khung giờ mới đã có người đặt! Vui lòng chọn giờ khác.', 16, 1);
            RETURN;
        END

        -- 5. Cập nhật
        UPDATE PHIEUDATSAN 
        SET NgayDat = @NgayMoi, GioBatDau = @GioBDMoi, GioKetThuc = @GioKTMoi, NgayTao = GETDATE()
        WHERE MaDatSan = @MaDatSan;

        COMMIT TRAN;
        PRINT N'Đổi lịch thành công!';
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRAN;
        DECLARE @Msg NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(@Msg, 16, 1);
    END CATCH
END;
GO

-- 4. Hàm kiểm tra đăng nhập
-- Input: Username, Password
-- Output: VaiTro (Nếu đúng), NULL (Nếu sai)
CREATE OR ALTER FUNCTION f_KiemTraDangNhap 
(
    @TenDangNhap VARCHAR(50), 
    @MatKhau VARCHAR(255)
)
RETURNS NVARCHAR(50)
AS
BEGIN
    DECLARE @VaiTro NVARCHAR(50);
    
    SELECT @VaiTro = VaiTro
    FROM TAIKHOAN
    WHERE TenDangNhap = @TenDangNhap AND MatKhau = @MatKhau;

    RETURN @VaiTro;
END
GO

-- 5. Hàm tính DOANH THU (Cộng dồn toàn hệ thống nếu @MaCS NULL)
CREATE OR ALTER FUNCTION f_BC_TinhDoanhThu
(
    @TuNgay DATE,
    @DenNgay DATE,
    @MaCS VARCHAR(20) 
)
RETURNS DECIMAL(18,2)
AS
BEGIN
    DECLARE @DoanhThu DECIMAL(18,2) = 0;

    SELECT @DoanhThu = SUM(HD.ThanhTien)
    FROM HOADON HD
    JOIN NHANVIEN NV ON HD.NguoiLap = NV.MaNV
    WHERE HD.NgayLap BETWEEN @TuNgay AND @DenNgay
      AND (@MaCS IS NULL OR NV.MaCS = @MaCS)
      AND HD.ThanhTien IS NOT NULL; 

    RETURN ISNULL(@DoanhThu, 0);
END
GO

-- 6. Hàm tính TỔNG GIỜ LÀM VIỆC (Cộng dồn toàn hệ thống nếu @MaCS NULL)
CREATE OR ALTER FUNCTION f_BC_TinhGioLamViec
(
    @TuNgay DATE,
    @DenNgay DATE,
    @MaCS VARCHAR(20)
)
RETURNS DECIMAL(18,2)
AS
BEGIN
    DECLARE @TongGio DECIMAL(18,2) = 0;

    SELECT @TongGio = SUM(DATEDIFF(MINUTE, C.GioBatDau, C.GioKetThuc)) / 60.0
    FROM THAMGIACATRUC T
    JOIN CATRUC C ON T.MaCaTruc = C.MaCaTruc
    JOIN NHANVIEN NV ON T.MaNV = NV.MaNV
    WHERE C.NgayTruc BETWEEN @TuNgay AND @DenNgay
      AND (@MaCS IS NULL OR NV.MaCS = @MaCS); -- NULL thì lấy hết nhân viên toàn hệ thống

    RETURN ISNULL(@TongGio, 0);
END
GO

--7. Hàm tính TỶ LỆ DÙNG SÂN (Tính trên tổng số sân toàn hệ thống nếu @MaCS NULL)
CREATE OR ALTER FUNCTION f_BC_TinhTyLeDungSan
(
    @TuNgay DATE,
    @DenNgay DATE,
    @MaCS VARCHAR(20)
)
RETURNS DECIMAL(5,2)
AS
BEGIN
    DECLARE @TyLe DECIMAL(5,2) = 0;
    DECLARE @TongGioThucTe DECIMAL(18,2);
    DECLARE @TongGioKhaDung DECIMAL(18,2);
    DECLARE @SoSan INT;
    DECLARE @SoGioMoCuaMotNgay INT = 16; -- (Mặc định mở từ 6h - 22h) 

    -- 1. Đếm số sân
    SELECT @SoSan = COUNT(*) FROM SAN WHERE (@MaCS IS NULL OR MaCS = @MaCS);

    -- 2. Tính tổng giờ (CHỈ TÍNH PHIẾU KHÔNG BỊ HỦY)
    SELECT @TongGioThucTe = SUM(DATEDIFF(MINUTE, P.GioBatDau, P.GioKetThuc)) / 60.0
    FROM PHIEUDATSAN P
    JOIN DATSAN D ON P.MaDatSan = D.MaDatSan
    JOIN SAN S ON D.MaSan = S.MaSan
    WHERE P.NgayDat BETWEEN @TuNgay AND @DenNgay
      AND (@MaCS IS NULL OR S.MaCS = @MaCS)
      AND P.TrangThai <> N'Đã hủy'; 

    -- 3. Tính Năng lực phục vụ
    DECLARE @SoNgay INT = DATEDIFF(DAY, @TuNgay, @DenNgay) + 1;
    SET @TongGioKhaDung = @SoSan * @SoGioMoCuaMotNgay * @SoNgay;

    IF @TongGioKhaDung > 0
        SET @TyLe = (@TongGioThucTe / @TongGioKhaDung) * 100;

    RETURN ISNULL(@TyLe, 0);
END
GO

--8. Hàm tính TIỀN MẤT DO HỦY
CREATE OR ALTER FUNCTION f_BC_TinhTienMatDoHuy
(
    @TuNgay DATE,
    @DenNgay DATE,
    @MaCS VARCHAR(20)
)
RETURNS DECIMAL(18,2)
AS
BEGIN
    DECLARE @TienMat DECIMAL(18,2) = 0;

    SELECT @TienMat = SUM(
        -- Tiền mất = (Tiền sân lẽ ra thu được) - (Tiền phạt thực tế thu được trong Hóa đơn)
        dbo.f_TinhTienSan(P.MaDatSan) - ISNULL(H.ThanhTien, 0)
    ) 
    FROM PHIEUDATSAN P
    JOIN DATSAN D ON P.MaDatSan = D.MaDatSan
    JOIN SAN S ON D.MaSan = S.MaSan
    -- LEFT JOIN với Hóa đơn để xem phiếu hủy này có bị phạt tiền không
    LEFT JOIN HOADON H ON P.MaDatSan = H.MaPhieu 
    WHERE P.NgayDat BETWEEN @TuNgay AND @DenNgay
      AND (@MaCS IS NULL OR S.MaCS = @MaCS)
      AND P.TrangThai = N'Đã hủy'; -- Chỉ xét các phiếu đã hủy

    RETURN ISNULL(@TienMat, 0);
END
GO


-- 9. Hàm tính số lượng (tình hình) No-show
CREATE OR ALTER FUNCTION f_BC_TinhNoShow
(
    @TuNgay DATE,
    @DenNgay DATE,
    @MaCS VARCHAR(20)
)
RETURNS INT
AS
BEGIN
    DECLARE @SoLuongNoShow INT = 0;

    SELECT @SoLuongNoShow = COUNT(DISTINCT P.MaDatSan)
    FROM PHIEUDATSAN P
    JOIN DATSAN D ON P.MaDatSan = D.MaDatSan
    JOIN SAN S ON D.MaSan = S.MaSan
    WHERE P.NgayDat BETWEEN @TuNgay AND @DenNgay
      AND (@MaCS IS NULL OR S.MaCS = @MaCS)
      
      AND (
          -- 1. Đã được xác nhận là No-Show
          P.TrangThai = N'No-Show' 
          
          OR 
          
          -- 2. Vẫn treo ở trạng thái 'Đã đặt' nhưng đã quá ngày
          (P.TrangThai = N'Đã đặt' AND P.NgayDat < CAST(GETDATE() AS DATE))
      );

    RETURN ISNULL(@SoLuongNoShow, 0);
END
GO

--10. Hàm tính tiền phạt hủy sân
CREATE OR ALTER FUNCTION f_TinhTienPhat
(
    @MaDatSan BIGINT,
    @ThoiDiemHuy DATETIME -- Thời điểm khách báo hủy
)
RETURNS DECIMAL(18,2)
AS
BEGIN
    DECLARE @TienPhat DECIMAL(18,2) = 0;
    DECLARE @NgayDa DATE;
    DECLARE @GioDa TIME;
    DECLARE @ThoiDiemDa DATETIME;
    DECLARE @TongTienSan DECIMAL(18,2);

    -- Lấy thông tin phiếu đặt
    SELECT @NgayDa = P.NgayDat, @GioDa = P.GioBatDau 
    FROM PHIEUDATSAN P WHERE MaDatSan = @MaDatSan;

    -- Ghép Ngày + Giờ để ra thời điểm đá
    SET @ThoiDiemDa = CAST(@NgayDa AS DATETIME) + CAST(@GioDa AS DATETIME);

    -- Tính tổng tiền sân (gọi lại hàm cũ)
    SET @TongTienSan = dbo.f_TinhTienSan(@MaDatSan);

    -- Tính số giờ chênh lệch
    DECLARE @GioChenhLech INT = DATEDIFF(HOUR, @ThoiDiemHuy, @ThoiDiemDa);

    IF @GioChenhLech >= 24
        SET @TienPhat = @TongTienSan * 0.1; -- Phạt 10%
    ELSE IF @GioChenhLech >= 0
        SET @TienPhat = @TongTienSan * 0.5; -- Phạt 50%
    ELSE 
        SET @TienPhat = @TongTienSan; -- No-show hoặc hủy sau khi đã bắt đầu (coi như 100%)

    RETURN @TienPhat;
END
GO

-- ===================================================================================
-- ==																				==
-- ==					  STORE PROCEDURE NGHIỆP VỤ									==
-- ==																				==
-- ===================================================================================

--1. Tạo báo cáo thống kê
CREATE OR ALTER PROCEDURE sp_TaoBaoCaoThongKe
    @NguoiLapPhieu VARCHAR(20),
    @MaCS VARCHAR(20) = NULL,
    @Thang INT,
    @Nam INT
AS
BEGIN
    SET NOCOUNT ON;

    -- 1. THIẾT LẬP THỜI GIAN
    DECLARE @NgayDauThang DATE = DATEFROMPARTS(@Nam, @Thang, 1);
    DECLARE @NgayCuoiThang DATE = EOMONTH(@NgayDauThang);
    DECLARE @Quy INT = (@Thang - 1) / 3 + 1;
    DECLARE @NgayDauQuy DATE = DATEFROMPARTS(@Nam, (@Quy - 1) * 3 + 1, 1);
    DECLARE @NgayCuoiQuy DATE = EOMONTH(DATEADD(MONTH, 2, @NgayDauQuy));
    DECLARE @NgayDauNam DATE = DATEFROMPARTS(@Nam, 1, 1);
    DECLARE @NgayCuoiNam DATE = DATEFROMPARTS(@Nam, 12, 31);

    -- 2. TÍNH TOÁN CÁC CHỈ SỐ
    DECLARE @TyLeDungSan DECIMAL(5,2);
    DECLARE @SoLuongDatOnline INT;
    DECLARE @SoLuongDatTrucTiep INT;
    DECLARE @TinhHinhHuySan INT;
    DECLARE @TinhHinhNoShow INT;
    DECLARE @SoTienBiMatDoHuy DECIMAL(18,2);
    DECLARE @SoLuongSuDungDichVu INT;

    -- Gọi các hàm tính toán 
    SET @TyLeDungSan = dbo.f_BC_TinhTyLeDungSan(@NgayDauThang, @NgayCuoiThang, @MaCS);
    SET @SoTienBiMatDoHuy = dbo.f_BC_TinhTienMatDoHuy(@NgayDauThang, @NgayCuoiThang, @MaCS);
    SET @TinhHinhNoShow = dbo.f_BC_TinhNoShow(@NgayDauThang, @NgayCuoiThang, @MaCS);

    -- Đếm Online/Trực tiếp (CHỈ ĐẾM PHIẾU KHÔNG HỦY)
    SELECT 
        @SoLuongDatOnline = COUNT(CASE WHEN P.KenhDat = N'Online' THEN 1 END),
        @SoLuongDatTrucTiep = COUNT(CASE WHEN P.KenhDat != N'Online' THEN 1 END)
    FROM PHIEUDATSAN P
    JOIN DATSAN D ON P.MaDatSan = D.MaDatSan JOIN SAN S ON D.MaSan = S.MaSan
    WHERE P.NgayDat BETWEEN @NgayDauThang AND @NgayCuoiThang
      AND (@MaCS IS NULL OR S.MaCS = @MaCS)
      AND P.TrangThai <> N'Đã hủy'; -- <== Lọc bỏ phiếu hủy

    -- TÍNH SỐ LƯỢNG HỦY SÂN
    SELECT @TinhHinhHuySan = COUNT(*)
    FROM PHIEUDATSAN P
    JOIN DATSAN D ON P.MaDatSan = D.MaDatSan JOIN SAN S ON D.MaSan = S.MaSan
    WHERE P.NgayDat BETWEEN @NgayDauThang AND @NgayCuoiThang
      AND (@MaCS IS NULL OR S.MaCS = @MaCS)
      AND P.TrangThai = N'Đã hủy'; -- <== Đếm phiếu hủy

    -- Đếm Dịch Vụ (Chỉ tính trên phiếu chưa hủy)
    SELECT @SoLuongSuDungDichVu = ISNULL(SUM(CT.SoLuong), 0)
    FROM CT_DICHVUDAT CT
    JOIN PHIEUDATSAN P ON CT.MaDatSan = P.MaDatSan
    JOIN DATSAN D ON P.MaDatSan = D.MaDatSan JOIN SAN S ON D.MaSan = S.MaSan
    WHERE P.NgayDat BETWEEN @NgayDauThang AND @NgayCuoiThang
      AND (@MaCS IS NULL OR S.MaCS = @MaCS)
      AND P.TrangThai <> N'Đã hủy'; 

    -- 3. INSERT VÀO BẢNG CHA
    INSERT INTO BAOCAOTHONGKE 
    (NguoiLapPhieu, NgayLap, MaCS, TyLeDungSan, SoLuongDatOnline, SoLuongDatTrucTiep, 
     TinhHinhHuySan, TinhHinhNoShow, SoTienBiMatDoHuy, SoLuongSuDungDichVu)
    VALUES 
    (@NguoiLapPhieu, GETDATE(), @MaCS, @TyLeDungSan, @SoLuongDatOnline, @SoLuongDatTrucTiep, 
     @TinhHinhHuySan, @TinhHinhNoShow, @SoTienBiMatDoHuy, @SoLuongSuDungDichVu);

    DECLARE @MaBaoCaoMoi BIGINT = SCOPE_IDENTITY();

    -- 4. INSERT CÁC BẢNG CON 
    INSERT INTO DOANHTHU (MaBaoCao, LoaiDoanhThu, Thang, Quy, Nam, TongDoanhThu) VALUES (@MaBaoCaoMoi, N'Tháng', @Thang, NULL, @Nam, dbo.f_BC_TinhDoanhThu(@NgayDauThang, @NgayCuoiThang, @MaCS));
    INSERT INTO DOANHTHU (MaBaoCao, LoaiDoanhThu, Thang, Quy, Nam, TongDoanhThu) VALUES (@MaBaoCaoMoi, N'Quý', NULL, @Quy, @Nam, dbo.f_BC_TinhDoanhThu(@NgayDauQuy, @NgayCuoiQuy, @MaCS));
    INSERT INTO DOANHTHU (MaBaoCao, LoaiDoanhThu, Thang, Quy, Nam, TongDoanhThu) VALUES (@MaBaoCaoMoi, N'Năm', NULL, NULL, @Nam, dbo.f_BC_TinhDoanhThu(@NgayDauNam, @NgayCuoiNam, @MaCS));

    INSERT INTO TONGGIOLAMVIECNV (MaBaoCao, PhanLoai, Thang, Quy, Nam, TongGio) VALUES (@MaBaoCaoMoi, N'Tháng', @Thang, NULL, @Nam, dbo.f_BC_TinhGioLamViec(@NgayDauThang, @NgayCuoiThang, @MaCS));
    INSERT INTO TONGGIOLAMVIECNV (MaBaoCao, PhanLoai, Thang, Quy, Nam, TongGio) VALUES (@MaBaoCaoMoi, N'Quý', NULL, @Quy, @Nam, dbo.f_BC_TinhGioLamViec(@NgayDauQuy, @NgayCuoiQuy, @MaCS));
    INSERT INTO TONGGIOLAMVIECNV (MaBaoCao, PhanLoai, Thang, Quy, Nam, TongGio) VALUES (@MaBaoCaoMoi, N'Năm', NULL, NULL, @Nam, dbo.f_BC_TinhGioLamViec(@NgayDauNam, @NgayCuoiNam, @MaCS));

    PRINT N'Đã tạo báo cáo thành công (Mã BC: ' + CAST(@MaBaoCaoMoi AS VARCHAR) + ')';
END
GO


-- ===================================================================================
-- ==                                                                               ==
-- ==					CÁC STORED PROCEDURE XỬ LÝ TRANH CHẤP                       ==
-- ==																                ==
-- ==                                                                               ==
-- ===================================================================================

-- =============================================================
-- NHÓM 1: QUẢN LÝ NHÂN SỰ & CA TRỰC (Tình huống 1, 2, 4, 15)
-- =============================================================

-- Tình huống 1: Unrepeatable Read (Xem lương vs Sửa lương)
-- [T1]: Xem thông tin nhân viên (Giải pháp: REPEATABLE READ)
CREATE OR ALTER PROCEDURE sp_XemThongTinNhanVien
    @MaNV VARCHAR(20)
AS
BEGIN
    SET TRAN ISOLATION LEVEL REPEATABLE READ
    BEGIN TRAN
        SELECT * FROM NHANVIEN WHERE MaNV = @MaNV;
        WAITFOR DELAY '00:00:05'; -- Giữ khóa để chờ T2
        SELECT * FROM NHANVIEN WHERE MaNV = @MaNV;
    COMMIT TRAN
END
GO

-- [T2]: Sửa thông tin nhân viên (Gây lỗi cho T1 nếu không chặn)
CREATE OR ALTER PROCEDURE sp_SuaThongTinNhanVien
    @MaNV VARCHAR(20),
    @LuongMoi DECIMAL(18,2)
AS
BEGIN
    BEGIN TRAN
        UPDATE NHANVIEN SET LuongCoBan = @LuongMoi WHERE MaNV = @MaNV;
    COMMIT TRAN
END
GO

-- Tình huống 2: Phantom Read (Xem danh sách vs Xóa nhân viên)
-- [T1]: Xem danh sách nhân viên (Giải pháp: SERIALIZABLE)
CREATE OR ALTER PROCEDURE sp_XemDanhSachNhanVien
AS
BEGIN
    SET TRAN ISOLATION LEVEL SERIALIZABLE
    BEGIN TRAN
        SELECT * FROM NHANVIEN;
        WAITFOR DELAY '00:00:05';
        SELECT * FROM NHANVIEN; 
    COMMIT TRAN
END
GO

-- [T2]: Xóa nhân viên (Gây lỗi Phantom cho T1)
CREATE OR ALTER PROCEDURE sp_XoaNhanVien
    @MaNV VARCHAR(20)
AS
BEGIN
    BEGIN TRAN
        DELETE FROM NHANVIEN WHERE MaNV = @MaNV;
    COMMIT TRAN
END
GO

-- Tình huống 4: Lost Update (Cập nhật lương - 2 Quản lý cùng sửa)
-- [T1] & [T2]: Cùng chạy thủ tục này (Giải pháp: REPEATABLE READ gây Deadlock để 1 người retry)
CREATE OR ALTER PROCEDURE sp_CapNhatLuongNV_AnToan
    @MaNV VARCHAR(20),
    @LuongMoi DECIMAL(18,2)
AS
BEGIN
    SET NOCOUNT ON;
    SET TRAN ISOLATION LEVEL REPEATABLE READ; 
    
    BEGIN TRY
        BEGIN TRAN;
        
        DECLARE @LuongCu DECIMAL(18,2);
        SELECT @LuongCu = LuongCoBan FROM NHANVIEN WHERE MaNV = @MaNV;

        WAITFOR DELAY '00:00:05'; -- Giả lập thời gian nhập liệu

        UPDATE NHANVIEN SET LuongCoBan = @LuongMoi WHERE MaNV = @MaNV;
        
        COMMIT TRAN;
        PRINT N'Cập nhật lương thành công.';
    END TRY
    BEGIN CATCH
        ROLLBACK TRAN;
        DECLARE @Msg NVARCHAR(4000) = ERROR_MESSAGE();
        PRINT N'Lỗi cập nhật (Deadlock detected - Lost Update prevented): ' + @Msg;
    END CATCH
END
GO

-- Tình huống 15: Phantom Read (Đếm ca trực vs Phân công mới)
-- [T1]: Đếm ca làm việc (Giải pháp: SERIALIZABLE)
CREATE OR ALTER PROCEDURE sp_DemCaLamViec
    @MaNV VARCHAR(20)
AS
BEGIN
    SET TRAN ISOLATION LEVEL SERIALIZABLE
    BEGIN TRAN
        SELECT COUNT(*) FROM THAMGIACATRUC WHERE MaNV = @MaNV;
        WAITFOR DELAY '00:00:05';
        SELECT COUNT(*) FROM THAMGIACATRUC WHERE MaNV = @MaNV;
    COMMIT TRAN
END
GO

-- [T2]: Phân công ca trực mới (Gây lỗi Phantom)
CREATE OR ALTER PROCEDURE sp_PhanCongCaTruc
    @MaCa BIGINT,
    @MaNV VARCHAR(20)
AS
BEGIN
    BEGIN TRAN
        INSERT INTO THAMGIACATRUC (MaCaTruc, MaNV) VALUES (@MaCa, @MaNV);
    COMMIT TRAN
END
GO

-- =============================================================
-- NHÓM 2: QUẢN LÝ SÂN	 (Tình huống 3, 5, 7, 8)
-- =============================================================

-- Tình huống 3: Dirty Read (Tìm sân vs Cập nhật chưa commit)
-- [T2]: Tìm kiếm sân (Giải pháp: READ COMMITTED - Mặc định)
CREATE OR ALTER PROCEDURE sp_TimKiemSan
    @MaCS VARCHAR(20),
    @MaLS VARCHAR(20)
AS
BEGIN
    SET TRAN ISOLATION LEVEL READ COMMITTED
    BEGIN TRAN
        SELECT * FROM SAN 
        WHERE MaCS = @MaCS AND MaLS = @MaLS AND TinhTrang = N'Còn Trống';
    COMMIT TRAN
END
GO

-- [T1]: Cập nhật trạng thái sân rồi Rollback (Gây lỗi Dirty Read cho T2)
CREATE OR ALTER PROCEDURE sp_CapNhatTrangThaiSan_Rollback
    @MaSan VARCHAR(20)
AS
BEGIN
    BEGIN TRAN
        UPDATE SAN SET TinhTrang = N'Bảo trì' WHERE MaSan = @MaSan;
        WAITFOR DELAY '00:00:10'; -- Giữ khóa X
    ROLLBACK TRAN -- Hủy bỏ
END
GO

-- Tình huống 5: Lost Update (Cập nhật trạng thái sân)
-- [T1] & [T2]: Cùng chạy thủ tục này (Giải pháp: REPEATABLE READ)
CREATE OR ALTER PROCEDURE sp_CapNhatTrangThaiSan_AnToan
    @MaSan VARCHAR(20),
    @TrangThaiMoi NVARCHAR(50)
AS
BEGIN
    SET TRAN ISOLATION LEVEL REPEATABLE READ;
    BEGIN TRY
        BEGIN TRAN
            DECLARE @TrangThaiCu NVARCHAR(50);
            SELECT @TrangThaiCu = TinhTrang FROM SAN WHERE MaSan = @MaSan;

            WAITFOR DELAY '00:00:05';

            UPDATE SAN SET TinhTrang = @TrangThaiMoi WHERE MaSan = @MaSan;
        COMMIT TRAN
    END TRY
    BEGIN CATCH
        ROLLBACK TRAN;
        PRINT N'Lỗi cập nhật trạng thái (Deadlock detected).';
    END CATCH
END
GO

-- Tình huống 7: Unrepeatable Read (Kiểm tra sân vs Đặt sân)
-- [T1]: Lễ tân kiểm tra sân (Giải pháp: REPEATABLE READ)
CREATE OR ALTER PROCEDURE sp_KiemTraTinhTrangSan
    @MaSan VARCHAR(20)
AS
BEGIN
    SET TRAN ISOLATION LEVEL REPEATABLE READ
    BEGIN TRAN
        SELECT TinhTrang FROM SAN WHERE MaSan = @MaSan;
        WAITFOR DELAY '00:00:05';
        SELECT TinhTrang FROM SAN WHERE MaSan = @MaSan;
    COMMIT TRAN
END
GO

-- [T2]: Khách đặt sân (Code ở phần 3 - sp_DatSan)

-- Tình huống 8: Phantom Read (Xem DS sân trống vs Thêm sân mới)
-- [T1]: Xem danh sách sân trống (Giải pháp: SERIALIZABLE)
CREATE OR ALTER PROCEDURE sp_XemDSSanTrong
    @MaLS VARCHAR(20)
AS
BEGIN
    SET TRAN ISOLATION LEVEL SERIALIZABLE
    BEGIN TRAN
        SELECT * FROM SAN WHERE MaLS = @MaLS AND TinhTrang = N'Còn Trống';
        WAITFOR DELAY '00:00:05';
        SELECT * FROM SAN WHERE MaLS = @MaLS AND TinhTrang = N'Còn Trống';
    COMMIT TRAN
END
GO

-- [T2]: Thêm sân trống mới
CREATE OR ALTER PROCEDURE sp_ThemSanTrongMoi
    @MaSan VARCHAR(20)
AS
BEGIN
    BEGIN TRAN
        UPDATE SAN SET TinhTrang = N'Còn Trống' WHERE MaSan = @MaSan;
    COMMIT TRAN
END
GO

-- =============================================================
-- NHÓM 3: GIÁ & KHUYẾN MÃI (Tình huống 6, 9, 14, 16, 18)
-- =============================================================

-- Tình huống 6 & 14: Dirty/Unrepeatable Read (Xem giá vs Sửa giá)
-- [T1] (Scenario 14) / [T2] (Scenario 6): Xem giá sân (Giải pháp: REPEATABLE READ)
CREATE OR ALTER PROCEDURE sp_XemGiaSan
    @MaLS VARCHAR(20),
    @MaKG VARCHAR(20)
AS
BEGIN
    SET TRAN ISOLATION LEVEL REPEATABLE READ
    BEGIN TRAN
        SELECT GiaApDung FROM KHUNGGIO WHERE MaLS = @MaLS AND MaKG = @MaKG;
        WAITFOR DELAY '00:00:05';
        SELECT GiaApDung FROM KHUNGGIO WHERE MaLS = @MaLS AND MaKG = @MaKG;
    COMMIT TRAN
END
GO

-- [T1] (Scenario 6) / [T2] (Scenario 14): Thay đổi giá thuê
CREATE OR ALTER PROCEDURE sp_ThayDoiGiaThueSan
    @MaLS VARCHAR(20),
    @MaKG VARCHAR(20),
    @GiaMoi DECIMAL(18,2)
AS
BEGIN
    BEGIN TRAN
        UPDATE KHUNGGIO SET GiaApDung = @GiaMoi WHERE MaLS = @MaLS AND MaKG = @MaKG;
    COMMIT TRAN
END
GO

-- Tình huống 9: Lost Update (Cập nhật Voucher)
-- [T1] & [T2]: Cùng chạy thủ tục này (Giải pháp: SERIALIZABLE)
CREATE OR ALTER PROCEDURE sp_CapNhatTyLeGiamGia_AnToan
    @MaUD VARCHAR(20),
    @TyLeMoi DECIMAL(5,2)
AS
BEGIN
    SET TRAN ISOLATION LEVEL SERIALIZABLE;
    BEGIN TRY
        BEGIN TRAN
            DECLARE @TyLeCu DECIMAL(5,2);
            SELECT @TyLeCu = TyLeGiamGia FROM UUDAI WHERE MaUD = @MaUD;

            WAITFOR DELAY '00:00:05';

            UPDATE UUDAI SET TyLeGiamGia = @TyLeMoi WHERE MaUD = @MaUD;
        COMMIT TRAN
    END TRY
    BEGIN CATCH
        ROLLBACK TRAN;
        PRINT N'Lỗi cập nhật Voucher.';
    END CATCH
END
GO

-- Tình huống 16: Dirty Read (Thanh toán áp dụng Voucher)
-- [T1]: Quản lý sửa voucher rồi rollback
CREATE OR ALTER PROCEDURE sp_CapNhatVoucher_Rollback
    @MaUD VARCHAR(20)
AS
BEGIN
    BEGIN TRAN
        UPDATE UUDAI SET TyLeGiamGia = 90 WHERE MaUD = @MaUD; -- Sửa thành 90%
        WAITFOR DELAY '00:00:10';
    ROLLBACK TRAN -- Hủy bỏ
END
GO

-- [T2]: Thu ngân thanh toán (Giải pháp: READ COMMITTED chặn đọc 90%)
CREATE OR ALTER PROCEDURE sp_ThanhToan_ApDungVoucher
    @MaDatSan BIGINT,
    @MaUD VARCHAR(20)
AS
BEGIN
    SET TRAN ISOLATION LEVEL READ COMMITTED 
    BEGIN TRAN
        DECLARE @TongTien DECIMAL(18,2);
        DECLARE @GiamGia DECIMAL(18,2) = 0;
        
        SELECT @TongTien = dbo.f_TinhTienSan(@MaDatSan) + dbo.f_TinhTienDichVu(@MaDatSan);

        DECLARE @TyLeGiam DECIMAL(5,2);
        SELECT @TyLeGiam = TyLeGiamGia FROM UUDAI WHERE MaUD = @MaUD;

        IF @TyLeGiam IS NOT NULL 
            SET @GiamGia = @TongTien * (@TyLeGiam / 100.0);

        PRINT N'Thanh toán thành công. Giảm giá: ' + CAST(@GiamGia AS VARCHAR);
    COMMIT TRAN
END
GO

-- Tình huống 18: Phantom Read (Xem Voucher Active vs Thêm/Sửa Voucher)
-- [T1]: Xem danh sách Voucher đang hoạt động (Giải pháp: SERIALIZABLE)
CREATE OR ALTER PROCEDURE sp_XemVoucherDangHoatDong
AS
BEGIN
    SET TRAN ISOLATION LEVEL SERIALIZABLE
    BEGIN TRAN
        SELECT * FROM UUDAI WHERE DieuKienApDung = N'Hoạt động';
        WAITFOR DELAY '00:00:05';
        SELECT * FROM UUDAI WHERE DieuKienApDung = N'Hoạt động';
    COMMIT TRAN
END
GO

-- [T2]: Quản lý thêm/sửa voucher
CREATE OR ALTER PROCEDURE sp_QL_ChinhSuaVoucher
    @MaUD_Moi VARCHAR(20),
    @MaUD_Cu VARCHAR(20)
AS
BEGIN
    BEGIN TRAN
        INSERT INTO UUDAI(MaUD, TenCT, TyLeGiamGia, DieuKienApDung) 
        VALUES (@MaUD_Moi, N'KM Mới', 10, N'Hoạt động');
        
        UPDATE UUDAI SET DieuKienApDung = N'Ngừng' WHERE MaUD = @MaUD_Cu;
    COMMIT TRAN
END
GO

-- =============================================================
-- NHÓM 4: THANH TOÁN & HÓA ĐƠN (Tình huống 10, 11, 12, 13, 17, 19)
-- =============================================================

-- Tình huống 10: Dirty Read (Thanh toán rollback vs Xuất HĐ)
-- [T1]: Thanh toán Online lỗi rollback
CREATE OR ALTER PROCEDURE sp_ThanhToanOnline_Rollback
    @MaDatSan BIGINT
AS
BEGIN
    BEGIN TRAN
        UPDATE CT_DICHVUDAT SET TrangThaiSuDung = N'Đã thanh toán' WHERE MaDatSan = @MaDatSan;
        WAITFOR DELAY '00:00:10';
    ROLLBACK TRAN
END
GO

-- [T2]: Xuất hóa đơn (Giải pháp: READ COMMITTED)
CREATE OR ALTER PROCEDURE sp_XuatHoaDon_AnToan
    @MaDatSan BIGINT
AS
BEGIN
    SET TRAN ISOLATION LEVEL READ COMMITTED
    BEGIN TRAN
        SELECT * FROM CT_DICHVUDAT 
        WHERE MaDatSan = @MaDatSan AND TrangThaiSuDung = N'Đã thanh toán';
    COMMIT TRAN
END
GO

-- Tình huống 11: Unrepeatable Read (Lập HĐ xem giá)
-- [T1]: Lập hóa đơn xem giá (Giải pháp: REPEATABLE READ)
CREATE OR ALTER PROCEDURE sp_LapHoaDon_XemGia
    @MaLS VARCHAR(20)
AS
BEGIN
    SET TRAN ISOLATION LEVEL REPEATABLE READ
    BEGIN TRAN
        SELECT GiaApDung FROM KHUNGGIO WHERE MaLS = @MaLS;
        WAITFOR DELAY '00:00:05';
        SELECT GiaApDung FROM KHUNGGIO WHERE MaLS = @MaLS;
    COMMIT TRAN
END
GO

-- [T2]: Thay đổi giá (Dùng sp_ThayDoiGiaThueSan ở trên)

-- Tình huống 12: Phantom Read (Báo cáo số lượng vs Đặt sân mới)
-- [T1]: Lập báo cáo số lượng đặt Online (Giải pháp: SERIALIZABLE)
CREATE OR ALTER PROCEDURE sp_LapBaoCao_DatOnline
    @NgayLap DATE
AS
BEGIN
    SET TRAN ISOLATION LEVEL SERIALIZABLE
    BEGIN TRAN
        SELECT COUNT(*) FROM PHIEUDATSAN WHERE NgayDat = @NgayLap AND KenhDat = N'Online';
        WAITFOR DELAY '00:00:05';
        SELECT COUNT(*) FROM PHIEUDATSAN WHERE NgayDat = @NgayLap AND KenhDat = N'Online';
    COMMIT TRAN
END
GO

-- [T2]: Đặt sân mới
CREATE OR ALTER PROCEDURE sp_DatSan_Moi
    @MaKH VARCHAR(20), @MaSan VARCHAR(20)
AS
BEGIN
    BEGIN TRAN
        INSERT INTO PHIEUDATSAN (MaKH, NgayDat, KenhDat, TrangThai, GioBatDau, GioKetThuc) 
        VALUES (@MaKH, GETDATE(), N'Online', N'Đã đặt', '08:00', '09:00');
        
        DECLARE @NewID BIGINT = SCOPE_IDENTITY();
        INSERT INTO DATSAN (MaDatSan, MaSan) VALUES (@NewID, @MaSan);
    COMMIT TRAN
END
GO

-- Tình huống 13: Dirty Read (Tính tiền vs Thêm dịch vụ rollback)
-- [T1 - GÂY LỖI]: NV Lễ tân thêm dịch vụ (Ví dụ: Thêm 5 chai nước) nhưng sau đó Rollback
CREATE OR ALTER PROCEDURE sp_ThemDichVu_Rollback
    @MaDatSan BIGINT,
    @MaDV VARCHAR(20)
AS
BEGIN
    BEGIN TRAN
        -- Giả sử thêm 5 món, đơn giá 10.000
        INSERT INTO CT_DICHVUDAT (MaDV, MaDatSan, SoLuong, ThanhTien, TrangThaiSuDung)
        VALUES (@MaDV, @MaDatSan, 5, 50000, N'Chưa thanh toán');
        
        PRINT N'T1: Đã thêm dịch vụ (Chưa Commit). Đang chờ...';
        WAITFOR DELAY '00:00:10'; -- Giữ khóa X để T2 đọc nhầm (nếu T2 không chặn)
        
    ROLLBACK TRAN -- Hủy bỏ (Khách đổi ý không gọi nữa)
    PRINT N'T1: Đã Rollback (Hủy dịch vụ).';
END
GO

-- [T2 - GIẢI QUYẾT]: Thu ngân tính tiền (Dùng READ COMMITTED để chờ T1 chốt xong mới đọc)
CREATE OR ALTER PROCEDURE sp_LapHoaDon_TinhTien
    @MaDatSan BIGINT
AS
BEGIN
    -- Mức cô lập READ COMMITTED sẽ chặn việc đọc dữ liệu chưa Commit
    SET TRAN ISOLATION LEVEL READ COMMITTED 
    BEGIN TRAN
        DECLARE @TongTien DECIMAL(18,2);
        
        -- Nếu T1 đang chạy, lệnh này sẽ phải CHỜ (Wait) cho đến khi T1 Rollback xong
        SELECT @TongTien = SUM(ThanhTien) 
        FROM CT_DICHVUDAT 
        WHERE MaDatSan = @MaDatSan;

        PRINT N'T2: Tổng tiền tính được là: ' + FORMAT(ISNULL(@TongTien, 0), 'N0');
    COMMIT TRAN
END
GO

-- [T1]: Dùng sp_ThanhToanOnline_Rollback hoặc tương tự để test

-- Tình huống 17: Unrepeatable Read (Xem chi tiết phiếu)
-- [T1]: Xem chi tiết phiếu (Giải pháp: REPEATABLE READ)
CREATE OR ALTER PROCEDURE sp_XemChiTietPhieuDatSan
    @MaDatSan BIGINT
AS
BEGIN
    SET TRAN ISOLATION LEVEL REPEATABLE READ
    BEGIN TRAN
        SELECT P.*, H.HinhThucTT 
        FROM PHIEUDATSAN P LEFT JOIN HOADON H ON P.MaDatSan = H.MaPhieu
        WHERE P.MaDatSan = @MaDatSan;
        
        WAITFOR DELAY '00:00:05';
        
        SELECT P.*, H.HinhThucTT 
        FROM PHIEUDATSAN P LEFT JOIN HOADON H ON P.MaDatSan = H.MaPhieu
        WHERE P.MaDatSan = @MaDatSan;
    COMMIT TRAN
END
GO

-- [T2]: Thanh toán (Dùng sp_ThanhToanVaXuatHoaDon bên dưới)

-- Tình huống 19: Lost Update (Cộng điểm tích lũy)
-- [T1] & [T2]: Cùng chạy (Giải pháp: SERIALIZABLE)
CREATE OR ALTER PROCEDURE sp_CapNhatDiemTichLuy_AnToan
    @MaKH VARCHAR(20),
    @DiemCong INT
AS
BEGIN
    SET NOCOUNT ON;
    SET TRAN ISOLATION LEVEL SERIALIZABLE;
    
    BEGIN TRY
        BEGIN TRAN;
        
        DECLARE @DiemCu INT;
        SELECT @DiemCu = DiemTichLuy FROM KHACHHANG WHERE MaKH = @MaKH;

        WAITFOR DELAY '00:00:05';

        UPDATE KHACHHANG SET DiemTichLuy = @DiemCu + @DiemCong WHERE MaKH = @MaKH;
        
        COMMIT TRAN;
        PRINT N'Cập nhật điểm thành công.';
    END TRY
    BEGIN CATCH
        ROLLBACK TRAN;
        PRINT N'Lỗi cập nhật điểm (Deadlock detected).';
    END CATCH
END
GO

-- ===================================================================================
-- ==             PHẦN 3: CÁC STORED PROCEDURE NGHIỆP VỤ CHÍNH		                ==
-- ==																			    ==																	==
-- ===================================================================================

-- 1. ĐẶT SÂN
CREATE OR ALTER PROCEDURE sp_DatSan
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
        IF dbo.f_KiemTraSanTrong(@MaSan, @NgayDat, @GioBatDau, @GioKetThuc) = 0
        BEGIN
            ROLLBACK TRAN; 
            RAISERROR(N'Lỗi: Sân đã bị người khác đặt!', 16, 1);
            RETURN;
        END
        IF @KenhDat = 'Online' AND DATEDIFF(HOUR, GETDATE(), CAST(@NgayDat AS DATETIME) + CAST(@GioBatDau AS DATETIME)) < 2
        BEGIN
             ROLLBACK TRAN;
             RAISERROR(N'Lỗi: Đặt Online phải trước 2 tiếng!', 16, 1);
             RETURN;
        END
        INSERT INTO PHIEUDATSAN (MaKH, NguoiLap, NgayDat, NgayKetThuc, GioBatDau, GioKetThuc, KenhDat, TrangThai)
        VALUES (@MaKH, @NguoiLap, @NgayDat, @NgayDat, @GioBatDau, @GioKetThuc, @KenhDat, N'Nháp');
        DECLARE @MaDatSan BIGINT = SCOPE_IDENTITY();
        INSERT INTO DATSAN (MaDatSan, MaSan) VALUES (@MaDatSan, @MaSan);
        -- REMOVED UPDATE SAN TinhTrang. Initial booking is Draft.
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

-- 2. THÊM DỊCH VỤ
CREATE OR ALTER PROCEDURE sp_ThemDichVu
    @MaDatSan BIGINT,
    @MaDV VARCHAR(20),
    @SoLuong INT
AS
BEGIN
    SET NOCOUNT ON;
    SET TRAN ISOLATION LEVEL REPEATABLE READ; -- Giữ khóa tồn kho
    
    BEGIN TRY
        BEGIN TRAN;
        DECLARE @MaCS VARCHAR(20);
        DECLARE @DonGia DECIMAL(18,2);
        DECLARE @IsStockItem BIT = 1; -- Mặc định là sản phẩm vật lý
        
        -- Lấy MaCS chính xác qua các bảng join
        SELECT TOP 1 @MaCS = S.MaCS 
        FROM PHIEUDATSAN P 
        JOIN DATSAN D ON P.MaDatSan = D.MaDatSan 
        JOIN SAN S ON D.MaSan = S.MaSan 
        WHERE P.MaDatSan = @MaDatSan;

        -- Lấy đơn giá dịch vụ
        SELECT @DonGia = DonGia FROM DICHVU WHERE MaDV = @MaDV;
        
        IF @DonGia IS NULL
        BEGIN
            ROLLBACK TRAN;
            RAISERROR(N'Dịch vụ không tồn tại!', 16, 1);
            RETURN;
        END

        -- Kiểm tra xem có phải HLV, VIP, Locker hay không (KHÔNG TRỪ KHO)
        IF EXISTS (
            SELECT 1 FROM DICHVU DV 
            JOIN LOAIDV L ON DV.MaLoaiDV = L.MaLoaiDV
            WHERE DV.MaDV = @MaDV 
            AND (
                -- Check theo Mã Loại (LDV001=HLV, LDV004=VIP, LDV005=Locker)
                L.MaLoaiDV IN ('LDV001', 'LDV004', 'LDV005') 
                -- Check fallback theo Tên
                OR L.TenLoai LIKE N'%Huấn luyện viên%' 
                OR L.TenLoai LIKE N'%VIP%' 
                OR L.TenLoai LIKE N'%Tủ đồ%'
            )
        )
        BEGIN
            SET @IsStockItem = 0;  -- Không là sản phẩm vật lý
        END

        -- CHỈ KIỂM TRA TỒN KHO NẾU LÀ SẢN PHẨM VẬT LÝ
        IF @IsStockItem = 1
        BEGIN
            DECLARE @TonKho INT;
            SELECT @TonKho = SoLuongTon FROM DV_COSO WHERE MaDV = @MaDV AND MaCS = @MaCS;
            
            IF @TonKho IS NULL OR @TonKho < @SoLuong
            BEGIN
                ROLLBACK TRAN;
                RAISERROR(N'Lỗi: Số lượng tồn kho không đủ!', 16, 1);
                RETURN;
            END
        END
        
        -- CẬP NHẬT HOẶC THÊM MỚI
        IF EXISTS (SELECT 1 FROM CT_DICHVUDAT WHERE MaDatSan = @MaDatSan AND MaDV = @MaDV)
        BEGIN
            UPDATE CT_DICHVUDAT SET SoLuong = SoLuong + @SoLuong, ThanhTien = (SoLuong + @SoLuong) * @DonGia WHERE MaDatSan = @MaDatSan AND MaDV = @MaDV;
        END
        ELSE
        BEGIN
            INSERT INTO CT_DICHVUDAT (MaDV, MaDatSan, SoLuong, ThanhTien, TrangThaiSuDung) VALUES (@MaDV, @MaDatSan, @SoLuong, @SoLuong * @DonGia, N'Chưa thanh toán');
        END
        
        -- TRỪ KHO CHỈ NẾU LÀ SẢN PHẨM VẬT LÝ
        IF @IsStockItem = 1
        BEGIN
            UPDATE DV_COSO SET SoLuongTon = SoLuongTon - @SoLuong WHERE MaDV = @MaDV AND MaCS = @MaCS;
        END
        
        COMMIT TRAN;
        PRINT N'Thêm dịch vụ thành công!';
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRAN;
        DECLARE @Msg NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(@Msg, 16, 1);
    END CATCH
END
GO

-- 3. THANH TOÁN (ONLINE - CONFIRM BOOKING)
CREATE OR ALTER PROCEDURE sp_ThanhToanOnline
    @MaDatSan BIGINT,
    @NguoiLap VARCHAR(20),
    @HinhThucTT NVARCHAR(50), 
    @MaUD VARCHAR(20) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SET TRAN ISOLATION LEVEL REPEATABLE READ;
    
    BEGIN TRY
        BEGIN TRAN;
        
        -- 1. KIỂM TRA LẠI TÌNH TRẠNG SÂN (DOUBLE-CHECK)
        DECLARE @MaSanCheck VARCHAR(20), @NgayDatCheck DATE, @GioBDCheck TIME, @GioKTCheck TIME;
        
        SELECT @MaSanCheck = D.MaSan, @NgayDatCheck = P.NgayDat, @GioBDCheck = P.GioBatDau, @GioKTCheck = P.GioKetThuc, @MaKH = P.MaKH
        FROM PHIEUDATSAN P
        JOIN DATSAN D ON P.MaDatSan = D.MaDatSan
        WHERE P.MaDatSan = @MaDatSan;
        
        -- Kiểm tra nếu đã có người khác đặt (Trừ chính đơn này - dù đơn này đang là Nháp)
        -- f_KiemTraSanTrong đã được sửa để bỏ qua Nháp. Nhưng nếu có đơn "Đã đặt" khác chèn vào thì sẽ trả về 0.
        IF dbo.f_KiemTraSanTrong(@MaSanCheck, @NgayDatCheck, @GioBDCheck, @GioKTCheck) = 0
        BEGIN
            ROLLBACK TRAN;
            RAISERROR(N'Rất tiếc! Sân đã bị người khác đặt trong lúc bạn đang thanh toán.', 16, 1);
            RETURN;
        END

        DECLARE @TongCong DECIMAL(18,2) = dbo.f_TinhTienSan(@MaDatSan) + dbo.f_TinhTienDichVu(@MaDatSan);
        DECLARE @GiamGia DECIMAL(18,2) = 0;
        DECLARE @ThanhTien DECIMAL(18,2);
        
        IF @MaUD IS NOT NULL
        BEGIN
            DECLARE @TyLeGiam DECIMAL(5,2);
            SELECT @TyLeGiam = TyLeGiamGia FROM UUDAI WHERE MaUD = @MaUD;
            IF @TyLeGiam IS NOT NULL SET @GiamGia = @TongCong * (@TyLeGiam / 100.0);
        END
        
        DECLARE @TyLeThanhVien DECIMAL(5,2) = 0;
        SELECT @TyLeThanhVien = CB.UuDai FROM KHACHHANG KH JOIN CAPBAC CB ON KH.MaCB = CB.MaCB WHERE KH.MaKH = @MaKH;
        SET @GiamGia = @GiamGia + (@TongCong * (@TyLeThanhVien / 100.0));
        
        SET @ThanhTien = @TongCong - @GiamGia;
        IF @ThanhTien < 0 SET @ThanhTien = 0;
        
        INSERT INTO HOADON (MaPhieu, NguoiLap, NgayLap, TongTien, GiamGia, ThanhTien, HinhThucTT)
        VALUES (@MaDatSan, @NguoiLap, GETDATE(), @TongCong, @GiamGia, @ThanhTien, @HinhThucTT);
        DECLARE @MaHD BIGINT = SCOPE_IDENTITY();
        
        -- UPDATE TRẠNG THÁI: CHÍNH THỨC ĐÃ ĐẶT
        UPDATE PHIEUDATSAN SET TrangThai = N'Đã đặt', NgayTao = GETDATE() WHERE MaDatSan = @MaDatSan;
        UPDATE CT_DICHVUDAT SET TrangThaiSuDung = N'Đã thanh toán' WHERE MaDatSan = @MaDatSan;
        
        -- REMOVED UPDATE SAN TinhTrang. We rely on time-based checking.
        
        DECLARE @DiemCong INT = CAST(@ThanhTien / 100000 AS INT);
        IF @DiemCong > 0
        BEGIN
            DECLARE @DiemCu INT;
            SELECT @DiemCu = DiemTichLuy FROM KHACHHANG WHERE MaKH = @MaKH;
            UPDATE KHACHHANG SET DiemTichLuy = @DiemCu + @DiemCong WHERE MaKH = @MaKH;
        END

        COMMIT TRAN;
        PRINT N'Thanh toán thành công. Mã HĐ: ' + CAST(@MaHD AS VARCHAR(20));
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRAN;
        DECLARE @Msg NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(@Msg, 16, 1);
    END CATCH
END
GO

-- 3b. THANH TOÁN (TẠI QUẦY/CHECK-OUT)
CREATE OR ALTER PROCEDURE sp_ThanhToanVaXuatHoaDon
    @MaDatSan BIGINT,
    @NguoiLap VARCHAR(20),
    @HinhThucTT NVARCHAR(50), 
    @MaUD VARCHAR(20) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SET TRAN ISOLATION LEVEL REPEATABLE READ; -- Chặn thay đổi giá/điểm
    
    BEGIN TRY
        BEGIN TRAN;
        DECLARE @TongCong DECIMAL(18,2) = dbo.f_TinhTienSan(@MaDatSan) + dbo.f_TinhTienDichVu(@MaDatSan);
        DECLARE @GiamGia DECIMAL(18,2) = 0;
        DECLARE @ThanhTien DECIMAL(18,2);
        DECLARE @MaKH VARCHAR(20);
        SELECT @MaKH = MaKH FROM PHIEUDATSAN WHERE MaDatSan = @MaDatSan;
        
        IF @MaUD IS NOT NULL
        BEGIN
            DECLARE @TyLeGiam DECIMAL(5,2);
            SELECT @TyLeGiam = TyLeGiamGia FROM UUDAI WHERE MaUD = @MaUD;
            IF @TyLeGiam IS NOT NULL SET @GiamGia = @TongCong * (@TyLeGiam / 100.0);
        END
        
        DECLARE @TyLeThanhVien DECIMAL(5,2) = 0;
        SELECT @TyLeThanhVien = CB.UuDai FROM KHACHHANG KH JOIN CAPBAC CB ON KH.MaCB = CB.MaCB WHERE KH.MaKH = @MaKH;
        SET @GiamGia = @GiamGia + (@TongCong * (@TyLeThanhVien / 100.0));
        
        SET @ThanhTien = @TongCong - @GiamGia;
        IF @ThanhTien < 0 SET @ThanhTien = 0;
        
        INSERT INTO HOADON (MaPhieu, NguoiLap, NgayLap, TongTien, GiamGia, ThanhTien, HinhThucTT)
        VALUES (@MaDatSan, @NguoiLap, GETDATE(), @TongCong, @GiamGia, @ThanhTien, @HinhThucTT);
        DECLARE @MaHD BIGINT = SCOPE_IDENTITY();
        
        UPDATE PHIEUDATSAN SET TrangThai = N'Hoàn thành' WHERE MaDatSan = @MaDatSan;
        UPDATE CT_DICHVUDAT SET TrangThaiSuDung = N'Đã thanh toán' WHERE MaDatSan = @MaDatSan;
        
        DECLARE @DiemCong INT = CAST(@ThanhTien / 100000 AS INT);
        IF @DiemCong > 0
        BEGIN
            DECLARE @DiemCu INT;
            SELECT @DiemCu = DiemTichLuy FROM KHACHHANG WHERE MaKH = @MaKH;
            UPDATE KHACHHANG SET DiemTichLuy = @DiemCu + @DiemCong WHERE MaKH = @MaKH;
        END
        
        UPDATE SAN SET TinhTrang = N'Còn Trống' 
        FROM SAN S JOIN DATSAN D ON S.MaSan = D.MaSan WHERE D.MaDatSan = @MaDatSan;
        
        COMMIT TRAN;
        PRINT N'Thanh toán thành công. Mã HĐ: ' + CAST(@MaHD AS VARCHAR(20));
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRAN;
        DECLARE @Msg NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(@Msg, 16, 1);
    END CATCH
END
GO

-- 4. HỦY SÂN
CREATE OR ALTER PROCEDURE sp_HuySan
    @MaDatSan BIGINT,
    @NguoiThucHien VARCHAR(20) 
AS
BEGIN
    SET NOCOUNT ON;
    SET TRAN ISOLATION LEVEL REPEATABLE READ;
    BEGIN TRY
        BEGIN TRAN;
        DECLARE @TienPhat DECIMAL(18,2) = dbo.f_TinhTienPhat(@MaDatSan, GETDATE());
        UPDATE PHIEUDATSAN SET TrangThai = N'Đã hủy' WHERE MaDatSan = @MaDatSan;
        UPDATE SAN SET TinhTrang = N'Còn Trống' 
        FROM SAN S JOIN DATSAN D ON S.MaSan = D.MaSan WHERE D.MaDatSan = @MaDatSan;
        
        IF @TienPhat > 0
        BEGIN
            INSERT INTO HOADON (MaPhieu, NguoiLap, NgayLap, TongTien, GiamGia, ThanhTien, HinhThucTT)
            VALUES (@MaDatSan, @NguoiThucHien, GETDATE(), @TienPhat, 0, @TienPhat, N'Trừ ví/Nợ');
            PRINT N'Đã hủy. Phí phạt: ' + CAST(@TienPhat AS VARCHAR(20));
        END
        ELSE PRINT N'Đã hủy thành công (Miễn phạt).';
        COMMIT TRAN;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRAN;
        DECLARE @Msg NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(@Msg, 16, 1);
    END CATCH
END
GO


-- ===================================================================================
-- ==                                                                               ==
-- ==                                  TRIGGER                                      ==
-- ==                                                                               ==
-- ===================================================================================

-- 1. Kiểm tra trùng lịch đặt sân
CREATE OR ALTER TRIGGER trg_KiemTraTrungLich
ON DATSAN
FOR INSERT, UPDATE
AS
BEGIN
    -- Kiểm tra nếu có bất kỳ dòng nào vừa insert bị trùng lịch
    IF EXISTS (
        SELECT 1
        FROM inserted I
        JOIN PHIEUDATSAN P_Moi ON I.MaDatSan = P_Moi.MaDatSan -- Lấy giờ của phiếu vừa đặt
        JOIN DATSAN D_Cu ON I.MaSan = D_Cu.MaSan -- Join với các đơn đặt cũ cùng sân
        JOIN PHIEUDATSAN P_Cu ON D_Cu.MaDatSan = P_Cu.MaDatSan -- Lấy giờ của các phiếu cũ
        WHERE P_Cu.MaDatSan <> P_Moi.MaDatSan -- Khác chính nó
          AND P_Cu.NgayDat = P_Moi.NgayDat -- Cùng ngày
          AND P_Cu.TrangThai NOT IN (N'Đã hủy', N'No-Show') -- Phiếu cũ chưa hủy
          AND (
              (P_Moi.GioBatDau >= P_Cu.GioBatDau AND P_Moi.GioBatDau < P_Cu.GioKetThuc) -- Giờ bắt đầu lọt vào ca cũ
              OR
              (P_Moi.GioKetThuc > P_Cu.GioBatDau AND P_Moi.GioKetThuc <= P_Cu.GioKetThuc) -- Giờ kết thúc lọt vào ca cũ
              OR
              (P_Moi.GioBatDau <= P_Cu.GioBatDau AND P_Moi.GioKetThuc >= P_Cu.GioKetThuc) -- Bao trùm ca cũ
          )
    )
    BEGIN
        RAISERROR (N'Lỗi: Sân này đã bị đặt trùng giờ với một phiếu khác!', 16, 1);
        ROLLBACK TRANSACTION;
    END
END;
GO
--2. Tự động trừ tồn kho khi đặt dịch vụ
CREATE OR ALTER TRIGGER trg_CapNhatTonKhoDichVu
ON CT_DICHVUDAT
FOR INSERT
AS
BEGIN
    DECLARE @MaDV VARCHAR(20);
    DECLARE @SoLuongDat INT;
    DECLARE @MaCS VARCHAR(20); 
    DECLARE @IsStockItem BIT = 1;

    -- Lấy thông tin từ dòng vừa insert
    SELECT @MaDV = I.MaDV, @SoLuongDat = I.SoLuong
    FROM inserted I;

    -- Kiểm tra xem có phải HLV, VIP, Locker hay không
    IF EXISTS (
        SELECT 1 FROM DICHVU DV 
        JOIN LOAIDV L ON DV.MaLoaiDV = L.MaLoaiDV
        WHERE DV.MaDV = @MaDV 
        AND (
            L.MaLoaiDV IN ('LDV001', 'LDV004', 'LDV005') 
            OR L.TenLoai LIKE N'%Huấn luyện viên%' 
            OR L.TenLoai LIKE N'%VIP%' 
            OR L.TenLoai LIKE N'%Tủ đồ%'
        )
    )
    BEGIN
        SET @IsStockItem = 0;  -- Không phải sản phẩm vật lý
    END

    -- Chỉ trừ tồn kho nếu LÀ sản phẩm vật lý
    IF @IsStockItem = 1
    BEGIN
        -- Truy vết ngược để lấy Mã Cơ Sở (MaCS)
        SELECT TOP 1 @MaCS = S.MaCS
        FROM SAN S
        JOIN DATSAN D ON S.MaSan = D.MaSan
        JOIN inserted I ON I.MaDatSan = D.MaDatSan;

        -- Kiểm tra tồn kho trong bảng DV_COSO
        IF EXISTS (
            SELECT 1 
            FROM DV_COSO 
            WHERE MaDV = @MaDV AND MaCS = @MaCS AND SoLuongTon < @SoLuongDat
        )
        BEGIN
            RAISERROR (N'Lỗi: Số lượng tồn kho không được nhỏ hơn 0!', 16, 1);
            ROLLBACK TRANSACTION;
        END
        ELSE
        BEGIN
            -- Trừ tồn kho
            UPDATE DV_COSO
            SET SoLuongTon = SoLuongTon - @SoLuongDat
            WHERE MaDV = @MaDV AND MaCS = @MaCS;
        END
    END
    -- Nếu không phải sản phẩm vật lý (HLV, VIP, Locker), không cần trừ kho
END;
GO

-- 3. Giới hạn số lượng đặt sân của khách
CREATE OR ALTER TRIGGER trg_GioiHanDatSan
ON PHIEUDATSAN
FOR INSERT
AS
BEGIN
    DECLARE @MaKH VARCHAR(20);
    DECLARE @NgayDat DATE;
    
    SELECT @MaKH = MaKH, @NgayDat = NgayDat FROM inserted;

    -- Đếm số phiếu của khách trong ngày (loại trừ phiếu hủy)
    IF (SELECT COUNT(*) 
        FROM PHIEUDATSAN 
        WHERE MaKH = @MaKH AND NgayDat = @NgayDat AND TrangThai <> N'Đã hủy') > 2
    BEGIN
        RAISERROR (N'Lỗi: Mỗi khách hàng chỉ được đặt tối đa 2 sân trong một ngày!', 16, 1);
        ROLLBACK TRANSACTION;
    END
END;
GO

-- 4. Chuyển trạng thái sân hợp lệ
CREATE TRIGGER trg_ChuyenTrangThaiSan
ON SAN
FOR UPDATE
AS
BEGIN
    DECLARE @Cu NVARCHAR(50), @Moi NVARCHAR(50);
    SELECT @Cu = d.TinhTrang, @Moi = i.TinhTrang
    FROM deleted d JOIN inserted i ON d.MaSan = i.MaSan;

    IF (@Cu = N'Còn Trống' AND @Moi NOT IN (N'Đã Đặt', N'Bảo Trì', N'Còn Trống'))
    OR (@Cu = N'Đã Đặt' AND @Moi NOT IN (N'Đang Sử Dụng', N'Đã Hủy', N'Còn Trống')) -- Đã Hủy/Còn Trống nếu khách hủy
    OR (@Cu = N'Đang Sử Dụng' AND @Moi NOT IN (N'Còn Trống', N'Bảo Trì'))
    OR (@Cu = N'Bảo Trì' AND @Moi NOT IN (N'Còn Trống'))
    BEGIN
        RAISERROR (N'Lỗi: Chuyển đổi trạng thái sân không hợp lệ!', 16, 1);
        ROLLBACK TRANSACTION;
    END
END;
GO

-- 5. Kiểm tra Thời lượng & Khung giờ hoạt động
-- 5a. Kiểm tra Thời lượng & Khung giờ hoạt động (Khi UPDATE PHIEU)
CREATE OR ALTER TRIGGER trg_KiemTraThoiLuongDat
ON PHIEUDATSAN
FOR UPDATE
AS
BEGIN
    IF NOT EXISTS (SELECT 1 FROM inserted) RETURN;
    
    -- Chỉ chạy khi có update giờ
    IF NOT UPDATE(GioBatDau) AND NOT UPDATE(GioKetThuc) RETURN;

    DECLARE @GioBD TIME, @GioKT TIME, @LoaiSan NVARCHAR(50);
    DECLARE @ThoiLuong INT;
    DECLARE @GioMoCua TIME, @GioDongCua TIME;

    SELECT @GioBD = I.GioBatDau, @GioKT = I.GioKetThuc, @LoaiSan = LS.TenLS,
           @GioMoCua = CS.GioMoCua, @GioDongCua = CS.GioDongCua
    FROM inserted I
    JOIN DATSAN D ON I.MaDatSan = D.MaDatSan
    JOIN SAN S ON D.MaSan = S.MaSan
    JOIN LOAISAN LS ON S.MaLS = LS.MaLS
    JOIN COSO CS ON S.MaCS = CS.MaCS;
    
    -- Safety check
    IF @GioBD IS NULL OR @GioMoCua IS NULL RETURN;

    -- 1. Kiểm tra khung giờ hoạt động
    IF @GioBD < @GioMoCua OR @GioKT > @GioDongCua
    BEGIN
        RAISERROR (N'Lỗi: Xin lỗi quý khách. Thời gian đặt của bạn nằm ngoài khung giờ hoạt động.', 16, 1);
        ROLLBACK TRANSACTION;
        RETURN;
    END

    -- 2. Kiểm tra thời lượng theo loại sân
    SET @ThoiLuong = DATEDIFF(MINUTE, @GioBD, @GioKT);

    IF @LoaiSan = N'Bóng đá mini' AND @ThoiLuong <> 90
    BEGIN
        RAISERROR (N'Lỗi: Sân bóng đá mini phải đặt đúng 90 phút/trận!', 16, 1);
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

-- 5b. Kiểm tra Thời lượng & Khung giờ hoạt động (Khi INSERT DATSAN - Lúc này mới có MaSan)
CREATE OR ALTER TRIGGER trg_KiemTraThoiLuongDat_OnDatSan
ON DATSAN
FOR INSERT, UPDATE
AS
BEGIN
    IF NOT EXISTS (SELECT 1 FROM inserted) RETURN;

    DECLARE @GioBD TIME, @GioKT TIME, @LoaiSan NVARCHAR(50);
    DECLARE @ThoiLuong INT;
    DECLARE @GioMoCua TIME, @GioDongCua TIME;

    -- Join ngược lại PHIEUDATSAN để lấy giờ
    SELECT @GioBD = P.GioBatDau, @GioKT = P.GioKetThuc, @LoaiSan = LS.TenLS,
           @GioMoCua = CS.GioMoCua, @GioDongCua = CS.GioDongCua
    FROM inserted I
    JOIN PHIEUDATSAN P ON I.MaDatSan = P.MaDatSan
    JOIN SAN S ON I.MaSan = S.MaSan
    JOIN LOAISAN LS ON S.MaLS = LS.MaLS
    JOIN COSO CS ON S.MaCS = CS.MaCS;

    -- 1. Kiểm tra khung giờ hoạt động
    IF @GioBD < @GioMoCua OR @GioKT > @GioDongCua
    BEGIN
        RAISERROR (N'Lỗi: Xin lỗi quý khách. Thời gian đặt của bạn nằm ngoài khung giờ hoạt động.', 16, 1);
        ROLLBACK TRANSACTION;
        RETURN;
    END

    -- 2. Kiểm tra thời lượng theo loại sân
    SET @ThoiLuong = DATEDIFF(MINUTE, @GioBD, @GioKT);

    IF @LoaiSan = N'Bóng đá mini' AND @ThoiLuong <> 90
    BEGIN
        RAISERROR (N'Lỗi: Sân bóng đá mini phải đặt đúng 90 phút/trận!', 16, 1);
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

-- 6. Kiểm tra trùng lịch HLV & Tài nguyên (Phòng/Tủ)
CREATE OR ALTER TRIGGER trg_KiemTraLichDichVu
ON CT_DICHVUDAT
FOR INSERT, UPDATE
AS
BEGIN
    IF NOT EXISTS (SELECT 1 FROM inserted) RETURN;

    -- Chỉ kiểm tra các dịch vụ có tính chất "Chiếm chỗ" (HLV, Phòng, Tủ)
    IF EXISTS (
        SELECT 1
        FROM inserted I
        JOIN DICHVU DV ON I.MaDV = DV.MaDV
        JOIN LOAIDV LDV ON DV.MaLoaiDV = LDV.MaLoaiDV -- <== SỬA: Join bảng này để lấy tên loại
        JOIN PHIEUDATSAN P_Moi ON I.MaDatSan = P_Moi.MaDatSan
        
        -- Tìm các phiếu cũ đã đặt cùng dịch vụ này (Cùng ông HLV/Cùng phòng đó)
        JOIN CT_DICHVUDAT CT_Cu ON I.MaDV = CT_Cu.MaDV 
        JOIN PHIEUDATSAN P_Cu ON CT_Cu.MaDatSan = P_Cu.MaDatSan
        
        WHERE I.MaDatSan <> CT_Cu.MaDatSan -- Khác phiếu hiện tại
          AND P_Cu.NgayDat = P_Moi.NgayDat -- Cùng ngày
          AND P_Cu.TrangThai NOT IN (N'Đã hủy', N'No-Show') -- Phiếu cũ chưa hủy
          
          -- Kiểm tra loại dịch vụ dựa trên tên trong bảng LOAIDV
          AND (LDV.TenLoai IN (N'Huấn luyện viên', N'Phòng VIP', N'Tủ đồ')) 
          
          -- Kiểm tra trùng giờ
          AND (
              (P_Moi.GioBatDau >= P_Cu.GioBatDau AND P_Moi.GioBatDau < P_Cu.GioKetThuc)
              OR
              (P_Moi.GioKetThuc > P_Cu.GioBatDau AND P_Moi.GioKetThuc <= P_Cu.GioKetThuc)
              OR
              (P_Moi.GioBatDau <= P_Cu.GioBatDau AND P_Moi.GioKetThuc >= P_Cu.GioKetThuc)
          )
    )
    BEGIN
        RAISERROR (N'Lỗi: Huấn luyện viên hoặc Phòng/Tủ đồ này đã được đặt kín trong khung giờ này!', 16, 1);
        ROLLBACK TRANSACTION;
    END
END;
GO

-- 7. Ràng buộc về thông tin nhân viên (Giới tính & Chức vụ)
CREATE OR ALTER TRIGGER trg_KiemTraNhanVien
ON NHANVIEN
FOR INSERT, UPDATE
AS
BEGIN
    IF EXISTS (
        SELECT 1 FROM inserted 
        WHERE GioiTinh NOT IN (N'Nam', N'Nữ', N'Khác')
           OR ChucVu NOT IN (N'Quản lý', N'Lễ tân', N'Kỹ thuật', N'Thu ngân', N'HLV')
    )
    BEGIN
        RAISERROR (N'Lỗi: Giới tính hoặc Chức vụ nhân viên không hợp lệ!', 16, 1);
        ROLLBACK TRANSACTION;
    END
END;
GO

--8. Kiểm tra sức chứa 
CREATE OR ALTER TRIGGER trg_KiemTraSucChua
ON SAN
FOR INSERT, UPDATE
AS
BEGIN
    IF EXISTS (SELECT 1 FROM inserted WHERE SucChua <= 0)
    BEGIN
        RAISERROR (N'Lỗi: Sức chứa của sân phải lớn hơn 0!', 16, 1);
        ROLLBACK TRANSACTION;
    END
END;
GO
-- 9. Kiểm tra đơn giá dịch vụ
CREATE OR ALTER TRIGGER trg_KiemTraGiaTriDichVu
ON DICHVU
FOR INSERT, UPDATE
AS
BEGIN
    -- Kiểm tra đơn giá dịch vụ >= 0
    IF EXISTS (SELECT 1 FROM inserted WHERE DonGia < 0)
    BEGIN
        RAISERROR (N'Lỗi: Đơn giá dịch vụ không được nhỏ hơn 0!', 16, 1);
        ROLLBACK TRANSACTION;
        RETURN;
    END
END;
GO
-- 10. Kiểm tra tồn kho
CREATE OR ALTER TRIGGER trg_KiemTraTonKhoDichVu
ON DV_COSO
FOR INSERT, UPDATE
AS
BEGIN
    -- Kiểm tra số lượng tồn >= 0
    IF EXISTS (SELECT 1 FROM inserted WHERE SoLuongTon < 0)
    BEGIN
        RAISERROR (N'Lỗi: Số lượng tồn kho không được nhỏ hơn 0!', 16, 1);
        ROLLBACK TRANSACTION;
        RETURN;
    END
END;
GO