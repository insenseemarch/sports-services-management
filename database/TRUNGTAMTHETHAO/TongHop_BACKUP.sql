USE master
GO

CREATE DATABASE TRUNGTAMTHETHAO
GO

USE TRUNGTAMTHETHAO
GO


-- ===================================================================================
-- ==																				==
-- ==					PARTITION FUNCTION & SCHEME									==
-- ==																				==
-- ===================================================================================

CREATE PARTITION FUNCTION pf_NamHoaDon (DATE)
AS RANGE RIGHT FOR VALUES 
('2020-01-01', '2021-01-01', '2022-01-01', '2023-01-01', 
 '2024-01-01', '2025-01-01', '2026-01-01');
GO

CREATE PARTITION SCHEME ps_NamHoaDon
AS PARTITION pf_NamHoaDon
ALL TO ([PRIMARY]);
GO

-- ===================================================================================
-- ==																				==
-- ==							CREATE TABLE CHƯA FK  								==
-- ==																				==
-- ===================================================================================

-- 1. BẢNG KHÁCH HÀNG 
CREATE TABLE KHACHHANG (
    MaKH VARCHAR(20) PRIMARY KEY,
    HoTen NVARCHAR(100) NOT NULL,
    NgaySinh DATE,
    CCCD VARCHAR(20) UNIQUE,
    SDT VARCHAR(15),
    Email VARCHAR(100),
    DiaChi NVARCHAR(200),
	LaHSSV BIT DEFAULT 0,
	DiemTichLuy INT DEFAULT 0,
	MaCB VARCHAR(20),
	MaTK VARCHAR(20) 
)
GO

-- 2. BẢNG PHIẾU ĐẶT SÂN 
CREATE TABLE PHIEUDATSAN (
    MaDatSan BIGINT IDENTITY(1,1) PRIMARY KEY,
    MaKH VARCHAR(20),
	NguoiLap VARCHAR(20), 
    NgayDat DATE NOT NULL,
    NgayKetThuc DATE,
    GioBatDau TIME NOT NULL,
    GioKetThuc TIME NOT NULL,
    KenhDat NVARCHAR(50),
	TrangThai NVARCHAR(50) NOT NULL
)
GO

-- 3. BẢNG HÓA ĐƠN 
CREATE TABLE HOADON (
    MaHD BIGINT IDENTITY(1,1),
    MaPhieu BIGINT,
	NguoiLap VARCHAR(20),
    NgayLap DATE NOT NULL,
    TongTien DECIMAL(18,2),
    GiamGia DECIMAL(18,2) DEFAULT 0,
    ThanhTien DECIMAL(18,2),
    HinhThucTT NVARCHAR(50),
	CONSTRAINT PK_HOADON PRIMARY KEY (MaHD, NgayLap)
) ON ps_NamHoaDon(NgayLap);
GO

-- 4. BẢNG CA TRỰC
CREATE TABLE CATRUC (
    MaCaTruc BIGINT IDENTITY(1,1) PRIMARY KEY,
    MaNV VARCHAR(20),
    NgayTruc DATE NOT NULL,
    GioBatDau TIME,
    GioKetThuc TIME,
    PhuCap DECIMAL(18,2) DEFAULT 0
)
GO

-- 5. BẢNG BÁO CÁO THÔNG KÊ 
CREATE TABLE BAOCAOTHONGKE (
    MaBaoCao BIGINT IDENTITY(1,1) PRIMARY KEY,
    NguoiLapPhieu VARCHAR(20),
    NgayLap DATE DEFAULT GETDATE(),
    MaCS VARCHAR(20),             
    
    -- Các chỉ số thống kê 
    TyLeDungSan DECIMAL(5,2),
    SoLuongDatOnline INT,
    SoLuongDatTrucTiep INT,
    TinhHinhHuySan INT,
    TinhHinhNoShow INT,
    SoTienBiMatDoHuy DECIMAL(18,2),
    SoLuongSuDungDichVu INT
);
GO

--6. BẢNG DOANH THU CHI TIẾT
CREATE TABLE DOANHTHU (
    MaBaoCao BIGINT,
    LoaiDoanhThu NVARCHAR(20),    -- 'Tháng', 'Quý', 'Năm'
    
    Thang INT,                    -- Lưu số tháng (1-12), nếu là quý/năm thì NULL
    Quy INT,                      -- Lưu số quý (1-4), nếu là tháng/năm thì NULL
    Nam INT,                      -- Lưu năm
    
    TongDoanhThu DECIMAL(18,2),
    
    CONSTRAINT PK_DOANHTHU PRIMARY KEY (MaBaoCao, LoaiDoanhThu),
    CONSTRAINT CK_LoaiDT CHECK (LoaiDoanhThu IN (N'Tháng', N'Quý', N'Năm'))
);
GO
--7. BẢNG TỔNG GIỜ LÀM VIỆC CỦA NHÂN VIÊN
CREATE TABLE TONGGIOLAMVIECNV (
    MaBaoCao BIGINT,
    PhanLoai NVARCHAR(20),        -- 'Tháng', 'Quý', 'Năm'
   
    Thang INT,
    Quy INT,
    Nam INT,
    
    TongGio DECIMAL(18,2),
    
    CONSTRAINT PK_TONGGIOLAMVIECNV PRIMARY KEY (MaBaoCao, PhanLoai),
    CONSTRAINT CK_PhanLoaiGio CHECK (PhanLoai IN (N'Tháng', N'Quý', N'Năm'))
);
GO


-- 8. BẢNG CẤP BẬC
CREATE TABLE CAPBAC (
    MaCB VARCHAR(20) PRIMARY KEY,
    TenCB NVARCHAR(100),
    UuDai DECIMAL(5,2) DEFAULT 0
)
GO

-- 9. BẢNG TÀI KHOẢN 
CREATE TABLE TAIKHOAN (
    MaTK VARCHAR(20) PRIMARY KEY,
    TenDangNhap VARCHAR(50) UNIQUE NOT NULL,
    MatKhau VARCHAR(255) NOT NULL,
    VaiTro NVARCHAR(50),
    NgayDangKy DATE DEFAULT GETDATE()
)
GO

-- 10. BẢNG CƠ SỞ
CREATE TABLE COSO (
    MaCS VARCHAR(20) PRIMARY KEY,
    TenCS NVARCHAR(100) NOT NULL,
    DiaChi NVARCHAR(200),
    ThanhPho NVARCHAR(100)
)
GO

-- 11. BẢNG LOẠI SÂN
CREATE TABLE LOAISAN (
    MaLS VARCHAR(20) PRIMARY KEY,
    TenLS NVARCHAR(100) NOT NULL,
    DVT NVARCHAR(20)
)
GO

-- 12. BẢNG SÂN
CREATE TABLE SAN (
    MaSan VARCHAR(20) PRIMARY KEY,
    MaLS VARCHAR(20),
    MaCS VARCHAR(20),
    SucChua INT,
    TinhTrang NVARCHAR(50) DEFAULT N'Còn Trống'
)
GO

-- 13. BẢNG LOẠI DỊCH VỤ
CREATE TABLE LOAIDV (
    MaLoaiDV VARCHAR(20) PRIMARY KEY,
    TenLoai NVARCHAR(100)
)
GO

-- 14. BẢNG DỊCH VỤ 
CREATE TABLE DICHVU (
    MaDV VARCHAR(20) PRIMARY KEY,
    MaLoaiDV VARCHAR(20),
    DonGia DECIMAL(18,2),
    DVT NVARCHAR(20),
    TinhTrang NVARCHAR(50)
)
GO

-- 15. BẢNG NHÂN VIÊN
CREATE TABLE NHANVIEN (
    MaNV VARCHAR(20) PRIMARY KEY,
    MaCS VARCHAR(20),
    HoTen NVARCHAR(100) NOT NULL,
    NgaySinh DATE,
    GioiTinh NVARCHAR(10),
    CMND_CCCD VARCHAR(20) UNIQUE NOT NULL,
    DiaChi NVARCHAR(200),
    SDT VARCHAR(15),
    ChucVu NVARCHAR(50),
    LuongCoBan DECIMAL(18,2) DEFAULT 0,
    MaLuong VARCHAR(20),
    MaTK VARCHAR(20)
)
GO

-- 16. BẢNG LƯƠNG
CREATE TABLE LUONG (
    MaLuong VARCHAR(20) PRIMARY KEY,
    TongGioLam INT DEFAULT 0,
    PhuCap DECIMAL(18,2) DEFAULT 0,
    ThuLao DECIMAL(18,2) DEFAULT 0,
    HoaHong DECIMAL(18,2) DEFAULT 0,
    TienPhat DECIMAL(18,2) DEFAULT 0
)
GO

-- 17. BẢNG ĐƠN NGHỈ PHÉP
CREATE TABLE DONNGHIPHEP (
    MaDon BIGINT IDENTITY(1,1) PRIMARY KEY,
    MaNV VARCHAR(20),
    NgayNghi DATE,
    CaNghi BIGINT,
    LyDo NVARCHAR(200),
    NgayDuyet DATE,
    TrangThai NVARCHAR(50),
    NguoiThayThe VARCHAR(20)
)
GO

-- 18. BẢNG PHIẾU BẢO TRÌ
CREATE TABLE PHIEUBAOTRI (
    MaPhieu BIGINT IDENTITY(1,1) PRIMARY KEY,
    MaNV VARCHAR(20),
    MaSan VARCHAR(20),
    NgayBatDau DATE,
    NgayKetThucDuKien DATE,
    NgayKetThucThucTe DATE,
    MoTaSuCo NVARCHAR(MAX),
    ChiPhi DECIMAL(18,2) DEFAULT 0,
    TrangThai NVARCHAR(50)
)
GO


-- 19. BẢNG DỊCH VỤ - CƠ SỞ
CREATE TABLE DV_COSO (
    MaDV VARCHAR(20),
    MaCS VARCHAR(20),
	SoLuongTon INT DEFAULT 0,
    CONSTRAINT PK_DV_COSO PRIMARY KEY (MaDV, MaCS)
)
GO

-- 20. BẢNG ĐẶT SÂN (PHEUDATSAN - SAN)
CREATE TABLE DATSAN (
    MaDatSan BIGINT,
    MaSan VARCHAR(20),
    CONSTRAINT PK_DATSAN PRIMARY KEY (MaDatSan, MaSan)
)
GO

-- 21. BẢNG CHI TIẾT DỊCH VỤ ĐẶT (DICHVU -  PHIEUDATSAN)
CREATE TABLE CT_DICHVUDAT (
    MaDV VARCHAR(20),
    MaDatSan BIGINT,
    SoLuong INT DEFAULT 1,
    SoGioThue INT DEFAULT 0,
    ThanhTien DECIMAL(18,2),
    TrangThaiSuDung NVARCHAR(50),
    GhiChu NVARCHAR(200),

    CONSTRAINT PK_CT_DICHVUDAT PRIMARY KEY (MaDV, MaDatSan)
)
GO

-- 22. BẢNG THAM GIA CA TRỰC (NHANVIEN - CATRUC)
CREATE TABLE THAMGIACATRUC (
    MaCaTruc BIGINT,
    MaNV VARCHAR(20),
    CONSTRAINT PK_THAMGIACATRUC PRIMARY KEY (MaCaTruc, MaNV)
)
GO

-- 23. BẢNG HUẤN LUYỆN VIÊN 
CREATE TABLE HLV (
    MaHLV VARCHAR(20) PRIMARY KEY,
	MaNV VARCHAR(20) UNIQUE,
    MaDV VARCHAR(20),
    ChuyenMon NVARCHAR(100),
    MucGia DECIMAL(18,2)
)
GO

-- 24. BẢNG ƯU ĐÃI
CREATE TABLE UUDAI (
    MaUD VARCHAR(20) PRIMARY KEY,
    TenCT NVARCHAR(100),
    TyLeGiamGia DECIMAL(5,2),
    DieuKienApDung NVARCHAR(200)
)
GO

-- 25. BẢNG ƯU ĐÃI - ÁP DỤNG (UUDAI - HOADON)
CREATE TABLE UUDAI_APDUNG (
    MaUD VARCHAR(20),
    MaHD BIGINT,
	NgayLapHD DATE,
    CONSTRAINT PK_UUDAI_APDUNG PRIMARY KEY (MaUD, MaHD, NgayLapHD)
)
GO

-- 26. BẢNG KHUNG GIỜ
CREATE TABLE KHUNGGIO (
    MaKG VARCHAR(20),
    MaLS VARCHAR(20),
    GioBatDau TIME,
    GioKetThuc TIME,
    NgayApDung DATE,
    GiaApDung DECIMAL(18,2),

    CONSTRAINT PK_KHUNGGIO PRIMARY KEY (MaKG, MaLS)
)
GO


-- ===================================================================================
-- ==																				==
-- ==								KHÓA NGOẠI	 									==
-- ==																				==
-- ===================================================================================

-- PHIEUDATSAN 
ALTER TABLE PHIEUDATSAN ADD
FOREIGN KEY (MaKH) REFERENCES KHACHHANG(MaKH),
FOREIGN KEY (NguoiLap) REFERENCES NHANVIEN(MaNV)
GO

ALTER TABLE PHIEUDATSAN
WITH CHECK ADD CONSTRAINT CK_PhieuDatSan_TrangThai 
CHECK (TrangThai IN (N'Đã đặt', N'Đang sử dụng', N'Hoàn thành', N'Đã hủy', N'No-Show'));
GO

-- HOADON 
ALTER TABLE HOADON ADD
FOREIGN KEY (MaPhieu) REFERENCES PHIEUDATSAN(MaDatSan),
FOREIGN KEY (NguoiLap) REFERENCES NHANVIEN(MaNV)
GO

-- CATRUC 
ALTER TABLE CATRUC
ADD CONSTRAINT FK_CATRUC_NHANVIEN
FOREIGN KEY (MaNV) REFERENCES NHANVIEN(MaNV)
GO

-- SAN
ALTER TABLE SAN ADD
FOREIGN KEY (MaLS) REFERENCES LOAISAN(MaLS),
FOREIGN KEY (MaCS) REFERENCES COSO(MaCS)
GO

-- DICHVU
ALTER TABLE DICHVU ADD
FOREIGN KEY (MaLoaiDV) REFERENCES LOAIDV(MaLoaiDV)
GO

-- NHANVIEN
ALTER TABLE NHANVIEN ADD
FOREIGN KEY (MaCS) REFERENCES COSO(MaCS),
FOREIGN KEY (MaLuong) REFERENCES LUONG(MaLuong),
FOREIGN KEY (MaTK) REFERENCES TAIKHOAN(MaTK)
GO

-- DATSAN
ALTER TABLE DATSAN ADD
FOREIGN KEY (MaDatSan) REFERENCES PHIEUDATSAN(MaDatSan),
FOREIGN KEY (MaSan) REFERENCES SAN(MaSan)
GO

-- CT_DICHVUDAT
ALTER TABLE CT_DICHVUDAT ADD
FOREIGN KEY (MaDV) REFERENCES DICHVU(MaDV),
FOREIGN KEY (MaDatSan) REFERENCES PHIEUDATSAN(MaDatSan)
GO

