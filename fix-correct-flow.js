const sql = require('mssql');

const config = {
    server: 'localhost',
    port: 1433,
    database: 'TRUNGTAMTHETHAO',
    user: 'sa',
    password: 'YourStrong@Passw0rd',
    options: {
        trustServerCertificate: true,
        encrypt: false,
    }
};

async function fixFlow() {
    const pool = new sql.ConnectionPool(config);
    
    try {
        await pool.connect();
        console.log('🔄 Fix thanh toán flow...\n');
        
        // Fix sp_DatSan: REMOVE UPDATE SAN
        console.log('1️⃣  Cập nhật sp_DatSan (xóa UPDATE SAN)...');
        
        const fixedDatSan = `
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
    SET TRAN ISOLATION LEVEL SERIALIZABLE;
    
    BEGIN TRY
        BEGIN TRAN; 
        IF dbo.f_KiemTraSanTrong(@MaSan, @NgayDat, @GioBatDau, @GioKetThuc) = 0
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
        VALUES (@MaKH, @NguoiLap, @NgayDat, @NgayDat, @GioBatDau, @GioKetThuc, @KenhDat, N'Chờ thanh toán');
        DECLARE @MaDatSan BIGINT = SCOPE_IDENTITY();
        INSERT INTO DATSAN (MaDatSan, MaSan) VALUES (@MaDatSan, @MaSan);
        
        -- REMOVED: UPDATE SAN - Sân vẫn là "Còn Trống" cho đến khi thanh toán
        
        COMMIT TRAN; 
        PRINT N'Đặt sân thành công! Mã: ' + CAST(@MaDatSan AS VARCHAR(20));
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRAN;
        DECLARE @Msg NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(@Msg, 16, 1);
    END CATCH
END;
`;
        
        await pool.request().batch(fixedDatSan);
        console.log('   ✅ sp_DatSan: UPDATE SAN đã xóa\n');
        
        // Verify sp_ThanhToanOnline has UPDATE SAN
        console.log('2️⃣  Kiểm tra sp_ThanhToanOnline (phải có UPDATE SAN)...');
        
        const spCheck = await pool.request()
            .query(`SELECT OBJECT_DEFINITION(OBJECT_ID('sp_ThanhToanOnline')) as Code`);
        
        if (spCheck.recordset[0].Code.includes('UPDATE SAN SET TinhTrang')) {
            console.log('   ✅ sp_ThanhToanOnline: UPDATE SAN có mặt\n');
        } else {
            console.log('   ⚠️  sp_ThanhToanOnline: UPDATE SAN KHÔNG có, thêm vào...\n');
            
            const fixedThanhToan = `
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
        
        -- UPDATE TRẠNG THÁI PHIẾU VÀ DỊCH VỤ
        UPDATE PHIEUDATSAN SET TrangThai = N'Đã thanh toán' WHERE MaDatSan = @MaDatSan;
        UPDATE CT_DICHVUDAT SET TrangThaiSuDung = N'Đã thanh toán' WHERE MaDatSan = @MaDatSan;
        
        -- UPDATE SAN: "Còn Trống" → "Đã đặt" (chỉ sau khi thanh toán)
        UPDATE SAN SET TinhTrang = N'Đã đặt' 
        FROM SAN S JOIN DATSAN D ON S.MaSan = D.MaSan WHERE D.MaDatSan = @MaDatSan;
        
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
END;
`;
            
            await pool.request().batch(fixedThanhToan);
            console.log('   ✅ sp_ThanhToanOnline: UPDATE SAN đã thêm\n');
        }
        
        console.log('✅ ✅ ✅ FLOW HOÀN TOÀN ĐÚNG ✅ ✅ ✅');
        console.log('\n📋 Logic:');
        console.log('   1️⃣  Đặt sân: PHIEUDATSAN = "Chờ thanh toán", SAN = "Còn Trống" (giữ nguyên)');
        console.log('   2️⃣  Thanh toán: PHIEUDATSAN = "Đã thanh toán", SAN = "Đã đặt"');
        
        await pool.close();
    } catch (err) {
        console.error('❌ Lỗi:', err.message);
        process.exit(1);
    }
}

fixFlow();
