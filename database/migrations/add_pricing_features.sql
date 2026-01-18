-- =====================================================
-- Migration: Add Price Management Features
-- Description: Thêm bảng THAMSO_HETHONG và mở rộng KHUNGGIO, UUDAI
-- Date: 2026-01-15
-- =====================================================

USE TRUNGTAMTHETHAO
GO

-- =====================================================
-- 1. Tạo bảng THAMSO_HETHONG
-- =====================================================

CREATE TABLE THAMSO_HETHONG (
    MaThamSo VARCHAR(50) PRIMARY KEY,
    TenThamSo NVARCHAR(200) NOT NULL,
    GiaTri NVARCHAR(MAX),
    DonVi NVARCHAR(50),
    MoTa NVARCHAR(500),
    NgayCapNhat DATETIME DEFAULT GETDATE()
)
GO

-- =====================================================
-- 2. Mở rộng bảng KHUNGGIO
-- =====================================================

-- Kiểm tra và thêm cột mới vào KHUNGGIO nếu chưa tồn tại
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('KHUNGGIO') AND name = 'LoaiNgay')
BEGIN
    ALTER TABLE KHUNGGIO ADD LoaiNgay NVARCHAR(20) DEFAULT N'Thường'
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('KHUNGGIO') AND name = 'TrangThai')
BEGIN
    ALTER TABLE KHUNGGIO ADD TrangThai BIT DEFAULT 1
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('KHUNGGIO') AND name = 'NgayTao')
BEGIN
    ALTER TABLE KHUNGGIO ADD NgayTao DATETIME DEFAULT GETDATE()
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('KHUNGGIO') AND name = 'TenKhungGio')
BEGIN
    ALTER TABLE KHUNGGIO ADD TenKhungGio NVARCHAR(100)
END
GO

-- Thêm constraint cho LoaiNgay
IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE name = 'CK_KHUNGGIO_LoaiNgay')
BEGIN
    ALTER TABLE KHUNGGIO ADD CONSTRAINT CK_KHUNGGIO_LoaiNgay 
    CHECK (LoaiNgay IN (N'Thường', N'Cuối tuần', N'Lễ'))
END
GO

-- =====================================================
-- 3. Mở rộng bảng UUDAI
-- =====================================================

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('UUDAI') AND name = 'NgayBatDau')
BEGIN
    ALTER TABLE UUDAI ADD NgayBatDau DATE
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('UUDAI') AND name = 'NgayKetThuc')
BEGIN
    ALTER TABLE UUDAI ADD NgayKetThuc DATE
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('UUDAI') AND name = 'LoaiUuDai')
BEGIN
    ALTER TABLE UUDAI ADD LoaiUuDai NVARCHAR(50)
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('UUDAI') AND name = 'GiaTriToiThieu')
BEGIN
    ALTER TABLE UUDAI ADD GiaTriToiThieu DECIMAL(18,2) DEFAULT 0
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('UUDAI') AND name = 'SoGioToiThieu')
BEGIN
    ALTER TABLE UUDAI ADD SoGioToiThieu INT DEFAULT 0
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('UUDAI') AND name = 'TrangThai')
BEGIN
    ALTER TABLE UUDAI ADD TrangThai BIT DEFAULT 1
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('UUDAI') AND name = 'NgayTao')
BEGIN
    ALTER TABLE UUDAI ADD NgayTao DATETIME DEFAULT GETDATE()
END
GO

-- Thêm constraint cho LoaiUuDai
IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE name = 'CK_UUDAI_LoaiUuDai')
BEGIN
    ALTER TABLE UUDAI ADD CONSTRAINT CK_UUDAI_LoaiUuDai 
    CHECK (LoaiUuDai IN (N'Cấp bậc', N'Học sinh/Sinh viên', N'Khung giờ', N'Ngày trong tuần', N'Combo giờ', N'Chung'))
END
GO

-- =====================================================
-- 4. Insert dữ liệu mẫu cho THAMSO_HETHONG
-- =====================================================

-- Xóa dữ liệu cũ nếu có
DELETE FROM THAMSO_HETHONG
GO

-- Thêm các tham số hệ thống mặc định
INSERT INTO THAMSO_HETHONG (MaThamSo, TenThamSo, GiaTri, DonVi, MoTa)
VALUES 
    ('TGDAT_MIN', N'Thời gian đặt tối thiểu', '1', N'giờ', N'Số giờ tối thiểu khách hàng phải đặt sân'),
    ('TGDAT_MAX', N'Thời gian đặt tối đa', '4', N'giờ', N'Số giờ tối đa khách hàng có thể đặt sân trong một lần'),
    ('SO_SAN_MAX', N'Số sân tối đa mỗi khách', '3', N'sân', N'Số lượng sân tối đa một khách hàng có thể đặt cùng lúc'),
    ('TG_HUY_MIEN_PHI', N'Thời gian hủy miễn phí', '24', N'giờ', N'Số giờ trước giờ đặt mà khách được hủy miễn phí'),
    ('PHI_HUY_MUON', N'Phí hủy muộn', '30', N'%', N'Phần trăm phí khách phải trả nếu hủy sau thời gian miễn phí'),
    ('TG_CHECKIN_TRUOC', N'Thời gian check-in trước', '15', N'phút', N'Số phút khách có thể check-in trước giờ đặt'),
    ('TG_CHECKIN_SAU', N'Thời gian check-in sau', '30', N'phút', N'Số phút tối đa khách được check-in muộn'),
    ('DIEM_TICH_LUY_RATE', N'Tỷ lệ tích điểm', '1000', N'VNĐ/điểm', N'Số tiền tương ứng với 1 điểm tích lũy')