-- PHIEUBAOTRI
ALTER TABLE PHIEUBAOTRI ADD
FOREIGN KEY (MaNV) REFERENCES NHANVIEN(MaNV),
FOREIGN KEY (MaSan) REFERENCES SAN(MaSan)
GO

-- DONNGHIPHEP
ALTER TABLE DONNGHIPHEP ADD
FOREIGN KEY (MaNV) REFERENCES NHANVIEN(MaNV),
FOREIGN KEY (NguoiThayThe) REFERENCES NHANVIEN(MaNV),
FOREIGN KEY (CaNghi) REFERENCES CATRUC(MaCaTruc)
GO

-- THAMGIACATRUC
ALTER TABLE THAMGIACATRUC ADD
FOREIGN KEY (MaCaTruc) REFERENCES CATRUC(MaCaTruc),
FOREIGN KEY (MaNV) REFERENCES NHANVIEN(MaNV)
GO

-- UUDAI_APDUNG
ALTER TABLE UUDAI_APDUNG ADD
FOREIGN KEY (MaUD) REFERENCES UUDAI(MaUD),
FOREIGN KEY (MaHD, NgayLapHD) REFERENCES HOADON(MaHD, NgayLap)
GO

-- DV_COSO
ALTER TABLE DV_COSO ADD
FOREIGN KEY (MaDV) REFERENCES DICHVU(MaDV), 
FOREIGN KEY (MaCS) REFERENCES COSO(MaCS)

-- HLV
ALTER TABLE HLV ADD 
FOREIGN KEY (MaNV) REFERENCES NHANVIEN(MaNV),
FOREIGN KEY (MaDV) REFERENCES DICHVU(MaDV);
GO

-- KHUNGGIO
ALTER TABLE KHUNGGIO ADD
CONSTRAINT FK_KHUNGGIO_LOAISAN
FOREIGN KEY (MaLS) REFERENCES LOAISAN(MaLS)
GO

-- KHACHHANG
ALTER TABLE KHACHHANG ADD
FOREIGN KEY (MaCB) REFERENCES CAPBAC(MaCB), 
FOREIGN KEY (MaTK) REFERENCES TAIKHOAN(MaTK)
GO

-- BAOCAOTHONGKE

ALTER TABLE BAOCAOTHONGKE ADD CONSTRAINT FK_BCTK_NHANVIEN FOREIGN KEY (NguoiLapPhieu) REFERENCES NHANVIEN(MaNV);
ALTER TABLE BAOCAOTHONGKE ADD CONSTRAINT FK_BCTK_COSO FOREIGN KEY (MaCS) REFERENCES COSO(MaCS);
GO
-- DOANHTHU
ALTER TABLE DOANHTHU ADD CONSTRAINT FK_DT_BAOCAO FOREIGN KEY (MaBaoCao) REFERENCES BAOCAOTHONGKE(MaBaoCao);
GO

-- TONGIOLAMVIECNV
ALTER TABLE TONGGIOLAMVIECNV ADD CONSTRAINT FK_TGLV_BAOCAO FOREIGN KEY (MaBaoCao) REFERENCES BAOCAOTHONGKE(MaBaoCao);
GO
-- ===================================================================================
-- ==																				==
-- ==									INDEX   									==
-- ==																				==
-- ===================================================================================

-- Tìm kiếm Khách hàng (Tìm theo SDT hay CCCD)
CREATE NONCLUSTERED INDEX IX_KHACHHANG_TimKiem 
ON KHACHHANG(SDT, CCCD) 
INCLUDE (HoTen, LaHSSV);
GO

-- Check trùng lịch đặt sân (giúp kiểm tra giờ trống nhanh chóng)
CREATE NONCLUSTERED INDEX IX_PHIEUDATSAN_CheckLich 
ON PHIEUDATSAN(NgayDat, GioBatDau, GioKetThuc, TrangThai) -- Thêm TrangThai vào Key
INCLUDE (MaDatSan, MaKH); 
GO

-- Tìm nhanh sân đang thuộc phiếu nào
CREATE NONCLUSTERED INDEX IX_DATSAN_MaSan 
ON DATSAN(MaSan) 
INCLUDE (MaDatSan);
GO

-- Tối ưu hóa đơn & báo cáo doanh thu (Hỗ trợ tính tổng tiền theo ngày)
CREATE NONCLUSTERED INDEX IX_HOADON_BaoCao 
ON HOADON(NgayLap) 
INCLUDE (TongTien, ThanhTien)
ON ps_NamHoaDon(NgayLap);
GO

-- Tìm nhân viên & tài khoản (Hỗ trợ chức năng Đăng nhập)
CREATE UNIQUE NONCLUSTERED INDEX IX_TAIKHOAN_Login 
ON TAIKHOAN(TenDangNhap) 
INCLUDE (MatKhau, VaiTro);
GO

-- Tìm kiếm sân theo loại & cơ sở (Khách tìm sân trống theo loại)
CREATE NONCLUSTERED INDEX IX_SAN_TimKiem 
ON SAN(MaCS, MaLS, TinhTrang);
GO

-- Tối ưu JOIN từ Hóa đơn sang Nhân viên (để báo cáo doanh số NV)
CREATE NONCLUSTERED INDEX IX_HOADON_NguoiLap ON HOADON(NguoiLap);
GO

-- Tối ưu JOIN từ Phiếu đặt sang Khách hàng (xem lịch sử đặt)
CREATE NONCLUSTERED INDEX IX_PHIEUDATSAN_MaKH ON PHIEUDATSAN(MaKH);
GO

-- Tối ưu Tìm kiếm Dịch vụ theo Loại (khi order đồ)
CREATE NONCLUSTERED INDEX IX_DICHVU_LoaiDV ON DICHVU(MaLoaiDV);
GO

-- Tối ưu tra cứu Tồn kho
CREATE NONCLUSTERED INDEX IX_DVCOSO_MaCS ON DV_COSO(MaCS) INCLUDE (SoLuongTon);
GO

-- Lọc nhân viên theo chức vụ 
CREATE NONCLUSTERED INDEX IX_NHANVIEN_ChucVu ON NHANVIEN(ChucVu) INCLUDE (HoTen, SDT);
GO

-- Tìm các phiếu đặt Online/Trực tiếp
CREATE NONCLUSTERED INDEX IX_PHIEUDATSAN_KenhDat ON PHIEUDATSAN(KenhDat);
GO

-- Lọc danh sách sân đang bảo trì hoặc đang trống
CREATE NONCLUSTERED INDEX IX_SAN_TinhTrang ON SAN(TinhTrang);
GO

-- Tìm chi tiết đặt sân theo Sân + Ngày (để hiển thị lịch sân)
CREATE NONCLUSTERED INDEX IX_PHIEUDATSAN_SanNgay 
ON PHIEUDATSAN(NgayDat) INCLUDE (MaDatSan); 
GO

-- Tìm dịch vụ của một phiếu (Để tính tổng tiền nhanh)
CREATE NONCLUSTERED INDEX IX_CT_DICHVUDAT_MaDatSan 
ON CT_DICHVUDAT(MaDatSan) 
INCLUDE (ThanhTien, SoLuong);
GO
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
        SET NgayDat = @NgayMoi, GioBatDau = @GioBDMoi, GioKetThuc = @GioKTMoi
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
        IF dbo.f_KiemTraSanTrong(@MaSan, @NgayDat, @GioBatDau, @GioKetThuc, NULL) = 0
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
        
        SELECT @MaSanCheck = D.MaSan, @NgayDatCheck = P.NgayDat, @GioBDCheck = P.GioBatDau, @GioKTCheck = P.GioKetThuc
        FROM PHIEUDATSAN P
        JOIN DATSAN D ON P.MaDatSan = D.MaDatSan
        WHERE P.MaDatSan = @MaDatSan;

        DECLARE @MaKH VARCHAR(20);
        SELECT @MaKH = MaKH FROM PHIEUDATSAN WHERE MaDatSan = @MaDatSan;
        
        -- Kiểm tra nếu đã có người khác đặt (Trừ chính đơn này - dù đơn này đang là Nháp)
        -- f_KiemTraSanTrong đã được sửa để bỏ qua Nháp. Nhưng nếu có đơn "Đã đặt" khác chèn vào thì sẽ trả về 0.
        IF dbo.f_KiemTraSanTrong(@MaSanCheck, @NgayDatCheck, @GioBDCheck, @GioKTCheck, NULL) = 0
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
        UPDATE PHIEUDATSAN SET TrangThai = N'Đã đặt' WHERE MaDatSan = @MaDatSan;
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

    SELECT @GioBD = I.GioBatDau, @GioKT = I.GioKetThuc, @LoaiSan = LS.TenLS
    --       @GioMoCua = CS.GioMoCua, @GioDongCua = CS.GioDongCua -- COMMENTED OUT to avoid unbound identifier
    FROM inserted I
    JOIN DATSAN D ON I.MaDatSan = D.MaDatSan
    JOIN SAN S ON D.MaSan = S.MaSan
    JOIN LOAISAN LS ON S.MaLS = LS.MaLS;

    -- Tạm thời comment logic này để tránh lỗi Invalid Column nếu bảng COSO chưa có cột
    -- JOIN COSO CS ON S.MaCS = CS.MaCS;
    
    SET @GioMoCua = '06:00:00';
    SET @GioDongCua = '22:00:00';
    
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
    SELECT @GioBD = P.GioBatDau, @GioKT = P.GioKetThuc, @LoaiSan = LS.TenLS
    --       @GioMoCua = CS.GioMoCua, @GioDongCua = CS.GioDongCua -- COMMENTED OUT to avoid unbound identifier
    FROM inserted I
    JOIN PHIEUDATSAN P ON I.MaDatSan = P.MaDatSan
    JOIN SAN S ON I.MaSan = S.MaSan
    JOIN LOAISAN LS ON S.MaLS = LS.MaLS;

    -- Tạm thời comment logic này để tránh lỗi Invalid Column nếu bảng COSO chưa có cột
    -- JOIN COSO CS ON S.MaCS = CS.MaCS;

    SET @GioMoCua = '06:00:00';
    SET @GioDongCua = '22:00:00';

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
USE master
GO

USE TRUNGTAMTHETHAO
GO

--	===================================================================================
--	==								BẢO MẬT & PHÂN QUYỀN							 ==
--	===================================================================================

-- Tạo các nhóm quyền (Role)
CREATE ROLE NhanVienQuanLy;
CREATE ROLE NhanVienLeTan;
CREATE ROLE NhanVienThuNgan;
CREATE ROLE NhanVienKyThuat;
CREATE ROLE KhachHang; 


-- I. NHÂN VIÊN QUẢN LÝ
-- Cấp quyền trên các bảng nhân sự và nghiệp vụ
GRANT SELECT, INSERT, UPDATE, DELETE ON NHANVIEN TO NhanVienQuanLy;
GRANT SELECT, INSERT, UPDATE, DELETE ON CATRUC TO NhanVienQuanLy;
GRANT SELECT, INSERT, UPDATE, DELETE ON THAMGIACATRUC TO NhanVienQuanLy;
GRANT SELECT, INSERT, UPDATE, DELETE ON DICHVU TO NhanVienQuanLy;
GRANT SELECT, INSERT, UPDATE, DELETE ON LOAIDV TO NhanVienQuanLy;
GRANT SELECT, INSERT, UPDATE, DELETE ON UUDAI TO NhanVienQuanLy;
GRANT SELECT, INSERT, UPDATE, DELETE ON COSO TO NhanVienQuanLy;
GRANT SELECT ON BAOCAOTHONGKE TO NhanVienQuanLy;
GRANT SELECT ON DOANHTHU TO NhanVienQuanLy;
GRANT SELECT ON TONGGIOLAMVIECNV TO NhanVienQuanLy;
GRANT EXECUTE TO NhanVienQuanLy; -- Quyền chạy các Procedure tính lương, báo cáo

-- II. NHÂN VIÊN LỄ TÂN
GRANT SELECT ON SAN TO NhanVienLeTan;
GRANT SELECT ON KHUNGGIO TO NhanVienLeTan;
GRANT SELECT ON LOAISAN TO NhanVienLeTan;
GRANT SELECT ON KHACHHANG TO NhanVienLeTan;
GRANT SELECT ON DV_COSO TO NhanVienLeTan;
GRANT INSERT ON KHACHHANG TO NhanVienLeTan;

-- Thao tác đặt sân (Sửa theo đúng bảng DATSAN)
GRANT SELECT, INSERT, UPDATE ON PHIEUDATSAN TO NhanVienLeTan;
GRANT SELECT, INSERT, UPDATE ON DATSAN TO NhanVienLeTan;
GRANT SELECT, INSERT, UPDATE ON CT_DICHVUDAT TO NhanVienLeTan;

-- Chặn xóa phiếu đặt sân (Bảo mật dữ liệu)
DENY DELETE ON PHIEUDATSAN TO NhanVienLeTan;

-- III. NHÂN VIÊN THU NGÂN
GRANT SELECT, INSERT ON HOADON TO NhanVienThuNgan;
GRANT SELECT ON PHIEUDATSAN TO NhanVienThuNgan;
GRANT SELECT ON CT_DICHVUDAT TO NhanVienThuNgan;
GRANT SELECT ON KHACHHANG TO NhanVienThuNgan;
GRANT SELECT ON UUDAI TO NhanVienThuNgan;

-- Cập nhật trạng thái phiếu sau khi thanh toán
GRANT UPDATE (TrangThai) ON PHIEUDATSAN TO NhanVienThuNgan;
GRANT UPDATE (TrangThaiSuDung) ON CT_DICHVUDAT TO NhanVienThuNgan;

-- IV. NHÂN VIÊN KỸ THUẬT
GRANT SELECT ON SAN TO NhanVienKyThuat;
GRANT UPDATE (TinhTrang) ON SAN TO NhanVienKyThuat; -- Cập nhật 'Bảo trì', 'Còn trống'
GRANT SELECT, INSERT, UPDATE ON PHIEUBAOTRI TO NhanVienKyThuat;

-- V. HUẤN LUYỆN VIÊN 
GRANT SELECT ON HLV TO NhanVienKyThuat; -- Cho phép xem hồ sơ HLV
GRANT SELECT ON CT_DICHVUDAT TO KhachHang; -- Khách hàng xem được dịch vụ đi kèm

-- VI. KHÁCH HÀNG
-- Đăng ký tài khoản, tìm kiếm và tự đặt sân online 
GRANT INSERT, UPDATE ON KHACHHANG TO KhachHang;
GRANT SELECT ON SAN TO KhachHang;
GRANT SELECT ON KHUNGGIO TO KhachHang;
GRANT SELECT ON LOAISAN TO KhachHang;
GRANT SELECT ON UUDAI TO KhachHang;

-- Thao tác đặt chỗ online 
GRANT INSERT ON PHIEUDATSAN TO KhachHang;
GRANT INSERT ON DATSAN TO KhachHang;
GRANT INSERT ON CT_DICHVUDAT TO KhachHang;
GO
USE TRUNGTAMTHETHAO
GO

