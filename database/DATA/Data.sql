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
INSERT INTO KHUNGGIO (MaKG, MaLS, GioBatDau, GioKetThuc, NgayApDung, GiaApDung, LoaiNgay, TenKhungGio, TrangThai) VALUES
-- Bóng đá mini
('KG001', 'LS001', '06:00:00', '08:00:00', '2024-01-01', 400000, N'Thường', N'Sáng sớm', N'Đang áp dụng'),
('KG002', 'LS001', '08:00:00', '10:00:00', '2024-01-01', 500000, N'Thường', N'Sáng', N'Đang áp dụng'),
('KG003', 'LS001', '10:00:00', '14:00:00', '2024-01-01', 450000, N'Thường', N'Trưa', N'Đang áp dụng'),
('KG004', 'LS001', '14:00:00', '17:00:00', '2024-01-01', 600000, N'Thường', N'Chiều', N'Đang áp dụng'),
('KG005', 'LS001', '17:00:00', '22:00:00', '2024-01-01', 750000, N'Thường', N'Tối', N'Đang áp dụng'),
-- Tennis
('KG006', 'LS002', '06:00:00', '10:00:00', '2024-01-01', 200000, N'Thường', N'Sáng', N'Đang áp dụng'),
('KG007', 'LS002', '10:00:00', '14:00:00', '2024-01-01', 250000, N'Thường', N'Trưa', N'Đang áp dụng'),
('KG008', 'LS002', '14:00:00', '18:00:00', '2024-01-01', 300000, N'Thường', N'Chiều', N'Đang áp dụng'),
('KG009', 'LS002', '18:00:00', '22:00:00', '2024-01-01', 350000, N'Thường', N'Tối', N'Đang áp dụng'),
-- Cầu lông
('KG010', 'LS003', '06:00:00', '09:00:00', '2024-01-01', 80000, N'Thường', N'Sáng', N'Đang áp dụng'),
('KG011', 'LS003', '09:00:00', '12:00:00', '2024-01-01', 100000, N'Thường', N'Trưa', N'Đang áp dụng'),
('KG012', 'LS003', '12:00:00', '17:00:00', '2024-01-01', 90000, N'Thường', N'Chiều', N'Đang áp dụng'),
('KG013', 'LS003', '17:00:00', '22:00:00', '2024-01-01', 150000, N'Thường', N'Tối', N'Đang áp dụng'),
-- Bóng rổ
('KG014', 'LS004', '08:00:00', '12:00:00', '2024-01-01', 150000, N'Thường', N'Sáng', N'Đang áp dụng'),
('KG015', 'LS004', '14:00:00', '18:00:00', '2024-01-01', 180000, N'Thường', N'Chiều', N'Đang áp dụng'),
('KG016', 'LS004', '18:00:00', '22:00:00', '2024-01-01', 200000, N'Thường', N'Tối', N'Đang áp dụng');
GO

-- 13. NẠP DATA VÀO BẢNG ƯU ĐÃI
INSERT INTO UUDAI (MaUD, TenCT, TyLeGiamGia, DieuKienApDung, LoaiUuDai, NgayBatDau, NgayKetThuc, GiaTriToiThieu, SoGioToiThieu, TrangThai) VALUES
('UD001', N'Giảm giá sinh viên', 10.00, N'Xuất trình thẻ HSSV', N'Đối tượng', '2024-01-01', '2026-01-01', 0, 0, N'Đang áp dụng'),
('UD002', N'Khuyến mãi đầu tuần', 8.00, N'Đặt sân từ thứ 2 đến thứ 4', N'Thời gian', '2024-01-01', '2026-01-01', 0, 0, N'Đang áp dụng'),
('UD003', N'Ưu đãi thành viên Vàng', 15.00, N'Cấp bậc từ Vàng trở lên', N'Thành viên', '2024-01-01', '2026-01-01', 0, 0, N'Đang áp dụng'),
('UD004', N'Combo 3 giờ', 12.00, N'Đặt sân từ 3 giờ trở lên', N'Combo', '2024-01-01', '2026-01-01', 0, 3, N'Đang áp dụng'),
('UD005', N'Happy Hour', 20.00, N'Khung giờ 14h-16h các ngày trong tuần', N'Thời gian', '2024-01-01', '2026-01-01', 0, 0, N'Đang áp dụng'),
('UD006', N'Cuối tuần vui vẻ', 5.00, N'Đặt sân vào thứ 7 hoặc Chủ nhật', N'Thời gian', '2024-01-01', '2026-01-01', 0, 0, N'Đang áp dụng'),
('UD007', N'Khách hàng mới', 10.00, N'Lần đầu tiên đặt sân tại hệ thống', N'Đối tượng', '2024-01-01', '2026-01-01', 0, 0, N'Đang áp dụng'),
('UD008', N'Đặt online', 7.00, N'Đặt sân qua website hoặc app', N'Kênh đặt', '2024-01-01', '2026-01-01', 0, 0, N'Đang áp dụng'),
('UD009', N'Nhóm đông', 10.00, N'Đặt từ 2 sân trở lên cùng lúc', N'Combo', '2024-01-01', '2026-01-01', 0, 0, N'Đang áp dụng'),
('UD010', N'Sinh nhật', 15.00, N'Trong tháng sinh nhật của khách hàng', N'Sự kiện', '2024-01-01', '2026-01-01', 0, 0, N'Đang áp dụng'),
('UD011', N'Khuyến mãi tháng 1', 12.00, N'Áp dụng cho tất cả đơn hàng tháng 1', N'Thời gian', '2025-01-01', '2025-01-31', 0, 0, N'Đang áp dụng'),
('UD012', N'Ưu đãi HLV', 5.00, N'Khi thuê HLV kèm theo sân', N'Combo', '2024-01-01', '2026-01-01', 0, 0, N'Đang áp dụng');
GO

-- 14. NẠP DAT VÀO BẢNG THAM SỐ HỆ THỐNG
INSERT INTO THAMSO_HETHONG (MaThamSo, TenThamSo, GiaTri, DonVi, MoTa, LoaiThamSo) VALUES 
(N'PHAT_HUY_SAN', N'Phí phạt hủy sân sát giờ', N'20', N'%', N'Phần trăm phí phạt khi hủy sân trước giờ đặt dưới 2 tiếng', N'Số'),
(N'THOI_GIAN_HUY_FREE', N'Thời gian hủy miễn phí', N'12', N'giờ', N'Số giờ tối thiểu hủy đơn để được hoàn tiền 100%', N'Số'),
(N'DIEM_THUONG_DAT_SAN', N'Điểm thưởng đặt sân', N'10', N'điểm/100k', N'Tỷ lệ điểm thưởng trên giá trị hóa đơn (100k = 10 điểm)', N'Số'),
(N'PHU_THU_NGAY_LE', N'Phụ thu ngày lễ', N'30', N'%', N'Phần trăm tăng giá vào ngày lễ', N'Số'),
(N'GIO_MO_CUA_MAC_DINH', N'Giờ mở cửa mặc định', N'05:00', N'', N'Giờ mở cửa áp dụng cho tất cả cơ sở nếu không quy định riêng', N'Thời gian'),
(N'GIO_DONG_CUA_MAC_DINH', N'Giờ đóng cửa mặc định', N'22:00', N'', N'Giờ đóng cửa áp dụng cho tất cả cơ sở nếu không quy định riêng', N'Thời gian');
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