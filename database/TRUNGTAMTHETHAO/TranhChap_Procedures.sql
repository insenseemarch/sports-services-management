-- ==					CÁC STORED PROCEDURE XỬ LÝ TRANH CHẤP                       ==
-- ==																                ==
-- ==                                                                               ==
-- ===================================================================================

-- =============================================================
-- NHÓM 1: QUẢN LÝ NHÂN SỰ & CA TRỰC (Tình huống 1, 2, 4, 15)
-- =============================================================

-- Tình huống 1: Unrepeatable Read (Xem lương vs Sửa lương)
-- [T1]: Xem thông tin nhân viên (Giải pháp: REPEATABLE READ)
CREATE OR ALTER PROCEDURE sp_XemThongTinNhanVien
    @MaNV VARCHAR(20)
AS
BEGIN
    SET TRAN ISOLATION LEVEL REPEATABLE READ
    BEGIN TRAN
        SELECT * FROM NHANVIEN WHERE MaNV = @MaNV;
        WAITFOR DELAY '00:00:05'; -- Giữ khóa để chờ T2
        SELECT * FROM NHANVIEN WHERE MaNV = @MaNV;
    COMMIT TRAN
END
GO

-- [T2]: Sửa thông tin nhân viên (Gây lỗi cho T1 nếu không chặn)
CREATE OR ALTER PROCEDURE sp_SuaThongTinNhanVien
    @MaNV VARCHAR(20),
    @LuongMoi DECIMAL(18,2)
AS
BEGIN
    BEGIN TRAN
        UPDATE NHANVIEN SET LuongCoBan = @LuongMoi WHERE MaNV = @MaNV;
    COMMIT TRAN
END
GO

-- Tình huống 2: Phantom Read (Xem danh sách vs Xóa nhân viên)
-- [T1]: Xem danh sách nhân viên (Giải pháp: SERIALIZABLE)
CREATE OR ALTER PROCEDURE sp_XemDanhSachNhanVien
AS
BEGIN
    SET TRAN ISOLATION LEVEL SERIALIZABLE
    BEGIN TRAN
        SELECT * FROM NHANVIEN;
        WAITFOR DELAY '00:00:05';
        SELECT * FROM NHANVIEN; 
    COMMIT TRAN
END
GO

-- [T2]: Xóa nhân viên (Gây lỗi Phantom cho T1)
CREATE OR ALTER PROCEDURE sp_XoaNhanVien
    @MaNV VARCHAR(20)
AS
BEGIN
    BEGIN TRAN
        DELETE FROM NHANVIEN WHERE MaNV = @MaNV;
    COMMIT TRAN
END
GO

-- Tình huống 4: Lost Update (Cập nhật lương - 2 Quản lý cùng sửa)
-- [T1] & [T2]: Cùng chạy thủ tục này (Giải pháp: REPEATABLE READ gây Deadlock để 1 người retry)
CREATE OR ALTER PROCEDURE sp_CapNhatLuongNV_AnToan
    @MaNV VARCHAR(20),
    @LuongMoi DECIMAL(18,2)
AS
BEGIN
    SET NOCOUNT ON;
    SET TRAN ISOLATION LEVEL REPEATABLE READ; 
    
    BEGIN TRY
        BEGIN TRAN;
        
        DECLARE @LuongCu DECIMAL(18,2);
        SELECT @LuongCu = LuongCoBan FROM NHANVIEN WHERE MaNV = @MaNV;

        WAITFOR DELAY '00:00:05'; -- Giả lập thời gian nhập liệu

        UPDATE NHANVIEN SET LuongCoBan = @LuongMoi WHERE MaNV = @MaNV;
        
        COMMIT TRAN;
        PRINT N'Cập nhật lương thành công.';
    END TRY
    BEGIN CATCH
        ROLLBACK TRAN;
        DECLARE @Msg NVARCHAR(4000) = ERROR_MESSAGE();
        PRINT N'Lỗi cập nhật (Deadlock detected - Lost Update prevented): ' + @Msg;
    END CATCH
END
GO

-- Tình huống 15: Phantom Read (Đếm ca trực vs Phân công mới)
-- [T1]: Đếm ca làm việc (Giải pháp: SERIALIZABLE)
CREATE OR ALTER PROCEDURE sp_DemCaLamViec
    @MaNV VARCHAR(20)
AS
BEGIN
    SET TRAN ISOLATION LEVEL SERIALIZABLE
    BEGIN TRAN
        SELECT COUNT(*) FROM THAMGIACATRUC WHERE MaNV = @MaNV;
        WAITFOR DELAY '00:00:05';
        SELECT COUNT(*) FROM THAMGIACATRUC WHERE MaNV = @MaNV;
    COMMIT TRAN
END
GO

-- [T2]: Phân công ca trực mới (Gây lỗi Phantom)
CREATE OR ALTER PROCEDURE sp_PhanCongCaTruc
    @MaCa BIGINT,
    @MaNV VARCHAR(20)
AS
BEGIN
    BEGIN TRAN
        INSERT INTO THAMGIACATRUC (MaCaTruc, MaNV) VALUES (@MaCa, @MaNV);
    COMMIT TRAN
END
GO

-- =============================================================
-- NHÓM 2: QUẢN LÝ SÂN	 (Tình huống 3, 5, 7, 8)
-- =============================================================

-- Tình huống 3: Dirty Read (Tìm sân vs Cập nhật chưa commit)
-- [T2]: Tìm kiếm sân (Giải pháp: READ COMMITTED - Mặc định)
CREATE OR ALTER PROCEDURE sp_TimKiemSan
    @MaCS VARCHAR(20),
    @MaLS VARCHAR(20)
AS
BEGIN
    SET TRAN ISOLATION LEVEL READ COMMITTED
    BEGIN TRAN
        SELECT * FROM SAN 
        WHERE MaCS = @MaCS AND MaLS = @MaLS AND TinhTrang = N'Còn Trống';
    COMMIT TRAN
END
GO

-- [T1]: Cập nhật trạng thái sân rồi Rollback (Gây lỗi Dirty Read cho T2)
CREATE OR ALTER PROCEDURE sp_CapNhatTrangThaiSan_Rollback
    @MaSan VARCHAR(20)
AS
BEGIN
    BEGIN TRAN
        UPDATE SAN SET TinhTrang = N'Bảo trì' WHERE MaSan = @MaSan;
        WAITFOR DELAY '00:00:10'; -- Giữ khóa X
    ROLLBACK TRAN -- Hủy bỏ
END
GO

-- Tình huống 5: Lost Update (Cập nhật trạng thái sân)
-- [T1] & [T2]: Cùng chạy thủ tục này (Giải pháp: REPEATABLE READ)
CREATE OR ALTER PROCEDURE sp_CapNhatTrangThaiSan_AnToan
    @MaSan VARCHAR(20),
    @TrangThaiMoi NVARCHAR(50)
AS
BEGIN
    SET TRAN ISOLATION LEVEL REPEATABLE READ;
    BEGIN TRY
        BEGIN TRAN
            DECLARE @TrangThaiCu NVARCHAR(50);
            SELECT @TrangThaiCu = TinhTrang FROM SAN WHERE MaSan = @MaSan;

            WAITFOR DELAY '00:00:05';

            UPDATE SAN SET TinhTrang = @TrangThaiMoi WHERE MaSan = @MaSan;
        COMMIT TRAN
    END TRY
    BEGIN CATCH
        ROLLBACK TRAN;
        PRINT N'Lỗi cập nhật trạng thái (Deadlock detected).';
    END CATCH
END
GO

-- Tình huống 7: Unrepeatable Read (Kiểm tra sân vs Đặt sân)
-- [T1]: Lễ tân kiểm tra sân (Giải pháp: REPEATABLE READ)
CREATE OR ALTER PROCEDURE sp_KiemTraTinhTrangSan
    @MaSan VARCHAR(20)
AS
BEGIN
    SET TRAN ISOLATION LEVEL REPEATABLE READ
    BEGIN TRAN
        SELECT TinhTrang FROM SAN WHERE MaSan = @MaSan;
        WAITFOR DELAY '00:00:05';
        SELECT TinhTrang FROM SAN WHERE MaSan = @MaSan;
    COMMIT TRAN
END
GO

-- [T2]: Khách đặt sân (Code ở phần 3 - sp_DatSan)

-- Tình huống 8: Phantom Read (Xem DS sân trống vs Thêm sân mới)
-- [T1]: Xem danh sách sân trống (Giải pháp: SERIALIZABLE)
CREATE OR ALTER PROCEDURE sp_XemDSSanTrong
    @MaLS VARCHAR(20)
AS
BEGIN
    SET TRAN ISOLATION LEVEL SERIALIZABLE
    BEGIN TRAN
        SELECT * FROM SAN WHERE MaLS = @MaLS AND TinhTrang = N'Còn Trống';
        WAITFOR DELAY '00:00:05';
        SELECT * FROM SAN WHERE MaLS = @MaLS AND TinhTrang = N'Còn Trống';
    COMMIT TRAN
END
GO

-- [T2]: Thêm sân trống mới
CREATE OR ALTER PROCEDURE sp_ThemSanTrongMoi
    @MaSan VARCHAR(20)
AS
BEGIN
    BEGIN TRAN
        UPDATE SAN SET TinhTrang = N'Còn Trống' WHERE MaSan = @MaSan;
    COMMIT TRAN
END
GO

-- =============================================================
-- NHÓM 3: GIÁ & KHUYẾN MÃI (Tình huống 6, 9, 14, 16, 18)
-- =============================================================

-- Tình huống 6 & 14: Dirty/Unrepeatable Read (Xem giá vs Sửa giá)
-- [T1] (Scenario 14) / [T2] (Scenario 6): Xem giá sân (Giải pháp: REPEATABLE READ)
CREATE OR ALTER PROCEDURE sp_XemGiaSan
    @MaLS VARCHAR(20),
    @MaKG VARCHAR(20)
AS
BEGIN
    SET TRAN ISOLATION LEVEL REPEATABLE READ
    BEGIN TRAN
        SELECT GiaApDung FROM KHUNGGIO WHERE MaLS = @MaLS AND MaKG = @MaKG;
        WAITFOR DELAY '00:00:05';
        SELECT GiaApDung FROM KHUNGGIO WHERE MaLS = @MaLS AND MaKG = @MaKG;
    COMMIT TRAN
END
GO

-- [T1] (Scenario 6) / [T2] (Scenario 14): Thay đổi giá thuê
CREATE OR ALTER PROCEDURE sp_ThayDoiGiaThueSan
    @MaLS VARCHAR(20),
    @MaKG VARCHAR(20),
    @GiaMoi DECIMAL(18,2)
AS
BEGIN
    BEGIN TRAN
        UPDATE KHUNGGIO SET GiaApDung = @GiaMoi WHERE MaLS = @MaLS AND MaKG = @MaKG;
    COMMIT TRAN
END
GO