-- ============================================================================
-- PATCH: Sửa lỗi thiếu cột trong Database
-- ============================================================================

PRINT N'🔧 Bắt đầu vá lỗi Database...';
GO

-- 1. Thêm cột NgayTao vào bảng PHIEUDATSAN (nếu chưa có)
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'PHIEUDATSAN' AND COLUMN_NAME = 'NgayTao')
BEGIN
    ALTER TABLE PHIEUDATSAN ADD NgayTao DATETIME DEFAULT GETDATE();
    PRINT N'✅ Đã thêm cột NgayTao vào bảng PHIEUDATSAN';
END
ELSE
BEGIN
    PRINT N'ℹ️  Cột NgayTao đã tồn tại.';
END
GO

-- 2. Cập nhật dữ liệu cũ: Gán NgayTao = NgayDat (tránh bị NULL)
UPDATE PHIEUDATSAN 
SET NgayTao = CAST(NgayDat AS DATETIME) 
WHERE NgayTao IS NULL;
PRINT N'✅ Đã điền dữ liệu cho các phiếu cũ bị trống NgayTao.';
GO

-- 3. Thêm cột GioMoCua vào bảng COSO (nếu chưa có)
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'COSO' AND COLUMN_NAME = 'GioMoCua')
BEGIN
    ALTER TABLE COSO ADD GioMoCua TIME;
    PRINT N'✅ Đã thêm cột GioMoCua vào bảng COSO';
END
ELSE
BEGIN
    PRINT N'ℹ️  Cột GioMoCua đã tồn tại.';
END
GO

-- 4. Thêm cột GioDongCua vào bảng COSO (nếu chưa có)
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'COSO' AND COLUMN_NAME = 'GioDongCua')
BEGIN
    ALTER TABLE COSO ADD GioDongCua TIME;
    PRINT N'✅ Đã thêm cột GioDongCua vào bảng COSO';
END
ELSE
BEGIN
    PRINT N'ℹ️  Cột GioDongCua đã tồn tại.';
END
GO

-- 5. Cập nhật giờ mở/đóng cửa mặc định cho các cơ sở cũ (nếu NULL)
UPDATE COSO 
SET GioMoCua = '06:00:00', GioDongCua = '22:00:00'
WHERE GioMoCua IS NULL OR GioDongCua IS NULL;
PRINT N'✅ Đã cập nhật giờ mở/đóng cửa mặc định cho các cơ sở.';
GO

PRINT N'';
PRINT N'🎉 Hoàn tất vá lỗi! Database đã sẵn sàng.';
GO
USE TRUNGTAMTHETHAO
GO

-- ============================================================================
-- PATCH: Bổ sung Schema cho module Quản Lý Giá & Ưu Đãi
-- ============================================================================

PRINT N'🔧 Bắt đầu cập nhật schema Quản Lý Giá...';
GO

-- 1. Bảng THAMSO_HETHONG
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[THAMSO_HETHONG]') AND type in (N'U'))
BEGIN
    CREATE TABLE THAMSO_HETHONG (
        MaThamSo VARCHAR(50) PRIMARY KEY,
        TenThamSo NVARCHAR(100),
        GiaTri NVARCHAR(MAX),
        MoTa NVARCHAR(200),
        DonVi NVARCHAR(50), -- New column
        NgayCapNhat DATETIME DEFAULT GETDATE()
    );
    PRINT N'✅ Đã tạo bảng THAMSO_HETHONG';
    
    -- Insert default data
    INSERT INTO THAMSO_HETHONG (MaThamSo, TenThamSo, GiaTri, MoTa, DonVi)
    VALUES 
    ('TyLeHuySan', N'Tỷ lệ phạt hủy sân', '10', N'Phần trăm phí phạt khi hủy sân (0-100)', N'%'),
    ('ThoiGianHuyTruoc', N'Thời gian hủy trước', '2', N'Số giờ tối thiểu phải hủy trước giờ đá', N'Giờ'),
    ('DiemTichLuyToiThieu', N'Điểm tích lũy tối thiểu', '100', N'Điểm tối thiểu để được đổi quà', N'Điểm');
END
ELSE 
BEGIN
    PRINT N'ℹ️  Bảng THAMSO_HETHONG đã tồn tại.';
    -- Check and add DonVi if missing (for legacy runs)
    IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'THAMSO_HETHONG' AND COLUMN_NAME = 'DonVi')
    BEGIN
        ALTER TABLE THAMSO_HETHONG ADD DonVi NVARCHAR(50);
        PRINT N'✅ Đã thêm cột DonVi cho THAMSO_HETHONG';
    END
END
GO

-- 2. Cập nhật bảng KHUNGGIO
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'KHUNGGIO' AND COLUMN_NAME = 'TenKhungGio')
BEGIN
    ALTER TABLE KHUNGGIO ADD TenKhungGio NVARCHAR(100);
    ALTER TABLE KHUNGGIO ADD LoaiNgay NVARCHAR(50); -- 'Ngày thường', 'Cuối tuần', 'Ngày lễ'
    ALTER TABLE KHUNGGIO ADD GiaTriToiThieu DECIMAL(18,2) DEFAULT 0;
    ALTER TABLE KHUNGGIO ADD SoGioToiThieu INT DEFAULT 1;
    ALTER TABLE KHUNGGIO ADD TrangThai BIT DEFAULT 1; -- Changed to BIT
    ALTER TABLE KHUNGGIO ADD NgayTao DATETIME DEFAULT GETDATE(); -- New column
    PRINT N'✅ Đã thêm cột cho bảng KHUNGGIO';
END
ELSE 
BEGIN
    PRINT N'ℹ️  Cột bảng KHUNGGIO đã tồn tại.';
    
    -- Fix: Change TrangThai from NVARCHAR to BIT if needed
    IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'KHUNGGIO' AND COLUMN_NAME = 'TrangThai' AND DATA_TYPE = 'nvarchar')
    BEGIN
        -- Drop default constraint if exists
        DECLARE @ConstraintName nvarchar(200)
        SELECT @ConstraintName = Name FROM sys.default_constraints 
        WHERE parent_object_id = OBJECT_ID('KHUNGGIO') 
        AND parent_column_id = (SELECT column_id FROM sys.columns WHERE object_id = OBJECT_ID('KHUNGGIO') AND name = 'TrangThai')
        
        IF @ConstraintName IS NOT NULL
            EXEC('ALTER TABLE KHUNGGIO DROP CONSTRAINT ' + @ConstraintName)

        -- Drop and Re-add column
        ALTER TABLE KHUNGGIO DROP COLUMN TrangThai;
        ALTER TABLE KHUNGGIO ADD TrangThai BIT DEFAULT 1;
        PRINT N'✅ Đã sửa kiểu dữ liệu TrangThai thành BIT cho KHUNGGIO';
    END

    IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'KHUNGGIO' AND COLUMN_NAME = 'NgayTao')
    BEGIN
        ALTER TABLE KHUNGGIO ADD NgayTao DATETIME DEFAULT GETDATE();
        PRINT N'✅ Đã thêm cột NgayTao cho KHUNGGIO';
    END
END
GO

-- 3. Cập nhật bảng UUDAI
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'UUDAI' AND COLUMN_NAME = 'LoaiUuDai')
BEGIN
    ALTER TABLE UUDAI ADD LoaiUuDai NVARCHAR(50); -- 'Giảm giá', 'Tặng giờ', 'Tích điểm'
    ALTER TABLE UUDAI ADD NgayBatDau DATE;
    ALTER TABLE UUDAI ADD NgayKetThuc DATE;
    ALTER TABLE UUDAI ADD GiaTriToiThieu DECIMAL(18,2) DEFAULT 0; -- Đơn hàng tối thiểu để áp dụng
    ALTER TABLE UUDAI ADD SoGioToiThieu INT DEFAULT 0; -- Số giờ đặt tối thiểu
    ALTER TABLE UUDAI ADD TrangThai BIT DEFAULT 1; -- Changed to BIT
    ALTER TABLE UUDAI ADD NgayTao DATETIME DEFAULT GETDATE(); -- New column
    PRINT N'✅ Đã thêm cột cho bảng UUDAI';
END
ELSE 
BEGIN
    PRINT N'ℹ️  Cột bảng UUDAI đã tồn tại.';
    
    -- Fix: Change TrangThai from NVARCHAR to BIT if needed
    IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'UUDAI' AND COLUMN_NAME = 'TrangThai' AND DATA_TYPE = 'nvarchar')
    BEGIN
        -- Drop default constraint if exists
        DECLARE @ConstraintNameUD nvarchar(200)
        SELECT @ConstraintNameUD = Name FROM sys.default_constraints 
        WHERE parent_object_id = OBJECT_ID('UUDAI') 
        AND parent_column_id = (SELECT column_id FROM sys.columns WHERE object_id = OBJECT_ID('UUDAI') AND name = 'TrangThai')
        
        IF @ConstraintNameUD IS NOT NULL
            EXEC('ALTER TABLE UUDAI DROP CONSTRAINT ' + @ConstraintNameUD)

        -- Drop and Re-add column
        ALTER TABLE UUDAI DROP COLUMN TrangThai;
        ALTER TABLE UUDAI ADD TrangThai BIT DEFAULT 1;
        PRINT N'✅ Đã sửa kiểu dữ liệu TrangThai thành BIT cho UUDAI';
    END

    IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'UUDAI' AND COLUMN_NAME = 'NgayTao')
    BEGIN
        ALTER TABLE UUDAI ADD NgayTao DATETIME DEFAULT GETDATE();
        PRINT N'✅ Đã thêm cột NgayTao cho UUDAI';
    END
END
GO

PRINT N'🎉 Cập nhật schema Quản Lý Giá hoàn tất!';
GO
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

-- 2. SP Add Service and Update Invoice
CREATE OR ALTER PROCEDURE sp_ThemDichVuVaCapNhatHoaDon
    @MaDatSan BIGINT,
    @MaDV VARCHAR(20),
    @SoLuong INT,
    @MaNV VARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;

        -- Check Invoice existence
        DECLARE @MaHD BIGINT
        DECLARE @TongTienCu DECIMAL(18,2)
        DECLARE @GiamGia DECIMAL(18,2)
        
        SELECT @MaHD = MaHD, @TongTienCu = TongTien, @GiamGia = GiamGia 
        FROM HOADON WHERE MaPhieu = @MaDatSan

        IF @MaHD IS NULL
        BEGIN
             RAISERROR(N'Phiếu đặt sân chưa được thanh toán (Chưa có hóa đơn). Vui lòng dùng chức năng đặt dịch vụ thường!', 16, 1);
        END

        -- Get Facility of the Booking
        DECLARE @MaCS VARCHAR(20)
        SELECT @MaCS = S.MaCS 
        FROM DATSAN D JOIN SAN S ON D.MaSan = S.MaSan 
        WHERE D.MaDatSan = @MaDatSan

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
        IF EXISTS (SELECT 1 FROM CT_DICHVUDAT WHERE MaDatSan = @MaDatSan AND MaDV = @MaDV)
        BEGIN
            UPDATE CT_DICHVUDAT
            SET SoLuong = SoLuong + @SoLuong,
                ThanhTien = ThanhTien + (@SoLuong * @DonGia)
            WHERE MaDatSan = @MaDatSan AND MaDV = @MaDV
        END
        ELSE
        BEGIN
            INSERT INTO CT_DICHVUDAT (MaDV, MaDatSan, SoLuong, ThanhTien, TrangThaiSuDung, GhiChu)
            VALUES (@MaDV, @MaDatSan, @SoLuong, @SoLuong * @DonGia, N'Đã thanh toán', N'Đặt thêm sau thanh toán')
        END

        -- Calculate Incremental Cost
        DECLARE @TienThem DECIMAL(18,2) = @SoLuong * @DonGia

        -- Update Invoice
        UPDATE HOADON
        SET TongTien = TongTien + @TienThem,
            ThanhTien = (TongTien + @TienThem) - GiamGia -- Assuming Discount is fixed amount. If percent, might need complex logic. But Schema says GiamGia is DECIMAL, usually amount.
        WHERE MaHD = @MaHD

        COMMIT TRANSACTION;

        -- Return Info
        SELECT 
            @MaHD as MaHD,
            @TongTienCu as TongTienCu,
            (@TongTienCu + @TienThem - @GiamGia) as TongTienMoi,
            @TienThem as CanThanhToanThem,
            N'Thêm dịch vụ thành công' as Message
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(@ErrorMessage, 16, 1);
    END CATCH
END
GO
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
-- =============================================
-- PATCH: Bỏ qua phiếu NHÁP khi kiểm tra trùng lịch
-- Mục đích: Phiếu nháp chưa xác nhận nên không block sân
-- =============================================

USE TRUNGTAMTHETHAO;
GO

-- Sửa trigger kiểm tra trùng lịch
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
          AND P_Cu.TrangThai NOT IN (N'Đã hủy', N'No-Show', N'Nháp') -- BỎ QUA PHIẾU NHÁP
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

PRINT 'PATCH COMPLETED: Trigger trg_KiemTraTrungLich đã được cập nhật để bỏ qua phiếu Nháp';
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
USE TRUNGTAMTHETHAO
GO

-- Trigger: Hoàn trả tồn kho dịch vụ khi hủy phiếu đặt sân
CREATE OR ALTER TRIGGER trg_HoanTraDichVuKhiHuy
ON PHIEUDATSAN
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    -- Chỉ xử lý khi trạng thái chuyển sang 'Đã hủy', 'No-Show', hoặc 'Không hợp lệ'
    -- Từ các trạng thái 'giữ chỗ' ('Chờ thanh toán', 'Đã đặt', 'Đã thanh toán')
    IF EXISTS (
        SELECT 1 
        FROM inserted i
        JOIN deleted d ON i.MaDatSan = d.MaDatSan
        WHERE i.TrangThai IN (N'Đã hủy', N'No-Show', N'Không hợp lệ')
          AND d.TrangThai NOT IN (N'Đã hủy', N'No-Show', N'Không hợp lệ', N'Nháp')
    )
    BEGIN
        -- Sử dụng bảng tạm để lưu các phiếu vừa bị hủy
        SELECT i.MaDatSan
        INTO #CancelledBookings
        FROM inserted i
        JOIN deleted d ON i.MaDatSan = d.MaDatSan
        WHERE i.TrangThai IN (N'Đã hủy', N'No-Show', N'Không hợp lệ')
          AND d.TrangThai NOT IN (N'Đã hủy', N'No-Show', N'Không hợp lệ', N'Nháp');

        -- Cập nhật lại tồn kho trong DV_COSO
        -- Logic: Cộng lại Số lượng đã đặt vào Số lượng tồn
        -- Cần biết MaCS (Cơ sở) của sân trong phiếu đặt.
        
        UPDATE KHO
        SET KHO.SoLuongTon = KHO.SoLuongTon + CT.SoLuong
        FROM DV_COSO KHO
        JOIN CT_DICHVUDAT CT ON KHO.MaDV = CT.MaDV
        JOIN DICHVU DV ON CT.MaDV = DV.MaDV
        JOIN LOAIDV LDV ON DV.MaLoaiDV = LDV.MaLoaiDV
        JOIN #CancelledBookings C ON CT.MaDatSan = C.MaDatSan
        JOIN DATSAN DS ON C.MaDatSan = DS.MaDatSan
        JOIN SAN S ON DS.MaSan = S.MaSan
        WHERE KHO.MaCS = S.MaCS
          AND LDV.TenLoai NOT IN (N'Huấn luyện viên', N'Phòng VIP', N'Tủ đồ'); -- Không cộng tồn kho cho các loại này (quản lý bằng lịch)
        
        DROP TABLE #CancelledBookings;
    END
