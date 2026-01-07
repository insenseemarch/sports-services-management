CREATE DATABASE TRUNGTAMTHETHAO
GO

USE TRUNGTAMTHETHAO
GO

--DROP DATABASE TRUNGTAMTHETHAO;
--USE master 

-- ===================================================================================
-- ==																				==
-- ==					PARTITION FUNCTION & SCHEME									==
-- ==																				==
-- ===================================================================================



-- ===================================================================================
-- ==																				==
-- ==							CREATE TABLE CHƯA FK  								==
-- ==																				==
-- ===================================================================================

-- 1. BẢNG KHÁCH HÀNG 
CREATE TABLE KHACHHANG (
    MaKH VARCHAR(20) PRIMARY KEY,
    HoTen NVARCHAR(100),
    NgaySinh DATE,
    CCCD VARCHAR(20),
    SDT VARCHAR(15),
    Email VARCHAR(100),
    DiaChi NVARCHAR(200),
	LaHSSV BIT,
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
    GioBatDau TIME,
    GioKetThuc TIME,
    KenhDat NVARCHAR(50)
)
GO

-- 3. BẢNG HÓA ĐƠN 
CREATE TABLE HOADON (
    MaHD BIGINT IDENTITY(1,1) PRIMARY KEY,
    MaPhieu BIGINT,
	NguoiLap VARCHAR(20),
    NgayLap DATE NOT NULL,
    TongTien DECIMAL(18,2),
    GiamGia DECIMAL(18,2),
    ThanhTien DECIMAL(18,2),
    HinhThucTT NVARCHAR(50),
)
GO

-- 4. BẢNG CA TRỰC
CREATE TABLE CATRUC (
    MaCaTruc BIGINT IDENTITY(1,1) PRIMARY KEY,
    MaNV VARCHAR(20),
    NgayTruc DATE NOT NULL,
    GioBatDau TIME,
    GioKetThuc TIME,
    PhuCap DECIMAL(18,2),
)
GO

-- 5. BẢNG BÁO CÁO THÔNG KÊ 
CREATE TABLE BAOCAOTHONGKE (
    MaBaoCao BIGINT IDENTITY(1,1) PRIMARY KEY,
    NgayLap DATE NOT NULL,
    DoanhThu DECIMAL(18,2),
    SoLuongDatOnline INT,
    SoLuongDatTrucTiep INT,
)
GO

-- 6. BẢNG CẤP BẬC
CREATE TABLE CAPBAC (
    MaCB VARCHAR(20) PRIMARY KEY,
    TenCB NVARCHAR(100),
    UuDai DECIMAL(5,2)
)
GO

-- 7. BẢNG TÀI KHOẢN 
CREATE TABLE TAIKHOAN (
    MaTK VARCHAR(20) PRIMARY KEY,
    TenDangNhap VARCHAR(50) UNIQUE,
    MatKhau VARCHAR(255),
    VaiTro NVARCHAR(50),
    NgayDangKy DATE
)
GO

-- 8. BẢNG CƠ SỞ
CREATE TABLE COSO (
    MaCS VARCHAR(20) PRIMARY KEY,
    TenCS NVARCHAR(100),
    DiaChi NVARCHAR(200),
    ThanhPho NVARCHAR(100)
)
GO

-- 9. BẢNG LOẠI SÂN
CREATE TABLE LOAISAN (
    MaLS VARCHAR(20) PRIMARY KEY,
    TenLS NVARCHAR(100),
    DVT NVARCHAR(20)
)
GO

-- 10. BẢNG SÂN
CREATE TABLE SAN (
    MaSan VARCHAR(20) PRIMARY KEY,
    MaLS VARCHAR(20),
    MaCS VARCHAR(20),
    SucChua INT,
    TinhTrang NVARCHAR(50)
)
GO

-- 11. BẢNG LOẠI DỊCH VỤ
CREATE TABLE LOAIDV (
    MaLoaiDV VARCHAR(20) PRIMARY KEY,
    TenLoai NVARCHAR(100)
)
GO

-- 12. BẢNG DỊCH VỤ 
CREATE TABLE DICHVU (
    MaDV VARCHAR(20) PRIMARY KEY,
    MaCS VARCHAR(20),
    MaLoaiDV VARCHAR(20),
    DonGia DECIMAL(18,2),
    DVT NVARCHAR(20),
    TinhTrang NVARCHAR(50)
)
GO

-- 13. BẢNG NHÂN VIÊN
CREATE TABLE NHANVIEN (
    MaNV VARCHAR(20) PRIMARY KEY,
    MaCS VARCHAR(20),
    HoTen NVARCHAR(100),
    NgaySinh DATE,
    GioiTinh NVARCHAR(10),
    CMND_CCCD VARCHAR(20),
    DiaChi NVARCHAR(200),
    SDT VARCHAR(15),
    ChucVu NVARCHAR(50),
    LuongCoBan DECIMAL(18,2),
    MaLuong VARCHAR(20),
    MaTK VARCHAR(20)
)
GO

-- 14. BẢNG LƯƠNG
CREATE TABLE LUONG (
    MaLuong VARCHAR(20) PRIMARY KEY,
    TongGioLam INT,
    PhuCap DECIMAL(18,2),
    ThuLao DECIMAL(18,2),
    HoaHong DECIMAL(18,2),
    TienPhat DECIMAL(18,2)
)
GO

-- 15. BẢNG ĐƠN NGHỈ PHÉP
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

-- 16. BẢNG PHIẾU BẢO TRÌ
CREATE TABLE PHIEUBAOTRI (
    MaPhieu BIGINT IDENTITY(1,1) PRIMARY KEY,
    MaNV VARCHAR(20),
    MaSan VARCHAR(20),
    NgayBatDau DATE,
    NgayKetThucDuKien DATE,
    NgayKetThucThucTe DATE,
    MoTaSuCo NVARCHAR(MAX),
    ChiPhi DECIMAL(18,2),
    TrangThai NVARCHAR(50)
)
GO


-- 17. BẢNG DỊCH VỤ - CƠ SỞ
CREATE TABLE DV_COSO (
    MaDV VARCHAR(20),
    MaCS VARCHAR(20),
    CONSTRAINT PK_DV_COSO PRIMARY KEY (MaDV, MaCS)
)
GO

-- 18. BẢNG ĐẶT SÂN (PHEUDATSAN - SAN)
CREATE TABLE DATSAN (
    MaDatSan BIGINT,
    MaSan VARCHAR(20),
    CONSTRAINT PK_DATSAN PRIMARY KEY (MaDatSan, MaSan)
)
GO

-- 19. BẢNG CHI TIẾT DỊCH VỤ ĐẶT (DICHVU -  PHIEUDATSAN)
CREATE TABLE CT_DICHVUDAT (
    MaDV VARCHAR(20),
    MaDatSan BIGINT,
    SoLuong INT,
    SoGioThue INT,
    ThanhTien DECIMAL(18,2),
    TrangThaiSuDung NVARCHAR(50),
    GhiChu NVARCHAR(200),

    CONSTRAINT PK_CT_DICHVUDAT PRIMARY KEY (MaDV, MaDatSan)
)
GO

-- 20. BẢNG THAM GIA CA TRỰC (NHANVIEN - CATRUC)
CREATE TABLE THAMGIACATRUC (
    MaCaTruc BIGINT,
    MaNV VARCHAR(20),
    CONSTRAINT PK_THAMGIACATRUC PRIMARY KEY (MaCaTruc, MaNV)
)
GO

-- 21. BẢNG HUẤN LUYỆN VIÊN 
CREATE TABLE HLV (
    MaHLV VARCHAR(20) PRIMARY KEY,
    MaDV VARCHAR(20),
    ChuyenMon NVARCHAR(100),
    MucGia DECIMAL(18,2)
)
GO

-- 22. BẢNG ƯU ĐÃI
CREATE TABLE UUDAI (
    MaUD VARCHAR(20) PRIMARY KEY,
    TenCT NVARCHAR(100),
    TyLeGiamGia DECIMAL(5,2),
    DieuKienApDung NVARCHAR(200)
)
GO

-- 23. BẢNG ƯU ĐÃI - ÁP DỤNG (UUDAI - HOADON)
CREATE TABLE UUDAI_APDUNG (
    MaUD VARCHAR(20),
    MaHD BIGINT,
    CONSTRAINT PK_UUDAI_APDUNG PRIMARY KEY (MaUD, MaHD)
)
GO

-- 24. BẢNG KHUNG GIỜ
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
FOREIGN KEY (MaCS) REFERENCES COSO(MaCS),
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
FOREIGN KEY (MaHD) REFERENCES HOADON(MaHD)
GO

-- DV_COSO
ALTER TABLE DV_COSO ADD
FOREIGN KEY (MaDV) REFERENCES DICHVU(MaDV), 
FOREIGN KEY (MaCS) REFERENCES COSO(MaCS)

-- HLV
ALTER TABLE HLV
ADD CONSTRAINT FK_HLV_DICHVU
FOREIGN KEY (MaDV) REFERENCES DICHVU(MaDV)

-- KHUNGGIO
ALTER TABLE KHUNGGIO ADD
CONSTRAINT FK_KHUNGGIO_LOAISAN
FOREIGN KEY (MaLS) REFERENCES LOAISAN(MaLS)
GO

