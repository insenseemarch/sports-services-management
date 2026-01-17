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