-- Tình huống 9: Lost Update (Cập nhật Voucher)
-- [T1] & [T2]: Cùng chạy thủ tục này (Giải pháp: SERIALIZABLE)
CREATE OR ALTER PROCEDURE sp_CapNhatTyLeGiamGia_AnToan
    @MaUD VARCHAR(20),
    @TyLeMoi DECIMAL(5,2)
AS
BEGIN
    SET TRAN ISOLATION LEVEL SERIALIZABLE;
    BEGIN TRY
        BEGIN TRAN
            DECLARE @TyLeCu DECIMAL(5,2);
            SELECT @TyLeCu = TyLeGiamGia FROM UUDAI WHERE MaUD = @MaUD;

            WAITFOR DELAY '00:00:05';

            UPDATE UUDAI SET TyLeGiamGia = @TyLeMoi WHERE MaUD = @MaUD;
        COMMIT TRAN
    END TRY
    BEGIN CATCH
        ROLLBACK TRAN;
        PRINT N'Lỗi cập nhật Voucher.';
    END CATCH
END
GO

-- Tình huống 16: Dirty Read (Thanh toán áp dụng Voucher)
-- [T1]: Quản lý sửa voucher rồi rollback
CREATE OR ALTER PROCEDURE sp_CapNhatVoucher_Rollback
    @MaUD VARCHAR(20)
AS
BEGIN
    BEGIN TRAN
        UPDATE UUDAI SET TyLeGiamGia = 90 WHERE MaUD = @MaUD; -- Sửa thành 90%
        WAITFOR DELAY '00:00:10';
    ROLLBACK TRAN -- Hủy bỏ
END
GO

-- [T2]: Thu ngân thanh toán (Giải pháp: READ COMMITTED chặn đọc 90%)
CREATE OR ALTER PROCEDURE sp_ThanhToan_ApDungVoucher
    @MaDatSan BIGINT,
    @MaUD VARCHAR(20)
AS
BEGIN
    SET TRAN ISOLATION LEVEL READ COMMITTED 
    BEGIN TRAN
        DECLARE @TongTien DECIMAL(18,2);
        DECLARE @GiamGia DECIMAL(18,2) = 0;
        
        SELECT @TongTien = dbo.f_TinhTienSan(@MaDatSan) + dbo.f_TinhTienDichVu(@MaDatSan);

        DECLARE @TyLeGiam DECIMAL(5,2);
        SELECT @TyLeGiam = TyLeGiamGia FROM UUDAI WHERE MaUD = @MaUD;

        IF @TyLeGiam IS NOT NULL 
            SET @GiamGia = @TongTien * (@TyLeGiam / 100.0);

        PRINT N'Thanh toán thành công. Giảm giá: ' + CAST(@GiamGia AS VARCHAR);
    COMMIT TRAN
END
GO

-- Tình huống 18: Phantom Read (Xem Voucher Active vs Thêm/Sửa Voucher)
-- [T1]: Xem danh sách Voucher đang hoạt động (Giải pháp: SERIALIZABLE)
CREATE OR ALTER PROCEDURE sp_XemVoucherDangHoatDong
AS
BEGIN
    SET TRAN ISOLATION LEVEL SERIALIZABLE
    BEGIN TRAN
        SELECT * FROM UUDAI WHERE DieuKienApDung = N'Hoạt động';
        WAITFOR DELAY '00:00:05';
        SELECT * FROM UUDAI WHERE DieuKienApDung = N'Hoạt động';
    COMMIT TRAN
END
GO

-- [T2]: Quản lý thêm/sửa voucher
CREATE OR ALTER PROCEDURE sp_QL_ChinhSuaVoucher
    @MaUD_Moi VARCHAR(20),
    @MaUD_Cu VARCHAR(20)
AS
BEGIN
    BEGIN TRAN
        INSERT INTO UUDAI(MaUD, TenCT, TyLeGiamGia, DieuKienApDung) 
        VALUES (@MaUD_Moi, N'KM Mới', 10, N'Hoạt động');
        
        UPDATE UUDAI SET DieuKienApDung = N'Ngừng' WHERE MaUD = @MaUD_Cu;
    COMMIT TRAN
END
GO

-- =============================================================
-- NHÓM 4: THANH TOÁN & HÓA ĐƠN (Tình huống 10, 11, 12, 13, 17, 19)
-- =============================================================

-- Tình huống 10: Dirty Read (Thanh toán rollback vs Xuất HĐ)
-- [T1]: Thanh toán Online lỗi rollback
CREATE OR ALTER PROCEDURE sp_ThanhToanOnline_Rollback
    @MaDatSan BIGINT
AS
BEGIN
    BEGIN TRAN
        UPDATE CT_DICHVUDAT SET TrangThaiSuDung = N'Đã thanh toán' WHERE MaDatSan = @MaDatSan;
        WAITFOR DELAY '00:00:10';
    ROLLBACK TRAN
END
GO

-- [T2]: Xuất hóa đơn (Giải pháp: READ COMMITTED)
CREATE OR ALTER PROCEDURE sp_XuatHoaDon_AnToan
    @MaDatSan BIGINT
AS
BEGIN
    SET TRAN ISOLATION LEVEL READ COMMITTED
    BEGIN TRAN
        SELECT * FROM CT_DICHVUDAT 
        WHERE MaDatSan = @MaDatSan AND TrangThaiSuDung = N'Đã thanh toán';
    COMMIT TRAN
END
GO

-- Tình huống 11: Unrepeatable Read (Lập HĐ xem giá)
-- [T1]: Lập hóa đơn xem giá (Giải pháp: REPEATABLE READ)
CREATE OR ALTER PROCEDURE sp_LapHoaDon_XemGia
    @MaLS VARCHAR(20)
AS
BEGIN
    SET TRAN ISOLATION LEVEL REPEATABLE READ
    BEGIN TRAN
        SELECT GiaApDung FROM KHUNGGIO WHERE MaLS = @MaLS;
        WAITFOR DELAY '00:00:05';
        SELECT GiaApDung FROM KHUNGGIO WHERE MaLS = @MaLS;
    COMMIT TRAN
END
GO

-- [T2]: Thay đổi giá (Dùng sp_ThayDoiGiaThueSan ở trên)

-- Tình huống 12: Phantom Read (Báo cáo số lượng vs Đặt sân mới)
-- [T1]: Lập báo cáo số lượng đặt Online (Giải pháp: SERIALIZABLE)
CREATE OR ALTER PROCEDURE sp_LapBaoCao_DatOnline
    @NgayLap DATE
AS
BEGIN
    SET TRAN ISOLATION LEVEL SERIALIZABLE
    BEGIN TRAN
        SELECT COUNT(*) FROM PHIEUDATSAN WHERE NgayDat = @NgayLap AND KenhDat = N'Online';
        WAITFOR DELAY '00:00:05';
        SELECT COUNT(*) FROM PHIEUDATSAN WHERE NgayDat = @NgayLap AND KenhDat = N'Online';
    COMMIT TRAN
END
GO

-- [T2]: Đặt sân mới
CREATE OR ALTER PROCEDURE sp_DatSan_Moi
    @MaKH VARCHAR(20), @MaSan VARCHAR(20)
AS
BEGIN
    BEGIN TRAN
        INSERT INTO PHIEUDATSAN (MaKH, NgayDat, KenhDat, TrangThai, GioBatDau, GioKetThuc) 
        VALUES (@MaKH, GETDATE(), N'Online', N'Đã đặt', '08:00', '09:00');
        
        DECLARE @NewID BIGINT = SCOPE_IDENTITY();
        INSERT INTO DATSAN (MaDatSan, MaSan) VALUES (@NewID, @MaSan);
    COMMIT TRAN
END
GO

-- Tình huống 13: Dirty Read (Tính tiền vs Thêm dịch vụ rollback)
-- [T1 - GÂY LỖI]: NV Lễ tân thêm dịch vụ (Ví dụ: Thêm 5 chai nước) nhưng sau đó Rollback
CREATE OR ALTER PROCEDURE sp_ThemDichVu_Rollback
    @MaDatSan BIGINT,
    @MaDV VARCHAR(20)
AS
BEGIN
    BEGIN TRAN
        -- Giả sử thêm 5 món, đơn giá 10.000
        INSERT INTO CT_DICHVUDAT (MaDV, MaDatSan, SoLuong, ThanhTien, TrangThaiSuDung)
        VALUES (@MaDV, @MaDatSan, 5, 50000, N'Chưa thanh toán');
        
        PRINT N'T1: Đã thêm dịch vụ (Chưa Commit). Đang chờ...';
        WAITFOR DELAY '00:00:10'; -- Giữ khóa X để T2 đọc nhầm (nếu T2 không chặn)
        
    ROLLBACK TRAN -- Hủy bỏ (Khách đổi ý không gọi nữa)
    PRINT N'T1: Đã Rollback (Hủy dịch vụ).';
END
GO

-- [T2 - GIẢI QUYẾT]: Thu ngân tính tiền (Dùng READ COMMITTED để chờ T1 chốt xong mới đọc)
CREATE OR ALTER PROCEDURE sp_LapHoaDon_TinhTien
    @MaDatSan BIGINT
AS
BEGIN
    -- Mức cô lập READ COMMITTED sẽ chặn việc đọc dữ liệu chưa Commit
    SET TRAN ISOLATION LEVEL READ COMMITTED 
    BEGIN TRAN
        DECLARE @TongTien DECIMAL(18,2);
        
        -- Nếu T1 đang chạy, lệnh này sẽ phải CHỜ (Wait) cho đến khi T1 Rollback xong
        SELECT @TongTien = SUM(ThanhTien) 
        FROM CT_DICHVUDAT 
        WHERE MaDatSan = @MaDatSan;

        PRINT N'T2: Tổng tiền tính được là: ' + FORMAT(ISNULL(@TongTien, 0), 'N0');
    COMMIT TRAN
END
GO

-- [T1]: Dùng sp_ThanhToanOnline_Rollback hoặc tương tự để test

-- Tình huống 17: Unrepeatable Read (Xem chi tiết phiếu)
-- [T1]: Xem chi tiết phiếu (Giải pháp: REPEATABLE READ)
CREATE OR ALTER PROCEDURE sp_XemChiTietPhieuDatSan
    @MaDatSan BIGINT
AS
BEGIN
    SET TRAN ISOLATION LEVEL REPEATABLE READ
    BEGIN TRAN
        SELECT P.*, H.HinhThucTT 
        FROM PHIEUDATSAN P LEFT JOIN HOADON H ON P.MaDatSan = H.MaPhieu
        WHERE P.MaDatSan = @MaDatSan;
        
        WAITFOR DELAY '00:00:05';
        
        SELECT P.*, H.HinhThucTT 
        FROM PHIEUDATSAN P LEFT JOIN HOADON H ON P.MaDatSan = H.MaPhieu
        WHERE P.MaDatSan = @MaDatSan;
    COMMIT TRAN
END
GO

-- [T2]: Thanh toán (Dùng sp_ThanhToanVaXuatHoaDon bên dưới)

-- Tình huống 19: Lost Update (Cộng điểm tích lũy)
-- [T1] & [T2]: Cùng chạy (Giải pháp: SERIALIZABLE)
CREATE OR ALTER PROCEDURE sp_CapNhatDiemTichLuy_AnToan
    @MaKH VARCHAR(20),
    @DiemCong INT