-- KHACHHANG
ALTER TABLE KHACHHANG ADD
FOREIGN KEY (MaCB) REFERENCES CAPBAC(MaCB), 
FOREIGN KEY (MaTK) REFERENCES TAIKHOAN(MaTK)


-- ===================================================================================
-- ==																				==
-- ==									INDEX   									==
-- ==																				==
-- ===================================================================================




-- ===================================================================================
-- ==																				==
-- ==									DATA    									==
-- ==																				==
-- ===================================================================================

-- drop database TRUNGTAMTHETHAO
-- use master

USE TRUNGTAMTHETHAO
GO

-- 1. BẢNG CƠ SỞ (2 Cơ sở: 1 ở trung tâm, 1 ở ngoại ô)
INSERT INTO COSO (MaCS, TenCS, DiaChi, ThanhPho) VALUES
('CS01', N'TTTT Quận 1', N'123 Nguyễn Huệ, P. Bến Nghé, Quận 1', N'TP. Hồ Chí Minh'),
('CS02', N'TTTT Thủ Đức', N'456 Võ Văn Ngân, P. Bình Thọ, TP. Thủ Đức', N'TP. Hồ Chí Minh'), 
('CS03', N'TTTT Quận 7', N'101 Nguyễn Văn Linh, P. Tân Phong, Quận 7', N'TP. Hồ Chí Minh'),
('CS04', N'TTTT Gò Vấp', N'22 Quang Trung, P. 10, Q. Gò Vấp', N'TP. Hồ Chí Minh'),
('CS05', N'TTTT Bình Thạnh', N'33 Xô Viết Nghệ Tĩnh, P. 25, Q. Bình Thạnh', N'TP. Hồ Chí Minh');
GO

-- 2. BẢNG LOẠI SÂN (Các môn thể thao phổ biến)
INSERT INTO LOAISAN (MaLS, TenLS, DVT) VALUES
('LS01', N'Sân Bóng Đá 5 Người', N'Giờ'),
('LS02', N'Sân Bóng Đá 7 Người', N'Giờ'),
('LS03', N'Sân Cầu Lông', N'Giờ'),
('LS04', N'Sân Tennis', N'Giờ'),
('LS05', N'Sân Bóng Rổ', N'Giờ');
GO

-- 3. BẢNG LOẠI DỊCH VỤ
INSERT INTO LOAIDV (MaLoaiDV, TenLoai) VALUES
('LDV01', N'Thức uống & Đồ ăn nhẹ'),
('LDV02', N'Dụng cụ thi đấu'),
('LDV03', N'Huấn luyện viên'),
('LDV04', N'Gửi xe & Tủ đồ');
GO

-- 4. BẢNG CẤP BẬC (Membership)
INSERT INTO CAPBAC (MaCB, TenCB, UuDai) VALUES
('CB01', N'Đồng (Bronze)', 0.00),
('CB02', N'Bạc (Silver)', 0.05), -- Giảm 5%
('CB03', N'Vàng (Gold)', 0.10),   -- Giảm 10%
('CB04', N'Kim Cương (Diamond)', 0.15); -- Giảm 15%
GO

-- 5. BẢNG LƯƠNG (Theo logic Bậc lương cố định như bạn yêu cầu)
-- Quy ước: L1-L3: Nhân viên, L4-L5: Quản lý/Chuyên gia
INSERT INTO LUONG (MaLuong, TongGioLam, PhuCap, ThuLao, HoaHong, TienPhat) VALUES
('L01', 160, 500000, 5000000, 0, 0),    -- NV Mới/Thử việc
('L02', 176, 1000000, 7000000, 200000, 0), -- NV Chính thức
('L03', 176, 1500000, 9000000, 500000, 0), -- NV Thâm niên
('L04', 176, 2000000, 15000000, 1000000, 0), -- Quản lý ca
('L05', 176, 3000000, 25000000, 2000000, 0); -- Quản lý cơ sở
GO

-- 6. BẢNG ƯU ĐÃI (Mã giảm giá)
INSERT INTO UUDAI (MaUD, TenCT, TyLeGiamGia, DieuKienApDung) VALUES
('UD00', N'Không áp dụng', 0.00, N'Mặc định'),
('UD01', N'Chào hè 2024', 0.10, N'Áp dụng T6-T8/2024'),
('UD02', N'Khai trương CS02', 0.20, N'Áp dụng tại CS02'),
('UD03', N'Khách hàng thân thiết', 0.05, N'Khách đặt trên 10 lần'),
('UD04', N'Happy Hour', 0.15, N'Khung giờ 09:00 - 14:00'),
('UD05', N'Sinh nhật khách hàng', 0.20, N'Có CMND trùng ngày sinh');
GO

-- 7. BẢNG TÀI KHOẢN
-- Sinh 50 tài khoản: 1 Admin, 10 Nhân viên, 39 Khách hàng
-- Mật khẩu chung: 123456
INSERT INTO TAIKHOAN (MaTK, TenDangNhap, MatKhau, VaiTro, NgayDangKy) VALUES
('TK00', 'admin', '123456', 'Admin', '2023-01-01'),
-- Tài khoản nhân viên (TK01 -> TK10)
('TK01', 'nhanvien01', '123456', 'NhanVien', '2023-05-01'),
('TK02', 'nhanvien02', '123456', 'NhanVien', '2023-05-02'),
('TK03', 'nhanvien03', '123456', 'NhanVien', '2023-06-01'),
('TK04', 'nhanvien04', '123456', 'NhanVien', '2023-06-15'),
('TK05', 'nhanvien05', '123456', 'NhanVien', '2023-07-01'),
('TK06', 'nhanvien06', '123456', 'NhanVien', '2023-07-20'),
('TK07', 'nhanvien07', '123456', 'NhanVien', '2023-08-01'),
('TK08', 'nhanvien08', '123456', 'NhanVien', '2023-09-01'),
('TK09', 'nhanvien09', '123456', 'NhanVien', '2023-10-01'),
('TK10', 'nhanvien10', '123456', 'NhanVien', '2023-11-01'),
-- Tài khoản khách hàng (TK11 -> TK30 đại diện)
('TK11', 'khachhang01', '123456', 'KhachHang', '2024-01-02'),
('TK12', 'khachhang02', '123456', 'KhachHang', '2024-01-05'),
('TK13', 'khachhang03', '123456', 'KhachHang', '2024-01-10'),
('TK14', 'khachhang04', '123456', 'KhachHang', '2024-02-01'),
('TK15', 'khachhang05', '123456', 'KhachHang', '2024-02-14'),
('TK16', 'khachhang06', '123456', 'KhachHang', '2024-02-20'),
('TK17', 'khachhang07', '123456', 'KhachHang', '2024-03-01'),
('TK18', 'khachhang08', '123456', 'KhachHang', '2024-03-05'),
('TK19', 'khachhang09', '123456', 'KhachHang', '2024-03-10'),
('TK20', 'khachhang10', '123456', 'KhachHang', '2024-03-15'),
('TK21', 'khachhang11', '123456', 'KhachHang', '2024-04-01'),
('TK22', 'khachhang12', '123456', 'KhachHang', '2024-04-05'),
('TK23', 'khachhang13', '123456', 'KhachHang', '2024-04-10'),
('TK24', 'khachhang14', '123456', 'KhachHang', '2024-04-15'),
('TK25', 'khachhang15', '123456', 'KhachHang', '2024-05-01'),
('TK26', 'khachhang16', '123456', 'KhachHang', '2024-05-05'),
('TK27', 'khachhang17', '123456', 'KhachHang', '2024-05-10'),
('TK28', 'khachhang18', '123456', 'KhachHang', '2024-06-01'),
('TK29', 'khachhang19', '123456', 'KhachHang', '2024-06-15'),
('TK30', 'khachhang20', '123456', 'KhachHang', '2024-07-01'), 
('TK31', 'khachhang21', '123456', 'KhachHang', '2024-08-01'),
('TK32', 'khachhang22', '123456', 'KhachHang', '2024-08-02'),
('TK33', 'khachhang23', '123456', 'KhachHang', '2024-08-05'),
('TK34', 'khachhang24', '123456', 'KhachHang', '2024-08-10'),
('TK35', 'khachhang25', '123456', 'KhachHang', '2024-09-01'),
('TK36', 'khachhang26', '123456', 'KhachHang', '2024-09-15'),
('TK37', 'khachhang27', '123456', 'KhachHang', '2024-10-01'),
('TK38', 'khachhang28', '123456', 'KhachHang', '2024-10-20'),
('TK39', 'khachhang29', '123456', 'KhachHang', '2024-11-01'),
('TK40', 'khachhang30', '123456', 'KhachHang', '2024-11-11');
GO