END
GO
USE TRUNGTAMTHETHAO
GO

-- Cập nhật sp_DatSan để thêm NGAYTAO khi insert
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
        IF dbo.f_KiemTraSanTrong(@MaSan, @NgayDat, @GioBatDau, @GioKetThuc, NULL) = 0
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
        
        -- Thêm NgayTao explicitly để đảm bảo không bị NULL (mặc dù có DEFAULT nhưng safety first)
        INSERT INTO PHIEUDATSAN (MaKH, NguoiLap, NgayDat, NgayKetThuc, GioBatDau, GioKetThuc, KenhDat, TrangThai, NgayTao)
        VALUES (@MaKH, @NguoiLap, @NgayDat, @NgayDat, @GioBatDau, @GioKetThuc, @KenhDat, N'Nháp', GETDATE());
        
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
GO

-- ============================================================================
-- PATCH: Thêm cột TrangThai vào bảng NHANVIEN để hỗ trợ Xóa mềm (Soft Delete)
-- ============================================================================

PRINT N'🔧 Bắt đầu cập nhật bảng NHANVIEN...';
GO

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'NHANVIEN' AND COLUMN_NAME = 'TrangThai')
BEGIN
    ALTER TABLE NHANVIEN ADD TrangThai NVARCHAR(50) DEFAULT N'Đang làm';
    PRINT N'✅ Đã thêm cột TrangThai vào bảng NHANVIEN';
    
    -- Cập nhật dữ liệu cũ
    EXEC('UPDATE NHANVIEN SET TrangThai = N''Đang làm'' WHERE TrangThai IS NULL');
    PRINT N'✅ Đã cập nhật trạng thái mặc định cho nhân viên cũ.';
END
ELSE
BEGIN
    PRINT N'ℹ️  Cột TrangThai đã tồn tại trong bảng NHANVIEN.';
END
GO
-- ===================================================================================
-- ==																				==
-- ==									DATA										==
-- ==																				==
-- ===================================================================================

USE TRUNGTAMTHETHAO
GO

-- ======================================================================================
-- BƯỚC 1: DỌN DẸP & CHUẨN BỊ
-- ======================================================================================

-- Tắt ràng buộc để nạp dữ liệu nhanh
EXEC sp_msforeachtable 'ALTER TABLE ? NOCHECK CONSTRAINT all'
GO
DISABLE TRIGGER ALL ON DATABASE;
GO

-- Xóa dữ liệu cũ
DELETE FROM TONGGIOLAMVIECNV; DELETE FROM DOANHTHU; DELETE FROM BAOCAOTHONGKE;
DELETE FROM UUDAI_APDUNG; DELETE FROM HOADON; DELETE FROM CT_DICHVUDAT; 
DELETE FROM DATSAN; DELETE FROM PHIEUDATSAN; DELETE FROM THAMGIACATRUC; 
DELETE FROM CATRUC; DELETE FROM HLV; DELETE FROM PHIEUBAOTRI; 
DELETE FROM DONNGHIPHEP; DELETE FROM NHANVIEN; DELETE FROM KHACHHANG; 
DELETE FROM TAIKHOAN; DELETE FROM SAN; DELETE FROM DICHVU; DELETE FROM DV_COSO;
DELETE FROM KHUNGGIO; DELETE FROM CAPBAC; DELETE FROM UUDAI; 
DELETE FROM LUONG; DELETE FROM LOAIDV; DELETE FROM LOAISAN; DELETE FROM COSO;
GO

-- ======================================================================================
-- BƯỚC 2: NẠP DỮ LIỆU DANH MỤC (MASTER DATA)
-- ======================================================================================

USE TRUNGTAMTHETHAO
GO

-- ===================================================================================
-- ==                         NẠP DỮ LIỆU VÀO CÁC BẢNG                            ==
-- ===================================================================================

USE TRUNGTAMTHETHAO
GO

-- ===================================================================================
-- ==                         NẠP DỮ LIỆU VÀO CÁC BẢNG                            ==
-- ===================================================================================

-- 1. NẠP DATA VÀO BẢNG CẤP BẬC
INSERT INTO CAPBAC (MaCB, TenCB, UuDai) VALUES
('CB001', N'Bronze', 0.00),
('CB002', N'Silver', 0.05),
('CB003', N'Gold', 0.10),
('CB004', N'Platinum', 0.20),
('CB005', N'Diamond', 0.25),
('CB006', N'Master', 0.30),
('CB007', N'Superstar', 0.35),
('CB008', N'Legend', 0.40),
('CB009', N'VIP Silver', 0.45),
('CB010', N'VIP Gold', 0.50);
GO

-- 2. NẠP DATA VÀO BẢNG TÀI KHOẢN
INSERT INTO TAIKHOAN (MaTK, TenDangNhap, MatKhau, VaiTro, NgayDangKy) VALUES
-- Nhân viên (10 account)
('TK001', 'quanly01', 'QL@2024Pass', N'Quản lý', '2024-01-15'),
('TK002', 'letan01', 'LT@2024Pass', N'Lễ tân', '2024-01-20'),
('TK003', 'letan02', 'LT@2024Pass', N'Lễ tân', '2024-02-01'),
('TK004', 'thungan01', 'TN@2024Pass', N'Thu ngân', '2024-02-10'),
('TK005', 'thungan02', 'TN@2024Pass', N'Thu ngân', '2024-02-15'),
('TK006', 'kythuat01', 'KT@2024Pass', N'Kỹ thuật', '2024-03-01'),
('TK007', 'kythuat02', 'KT@2024Pass', N'Kỹ thuật', '2024-03-05'),
('TK008', 'hlv01', 'HLV@2024Pass', N'HLV', '2024-03-10'),
('TK009', 'hlv02', 'HLV@2024Pass', N'HLV', '2024-03-15'),
('TK010', 'hlv03', 'HLV@2024Pass', N'HLV', '2024-03-20'),
-- Khách hàng (12 account)
('TK011', 'khachhang01', 'KH@2024Pass', N'Khách hàng', '2024-04-01'),
('TK012', 'khachhang02', 'KH@2024Pass', N'Khách hàng', '2024-04-05'),
('TK013', 'khachhang03', 'KH@2024Pass', N'Khách hàng', '2024-04-10'),
('TK014', 'khachhang04', 'KH@2024Pass', N'Khách hàng', '2024-04-15'),
('TK015', 'khachhang05', 'KH@2024Pass', N'Khách hàng', '2024-04-20'),
('TK016', 'khachhang06', 'KH@2024Pass', N'Khách hàng', '2024-04-25'),
('TK017', 'khachhang07', 'KH@2024Pass', N'Khách hàng', '2024-05-01'),
('TK018', 'khachhang08', 'KH@2024Pass', N'Khách hàng', '2024-05-05'),
('TK019', 'khachhang09', 'KH@2024Pass', N'Khách hàng', '2024-05-10'),
('TK020', 'khachhang10', 'KH@2024Pass', N'Khách hàng', '2024-05-15'),
('TK021', 'khachhang11', 'KH@2024Pass', N'Khách hàng', '2024-05-20'),
('TK022', 'khachhang12', 'KH@2024Pass', N'Khách hàng', '2024-05-25');
GO

-- 3. NẠP DATA VÀO BẢNG CƠ SỞ
INSERT INTO COSO (MaCS, TenCS, DiaChi, ThanhPho, GioMoCua, GioDongCua) VALUES
('CS001', N'Trung Tâm Thể Thao Quận 1', N'123 Nguyễn Huệ, Quận 1', N'TP. Hồ Chí Minh', '06:00:00', '22:00:00'),
('CS002', N'Trung Tâm Thể Thao Quận 3', N'456 Võ Văn Tần, Quận 3', N'TP. Hồ Chí Minh', '06:00:00', '22:00:00'),
('CS003', N'Trung Tâm Thể Thao Thủ Đức', N'789 Võ Văn Ngân, Thủ Đức', N'TP. Hồ Chí Minh', '05:00:00', '23:00:00'),
('CS004', N'Trung Tâm Thể Thao Bình Thạnh', N'321 Xô Viết Nghệ Tĩnh, Bình Thạnh', N'TP. Hồ Chí Minh', '06:00:00', '22:00:00'),
('CS005', N'Trung Tâm Thể Thao Quận 7', N'147 Nguyễn Thị Thập, Quận 7', N'TP. Hồ Chí Minh', '06:00:00', '22:00:00'),
('CS006', N'Trung Tâm Thể Thao Tân Bình', N'258 Cộng Hòa, Tân Bình', N'TP. Hồ Chí Minh', '06:00:00', '22:00:00'),
('CS007', N'Trung Tâm Thể Thao Phú Nhuận', N'369 Phan Xích Long, Phú Nhuận', N'TP. Hồ Chí Minh', '06:00:00', '22:00:00'),
('CS008', N'Trung Tâm Thể Thao Gò Vấp', N'741 Quang Trung, Gò Vấp', N'TP. Hồ Chí Minh', '06:00:00', '22:00:00')
GO

-- 4. NẠP DATA VÀO BẢNG LƯƠNG (Tạo trước để map với NHANVIEN)
INSERT INTO LUONG (MaLuong, TongGioLam, PhuCap, ThuLao, HoaHong, TienPhat) VALUES
('L001', 176, 2000000, 0, 0, 0),
('L002', 168, 1500000, 0, 0, 0),
('L003', 172, 1500000, 0, 0, 0),
('L004', 180, 1200000, 0, 0, 0),
('L005', 176, 1200000, 0, 0, 0),
('L006', 160, 800000, 0, 0, 0),
('L007', 168, 800000, 0, 0, 0),
('L008', 120, 0, 15000000, 0, 0),
('L009', 128, 0, 16000000, 0, 0),
('L010', 144, 0, 18000000, 0, 0);
GO

-- 5. NẠP DATA VÀO BẢNG NHÂN VIÊN
INSERT INTO NHANVIEN (MaNV, MaCS, HoTen, NgaySinh, GioiTinh, CMND_CCCD, DiaChi, SDT, ChucVu, LuongCoBan, MaLuong, MaTK) VALUES
('QL001', 'CS001', N'Nguyễn Văn An', '1985-05-15', N'Nam', '001085012345', N'123 Lê Lợi, Quận 1, TP.HCM', '0901234567', N'Quản lý', 20000000, 'L001', 'TK001'),
('NVLT001', 'CS001', N'Trần Thị Bình', '1995-08-20', N'Nữ', '001095023456', N'456 Nguyễn Trãi, Quận 5, TP.HCM', '0912345678', N'Lễ tân', 10000000, 'L002', 'TK002'),
('NVLT002', 'CS002', N'Lê Văn Cường', '1993-03-10', N'Nam', '001093034567', N'789 Lý Thường Kiệt, Quận 10, TP.HCM', '0923456789', N'Lễ tân', 10000000, 'L003', 'TK003'),
('NVTN001', 'CS001', N'Phạm Thị Dung', '1990-11-25', N'Nữ', '001090045678', N'321 Hai Bà Trưng, Quận 3, TP.HCM', '0934567890', N'Thu ngân', 9500000, 'L004', 'TK004'),
('NVTN002', 'CS002', N'Hoàng Văn Em', '1992-07-18', N'Nam', '001092056789', N'654 Cách Mạng Tháng 8, Quận 10, TP.HCM', '0945678901', N'Thu ngân', 9500000, 'L005', 'TK005'),
('NVKT001', 'CS001', N'Vũ Thị Phượng', '1988-02-14', N'Nữ', '001088067890', N'987 Điện Biên Phủ, Quận 3, TP.HCM', '0956789012', N'Kỹ thuật', 8000000, 'L006', 'TK006'),
('NVKT002', 'CS003', N'Đỗ Văn Giang', '1991-09-05', N'Nam', '001091078901', N'147 Lê Văn Việt, Quận 9, TP.HCM', '0967890123', N'Kỹ thuật', 8000000, 'L007', 'TK007'),
('HLV001', 'CS001', N'Ngô Văn Hùng', '1987-06-30', N'Nam', '001087089012', N'258 Phan Văn Trị, Gò Vấp, TP.HCM', '0978901234', N'HLV', 12000000, 'L008', 'TK008'),
('HLV002', 'CS002', N'Bùi Thị Lan', '1989-12-12', N'Nữ', '001089090123', N'369 Nguyễn Văn Cừ, Quận 5, TP.HCM', '0989012345', N'HLV', 12000000, 'L009', 'TK009'),
('HLV003', 'CS003', N'Trương Văn Minh', '1986-04-22', N'Nam', '001086091234', N'741 Quốc Lộ 13, Thủ Đức, TP.HCM', '0990123456', N'HLV', 12000000, 'L010', 'TK010');
GO

-- 6. NẠP DATA VÀO BẢNG KHÁCH HÀNG
INSERT INTO KHACHHANG (MaKH, HoTen, NgaySinh, CCCD, SDT, Email, DiaChi, LaHSSV, DiemTichLuy, MaCB, MaTK) VALUES
('KH001', N'Lê Minh Tuấn', '2000-01-15', '079200001234', '0909123456', 'leminhtuan.2000@gmail.com', N'15 Lê Thánh Tôn, Quận 1, TP.HCM', 0, 150, 'CB002', 'TK011'),
('KH002', N'Nguyễn Thị Thanh Hoa', '1998-05-20', '079198002345', '0918234567', 'thanhhoa.nguyen@outlook.com', N'246 Nam Kỳ Khởi Nghĩa, Quận 3, TP.HCM', 0, 520, 'CB004', 'TK012'),
('KH003', N'Trần Văn Nam', '2003-08-10', '079203003456', '0939345678', 'namtran.student@edu.vn', N'10 Võ Văn Ngân, TP. Thủ Đức, TP.HCM', 1, 80, 'CB001', 'TK013'),
('KH004', N'Phạm Thu Mai', '1995-03-25', '079195004567', '0987456789', 'thumai.pham95@gmail.com', N'88 Trần Hưng Đạo, Quận 5, TP.HCM', 0, 1250, 'CB006', 'TK014'),
('KH005', N'Hoàng Anh Đức', '1990-11-30', '079190005678', '0903567890', 'anhduc.hoang@company.com', N'12 Lý Chính Thắng, Quận 3, TP.HCM', 0, 350, 'CB003', 'TK015'),
('KH006', N'Vũ Thị Lan Anh', '2002-07-18', '079202006789', '0945678901', 'lananh.vu2k2@gmail.com', N'56 Pasteur, Quận 1, TP.HCM', 1, 45, 'CB001', 'TK016'),
('KH007', N'Đỗ Minh Hải', '1997-12-05', '079197007890', '0976789012', 'minhhai.do@tech.vn', N'79 Điện Biên Phủ, Bình Thạnh, TP.HCM', 0, 890, 'CB005', 'TK017'),
('KH008', N'Bùi Phương Nga', '2000-09-22', '079200008901', '0933890123', 'phuongnga.bui@gmail.com', N'102 Xô Viết Nghệ Tĩnh, Bình Thạnh, TP.HCM', 1, 210, 'CB002', 'TK018'),
('KH009', N'Ngô Thanh Sơn', '1988-04-14', '079188009012', '0912901234', 'thanhson.ngo@realestate.com', N'34 Cộng Hòa, Tân Bình, TP.HCM', 0, 1580, 'CB007', 'TK019'),
('KH010', N'Trương Ngọc Tâm', '2004-02-28', '079204010123', '0988012345', 'ngoctam.truong@ueh.edu.vn', N'21 Hoàng Văn Thụ, Tân Bình, TP.HCM', 1, 95, 'CB001', 'TK020'),
('KH011', N'Lý Văn Phúc', '1994-06-17', '079194011234', '0905123456', 'phucly.designer@gmail.com', N'45 Lạc Long Quân, Tân Bình, TP.HCM', 0, 2150, 'CB008', 'TK021'),
('KH012', N'Phan Thu Hương', '1999-10-03', '079199012345', '0937234567', 'thuhuong.phan99@gmail.com', N'67 Nguyễn Oanh, Gò Vấp, TP.HCM', 0, 670, 'CB004', 'TK022');
GO