AS
BEGIN
    SET NOCOUNT ON;
    SET TRAN ISOLATION LEVEL SERIALIZABLE;
    
    BEGIN TRY
        BEGIN TRAN;
        
        DECLARE @DiemCu INT;
        SELECT @DiemCu = DiemTichLuy FROM KHACHHANG WHERE MaKH = @MaKH;

        WAITFOR DELAY '00:00:05';

        UPDATE KHACHHANG SET DiemTichLuy = @DiemCu + @DiemCong WHERE MaKH = @MaKH;
        
        COMMIT TRAN;
        PRINT N'Cập nhật điểm thành công.';
    END TRY
    BEGIN CATCH
        ROLLBACK TRAN;
        PRINT N'Lỗi cập nhật điểm (Deadlock detected).';
    END CATCH
END
GO

-- ===================================================================================
-- ==             PHẦN 3: CÁC STORED PROCEDURE NGHIỆP VỤ CHÍNH		                ==
-- ==																			    ==																	==
-- ===================================================================================

-- 1. ĐẶT SÂN

GO

-- 2. THÊM DỊCH VỤ


-- 3. THANH TOÁN
CREATE OR ALTER PROCEDURE sp_ThanhToanVaXuatHoaDon
    @MaDatSan BIGINT,
    @NguoiLap VARCHAR(20),
    @HinhThucTT NVARCHAR(50), 
    @MaUD VARCHAR(20) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SET TRAN ISOLATION LEVEL REPEATABLE READ; -- Chặn thay đổi giá/điểm
    
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
        
        UPDATE PHIEUDATSAN SET TrangThai = N'Hoàn thành' WHERE MaDatSan = @MaDatSan;
        UPDATE CT_DICHVUDAT SET TrangThaiSuDung = N'Đã thanh toán' WHERE MaDatSan = @MaDatSan;
        
        DECLARE @DiemCong INT = CAST(@ThanhTien / 100000 AS INT);
        IF @DiemCong > 0
        BEGIN
            DECLARE @DiemCu INT;
            SELECT @DiemCu = DiemTichLuy FROM KHACHHANG WHERE MaKH = @MaKH;
            UPDATE KHACHHANG SET DiemTichLuy = @DiemCu + @DiemCong WHERE MaKH = @MaKH;
        END
        
        UPDATE SAN SET TinhTrang = N'Còn Trống' 
        FROM SAN S JOIN DATSAN D ON S.MaSan = D.MaSan WHERE D.MaDatSan = @MaDatSan;
        
        COMMIT TRAN;
        PRINT N'Thanh toán thành công. Mã HĐ: ' + CAST(@MaHD AS VARCHAR(20));
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRAN;
        DECLARE @Msg NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(@Msg, 16, 1);
    END CATCH
END
GO

-- 4. HỦY SÂN



-- ===================================================================================
-- ==                                                                               ==
-- ==                                  TRIGGER                                      ==
-- ==                                                                               ==
-- ===================================================================================

-- 1. Kiểm tra trùng lịch đặt sân

--2. Tự động trừ tồn kho khi đặt dịch vụ
CREATE OR ALTER TRIGGER trg_CapNhatTonKhoDichVu
ON CT_DICHVUDAT
FOR INSERT
AS
BEGIN
    DECLARE @MaDV VARCHAR(20);
    DECLARE @SoLuongDat INT;
    DECLARE @MaCS VARCHAR(20); 

    -- Lấy thông tin từ dòng vừa insert
    SELECT @MaDV = I.MaDV, @SoLuongDat = I.SoLuong
    FROM inserted I;

    -- Truy vết ngược để lấy Mã Cơ Sở (MaCS)
    SELECT TOP 1 @MaCS = S.MaCS
    FROM SAN S
    JOIN DATSAN D ON S.MaSan = D.MaSan
    JOIN inserted I ON I.MaDatSan = D.MaDatSan;

    -- Kiểm tra tồn kho trong bảng DV_COSO
    IF EXISTS (
        SELECT 1 
        FROM DV_COSO 
        WHERE MaDV = @MaDV AND MaCS = @MaCS AND SoLuongTon < @SoLuongDat
    )
    BEGIN
        RAISERROR (N'Lỗi: Số lượng dịch vụ trong kho không đủ!', 16, 1);
        ROLLBACK TRANSACTION;
    END
    ELSE
    BEGIN
        -- Trừ tồn kho
        UPDATE DV_COSO
        SET SoLuongTon = SoLuongTon - @SoLuongDat
        WHERE MaDV = @MaDV AND MaCS = @MaCS;
    END
END;
GO

-- 3. Giới hạn số lượng đặt sân của khách
CREATE OR ALTER TRIGGER trg_GioiHanDatSan
ON PHIEUDATSAN
FOR INSERT
AS
BEGIN
    DECLARE @MaKH VARCHAR(20);
    DECLARE @NgayDat DATE;
    
    SELECT @MaKH = MaKH, @NgayDat = NgayDat FROM inserted;

    -- Đếm số phiếu của khách trong ngày (loại trừ phiếu hủy)
    IF (SELECT COUNT(*) 
        FROM PHIEUDATSAN 
        WHERE MaKH = @MaKH AND NgayDat = @NgayDat AND TrangThai <> N'Đã hủy') > 2
    BEGIN
        RAISERROR (N'Lỗi: Mỗi khách hàng chỉ được đặt tối đa 2 sân trong một ngày!', 16, 1);
        ROLLBACK TRANSACTION;
    END
END;
GO

-- 4. Chuyển trạng thái sân hợp lệ
CREATE TRIGGER trg_ChuyenTrangThaiSan
ON SAN
FOR UPDATE
AS
BEGIN
    DECLARE @Cu NVARCHAR(50), @Moi NVARCHAR(50);
    SELECT @Cu = d.TinhTrang, @Moi = i.TinhTrang
    FROM deleted d JOIN inserted i ON d.MaSan = i.MaSan;

    IF (@Cu = N'Còn Trống' AND @Moi NOT IN (N'Đã Đặt', N'Bảo Trì', N'Còn Trống'))
    OR (@Cu = N'Đã Đặt' AND @Moi NOT IN (N'Đang Sử Dụng', N'Đã Hủy', N'Còn Trống')) -- Đã Hủy/Còn Trống nếu khách hủy
    OR (@Cu = N'Đang Sử Dụng' AND @Moi NOT IN (N'Còn Trống', N'Bảo Trì'))
    OR (@Cu = N'Bảo Trì' AND @Moi NOT IN (N'Còn Trống'))
    BEGIN
        RAISERROR (N'Lỗi: Chuyển đổi trạng thái sân không hợp lệ!', 16, 1);
        ROLLBACK TRANSACTION;
    END
END;
GO

-- 5. Kiểm tra Thời lượng & Khung giờ hoạt động
CREATE OR ALTER TRIGGER trg_KiemTraThoiLuongDat
ON PHIEUDATSAN
FOR INSERT, UPDATE
AS
BEGIN
    -- Nếu là UPDATE và KHÔNG đổi giờ thì bỏ qua (cho phép Hủy sân thoải mái)
    IF EXISTS (SELECT 1 FROM deleted) 
    BEGIN
        IF NOT UPDATE(GioBatDau) AND NOT UPDATE(GioKetThuc)
            RETURN;
    END

    IF NOT EXISTS (SELECT 1 FROM inserted) RETURN;

    DECLARE @GioBD TIME, @GioKT TIME, @LoaiSan NVARCHAR(50);
    DECLARE @ThoiLuong INT;
    DECLARE @GioMoCua TIME = '06:00:00'; -- Giả định giờ mở cửa
    DECLARE @GioDongCua TIME = '22:00:00'; -- Giả định giờ đóng cửa

    SELECT @GioBD = I.GioBatDau, @GioKT = I.GioKetThuc, @LoaiSan = LS.TenLS
    FROM inserted I
    JOIN DATSAN D ON I.MaDatSan = D.MaDatSan
    JOIN SAN S ON D.MaSan = S.MaSan
    JOIN LOAISAN LS ON S.MaLS = LS.MaLS;

    -- 1. Kiểm tra khung giờ hoạt động
    IF @GioBD < @GioMoCua OR @GioKT > @GioDongCua
    BEGIN
        RAISERROR (N'Lỗi: Thời gian đặt nằm ngoài khung giờ hoạt động của cơ sở!', 16, 1);
        ROLLBACK TRANSACTION;
        RETURN;
    END

    -- 2. Kiểm tra thời lượng theo loại sân
    SET @ThoiLuong = DATEDIFF(MINUTE, @GioBD, @GioKT);

    IF @LoaiSan = N'Bóng đá mini' AND (@ThoiLuong % 90 <> 0 OR @ThoiLuong < 90)
    BEGIN
        RAISERROR (N'Lỗi: Sân bóng đá mini phải đặt theo bội số của 90 phút (1 trận = 90 phút)!', 16, 1);
        ROLLBACK TRANSACTION;
        RETURN;
    END

    IF @LoaiSan = N'Tennis' AND (@ThoiLuong % 120 <> 0)
    BEGIN
        RAISERROR (N'Lỗi: Sân Tennis phải đặt theo bội số của 2 giờ (120 phút)!', 16, 1);
        ROLLBACK TRANSACTION;
        RETURN;
    END

    IF (@LoaiSan = N'Cầu lông' OR @LoaiSan = N'Bóng rổ') AND (@ThoiLuong % 60 <> 0)
    BEGIN
        RAISERROR (N'Lỗi: Sân Cầu lông/Bóng rổ phải đặt theo bội số của 1 giờ!', 16, 1);
        ROLLBACK TRANSACTION;
        RETURN;
    END
END;
GO

