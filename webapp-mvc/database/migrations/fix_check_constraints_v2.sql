-- Fix CHECK constraints - Match EXACTLY frontend values
USE TRUNGTAMTHETHAO
GO

-- Drop existing constraints
IF EXISTS (SELECT * FROM sys.check_constraints WHERE name = 'CK_KHUNGGIO_LoaiNgay')
BEGIN
    ALTER TABLE KHUNGGIO DROP CONSTRAINT CK_KHUNGGIO_LoaiNgay
END
GO

IF EXISTS (SELECT * FROM sys.check_constraints WHERE name = 'CK_UUDAI_LoaiUuDai')
BEGIN
    ALTER TABLE UUDAI DROP CONSTRAINT CK_UUDAI_LoaiUuDai
END
GO

-- Add constraints matching EXACTLY what frontend sends
-- For KHUNGGIO: Values from QuanLyGia.cshtml line 568-570
ALTER TABLE KHUNGGIO ADD CONSTRAINT CK_KHUNGGIO_LoaiNgay 
CHECK (LoaiNgay IN (N'Thường', N'Cuối tuần', N'Lễ'))
GO

-- For UUDAI: Values from QuanLyGia.cshtml line 628-632
ALTER TABLE UUDAI ADD CONSTRAINT CK_UUDAI_LoaiUuDai 
CHECK (LoaiUuDai IN (N'Cấp bậc', N'Học sinh/Sinh viên', N'Khung giờ', N'Ngày trong tuần', N'Combo giờ'))
GO

PRINT 'Constraints recreated successfully!'
GO