-- 8. BẢNG NHÂN VIÊN (15 NV, rải đều 5 cơ sở)
INSERT INTO NHANVIEN (MaNV, MaCS, HoTen, NgaySinh, GioiTinh, CMND_CCCD, DiaChi, SDT, ChucVu, LuongCoBan, MaLuong, MaTK) VALUES
('NV01', 'CS01', N'Nguyễn Văn An', '1990-01-15', N'Nam', '079190000001', N'Q1, TP.HCM', '0909000001', N'Quản lý', 15000000, 'L04', 'TK01'),
('NV02', 'CS01', N'Trần Thị Bích', '1995-05-20', N'Nữ', '079195000002', N'Q3, TP.HCM', '0909000002', N'Thu ngân', 7000000, 'L02', 'TK02'),
('NV03', 'CS01', N'Lê Văn Cường', '1998-08-10', N'Nam', '079198000003', N'Q4, TP.HCM', '0909000003', N'Nhân viên sân', 5000000, 'L01', 'TK03'),
('NV04', 'CS02', N'Phạm Minh Duy', '1992-02-28', N'Nam', '079192000004', N'Thủ Đức, TP.HCM', '0909000004', N'Quản lý', 15000000, 'L04', 'TK04'),
('NV05', 'CS02', N'Hoàng Thu E', '1996-11-12', N'Nữ', '079196000005', N'Thủ Đức, TP.HCM', '0909000005', N'Thu ngân', 7000000, 'L02', 'TK05'),
('NV06', 'CS03', N'Đặng Văn F', '1993-06-15', N'Nam', '079193000006', N'Q7, TP.HCM', '0909000006', N'Quản lý', 15000000, 'L04', 'TK06'),
('NV07', 'CS03', N'Bùi Thị G', '1997-09-22', N'Nữ', '079197000007', N'Nhà Bè, TP.HCM', '0909000007', N'Thu ngân', 7000000, 'L02', 'TK07'),
('NV08', 'CS04', N'Vũ Văn H', '1991-03-30', N'Nam', '079191000008', N'Gò Vấp, TP.HCM', '0909000008', N'Quản lý', 15000000, 'L04', 'TK08'),
('NV09', 'CS04', N'Ngô Thị I', '1999-12-05', N'Nữ', '079199000009', N'Q12, TP.HCM', '0909000009', N'Thu ngân', 7000000, 'L02', 'TK09'),
('NV10', 'CS05', N'Đỗ Văn K', '1989-07-19', N'Nam', '079189000010', N'Bình Thạnh, TP.HCM', '0909000010', N'Quản lý', 25000000, 'L05', 'TK10'),
-- Nhân viên không có tài khoản hệ thống
('NV11', 'CS02', N'Lý Văn L', '1985-04-12', N'Nam', '079185000011', N'Thủ Đức, TP.HCM', '0909000011', N'Bảo vệ', 5000000, 'L01', NULL),
('NV12', 'CS03', N'Trương Thị M', '1980-08-25', N'Nữ', '079180000012', N'Q7, TP.HCM', '0909000012', N'Tạp vụ', 5000000, 'L01', NULL),
('NV13', 'CS04', N'Đinh Văn N', '1994-01-30', N'Nam', '079194000013', N'Gò Vấp, TP.HCM', '0909000013', N'Kỹ thuật', 9000000, 'L03', NULL),
('NV14', 'CS05', N'Lâm Thị O', '2000-10-10', N'Nữ', '079200000014', N'Bình Thạnh, TP.HCM', '0909000014', N'Phục vụ', 5000000, 'L01', NULL),
('NV15', 'CS05', N'Mai Văn P', '1998-02-14', N'Nam', '079198000015', N'Bình Thạnh, TP.HCM', '0909000015', N'Nhân viên sân', 5000000, 'L01', NULL),
('NV16', 'CS01', N'Trần Văn Thời', '2001-01-01', N'Nam', '070201000016', N'Q1, TP.HCM', '0901000016', N'Nhặt bóng', 4000000, 'L01', NULL),
('NV17', 'CS02', N'Lê Thị Vụ', '2002-02-02', N'Nữ', '070202000017', N'Thủ Đức', '0901000017', N'Tạp vụ', 4500000, 'L01', NULL),
('NV18', 'CS03', N'Phạm Văn Bảo', '1985-05-05', N'Nam', '070185000018', N'Q7', '0901000018', N'Bảo vệ', 6000000, 'L01', NULL),
('NV19', 'CS04', N'Nguyễn Thị Bếp', '1990-09-09', N'Nữ', '070190000019', N'Gò Vấp', '0901000019', N'Pha chế', 7000000, 'L02', NULL),
('NV20', 'CS05', N'Hoàng Văn Kỹ', '1993-03-03', N'Nam', '070193000020', N'Bình Thạnh', '0901000020', N'Kỹ thuật viên', 8000000, 'L03', NULL);
GO

-- 9. BẢNG KHÁCH HÀNG (20 KH, Map với TK11 -> TK30)
INSERT INTO KHACHHANG (MaKH, HoTen, NgaySinh, CCCD, SDT, Email, DiaChi, LaHSSV, MaCB, MaTK) VALUES
('KH01', N'Nguyễn Thị Khách A', '2002-01-01', '012345678901', '0912000001', 'kha@gmail.com', N'Q1, TP.HCM', 1, 'CB01', 'TK11'),
('KH02', N'Trần Văn Khách B', '1995-05-15', '012345678902', '0912000002', 'khb@gmail.com', N'Q2, TP.HCM', 0, 'CB02', 'TK12'),
('KH03', N'Lê Thị Khách C', '1990-11-20', '012345678903', '0912000003', 'khc@gmail.com', N'Q3, TP.HCM', 0, 'CB03', 'TK13'),
('KH04', N'Phạm Văn Khách D', '1988-03-10', '012345678904', '0912000004', 'khd@gmail.com', N'Q4, TP.HCM', 0, 'CB01', 'TK14'),
('KH05', N'Hoàng Thị Khách E', '2003-07-25', '012345678905', '0912000005', 'khe@gmail.com', N'Q5, TP.HCM', 1, 'CB01', 'TK15'),
('KH06', N'Vũ Văn Khách F', '1992-09-30', '012345678906', '0912000006', 'khf@gmail.com', N'Q6, TP.HCM', 0, 'CB04', 'TK16'),
('KH07', N'Đặng Thị Khách G', '1999-12-12', '012345678907', '0912000007', 'khg@gmail.com', N'Q7, TP.HCM', 0, 'CB02', 'TK17'),
('KH08', N'Bùi Văn Khách H', '1985-04-05', '012345678908', '0912000008', 'khh@gmail.com', N'Q8, TP.HCM', 0, 'CB03', 'TK18'),
('KH09', N'Đỗ Thị Khách I', '2001-06-18', '012345678909', '0912000009', 'khi@gmail.com', N'Q9, TP.HCM', 1, 'CB01', 'TK19'),
('KH10', N'Ngô Văn Khách K', '1996-08-22', '012345678910', '0912000010', 'khk@gmail.com', N'Q10, TP.HCM', 0, 'CB02', 'TK20'),
('KH11', N'Lý Thị Khách L', '1994-02-14', '012345678911', '0912000011', 'khl@gmail.com', N'Q11, TP.HCM', 0, 'CB01', 'TK21'),
('KH12', N'Mai Văn Khách M', '1991-10-30', '012345678912', '0912000012', 'khm@gmail.com', N'Q12, TP.HCM', 0, 'CB01', 'TK22'),
('KH13', N'Trương Thị Khách N', '2004-05-05', '012345678913', '0912000013', 'khn@gmail.com', N'Bình Thạnh, TP.HCM', 1, 'CB01', 'TK23'),
('KH14', N'Lâm Văn Khách O', '1989-11-11', '012345678914', '0912000014', 'kho@gmail.com', N'Gò Vấp, TP.HCM', 0, 'CB02', 'TK24'),
('KH15', N'Đoàn Thị Khách P', '1997-03-20', '012345678915', '0912000015', 'khp@gmail.com', N'Tân Bình, TP.HCM', 0, 'CB03', 'TK25'),
('KH16', N'Hồ Văn Khách Q', '1993-07-07', '012345678916', '0912000016', 'khq@gmail.com', N'Phú Nhuận, TP.HCM', 0, 'CB01', 'TK26'),
('KH17', N'Dương Thị Khách R', '2000-09-09', '012345678917', '0912000017', 'khr@gmail.com', N'Thủ Đức, TP.HCM', 0, 'CB01', 'TK27'),
('KH18', N'Cao Văn Khách S', '1987-12-25', '012345678918', '0912000018', 'khs@gmail.com', N'Bình Chánh, TP.HCM', 0, 'CB02', 'TK28'),
('KH19', N'Phan Thị Khách T', '2002-04-30', '012345678919', '0912000019', 'kht@gmail.com', N'Hóc Môn, TP.HCM', 1, 'CB01', 'TK29'),
('KH20', N'Vương Văn Khách U', '1995-01-15', '012345678920', '0912000020', 'khu@gmail.com', N'Củ Chi, TP.HCM', 0, 'CB01', 'TK30'),
('KH21', N'Trịnh Văn V', '1998-01-01', '079198000021', '0912000021', 'khv@gmail.com', N'Q1, TP.HCM', 0, 'CB01', 'TK31'),
('KH22', N'Đinh Thị X', '2000-05-05', '079200000022', '0912000022', 'khx@gmail.com', N'Q3, TP.HCM', 1, 'CB02', 'TK32'),
('KH23', N'La Văn Y', '1995-10-10', '079195000023', '0912000023', 'khy@gmail.com', N'Q5, TP.HCM', 0, 'CB01', 'TK33'),
('KH24', N'Mạc Thị Z', '1990-12-12', '079190000024', '0912000024', 'khz@gmail.com', N'Thủ Đức, TP.HCM', 0, 'CB03', 'TK34'),
('KH25', N'Lưu Văn Thanh', '1988-02-02', '079188000025', '0912000025', 'kht@gmail.com', N'Bình Thạnh, TP.HCM', 0, 'CB01', 'TK35'),
('KH26', N'Quách Thị Mận', '2003-03-03', '079203000026', '0912000026', 'khm@gmail.com', N'Gò Vấp, TP.HCM', 1, 'CB01', 'TK36'),
('KH27', N'Tiêu Phong', '1992-07-07', '079192000027', '0912000027', 'khp@gmail.com', N'Q7, TP.HCM', 0, 'CB04', 'TK37'),
('KH28', N'Hư Trúc', '1994-08-08', '079194000028', '0912000028', 'khht@gmail.com', N'Q8, TP.HCM', 0, 'CB02', 'TK38'),
('KH29', N'Đoàn Dự', '1996-09-09', '079196000029', '0912000029', 'khdd@gmail.com', N'Q10, TP.HCM', 0, 'CB03', 'TK39'),
('KH30', N'Vương Ngữ Yên', '1999-11-11', '079199000030', '0912000030', 'khvny@gmail.com', N'Q1, TP.HCM', 1, 'CB02', 'TK40');
GO