-- 6. Kiểm tra trùng lịch HLV & Tài nguyên (Phòng/Tủ)
CREATE OR ALTER TRIGGER trg_KiemTraLichDichVu
ON CT_DICHVUDAT
FOR INSERT, UPDATE
AS
BEGIN
    -- Nếu là UPDATE và KHÔNG đổi giờ thì bỏ qua (cho phép Hủy sân thoải mái)
    IF EXISTS (SELECT 1 FROM deleted) 
    BEGIN
        IF NOT UPDATE(GioBatDau) AND NOT UPDATE(GioKetThuc)
            RETURN;
    END

    IF NOT EXISTS (SELECT 1 FROM inserted) RETURN;

    -- Chỉ kiểm tra các dịch vụ có tính chất "Chiếm chỗ" (HLV, Phòng, Tủ)
    IF EXISTS (
        SELECT 1
        FROM inserted I
        JOIN DICHVU DV ON I.MaDV = DV.MaDV
        JOIN LOAIDV LDV ON DV.MaLoaiDV = LDV.MaLoaiDV -- <== SỬA: Join bảng này để lấy tên loại
        JOIN PHIEUDATSAN P_Moi ON I.MaDatSan = P_Moi.MaDatSan
        
        -- Tìm các phiếu cũ đã đặt cùng dịch vụ này (Cùng ông HLV/Cùng phòng đó)
        JOIN CT_DICHVUDAT CT_Cu ON I.MaDV = CT_Cu.MaDV 
        JOIN PHIEUDATSAN P_Cu ON CT_Cu.MaDatSan = P_Cu.MaDatSan
        
        WHERE I.MaDatSan <> CT_Cu.MaDatSan -- Khác phiếu hiện tại
          AND P_Cu.NgayDat = P_Moi.NgayDat -- Cùng ngày
          AND P_Cu.TrangThai NOT IN (N'Đã hủy', N'No-Show', N'Nháp') -- Phiếu cũ chưa hủy và không phải nháp
          
          -- Kiểm tra loại dịch vụ dựa trên tên trong bảng LOAIDV
          AND (LDV.TenLoai IN (N'Huấn luyện viên', N'Phòng VIP', N'Tủ đồ')) 
          
          -- Kiểm tra trùng giờ
          AND (
              (P_Moi.GioBatDau >= P_Cu.GioBatDau AND P_Moi.GioBatDau < P_Cu.GioKetThuc)
              OR
              (P_Moi.GioKetThuc > P_Cu.GioBatDau AND P_Moi.GioKetThuc <= P_Cu.GioKetThuc)
              OR
              (P_Moi.GioBatDau <= P_Cu.GioBatDau AND P_Moi.GioKetThuc >= P_Cu.GioKetThuc)
          )
    )
    BEGIN
        RAISERROR (N'Lỗi: Huấn luyện viên hoặc Phòng/Tủ đồ này đã được đặt kín trong khung giờ này!', 16, 1);
        ROLLBACK TRANSACTION;
    END
END;
GO

-- 7. Ràng buộc về thông tin nhân viên (Giới tính & Chức vụ)
CREATE OR ALTER TRIGGER trg_KiemTraNhanVien
ON NHANVIEN
FOR INSERT, UPDATE
AS
BEGIN
    IF EXISTS (
        SELECT 1 FROM inserted 
        WHERE GioiTinh NOT IN (N'Nam', N'Nữ', N'Khác')
           OR ChucVu NOT IN (N'Quản lý', N'Lễ tân', N'Kỹ thuật', N'Thu ngân', N'HLV')
    )
    BEGIN
        RAISERROR (N'Lỗi: Giới tính hoặc Chức vụ nhân viên không hợp lệ!', 16, 1);
        ROLLBACK TRANSACTION;
    END
END;
GO

--8. Kiểm tra sức chứa 
CREATE OR ALTER TRIGGER trg_KiemTraSucChua
ON SAN
FOR INSERT, UPDATE
AS
BEGIN
    IF EXISTS (SELECT 1 FROM inserted WHERE SucChua <= 0)
    BEGIN
        RAISERROR (N'Lỗi: Sức chứa của sân phải lớn hơn 0!', 16, 1);
        ROLLBACK TRANSACTION;
    END
END;
GO
-- 9. Kiểm tra đơn giá dịch vụ
CREATE OR ALTER TRIGGER trg_KiemTraGiaTriDichVu
ON DICHVU
FOR INSERT, UPDATE
AS
BEGIN
    -- Kiểm tra đơn giá dịch vụ >= 0
    IF EXISTS (SELECT 1 FROM inserted WHERE DonGia < 0)
    BEGIN
        RAISERROR (N'Lỗi: Đơn giá dịch vụ không được nhỏ hơn 0!', 16, 1);
        ROLLBACK TRANSACTION;
        RETURN;
    END
END;
GO
-- 10. Kiểm tra tồn kho
CREATE OR ALTER TRIGGER trg_KiemTraTonKhoDichVu
ON DV_COSO
FOR INSERT, UPDATE
AS
BEGIN
    -- Kiểm tra số lượng tồn >= 0
    IF EXISTS (SELECT 1 FROM inserted WHERE SoLuongTon < 0)
    BEGIN
        RAISERROR (N'Lỗi: Số lượng tồn kho không được nhỏ hơn 0!', 16, 1);
        ROLLBACK TRANSACTION;
        RETURN;
    END
END;
GO

-- 11. Hoàn trả tồn kho dịch vụ khi hủy phiếu


--	===================================================================================
--	==								BẢO MẬT & PHÂN QUYỀN							 ==
--	===================================================================================

CREATE ROLE Role_QuanLy;
CREATE ROLE Role_LeTan;
CREATE ROLE Role_ThuNgan;
CREATE ROLE Role_KyThuat;
CREATE ROLE Role_KhachHang;
CREATE ROLE Role_HLV;
GO

-- CẤP QUYỀN
GRANT SELECT, INSERT, UPDATE, DELETE ON SCHEMA::dbo TO Role_QuanLy;

GRANT SELECT ON SAN TO Role_LeTan;
GRANT SELECT, INSERT, UPDATE ON KHACHHANG TO Role_LeTan;
GRANT SELECT, INSERT, UPDATE ON PHIEUDATSAN TO Role_LeTan;
GRANT SELECT, INSERT ON DATSAN TO Role_LeTan;
GRANT SELECT, INSERT, UPDATE ON CT_DICHVUDAT TO Role_LeTan;
GRANT SELECT ON KHUNGGIO TO Role_LeTan;
GRANT SELECT ON DV_COSO TO Role_LeTan; 
DENY SELECT ON BAOCAOTHONGKE TO Role_LeTan;

GRANT SELECT ON PHIEUDATSAN TO Role_ThuNgan;
GRANT SELECT, INSERT, UPDATE ON HOADON TO Role_ThuNgan;
GRANT SELECT ON UUDAI TO Role_ThuNgan;
DENY UPDATE ON NHANVIEN TO Role_ThuNgan;

GRANT SELECT, UPDATE ON SAN TO Role_KyThuat;
GRANT SELECT, INSERT, UPDATE ON PHIEUBAOTRI TO Role_KyThuat;

GRANT SELECT ON PHIEUDATSAN TO Role_HLV;
GRANT SELECT ON CT_DICHVUDAT TO Role_HLV;
-- HLV xem được lịch trực của chính mình (Tham gia ca trực)
GRANT SELECT ON THAMGIACATRUC TO Role_HLV;

GRANT SELECT ON SAN TO Role_KhachHang;
GRANT SELECT ON KHUNGGIO TO Role_KhachHang;
GRANT SELECT, UPDATE ON KHACHHANG TO Role_KhachHang;
-- ===================================================================================
-- PHẦN BỔ SUNG: FUNCTIONS VÀ STORED PROCEDURES ĐÃ CẬP NHẬT MỚI NHẤT
-- ===================================================================================
GO

-- ===================================================================================
-- ==                                                                               ==
-- ==                          PHẦN 1: FUNCTIONS                                    ==
-- ==                                                                               ==
-- ===================================================================================

/*
--------------------------------------------------------------------------------
FUNCTION 1: f_TinhTienSan
--------------------------------------------------------------------------------
Mục đích: Tính tiền sân dựa vào loại sân và khung giờ
Input: @MaDatSan (Mã phiếu đặt sân)
Output: Tiền sân (DECIMAL)
Sử dụng trong: Controllers (ThanhToanController, DichVuController, LichSuDatSanController)
*/
GO
CREATE OR ALTER FUNCTION f_TinhTienSan (@MaDatSan BIGINT)
RETURNS DECIMAL(18,2)
AS
BEGIN
    DECLARE @TienSan DECIMAL(18,2) = 0;
    DECLARE @MaLS VARCHAR(20);
    DECLARE @TenLS NVARCHAR(50);
    DECLARE @GioBatDau TIME;
    DECLARE @GioKetThuc TIME;
    DECLARE @NgayDat DATE;
    DECLARE @CurrentHour TIME;
    DECLARE @NextHour TIME;
    DECLARE @GiaKhung DECIMAL(18,2);
    DECLARE @DurationMinutes INT;

    -- Lấy thông tin phiếu đặt và loại sân
    SELECT @MaLS = S.MaLS, @TenLS = LS.TenLS, 
           @GioBatDau = P.GioBatDau, @GioKetThuc = P.GioKetThuc,
           @NgayDat = P.NgayDat
    FROM PHIEUDATSAN P
    JOIN DATSAN D ON P.MaDatSan = D.MaDatSan
    JOIN SAN S ON D.MaSan = S.MaSan
    JOIN LOAISAN LS ON S.MaLS = LS.MaLS
    WHERE P.MaDatSan = @MaDatSan;

    -- Tính thời lượng (phút)
    SET @DurationMinutes = DATEDIFF(MINUTE, @GioBatDau, @GioKetThuc);

    -- TÍNH TIỀN THEO TỪNG KHUNG GIỜ
    -- Duyệt từng giờ từ GioBatDau đến GioKetThuc
    SET @CurrentHour = @GioBatDau;
    
    WHILE @CurrentHour < @GioKetThuc
    BEGIN
        -- Tính giờ tiếp theo (mỗi lần tăng 1 giờ)
        SET @NextHour = DATEADD(HOUR, 1, @CurrentHour);
        IF @NextHour > @GioKetThuc
            SET @NextHour = @GioKetThuc;
        
        -- Tìm giá khung giờ chứa @CurrentHour
        SELECT TOP 1 @GiaKhung = K.GiaApDung
        FROM KHUNGGIO K
        WHERE K.MaLS = @MaLS 
          AND @CurrentHour >= K.GioBatDau 
          AND @CurrentHour < K.GioKetThuc
          AND K.NgayApDung <= @NgayDat
        ORDER BY K.NgayApDung DESC;
        
        -- Nếu không tìm thấy khung giờ phù hợp, lấy khung GẦN NHẤT
        IF @GiaKhung IS NULL
        BEGIN
            -- Tìm khung giờ gần nhất (trước hoặc sau)
            SELECT TOP 1 @GiaKhung = K.GiaApDung
            FROM KHUNGGIO K
            WHERE K.MaLS = @MaLS 
              AND K.NgayApDung <= @NgayDat
            ORDER BY 
                ABS(DATEDIFF(MINUTE, @CurrentHour, K.GioBatDau)),
                K.NgayApDung DESC;
        END
        
        -- Cộng giá vào tổng (tính theo tỷ lệ thời gian thực tế)
        DECLARE @ActualMinutes INT = DATEDIFF(MINUTE, @CurrentHour, @NextHour);
        SET @TienSan = @TienSan + (ISNULL(@GiaKhung, 0) * @ActualMinutes / 60.0);
        
        -- Reset giá cho vòng lặp tiếp theo
        SET @GiaKhung = NULL;
        
        -- Tăng giờ
        SET @CurrentHour = @NextHour;
    END

    RETURN @TienSan;
END
GO

/*
--------------------------------------------------------------------------------
FUNCTION 2: f_TinhTienDichVu
--------------------------------------------------------------------------------
Mục đích: Tính tổng tiền dịch vụ của một phiếu đặt
Input: @MaDatSan (Mã phiếu đặt sân)
Output: Tổng tiền dịch vụ (DECIMAL)
*/
GO
CREATE OR ALTER FUNCTION f_TinhTienDichVu (@MaDatSan BIGINT)
RETURNS DECIMAL(18,2)
AS
BEGIN
    DECLARE @TongTienDV DECIMAL(18,2) = 0;
    
    SELECT @TongTienDV = SUM(ThanhTien)
    FROM CT_DICHVUDAT
    WHERE MaDatSan = @MaDatSan;
    
    RETURN ISNULL(@TongTienDV, 0);
