USE master
GO

USE TRUNGTAMTHETHAO
GO

--	===================================================================================
--	==								BẢO MẬT & PHÂN QUYỀN							 ==
--	===================================================================================

-- Tạo các nhóm quyền (Role)
CREATE ROLE NhanVienQuanLy;
CREATE ROLE NhanVienLeTan;
CREATE ROLE NhanVienThuNgan;
CREATE ROLE NhanVienKyThuat;
CREATE ROLE KhachHang; 


-- I. NHÂN VIÊN QUẢN LÝ
-- Cấp quyền trên các bảng nhân sự và nghiệp vụ
GRANT SELECT, INSERT, UPDATE, DELETE ON NHANVIEN TO NhanVienQuanLy;
GRANT SELECT, INSERT, UPDATE, DELETE ON CATRUC TO NhanVienQuanLy;
GRANT SELECT, INSERT, UPDATE, DELETE ON THAMGIACATRUC TO NhanVienQuanLy;
GRANT SELECT, INSERT, UPDATE, DELETE ON DICHVU TO NhanVienQuanLy;
GRANT SELECT, INSERT, UPDATE, DELETE ON LOAIDV TO NhanVienQuanLy;
GRANT SELECT, INSERT, UPDATE, DELETE ON UUDAI TO NhanVienQuanLy;
GRANT SELECT, INSERT, UPDATE, DELETE ON COSO TO NhanVienQuanLy;
GRANT SELECT ON BAOCAOTHONGKE TO NhanVienQuanLy;
GRANT SELECT ON DOANHTHU TO NhanVienQuanLy;
GRANT SELECT ON TONGGIOLAMVIECNV TO NhanVienQuanLy;
GRANT EXECUTE TO NhanVienQuanLy; -- Quyền chạy các Procedure tính lương, báo cáo

-- II. NHÂN VIÊN LỄ TÂN
GRANT SELECT ON SAN TO NhanVienLeTan;
GRANT SELECT ON KHUNGGIO TO NhanVienLeTan;
GRANT SELECT ON LOAISAN TO NhanVienLeTan;
GRANT SELECT ON KHACHHANG TO NhanVienLeTan;
GRANT SELECT ON DV_COSO TO NhanVienLeTan;
GRANT INSERT ON KHACHHANG TO NhanVienLeTan;

-- Thao tác đặt sân (Sửa theo đúng bảng DATSAN)
GRANT SELECT, INSERT, UPDATE ON PHIEUDATSAN TO NhanVienLeTan;
GRANT SELECT, INSERT, UPDATE ON DATSAN TO NhanVienLeTan;
GRANT SELECT, INSERT, UPDATE ON CT_DICHVUDAT TO NhanVienLeTan;

-- Chặn xóa phiếu đặt sân (Bảo mật dữ liệu)
DENY DELETE ON PHIEUDATSAN TO NhanVienLeTan;

-- III. NHÂN VIÊN THU NGÂN
GRANT SELECT, INSERT ON HOADON TO NhanVienThuNgan;
GRANT SELECT ON PHIEUDATSAN TO NhanVienThuNgan;
GRANT SELECT ON CT_DICHVUDAT TO NhanVienThuNgan;
GRANT SELECT ON KHACHHANG TO NhanVienThuNgan;
GRANT SELECT ON UUDAI TO NhanVienThuNgan;

-- Cập nhật trạng thái phiếu sau khi thanh toán
GRANT UPDATE (TrangThai) ON PHIEUDATSAN TO NhanVienThuNgan;
GRANT UPDATE (TrangThaiSuDung) ON CT_DICHVUDAT TO NhanVienThuNgan;

-- IV. NHÂN VIÊN KỸ THUẬT
GRANT SELECT ON SAN TO NhanVienKyThuat;
GRANT UPDATE (TinhTrang) ON SAN TO NhanVienKyThuat; -- Cập nhật 'Bảo trì', 'Còn trống'
GRANT SELECT, INSERT, UPDATE ON PHIEUBAOTRI TO NhanVienKyThuat;

-- V. HUẤN LUYỆN VIÊN 
GRANT SELECT ON HLV TO NhanVienKyThuat; -- Cho phép xem hồ sơ HLV
GRANT SELECT ON CT_DICHVUDAT TO KhachHang; -- Khách hàng xem được dịch vụ đi kèm

-- VI. KHÁCH HÀNG
-- Đăng ký tài khoản, tìm kiếm và tự đặt sân online 
GRANT INSERT, UPDATE ON KHACHHANG TO KhachHang;
GRANT SELECT ON SAN TO KhachHang;
GRANT SELECT ON KHUNGGIO TO KhachHang;
GRANT SELECT ON LOAISAN TO KhachHang;
GRANT SELECT ON UUDAI TO KhachHang;

-- Thao tác đặt chỗ online 
GRANT INSERT ON PHIEUDATSAN TO KhachHang;
GRANT INSERT ON DATSAN TO KhachHang;
GRANT INSERT ON CT_DICHVUDAT TO KhachHang;
GO