-- 7. NẠP DATA VÀO BẢNG LOẠI SÂN
INSERT INTO LOAISAN (MaLS, TenLS, DVT) VALUES
('LS001', N'Bóng đá mini', N'Trận'),
('LS002', N'Tennis', N'Giờ'),
('LS003', N'Cầu lông', N'Giờ'),
('LS004', N'Bóng rổ', N'Giờ'),
('LS005', N'Bóng chuyền', N'Giờ'),
('LS006', N'Bóng bàn', N'Giờ'),
('LS007', N'Cầu lông đôi', N'Giờ'),
('LS008', N'Tennis đôi', N'Giờ'),
('LS009', N'Bóng đá 7 người', N'Trận'),
('LS010', N'Futsal', N'Trận');
GO

-- 8. NẠP DATA VÀO BẢNG SÂN
INSERT INTO SAN (MaSan, MaLS, MaCS, SucChua, TinhTrang) VALUES
-- Cơ sở 1
('S001', 'LS001', 'CS001', 10, N'Còn Trống'),
('S002', 'LS001', 'CS001', 10, N'Còn Trống'),
('S003', 'LS002', 'CS001', 4, N'Còn Trống'),
('S004', 'LS003', 'CS001', 4, N'Còn Trống'),
('S005', 'LS004', 'CS001', 10, N'Còn Trống'),
-- Cơ sở 2
('S006', 'LS001', 'CS002', 10, N'Còn Trống'),
('S007', 'LS002', 'CS002', 4, N'Còn Trống'),
('S008', 'LS003', 'CS002', 4, N'Còn Trống'),
('S009', 'LS005', 'CS002', 12, N'Còn Trống'),
('S010', 'LS006', 'CS002', 2, N'Còn Trống'),
-- Cơ sở 3
('S011', 'LS001', 'CS003', 10, N'Còn Trống'),
('S012', 'LS003', 'CS003', 4, N'Còn Trống'),
('S013', 'LS004', 'CS003', 10, N'Còn Trống'),
('S014', 'LS007', 'CS003', 4, N'Còn Trống'),
('S015', 'LS008', 'CS003', 4, N'Còn Trống');
GO

-- 9. NẠP DATA VÀO BẢNG LOẠI DỊCH VỤ
INSERT INTO LOAIDV (MaLoaiDV, TenLoai) VALUES
('LDV001', N'Huấn luyện viên'),
('LDV002', N'Đồ uống'),
('LDV003', N'Dụng cụ thể thao'),
('LDV004', N'Phòng VIP'),
('LDV005', N'Tủ đồ'),
('LDV006', N'Đồ ăn nhẹ'),
('LDV007', N'Thiết bị bảo hộ'),
('LDV008', N'Dịch vụ massage'),
('LDV009', N'Dịch vụ giặt đồ'),
('LDV010', N'Cho thuê xe');
GO

-- 10. NẠP DATA VÀO BẢNG DỊCH VỤ
INSERT INTO DICHVU (MaDV, MaLoaiDV, DonGia, DVT, TinhTrang) VALUES
('DV001', 'LDV001', 200000, N'Giờ', N'Còn'),
('DV002', 'LDV001', 250000, N'Giờ', N'Còn'),
('DV003', 'LDV002', 15000, N'Chai', N'Còn'),
('DV004', 'LDV002', 20000, N'Chai', N'Còn'),
('DV005', 'LDV003', 50000, N'Bộ', N'Còn'),
('DV006', 'LDV003', 80000, N'Bộ', N'Còn'),
('DV007', 'LDV004', 500000, N'Giờ', N'Còn'),
('DV008', 'LDV005', 30000, N'Lần', N'Còn'),
('DV009', 'LDV006', 25000, N'Phần', N'Còn'),
('DV010', 'LDV007', 100000, N'Bộ', N'Còn'),
('DV011', 'LDV008', 300000, N'Lần', N'Còn'),
('DV012', 'LDV009', 50000, N'Bộ', N'Còn');
GO

-- 11. NẠP DATA VÀO BẢNG DỊCH VỤ - CƠ SỞ
INSERT INTO DV_COSO (MaDV, MaCS, SoLuongTon) VALUES
-- Cơ sở 1
('DV003', 'CS001', 100),
('DV004', 'CS001', 80),
('DV005', 'CS001', 50),
('DV006', 'CS001', 30),
('DV009', 'CS001', 100),
('DV010', 'CS001', 40),
-- Cơ sở 2
('DV003', 'CS002', 90),
('DV004', 'CS002', 70),
('DV005', 'CS002', 45),
('DV006', 'CS002', 25);
GO

-- 12. NẠP DATA VÀO BẢNG KHUNG GIỜ
INSERT INTO KHUNGGIO (MaKG, MaLS, GioBatDau, GioKetThuc, NgayApDung, GiaApDung) VALUES
-- Bóng đá mini
('KG001', 'LS001', '06:00:00', '08:00:00', '2024-01-01', 400000),
('KG002', 'LS001', '08:00:00', '10:00:00', '2024-01-01', 500000),
('KG003', 'LS001', '10:00:00', '14:00:00', '2024-01-01', 450000),
('KG004', 'LS001', '14:00:00', '17:00:00', '2024-01-01', 600000),
('KG005', 'LS001', '17:00:00', '22:00:00', '2024-01-01', 750000),
-- Tennis
('KG006', 'LS002', '06:00:00', '10:00:00', '2024-01-01', 200000),
('KG007', 'LS002', '10:00:00', '14:00:00', '2024-01-01', 250000),
('KG008', 'LS002', '14:00:00', '18:00:00', '2024-01-01', 300000),
('KG009', 'LS002', '18:00:00', '22:00:00', '2024-01-01', 350000),
-- Cầu lông
('KG010', 'LS003', '06:00:00', '09:00:00', '2024-01-01', 80000),
('KG011', 'LS003', '09:00:00', '12:00:00', '2024-01-01', 100000),
('KG012', 'LS003', '12:00:00', '17:00:00', '2024-01-01', 90000),
('KG013', 'LS003', '17:00:00', '22:00:00', '2024-01-01', 150000),
-- Bóng rổ
('KG014', 'LS004', '08:00:00', '12:00:00', '2024-01-01', 150000),
('KG015', 'LS004', '14:00:00', '18:00:00', '2024-01-01', 180000),
('KG016', 'LS004', '18:00:00', '22:00:00', '2024-01-01', 200000);
GO

-- 13. NẠP DATA VÀO BẢNG ƯU ĐÃI
INSERT INTO UUDAI (MaUD, TenCT, TyLeGiamGia, DieuKienApDung) VALUES
('UD001', N'Giảm giá sinh viên', 10.00, N'Xuất trình thẻ HSSV'),
('UD002', N'Khuyến mãi đầu tuần', 8.00, N'Đặt sân từ thứ 2 đến thứ 4'),
('UD003', N'Ưu đãi thành viên Vàng', 15.00, N'Cấp bậc từ Vàng trở lên'),
('UD004', N'Combo 3 giờ', 12.00, N'Đặt sân từ 3 giờ trở lên'),
('UD005', N'Happy Hour', 20.00, N'Khung giờ 14h-16h các ngày trong tuần'),
('UD006', N'Cuối tuần vui vẻ', 5.00, N'Đặt sân vào thứ 7 hoặc Chủ nhật'),
('UD007', N'Khách hàng mới', 10.00, N'Lần đầu tiên đặt sân tại hệ thống'),
('UD008', N'Đặt online', 7.00, N'Đặt sân qua website hoặc app'),
('UD009', N'Nhóm đông', 10.00, N'Đặt từ 2 sân trở lên cùng lúc'),
('UD010', N'Sinh nhật', 15.00, N'Trong tháng sinh nhật của khách hàng'),
('UD011', N'Khuyến mãi tháng 1', 12.00, N'Áp dụng cho tất cả đơn hàng tháng 1'),
('UD012', N'Ưu đãi HLV', 5.00, N'Khi thuê HLV kèm theo sân');
GO

-- 15. NẠP DATA VÀO BẢNG PHIẾU ĐẶT SÂN
SET IDENTITY_INSERT PHIEUDATSAN ON;
INSERT INTO PHIEUDATSAN (MaDatSan, MaKH, NguoiLap, NgayDat, NgayKetThuc, GioBatDau, GioKetThuc, KenhDat, TrangThai) VALUES
(1, 'KH001', 'NVLT001', '2025-01-05', '2025-01-05', '08:00:00', '09:30:00', N'Online', N'Đã thanh toán'),
(2, 'KH002', 'NVLT001', '2025-01-05', '2025-01-05', '10:00:00', '12:00:00', N'Trực tiếp', N'Đã thanh toán'),
(3, 'KH003', 'NVLT002', '2025-01-06', '2025-01-06', '14:00:00', '15:00:00', N'Online', N'Đã thanh toán'),
(4, 'KH004', 'NVLT001', '2025-01-06', '2025-01-06', '17:00:00', '18:00:00', N'Trực tiếp', N'Đã thanh toán'),
(5, 'KH005', 'NVLT002', '2025-01-07', '2025-01-07', '19:00:00', '21:00:00', N'Online', N'Đã thanh toán'),
(6, 'KH006', 'NVLT001', '2025-01-07', '2025-01-07', '08:00:00', '09:00:00', N'Trực tiếp', N'Đã thanh toán'),
(7, 'KH007', 'NVLT001', '2025-01-08', '2025-01-08', '15:00:00', '16:00:00', N'Online', N'Đã thanh toán'),
(8, 'KH008', 'NVLT002', '2025-01-08', '2025-01-08', '10:00:00', '11:00:00', N'Trực tiếp', N'Đã thanh toán'),
(9, 'KH009', 'NVLT001', '2025-01-09', '2025-01-09', '18:00:00', '20:00:00', N'Online', N'Đã thanh toán'),
(10, 'KH010', 'NVLT002', '2025-01-09', '2025-01-09', '06:00:00', '07:00:00', N'Trực tiếp', N'Đã thanh toán'),
(11, 'KH011', 'NVLT001', '2025-01-10', '2025-01-10', '16:00:00', '18:00:00', N'Online', N'Đã thanh toán'),
(12, 'KH012', 'NVLT002', '2025-01-10', '2025-01-10', '09:00:00', '10:00:00', N'Trực tiếp', N'Đã thanh toán');
SET IDENTITY_INSERT PHIEUDATSAN OFF;
GO

-- 16. NẠP DATA VÀO BẢNG ĐẶT SÂN (PHIEUDATSAN - SAN)
INSERT INTO DATSAN (MaDatSan, MaSan) VALUES
(1, 'S001'),
(2, 'S003'),
(3, 'S004'),
(4, 'S004'),
(5, 'S007'),
(6, 'S008'),
(7, 'S012'),
(8, 'S012'),
(9, 'S007'),
(10, 'S012'),
(11, 'S007'),
(12, 'S008');
GO

-- 17. NẠP DATA VÀO BẢNG CHI TIẾT DỊCH VỤ ĐẶT
INSERT INTO CT_DICHVUDAT (MaDV, MaDatSan, SoLuong, SoGioThue, ThanhTien, TrangThaiSuDung, GhiChu) VALUES
('DV003', 1, 2, 0, 30000, N'Đã sử dụng', N'Nước suối'),
('DV005', 1, 1, 0, 50000, N'Đã sử dụng', N'Bóng đá'),
('DV001', 2, 1, 2, 400000, N'Đã sử dụng', N'HLV cơ bản'),
('DV004', 2, 2, 0, 40000, N'Đã sử dụng', N'Nước ngọt'),
('DV003', 3, 3, 0, 45000, N'Đã sử dụng', N'Nước suối'),
('DV009', 4, 2, 0, 50000, N'Đã sử dụng', N'Bánh snack'),
('DV002', 5, 1, 2, 500000, N'Đã sử dụng', N'HLV cao cấp'),
('DV003', 5, 4, 0, 60000, N'Đã sử dụng', N'Nước suối'),
('DV008', 6, 1, 0, 30000, N'Đã sử dụng', N'Tủ đồ'),
('DV003', 7, 2, 0, 30000, N'Đã sử dụng', N'Nước suối'),
('DV005', 8, 1, 0, 50000, N'Đã sử dụng', N'Vợt cầu lông'),
('DV003', 8, 2, 0, 30000, N'Đã sử dụng', N'Nước suối'),
('DV001', 9, 1, 2, 400000, N'Đã sử dụng', N'HLV tennis');
GO

-- 18. NẠP DATA VÀO BẢNG HÓA ĐƠN
SET IDENTITY_INSERT HOADON ON;
INSERT INTO HOADON (MaHD, MaPhieu, NguoiLap, NgayLap, TongTien, GiamGia, ThanhTien, HinhThucTT) VALUES
(1, 1, 'NVTN001', '2025-01-05', 580000, 58000, 522000, N'Tiền mặt'),
(2, 2, 'NVTN001', '2025-01-05', 940000, 0, 940000, N'Chuyển khoản'),
(3, 3, 'NVTN002', '2025-01-06', 165000, 16500, 148500, N'Tiền mặt'),
(4, 4, 'NVTN001', '2025-01-06', 170000, 0, 170000, N'Chuyển khoản'),
(5, 5, 'NVTN002', '2025-01-07', 1260000, 0, 1260000, N'Thẻ'),
(6, 6, 'NVTN001', '2025-01-07', 130000, 13000, 117000, N'Tiền mặt'),
(7, 7, 'NVTN001', '2025-01-08', 150000, 0, 150000, N'Chuyển khoản'),
(8, 8, 'NVTN002', '2025-01-08', 180000, 18000, 162000, N'Tiền mặt'),
(9, 9, 'NVTN001', '2025-01-09', 1100000, 0, 1100000, N'Thẻ'),
(10, 10, 'NVTN002', '2025-01-09', 110000, 11000, 99000, N'Tiền mặt'),
(11, 11, 'NVTN001', '2025-01-10', 730000, 0, 730000, N'Chuyển khoản'),
(12, 12, 'NVTN002', '2025-01-10', 130000, 0, 130000, N'Tiền mặt');
SET IDENTITY_INSERT HOADON OFF;
GO