END
GO

/*
--------------------------------------------------------------------------------
FUNCTION 3: f_KiemTraSanTrong
--------------------------------------------------------------------------------
Mục đích: Kiểm tra sân có trống trong khung giờ không
Input: @MaSan, @NgayDat, @GioBD, @GioKT, @MaDatSanExclude (optional)
Output: 1 (Trống), 0 (Bận)
*/
GO
CREATE OR ALTER FUNCTION f_KiemTraSanTrong 
(
    @MaSan VARCHAR(20), 
    @NgayDat DATE, 
    @GioBD TIME, 
    @GioKT TIME,
    @MaDatSanExclude BIGINT = NULL -- Tham số tùy chọn: Mã phiếu cần bỏ qua (để đổi lịch)
)
RETURNS BIT
AS
BEGIN
    DECLARE @KetQua BIT = 1; -- Mặc định là trống (1)

    IF EXISTS (
        SELECT 1
        FROM PHIEUDATSAN P
        JOIN DATSAN D ON P.MaDatSan = D.MaDatSan
        WHERE D.MaSan = @MaSan
          AND P.NgayDat = @NgayDat
          AND P.TrangThai <> N'Đã hủy' AND P.TrangThai <> N'Nháp'
          AND (@MaDatSanExclude IS NULL OR P.MaDatSan <> @MaDatSanExclude) -- Bỏ qua chính nó
          AND (
              (@GioBD >= P.GioBatDau AND @GioBD < P.GioKetThuc) OR 
              (@GioKT > P.GioBatDau AND @GioKT <= P.GioKetThuc) OR 
              (P.GioBatDau >= @GioBD AND P.GioBatDau < @GioKT)      
          )
    )
    BEGIN
        SET @KetQua = 0; -- Đã bị trùng (0)
    END

    RETURN @KetQua;
END
GO

/*
--------------------------------------------------------------------------------
FUNCTION 4: f_TinhTienPhat
--------------------------------------------------------------------------------
Mục đích: Tính tiền phạt khi hủy sân
Input: @MaDatSan, @ThoiDiemHuy
Output: Tiền phạt (DECIMAL)
Quy tắc:
  - Hủy trước >= 24h: Phạt 10%
  - Hủy trước 0-24h: Phạt 50%
  - No-show hoặc hủy sau giờ đá: Phạt 100%
*/
GO
CREATE OR ALTER FUNCTION f_TinhTienPhat
(
    @MaDatSan BIGINT,
    @ThoiDiemHuy DATETIME -- Thời điểm khách báo hủy
)
RETURNS DECIMAL(18,2)
AS
BEGIN
    DECLARE @TienPhat DECIMAL(18,2) = 0;
    DECLARE @NgayDa DATE;
    DECLARE @GioDa TIME;
    DECLARE @ThoiDiemDa DATETIME;
    DECLARE @TongTienSan DECIMAL(18,2);

    -- Lấy thông tin phiếu đặt
    SELECT @NgayDa = P.NgayDat, @GioDa = P.GioBatDau 
    FROM PHIEUDATSAN P WHERE MaDatSan = @MaDatSan;

    -- Ghép Ngày + Giờ để ra thời điểm đá
    SET @ThoiDiemDa = CAST(@NgayDa AS DATETIME) + CAST(@GioDa AS DATETIME);

    -- Tính tổng tiền sân (gọi lại hàm cũ)
    SET @TongTienSan = dbo.f_TinhTienSan(@MaDatSan);

    -- Tính số giờ chênh lệch
    DECLARE @GioChenhLech INT = DATEDIFF(HOUR, @ThoiDiemHuy, @ThoiDiemDa);

    IF @GioChenhLech >= 24
        SET @TienPhat = @TongTienSan * 0.1; -- Phạt 10%
    ELSE IF @GioChenhLech >= 0
        SET @TienPhat = @TongTienSan * 0.5; -- Phạt 50%
    ELSE 
        SET @TienPhat = @TongTienSan; -- No-show hoặc hủy sau khi đã bắt đầu (coi như 100%)

    RETURN @TienPhat;
END
GO

-- ===================================================================================
-- ==                                                                               ==
-- ==                    PHẦN 2: STORED PROCEDURES                                  ==
-- ==                                                                               ==
-- ===================================================================================

/*
--------------------------------------------------------------------------------
SP 1: sp_DatSan
--------------------------------------------------------------------------------
Mục đích: Tạo phiếu đặt sân mới (trạng thái Nháp)
Sử dụng trong: DatSanController.cs
*/
GO
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
    SET TRAN ISOLATION LEVEL SERIALIZABLE; -- Khóa phạm vi thời gian
    
    BEGIN TRY
        BEGIN TRAN; 
        IF dbo.f_KiemTraSanTrong(@MaSan, @NgayDat, @GioBatDau, @GioKetThuc, NULL) = 0
        BEGIN
            ROLLBACK TRAN; 
            RAISERROR(N'Lỗi: Sân đã bị người khác đặt!', 16, 1);
            RETURN;
        END
        
        -- Valid giờ hoạt động
        DECLARE @GioMoCua TIME, @GioDongCua TIME;
        SELECT @GioMoCua = C.GioMoCua, @GioDongCua = C.GioDongCua
        FROM SAN S
        JOIN COSO C ON S.MaCS = C.MaCS
        WHERE S.MaSan = @MaSan;

        IF @GioBatDau < @GioMoCua OR @GioKetThuc > @GioDongCua
        BEGIN
             ROLLBACK TRAN;
             RAISERROR(N'Lỗi: Thời gian đặt nằm ngoài giờ hoạt động!', 16, 1);
             RETURN;
        END
        IF @KenhDat = 'Online' AND DATEDIFF(HOUR, GETDATE(), CAST(@NgayDat AS DATETIME) + CAST(@GioBatDau AS DATETIME)) < 2
        BEGIN
             ROLLBACK TRAN;
             RAISERROR(N'Lỗi: Đặt Online phải trước 2 tiếng!', 16, 1);
             RETURN;
        END
        INSERT INTO PHIEUDATSAN (MaKH, NguoiLap, NgayDat, NgayKetThuc, GioBatDau, GioKetThuc, KenhDat, TrangThai, NgayTao)
        VALUES (@MaKH, @NguoiLap, @NgayDat, @NgayDat, @GioBatDau, @GioKetThuc, @KenhDat, N'Nháp', GETDATE());
        DECLARE @MaDatSan BIGINT = SCOPE_IDENTITY();
        INSERT INTO DATSAN (MaDatSan, MaSan) VALUES (@MaDatSan, @MaSan);
        COMMIT TRAN; 
        PRINT N'Đặt sân thành công! Mã: ' + CAST(@MaDatSan AS VARCHAR(20));
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRAN;
        DECLARE @Msg NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(@Msg, 16, 1);
    END CATCH
END
GO

/*
--------------------------------------------------------------------------------
SP 2: sp_ThemDichVu
--------------------------------------------------------------------------------
Mục đích: Thêm dịch vụ vào phiếu đặt sân
Sử dụng trong: DichVuController.cs
Fix lỗi: Bỏ qua kiểm tra tồn kho cho HLV, VIP, Tủ đồ (dịch vụ vô hình)
*/
GO
CREATE OR ALTER PROCEDURE sp_ThemDichVu
    @MaDatSan BIGINT,
    @MaDV VARCHAR(20),
    @SoLuong INT,
    @MaCSContext VARCHAR(20) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SET TRAN ISOLATION LEVEL SERIALIZABLE; 
    
    BEGIN TRY
        BEGIN TRAN;
        
        DECLARE @DonGia DECIMAL(18,2);
        DECLARE @MaCS VARCHAR(20);
        
        -- Lấy giá và Mã cơ sở của Sân đang đặt
        SELECT @DonGia = DonGia, @MaCS = S.MaCS 
        FROM DICHVU DV
        LEFT JOIN DATSAN DS ON DS.MaDatSan = @MaDatSan
        LEFT JOIN SAN S ON DS.MaSan = S.MaSan
        WHERE DV.MaDV = @MaDV;

        IF @DonGia IS NULL
        BEGIN
             ROLLBACK TRAN;
             RAISERROR(N'Dịch vụ không tồn tại!', 16, 1);
             RETURN;
        END
        
        -- Nếu không có MaCS từ sân (service-only order), dùng @MaCSContext
        IF @MaCS IS NULL AND @MaCSContext IS NOT NULL
        BEGIN
            SET @MaCS = @MaCSContext;
        END

        -- MẶC ĐỊNH LÀ CÓ TRỪ KHO (IsStockItem = 1)
        DECLARE @IsStockItem BIT = 1;
        
        -- LOGIC FIX: Kiểm tra nếu là HLV (LDV001), VIP (LDV004), Locker (LDV005) thì KHÔNG TRỪ KHO
        IF EXISTS (
            SELECT 1 FROM DICHVU DV 
            JOIN LOAIDV L ON DV.MaLoaiDV = L.MaLoaiDV
            WHERE DV.MaDV = @MaDV 
            AND (
                -- Check theo Mã Loại Cứng (Ưu tiên)
                L.MaLoaiDV IN ('LDV001', 'LDV004', 'LDV005') 
                OR L.MaLoaiDV LIKE 'LDV001%' 
                OR L.MaLoaiDV LIKE 'LDV004%' 
                OR L.MaLoaiDV LIKE 'LDV005%'
                -- Check fallback theo Tên (Phòng trường hợp mã khác)
                OR L.TenLoai LIKE N'%Huấn luyện viên%' 
                OR L.TenLoai LIKE N'%VIP%' 
                OR L.TenLoai LIKE N'%Tủ đồ%'
            )
        )
        BEGIN
            SET @IsStockItem = 0; 
        END

        -- CHỈ KIỂM TRA TỒN KHO NẾU LÀ SẢN PHẨM VẬT LÝ VÀ ĐANG THÊM (DELTA DƯƠNG)
        IF @IsStockItem = 1 AND @SoLuong > 0
        BEGIN
            DECLARE @TonKho INT;
            SELECT @TonKho = SoLuongTon FROM DV_COSO WHERE MaDV = @MaDV AND MaCS = @MaCS;
            
            -- Nếu không tìm thấy kho hoặc số lượng không đủ -> Báo lỗi chi tiết
            IF @TonKho IS NULL OR @TonKho < @SoLuong
            BEGIN
                ROLLBACK TRAN;
                RAISERROR(N'Lỗi: Không đủ tồn kho cho dịch vụ này tại cơ sở hiện tại!', 16, 1);
                RETURN;
            END
        END

        -- CẬP NHẬT HOẶC THÊM MỚI VÀO CHI TIẾT DỊCH VỤ ĐẶT
        IF EXISTS (SELECT 1 FROM CT_DICHVUDAT WHERE MaDatSan = @MaDatSan AND MaDV = @MaDV)
        BEGIN
            -- Nếu đã có -> Cộng dồn số lượng và cập nhật thành tiền
            UPDATE CT_DICHVUDAT 
            SET SoLuong = SoLuong + @SoLuong, 
                ThanhTien = (SoLuong + @SoLuong) * @DonGia 
            WHERE MaDatSan = @MaDatSan AND MaDV = @MaDV;
        END
        ELSE
        BEGIN
            -- lấy tên dịch vụ để lưu vào Ghi chú (để dễ debug)
            DECLARE @TenDV NVARCHAR(100);
            SELECT @TenDV = TenDV FROM DICHVU WHERE MaDV = @MaDV;

            -- Nếu chưa có -> Thêm mới
            INSERT INTO CT_DICHVUDAT (MaDV, MaDatSan, SoLuong, ThanhTien, TrangThaiSuDung, GhiChu) 
            VALUES (@MaDV, @MaDatSan, @SoLuong, @SoLuong * @DonGia, N'Chưa thanh toán', @TenDV);
        END

        
        -- TRỪ/CỘNG KHO NẾU LÀ SẢN PHẨM VẬT LÝ
        IF @IsStockItem = 1
        BEGIN
            UPDATE DV_COSO SET SoLuongTon = SoLuongTon - @SoLuong WHERE MaDV = @MaDV AND MaCS = @MaCS;
        END

        COMMIT TRAN;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRAN;
        DECLARE @Msg NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(@Msg, 16, 1);
    END CATCH