-- 10. BẢNG SÂN (Sinh 24 sân cho 5 cơ sở)
-- CS01: 2 bóng đá, 1 cầu lông
-- CS02: 2 bóng đá, 1 tennis
-- CS03: 1 bóng đá, 2 tennis, 1 bóng rổ
-- CS04: 3 cầu lông, 1 bóng đá
-- CS05: 2 bóng đá, 2 cầu lông
INSERT INTO SAN (MaSan, MaLS, MaCS, SucChua, TinhTrang) VALUES
('S0101', 'LS01', 'CS01', 10, N'Hoạt động'), -- Bóng đá 5
('S0102', 'LS01', 'CS01', 10, N'Hoạt động'),
('S0103', 'LS03', 'CS01', 4, N'Hoạt động'),  -- Cầu lông
('S0201', 'LS01', 'CS02', 10, N'Hoạt động'),
('S0202', 'LS02', 'CS02', 14, N'Bảo trì'),    -- Bóng đá 7 (Đang bảo trì)
('S0203', 'LS04', 'CS02', 4, N'Hoạt động'),   -- Tennis
('S0301', 'LS01', 'CS03', 10, N'Hoạt động'),
('S0302', 'LS04', 'CS03', 4, N'Hoạt động'),
('S0303', 'LS04', 'CS03', 4, N'Hoạt động'),
('S0304', 'LS05', 'CS03', 10, N'Hoạt động'),  -- Bóng rổ
('S0401', 'LS03', 'CS04', 4, N'Hoạt động'),
('S0402', 'LS03', 'CS04', 4, N'Hoạt động'),
('S0403', 'LS03', 'CS04', 4, N'Sửa chữa'),
('S0404', 'LS01', 'CS04', 10, N'Hoạt động'),
('S0501', 'LS01', 'CS05', 10, N'Hoạt động'),
('S0502', 'LS01', 'CS05', 10, N'Hoạt động'),
('S0503', 'LS03', 'CS05', 4, N'Hoạt động'),
('S0504', 'LS03', 'CS05', 4, N'Hoạt động'),
('S0104', 'LS05', 'CS01', 10, N'Hoạt động'),
('S0204', 'LS02', 'CS02', 14, N'Hoạt động'),
('S0305', 'LS01', 'CS03', 10, N'Hoạt động'),
('S0405', 'LS01', 'CS04', 10, N'Hoạt động'),
('S0505', 'LS04', 'CS05', 4, N'Hoạt động'),
('S0506', 'LS04', 'CS05', 4, N'Hoạt động');
GO

-- 11. BẢNG DỊCH VỤ 
INSERT INTO DICHVU (MaDV, MaCS, MaLoaiDV, DonGia, DVT, TinhTrang) VALUES
('DV01', 'CS01', 'LDV01', 10000, N'Chai', N'Còn hàng'),  -- Nước suối
('DV02', 'CS01', 'LDV02', 50000, N'Cái', N'Còn hàng'),   -- Thuê vợt
('DV03', 'CS02', 'LDV01', 12000, N'Chai', N'Còn hàng'),  -- Nước ngọt
('DV04', 'CS02', 'LDV04', 5000, N'Lượt', N'Còn hàng'),   -- Gửi xe
('DV05', 'CS03', 'LDV03', 200000, N'Giờ', N'Còn hàng'),  -- Thuê HLV
('DV06', 'CS03', 'LDV01', 15000, N'Chai', N'Còn hàng'),
('DV07', 'CS04', 'LDV02', 30000, N'Quả', N'Còn hàng'),   -- Thuê bóng
('DV08', 'CS05', 'LDV04', 0, N'Lượt', N'Miễn phí'),      -- Gửi xe MP
('DV09', 'CS01', 'LDV03', 300000, N'Giờ', N'Còn hàng'),  -- HLV VIP
('DV10', 'CS05', 'LDV01', 10000, N'Chai', N'Còn hàng'), 
('DV11', 'CS01', 'LDV01', 15000, N'Lon', N'Còn hàng'),  -- Redbull
('DV12', 'CS01', 'LDV01', 12000, N'Chai', N'Còn hàng'),  -- Revive
('DV13', 'CS01', 'LDV02', 20000, N'Đôi', N'Còn hàng'),   -- Thuê giày
('DV14', 'CS02', 'LDV02', 10000, N'Cái', N'Còn hàng'),   -- Thuê khăn
('DV15', 'CS02', 'LDV01', 25000, N'Ly', N'Còn hàng'),    -- Cà phê đá
('DV16', 'CS03', 'LDV02', 50000, N'Bộ', N'Còn hàng'),    -- Thuê đồng phục
('DV17', 'CS03', 'LDV01', 10000, N'Cái', N'Còn hàng'),   -- Trứng luộc
('DV18', 'CS04', 'LDV02', 15000, N'Đôi', N'Còn hàng'),   -- Găng tay thủ môn
('DV19', 'CS04', 'LDV01', 15000, N'Trái', N'Còn hàng'),  -- Dừa tươi
('DV20', 'CS05', 'LDV02', 30000, N'Cái', N'Còn hàng'),   -- Băng gối
('DV21', 'CS05', 'LDV01', 20000, N'Chai', N'Còn hàng'),  -- Pocari Sweat
('DV22', 'CS01', 'LDV04', 5000, N'Lượt', N'Còn hàng'),   -- Giữ xe máy
('DV23', 'CS01', 'LDV04', 30000, N'Lượt', N'Còn hàng'),  -- Giữ ô tô
('DV24', 'CS02', 'LDV03', 500000, N'Buổi', N'Còn hàng'), -- HLV kèm riêng 1-1
('DV25', 'CS03', 'LDV03', 1500000, N'Tháng', N'Còn hàng');-- Gói HLV tháng
GO

-- 12. BẢNG DỊCH VỤ - CƠ SỞ (Phân phối dịch vụ cho các cơ sở khác)
-- Giả sử DV01 cũng có ở CS02, CS03...
INSERT INTO DV_COSO (MaDV, MaCS) VALUES
('DV01', 'CS02'), ('DV01', 'CS03'), ('DV01', 'CS04'), ('DV01', 'CS05'),
('DV02', 'CS04'), ('DV02', 'CS05'),
('DV04', 'CS01'), ('DV04', 'CS04');
GO

-- 13. BẢNG HUẤN LUYỆN VIÊN (Liên kết với Dịch vụ loại HLV - DV05, DV09)
INSERT INTO HLV (MaHLV, MaDV, ChuyenMon, MucGia) VALUES
('HLV01', 'DV05', N'Bóng đá căn bản', 200000),
('HLV02', 'DV05', N'Tennis nâng cao', 250000),
('HLV03', 'DV09', N'Gym & Fitness', 300000),
('HLV04', 'DV05', N'Cầu lông', 150000),
('HLV05', 'DV09', N'Bóng rổ', 280000),
('HLV06', 'DV05', N'Yoga thể thao', 200000),
('HLV07', 'DV05', N'Phục hồi chấn thương', 300000),
('HLV08', 'DV09', N'Cardio', 250000),
('HLV09', 'DV05', N'Bóng đá thiếu nhi', 150000),
('HLV10', 'DV05', N'Bơi lội', 200000),
('HLV11', 'DV09', N'Thể lực nâng cao', 350000),
('HLV12', 'DV05', N'Tennis cơ bản', 200000),
('HLV13', 'DV05', N'Cầu lông nâng cao', 220000),
('HLV14', 'DV24', N'Bóng rổ chuyên nghiệp', 500000),
('HLV15', 'DV24', N'Võ thuật tự vệ', 450000),
('HLV16', 'DV24', N'Gym Personal Trainer', 400000),
('HLV17', 'DV25', N'Dance Sport', 300000),
('HLV18', 'DV25', N'Zumba', 150000),
('HLV19', 'DV05', N'Pilates', 350000),
('HLV20', 'DV05', N'Aerobic', 120000);
GO