-- 19. NẠP DATA VÀO BẢNG ƯU ĐÃI ÁP DỤNG
INSERT INTO UUDAI_APDUNG (MaUD, MaHD, NgayLapHD) VALUES
('UD001', 1, '2025-01-05'),
('UD001', 3, '2025-01-06'),
('UD001', 6, '2025-01-07'),
('UD001', 8, '2025-01-08'),
('UD001', 10, '2025-01-09'),
('UD008', 1, '2025-01-05'),
('UD008', 3, '2025-01-06'),
('UD008', 5, '2025-01-07'),
('UD008', 7, '2025-01-08'),
('UD008', 9, '2025-01-09'),
('UD011', 11, '2025-01-10'),
('UD011', 12, '2025-01-10');
GO

-- 20. NẠP DATA VÀO BẢNG CA TRỰC
SET IDENTITY_INSERT CATRUC ON;
INSERT INTO CATRUC (MaCaTruc, MaNV, NgayTruc, GioBatDau, GioKetThuc, PhuCap) VALUES
(1, 'NVLT001', '2025-01-05', '06:00:00', '14:00:00', 100000),
(2, 'NVLT002', '2025-01-05', '14:00:00', '22:00:00', 100000),
(3, 'NVTN001', '2025-01-05', '08:00:00', '16:00:00', 80000),
(4, 'NVTN002', '2025-01-05', '16:00:00', '22:00:00', 80000),
(5, 'NVLT001', '2025-01-06', '06:00:00', '14:00:00', 100000),
(6, 'NVLT002', '2025-01-06', '14:00:00', '22:00:00', 100000),
(7, 'NVTN001', '2025-01-06', '08:00:00', '16:00:00', 80000),
(8, 'NVTN002', '2025-01-06', '16:00:00', '22:00:00', 80000),
(9, 'NVLT001', '2025-01-07', '06:00:00', '14:00:00', 100000),
(10, 'NVLT002', '2025-01-07', '14:00:00', '22:00:00', 100000),
(11, 'NVTN001', '2025-01-07', '08:00:00', '16:00:00', 80000),
(12, 'NVTN002', '2025-01-07', '16:00:00', '22:00:00', 80000);
SET IDENTITY_INSERT CATRUC OFF;
GO

-- 21. NẠP DATA VÀO BẢNG THAM GIA CA TRỰC
INSERT INTO THAMGIACATRUC (MaCaTruc, MaNV) VALUES
(1, 'NVLT001'),
(2, 'NVLT002'),
(3, 'NVTN001'),
(4, 'NVTN002'),
(5, 'NVLT001'),
(6, 'NVLT002'),
(7, 'NVTN001'),
(8, 'NVTN002'),
(9, 'NVLT001'),
(10, 'NVLT002'),
(11, 'NVTN001'),
(12, 'NVTN002');
GO

-- 22. NẠP DATA VÀO BẢNG HUẤN LUYỆN VIÊN
INSERT INTO HLV (MaHLV, MaNV, MaDV, ChuyenMon, MucGia) VALUES
('HLV_001', 'HLV001', 'DV001', N'Bóng đá mini', 200000),
('HLV_002', 'HLV002', 'DV002', N'Tennis chuyên nghiệp', 250000),
('HLV_003', 'HLV003', 'DV001', N'Cầu lông', 200000),
('HLV_004', 'NVLT001', 'DV002', N'Bóng rổ', 250000),
('HLV_005', 'NVLT002', 'DV001', N'Bóng chuyền', 200000),
('HLV_006', 'NVTN001', 'DV002', N'Thể hình', 250000),
('HLV_007', 'NVTN002', 'DV001', N'Yoga', 200000),
('HLV_008', 'NVKT001', 'DV002', N'Aerobic', 250000),
('HLV_009', 'NVKT002', 'DV001', N'Boxing', 200000),
('HLV_010', 'QL001', 'DV002', N'Bơi lội', 250000);
GO

-- 23. NẠP DATA VÀO BẢNG ĐƠN NGHỈ PHÉP
SET IDENTITY_INSERT DONNGHIPHEP ON;
INSERT INTO DONNGHIPHEP (MaDon, MaNV, NgayNghi, CaNghi, LyDo, NgayDuyet, TrangThai, NguoiThayThe) VALUES
(1, 'NVLT001', '2025-01-15', 1, N'Khám bệnh định kỳ', '2025-01-10', N'Đã duyệt', 'NVLT002'),
(2, 'NVTN001', '2025-01-18', 3, N'Việc gia đình', '2025-01-12', N'Đã duyệt', 'NVTN002'),
(3, 'NVKT001', '2025-01-20', 5, N'Du lịch', '2025-01-14', N'Đã duyệt', 'NVKT002'),
(4, 'HLV001', '2025-01-22', 8, N'Tham gia hội thảo', '2025-01-16', N'Đã duyệt', 'HLV002'),
(5, 'NVLT002', '2025-01-25', 2, N'Nghỉ bù', '2025-01-18', N'Đã duyệt', 'NVLT001'),
(6, 'NVTN002', '2025-01-28', 4, N'Khám sức khỏe', '2025-01-20', N'Đã duyệt', 'NVTN001'),
(7, 'HLV002', '2025-02-01', 9, N'Huấn luyện đội tuyển', '2025-01-22', N'Đã duyệt', 'HLV003'),
(8, 'NVKT002', '2025-02-03', 7, N'Khóa đào tạo', '2025-01-24', N'Đã duyệt', 'NVKT001'),
(9, 'NVLT001', '2025-02-05', 1, N'Nghỉ phép năm', '2025-01-26', N'Đã duyệt', 'NVLT002'),
(10, 'NVTN001', '2025-02-08', 3, N'Tang lễ người thân', '2025-01-28', N'Đã duyệt', 'NVTN002');
SET IDENTITY_INSERT DONNGHIPHEP OFF;
GO

-- 24. NẠP DATA VÀO BẢNG PHIẾU BẢO TRÌ
SET IDENTITY_INSERT PHIEUBAOTRI ON;
INSERT INTO PHIEUBAOTRI (MaPhieu, MaNV, MaSan, NgayBatDau, NgayKetThucDuKien, NgayKetThucThucTe, MoTaSuCo, ChiPhi, TrangThai) VALUES
(1, 'NVKT001', 'S001', '2024-12-20', '2024-12-21', '2024-12-21', N'Thay lưới sân', 500000, N'Hoàn thành'),
(2, 'NVKT002', 'S003', '2024-12-22', '2024-12-23', '2024-12-23', N'Sơn lại mặt sân tennis', 2000000, N'Hoàn thành'),
(3, 'NVKT001', 'S006', '2024-12-25', '2024-12-26', '2024-12-26', N'Thay cỏ nhân tạo', 5000000, N'Hoàn thành'),
(4, 'NVKT002', 'S008', '2024-12-28', '2024-12-29', '2024-12-29', N'Sửa chữa hệ thống đèn', 1500000, N'Hoàn thành'),
(5, 'NVKT001', 'S012', '2025-01-02', '2025-01-03', '2025-01-03', N'Thay lưới cầu lông', 300000, N'Hoàn thành'),
(6, 'NVKT002', 'S013', '2025-01-05', '2025-01-06', '2025-01-06', N'Sơn vạch sân bóng rổ', 800000, N'Hoàn thành'),
(7, 'NVKT001', 'S007', '2025-01-08', '2025-01-09', '2025-01-09', N'Thay dây tennis', 1200000, N'Hoàn thành'),
(8, 'NVKT002', 'S004', '2025-01-10', '2025-01-11', '2025-01-11', N'Bảo trì hệ thống điều hòa', 3000000, N'Hoàn thành'),
(9, 'NVKT001', 'S009', '2025-01-12', '2025-01-13', '2025-01-13', N'Thay lưới bóng chuyền', 400000, N'Hoàn thành'),
(10, 'NVKT002', 'S015', '2025-01-15', '2025-01-16', '2025-01-16', N'Sửa chữa bảng điểm', 600000, N'Hoàn thành');
SET IDENTITY_INSERT PHIEUBAOTRI OFF;
GO

-- 25. NẠP DATA VÀO BẢNG BÁO CÁO THỐNG KÊ
SET IDENTITY_INSERT BAOCAOTHONGKE ON;
INSERT INTO BAOCAOTHONGKE (MaBaoCao, NguoiLapPhieu, NgayLap, MaCS, TyLeDungSan, SoLuongDatOnline, SoLuongDatTrucTiep, TinhHinhHuySan, TinhHinhNoShow, SoTienBiMatDoHuy, SoLuongSuDungDichVu) VALUES
-- Báo cáo tháng
(1, 'QL001', '2025-01-31', 'CS001', 75.50, 120, 80, 5, 3, 2500000, 150),
(2, 'QL001', '2025-02-28', 'CS001', 78.20, 130, 85, 4, 2, 2000000, 160),
(3, 'QL001', '2025-03-31', 'CS001', 80.30, 140, 90, 3, 1, 1500000, 170),
(4, 'QL001', '2025-04-30', 'CS002', 72.40, 110, 75, 6, 4, 3000000, 140),
(5, 'QL001', '2025-05-31', 'CS002', 76.80, 125, 82, 5, 2, 2200000, 155),
(6, 'QL001', '2025-06-30', 'CS002', 82.10, 145, 95, 2, 1, 1000000, 180),
(7, 'QL001', '2025-07-31', 'CS003', 85.50, 150, 100, 1, 0, 500000, 190),
(8, 'QL001', '2025-08-31', 'CS003', 79.30, 135, 88, 4, 2, 1800000, 165),
(9, 'QL001', '2025-09-30', 'CS004', 77.60, 128, 84, 5, 3, 2400000, 158),
(10, 'QL001', '2025-10-31', 'CS004', 83.40, 148, 98, 2, 1, 1200000, 185),
(11, 'QL001', '2025-11-30', 'CS005', 76.80, 125, 82, 5, 2, 2200000, 155), 
(12, 'QL001', '2025-12-31', 'CS005', 85.50, 150, 100, 1, 0, 500000, 190),
-- Báo cáo quý
(13, 'QL001', '2025-03-31', 'CS001', 78.00, 390, 255, 12, 6, 6000000, 480),
(14, 'QL001', '2025-06-30', 'CS002', 77.10, 380, 252, 13, 7, 6200000, 475),
(15, 'QL001', '2025-09-30', 'CS003', 80.80, 413, 272, 10, 5, 4700000, 513),
(16, 'QL001', '2025-12-31', 'CS004', 81.90, 423, 280, 8, 3, 3900000, 530),
-- Báo cáo năm
(17, 'QL001', '2025-12-31', 'CS001', 79.45, 1606, 1059, 43, 21, 20800000, 1998);
SET IDENTITY_INSERT BAOCAOTHONGKE OFF;
GO

-- 26. NẠP DATA VÀO BẢNG DOANH THU
INSERT INTO DOANHTHU (MaBaoCao, LoaiDoanhThu, Thang, Quy, Nam, TongDoanhThu) VALUES
-- Doanh thu từng tháng
(1, N'Tháng', 1, 0, 2025, 150000000),
(2, N'Tháng', 2, 0, 2025, 155000000),
(3, N'Tháng', 3, 0, 2025, 160000000),
(4, N'Tháng', 4, 0, 2025, 145000000),
(5, N'Tháng', 5, 0, 2025, 158000000),
(6, N'Tháng', 6, 0, 2025, 165000000),
(7, N'Tháng', 7, 0, 2025, 170000000),
(8, N'Tháng', 8, 0, 2025, 162000000),
(9, N'Tháng', 9, 0, 2025, 156000000),
(10, N'Tháng', 10, 0, 2025, 168000000),
(11, N'Tháng', 11, 0, 2025, 158000000),
(12, N'Tháng', 12, 0, 2025, 170000000),
-- Doanh thu từng quý
(13, N'Quý', 0, 1, 2025, 465000000),
(14, N'Quý', 0, 2, 2025, 468000000),
(15, N'Quý', 0, 3, 2025, 488000000),
(16, N'Quý', 0, 4, 2025, 496000000),
-- Doanh thu năm
(17, N'Năm', 0, 0, 2025, 1917000000);
GO

-- 27. NẠP DATA VÀO BẢNG TỔNG GIỜ LÀM VIỆC NHÂN VIÊN
INSERT INTO TONGGIOLAMVIECNV (MaBaoCao, PhanLoai, Thang, Quy, Nam, TongGio) VALUES
-- Tổng giờ từng tháng
(1, N'Tháng', 1, 0, 2025, 176.00),
(2, N'Tháng', 2, 0, 2025, 180.00),
(3, N'Tháng', 3, 0, 2025, 172.00),
(4, N'Tháng', 4, 0, 2025, 168.00),
(5, N'Tháng', 5, 0, 2025, 184.00),
(6, N'Tháng', 6, 0, 2025, 176.00),
(7, N'Tháng', 7, 0, 2025, 188.00),
(8, N'Tháng', 8, 0, 2025, 172.00),
(9, N'Tháng', 9, 0, 2025, 180.00),
(10, N'Tháng', 10, 0, 2025, 176.00),
(11, N'Tháng', 11, 0, 2025, 184.00),
(12, N'Tháng', 12, 0, 2025, 188.00),
-- Tổng giờ từng quý
(13, N'Quý', 0, 1, 2025, 528.00),
(14, N'Quý', 0, 2, 2025, 528.00),
(15, N'Quý', 0, 3, 2025, 540.00),
(16, N'Quý', 0, 4, 2025, 548.00),
-- Tổng giờ năm
(17, N'Năm', 0, 0, 2025, 2144.00);
GO

PRINT N'===== ĐÃ NẠP XONG DỮ LIỆU CHO TẤT CẢ 26 BẢNG =====';
PRINT N'Các bảng đã nạp dữ liệu:';
PRINT N'1. CAPBAC, 2. TAIKHOAN, 3. COSO, 4. LUONG, 5. NHANVIEN';
PRINT N'6. KHACHHANG, 7. LOAISAN, 8. SAN, 9. LOAIDV, 10. DICHVU';
PRINT N'11. DV_COSO, 12. KHUNGGIO, 13. UUDAI, 14. PHIEUDATSAN, 15. DATSAN';
PRINT N'16. CT_DICHVUDAT, 17. HOADON, 18. UUDAI_APDUNG, 19. CATRUC';
PRINT N'20. THAMGIACATRUC, 21. HLV, 22. DONNGHIPHEP, 23. PHIEUBAOTRI';
PRINT N'24. BAOCAOTHONGKE, 25. DOANHTHU, 26. TONGGIOLAMVIECNV';
GO


-- =======================================================

