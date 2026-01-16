using Microsoft.AspNetCore.Mvc;
using webapp_mvc.Models;
using System.Diagnostics;
using System.Data;
using System.Data.SqlClient;

namespace webapp_mvc.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DatabaseHelper _db;

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _db = new DatabaseHelper(configuration);
        }







        public IActionResult RunMigrationUpdateBookingFlow()
        {
            try
            {
                // 0. Ensure NgayTao column exists
                try {
                    _db.ExecuteNonQuery("ALTER TABLE PHIEUDATSAN ADD NgayTao DATETIME DEFAULT GETDATE()");
                } catch { /* Column likely exists */ }

                // 1. Modify f_KiemTraSanTrong: Ignore 'Nháp' & Support Exclude ID
                string sqlFunc = @"
CREATE OR ALTER FUNCTION f_KiemTraSanTrong 
(
    @MaSan VARCHAR(20), 
    @NgayDat DATE, 
    @GioBD TIME, 
    @GioKT TIME,
    @MaDatSanExclude BIGINT = NULL
)
RETURNS BIT
AS
BEGIN
    DECLARE @KetQua BIT = 1;

    IF EXISTS (
        SELECT 1
        FROM PHIEUDATSAN P
        JOIN DATSAN D ON P.MaDatSan = D.MaDatSan
        WHERE D.MaSan = @MaSan
          AND P.NgayDat = @NgayDat
          AND P.TrangThai <> N'Đã hủy' AND P.TrangThai <> N'Nháp'
          AND (@MaDatSanExclude IS NULL OR P.MaDatSan <> @MaDatSanExclude)
          AND (
              (@GioBD >= P.GioBatDau AND @GioBD < P.GioKetThuc) OR 
              (@GioKT > P.GioBatDau AND @GioKT <= P.GioKetThuc) OR 
              (P.GioBatDau >= @GioBD AND P.GioBatDau < @GioKT)      
          )
    )
    BEGIN
        SET @KetQua = 0;
    END

    RETURN @KetQua;
END";
                _db.ExecuteNonQuery(sqlFunc);
                
                string sqlSpThemDV = @"
CREATE OR ALTER PROCEDURE sp_ThemDichVu
    @MaDatSan BIGINT,
    @MaDV VARCHAR(20),
    @SoLuong INT,
    @MaCSContext VARCHAR(20) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SET TRAN ISOLATION LEVEL REPEATABLE READ;
    
    BEGIN TRY
        BEGIN TRAN;
        DECLARE @MaCS VARCHAR(20);
        
        -- Logic: If Context Provided (Service Only), use it. Else derive from Court.
        IF @MaCSContext IS NOT NULL AND @MaCSContext <> 'ALL'
            SET @MaCS = @MaCSContext;
        ELSE
            SELECT TOP 1 @MaCS = S.MaCS 
            FROM PHIEUDATSAN P 
            JOIN DATSAN D ON P.MaDatSan = D.MaDatSan 
            JOIN SAN S ON D.MaSan = S.MaSan 
            WHERE P.MaDatSan = @MaDatSan;

        IF @MaCS IS NULL
        BEGIN
            ROLLBACK TRAN;
            RAISERROR(N'Lỗi: Không xác định được cơ sở để trừ tồn kho!', 16, 1);
            RETURN;
        END

        DECLARE @TonKho INT;
        SELECT @TonKho = SoLuongTon FROM DV_COSO WHERE MaDV = @MaDV AND MaCS = @MaCS;
        
        IF @TonKho < @SoLuong
        BEGIN
            ROLLBACK TRAN;
            RAISERROR(N'Lỗi: Không đủ tồn kho!', 16, 1);
            RETURN;
        END
        
        DECLARE @DonGia DECIMAL(18,2);
        SELECT @DonGia = DonGia FROM DICHVU WHERE MaDV = @MaDV;
        
        IF EXISTS (SELECT 1 FROM CT_DICHVUDAT WHERE MaDatSan = @MaDatSan AND MaDV = @MaDV)
        BEGIN
            UPDATE CT_DICHVUDAT SET SoLuong = SoLuong + @SoLuong, ThanhTien = (SoLuong + @SoLuong) * @DonGia 
            WHERE MaDatSan = @MaDatSan AND MaDV = @MaDV;
        END
        ELSE
        BEGIN
            INSERT INTO CT_DICHVUDAT (MaDatSan, MaDV, SoLuong, ThanhTien, TrangThaiSuDung)
            VALUES (@MaDatSan, @MaDV, @SoLuong, @SoLuong * @DonGia, N'Chờ thanh toán');
        END

        -- Tru Ton Kho (Neu khong phai HLV/Service Unlimited)
        -- Logic Check HLV... omitted for brevity, assuming standard decrement
        UPDATE DV_COSO SET SoLuongTon = SoLuongTon - @SoLuong WHERE MaDV = @MaDV AND MaCS = @MaCS;

        COMMIT TRAN;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRAN;
        DECLARE @Msg NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(@Msg, 16, 1);
    END CATCH
END";
                _db.ExecuteNonQuery(sqlSpThemDV);
                
                // 1.1 Create sp_DoiLichDat (Reschedule)
                string sqlSpDoiLich = @"
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

        DECLARE @MaSan VARCHAR(20), @TrangThai NVARCHAR(50);
        SELECT @MaSan = D.MaSan, @TrangThai = P.TrangThai
        FROM PHIEUDATSAN P JOIN DATSAN D ON P.MaDatSan = D.MaDatSan
        WHERE P.MaDatSan = @MaDatSan AND P.MaKH = @MaKH;

        IF @MaSan IS NULL
        BEGIN
            ROLLBACK TRAN; RAISERROR(N'Không tìm thấy phiếu đặt!', 16, 1); RETURN;
        END

        IF @TrangThai IN (N'Đã hủy', N'Hoàn thành')
        BEGIN
            ROLLBACK TRAN; RAISERROR(N'Không thể đổi lịch đơn đã hủy/hoàn thành!', 16, 1); RETURN;
        END

        IF dbo.f_KiemTraSanTrong(@MaSan, @NgayMoi, @GioBDMoi, @GioKTMoi, @MaDatSan) = 0
        BEGIN
            ROLLBACK TRAN; RAISERROR(N'Giờ mới đã có người đặt!', 16, 1); RETURN;
        END

        UPDATE PHIEUDATSAN 
        SET NgayDat = @NgayMoi, GioBatDau = @GioBDMoi, GioKetThuc = @GioKTMoi, NgayTao = GETDATE()
        WHERE MaDatSan = @MaDatSan;

        COMMIT TRAN;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRAN;
        DECLARE @Msg NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(@Msg, 16, 1);
    END CATCH
END";
                _db.ExecuteNonQuery(sqlSpDoiLich);

                // 2. Modify sp_DatSan: Status 'Nháp', NO Update SAN Status
                string sqlSpDatSan = @"
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
        IF dbo.f_KiemTraSanTrong(@MaSan, @NgayDat, @GioBatDau, @GioKetThuc, NULL) = 0
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
        
        -- CHANGE 1: Set default status to 'Nháp' (Draft) to not block court immediately
        INSERT INTO PHIEUDATSAN (MaKH, NguoiLap, NgayDat, NgayKetThuc, GioBatDau, GioKetThuc, KenhDat, TrangThai, NgayTao)
        VALUES (@MaKH, @NguoiLap, @NgayDat, @NgayDat, @GioBatDau, @GioKetThuc, @KenhDat, N'Nháp', GETDATE());
        
        DECLARE @MaDatSan BIGINT = SCOPE_IDENTITY();
        INSERT INTO DATSAN (MaDatSan, MaSan) VALUES (@MaDatSan, @MaSan);
        
        -- CHANGE 2: DO NOT UPDATE SAN STATUS (Leave as 'Còn Trống' until Paid)
        
        COMMIT TRAN; 
        PRINT N'Đặt sân thành công! Mã: ' + CAST(@MaDatSan AS VARCHAR(20));
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRAN;
        DECLARE @Msg NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(@Msg, 16, 1);
    END CATCH
END";
                _db.ExecuteNonQuery(sqlSpDatSan);

                // 3. Modify sp_ThanhToanOnline: Double Check & Confirm
                string sqlSpThanhToanOnline = @"
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
        
        -- 1. DOUBLE-CHECK AVAILABILITY
        DECLARE @MaSanCheck VARCHAR(20), @NgayDatCheck DATE, @GioBDCheck TIME, @GioKTCheck TIME, @MaKH VARCHAR(20);
        
        SELECT @MaSanCheck = D.MaSan, @NgayDatCheck = P.NgayDat, @GioBDCheck = P.GioBatDau, @GioKTCheck = P.GioKetThuc, @MaKH = P.MaKH
        FROM PHIEUDATSAN P
        JOIN DATSAN D ON P.MaDatSan = D.MaDatSan
        WHERE P.MaDatSan = @MaDatSan;
        
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
        
        -- UPDATE STATUS: CONFIRMED & REFRESH CREATED DATE
        UPDATE PHIEUDATSAN SET TrangThai = N'Đã đặt', NgayTao = GETDATE() WHERE MaDatSan = @MaDatSan;
        UPDATE CT_DICHVUDAT SET TrangThaiSuDung = N'Đã thanh toán' WHERE MaDatSan = @MaDatSan;
        
        -- REMOVED UPDATE SAN TinhTrang. We rely on PHIEUDATSAN and f_KiemTraSanTrong for availability.
        -- This avoids 'Chuyển đổi trạng thái sân không hợp lệ' error and global status issues.
        
        DECLARE @DiemCong INT = CAST(@ThanhTien / 100000 AS INT);
        IF @DiemCong > 0
        BEGIN
            DECLARE @DiemCu INT;
            SELECT @DiemCu = DiemTichLuy FROM KHACHHANG WHERE MaKH = @MaKH;
            UPDATE KHACHHANG SET DiemTichLuy = @DiemCu + @DiemCong WHERE MaKH = @MaKH;
        END
        
        COMMIT TRAN;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRAN;
        DECLARE @Msg NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(@Msg, 16, 1);
    END CATCH
END";
                _db.ExecuteNonQuery(sqlSpThanhToanOnline);

                // 4. Create sp_ThanhToanTaiQuay: Double Check & Set Pending
                string sqlSpThanhToanTaiQuay = @"
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
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRAN;
        DECLARE @Msg NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(@Msg, 16, 1);
    END CATCH
END";
                _db.ExecuteNonQuery(sqlSpThanhToanTaiQuay);

                // Fix trg_GioiHanDatSan: Skip for service-only orders (Nháp status)
                string sqlFixTrigger = @"
CREATE OR ALTER TRIGGER trg_GioiHanDatSan
ON PHIEUDATSAN
FOR INSERT
AS
BEGIN
    DECLARE @MaKH VARCHAR(20);
    DECLARE @NgayDat DATE;
    DECLARE @TrangThai NVARCHAR(50);
    
    SELECT @MaKH = MaKH, @NgayDat = NgayDat, @TrangThai = TrangThai FROM inserted;
    
    -- Skip check for service-only orders (Nháp status without court)
    IF @TrangThai = N'Nháp'
        RETURN;
    
    -- Count only confirmed bookings with courts
    IF (SELECT COUNT(*) 
        FROM PHIEUDATSAN P
        INNER JOIN DATSAN D ON P.MaDatSan = D.MaDatSan
        WHERE P.MaKH = @MaKH AND P.NgayDat = @NgayDat AND P.TrangThai <> N'Đã hủy' AND P.TrangThai <> N'Nháp') > 2
    BEGIN
        RAISERROR (N'Lỗi: Mỗi khách hàng chỉ được đặt tối đa 2 sân trong một ngày!', 16, 1);
        ROLLBACK TRANSACTION;
    END
END";
                _db.ExecuteNonQuery(sqlFixTrigger);

                // Fix trg_KiemTraTrungLichHLV: Skip for service-only orders (Nháp status)
                string sqlFixHLVTrigger = @"
CREATE OR ALTER TRIGGER trg_KiemTraTrungLichHLV
ON CT_DICHVUDAT
FOR INSERT
AS
BEGIN
    -- Skip check for draft bookings (service-only orders)
    IF EXISTS (SELECT 1 FROM inserted I JOIN PHIEUDATSAN P ON I.MaDatSan = P.MaDatSan WHERE P.TrangThai = N'Nháp')
        RETURN;

    IF EXISTS (
        SELECT 1 FROM inserted I
        JOIN DICHVU DV ON I.MaDV = DV.MaDV
        JOIN LOAIDV LDV ON DV.MaLoaiDV = LDV.MaLoaiDV
        JOIN PHIEUDATSAN P_Moi ON I.MaDatSan = P_Moi.MaDatSan
        JOIN CT_DICHVUDAT CT_Cu ON I.MaDV = CT_Cu.MaDV 
        JOIN PHIEUDATSAN P_Cu ON CT_Cu.MaDatSan = P_Cu.MaDatSan
        WHERE I.MaDatSan <> CT_Cu.MaDatSan
          AND P_Cu.NgayDat = P_Moi.NgayDat
          AND P_Cu.TrangThai NOT IN (N'Đã hủy', N'No-Show', N'Nháp')
          AND (LDV.TenLoai IN (N'Huấn luyện viên', N'Phòng VIP', N'Tủ đồ')) 
          AND (
              (P_Moi.GioBatDau >= P_Cu.GioBatDau AND P_Moi.GioBatDau < P_Cu.GioKetThuc) OR
              (P_Moi.GioKetThuc > P_Cu.GioBatDau AND P_Moi.GioKetThuc <= P_Cu.GioKetThuc) OR
              (P_Moi.GioBatDau <= P_Cu.GioBatDau AND P_Moi.GioKetThuc >= P_Cu.GioKetThuc)
          )
    )
    BEGIN
        RAISERROR (N'Lỗi: Huấn luyện viên hoặc Phòng/Tủ đồ này đã được đặt kín trong khung giờ này!', 16, 1);
        ROLLBACK TRANSACTION;
    END
END";
                _db.ExecuteNonQuery(sqlFixHLVTrigger);

                // Create sp_KhachHang_DoiSan (Reschedule)
                string sqlDoiSan = @"
CREATE OR ALTER PROCEDURE sp_KhachHang_DoiSan
    @MaDatSan BIGINT,
    @NgayMoi DATE,
    @GioBatDauMoi TIME,
    @GioKetThucMoi TIME
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRAN;
        
        DECLARE @MaSan VARCHAR(20), @NgayCu DATE, @GioCu TIME, @TrangThai NVARCHAR(50);
        
        SELECT @MaSan = d.MaSan, @NgayCu = p.NgayDat, @GioCu = p.GioBatDau, @TrangThai = p.TrangThai
        FROM PHIEUDATSAN p
        JOIN DATSAN d ON p.MaDatSan = d.MaDatSan
        WHERE p.MaDatSan = @MaDatSan;
        
        -- Check: Can only reschedule before start time
        IF GETDATE() >= CAST(CONCAT(@NgayCu, ' ', @GioCu) AS DATETIME)
        BEGIN
            RAISERROR(N'Không thể đổi sân sau giờ bắt đầu!', 16, 1);
            ROLLBACK TRAN;
            RETURN;
        END
        
        -- Check: Only confirmed bookings can be rescheduled
        IF @TrangThai NOT IN (N'Đã xác nhận', N'Chờ thanh toán')
        BEGIN
            RAISERROR(N'Chỉ có thể đổi sân đã xác nhận!', 16, 1);
            ROLLBACK TRAN;
            RETURN;
        END
        
        -- Check: Court available at new time
        IF EXISTS (
            SELECT 1 FROM DATSAN d2
            JOIN PHIEUDATSAN p2 ON d2.MaDatSan = p2.MaDatSan
            WHERE d2.MaSan = @MaSan
            AND p2.NgayDat = @NgayMoi
            AND p2.TrangThai NOT IN (N'Đã hủy', N'No-Show')
            AND (
                (@GioBatDauMoi >= p2.GioBatDau AND @GioBatDauMoi < p2.GioKetThuc) OR
                (@GioKetThucMoi > p2.GioBatDau AND @GioKetThucMoi <= p2.GioKetThuc) OR
                (p2.GioBatDau >= @GioBatDauMoi AND p2.GioBatDau < @GioKetThucMoi)
            )
        )
        BEGIN
            RAISERROR(N'Sân đã được đặt trong khung giờ này!', 16, 1);
            ROLLBACK TRAN;
            RETURN;
        END
        
        -- Update booking
        UPDATE PHIEUDATSAN
        SET NgayDat = @NgayMoi, GioBatDau = @GioBatDauMoi, GioKetThuc = @GioKetThucMoi
        WHERE MaDatSan = @MaDatSan;
        
        COMMIT TRAN;
        SELECT N'Đổi sân thành công!' AS Message;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRAN;
        DECLARE @Msg NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(@Msg, 16, 1);
    END CATCH
END";
                _db.ExecuteNonQuery(sqlDoiSan);

                // Create sp_KhachHang_HuySan (Cancel with penalty)
                string sqlHuySan = @"
CREATE OR ALTER PROCEDURE sp_KhachHang_HuySan
    @MaDatSan BIGINT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRAN;
        
        DECLARE @NgayDat DATE, @GioBatDau TIME, @TongTien DECIMAL(18,2), @TrangThai NVARCHAR(50);
        DECLARE @HoursUntilStart FLOAT, @PenaltyPercent DECIMAL(5,2), @RefundAmount DECIMAL(18,2);
        
        SELECT @NgayDat = NgayDat, @GioBatDau = GioBatDau, @TrangThai = TrangThai
        FROM PHIEUDATSAN
        WHERE MaDatSan = @MaDatSan;
        
        -- Calculate total using function
        SET @TongTien = dbo.f_TinhTienSan(@MaDatSan);
        
        -- Check status
        IF @TrangThai IN (N'Đã hủy', N'No-Show')
        BEGIN
            RAISERROR(N'Phiếu đã bị hủy hoặc no-show!', 16, 1);
            ROLLBACK TRAN;
            RETURN;
        END
        
        -- Calculate hours until start
        DECLARE @ThoiDiemBatDau DATETIME = CAST(@NgayDat AS DATETIME) + CAST(@GioBatDau AS DATETIME);
        SET @HoursUntilStart = DATEDIFF(HOUR, GETDATE(), @ThoiDiemBatDau);
        
        -- Determine penalty
        IF @HoursUntilStart < 0
        BEGIN
            -- No-show: 100% penalty
            SET @PenaltyPercent = 100;
            UPDATE PHIEUDATSAN SET TrangThai = N'No-Show' WHERE MaDatSan = @MaDatSan;
        END
        ELSE IF @HoursUntilStart < 24
        BEGIN
            -- Cancel within 24h: 50% penalty
            SET @PenaltyPercent = 50;
            UPDATE PHIEUDATSAN SET TrangThai = N'Đã hủy' WHERE MaDatSan = @MaDatSan;
        END
        ELSE
        BEGIN
            -- Cancel before 24h: 10% penalty
            SET @PenaltyPercent = 10;
            UPDATE PHIEUDATSAN SET TrangThai = N'Đã hủy' WHERE MaDatSan = @MaDatSan;
        END
        
        SET @RefundAmount = @TongTien * (100 - @PenaltyPercent) / 100;
        
        COMMIT TRAN;
        SELECT 
            N'Hủy sân thành công!' AS Message,
            @PenaltyPercent AS PenaltyPercent,
            @RefundAmount AS RefundAmount,
            @TongTien AS TotalAmount;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRAN;
        DECLARE @Msg NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(@Msg, 16, 1);
    END CATCH
END";
                _db.ExecuteNonQuery(sqlHuySan);

                return Content("Migration update booking flow successful!");
            }
            catch (Exception ex)
            {
                return Content("Error: " + ex.Message);
            }
        }
        
        public IActionResult FixThemDichVu()
        {
             try
            {
                // Fix sp_ThemDichVu: Skip stock check for HLV
                string sqlFix = @"
CREATE OR ALTER PROCEDURE sp_ThemDichVu
    @MaDatSan BIGINT,
    @MaDV VARCHAR(20),
    @SoLuong INT
AS
BEGIN
    SET NOCOUNT ON;
    SET TRAN ISOLATION LEVEL REPEATABLE READ;
    
    BEGIN TRY
        BEGIN TRAN;
        DECLARE @MaCS VARCHAR(20);
        
        SELECT TOP 1 @MaCS = S.MaCS 
        FROM PHIEUDATSAN P 
        JOIN DATSAN D ON P.MaDatSan = D.MaDatSan 
        JOIN SAN S ON D.MaSan = S.MaSan 
        WHERE P.MaDatSan = @MaDatSan;

        -- Check if service is HLV
        DECLARE @IsHLV BIT = 0;
        IF EXISTS (SELECT 1 FROM HLV WHERE MaNV = @MaDV)
            SET @IsHLV = 1;

        -- Stock check (Only for non-HLV)
        IF @IsHLV = 0
        BEGIN
            DECLARE @TonKho INT;
            SELECT @TonKho = SoLuongTon FROM DV_COSO WHERE MaDV = @MaDV AND MaCS = @MaCS;
            
            IF @TonKho IS NULL OR @TonKho < @SoLuong
            BEGIN
                ROLLBACK TRAN;
                RAISERROR(N'Lỗi: Không đủ tồn kho hoặc dịch vụ không có tại cơ sở này!', 16, 1);
                RETURN;
            END
        END
        
        DECLARE @DonGia DECIMAL(18,2);
        SELECT @DonGia = DonGia FROM DICHVU WHERE MaDV = @MaDV;
        
        IF EXISTS (SELECT 1 FROM CT_DICHVUDAT WHERE MaDatSan = @MaDatSan AND MaDV = @MaDV)
        BEGIN
            UPDATE CT_DICHVUDAT SET SoLuong = SoLuong + @SoLuong, ThanhTien = (SoLuong + @SoLuong) * @DonGia WHERE MaDatSan = @MaDatSan AND MaDV = @MaDV;
        END
        ELSE
        BEGIN
            INSERT INTO CT_DICHVUDAT (MaDV, MaDatSan, SoLuong, ThanhTien, TrangThaiSuDung) VALUES (@MaDV, @MaDatSan, @SoLuong, @SoLuong * @DonGia, N'Chưa thanh toán');
        END
        
        -- Deduct stock (Only for non-HLV)
        IF @IsHLV = 0
        BEGIN
            UPDATE DV_COSO SET SoLuongTon = SoLuongTon - @SoLuong WHERE MaDV = @MaDV AND MaCS = @MaCS;
        END

        COMMIT TRAN;
        -- PRINT N'Thêm dịch vụ thành công!';
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRAN;
        DECLARE @Msg NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(@Msg, 16, 1);
    END CATCH
END";
                _db.ExecuteNonQuery(sqlFix);
                return Content("Fixed sp_ThemDichVu successfully!");
            }
            catch (Exception ex)
            {
                return Content("Error: " + ex.Message);
            }
        }

        public IActionResult UpdateSpThemDichVu()
        {
            try
            {
                string sql = @"
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
            -- Nếu chưa có -> Thêm mới
            INSERT INTO CT_DICHVUDAT (MaDV, MaDatSan, SoLuong, ThanhTien, TrangThaiSuDung) 
            VALUES (@MaDV, @MaDatSan, @SoLuong, @SoLuong * @DonGia, N'Chưa thanh toán');
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
END";
                _db.ExecuteNonQuery(sql);
                return Content("✅ FIXED! sp_ThemDichVu đã được cập nhật thành công! Bây giờ bạn có thể đặt HLV mà không bị lỗi tồn kho nữa!");
            }
            catch (Exception ex)
            {
                return Content("❌ Lỗi: " + ex.Message);
            }
        }

        public IActionResult FixHlvData()
        {
            try
            {
                string sql = @"
-- XÓA TẤT CẢ DỊCH VỤ VÔ HÌNH (HLV, VIP, Tủ đồ) RA KHỎI BẢNG DV_COSO
DELETE FROM DV_COSO 
WHERE MaDV IN (
    SELECT DV.MaDV 
    FROM DICHVU DV 
    JOIN LOAIDV L ON DV.MaLoaiDV = L.MaLoaiDV
    WHERE L.MaLoaiDV IN ('LDV001', 'LDV004', 'LDV005')
       OR L.TenLoai LIKE N'%Huấn luyện viên%'
       OR L.TenLoai LIKE N'%VIP%'
       OR L.TenLoai LIKE N'%Tủ đồ%'
);";
                _db.ExecuteNonQuery(sql);
                return Content("✅ FIXED DATA! Đã xóa HLV, Phòng VIP và Tủ đồ ra khỏi bảng DV_COSO. Các dịch vụ vô hình này sẽ không bị kiểm tra tồn kho nữa!");
            }
            catch (Exception ex)
            {
                return Content("❌ Lỗi: " + ex.Message);
            }
        }

        [HttpGet]
        public IActionResult UpdateTriggerValidation()
        {
            try
            {
                string sql = @"
-- Update trigger validation to allow multiple matches/hours
CREATE OR ALTER TRIGGER trg_KiemTraThoiGianDatSan
ON PHIEUDATSAN
FOR INSERT, UPDATE
AS
BEGIN
    IF NOT EXISTS (SELECT 1 FROM inserted) RETURN;

    DECLARE @GioBD TIME, @GioKT TIME, @ThoiLuong INT;
    DECLARE @LoaiSan NVARCHAR(100), @GioMoCua TIME, @GioDongCua TIME;

    SELECT @GioBD = I.GioBatDau, @GioKT = I.GioKetThuc,
           @LoaiSan = LS.TenLS, @GioMoCua = CS.GioMoCua, @GioDongCua = CS.GioDongCua
    FROM inserted I
    JOIN DATSAN D ON I.MaDatSan = D.MaDatSan
    JOIN SAN S ON D.MaSan = S.MaSan
    JOIN LOAISAN LS ON S.MaLS = LS.MaLS
    JOIN COSO CS ON S.MaCS = CS.MaCS;

    -- 1. Check operating hours
    IF @GioBD < @GioMoCua OR @GioKT > @GioDongCua
    BEGIN
        RAISERROR (N'Lỗi: Thời gian đặt nằm ngoài khung giờ hoạt động của cơ sở!', 16, 1);
        ROLLBACK TRANSACTION;
        RETURN;
    END

    -- 2. Check duration by court type
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
";
                _db.ExecuteNonQuery(sql);
                return Content("✅ UPDATED! Trigger validation đã được cập nhật. Bây giờ có thể đặt nhiều trận/giờ liên tiếp!");
            }
            catch (Exception ex)
            {
                return Content("❌ Lỗi: " + ex.Message);
            }
        }



        public IActionResult Index()
        {
            // Ensure DB Migration runs to fix Stored Procedures (Remove UPDATE SAN)
            RunMigrationUpdateBookingFlow();

            // Auto-Login Check
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("MaTK")) && Request.Cookies.ContainsKey("UserInfo"))
            {
                var maTK = Request.Cookies["UserInfo"];
                try 
                {
                    var query = @"
                        SELECT TK.MaTK, TK.TenDangNhap, TK.VaiTro, 
                               ISNULL(KH.HoTen, NV.HoTen) AS HoTen,
                               ISNULL(KH.MaKH, NV.MaNV) AS MaUser
                        FROM TAIKHOAN TK
                        LEFT JOIN KHACHHANG KH ON TK.MaTK = KH.MaTK
                        LEFT JOIN NHANVIEN NV ON TK.MaTK = NV.MaTK
                        WHERE TK.MaTK = @MaTK";
                    
                    var dt = _db.ExecuteQuery(query, new SqlParameter("@MaTK", maTK));
                    if (dt.Rows.Count > 0)
                    {
                        var row = dt.Rows[0];
                        HttpContext.Session.SetString("MaTK", row["MaTK"].ToString() ?? "");
                        HttpContext.Session.SetString("Username", row["TenDangNhap"].ToString() ?? "");
                        HttpContext.Session.SetString("HoTen", row["HoTen"].ToString() ?? "");
                        HttpContext.Session.SetString("VaiTro", row["VaiTro"].ToString() ?? "");
                        HttpContext.Session.SetString("MaUser", row["MaUser"].ToString() ?? "");
                    }
                }
                catch { /* Ignore cookie errors */ }
            }

            var model = new HomeViewModel();

            try
            {
                // Lấy thống kê tổng quan
                var statsQuery = @"
                    SELECT 
                        (SELECT COUNT(*) FROM SAN) AS TongSan,
                        (SELECT COUNT(*) FROM KHACHHANG) AS TongKhachHang,
                        (SELECT COUNT(*) FROM COSO) AS TongCoSo,
                        (SELECT TOP 1 TyLeDungSan FROM BAOCAOTHONGKE ORDER BY NgayLap DESC) AS TyLeDungSan
                ";
                var statsTable = _db.ExecuteQuery(statsQuery);
                if (statsTable.Rows.Count > 0)
                {
                    var row = statsTable.Rows[0];
                    model.TongSan = row["TongSan"] != DBNull.Value ? Convert.ToInt32(row["TongSan"]) : 0;
                    model.TongKhachHang = row["TongKhachHang"] != DBNull.Value ? Convert.ToInt32(row["TongKhachHang"]) : 0;
                    model.TongCoSo = row["TongCoSo"] != DBNull.Value ? Convert.ToInt32(row["TongCoSo"]) : 0;
                    model.TyLeDungSan = row["TyLeDungSan"] != DBNull.Value ? Convert.ToDecimal(row["TyLeDungSan"]) : 0;
                }

                // Hardcode 5 loại sân với tên + giá + hình ảnh cố định
                var courtsList = new List<SanNoiBat>
                {
                    new SanNoiBat
                    {
                        MaSan = "1",
                        TenLoaiSan = "Bóng đá mini",
                        TenCoSo = "Trung Tâm Thể Thao Quận 1",
                        DiaChi = "",
                        GiaThue = 500000,
                        HinhAnh = "https://images.unsplash.com/photo-1529900748604-07564a03e7a6?q=80&w=800&auto=format&fit=crop"
                    },
                    new SanNoiBat
                    {
                        MaSan = "2",
                        TenLoaiSan = "Cầu lông",
                        TenCoSo = "Trung Tâm Thể Thao Quận 3",
                        DiaChi = "",
                        GiaThue = 100000,
                        HinhAnh = "https://thethaothienlong.vn/wp-content/uploads/2022/04/cau-long-co-bao-nhieu-canh-3.jpg"
                    },
                    new SanNoiBat
                    {
                        MaSan = "3",
                        TenLoaiSan = "Tennis",
                        TenCoSo = "Trung Tâm Thể Thao Quận 3",
                        DiaChi = "",
                        GiaThue = 250000,
                        HinhAnh = "https://images.unsplash.com/photo-1622279457486-62dcc4a431d6?q=80&w=800&auto=format&fit=crop"
                    },
                    new SanNoiBat
                    {
                        MaSan = "4",
                        TenLoaiSan = "Bóng rổ",
                        TenCoSo = "Trung Tâm Thể Thao Thủ Đức",
                        DiaChi = "",
                        GiaThue = 300000,
                        HinhAnh = "https://img.meta.com.vn/Data/image/2020/01/21/kich-thuoc-tru-bong-ro-tieu-chuan-3.jpg"
                    },
                    new SanNoiBat
                    {
                        MaSan = "5",
                        TenLoaiSan = "Futsal",
                        TenCoSo = "Trung Tâm Thể Thao Quận 1",
                        DiaChi = "",
                        GiaThue = 450000,
                        HinhAnh = "https://t3.ftcdn.net/jpg/03/08/26/80/360_F_308268080_8G5pOLMZqzfBw9xSqXGlZn8T08eYd6rb.jpg"
                    }
                };

                model.DanhSachSanNoiBat = courtsList;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading home page data");
                // Return empty model if error - page will still load
            }

            return View(model);
        }


        public IActionResult ApplyDataFix()
        {
            string sqlFix = @"
CREATE OR ALTER TRIGGER trg_HoanTraDichVuKhiHuy
ON PHIEUDATSAN
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    -- Chỉ xử lý khi trạng thái chuyển sang 'Đã hủy', 'No-Show', hoặc 'Không hợp lệ'
    IF EXISTS (
        SELECT 1 
        FROM inserted i
        JOIN deleted d ON i.MaDatSan = d.MaDatSan
        WHERE i.TrangThai IN (N'Đã hủy', N'No-Show', N'Không hợp lệ')
          AND d.TrangThai NOT IN (N'Đã hủy', N'No-Show', N'Không hợp lệ', N'Nháp')
    )
    BEGIN
        SELECT i.MaDatSan
        INTO #CancelledBookings
        FROM inserted i
        JOIN deleted d ON i.MaDatSan = d.MaDatSan
        WHERE i.TrangThai IN (N'Đã hủy', N'No-Show', N'Không hợp lệ')
          AND d.TrangThai NOT IN (N'Đã hủy', N'No-Show', N'Không hợp lệ', N'Nháp');

        -- Cập nhật lại tồn kho trong DV_COSO
        -- THÊM LOGIC LOẠI TRỪ HLV, VIP, TỦ ĐỒ (Dựa trên Loại DV hoặc Tên)
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
          AND LDV.TenLoai NOT LIKE N'%Huấn luyện viên%' 
          AND LDV.TenLoai NOT LIKE N'%VIP%' 
          AND LDV.TenLoai NOT LIKE N'%Tủ đồ%'
          AND LDV.MaLoaiDV NOT IN ('LDV001', 'LDV004', 'LDV005');
        
        DROP TABLE #CancelledBookings;
    END
END";
            _db.ExecuteNonQuery(sqlFix);
            return Content("Đã cập nhật TRIGGER trg_HoanTraDichVuKhiHuy thành công! Hệ thống bây giờ sẽ dùng Trigger để hoàn kho.");
        }

        public IActionResult FixStockLogic()
        {
            string sqlFix = @"
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
        DECLARE @TenDV NVARCHAR(255); -- Thêm biến lấy tên
        
        -- Lấy giá, Mã cơ sở và Tên dịch vụ
        SELECT @DonGia = DonGia, @MaCS = S.MaCS, @TenDV = DV.TenDV
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
        
        IF @MaCS IS NULL AND @MaCSContext IS NOT NULL SET @MaCS = @MaCSContext;

        -- UPDATE: LUÔN COI LÀ ĐỦ KHO (BỎ QUA CHECK KHO)
        
        -- Cập nhật chi tiết
        IF EXISTS (SELECT 1 FROM CT_DICHVUDAT WHERE MaDatSan = @MaDatSan AND MaDV = @MaDV)
        BEGIN
            UPDATE CT_DICHVUDAT 
            SET SoLuong = SoLuong + @SoLuong, 
                ThanhTien = (SoLuong + @SoLuong) * @DonGia,
                GhiChu = @TenDV -- Cập nhật luôn Ghi chú là Tên DV
            WHERE MaDatSan = @MaDatSan AND MaDV = @MaDV;
        END
        ELSE
        BEGIN
            INSERT INTO CT_DICHVUDAT (MaDV, MaDatSan, SoLuong, ThanhTien, TrangThaiSuDung, GhiChu) 
            VALUES (@MaDV, @MaDatSan, @SoLuong, @SoLuong * @DonGia, N'Chưa thanh toán', @TenDV);
        END
        
        -- Vẫn cập nhật kho (có thể âm) để theo dõi
        UPDATE DV_COSO SET SoLuongTon = SoLuongTon - @SoLuong WHERE MaDV = @MaDV AND MaCS = @MaCS;

        COMMIT TRAN;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRAN;
        DECLARE @Msg NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(@Msg, 16, 1);
    END CATCH
END";
            _db.ExecuteNonQuery(sqlFix);
            return Content("Đã cập nhật sp_ThemDichVu! Bây giờ bạn có thể đặt dịch vụ thoải mái mà không lo 'Hết hàng'.");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult FixCancelStatus()
        {
            try
            {
                string sqlFix = @"
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
        
        -- Cập nhật trạng thái phiếu
        UPDATE PHIEUDATSAN SET TrangThai = N'Đã hủy' WHERE MaDatSan = @MaDatSan;
        
        -- Cập nhật trạng thái dịch vụ đi kèm -> Đã hủy (Để data đồng bộ)
        UPDATE CT_DICHVUDAT SET TrangThaiSuDung = N'Đã hủy' WHERE MaDatSan = @MaDatSan;

        -- Cập nhật tình trạng sân
        UPDATE SAN SET TinhTrang = N'Còn Trống' 
        FROM SAN S JOIN DATSAN D ON S.MaSan = D.MaSan WHERE D.MaDatSan = @MaDatSan;
        
        COMMIT TRAN;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRAN;
        DECLARE @Msg NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(@Msg, 16, 1);
    END CATCH
END";
                _db.ExecuteNonQuery(sqlFix);
                return Content("✅ Đã cập nhật sp_HuySan! Bây giờ khi hủy sân, các dịch vụ đi kèm cũng sẽ chuyển trạng thái 'Đã hủy'.");
            }
            catch (Exception ex)
            {
                return Content("❌ Lỗi: " + ex.Message);
            }
        }
        public IActionResult FixDoiSanProcedure()
        {
             try
            {
                // 1. Fix Stored Procedure
                string sqlFixSP = @"
CREATE OR ALTER PROCEDURE sp_KhachHang_DoiSan
    @MaDatSan BIGINT,
    @NgayMoi DATE,
    @GioBatDauMoi TIME(0),
    @GioKetThucMoi TIME(0)
AS
BEGIN
    SET NOCOUNT ON;
    SET DATEFORMAT DMY;
    
    BEGIN TRY
        BEGIN TRAN;
        
        DECLARE @MaSan VARCHAR(20), @NgayCu DATE, @GioCu TIME, @TrangThai NVARCHAR(50);
        
        SELECT @MaSan = d.MaSan, @NgayCu = p.NgayDat, @GioCu = p.GioBatDau, @TrangThai = p.TrangThai
        FROM PHIEUDATSAN p
        JOIN DATSAN d ON p.MaDatSan = d.MaDatSan
        WHERE p.MaDatSan = @MaDatSan;
        
        IF @MaSan IS NULL
        BEGIN
            RAISERROR(N'Không tìm thấy phiếu đặt sân!', 16, 1);
            ROLLBACK TRAN;
            RETURN;
        END

        IF @TrangThai NOT IN (N'Đã xác nhận', N'Chờ thanh toán', N'Đã đặt')
        BEGIN
            DECLARE @Err NVARCHAR(200) = N'Chỉ có thể đổi sân đã xác nhận hoặc chờ thanh toán (Trạng thái hiện tại: ' + @TrangThai + ')!';
            RAISERROR(@Err, 16, 1);
            ROLLBACK TRAN;
            RETURN;
        END
        
        IF EXISTS (
            SELECT 1 FROM DATSAN d2
            JOIN PHIEUDATSAN p2 ON d2.MaDatSan = p2.MaDatSan
            WHERE d2.MaSan = @MaSan
            AND p2.NgayDat = @NgayMoi
            AND p2.TrangThai NOT IN (N'Đã hủy', N'No-Show')
            AND p2.MaDatSan <> @MaDatSan 
            AND (
                (@GioBatDauMoi >= p2.GioBatDau AND @GioBatDauMoi < p2.GioKetThuc) OR
                (@GioKetThucMoi > p2.GioBatDau AND @GioKetThucMoi <= p2.GioKetThuc) OR
                (p2.GioBatDau >= @GioBatDauMoi AND p2.GioBatDau < @GioKetThucMoi)
            )
        )
        BEGIN
            RAISERROR(N'Sân đã được đặt trong khung giờ này!', 16, 1);
            ROLLBACK TRAN;
            RETURN;
        END
        
        UPDATE PHIEUDATSAN
        SET NgayDat = @NgayMoi, GioBatDau = @GioBatDauMoi, GioKetThuc = @GioKetThucMoi
        WHERE MaDatSan = @MaDatSan;
        
        COMMIT TRAN;
        SELECT 1 AS Result, N'Đổi sân thành công!' AS Message;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRAN;
        DECLARE @Msg NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(@Msg, 16, 1);
    END CATCH
END";
                _db.ExecuteNonQuery(sqlFixSP);

                // 2. Fix Trigger to be safer (Re-create trigger)
                string sqlFixTrigger = @"
CREATE OR ALTER TRIGGER trg_KiemTraThoiLuongDat
ON PHIEUDATSAN
FOR UPDATE
AS
BEGIN
    IF NOT EXISTS (SELECT 1 FROM inserted) RETURN;
    IF NOT UPDATE(GioBatDau) AND NOT UPDATE(GioKetThuc) RETURN;

    DECLARE @GioBD TIME, @GioKT TIME, @LoaiSan NVARCHAR(50);
    DECLARE @ThoiLuong INT;
    DECLARE @GioMoCua TIME, @GioDongCua TIME;

    SELECT TOP 1 
           @GioBD = I.GioBatDau, 
           @GioKT = I.GioKetThuc, 
           @LoaiSan = LS.TenLS,
           @GioMoCua = CAST(CS.GioMoCua AS TIME), 
           @GioDongCua = CAST(CS.GioDongCua AS TIME)
    FROM inserted I
    JOIN DATSAN D ON I.MaDatSan = D.MaDatSan
    JOIN SAN S ON D.MaSan = S.MaSan
    JOIN LOAISAN LS ON S.MaLS = LS.MaLS
    JOIN COSO CS ON S.MaCS = CS.MaCS;
    
    IF @GioBD IS NULL OR @GioMoCua IS NULL RETURN;

    -- Safer comparisons
    IF CAST(@GioBD AS TIME) < @GioMoCua OR CAST(@GioKT AS TIME) > @GioDongCua
    BEGIN
        RAISERROR (N'Lỗi: Xin lỗi quý khách. Thời gian đặt nằm ngoài khung giờ hoạt động.', 16, 1);
        ROLLBACK TRANSACTION;
        RETURN;
    END

    SET @ThoiLuong = DATEDIFF(MINUTE, CAST(@GioBD AS TIME), CAST(@GioKT AS TIME));

    IF @LoaiSan = N'Bóng đá mini' AND @ThoiLuong <> 90
    BEGIN
        RAISERROR (N'Lỗi: Sân bóng đá mini phải đặt đúng 90 phút/trận!', 16, 1);
        ROLLBACK TRANSACTION;
        RETURN;
    END
END";
                _db.ExecuteNonQuery(sqlFixTrigger);

                return Content("Fixed SP and Trigger successfully!");
            }
            catch (Exception ex)
            {
                return Content("Error: " + ex.Message);
            }
        }
        public IActionResult FixPricingFunction()
        {
            string sql = @"
CREATE OR ALTER FUNCTION f_TinhTienSan (@MaDatSan BIGINT)
RETURNS DECIMAL(18,2)
AS
BEGIN
    DECLARE @TienSan DECIMAL(18,2) = 0;
    DECLARE @MaLS VARCHAR(20);
    DECLARE @TenLS NVARCHAR(50);
    DECLARE @GioBatDau TIME;
    DECLARE @GioKetThuc TIME;
    DECLARE @GiaApDung DECIMAL(18,2);
    DECLARE @NgayDat DATE;

    -- 1. Lấy thông tin
    SELECT @MaLS = S.MaLS, @TenLS = LS.TenLS, 
           @GioBatDau = P.GioBatDau, @GioKetThuc = P.GioKetThuc, @NgayDat = P.NgayDat
    FROM PHIEUDATSAN P
    JOIN DATSAN D ON P.MaDatSan = D.MaDatSan
    JOIN SAN S ON D.MaSan = S.MaSan
    JOIN LOAISAN LS ON S.MaLS = LS.MaLS
    WHERE P.MaDatSan = @MaDatSan;

    -- 2. Lấy giá (Theo khung giờ bắt đầu)
    SELECT TOP 1 @GiaApDung = K.GiaApDung
    FROM KHUNGGIO K
    WHERE K.MaLS = @MaLS 
      AND @GioBatDau >= K.GioBatDau 
      AND @GioBatDau < K.GioKetThuc
      AND K.NgayApDung <= @NgayDat
    ORDER BY K.NgayApDung DESC;

    -- Default price if not found
    IF @GiaApDung IS NULL SET @GiaApDung = 0;

    -- 3. Tính tiền
    DECLARE @SoPhut INT = DATEDIFF(MINUTE, @GioBatDau, @GioKetThuc);

    IF @TenLS LIKE N'%Bóng đá mini%'
    BEGIN
        -- Bóng đá giá theo trận 90p -> Quy đổi ra phút
        -- Công thức: Giá niêm yết * (Số phút thực tế / 90)
        SET @TienSan = @GiaApDung * (@SoPhut / 90.0);
    END
    ELSE
    BEGIN
        -- Các môn khác giá theo Giờ (60p) -> Quy đổi ra phút
        -- Công thức: Giá niêm yết * (Số phút thực tế / 60)
        SET @TienSan = @GiaApDung * (@SoPhut / 60.0);
    END

    RETURN @TienSan;
END";
            try
            {
                _db.ExecuteNonQuery(sql);
                return Content("Đã cập nhật công thức tính tiền thành công! (Bóng đá/90p, Môn khác/60p)");
            }
            catch (Exception ex)
            {
                return Content("Lỗi cập nhật: " + ex.Message);
            }
        }
        public IActionResult AddMissingPrices()
        {
            string sql = @"
            INSERT INTO KHUNGGIO (MaKG, MaLS, GioBatDau, GioKetThuc, NgayApDung, GiaApDung) VALUES
            ('KG020', 'LS005', '06:00:00', '22:00:00', '2024-01-01', 150000), -- Bóng chuyền
            ('KG021', 'LS006', '06:00:00', '22:00:00', '2024-01-01', 50000),  -- Bóng bàn
            ('KG022', 'LS007', '06:00:00', '22:00:00', '2024-01-01', 120000), -- Cầu lông đôi
            ('KG023', 'LS008', '06:00:00', '22:00:00', '2024-01-01', 300000), -- Tennis đôi
            ('KG024', 'LS009', '06:00:00', '22:00:00', '2024-01-01', 800000), -- Bóng đá 7 người
            ('KG025', 'LS010', '06:00:00', '22:00:00', '2024-01-01', 600000); -- Futsal
            ";
            try
            {
                _db.ExecuteNonQuery(sql);
                return Content("Đã thêm giá cho các sân còn thiếu!");
            }
            catch (Exception ex)
            {
                return Content("Lỗi (có thể đã thêm rồi): " + ex.Message);
            }
        }
        public IActionResult FixTriggers()
        {
            // 1. Fix Trùng Lịch Trigger: Bỏ qua trạng thái Nháp để không chặn chính mình khi chưa thanh toán
            string sql1 = @"
CREATE OR ALTER TRIGGER trg_KiemTraTrungLich
ON DATSAN
FOR INSERT, UPDATE
AS
BEGIN
    IF EXISTS (
        SELECT 1
        FROM inserted I
        JOIN PHIEUDATSAN P_Moi ON I.MaDatSan = P_Moi.MaDatSan
        JOIN DATSAN D_Cu ON I.MaSan = D_Cu.MaSan
        JOIN PHIEUDATSAN P_Cu ON D_Cu.MaDatSan = P_Cu.MaDatSan
        WHERE P_Cu.MaDatSan <> P_Moi.MaDatSan
          AND P_Cu.NgayDat = P_Moi.NgayDat
          AND P_Cu.TrangThai NOT IN (N'Đã hủy', N'No-Show', N'Nháp') -- <== FIX IMPORTANT: Bỏ qua Nháp
          AND (
              (P_Moi.GioBatDau >= P_Cu.GioBatDau AND P_Moi.GioBatDau < P_Cu.GioKetThuc) OR
              (P_Moi.GioKetThuc > P_Cu.GioBatDau AND P_Moi.GioKetThuc <= P_Cu.GioKetThuc) OR
              (P_Moi.GioBatDau <= P_Cu.GioBatDau AND P_Moi.GioKetThuc >= P_Cu.GioKetThuc)
          )
    )
    BEGIN
        RAISERROR (N'Lỗi: Sân này đã bị đặt trùng giờ (Đã có phiếu xác nhận)!', 16, 1);
        ROLLBACK TRANSACTION;
    END
END";

            // 2. Fix Trừ Kho Trigger: Không trừ kho với HLV và DV theo Giờ
            string sql2 = @"
CREATE OR ALTER TRIGGER trg_CapNhatTonKhoDichVu
ON CT_DICHVUDAT
FOR INSERT
AS
BEGIN
    DECLARE @MaDV VARCHAR(20);
    DECLARE @SoLuongDat INT;
    DECLARE @MaCS VARCHAR(20); 
    DECLARE @TenLoaiDV NVARCHAR(50);

    SELECT @MaDV = I.MaDV, @SoLuongDat = I.SoLuong
    FROM inserted I;

    SELECT TOP 1 @MaCS = S.MaCS
    FROM SAN S
    JOIN DATSAN D ON S.MaSan = D.MaSan
    JOIN inserted I ON I.MaDatSan = D.MaDatSan;

    -- Lấy tên loại dịch vụ
    SELECT @TenLoaiDV = LD.TenLoai
    FROM DICHVU DV
    JOIN LOAIDV LD ON DV.MaLoaiDV = LD.MaLoaiDV
    WHERE DV.MaDV = @MaDV;

    -- Nếu là HLV hoặc Dịch vụ Thuê Giờ (Trọng tài, Tennis...) -> KHÔNG TRỪ KHO
    IF @TenLoaiDV LIKE N'%Huấn luyện%' OR @TenLoaiDV LIKE N'%HLV%' OR @TenLoaiDV LIKE N'%Trọng tài%' OR @TenLoaiDV LIKE N'%Giờ%'
    BEGIN
        RETURN; -- Bỏ qua update kho
    END

    -- Nếu là Hàng hóa vật lý -> Trừ kho
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
        UPDATE DV_COSO
        SET SoLuongTon = SoLuongTon - @SoLuongDat
        WHERE MaDV = @MaDV AND MaCS = @MaCS;
    END
END";

            try
            {
                _db.ExecuteNonQuery(sql1);
                _db.ExecuteNonQuery(sql2);
                return Content("Đã fix xong 2 trigger lỗi (Trùng lịch ảo & Trừ kho HLV)!");
            }
            catch (Exception ex)
            {
                return Content("Lỗi: " + ex.Message);
            }
        }
        public IActionResult FixServiceProcedure()
        {
            string sql = @"
CREATE OR ALTER PROCEDURE sp_ThemDichVu
    @MaDatSan BIGINT,
    @MaDV VARCHAR(20),
    @SoLuong INT
AS
BEGIN
    SET NOCOUNT ON;
    SET TRAN ISOLATION LEVEL REPEATABLE READ;
    
    BEGIN TRY
        BEGIN TRAN;
        DECLARE @MaCS VARCHAR(20);
        
        -- Lấy MaCS
        SELECT TOP 1 @MaCS = S.MaCS 
        FROM PHIEUDATSAN P 
        JOIN DATSAN D ON P.MaDatSan = D.MaDatSan 
        JOIN SAN S ON D.MaSan = S.MaSan 
        WHERE P.MaDatSan = @MaDatSan;

        -- Kiểm tra loại dịch vụ
        DECLARE @TenLoaiDV NVARCHAR(50);
        SELECT @TenLoaiDV = LD.TenLoai
        FROM DICHVU DV
        JOIN LOAIDV LD ON DV.MaLoaiDV = LD.MaLoaiDV
        WHERE DV.MaDV = @MaDV;

        -- Nếu KHÔNG PHẢI (HLV hoặc Giờ hoặc Trọng tài) mới check kho va tru kho
        IF NOT (@TenLoaiDV LIKE N'%Huấn luyện%' OR @TenLoaiDV LIKE N'%HLV%' OR @TenLoaiDV LIKE N'%Trọng tài%' OR @TenLoaiDV LIKE N'%Giờ%')
        BEGIN
            DECLARE @TonKho INT;
            SELECT @TonKho = SoLuongTon FROM DV_COSO WHERE MaDV = @MaDV AND MaCS = @MaCS;
            
            IF @TonKho IS NULL OR @TonKho < @SoLuong
            BEGIN
                ROLLBACK TRAN;
                RAISERROR(N'Lỗi: Không đủ tồn kho!', 16, 1);
                RETURN;
            END

            -- Trừ kho (Chi tru kho voi hang hoa)
            UPDATE DV_COSO SET SoLuongTon = SoLuongTon - @SoLuong WHERE MaDV = @MaDV AND MaCS = @MaCS;
        END

        DECLARE @DonGia DECIMAL(18,2);
        SELECT @DonGia = DonGia FROM DICHVU WHERE MaDV = @MaDV;
        
        IF EXISTS (SELECT 1 FROM CT_DICHVUDAT WHERE MaDatSan = @MaDatSan AND MaDV = @MaDV)
        BEGIN
            UPDATE CT_DICHVUDAT SET SoLuong = SoLuong + @SoLuong, ThanhTien = (SoLuong + @SoLuong) * @DonGia WHERE MaDatSan = @MaDatSan AND MaDV = @MaDV;
        END
        ELSE
        BEGIN
            INSERT INTO CT_DICHVUDAT (MaDV, MaDatSan, SoLuong, ThanhTien, TrangThaiSuDung) VALUES (@MaDV, @MaDatSan, @SoLuong, @SoLuong * @DonGia, N'Chưa thanh toán');
        END
        
        COMMIT TRAN;
        PRINT N'Thêm dịch vụ thành công!';
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRAN;
        DECLARE @Msg NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(@Msg, 16, 1);
    END CATCH
END";
            _db.ExecuteNonQuery(sql);
            return Content("Đã sửa Procedure sp_ThemDichVu thành công!");
        }
        public IActionResult DebugPrices()
        {
            var dt = _db.ExecuteQuery(@"
                SELECT LS.MaLS, LS.TenLS, MIN(K.GiaApDung) as MinGia, MAX(K.GiaApDung) as MaxGia
                FROM LOAISAN LS
                LEFT JOIN KHUNGGIO K ON LS.MaLS = K.MaLS
                GROUP BY LS.MaLS, LS.TenLS
            ");
            string res = "<html><body><h1>Pricing Debug</h1>";
            foreach(DataRow r in dt.Rows) {
                res += $"<p><strong>{r["TenLS"]} ({r["MaLS"]})</strong>: Min = {r["MinGia"]}, Max = {r["MaxGia"]}</p>";
            }
            res += "</body></html>";
            return Content(res, "text/html");
        }
        public IActionResult FixServiceTrigger()
        {
            var sql = @"
CREATE OR ALTER TRIGGER trg_KiemTraLichDichVu
ON CT_DICHVUDAT
FOR INSERT, UPDATE
AS
BEGIN
    IF NOT EXISTS (SELECT 1 FROM inserted) RETURN;

    -- Chỉ kiểm tra các dịch vụ có tính chất ""Chiếm chỗ"" (HLV, Phòng, Tủ)
    IF EXISTS (
        SELECT 1
        FROM inserted I
        JOIN DICHVU DV ON I.MaDV = DV.MaDV
        JOIN LOAIDV LDV ON DV.MaLoaiDV = LDV.MaLoaiDV
        JOIN PHIEUDATSAN P_Moi ON I.MaDatSan = P_Moi.MaDatSan
        
        -- Tìm các phiếu cũ đã đặt cùng dịch vụ này
        JOIN CT_DICHVUDAT CT_Cu ON I.MaDV = CT_Cu.MaDV 
        JOIN PHIEUDATSAN P_Cu ON CT_Cu.MaDatSan = P_Cu.MaDatSan
        
        WHERE I.MaDatSan <> CT_Cu.MaDatSan -- Khác phiếu hiện tại
          AND P_Cu.NgayDat = P_Moi.NgayDat -- Cùng ngày
          AND P_Cu.TrangThai NOT IN (N'Đã hủy', N'No-Show', N'Nháp') -- FIX: Bo qua Nhap
          
          -- Kiểm tra loại dịch vụ 
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
END";
            _db.ExecuteNonQuery(sql);
            return Content("Đã sửa Trigger trg_KiemTraLichDichVu thành công!");
        }
        public IActionResult FixAutoCancel()
        {
            var sql = @"
CREATE OR ALTER PROCEDURE sp_TuDongHuyDonQuaHan
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @SoPhutGioiHan INT = 30;
    
    UPDATE PHIEUDATSAN
    SET TrangThai = N'Đã hủy'
    WHERE TrangThai = N'Chờ thanh toán'
      AND DATEDIFF(MINUTE, NgayTao, GETDATE()) > @SoPhutGioiHan;
      
    SELECT @@ROWCOUNT AS SoLuongHuy;
END";
            _db.ExecuteNonQuery(sql);
            return Content("Đã sửa SP AutoCancel thành công!");
        }
        public IActionResult ForceFixSP()
        {
            var sql = @"
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
        
        SELECT @DonGia = DonGia, @MaCS = S.MaCS 
        FROM DICHVU DV
        JOIN DATSAN DS ON DS.MaSan = (SELECT D.MaSan FROM DATSAN D WHERE D.MaDatSan = @MaDatSan)
        JOIN SAN S ON DS.MaSan = S.MaSan
        WHERE DV.MaDV = @MaDV;

        IF @DonGia IS NULL
        BEGIN
             ROLLBACK TRAN;
             RAISERROR(N'Dịch vụ không tồn tại!', 16, 1);
             RETURN;
        END

        DECLARE @IsStockItem BIT = 1;
        -- CHECK LOGIC FIX: ID LDV001=HLV, LDV004=VIP, LDV005=LOCKER
        IF EXISTS (
            SELECT 1 FROM DICHVU DV 
            JOIN LOAIDV L ON DV.MaLoaiDV = L.MaLoaiDV
            WHERE DV.MaDV = @MaDV 
            AND L.MaLoaiDV IN ('LDV001', 'LDV004', 'LDV005') 
        )
        BEGIN
            SET @IsStockItem = 0; 
        END

        IF @IsStockItem = 1
        BEGIN
            DECLARE @TonKho INT;
            SELECT @TonKho = SoLuongTon FROM DV_COSO WHERE MaDV = @MaDV AND MaCS = @MaCS;
            IF @TonKho IS NULL OR @TonKho < @SoLuong
            BEGIN
                ROLLBACK TRAN;
                RAISERROR(N'Lỗi: Không đủ tồn kho!', 16, 1);
                RETURN;
            END
        END

        IF EXISTS (SELECT 1 FROM CT_DICHVUDAT WHERE MaDatSan = @MaDatSan AND MaDV = @MaDV)
        BEGIN
            UPDATE CT_DICHVUDAT SET SoLuong = SoLuong + @SoLuong, ThanhTien = (SoLuong + @SoLuong) * @DonGia WHERE MaDatSan = @MaDatSan AND MaDV = @MaDV;
        END
        ELSE
        BEGIN
            INSERT INTO CT_DICHVUDAT (MaDV, MaDatSan, SoLuong, ThanhTien, TrangThaiSuDung) VALUES (@MaDV, @MaDatSan, @SoLuong, @SoLuong * @DonGia, N'Chưa thanh toán');
        END
        
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
END";
            _db.ExecuteNonQuery(sql);
            
            // Verify
             var dt = _db.ExecuteQuery("sp_helptext 'sp_ThemDichVu'");
             string res = "";
             foreach(System.Data.DataRow r in dt.Rows) res += r[0].ToString();
            
            return Content("FORCE FIX APPLIED. NEW CONTENT:\n" + res);
        }

        public IActionResult CheckServiceType(string name)
        {
            var sql = $"SELECT DV.MaDV, DV.TenDV, L.MaLoaiDV, L.TenLoai FROM DICHVU DV JOIN LOAIDV L ON DV.MaLoaiDV = L.MaLoaiDV WHERE DV.TenDV LIKE N'%{name}%'";
            var dt = _db.ExecuteQuery(sql);
            string res = "";
            if (dt.Rows.Count == 0) return Content("Không tìm thấy dịch vụ nào tên chứa " + name);
            foreach(System.Data.DataRow r in dt.Rows)
            {
                res += $"DV: {r["MaDV"]} - {r["TenDV"]} | Loai: '{r["MaLoaiDV"]}' (Length: {r["MaLoaiDV"].ToString().Length}) - {r["TenLoai"]}\n";
            }
            return Content(res);
        }

        public IActionResult ViewSPContent()
        {
             var dt = _db.ExecuteQuery("sp_helptext 'sp_ThemDichVu'");
             string res = "";
             foreach(System.Data.DataRow r in dt.Rows) res += r[0].ToString();
             return Content(res, "text/plain");
        }
    }
}