GO

-- =====================================================
-- 5. Insert dữ liệu mẫu cho KHUNGGIO (nếu trống)
-- =====================================================

-- Chỉ insert nếu bảng KHUNGGIO đang trống
IF NOT EXISTS (SELECT 1 FROM KHUNGGIO)
BEGIN
    -- Lấy danh sách loại sân
    DECLARE @MaLS_BongDa VARCHAR(20) = (SELECT TOP 1 MaLS FROM LOAISAN WHERE TenLS LIKE N'%bóng đá%')
    DECLARE @MaLS_CauLong VARCHAR(20) = (SELECT TOP 1 MaLS FROM LOAISAN WHERE TenLS LIKE N'%cầu lông%')
    
    -- Insert khung giờ cho bóng đá (ngày thường)
    IF @MaLS_BongDa IS NOT NULL
    BEGIN
        INSERT INTO KHUNGGIO (MaKG, MaLS, GioBatDau, GioKetThuc, NgayApDung, GiaApDung, LoaiNgay, TenKhungGio)
        VALUES 
            ('KG_BD_SANG_T', @MaLS_BongDa, '06:00', '12:00', '2026-01-01', 200000, N'Thường', N'Sáng - Ngày thường'),
            ('KG_BD_CHIEU_T', @MaLS_BongDa, '12:00', '18:00', '2026-01-01', 250000, N'Thường', N'Chiều - Ngày thường'),
            ('KG_BD_TOI_T', @MaLS_BongDa, '18:00', '23:00', '2026-01-01', 300000, N'Thường', N'Tối - Ngày thường'),
            ('KG_BD_SANG_CN', @MaLS_BongDa, '06:00', '12:00', '2026-01-01', 250000, N'Cuối tuần', N'Sáng - Cuối tuần'),
            ('KG_BD_CHIEU_CN', @MaLS_BongDa, '12:00', '18:00', '2026-01-01', 300000, N'Cuối tuần', N'Chiều - Cuối tuần'),
            ('KG_BD_TOI_CN', @MaLS_BongDa, '18:00', '23:00', '2026-01-01', 350000, N'Cuối tuần', N'Tối - Cuối tuần')
    END

    -- Insert khung giờ cho cầu lông (ngày thường)
    IF @MaLS_CauLong IS NOT NULL
    BEGIN
        INSERT INTO KHUNGGIO (MaKG, MaLS, GioBatDau, GioKetThuc, NgayApDung, GiaApDung, LoaiNgay, TenKhungGio)
        VALUES 
            ('KG_CL_SANG_T', @MaLS_CauLong, '06:00', '12:00', '2026-01-01', 50000, N'Thường', N'Sáng - Ngày thường'),
            ('KG_CL_CHIEU_T', @MaLS_CauLong, '12:00', '18:00', '2026-01-01', 70000, N'Thường', N'Chiều - Ngày thường'),
            ('KG_CL_TOI_T', @MaLS_CauLong, '18:00', '23:00', '2026-01-01', 90000, N'Thường', N'Tối - Ngày thường'),
            ('KG_CL_SANG_CN', @MaLS_CauLong, '06:00', '12:00', '2026-01-01', 70000, N'Cuối tuần', N'Sáng - Cuối tuần'),
            ('KG_CL_CHIEU_CN', @MaLS_CauLong, '12:00', '18:00', '2026-01-01', 90000, N'Cuối tuần', N'Chiều - Cuối tuần'),
            ('KG_CL_TOI_CN', @MaLS_CauLong, '18:00', '23:00', '2026-01-01', 110000, N'Cuối tuần', N'Tối - Cuối tuần')
    END
END
GO

-- =====================================================
-- 6. Insert dữ liệu mẫu cho UUDAI (nếu trống)
-- =====================================================

IF NOT EXISTS (SELECT 1 FROM UUDAI)
BEGIN
    INSERT INTO UUDAI (MaUD, TenCT, TyLeGiamGia, DieuKienApDung, LoaiUuDai, NgayBatDau, NgayKetThuc, GiaTriToiThieu, SoGioToiThieu)
    VALUES 
        ('UD_HSSV', N'Ưu đãi học sinh - sinh viên', 15.00, N'Xuất trình thẻ học sinh/sinh viên còn hiệu lực', N'Học sinh/Sinh viên', '2026-01-01', '2026-12-31', 0, 0),
        ('UD_SANG', N'Giảm giá khung giờ sáng', 10.00, N'Đặt sân từ 6h-9h sáng', N'Khung giờ', '2026-01-01', '2026-12-31', 0, 0),
        ('UD_COMBO3H', N'Combo đặt 3 giờ', 20.00, N'Đặt từ 3 giờ trở lên', N'Combo giờ', '2026-01-01', '2026-12-31', 0, 3),
        ('UD_T2T5', N'Ưu đãi đầu tuần', 12.00, N'Áp dụng thứ 2 đến thứ 5', N'Ngày trong tuần', '2026-01-01', '2026-12-31', 0, 0)
END
GO

PRINT 'Migration completed successfully!'
GO