--					LẤY KẾT QUẢ ĐÃ NẠP

-- =======================================================

USE TRUNGTAMTHETHAO
GO

-- =======================================================
-- 1. NHÓM DANH MỤC & CẤU HÌNH (Master Data)
-- =======================================================
SELECT * FROM COSO;        -- Danh sách cơ sở
SELECT * FROM LOAISAN;     -- Các loại sân
SELECT * FROM LOAIDV;      -- Các loại dịch vụ
SELECT * FROM CAPBAC;      -- Hạng thành viên
SELECT * FROM LUONG;       -- Bậc lương
SELECT * FROM UUDAI;       -- Các chương trình khuyến mãi
SELECT * FROM TAIKHOAN;    -- Tài khoản đăng nhập
SELECT * FROM KHUNGGIO;    -- Giá tiền theo khung giờ

-- =======================================================
-- 2. NHÓM CON NGƯỜI & TÀI NGUYÊN
-- =======================================================
SELECT * FROM NHANVIEN;    -- Nhân viên (Check MaCS, MaLuong, MaTK)
SELECT * FROM KHACHHANG;   -- Khách hàng (Check MaCB, MaTK)
SELECT * FROM SAN;         -- Sân bãi (Check MaLS, MaCS)
SELECT * FROM DICHVU;      -- Dịch vụ tại cơ sở
SELECT * FROM DV_COSO;     -- Phân phối dịch vụ
SELECT * FROM HLV;         -- Huấn luyện viên

-- =======================================================
-- 3. NHÓM GIAO DỊCH ĐẶT SÂN & HÓA ĐƠN
-- =======================================================
SELECT * FROM PHIEUDATSAN; -- Phiếu đặt (Check NgayDat, GioBatDau)
SELECT * FROM DATSAN;      -- Chi tiết sân được đặt
SELECT * FROM CT_DICHVUDAT;-- Dịch vụ dùng thêm (Nước, bóng...)
SELECT * FROM HOADON;      -- Hóa đơn thanh toán (Check TongTien, GiamGia)
SELECT * FROM UUDAI_APDUNG;-- Hóa đơn nào dùng ưu đãi gì

-- =======================================================
-- 4. NHÓM QUẢN LÝ NHÂN SỰ & BẢO TRÌ
-- =======================================================
SELECT * FROM CATRUC;        -- Lịch trực chính
SELECT * FROM THAMGIACATRUC; -- Nhân viên tham gia ca trực
SELECT * FROM DONNGHIPHEP;   -- Đơn xin nghỉ
SELECT * FROM PHIEUBAOTRI;   -- Lịch sử bảo trì sân
SELECT * FROM BAOCAOTHONGKE; -- Số liệu báo cáo tháng
SELECT * FROM TONGGIOLAMVIECNV;
SELECT * FROM DOANHTHU
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
            RAISERROR(N'Dá»‹ch vá»¥ khĂ´ng tá»“n táº¡i táº¡i cÆ¡ sá»Ÿ nĂ y!', 16, 1);
        END

        IF @SoLuongTon < @SoLuong
        BEGIN
            RAISERROR(N'Sá»‘ lÆ°á»£ng tá»“n kho khĂ´ng Ä‘á»§!', 16, 1);
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
            DECLARE @TrangThaiDV NVARCHAR(50) = CASE WHEN @DaThanhToan = 1 THEN N'ChÆ°a thanh toĂ¡n' ELSE N'Chá» thanh toĂ¡n' END
            INSERT INTO CT_DICHVUDAT (MaDV, MaDatSan, SoLuong, ThanhTien, TrangThaiSuDung, GhiChu)
            VALUES (@MaDV, @MaDatSan, @SoLuong, @TienThem, @TrangThaiDV, N'Äáº·t thĂªm')
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
            N'ThĂªm dá»‹ch vá»¥ thĂ nh cĂ´ng!' as Message
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(@ErrorMessage, 16, 1);
    END CATCH
END
GO
USE TRUNGTAMTHETHAO
GO

USE TRUNGTAMTHETHAO
GO

-- ============================================================
-- PATCH: Thêm cột NgayTao vào bảng PHIEUDATSAN
-- ============================================================

-- 1. Thêm cột NgayTao nếu chưa tồn tại
IF NOT EXISTS (
    SELECT 1 
    FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE TABLE_NAME = 'PHIEUDATSAN' 
      AND COLUMN_NAME = 'NgayTao'
)
BEGIN
    ALTER TABLE PHIEUDATSAN
    ADD NgayTao DATETIME NOT NULL DEFAULT GETDATE();

    PRINT N'Đã thêm cột NgayTao vào bảng PHIEUDATSAN';
END
ELSE
BEGIN
    PRINT N'Cột NgayTao đã tồn tại, không cần thêm';
END
GO

-- 2. Cập nhật dữ liệu cũ: gán NgayTao = NgayDat (tránh NULL)
UPDATE PHIEUDATSAN
SET NgayTao = CAST(NgayDat AS DATETIME)
WHERE NgayTao IS NULL;
GO

PRINT N'Đã cập nhật NgayTao cho các phiếu cũ';
GO

USE TRUNGTAMTHETHAO
GO

-- ============================================================================
-- PATCH: Bổ sung Schema cho module Quản Lý Giá & Ưu Đãi
-- ============================================================================

PRINT N'Bắt đầu cập nhật schema Quản Lý Giá & Ưu Đãi';
GO

-- ============================================================================
-- 1. BẢNG THAMSO_HETHONG
-- ============================================================================
IF NOT EXISTS (
    SELECT 1
    FROM sys.objects
    WHERE object_id = OBJECT_ID(N'dbo.THAMSO_HETHONG')
      AND type = 'U'
)
BEGIN
    CREATE TABLE THAMSO_HETHONG (
        MaThamSo VARCHAR(50) PRIMARY KEY,
        TenThamSo NVARCHAR(100),
        GiaTri NVARCHAR(MAX),
        MoTa NVARCHAR(200),
        DonVi NVARCHAR(50),
        NgayCapNhat DATETIME DEFAULT GETDATE()
    );

    PRINT N'Đã tạo bảng THAMSO_HETHONG';

    INSERT INTO THAMSO_HETHONG (MaThamSo, TenThamSo, GiaTri, MoTa, DonVi)
    VALUES
    ('TyLeHuySan', N'Tỷ lệ phạt hủy sân', '10', N'Phần trăm phí phạt khi hủy sân (0–100)', N'%'),
    ('ThoiGianHuyTruoc', N'Thời gian hủy trước', '2', N'Số giờ tối thiểu phải hủy trước giờ đá', N'Giờ'),
    ('DiemTichLuyToiThieu', N'Điểm tích lũy tối thiểu', '100', N'Điểm tối thiểu để được đổi quà', N'Điểm');
END
ELSE
BEGIN
    PRINT N'Bảng THAMSO_HETHONG đã tồn tại';

    IF NOT EXISTS (
        SELECT 1
        FROM INFORMATION_SCHEMA.COLUMNS
        WHERE TABLE_NAME = 'THAMSO_HETHONG'
          AND COLUMN_NAME = 'DonVi'
    )
    BEGIN
        ALTER TABLE THAMSO_HETHONG ADD DonVi NVARCHAR(50);
        PRINT N'Đã thêm cột DonVi cho bảng THAMSO_HETHONG';
    END
END
GO

-- ============================================================================
-- 2. CẬP NHẬT BẢNG KHUNGGIO
-- ============================================================================
IF NOT EXISTS (
    SELECT 1
    FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_NAME = 'KHUNGGIO'
      AND COLUMN_NAME = 'TenKhungGio'
)
BEGIN
    ALTER TABLE KHUNGGIO ADD
        TenKhungGio NVARCHAR(100),
        LoaiNgay NVARCHAR(50), -- Ngày thường / Cuối tuần / Ngày lễ
        GiaTriToiThieu DECIMAL(18,2) DEFAULT 0,
        SoGioToiThieu INT DEFAULT 1,
        TrangThai BIT DEFAULT 1,
        NgayTao DATETIME DEFAULT GETDATE();

    PRINT N'Đã bổ sung các cột mở rộng cho bảng KHUNGGIO';
END
ELSE
BEGIN
    PRINT N'Bảng KHUNGGIO đã có cấu trúc mở rộng';

    -- Chuyển TrangThai từ NVARCHAR sang BIT nếu cần
    IF EXISTS (
        SELECT 1
        FROM INFORMATION_SCHEMA.COLUMNS
        WHERE TABLE_NAME = 'KHUNGGIO'
          AND COLUMN_NAME = 'TrangThai'
          AND DATA_TYPE = 'nvarchar'
    )
    BEGIN
        DECLARE @DF_KG NVARCHAR(200);

        SELECT @DF_KG = dc.name
        FROM sys.default_constraints dc
        JOIN sys.columns c
            ON dc.parent_object_id = c.object_id
           AND dc.parent_column_id = c.column_id
        WHERE OBJECT_NAME(dc.parent_object_id) = 'KHUNGGIO'
          AND c.name = 'TrangThai';

        IF @DF_KG IS NOT NULL
            EXEC('ALTER TABLE KHUNGGIO DROP CONSTRAINT ' + @DF_KG);

        ALTER TABLE KHUNGGIO DROP COLUMN TrangThai;
        ALTER TABLE KHUNGGIO ADD TrangThai BIT DEFAULT 1;

        PRINT N'Đã chuyển kiểu dữ liệu TrangThai sang BIT cho KHUNGGIO';
    END

    IF NOT EXISTS (
        SELECT 1
        FROM INFORMATION_SCHEMA.COLUMNS
        WHERE TABLE_NAME = 'KHUNGGIO'
          AND COLUMN_NAME = 'NgayTao'
    )
    BEGIN
        ALTER TABLE KHUNGGIO ADD NgayTao DATETIME DEFAULT GETDATE();
        PRINT N'Đã thêm cột NgayTao cho KHUNGGIO';
    END
END
GO

-- ============================================================================
-- 3. CẬP NHẬT BẢNG UUDAI
-- ============================================================================
IF NOT EXISTS (
    SELECT 1
    FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_NAME = 'UUDAI'
      AND COLUMN_NAME = 'LoaiUuDai'
)
BEGIN
    ALTER TABLE UUDAI ADD
        LoaiUuDai NVARCHAR(50), -- Giảm giá / Tặng giờ / Tích điểm
        NgayBatDau DATE,
        NgayKetThuc DATE,
        GiaTriToiThieu DECIMAL(18,2) DEFAULT 0,
        SoGioToiThieu INT DEFAULT 0,
        TrangThai BIT DEFAULT 1,
        NgayTao DATETIME DEFAULT GETDATE();

    PRINT N'Đã bổ sung các cột mở rộng cho bảng UUDAI';
END
ELSE
BEGIN
    PRINT N'Bảng UUDAI đã có cấu trúc mở rộng';

    IF EXISTS (
        SELECT 1
        FROM INFORMATION_SCHEMA.COLUMNS
        WHERE TABLE_NAME = 'UUDAI'
          AND COLUMN_NAME = 'TrangThai'
          AND DATA_TYPE = 'nvarchar'
    )
    BEGIN
        DECLARE @DF_UD NVARCHAR(200);

        SELECT @DF_UD = dc.name
        FROM sys.default_constraints dc
        JOIN sys.columns c
            ON dc.parent_object_id = c.object_id
           AND dc.parent_column_id = c.column_id
        WHERE OBJECT_NAME(dc.parent_object_id) = 'UUDAI'
          AND c.name = 'TrangThai';

        IF @DF_UD IS NOT NULL
            EXEC('ALTER TABLE UUDAI DROP CONSTRAINT ' + @DF_UD);

        ALTER TABLE UUDAI DROP COLUMN TrangThai;
        ALTER TABLE UUDAI ADD TrangThai BIT DEFAULT 1;

        PRINT N'Đã chuyển kiểu dữ liệu TrangThai sang BIT cho UUDAI';
    END

    IF NOT EXISTS (
        SELECT 1
        FROM INFORMATION_SCHEMA.COLUMNS
        WHERE TABLE_NAME = 'UUDAI'
          AND COLUMN_NAME = 'NgayTao'
    )
    BEGIN
        ALTER TABLE UUDAI ADD NgayTao DATETIME DEFAULT GETDATE();
        PRINT N'Đã thêm cột NgayTao cho UUDAI';
    END
END
GO

PRINT N'Hoàn tất cập nhật schema Quản Lý Giá & Ưu Đãi';
GO


USE TRUNGTAMTHETHAO
GO

-- ============================================================================
-- PATCH: Thêm cột TrangThai vào bảng NHANVIEN để hỗ trợ Xóa mềm (Soft Delete)
-- ============================================================================

PRINT N'Bắt đầu cập nhật bảng NHANVIEN';
GO

IF NOT EXISTS (
    SELECT 1
    FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_NAME = 'NHANVIEN'
      AND COLUMN_NAME = 'TrangThai'
)
BEGIN
    ALTER TABLE NHANVIEN
    ADD TrangThai NVARCHAR(50) NOT NULL DEFAULT N'Đang làm';

    PRINT N'Đã thêm cột TrangThai vào bảng NHANVIEN';

    -- Cập nhật dữ liệu cũ
    UPDATE NHANVIEN
    SET TrangThai = N'Đang làm'
    WHERE TrangThai IS NULL;

    PRINT N'Đã cập nhật trạng thái mặc định cho nhân viên cũ';
END
ELSE
BEGIN
    PRINT N'Cột TrangThai đã tồn tại trong bảng NHANVIEN';
END
GO

-- ============================================================================
-- PATCH: Sửa trigger kiểm tra thời lượng đặt sân
-- Mục đích: Cho phép HỦY SÂN mà không bị validate lại giờ đặt (tránh lỗi dữ liệu cũ)
-- ============================================================================

PRINT N'Bắt đầu cập nhật trigger kiểm tra thời lượng đặt sân';
GO

