-- Fix CHECK constraints for KHUNGGIO and UUDAI tables
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

-- Recreate constraints with correct values
ALTER TABLE KHUNGGIO ADD CONSTRAINT CK_KHUNGGIO_LoaiNgay 
CHECK (LoaiNgay IN (N'Thường', N'Cuối tuần', N'Lễ'))
GO

ALTER TABLE UUDAI ADD CONSTRAINT CK_UUDAI_LoaiUuDai 
CHECK (LoaiUuDai IN (N'Cấp bậc', N'Học sinh/Sinh viên', N'Khung giờ', N'Ngày trong tuần', N'Combo giờ', N'Chung'))
GO

PRINT 'CHECK constraints fixed successfully!'
GO