-- 14. BẢNG KHUNG GIỜ (Giá sân theo giờ)
INSERT INTO KHUNGGIO (MaKG, MaLS, GioBatDau, GioKetThuc, NgayApDung, GiaApDung) VALUES
('KG01', 'LS01', '05:00', '16:00', '2024-01-01', 200000), -- Bóng đá sáng
('KG02', 'LS01', '16:00', '22:00', '2024-01-01', 350000), -- Bóng đá tối (đắt hơn)
('KG03', 'LS03', '05:00', '17:00', '2024-01-01', 60000),  -- Cầu lông sáng
('KG04', 'LS03', '17:00', '22:00', '2024-01-01', 90000),  -- Cầu lông tối
('KG05', 'LS04', '05:00', '22:00', '2024-01-01', 150000), -- Tennis (đồng giá)
('KG06', 'LS02', '05:00', '22:00', '2024-01-01', 500000); -- Sân 7 người
GO

-- 1. DỮ LIỆU ĐẶT SÂN - DỊCH VỤ - HÓA ĐƠN
-- (Lưu ý: Vì MaDatSan là tự tăng, nên ta giả định các lệnh Insert chạy tuần tự sẽ sinh ID từ 1->30)

-- Giao dịch 1
INSERT INTO PHIEUDATSAN (MaKH, NguoiLap, NgayDat, NgayKetThuc, GioBatDau, GioKetThuc, KenhDat) VALUES ('KH01', 'NV01', '2024-01-10', '2024-01-10', '17:00:00', '19:00:00', N'Trực tiếp');
INSERT INTO DATSAN (MaDatSan, MaSan) VALUES (1, 'S0101'); -- Sân bóng
INSERT INTO CT_DICHVUDAT (MaDV, MaDatSan, SoLuong, SoGioThue, ThanhTien, TrangThaiSuDung, GhiChu) VALUES ('DV01', 1, 5, 0, 50000, N'Đã dùng', NULL); -- Nước suối
INSERT INTO HOADON (MaPhieu, NguoiLap, NgayLap, TongTien, GiamGia, ThanhTien, HinhThucTT) VALUES (1, 'NV01', '2024-01-10', 450000, 0, 450000, N'Tiền mặt');

-- Giao dịch 2 (Có áp dụng ưu đãi)
INSERT INTO PHIEUDATSAN (MaKH, NguoiLap, NgayDat, NgayKetThuc, GioBatDau, GioKetThuc, KenhDat) VALUES ('KH05', 'NV02', '2024-02-14', '2024-02-14', '09:00:00', '11:00:00', N'Online');
INSERT INTO DATSAN (MaDatSan, MaSan) VALUES (2, 'S0302'); -- Sân Tennis
INSERT INTO HOADON (MaPhieu, NguoiLap, NgayLap, TongTien, GiamGia, ThanhTien, HinhThucTT) VALUES (2, 'NV02', '2024-02-14', 300000, 30000, 270000, N'Chuyển khoản');
INSERT INTO UUDAI_APDUNG (MaUD, MaHD) VALUES ('UD01', 2);

-- Giao dịch 3
INSERT INTO PHIEUDATSAN (MaKH, NguoiLap, NgayDat, NgayKetThuc, GioBatDau, GioKetThuc, KenhDat) VALUES ('KH10', 'NV03', '2024-03-20', '2024-03-20', '18:00:00', '20:00:00', N'Điện thoại');
INSERT INTO DATSAN (MaDatSan, MaSan) VALUES (3, 'S0201');
INSERT INTO CT_DICHVUDAT (MaDV, MaDatSan, SoLuong, SoGioThue, ThanhTien, TrangThaiSuDung, GhiChu) VALUES ('DV02', 3, 2, 0, 100000, N'Đã dùng', N'Thuê vợt');
INSERT INTO HOADON (MaPhieu, NguoiLap, NgayLap, TongTien, GiamGia, ThanhTien, HinhThucTT) VALUES (3, 'NV03', '2024-03-20', 500000, 0, 500000, N'Tiền mặt');

-- Giao dịch 4
INSERT INTO PHIEUDATSAN (MaKH, NguoiLap, NgayDat, NgayKetThuc, GioBatDau, GioKetThuc, KenhDat) VALUES ('KH02', 'NV04', '2024-04-30', '2024-04-30', '08:00:00', '10:00:00', N'Trực tiếp');
INSERT INTO DATSAN (MaDatSan, MaSan) VALUES (4, 'S0401'); -- Cầu lông
INSERT INTO HOADON (MaPhieu, NguoiLap, NgayLap, TongTien, GiamGia, ThanhTien, HinhThucTT) VALUES (4, 'NV04', '2024-04-30', 120000, 12000, 108000, N'Tiền mặt');
INSERT INTO UUDAI_APDUNG (MaUD, MaHD) VALUES ('UD03', 4);

-- Giao dịch 5 (Thuê HLV)
INSERT INTO PHIEUDATSAN (MaKH, NguoiLap, NgayDat, NgayKetThuc, GioBatDau, GioKetThuc, KenhDat) VALUES ('KH15', 'NV05', '2024-05-15', '2024-05-15', '15:00:00', '17:00:00', N'Online');
INSERT INTO DATSAN (MaDatSan, MaSan) VALUES (5, 'S0501');
INSERT INTO CT_DICHVUDAT (MaDV, MaDatSan, SoLuong, SoGioThue, ThanhTien, TrangThaiSuDung, GhiChu) VALUES ('DV05', 5, 1, 2, 400000, N'Đã dùng', N'Thuê HLV 2h');
INSERT INTO HOADON (MaPhieu, NguoiLap, NgayLap, TongTien, GiamGia, ThanhTien, HinhThucTT) VALUES (5, 'NV05', '2024-05-15', 800000, 0, 800000, N'Chuyển khoản');

-- Giao dịch 6
INSERT INTO PHIEUDATSAN (MaKH, NguoiLap, NgayDat, NgayKetThuc, GioBatDau, GioKetThuc, KenhDat) VALUES ('KH08', 'NV06', '2024-06-01', '2024-06-01', '19:00:00', '21:00:00', N'Trực tiếp');
INSERT INTO DATSAN (MaDatSan, MaSan) VALUES (6, 'S0102');
INSERT INTO HOADON (MaPhieu, NguoiLap, NgayLap, TongTien, GiamGia, ThanhTien, HinhThucTT) VALUES (6, 'NV06', '2024-06-01', 700000, 0, 700000, N'Tiền mặt');

-- Giao dịch 7
INSERT INTO PHIEUDATSAN (MaKH, NguoiLap, NgayDat, NgayKetThuc, GioBatDau, GioKetThuc, KenhDat) VALUES ('KH12', 'NV07', '2024-07-20', '2024-07-20', '06:00:00', '08:00:00', N'Online');
INSERT INTO DATSAN (MaDatSan, MaSan) VALUES (7, 'S0304'); -- Bóng rổ
INSERT INTO CT_DICHVUDAT (MaDV, MaDatSan, SoLuong, SoGioThue, ThanhTien, TrangThaiSuDung, GhiChu) VALUES ('DV01', 7, 10, 0, 100000, N'Đã dùng', N'Nước đội bóng');
INSERT INTO HOADON (MaPhieu, NguoiLap, NgayLap, TongTien, GiamGia, ThanhTien, HinhThucTT) VALUES (7, 'NV07', '2024-07-20', 500000, 50000, 450000, N'Tiền mặt');
INSERT INTO UUDAI_APDUNG (MaUD, MaHD) VALUES ('UD04', 7);

-- Giao dịch 8
INSERT INTO PHIEUDATSAN (MaKH, NguoiLap, NgayDat, NgayKetThuc, GioBatDau, GioKetThuc, KenhDat) VALUES ('KH18', 'NV08', '2024-09-02', '2024-09-02', '16:00:00', '18:00:00', N'Trực tiếp');
INSERT INTO DATSAN (MaDatSan, MaSan) VALUES (8, 'S0204');
INSERT INTO HOADON (MaPhieu, NguoiLap, NgayLap, TongTien, GiamGia, ThanhTien, HinhThucTT) VALUES (8, 'NV08', '2024-09-02', 400000, 0, 400000, N'Tiền mặt');

-- Giao dịch 9 
INSERT INTO PHIEUDATSAN (MaKH, NguoiLap, NgayDat, NgayKetThuc, GioBatDau, GioKetThuc, KenhDat) VALUES ('KH03', 'NV09', '2024-11-20', '2024-11-20', '17:00:00', '19:00:00', N'Online');
INSERT INTO DATSAN (MaDatSan, MaSan) VALUES (9, 'S0404');
INSERT INTO HOADON (MaPhieu, NguoiLap, NgayLap, TongTien, GiamGia, ThanhTien, HinhThucTT) VALUES (9, 'NV09', '2024-11-20', 400000, 0, 400000, N'Chuyển khoản');

-- Giao dịch 10
INSERT INTO PHIEUDATSAN (MaKH, NguoiLap, NgayDat, NgayKetThuc, GioBatDau, GioKetThuc, KenhDat) VALUES ('KH09', 'NV10', '2024-12-25', '2024-12-25', '08:00:00', '10:00:00', N'Điện thoại');
INSERT INTO DATSAN (MaDatSan, MaSan) VALUES (10, 'S0505');
INSERT INTO HOADON (MaPhieu, NguoiLap, NgayLap, TongTien, GiamGia, ThanhTien, HinhThucTT) VALUES (10, 'NV10', '2024-12-25', 300000, 60000, 240000, N'Tiền mặt');
INSERT INTO UUDAI_APDUNG (MaUD, MaHD) VALUES ('UD05', 10);