CREATE OR ALTER TRIGGER trg_KiemTraThoiLuongDat
ON PHIEUDATSAN
FOR INSERT, UPDATE
AS
BEGIN
    -- Nếu là UPDATE nhưng KHÔNG đổi giờ thì bỏ qua (cho phép hủy sân)
    IF EXISTS (SELECT 1 FROM deleted)
    BEGIN
        IF NOT UPDATE(GioBatDau) AND NOT UPDATE(GioKetThuc)
            RETURN;
    END

    IF NOT EXISTS (SELECT 1 FROM inserted)
        RETURN;

    DECLARE @GioBD TIME;
    DECLARE @GioKT TIME;
    DECLARE @LoaiSan NVARCHAR(50);
    DECLARE @ThoiLuong INT;

    -- Giờ hoạt động mặc định (fallback)
    DECLARE @GioMoCua TIME = '06:00:00';
    DECLARE @GioDongCua TIME = '22:00:00';

    SELECT 
        @GioBD = I.GioBatDau,
        @GioKT = I.GioKetThuc,
        @LoaiSan = LS.TenLS
    FROM inserted I
    JOIN DATSAN D ON I.MaDatSan = D.MaDatSan
    JOIN SAN S ON D.MaSan = S.MaSan
    JOIN LOAISAN LS ON S.MaLS = LS.MaLS;

    -- 1. Kiểm tra khung giờ hoạt động
    IF @GioBD < @GioMoCua OR @GioKT > @GioDongCua
    BEGIN
        RAISERROR (
            N'Lỗi: Thời gian đặt nằm ngoài khung giờ hoạt động của cơ sở!',
            16, 1
        );
        ROLLBACK TRANSACTION;
        RETURN;
    END

    -- 2. Kiểm tra thời lượng theo loại sân
    SET @ThoiLuong = DATEDIFF(MINUTE, @GioBD, @GioKT);

    IF @LoaiSan = N'Bóng đá mini'
       AND (@ThoiLuong < 90 OR @ThoiLuong % 90 <> 0)
    BEGIN
        RAISERROR (
            N'Lỗi: Sân bóng đá mini phải đặt theo bội số của 90 phút (1 trận = 90 phút)!',
            16, 1
        );
        ROLLBACK TRANSACTION;
        RETURN;
    END

    IF @LoaiSan = N'Tennis'
       AND (@ThoiLuong % 120 <> 0)
    BEGIN
        RAISERROR (
            N'Lỗi: Sân Tennis phải đặt theo bội số của 2 giờ (120 phút)!',
            16, 1
        );
        ROLLBACK TRANSACTION;
        RETURN;
    END

    IF @LoaiSan IN (N'Cầu lông', N'Bóng rổ')
       AND (@ThoiLuong % 60 <> 0)
    BEGIN
        RAISERROR (
            N'Lỗi: Sân Cầu lông / Bóng rổ phải đặt theo bội số của 1 giờ!',
            16, 1
        );
        ROLLBACK TRANSACTION;
        RETURN;
    END
END;
GO

PRINT N'Hoàn tất cập nhật trigger trg_KiemTraThoiLuongDat';
GO


USE TRUNGTAMTHETHAO
GO

-- ============================================================================
-- PATCH: Sửa lỗi thiếu cột trong Database
-- ============================================================================

PRINT N'Bắt đầu vá lỗi Database...';
GO

-- 1. Thêm cột NgayTao vào bảng PHIEUDATSAN (nếu chưa có)
IF NOT EXISTS (
    SELECT 1
    FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_NAME = 'PHIEUDATSAN'
      AND COLUMN_NAME = 'NgayTao'
)
BEGIN
    ALTER TABLE PHIEUDATSAN
    ADD NgayTao DATETIME DEFAULT GETDATE();

    PRINT N'Đã thêm cột NgayTao vào bảng PHIEUDATSAN';
END
ELSE
BEGIN
    PRINT N'Cột NgayTao đã tồn tại trong bảng PHIEUDATSAN';
END
GO

-- 2. Cập nhật dữ liệu cũ: gán NgayTao = NgayDat (tránh NULL gây lỗi web)
UPDATE PHIEUDATSAN
SET NgayTao = CAST(NgayDat AS DATETIME)
WHERE NgayTao IS NULL;

PRINT N'Đã cập nhật NgayTao cho các phiếu cũ';
GO

-- 3. Thêm cột GioMoCua vào bảng COSO (nếu chưa có)
IF NOT EXISTS (
    SELECT 1
    FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_NAME = 'COSO'
      AND COLUMN_NAME = 'GioMoCua'
)
BEGIN
    ALTER TABLE COSO ADD GioMoCua TIME;
    PRINT N'Đã thêm cột GioMoCua vào bảng COSO';
END
ELSE
BEGIN
    PRINT N'Cột GioMoCua đã tồn tại trong bảng COSO';
END
GO

-- 4. Thêm cột GioDongCua vào bảng COSO (nếu chưa có)
IF NOT EXISTS (
    SELECT 1
    FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_NAME = 'COSO'
      AND COLUMN_NAME = 'GioDongCua'
)
BEGIN
    ALTER TABLE COSO ADD GioDongCua TIME;
    PRINT N'Đã thêm cột GioDongCua vào bảng COSO';
END
ELSE
BEGIN
    PRINT N'Cột GioDongCua đã tồn tại trong bảng COSO';
END
GO

-- 5. Gán giờ mở / đóng cửa mặc định cho các cơ sở cũ
UPDATE COSO
SET GioMoCua = '06:00:00',
    GioDongCua = '22:00:00'
WHERE GioMoCua IS NULL
   OR GioDongCua IS NULL;

PRINT N'Đã cập nhật giờ mở / đóng cửa mặc định cho các cơ sở';
GO

PRINT N'Hoàn tất vá lỗi Database. Database đã sẵn sàng.';
GO

-- ============================================================================
-- PATCH: Thanh toán tại quầy – bỏ qua check trùng (emergency fix)
-- ============================================================================

CREATE OR ALTER PROCEDURE sp_ThanhToanTaiQuay
    @MaDatSan BIGINT
AS
BEGIN
    SET NOCOUNT ON;
    SET TRANSACTION ISOLATION LEVEL REPEATABLE READ;

    BEGIN TRY
        BEGIN TRAN;

        DECLARE @TrangThaiHienTai NVARCHAR(50);

        SELECT @TrangThaiHienTai = TrangThai
        FROM PHIEUDATSAN
        WHERE MaDatSan = @MaDatSan;

        -- Bỏ qua kiểm tra trùng lịch (emergency fix)
        IF @TrangThaiHienTai = N'Nháp'
        BEGIN
            UPDATE PHIEUDATSAN
            SET TrangThai = N'Chờ thanh toán',
                NgayTao = GETDATE()
            WHERE MaDatSan = @MaDatSan;
        END

        COMMIT TRAN;
        PRINT N'Chuyển sang trạng thái chờ thanh toán thành công';
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRAN;
        THROW;
    END CATCH
END
GO

-- ============================================================================
-- PATCH: Bỏ qua phiếu NHÁP khi kiểm tra trùng lịch đặt sân
-- ============================================================================

CREATE OR ALTER TRIGGER trg_KiemTraTrungLich
ON DATSAN
FOR INSERT, UPDATE
AS
BEGIN
    IF EXISTS (
        SELECT 1
        FROM inserted I
        JOIN PHIEUDATSAN P_Moi ON I.MaDatSan = P_Moi.MaDatSan
        JOIN DATSAN D_Cu ON I.MaSan = D_Cu.MaSan
        JOIN PHIEUDATSAN P_Cu ON D_Cu.MaDatSan = P_Cu.MaDatSan
        WHERE P_Cu.MaDatSan <> P_Moi.MaDatSan
          AND P_Cu.NgayDat = P_Moi.NgayDat
          AND P_Cu.TrangThai NOT IN (N'Đã hủy', N'No-Show', N'Nháp')
          AND (
              (P_Moi.GioBatDau >= P_Cu.GioBatDau AND P_Moi.GioBatDau < P_Cu.GioKetThuc)
              OR
              (P_Moi.GioKetThuc > P_Cu.GioBatDau AND P_Moi.GioKetThuc <= P_Cu.GioKetThuc)
              OR
              (P_Moi.GioBatDau <= P_Cu.GioBatDau AND P_Moi.GioKetThuc >= P_Cu.GioKetThuc)
          )
    )
    BEGIN
        RAISERROR (
            N'Lỗi: Sân đã bị đặt trùng giờ với một phiếu khác!',
            16, 1
        );
        ROLLBACK TRANSACTION;
    END
END
GO

PRINT N'Hoàn tất cập nhật trigger kiểm tra trùng lịch (đã bỏ qua phiếu Nháp)';
GO
USE TRUNGTAMTHETHAO
GO

-- ============================================================
-- PATCH: Sửa function f_TinhTienSan – Tính giá theo TỪNG KHUNG GIỜ
-- Mục đích: Tính chính xác giá sân khi đặt qua nhiều khung giờ
-- ============================================================

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
    DECLARE @CurrentTime TIME;
    DECLARE @NextTime TIME;
    DECLARE @GiaKhung DECIMAL(18,2);

    -- Lấy thông tin phiếu đặt và loại sân
    SELECT 
        @MaLS = S.MaLS,
        @TenLS = LS.TenLS,
        @GioBatDau = P.GioBatDau,
        @GioKetThuc = P.GioKetThuc,
        @NgayDat = P.NgayDat
    FROM PHIEUDATSAN P
    JOIN DATSAN D ON P.MaDatSan = D.MaDatSan
    JOIN SAN S ON D.MaSan = S.MaSan
    JOIN LOAISAN LS ON S.MaLS = LS.MaLS
    WHERE P.MaDatSan = @MaDatSan;

    -- Duyệt từng đoạn thời gian từ giờ bắt đầu đến giờ kết thúc
    SET @CurrentTime = @GioBatDau;

    WHILE @CurrentTime < @GioKetThuc
    BEGIN
        SET @NextTime = DATEADD(HOUR, 1, @CurrentTime);
        IF @NextTime > @GioKetThuc
            SET @NextTime = @GioKetThuc;

        -- Lấy giá khung giờ hiện tại
        SELECT TOP 1 @GiaKhung = K.GiaApDung
        FROM KHUNGGIO K
        WHERE K.MaLS = @MaLS
          AND @CurrentTime >= K.GioBatDau
          AND @CurrentTime < K.GioKetThuc
          AND K.NgayApDung <= @NgayDat
        ORDER BY K.NgayApDung DESC;

        -- Nếu không có khung phù hợp, lấy khung gần nhất
        IF @GiaKhung IS NULL
        BEGIN
            SELECT TOP 1 @GiaKhung = K.GiaApDung
            FROM KHUNGGIO K
            WHERE K.MaLS = @MaLS
              AND K.NgayApDung <= @NgayDat
            ORDER BY ABS(DATEDIFF(MINUTE, @CurrentTime, K.GioBatDau));
        END

        -- Cộng tiền theo số phút thực tế
        SET @TienSan += ISNULL(@GiaKhung, 0)
                         * DATEDIFF(MINUTE, @CurrentTime, @NextTime) / 60.0;

        SET @CurrentTime = @NextTime;
        SET @GiaKhung = NULL;
    END

    RETURN @TienSan;
END
GO

PRINT N'Hoàn tất cập nhật hàm f_TinhTienSan (tính giá theo từng khung giờ)';
GO

-- ============================================================
-- PATCH: Trigger hoàn trả tồn kho dịch vụ khi HỦY phiếu đặt sân
-- ============================================================

CREATE OR ALTER TRIGGER trg_HoanTraDichVuKhiHuy
ON PHIEUDATSAN
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    -- Chỉ xử lý khi trạng thái chuyển sang HỦY / NO-SHOW
    IF EXISTS (
        SELECT 1
        FROM inserted i
        JOIN deleted d ON i.MaDatSan = d.MaDatSan
        WHERE i.TrangThai IN (N'Đã hủy', N'No-Show')
          AND d.TrangThai NOT IN (N'Đã hủy', N'No-Show', N'Nháp')
    )
    BEGIN
        -- Lưu danh sách phiếu bị hủy
        SELECT i.MaDatSan
        INTO #PhieuBiHuy
        FROM inserted i
        JOIN deleted d ON i.MaDatSan = d.MaDatSan
        WHERE i.TrangThai IN (N'Đã hủy', N'No-Show');

        -- Hoàn trả tồn kho (KHÔNG áp dụng cho HLV, Phòng VIP, Tủ đồ)
        UPDATE DVC
        SET DVC.SoLuongTon = DVC.SoLuongTon + CT.SoLuong
        FROM DV_COSO DVC
        JOIN CT_DICHVUDAT CT ON DVC.MaDV = CT.MaDV
        JOIN DICHVU DV ON CT.MaDV = DV.MaDV
        JOIN LOAIDV LDV ON DV.MaLoaiDV = LDV.MaLoaiDV
        JOIN #PhieuBiHuy P ON CT.MaDatSan = P.MaDatSan
        JOIN DATSAN DS ON P.MaDatSan = DS.MaDatSan
        JOIN SAN S ON DS.MaSan = S.MaSan
        WHERE DVC.MaCS = S.MaCS
          AND LDV.TenLoai NOT IN (N'Huấn luyện viên', N'Phòng VIP', N'Tủ đồ');

        DROP TABLE #PhieuBiHuy;
    END
END
GO

PRINT N'Hoàn tất trigger hoàn trả tồn kho khi hủy phiếu';
GO

-- ============================================================
-- PATCH: Cập nhật sp_DatSan – đảm bảo luôn có NgayTao
-- ============================================================

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
    SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;

    BEGIN TRY
        BEGIN TRAN;

        -- Kiểm tra trùng lịch
        IF dbo.f_KiemTraSanTrong(@MaSan, @NgayDat, @GioBatDau, @GioKetThuc, NULL) = 0
        BEGIN
            ROLLBACK TRAN;
            RAISERROR(N'Sân đã bị người khác đặt!', 16, 1);
            RETURN;
        END

        -- Điều kiện đặt online
        IF @KenhDat = N'Online'
           AND DATEDIFF(HOUR, GETDATE(),
                CAST(@NgayDat AS DATETIME) + CAST(@GioBatDau AS DATETIME)) < 2
        BEGIN
            ROLLBACK TRAN;
            RAISERROR(N'Đặt online phải trước ít nhất 2 tiếng!', 16, 1);
            RETURN;
        END

        -- Tạo phiếu đặt (luôn có NgayTao)
        INSERT INTO PHIEUDATSAN
        (MaKH, NguoiLap, NgayDat, NgayKetThuc, GioBatDau, GioKetThuc,
         KenhDat, TrangThai, NgayTao)
        VALUES
        (@MaKH, @NguoiLap, @NgayDat, @NgayDat, @GioBatDau, @GioKetThuc,
         @KenhDat, N'Nháp', GETDATE());

        DECLARE @MaDatSan BIGINT = SCOPE_IDENTITY();

        INSERT INTO DATSAN (MaDatSan, MaSan)
        VALUES (@MaDatSan, @MaSan);

        COMMIT TRAN;
        PRINT N'Đặt sân thành công. Mã đặt sân: ' + CAST(@MaDatSan AS NVARCHAR);
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRAN;
        THROW;
    END CATCH
END
GO

PRINT N'PATCH HOÀN TẤT: f_TinhTienSan + trigger + sp_DatSan';
GO

-- =============================================
-- Stored Procedure: sp_TuDongHuyDonQuaHan
-- Tự động hủy các đơn đặt sân quá hạn thanh toán (30 phút)
-- =============================================
CREATE OR ALTER PROCEDURE sp_TuDongHuyDonQuaHan
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @SoPhutGioiHan INT = 30;
    
    -- Hủy các booking đang chờ thanh toán và đã quá thời gian giới hạn
    UPDATE PHIEUDATSAN
    SET TrangThai = N'Đã hủy'
    WHERE TrangThai = N'Chờ thanh toán'
      AND DATEDIFF(MINUTE, NgayTao, GETDATE()) > @SoPhutGioiHan;
      
    -- Trả về số lượng đơn đã hủy
    SELECT @@ROWCOUNT AS SoLuongHuy;
END
GO

PRINT N'Đã tạo stored procedure sp_TuDongHuyDonQuaHan';
GO
