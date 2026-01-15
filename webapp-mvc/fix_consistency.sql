-- 1. Free up usernames
UPDATE TAIKHOAN SET TenDangNhap = 'old_' + TenDangNhap WHERE MaTK IN ('TK010','TK011','TK012','TK013','TK014');

-- 2. Transform TK010 to HLV
UPDATE TAIKHOAN SET TenDangNhap = 'hlv03', VaiTro = N'HLV' WHERE MaTK = 'TK010';

-- 3. Shift usernames for existing customer accounts
UPDATE TAIKHOAN SET TenDangNhap = 'khachhang01' WHERE MaTK = 'TK011';
UPDATE TAIKHOAN SET TenDangNhap = 'khachhang02' WHERE MaTK = 'TK012';
UPDATE TAIKHOAN SET TenDangNhap = 'khachhang03' WHERE MaTK = 'TK013';
UPDATE TAIKHOAN SET TenDangNhap = 'khachhang04' WHERE MaTK = 'TK014';

-- 4. INSERT new accounts
INSERT INTO TAIKHOAN (MaTK, TenDangNhap, MatKhau, VaiTro, NgayDangKy) VALUES
('TK015', 'khachhang05', 'KH@2024Pass', N'Khách hàng', '2024-04-20'),
('TK016', 'khachhang06', 'KH@2024Pass', N'Khách hàng', '2024-04-25'),
('TK017', 'khachhang07', 'KH@2024Pass', N'Khách hàng', '2024-05-01'),
('TK018', 'khachhang08', 'KH@2024Pass', N'Khách hàng', '2024-05-05'),
('TK019', 'khachhang09', 'KH@2024Pass', N'Khách hàng', '2024-05-10'),
('TK020', 'khachhang10', 'KH@2024Pass', N'Khách hàng', '2024-05-15'),
('TK021', 'khachhang11', 'KH@2024Pass', N'Khách hàng', '2024-05-20'),
('TK022', 'khachhang12', 'KH@2024Pass', N'Khách hàng', '2024-05-25');

-- 5. Update KHACHHANG references
UPDATE KHACHHANG SET MaTK = 'TK015' WHERE MaKH = 'KH005';
UPDATE KHACHHANG SET MaTK = 'TK016' WHERE MaKH = 'KH006';
UPDATE KHACHHANG SET MaTK = 'TK017' WHERE MaKH = 'KH007';
UPDATE KHACHHANG SET MaTK = 'TK018' WHERE MaKH = 'KH008';
UPDATE KHACHHANG SET MaTK = 'TK019' WHERE MaKH = 'KH009';
UPDATE KHACHHANG SET MaTK = 'TK020' WHERE MaKH = 'KH010';
UPDATE KHACHHANG SET MaTK = 'TK021' WHERE MaKH = 'KH011';
UPDATE KHACHHANG SET MaTK = 'TK022' WHERE MaKH = 'KH012';
