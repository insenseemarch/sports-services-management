USE TRUNGTAMTHETHAO
GO

-- XÓA HLV RA KHỎI BẢNG DV_COSO (vì HLV không cần quản lý tồn kho)
DELETE FROM DV_COSO 
WHERE MaDV IN (
    SELECT DV.MaDV 
    FROM DICHVU DV 
    JOIN LOAIDV L ON DV.MaLoaiDV = L.MaLoaiDV
    WHERE L.MaLoaiDV = 'LDV001' OR L.TenLoai LIKE N'%Huấn luyện viên%'
);

PRINT 'Đã xóa HLV ra khỏi bảng DV_COSO - HLV không cần quản lý tồn kho!';
GO