-- Giao dịch 11 
INSERT INTO PHIEUDATSAN (MaKH, NguoiLap, NgayDat, NgayKetThuc, GioBatDau, GioKetThuc, KenhDat) VALUES ('KH04', 'NV01', '2025-01-01', '2025-01-01', '14:00:00', '16:00:00', N'Trực tiếp');
INSERT INTO DATSAN (MaDatSan, MaSan) VALUES (11, 'S0104');
INSERT INTO HOADON (MaPhieu, NguoiLap, NgayLap, TongTien, GiamGia, ThanhTien, HinhThucTT) VALUES (11, 'NV01', '2025-01-01', 300000, 0, 300000, N'Tiền mặt');

-- Giao dịch 12
INSERT INTO PHIEUDATSAN (MaKH, NguoiLap, NgayDat, NgayKetThuc, GioBatDau, GioKetThuc, KenhDat) VALUES ('KH20', 'NV02', '2025-02-14', '2025-02-14', '18:00:00', '20:00:00', N'Online');
INSERT INTO DATSAN (MaDatSan, MaSan) VALUES (12, 'S0302');
INSERT INTO HOADON (MaPhieu, NguoiLap, NgayLap, TongTien, GiamGia, ThanhTien, HinhThucTT) VALUES (12, 'NV02', '2025-02-14', 300000, 0, 300000, N'Chuyển khoản');

-- Giao dịch 13
INSERT INTO PHIEUDATSAN (MaKH, NguoiLap, NgayDat, NgayKetThuc, GioBatDau, GioKetThuc, KenhDat) VALUES ('KH11', 'NV03', '2025-03-08', '2025-03-08', '09:00:00', '11:00:00', N'Trực tiếp');
INSERT INTO DATSAN (MaDatSan, MaSan) VALUES (13, 'S0203');
INSERT INTO HOADON (MaPhieu, NguoiLap, NgayLap, TongTien, GiamGia, ThanhTien, HinhThucTT) VALUES (13, 'NV03', '2025-03-08', 300000, 30000, 270000, N'Tiền mặt');
INSERT INTO UUDAI_APDUNG (MaUD, MaHD) VALUES ('UD01', 13);

-- Giao dịch 14
INSERT INTO PHIEUDATSAN (MaKH, NguoiLap, NgayDat, NgayKetThuc, GioBatDau, GioKetThuc, KenhDat) VALUES ('KH07', 'NV04', '2025-04-30', '2025-04-30', '15:00:00', '17:00:00', N'Online');
INSERT INTO DATSAN (MaDatSan, MaSan) VALUES (14, 'S0402');
INSERT INTO HOADON (MaPhieu, NguoiLap, NgayLap, TongTien, GiamGia, ThanhTien, HinhThucTT) VALUES (14, 'NV04', '2025-04-30', 120000, 0, 120000, N'Tiền mặt');

-- Giao dịch 15
INSERT INTO PHIEUDATSAN (MaKH, NguoiLap, NgayDat, NgayKetThuc, GioBatDau, GioKetThuc, KenhDat) VALUES ('KH16', 'NV05', '2025-05-19', '2025-05-19', '07:00:00', '09:00:00', N'Trực tiếp');
INSERT INTO DATSAN (MaDatSan, MaSan) VALUES (15, 'S0503');
INSERT INTO HOADON (MaPhieu, NguoiLap, NgayLap, TongTien, GiamGia, ThanhTien, HinhThucTT) VALUES (15, 'NV05', '2025-05-19', 120000, 0, 120000, N'Tiền mặt');

-- Giao dịch 16
INSERT INTO PHIEUDATSAN (MaKH, NguoiLap, NgayDat, NgayKetThuc, GioBatDau, GioKetThuc, KenhDat) VALUES ('KH21', 'NV01', '2025-06-01', '2025-06-01', '17:00:00', '19:00:00', N'Online');
INSERT INTO DATSAN (MaDatSan, MaSan) VALUES (16, 'S0101');
INSERT INTO CT_DICHVUDAT (MaDV, MaDatSan, SoLuong, SoGioThue, ThanhTien, TrangThaiSuDung, GhiChu) VALUES ('DV11', 16, 10, 0, 150000, N'Đã dùng', N'Nước đội');
INSERT INTO HOADON (MaPhieu, NguoiLap, NgayLap, TongTien, GiamGia, ThanhTien, HinhThucTT) VALUES (16, 'NV01', '2025-06-01', 550000, 0, 550000, N'Momo');

-- Giao dịch 17
INSERT INTO PHIEUDATSAN (MaKH, NguoiLap, NgayDat, NgayKetThuc, GioBatDau, GioKetThuc, KenhDat) VALUES ('KH22', 'NV02', '2025-06-10', '2025-06-10', '05:00:00', '07:00:00', N'Trực tiếp');
INSERT INTO DATSAN (MaDatSan, MaSan) VALUES (17, 'S0203');
INSERT INTO HOADON (MaPhieu, NguoiLap, NgayLap, TongTien, GiamGia, ThanhTien, HinhThucTT) VALUES (17, 'NV02', '2025-06-10', 300000, 0, 300000, N'Tiền mặt');

-- Giao dịch 18
INSERT INTO PHIEUDATSAN (MaKH, NguoiLap, NgayDat, NgayKetThuc, GioBatDau, GioKetThuc, KenhDat) VALUES ('KH23', 'NV04', '2025-06-15', '2025-06-15', '18:00:00', '20:00:00', N'Điện thoại');
INSERT INTO DATSAN (MaDatSan, MaSan) VALUES (18, 'S0401');
INSERT INTO CT_DICHVUDAT (MaDV, MaDatSan, SoLuong, SoGioThue, ThanhTien, TrangThaiSuDung, GhiChu) VALUES ('DV02', 18, 2, 0, 100000, N'Đã dùng', N'Thuê 2 vợt');
INSERT INTO HOADON (MaPhieu, NguoiLap, NgayLap, TongTien, GiamGia, ThanhTien, HinhThucTT) VALUES (18, 'NV04', '2025-06-15', 280000, 28000, 252000, N'Tiền mặt');
INSERT INTO UUDAI_APDUNG (MaUD, MaHD) VALUES ('UD01', 18);

-- Giao dịch 19
INSERT INTO PHIEUDATSAN (MaKH, NguoiLap, NgayDat, NgayKetThuc, GioBatDau, GioKetThuc, KenhDat) VALUES ('KH24', 'NV05', '2025-06-20', '2025-06-20', '19:00:00', '20:00:00', N'Online');
INSERT INTO DATSAN (MaDatSan, MaSan) VALUES (19, 'S0505');
INSERT INTO HOADON (MaPhieu, NguoiLap, NgayLap, TongTien, GiamGia, ThanhTien, HinhThucTT) VALUES (19, 'NV05', '2025-06-20', 150000, 0, 150000, N'QR Code');

-- Giao dịch 20
INSERT INTO PHIEUDATSAN (MaKH, NguoiLap, NgayDat, NgayKetThuc, GioBatDau, GioKetThuc, KenhDat) VALUES ('KH25', 'NV06', '2025-07-01', '2025-07-01', '08:00:00', '10:00:00', N'Trực tiếp');
INSERT INTO DATSAN (MaDatSan, MaSan) VALUES (20, 'S0301');
INSERT INTO CT_DICHVUDAT (MaDV, MaDatSan, SoLuong, SoGioThue, ThanhTien, TrangThaiSuDung, GhiChu) VALUES ('DV24', 20, 1, 2, 500000, N'Đã dùng', N'HLV kèm 2h');
INSERT INTO HOADON (MaPhieu, NguoiLap, NgayLap, TongTien, GiamGia, ThanhTien, HinhThucTT) VALUES (20, 'NV06', '2025-07-01', 900000, 90000, 810000, N'Thẻ tín dụng');
INSERT INTO UUDAI_APDUNG (MaUD, MaHD) VALUES ('UD03', 20);

-- Giao dịch 21
INSERT INTO PHIEUDATSAN (MaKH, NguoiLap, NgayDat, NgayKetThuc, GioBatDau, GioKetThuc, KenhDat) VALUES ('KH26', 'NV07', '2025-07-05', '2025-07-05', '16:00:00', '18:00:00', N'Online');
INSERT INTO DATSAN (MaDatSan, MaSan) VALUES (21, 'S0102');
INSERT INTO HOADON (MaPhieu, NguoiLap, NgayLap, TongTien, GiamGia, ThanhTien, HinhThucTT) VALUES (21, 'NV07', '2025-07-05', 400000, 0, 400000, N'Tiền mặt');

