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