END
GO

/*
--------------------------------------------------------------------------------
SP 3: sp_ThanhToanOnline
--------------------------------------------------------------------------------
Mục đích: Thanh toán online và xác nhận đặt sân (Nháp -> Đã đặt)
Sử dụng trong: DatSanThanhToanController.cs
*/
GO
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
        
        DECLARE @MaKH VARCHAR(20);
        
        -- 1. KIỂM TRA LẠI TÌNH TRẠNG SÂN (DOUBLE-CHECK)
        DECLARE @MaSanCheck VARCHAR(20), @NgayDatCheck DATE, @GioBDCheck TIME, @GioKTCheck TIME;
        
        SELECT @MaSanCheck = D.MaSan, @NgayDatCheck = P.NgayDat, @GioBDCheck = P.GioBatDau, @GioKTCheck = P.GioKetThuc, @MaKH = P.MaKH
        FROM PHIEUDATSAN P
        JOIN DATSAN D ON P.MaDatSan = D.MaDatSan
        WHERE P.MaDatSan = @MaDatSan;
        
        -- Kiểm tra nếu đã có người khác đặt (Trừ chính đơn này - dù đơn này đang là Nháp)
        IF dbo.f_KiemTraSanTrong(@MaSanCheck, @NgayDatCheck, @GioBDCheck, @GioKTCheck, NULL) = 0
        BEGIN
            ROLLBACK TRAN;
            RAISERROR(N'Rất tiếc! Sân đã bị người khác đặt trong lúc bạn đang thanh toán.', 16, 1);
            RETURN;
        END

        DECLARE @TongCong DECIMAL(18,2) = dbo.f_TinhTienSan(@MaDatSan) + dbo.f_TinhTienDichVu(@MaDatSan);
        DECLARE @GiamGia DECIMAL(18,2) = 0;
        DECLARE @ThanhTien DECIMAL(18,2);
        
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
        
        -- UPDATE TRẠNG THÁI: CHÍNH THỨC ĐÃ ĐẶT
        UPDATE PHIEUDATSAN SET TrangThai = N'Đã đặt', NgayTao = GETDATE() WHERE MaDatSan = @MaDatSan;
        UPDATE CT_DICHVUDAT SET TrangThaiSuDung = N'Đã thanh toán' WHERE MaDatSan = @MaDatSan;
        
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
END
GO

/*
--------------------------------------------------------------------------------
SP 4: sp_ThanhToanVaXuatHoaDon
--------------------------------------------------------------------------------
Mục đích: Thanh toán tại quầy và xuất hóa đơn (Check-out)
Sử dụng trong: ThanhToanController.cs
*/
GO
CREATE OR ALTER PROCEDURE sp_ThanhToanVaXuatHoaDon
    @MaDatSan BIGINT,
    @NguoiLap VARCHAR(20),
    @HinhThucTT NVARCHAR(50), 
    @MaUD VARCHAR(20) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SET TRAN ISOLATION LEVEL REPEATABLE READ; -- Chặn thay đổi giá/điểm
    
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
        
        UPDATE PHIEUDATSAN SET TrangThai = N'Hoàn thành' WHERE MaDatSan = @MaDatSan;
        UPDATE CT_DICHVUDAT SET TrangThaiSuDung = N'Đã thanh toán' WHERE MaDatSan = @MaDatSan;
        
        DECLARE @DiemCong INT = CAST(@ThanhTien / 100000 AS INT);
        IF @DiemCong > 0
        BEGIN
            DECLARE @DiemCu INT;
            SELECT @DiemCu = DiemTichLuy FROM KHACHHANG WHERE MaKH = @MaKH;
            UPDATE KHACHHANG SET DiemTichLuy = @DiemCu + @DiemCong WHERE MaKH = @MaKH;
        END
        
        UPDATE SAN SET TinhTrang = N'Còn Trống' 
        FROM SAN S JOIN DATSAN D ON S.MaSan = D.MaSan WHERE D.MaDatSan = @MaDatSan;
        
        COMMIT TRAN;
        PRINT N'Thanh toán thành công. Mã HĐ: ' + CAST(@MaHD AS VARCHAR(20));
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRAN;
        DECLARE @Msg NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(@Msg, 16, 1);
    END CATCH
END
GO

/*
--------------------------------------------------------------------------------
SP 5: sp_HuySan
--------------------------------------------------------------------------------
Mục đích: Hủy sân và tính tiền phạt (nếu có)
Sử dụng trong: LichSuDatSanController.cs (sp_KhachHang_HuySan gọi SP này)
*/
GO
CREATE OR ALTER PROCEDURE sp_HuySan
    @MaDatSan BIGINT,
    @NguoiThucHien VARCHAR(20) 
AS
BEGIN
    SET NOCOUNT ON;
    SET TRAN ISOLATION LEVEL REPEATABLE READ;
    BEGIN TRY
        BEGIN TRAN;
        DECLARE @TienPhat DECIMAL(18,2) = dbo.f_TinhTienPhat(@MaDatSan, GETDATE());
        
        -- Nếu có tiền phạt -> Tạo hóa đơn phạt
        IF @TienPhat > 0
        BEGIN
            INSERT INTO HOADON (MaPhieu, NguoiLap, NgayLap, TongTien, GiamGia, ThanhTien, HinhThucTT)
            VALUES (@MaDatSan, @NguoiThucHien, GETDATE(), @TienPhat, 0, @TienPhat, N'Tiền phạt hủy sân');
        END
        
        UPDATE PHIEUDATSAN SET TrangThai = N'Đã hủy' WHERE MaDatSan = @MaDatSan;
        -- Cập nhật trạng thái dịch vụ đi kèm thành Đã hủy
        UPDATE CT_DICHVUDAT SET TrangThaiSuDung = N'Đã hủy' WHERE MaDatSan = @MaDatSan;

        UPDATE SAN SET TinhTrang = N'Còn Trống' 
        FROM SAN S JOIN DATSAN D ON S.MaSan = D.MaSan WHERE D.MaDatSan = @MaDatSan;
        
        COMMIT TRAN;
        PRINT N'Hủy sân thành công. Tiền phạt: ' + CAST(@TienPhat AS VARCHAR(20));
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRAN;
        DECLARE @Msg NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(@Msg, 16, 1);
    END CATCH
END
GO

/*
--------------------------------------------------------------------------------
SP 6: sp_DoiLichDat
--------------------------------------------------------------------------------
Mục đích: Đổi lịch đặt sân (Reschedule)
Sử dụng trong: LichSuDatSanController.cs
*/
GO
CREATE OR ALTER PROCEDURE sp_DoiLichDat
    @MaDatSan BIGINT,
    @MaKH VARCHAR(20),
    @NgayMoi DATE,
    @GioBDMoi TIME,
    @GioKTMoi TIME
AS
BEGIN
    SET NOCOUNT ON;
    SET TRAN ISOLATION LEVEL REPEATABLE READ;

    BEGIN TRY
        BEGIN TRAN;

        -- 1. Kiểm tra tồn tại và quyền sở hữu
        DECLARE @MaSan VARCHAR(20), @NgayCu DATE, @GioBDCu TIME, @TrangThai NVARCHAR(50);
        
        SELECT @MaSan = D.MaSan, @NgayCu = P.NgayDat, @GioBDCu = P.GioBatDau, @TrangThai = P.TrangThai
        FROM PHIEUDATSAN P
        JOIN DATSAN D ON P.MaDatSan = D.MaDatSan
        WHERE P.MaDatSan = @MaDatSan AND P.MaKH = @MaKH;

        IF @MaSan IS NULL
        BEGIN
            ROLLBACK TRAN;
            RAISERROR(N'Không tìm thấy phiếu đặt hoặc bạn không có quyền thay đổi!', 16, 1);
            RETURN;
        END

        -- 2. Kiểm tra trạng thái (Chỉ được đổi khi chưa hoàn thành/hủy)
        IF @TrangThai IN (N'Đã hủy', N'Hoàn thành', N'No-Show')
        BEGIN
            ROLLBACK TRAN;
            RAISERROR(N'Không thể đổi lịch cho đơn đã hủy hoặc đã hoàn thành!', 16, 1);
            RETURN;
        END
        
        -- 3. Kiểm tra Sân Trống cho Giờ Mới (Trừ chính đơn này ra)
        IF dbo.f_KiemTraSanTrong(@MaSan, @NgayMoi, @GioBDMoi, @GioKTMoi, @MaDatSan) = 0
        BEGIN
            ROLLBACK TRAN;
            RAISERROR(N'Khung giờ mới đã có người đặt! Vui lòng chọn giờ khác.', 16, 1);
            RETURN;
        END

        -- 4. Cập nhật
        UPDATE PHIEUDATSAN 
        SET NgayDat = @NgayMoi, GioBatDau = @GioBDMoi, GioKetThuc = @GioKTMoi, NgayTao = GETDATE()
        WHERE MaDatSan = @MaDatSan;

        COMMIT TRAN;
        PRINT N'Đổi lịch thành công!';
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRAN;
        DECLARE @Msg NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(@Msg, 16, 1);
    END CATCH
END
GO