-- Giao dịch 22
INSERT INTO PHIEUDATSAN (MaKH, NguoiLap, NgayDat, NgayKetThuc, GioBatDau, GioKetThuc, KenhDat) VALUES ('KH27', 'NV08', '2025-07-15', '2025-07-15', '17:00:00', '19:00:00', N'Điện thoại');
INSERT INTO DATSAN (MaDatSan, MaSan) VALUES (22, 'S0201');
INSERT INTO CT_DICHVUDAT (MaDV, MaDatSan, SoLuong, SoGioThue, ThanhTien, TrangThaiSuDung, GhiChu) VALUES ('DV11', 22, 5, 0, 75000, N'Đã dùng', NULL);
INSERT INTO HOADON (MaPhieu, NguoiLap, NgayLap, TongTien, GiamGia, ThanhTien, HinhThucTT) VALUES (22, 'NV08', '2025-07-15', 475000, 0, 475000, N'Tiền mặt');

-- Giao dịch 23
INSERT INTO PHIEUDATSAN (MaKH, NguoiLap, NgayDat, NgayKetThuc, GioBatDau, GioKetThuc, KenhDat) VALUES ('KH28', 'NV09', '2025-08-01', '2025-08-01', '06:00:00', '08:00:00', N'Trực tiếp');
INSERT INTO DATSAN (MaDatSan, MaSan) VALUES (23, 'S0304');
INSERT INTO HOADON (MaPhieu, NguoiLap, NgayLap, TongTien, GiamGia, ThanhTien, HinhThucTT) VALUES (23, 'NV09', '2025-08-01', 300000, 0, 300000, N'Tiền mặt');

-- Giao dịch 24
INSERT INTO PHIEUDATSAN (MaKH, NguoiLap, NgayDat, NgayKetThuc, GioBatDau, GioKetThuc, KenhDat) VALUES ('KH29', 'NV10', '2025-08-10', '2025-08-10', '20:00:00', '22:00:00', N'Online');
INSERT INTO DATSAN (MaDatSan, MaSan) VALUES (24, 'S0404');
INSERT INTO CT_DICHVUDAT (MaDV, MaDatSan, SoLuong, SoGioThue, ThanhTien, TrangThaiSuDung, GhiChu) VALUES ('DV18', 24, 1, 0, 15000, N'Đã dùng', N'Mua găng tay');
INSERT INTO HOADON (MaPhieu, NguoiLap, NgayLap, TongTien, GiamGia, ThanhTien, HinhThucTT) VALUES (24, 'NV10', '2025-08-10', 415000, 0, 415000, N'Momo');

-- Giao dịch 25
INSERT INTO PHIEUDATSAN (MaKH, NguoiLap, NgayDat, NgayKetThuc, GioBatDau, GioKetThuc, KenhDat) VALUES ('KH30', 'NV01', '2025-08-20', '2025-08-20', '09:00:00', '11:00:00', N'Trực tiếp');
INSERT INTO DATSAN (MaDatSan, MaSan) VALUES (25, 'S0501');
INSERT INTO HOADON (MaPhieu, NguoiLap, NgayLap, TongTien, GiamGia, ThanhTien, HinhThucTT) VALUES (25, 'NV01', '2025-08-20', 400000, 40000, 360000, N'Tiền mặt');
INSERT INTO UUDAI_APDUNG (MaUD, MaHD) VALUES ('UD02', 25);

-- Giao dịch 26
INSERT INTO PHIEUDATSAN (MaKH, NguoiLap, NgayDat, NgayKetThuc, GioBatDau, GioKetThuc, KenhDat) VALUES ('KH01', 'NV02', '2025-09-01', '2025-09-01', '15:00:00', '16:00:00', N'Điện thoại');
INSERT INTO DATSAN (MaDatSan, MaSan) VALUES (26, 'S0103');
INSERT INTO HOADON (MaPhieu, NguoiLap, NgayLap, TongTien, GiamGia, ThanhTien, HinhThucTT) VALUES (26, 'NV02', '2025-09-01', 60000, 0, 60000, N'Tiền mặt');

-- Giao dịch 27
INSERT INTO PHIEUDATSAN (MaKH, NguoiLap, NgayDat, NgayKetThuc, GioBatDau, GioKetThuc, KenhDat) VALUES ('KH05', 'NV03', '2025-09-05', '2025-09-05', '10:00:00', '12:00:00', N'Online');
INSERT INTO DATSAN (MaDatSan, MaSan) VALUES (27, 'S0202');
INSERT INTO HOADON (MaPhieu, NguoiLap, NgayLap, TongTien, GiamGia, ThanhTien, HinhThucTT) VALUES (27, 'NV03', '2025-09-05', 1000000, 100000, 900000, N'Chuyển khoản');
INSERT INTO UUDAI_APDUNG (MaUD, MaHD) VALUES ('UD05', 27);

-- Giao dịch 28
INSERT INTO PHIEUDATSAN (MaKH, NguoiLap, NgayDat, NgayKetThuc, GioBatDau, GioKetThuc, KenhDat) VALUES ('KH10', 'NV04', '2025-09-20', '2025-09-20', '18:00:00', '21:00:00', N'Trực tiếp');
INSERT INTO DATSAN (MaDatSan, MaSan) VALUES (28, 'S0305');
INSERT INTO CT_DICHVUDAT (MaDV, MaDatSan, SoLuong, SoGioThue, ThanhTien, TrangThaiSuDung, GhiChu) VALUES ('DV12', 28, 6, 0, 72000, N'Đã dùng', NULL);
INSERT INTO HOADON (MaPhieu, NguoiLap, NgayLap, TongTien, GiamGia, ThanhTien, HinhThucTT) VALUES (28, 'NV04', '2025-09-20', 672000, 0, 672000, N'Tiền mặt');

-- Giao dịch 29
INSERT INTO PHIEUDATSAN (MaKH, NguoiLap, NgayDat, NgayKetThuc, GioBatDau, GioKetThuc, KenhDat) VALUES ('KH15', 'NV05', '2025-10-10', '2025-10-10', '06:00:00', '07:00:00', N'Online');
INSERT INTO DATSAN (MaDatSan, MaSan) VALUES (29, 'S0403');
INSERT INTO HOADON (MaPhieu, NguoiLap, NgayLap, TongTien, GiamGia, ThanhTien, HinhThucTT) VALUES (29, 'NV05', '2025-10-10', 60000, 0, 60000, N'Tiền mặt');

-- Giao dịch 30
INSERT INTO PHIEUDATSAN (MaKH, NguoiLap, NgayDat, NgayKetThuc, GioBatDau, GioKetThuc, KenhDat) VALUES ('KH20', 'NV06', '2025-10-25', '2025-10-25', '19:00:00', '21:00:00', N'Điện thoại');
INSERT INTO DATSAN (MaDatSan, MaSan) VALUES (30, 'S0506');
INSERT INTO HOADON (MaPhieu, NguoiLap, NgayLap, TongTien, GiamGia, ThanhTien, HinhThucTT) VALUES (30, 'NV06', '2025-10-25', 300000, 0, 300000, N'Momo');

-- Giao dịch 31
INSERT INTO PHIEUDATSAN (MaKH, NguoiLap, NgayDat, NgayKetThuc, GioBatDau, GioKetThuc, KenhDat) VALUES ('KH21', 'NV07', '2025-11-11', '2025-11-11', '17:00:00', '19:00:00', N'Trực tiếp');
INSERT INTO DATSAN (MaDatSan, MaSan) VALUES (31, 'S0104');
INSERT INTO HOADON (MaPhieu, NguoiLap, NgayLap, TongTien, GiamGia, ThanhTien, HinhThucTT) VALUES (31, 'NV07', '2025-11-11', 300000, 30000, 270000, N'Tiền mặt');
INSERT INTO UUDAI_APDUNG (MaUD, MaHD) VALUES ('UD01', 31);

-- Giao dịch 32
INSERT INTO PHIEUDATSAN (MaKH, NguoiLap, NgayDat, NgayKetThuc, GioBatDau, GioKetThuc, KenhDat) VALUES ('KH22', 'NV08', '2025-11-20', '2025-11-20', '14:00:00', '16:00:00', N'Online');
INSERT INTO DATSAN (MaDatSan, MaSan) VALUES (32, 'S0204');
INSERT INTO CT_DICHVUDAT (MaDV, MaDatSan, SoLuong, SoGioThue, ThanhTien, TrangThaiSuDung, GhiChu) VALUES ('DV03', 32, 4, 0, 48000, N'Đã dùng', NULL);
INSERT INTO HOADON (MaPhieu, NguoiLap, NgayLap, TongTien, GiamGia, ThanhTien, HinhThucTT) VALUES (32, 'NV08', '2025-11-20', 448000, 0, 448000, N'Chuyển khoản');

-- Giao dịch 33
INSERT INTO PHIEUDATSAN (MaKH, NguoiLap, NgayDat, NgayKetThuc, GioBatDau, GioKetThuc, KenhDat) VALUES ('KH25', 'NV09', '2025-12-01', '2025-12-01', '18:00:00', '19:00:00', N'Trực tiếp');
INSERT INTO DATSAN (MaDatSan, MaSan) VALUES (33, 'S0303');
INSERT INTO HOADON (MaPhieu, NguoiLap, NgayLap, TongTien, GiamGia, ThanhTien, HinhThucTT) VALUES (33, 'NV09', '2025-12-01', 150000, 0, 150000, N'Tiền mặt');

