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
    -- Kiểm tra nếu có bất kỳ dòng nào vừa insert bị trùng lịch
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
