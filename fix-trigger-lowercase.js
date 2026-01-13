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

async function fixTrigger() {
    const pool = new sql.ConnectionPool(config);
    
    try {
        await pool.connect();
        console.log('🔄 Fix trigger trg_ChuyenTrangThaiSan...');
        
        // Drop trigger
        await pool.request()
            .query(`DROP TRIGGER IF EXISTS trg_ChuyenTrangThaiSan`);
        
        // Create fixed trigger
        const fixedTrigger = `
CREATE TRIGGER trg_ChuyenTrangThaiSan
ON SAN
FOR UPDATE
AS
BEGIN
    DECLARE @Cu NVARCHAR(50), @Moi NVARCHAR(50);
    SELECT @Cu = d.TinhTrang, @Moi = i.TinhTrang
    FROM deleted d JOIN inserted i ON d.MaSan = i.MaSan;

    IF (@Cu = N'Còn Trống' AND @Moi NOT IN (N'Đã đặt', N'Bảo Trì', N'Còn Trống'))
    OR (@Cu = N'Đã đặt' AND @Moi NOT IN (N'Đang Sử Dụng', N'Đã Hủy', N'Còn Trống'))
    OR (@Cu = N'Đang Sử Dụng' AND @Moi NOT IN (N'Còn Trống', N'Bảo Trì'))
    OR (@Cu = N'Bảo Trì' AND @Moi NOT IN (N'Còn Trống'))
    BEGIN
        RAISERROR (N'Lỗi: Chuyển đổi trạng thái sân không hợp lệ!', 16, 1);
        ROLLBACK TRANSACTION;
    END
END;
`;
        
        await pool.request().batch(fixedTrigger);
        
        console.log('✅ Trigger fixed! (lowercase: "Đã đặt")');
        
        // Also fix sp_ThanhToanOnline to use lowercase
        console.log('\n🔄 Fix sp_ThanhToanOnline...');
        
        const fixedSP = `
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
        
        -- UPDATE SAN TRẠNG THÁI (CHÍNH XÁC: chữ thường "Đã đặt")
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
        
        await pool.request().batch(fixedSP);
        console.log('✅ SP fixed!');
        
        await pool.close();
        console.log('\n✅ ✅ ✅ FIX HOÀN TẤT ✅ ✅ ✅');
    } catch (err) {
        console.error('❌ Lỗi:', err.message);
        process.exit(1);
    }
}

fixTrigger();