-- Giao dịch 34
INSERT INTO PHIEUDATSAN (MaKH, NguoiLap, NgayDat, NgayKetThuc, GioBatDau, GioKetThuc, KenhDat) VALUES ('KH30', 'NV10', '2025-12-24', '2025-12-24', '20:00:00', '22:00:00', N'Online');
INSERT INTO DATSAN (MaDatSan, MaSan) VALUES (34, 'S0405');
INSERT INTO CT_DICHVUDAT (MaDV, MaDatSan, SoLuong, SoGioThue, ThanhTien, TrangThaiSuDung, GhiChu) VALUES ('DV19', 34, 2, 0, 30000, N'Đã dùng', N'Dừa tươi');
INSERT INTO HOADON (MaPhieu, NguoiLap, NgayLap, TongTien, GiamGia, ThanhTien, HinhThucTT) VALUES (34, 'NV10', '2025-12-24', 430000, 0, 430000, N'Momo');

-- Giao dịch 35
INSERT INTO PHIEUDATSAN (MaKH, NguoiLap, NgayDat, NgayKetThuc, GioBatDau, GioKetThuc, KenhDat) VALUES ('KH08', 'NV01', '2025-12-31', '2025-12-31', '21:00:00', '22:00:00', N'Trực tiếp');
INSERT INTO DATSAN (MaDatSan, MaSan) VALUES (35, 'S0502');
INSERT INTO HOADON (MaPhieu, NguoiLap, NgayLap, TongTien, GiamGia, ThanhTien, HinhThucTT) VALUES (35, 'NV01', '2025-12-31', 200000, 0, 200000, N'Tiền mặt');
GO

-- 2. DỮ LIỆU CA TRỰC & THAM GIA CA TRỰC
-- (Sinh 10 ca trực mẫu)
INSERT INTO CATRUC (MaNV, NgayTruc, GioBatDau, GioKetThuc, PhuCap) VALUES
('NV01', '2024-01-10', '06:00:00', '14:00:00', 100000), -- Ca: 1
('NV04', '2024-01-10', '14:00:00', '22:00:00', 100000), -- Ca: 2
('NV06', '2024-01-11', '06:00:00', '14:00:00', 100000), -- Ca: 3
('NV08', '2024-01-11', '14:00:00', '22:00:00', 100000), -- Ca: 4
('NV10', '2024-01-12', '06:00:00', '14:00:00', 100000), -- Ca: 5
('NV01', '2024-05-01', '06:00:00', '14:00:00', 150000), -- Ca 6 (Lễ tăng phụ cấp)
('NV04', '2024-05-01', '14:00:00', '22:00:00', 150000), -- Ca 7
('NV06', '2024-06-15', '06:00:00', '14:00:00', 100000), -- Ca 8
('NV08', '2024-06-15', '14:00:00', '22:00:00', 100000), -- Ca 9
('NV10', '2024-07-20', '06:00:00', '14:00:00', 100000), -- Ca 10
('NV01', '2024-08-15', '14:00:00', '22:00:00', 100000), -- Ca 11
('NV04', '2024-09-02', '06:00:00', '14:00:00', 150000), -- Ca 12 (Lễ)
('NV06', '2024-10-10', '14:00:00', '22:00:00', 100000), -- Ca 13
('NV08', '2024-11-20', '06:00:00', '14:00:00', 100000), -- Ca 14
('NV10', '2024-12-24', '14:00:00', '22:00:00', 120000); -- Ca 15 (Noel)

-- Tham gia ca trực (Mỗi ca thêm 2 nhân viên phụ)
INSERT INTO THAMGIACATRUC (MaCaTruc, MaNV) VALUES
(1, 'NV02'), (1, 'NV03'),
(2, 'NV05'), (2, 'NV11'),
(3, 'NV07'), (3, 'NV12'),
(4, 'NV09'), (4, 'NV13'),
(5, 'NV01'), (5, 'NV14'), 
(6, 'NV02'), (6, 'NV16'),
(7, 'NV05'), (7, 'NV17'),
(8, 'NV07'), (8, 'NV18'),
(9, 'NV09'), (9, 'NV19'),
(10, 'NV03'), (10, 'NV20'),
(11, 'NV02'), (11, 'NV11'),
(12, 'NV05'), (12, 'NV12'),
(13, 'NV07'), (13, 'NV13'),
(14, 'NV09'), (14, 'NV14'),
(15, 'NV03'), (15, 'NV15');


-- 3. DỮ LIỆU BẢO TRÌ SÂN
INSERT INTO PHIEUBAOTRI (MaNV, MaSan, NgayBatDau, NgayKetThucDuKien, NgayKetThucThucTe, MoTaSuCo, ChiPhi, TrangThai) VALUES
('NV01', 'S0101', '2024-07-01', '2024-07-02', '2024-07-02', N'Thay bóng đèn cao áp', 1200000, N'Hoàn thành'),
('NV04', 'S0201', '2024-08-05', '2024-08-07', '2024-08-07', N'Vẽ lại vạch sân', 800000, N'Hoàn thành'),
('NV06', 'S0301', '2024-09-10', '2024-09-15', '2024-09-14', N'Sửa chữa mái che khán đài', 5500000, N'Hoàn thành'),
('NV08', 'S0401', '2024-10-01', '2024-10-01', '2024-10-01', N'Thay lưới cầu lông', 300000, N'Hoàn thành'),
('NV10', 'S0501', '2024-11-20', '2024-11-25', NULL, N'Bảo dưỡng mặt cỏ định kỳ', 4000000, N'Đang thực hiện'),
('NV01', 'S0102', '2024-12-05', '2024-12-06', '2024-12-06', N'Sửa vòi nước khu vệ sinh', 450000, N'Hoàn thành'),
('NV04', 'S0203', '2025-01-15', '2025-01-16', '2025-01-16', N'Căng lại lưới Tennis', 600000, N'Hoàn thành'),
('NV06', 'S0304', '2025-02-20', '2025-02-22', '2025-02-22', N'Sơn lại cột bóng rổ', 1500000, N'Hoàn thành'),
('NV08', 'S0404', '2025-03-10', '2025-03-12', NULL, N'Chống thấm trần nhà thi đấu', 3000000, N'Đang chờ vật tư'),
('NV10', 'S0502', '2025-04-01', '2025-04-05', NULL, N'Thay thế ghế ngồi khán giả hỏng', 2500000, N'Đang thực hiện');


-- 4. DỮ LIỆU NGHỈ PHÉP
INSERT INTO DONNGHIPHEP (MaNV, NgayNghi, CaNghi, LyDo, NgayDuyet, TrangThai, NguoiThayThe) VALUES
('NV02', '2024-02-20', 1, N'Việc gia đình', '2024-02-18', N'Đã duyệt', 'NV03'),
('NV05', '2024-05-01', 2, N'Về quê', '2024-04-28', N'Đã duyệt', 'NV04'),
('NV09', '2024-08-15', 3, N'Ốm', NULL, N'Chờ duyệt', 'NV07'),
('NV02', '2024-05-01', 6, N'Nghỉ lễ về quê', '2024-04-25', N'Đã duyệt', 'NV03'),
('NV05', '2024-05-01', 7, N'Bị sốt xuất huyết', NULL, N'Chờ duyệt', 'NV09'),
('NV07', '2024-06-15', 8, N'Đi khám bệnh', '2024-06-14', N'Đã duyệt', 'NV02'),
('NV09', '2024-06-15', 9, N'Con ốm', '2024-06-15', N'Đã duyệt', 'NV05'),
('NV03', '2024-07-20', 10, N'Đám cưới bạn thân', '2024-07-10', N'Đã duyệt', 'NV07'),
('NV11', '2024-08-15', 11, N'Việc riêng gia đình', '2024-08-12', N'Đã duyệt', 'NV12'),
('NV12', '2024-09-02', 12, N'Đi du lịch', '2024-08-25', N'Đã duyệt', 'NV11'),
('NV13', '2024-10-10', 13, N'Đau răng', '2024-10-09', N'Đã duyệt', 'NV14'),
('NV14', '2024-11-20', 14, N'Về thăm trường cũ', '2024-11-15', N'Từ chối', NULL),
('NV15', '2024-12-24', 15, N'Đi chơi Noel', NULL, N'Chờ duyệt', 'NV13');


-- 5. BÁO CÁO THỐNG KÊ (Dữ liệu tổng hợp giả định)
INSERT INTO BAOCAOTHONGKE (NgayLap, DoanhThu, SoLuongDatOnline, SoLuongDatTrucTiep) VALUES
('2024-01-31', 150000000, 120, 80),
('2024-02-29', 180000000, 150, 60),
('2024-03-31', 160000000, 130, 90),
('2024-04-30', 200000000, 180, 100),
('2024-05-31', 220000000, 200, 110),
('2024-06-30', 250000000, 230, 120),
('2024-07-31', 240000000, 220, 115),
('2024-08-31', 210000000, 190, 100),
('2024-09-30', 190000000, 170, 90),
('2024-10-31', 180000000, 160, 85),
('2024-11-30', 200000000, 185, 95),
('2024-12-31', 280000000, 250, 130),
('2025-01-31', 160000000, 140, 70),
('2025-02-28', 170000000, 150, 75);
GO

-- ===================================================================================
-- ==																				==
-- ==								CHECK DATA 	 									==
-- ==																				==
-- ===================================================================================

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
GO