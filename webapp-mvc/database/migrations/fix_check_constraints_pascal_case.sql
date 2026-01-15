-- Fix CHECK constraints to match PascalCase values from Frontend
USE TRUNGTAMTHETHAO
GO

-- 1. KHUNGGIO Constraints
IF EXISTS (SELECT * FROM sys.check_constraints WHERE name = 'CK_KHUNGGIO_LoaiNgay')
BEGIN
    ALTER TABLE KHUNGGIO DROP CONSTRAINT CK_KHUNGGIO_LoaiNgay
END
GO

ALTER TABLE KHUNGGIO ADD CONSTRAINT CK_KHUNGGIO_LoaiNgay 
CHECK (LoaiNgay IN ('Thuong', 'CuoiTuan', 'Le'))
GO

-- 2. UUDAI Constraints
IF EXISTS (SELECT * FROM sys.check_constraints WHERE name = 'CK_UUDAI_LoaiUuDai')
BEGIN
    ALTER TABLE UUDAI DROP CONSTRAINT CK_UUDAI_LoaiUuDai
END
GO

ALTER TABLE UUDAI ADD CONSTRAINT CK_UUDAI_LoaiUuDai 
CHECK (LoaiUuDai IN ('CapBac', 'HocSinhSinhVien', 'KhungGio', 'NgayTrongTuan', 'ComboGio', 'Chung'))
GO

PRINT 'Constraints updated to PascalCase successfully!'
GO