/*
--------------------------------------------------------------------------------
SP 7: sp_TuDongHuyDonQuaHan
--------------------------------------------------------------------------------
Mục đích: Tự động hủy các đơn "Chờ thanh toán" quá hạn (> 30 phút)
Sử dụng trong: Background job / Scheduled task
Fix lỗi: Đổi trạng thái thành 'Đã hủy' thay vì 'Quá hạn thanh toán'
*/
GO
CREATE OR ALTER PROCEDURE sp_TuDongHuyDonQuaHan
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @SoPhutGioiHan INT = 30; -- Thời gian cho phép giữ chỗ (phút)
    
    -- Cập nhật các đơn 'Chờ thanh toán' quá hạn thành 'Đã hủy'
    UPDATE PHIEUDATSAN
    SET TrangThai = N'Đã hủy'
    WHERE TrangThai = N'Chờ thanh toán'
      AND DATEDIFF(MINUTE, NgayTao, GETDATE()) > @SoPhutGioiHan;
      
    -- Trả về số lượng đơn đã hủy để log (nếu cần)
    SELECT @@ROWCOUNT AS SoLuongHuy;
END
GO

/*
--------------------------------------------------------------------------------
SP 8: sp_ThanhToanTaiQuay
--------------------------------------------------------------------------------
Mục đích: Xác nhận đặt sân tại quầy (Nháp -> Chờ thanh toán)
Sử dụng trong: DatSanThanhToanController.cs
*/
GO
CREATE OR ALTER PROCEDURE sp_ThanhToanTaiQuay
    @MaDatSan BIGINT
AS
BEGIN
    SET NOCOUNT ON;
    SET TRAN ISOLATION LEVEL REPEATABLE READ;
    
    BEGIN TRY
        BEGIN TRAN;
        
        -- 1. DOUBLE-CHECK AVAILABILITY
        DECLARE @MaSanCheck VARCHAR(20), @NgayDatCheck DATE, @GioBDCheck TIME, @GioKTCheck TIME;
        
        SELECT @MaSanCheck = D.MaSan, @NgayDatCheck = P.NgayDat, @GioBDCheck = P.GioBatDau, @GioKTCheck = P.GioKetThuc
        FROM PHIEUDATSAN P
        JOIN DATSAN D ON P.MaDatSan = D.MaDatSan
        WHERE P.MaDatSan = @MaDatSan;
        
        IF dbo.f_KiemTraSanTrong(@MaSanCheck, @NgayDatCheck, @GioBDCheck, @GioKTCheck, NULL) = 0
        BEGIN
            ROLLBACK TRAN;
            RAISERROR(N'Rất tiếc! Sân đã bị người khác đặt trong lúc bạn đang thanh toán.', 16, 1);
            RETURN;
        END

        -- UPDATE STATUS: PENDING & REFRESH CREATED DATE
        UPDATE PHIEUDATSAN SET TrangThai = N'Chờ thanh toán', NgayTao = GETDATE() WHERE MaDatSan = @MaDatSan;
        
        COMMIT TRAN;
        PRINT N'Chuyển sang trạng thái chờ thanh toán thành công!';
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRAN;
        DECLARE @Msg NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(@Msg, 16, 1);
    END CATCH
END
GO

/*
--------------------------------------------------------------------------------
SP 9: sp_KhachHang_HuySan
--------------------------------------------------------------------------------
Mục đích: Khách hàng hủy sân với tính phạt theo thời gian
Sử dụng trong: LichSuDatSanController.cs
Quy tắc phạt:
  - Hủy trước >= 24h: Phạt 10%
  - Hủy trong vòng 24h: Phạt 50%
  - No-show (hủy sau giờ bắt đầu): Phạt 100%
*/
GO
CREATE OR ALTER PROCEDURE sp_KhachHang_HuySan
    @MaDatSan BIGINT
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @TienPhat DECIMAL(18,2);
    
    -- Calculate penalty using existing function
    SET @TienPhat = dbo.f_TinhTienPhat(@MaDatSan, GETDATE());

    -- Update status
    UPDATE PHIEUDATSAN
    SET TrangThai = N'Đã hủy'
    WHERE MaDatSan = @MaDatSan;

    -- Return penalty amount
    SELECT @TienPhat AS TienPhat;
END
GO

/*
--------------------------------------------------------------------------------
SP 10: sp_KhachHang_DanhGia
--------------------------------------------------------------------------------
*/
GO
CREATE OR ALTER PROCEDURE sp_KhachHang_DanhGia
    @MaDatSan BIGINT,
    @Diem INT,
    @NoiDung NVARCHAR(500)
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO DANHGIA (MaDatSan, Diem, NoiDung)
    VALUES (@MaDatSan, @Diem, @NoiDung);
END
GO

/*
--------------------------------------------------------------------------------
SP 11: sp_KhachHang_DoiSan (Logic đổi lịch check trùng)
--------------------------------------------------------------------------------
*/
GO
CREATE OR ALTER PROCEDURE sp_KhachHang_DoiSan
    @MaDatSan BIGINT,
    @NgayMoi DATE,
    @GioBatDauMoi TIME,
    @GioKetThucMoi TIME
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @MaSan VARCHAR(20);
    SELECT @MaSan = D.MaSan 
    FROM PHIEUDATSAN P JOIN DATSAN D ON P.MaDatSan = D.MaDatSan 
    WHERE P.MaDatSan = @MaDatSan;

    -- Check if new time is available (Truyền 5 tham số để exclude chính mã phiếu này)
    -- Hàm f_KiemTraSanTrong(@MaSan, @Ngay, @BD, @KT, @MaDatSanExclude)
    IF dbo.f_KiemTraSanTrong(@MaSan, @NgayMoi, @GioBatDauMoi, @GioKetThucMoi, @MaDatSan) = 0
    BEGIN
        SELECT 0 AS Result, N'Sân đã bị đặt trong khung giờ này.' AS Message;
        RETURN;
    END

    -- Update
    UPDATE PHIEUDATSAN
    SET NgayDat = @NgayMoi, GioBatDau = @GioBatDauMoi, GioKetThuc = @GioKetThucMoi
    WHERE MaDatSan = @MaDatSan;

    SELECT 1 AS Result, N'Đổi giờ thành công.' AS Message;
END
GO

/*
--------------------------------------------------------------------------------
SCRIPT FIX DATA: HLV KHÔNG CẦN QUẢN LÝ TỒN KHO
--------------------------------------------------------------------------------
*/
-- XÓA HLV RA KHỎI BẢNG DV_COSO (vì HLV không cần quản lý tồn kho)
DELETE FROM DV_COSO 
WHERE MaDV IN (
    SELECT DV.MaDV 
    FROM DICHVU DV 
    JOIN LOAIDV L ON DV.MaLoaiDV = L.MaLoaiDV
    WHERE L.MaLoaiDV IN ('LDV001', 'LDV004', 'LDV005')
       OR L.MaLoaiDV LIKE 'LDV001%' -- Huấn luyện viên
       OR L.MaLoaiDV LIKE 'LDV004%' -- Gói VIP
       OR L.MaLoaiDV LIKE 'LDV005%' -- Tủ đồ
       OR L.TenLoai LIKE N'%Huấn luyện viên%'
);

PRINT N'Đã chạy FIX_HLV_DATA: Xóa HLV/VIP/Tủ đồ khỏi kho!';
GO

﻿USE master
GO



-- =======================================================
-- KHU VỰC CÁC BẢN VÁ (PATCHES)
-- =======================================================
GO-- =============================================
-- PATCH: Sửa trigger KiemTraThoiLuongDat để bỏ qua khi Hủy sân
-- Mục đích: Cho phép hủy sân mà không bị validate lại thời lượng (tránh lỗi với data cũ)
-- =============================================

GO
USE TRUNGTAMTHETHAO;
GO

CREATE OR ALTER TRIGGER trg_KiemTraThoiLuongDat
ON PHIEUDATSAN
FOR INSERT, UPDATE
AS
BEGIN
    -- Nếu là UPDATE và KHÔNG đổi giờ thì bỏ qua (cho phép Hủy sân thoải mái)
    IF EXISTS (SELECT 1 FROM deleted) 
    BEGIN
        IF NOT UPDATE(GioBatDau) AND NOT UPDATE(GioKetThuc)
            RETURN;
    END

    IF NOT EXISTS (SELECT 1 FROM inserted) RETURN;

    DECLARE @GioBD TIME, @GioKT TIME, @LoaiSan NVARCHAR(50);
    DECLARE @ThoiLuong INT;
    DECLARE @GioMoCua TIME = '06:00:00'; -- Giả định giờ mở cửa
    DECLARE @GioDongCua TIME = '22:00:00'; -- Giả định giờ đóng cửa

    SELECT @GioBD = I.GioBatDau, @GioKT = I.GioKetThuc, @LoaiSan = LS.TenLS
    FROM inserted I
    JOIN DATSAN D ON I.MaDatSan = D.MaDatSan
    JOIN SAN S ON D.MaSan = S.MaSan
    JOIN LOAISAN LS ON S.MaLS = LS.MaLS;

    -- 1. Kiểm tra khung giờ hoạt động
    IF @GioBD < @GioMoCua OR @GioKT > @GioDongCua
    BEGIN
        RAISERROR (N'Lỗi: Thời gian đặt nằm ngoài khung giờ hoạt động của cơ sở!', 16, 1);
        ROLLBACK TRANSACTION;
        RETURN;
    END

    -- 2. Kiểm tra thời lượng theo loại sân
    SET @ThoiLuong = DATEDIFF(MINUTE, @GioBD, @GioKT);

    IF @LoaiSan = N'Bóng đá mini' AND (@ThoiLuong % 90 <> 0 OR @ThoiLuong < 90)
    BEGIN
        RAISERROR (N'Lỗi: Sân bóng đá mini phải đặt theo bội số của 90 phút (1 trận = 90 phút)!', 16, 1);
        ROLLBACK TRANSACTION;
        RETURN;
    END

    IF @LoaiSan = N'Tennis' AND (@ThoiLuong % 120 <> 0)
    BEGIN
        RAISERROR (N'Lỗi: Sân Tennis phải đặt theo bội số của 2 giờ (120 phút)!', 16, 1);
        ROLLBACK TRANSACTION;
        RETURN;
    END

    IF (@LoaiSan = N'Cầu lông' OR @LoaiSan = N'Bóng rổ') AND (@ThoiLuong % 60 <> 0)
    BEGIN
        RAISERROR (N'Lỗi: Sân Cầu lông/Bóng rổ phải đặt theo bội số của 1 giờ!', 16, 1);
        ROLLBACK TRANSACTION;
        RETURN;
    END
END;
GO

PRINT 'PATCH COMPLETED: Trigger trg_KiemTraThoiLuongDat đã được cập nhật';
GO
USE TRUNGTAMTHETHAO
GO

-- Cập nhật sp_DatSan để thêm NGAYTAO khi insert

GO
-- =============================================
-- PATCH: Sửa function f_TinhTienSan tính giá theo TỪNG KHUNG GIỜ
-- Mục đích: Tính chính xác giá sân khi đặt qua nhiều khung giờ khác nhau
-- =============================================

USE TRUNGTAMTHETHAO;
GO

