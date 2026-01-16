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