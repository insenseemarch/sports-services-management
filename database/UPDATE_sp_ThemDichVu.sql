USE TRUNGTAMTHETHAO
GO

/*
FIX LỖI: "Không đủ tồn kho" khi đặt HLV
Nguyên nhân: SP kiểm tra tồn kho ngay cả khi delta âm (giảm số lượng)
Giải pháp: Chỉ kiểm tra tồn kho khi @SoLuong > 0 (thêm mới)
*/

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
END
GO

PRINT 'Updated sp_ThemDichVu successfully - Fixed HLV stock check issue!'
GO