CREATE OR ALTER FUNCTION f_TinhTienSan (@MaDatSan BIGINT)
RETURNS DECIMAL(18,2)
AS
BEGIN
    DECLARE @TienSan DECIMAL(18,2) = 0;
    DECLARE @MaLS VARCHAR(20);
    DECLARE @TenLS NVARCHAR(50);
    DECLARE @GioBatDau TIME;
    DECLARE @GioKetThuc TIME;
    DECLARE @NgayDat DATE;
    DECLARE @CurrentHour TIME;
    DECLARE @NextHour TIME;
    DECLARE @GiaKhung DECIMAL(18,2);
    DECLARE @DurationMinutes INT;

    -- Lấy thông tin phiếu đặt và loại sân
    SELECT @MaLS = S.MaLS, @TenLS = LS.TenLS, 
           @GioBatDau = P.GioBatDau, @GioKetThuc = P.GioKetThuc,
           @NgayDat = P.NgayDat
    FROM PHIEUDATSAN P
    JOIN DATSAN D ON P.MaDatSan = D.MaDatSan
    JOIN SAN S ON D.MaSan = S.MaSan
    JOIN LOAISAN LS ON S.MaLS = LS.MaLS
    WHERE P.MaDatSan = @MaDatSan;

    -- Tính thời lượng (phút)
    SET @DurationMinutes = DATEDIFF(MINUTE, @GioBatDau, @GioKetThuc);

    -- TÍNH TIỀN THEO TỪNG KHUNG GIỜ
    -- Duyệt từng giờ từ GioBatDau đến GioKetThuc
    SET @CurrentHour = @GioBatDau;
    
    WHILE @CurrentHour < @GioKetThuc
    BEGIN
        -- Tính giờ tiếp theo (mỗi lần tăng 1 giờ)
        SET @NextHour = DATEADD(HOUR, 1, @CurrentHour);
        IF @NextHour > @GioKetThuc
            SET @NextHour = @GioKetThuc;
        
        -- Tìm giá khung giờ chứa @CurrentHour
        SELECT TOP 1 @GiaKhung = K.GiaApDung
        FROM KHUNGGIO K
        WHERE K.MaLS = @MaLS 
          AND @CurrentHour >= K.GioBatDau 
          AND @CurrentHour < K.GioKetThuc
          AND K.NgayApDung <= @NgayDat
        ORDER BY K.NgayApDung DESC;
        
        -- Nếu không tìm thấy khung giờ phù hợp, lấy khung GẦN NHẤT
        IF @GiaKhung IS NULL
        BEGIN
            -- Tìm khung giờ gần nhất (trước hoặc sau)
            SELECT TOP 1 @GiaKhung = K.GiaApDung
            FROM KHUNGGIO K
            WHERE K.MaLS = @MaLS 
              AND K.NgayApDung <= @NgayDat
            ORDER BY 
                ABS(DATEDIFF(MINUTE, @CurrentHour, K.GioBatDau)),
                K.NgayApDung DESC;
        END
        
        -- Cộng giá vào tổng (tính theo tỷ lệ thời gian thực tế)
        DECLARE @ActualMinutes INT = DATEDIFF(MINUTE, @CurrentHour, @NextHour);
        SET @TienSan = @TienSan + (ISNULL(@GiaKhung, 0) * @ActualMinutes / 60.0);
        
        -- Reset giá cho vòng lặp tiếp theo
        SET @GiaKhung = NULL;
        
        -- Tăng giờ
        SET @CurrentHour = @NextHour;
    END

    RETURN @TienSan;
END
GO

PRINT 'PATCH COMPLETED: Function f_TinhTienSan đã được cập nhật để tính giá theo từng khung giờ';
GO
CREATE OR ALTER PROCEDURE sp_ThanhToanTaiQuay
    -- VERSION: FIX_PAYMENT_V3_NO_CHECK
    @MaDatSan BIGINT
AS
BEGIN
    SET NOCOUNT ON;
    SET TRAN ISOLATION LEVEL REPEATABLE READ;
    
    BEGIN TRY
        BEGIN TRAN;
        
        DECLARE @TrangThaiHienTai NVARCHAR(50);
        SELECT @TrangThaiHienTai = TrangThai FROM PHIEUDATSAN WHERE MaDatSan = @MaDatSan;

        -- [ EMERGENCY FIX ] BỎ QUA CHECK TRÙNG

        -- UPDATE STATUS
        IF @TrangThaiHienTai = N'Nháp'
             UPDATE PHIEUDATSAN SET TrangThai = N'Chờ thanh toán', NgayTao = GETDATE() WHERE MaDatSan = @MaDatSan;
        
        COMMIT TRAN;
        PRINT N'Chuyển sang trạng thái chờ thanh toán thành công!';
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRAN;
        DECLARE @Msg NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(@Msg, 16, 1);
    END CATCH
END
GO
USE TRUNGTAMTHETHAO
GO

-- Trigger: Hoàn trả tồn kho dịch vụ khi hủy phiếu đặt sân
CREATE OR ALTER TRIGGER trg_HoanTraDichVuKhiHuy
ON PHIEUDATSAN
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    -- Chỉ xử lý khi trạng thái chuyển sang 'Đã hủy', 'No-Show', hoặc 'Không hợp lệ'
    -- Từ các trạng thái 'giữ chỗ' ('Chờ thanh toán', 'Đã đặt', 'Đã thanh toán')
    IF EXISTS (
        SELECT 1 
        FROM inserted i
        JOIN deleted d ON i.MaDatSan = d.MaDatSan
        WHERE i.TrangThai IN (N'Đã hủy', N'No-Show', N'Không hợp lệ')
          AND d.TrangThai NOT IN (N'Đã hủy', N'No-Show', N'Không hợp lệ', N'Nháp')
    )
    BEGIN
        -- Sử dụng bảng tạm để lưu các phiếu vừa bị hủy
        SELECT i.MaDatSan
        INTO #CancelledBookings
        FROM inserted i
        JOIN deleted d ON i.MaDatSan = d.MaDatSan
        WHERE i.TrangThai IN (N'Đã hủy', N'No-Show', N'Không hợp lệ')
          AND d.TrangThai NOT IN (N'Đã hủy', N'No-Show', N'Không hợp lệ', N'Nháp');

        -- Cập nhật lại tồn kho trong DV_COSO
        -- Logic: Cộng lại Số lượng đã đặt vào Số lượng tồn
        -- Cần biết MaCS (Cơ sở) của sân trong phiếu đặt.
        
        UPDATE KHO
        SET KHO.SoLuongTon = KHO.SoLuongTon + CT.SoLuong
        FROM DV_COSO KHO
        JOIN CT_DICHVUDAT CT ON KHO.MaDV = CT.MaDV
        JOIN DICHVU DV ON CT.MaDV = DV.MaDV
        JOIN LOAIDV LDV ON DV.MaLoaiDV = LDV.MaLoaiDV
        JOIN #CancelledBookings C ON CT.MaDatSan = C.MaDatSan
        JOIN DATSAN DS ON C.MaDatSan = DS.MaDatSan
        JOIN SAN S ON DS.MaSan = S.MaSan
        WHERE KHO.MaCS = S.MaCS
          AND LDV.TenLoai NOT IN (N'Huấn luyện viên', N'Phòng VIP', N'Tủ đồ'); -- Không cộng tồn kho cho các loại này (quản lý bằng lịch)
        
        DROP TABLE #CancelledBookings;
    END
END
GO

-- Trigger: Hoàn trả tồn kho dịch vụ khi hủy phiếu đặt sân

GO
-- =============================================
-- PATCH: Bỏ qua phiếu NHÁP khi kiểm tra trùng lịch
-- Mục đích: Phiếu nháp chưa xác nhận nên không block sân
-- =============================================

USE TRUNGTAMTHETHAO;
GO

-- Sửa trigger kiểm tra trùng lịch
CREATE OR ALTER TRIGGER trg_KiemTraTrungLich
ON DATSAN
FOR INSERT, UPDATE
AS
BEGIN
    -- 1. KIỂM TRA GIỜ HOẠT ĐỘNG CỦA CƠ SỞ
    IF EXISTS (
        SELECT 1
        FROM inserted I
        JOIN SAN S ON I.MaSan = S.MaSan
        JOIN COSO CS ON S.MaCS = CS.MaCS
        JOIN PHIEUDATSAN P ON I.MaDatSan = P.MaDatSan
        WHERE (CS.GioMoCua IS NOT NULL AND P.GioBatDau < CS.GioMoCua)
           OR (CS.GioDongCua IS NOT NULL AND P.GioKetThuc > CS.GioDongCua)
    )
    BEGIN
        DECLARE @GioMoCua TIME, @GioDongCua TIME;
        SELECT TOP 1 @GioMoCua = CS.GioMoCua, @GioDongCua = CS.GioDongCua
        FROM inserted I
        JOIN SAN S ON I.MaSan = S.MaSan
        JOIN COSO CS ON S.MaCS = CS.MaCS;
        
        DECLARE @ErrorMsg NVARCHAR(200) = N'Lỗi: Giờ đặt sân phải nằm trong giờ hoạt động của cơ sở (' 
            + CONVERT(VARCHAR(5), @GioMoCua, 108) + N' - ' + CONVERT(VARCHAR(5), @GioDongCua, 108) + N')';
        RAISERROR (@ErrorMsg, 16, 1);
        ROLLBACK TRANSACTION;
        RETURN;
    END

    -- 2. KIỂM TRA TRÙNG LỊCH
    IF EXISTS (
        SELECT 1
        FROM inserted I
        JOIN PHIEUDATSAN P_Moi ON I.MaDatSan = P_Moi.MaDatSan -- Lấy giờ của phiếu vừa đặt
        JOIN DATSAN D_Cu ON I.MaSan = D_Cu.MaSan -- Join với các đơn đặt cũ cùng sân
        JOIN PHIEUDATSAN P_Cu ON D_Cu.MaDatSan = P_Cu.MaDatSan -- Lấy giờ của các phiếu cũ
        WHERE P_Cu.MaDatSan <> P_Moi.MaDatSan -- Khác chính nó
          AND P_Cu.NgayDat = P_Moi.NgayDat -- Cùng ngày
          AND P_Cu.TrangThai NOT IN (N'Đã hủy', N'No-Show', N'Nháp') -- BỎ QUA PHIẾU NHÁP
          AND (
              (P_Moi.GioBatDau >= P_Cu.GioBatDau AND P_Moi.GioBatDau < P_Cu.GioKetThuc) -- Giờ bắt đầu lọt vào ca cũ
              OR
              (P_Moi.GioKetThuc > P_Cu.GioBatDau AND P_Moi.GioKetThuc <= P_Cu.GioKetThuc) -- Giờ kết thúc lọt vào ca cũ
              OR
              (P_Moi.GioBatDau <= P_Cu.GioBatDau AND P_Moi.GioKetThuc >= P_Cu.GioKetThuc) -- Bao trùm ca cũ
          )
    )
    BEGIN
        RAISERROR (N'Lỗi: Sân này đã bị đặt trùng giờ với một phiếu khác!', 16, 1);
        ROLLBACK TRANSACTION;
    END
END;
GO

PRINT 'PATCH COMPLETED: Trigger trg_KiemTraTrungLich đã được cập nhật để bỏ qua phiếu Nháp';
