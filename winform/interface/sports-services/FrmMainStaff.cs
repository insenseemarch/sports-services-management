using System;
using System.Drawing;
using System.Windows.Forms;
using SportsServices.Dto; // Chứa class TaiKhoan
using SportsServices.Forms; // Để mở các Form con

namespace SportsServices
{
    public partial class FrmMainStaff : Form
    {
        private TaiKhoan _user;
        private Form currentChildForm;

        public FrmMainStaff(TaiKhoan user)
        {
            InitializeComponent();
            _user = user;
            this.WindowState = FormWindowState.Maximized;
            lblXinChao.Text = "Xin chào:" + $"{_user.HoTen} ({GetTenChucVu(_user.Role)})";
        }

        private void FrmMainStaff_Load(object sender, EventArgs e)
        {
            PhanQuyenSuDung();
        }

        // Hàm hiển thị tên chức vụ tiếng Việt cho đẹp
        private string GetTenChucVu(string role)
        {
            switch (role)
            {
                case "LE_TAN": return "Lễ Tân";
                case "THU_NGAN": return "Thu Ngân";
                case "IT": return "Quản trị hệ thống";
                case "KY_THUAT": return "Kỹ thuật sân";
                case "QUAN_LY": return "Quản lý";
                case "HLV": return "HLV Cá nhân";
                default: return "Nhân viên";
            }
        }

        // === LOGIC PHÂN QUYỀN ===
        private void PhanQuyenSuDung()
        {
            // BƯỚC 1: Reset trạng thái ban đầu
            // Cho hiện hết (Visible = true) nhưng KHÓA lại (Enabled = false)

            // Nhóm Lễ tân
            btnDatSan.Enabled = false;

            // Nhóm Thu ngân
            btnThanhToan.Enabled = false;

            // Nhóm IT
            btnHeThong.Enabled = false;

            // Nhóm Kỹ thuật
            btnBaoTri.Enabled = false;

            // Nhóm Quản lý
            btnNhanSu.Enabled = false;
            btnPhanCa.Enabled = false;
            btnDuyetPhep.Enabled = false;
            btnBaoCaoLoi.Enabled = false;
            btnBaoCaoThongKe.Enabled = false;

            // Lưu ý: Các nút chung như "Lịch làm việc", "Cá nhân", "Đăng xuất" 
            // thì luôn Enabled = true nên không cần chỉnh ở đây.

            // BƯỚC 2: MỞ KHÓA (Enabled = true) dựa theo vai trò
            switch (_user.Role)
            {
                case "LE_TAN":
                    btnDatSan.Enabled = true; // Chỉ mở nút đặt sân
                    break;

                case "THU_NGAN":
                    btnThanhToan.Enabled = true; // Chỉ mở nút thanh toán
                    break;

                case "IT":
                    btnHeThong.Enabled = true; // Chỉ mở cấu hình hệ thống
                    break;

                case "KY_THUAT":
                    btnBaoTri.Enabled = true; // Chỉ mở bảo trì
                    break;

                case "QUAN_LY":
                    // Quản lý được dùng full quyền quản trị
                    btnNhanSu.Enabled = true;
                    btnPhanCa.Enabled = true;
                    btnDuyetPhep.Enabled = true;
                    btnBaoCaoLoi.Enabled = true;
                    btnBaoCaoThongKe.Enabled = true;

                    // Nếu Quản lý được quyền bán hàng giúp nhân viên thì mở luôn:
                    btnDatSan.Enabled = true;
                    btnThanhToan.Enabled = true;
                    break;

                case "HLV":
                    // HLV không được mở các chức năng nghiệp vụ trên
                    // Chỉ dùng được các nút chung (đã mặc định mở)
                    break;
            }
        }

        // === HÀM MỞ FORM CON (Giống bên Khách hàng) ===
        private void OpenChildForm(Form childForm)
        {
            if (currentChildForm != null) currentChildForm.Close();
            currentChildForm = childForm;
            childForm.TopLevel = false;
            childForm.FormBorderStyle = FormBorderStyle.None;
            childForm.Dock = DockStyle.Fill;
            panelDesktop.Controls.Add(childForm);
            panelDesktop.Tag = childForm;
            childForm.BringToFront();
            childForm.Show();
        }

        // === XỬ LÝ SỰ KIỆN CÁC NÚT (Mapping vào các Form bạn đã có) ===

        // 1. Nhóm Chung
        private void btnLichLamViec_Click(object sender, EventArgs e)
        {
            OpenChildForm(new FormLichLamViec());
        }

        private void btnCaNhan_Click(object sender, EventArgs e)
        {
            OpenChildForm(new FrmXemProfileNhanVien(_user)); // Bạn đã có form này
        }

        private void btnDangXuat_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // 2. Lễ Tân
        private void btnDatSan_Click(object sender, EventArgs e)
        {
            OpenChildForm(new FormDatSan());
        }

        // 3. Thu Ngân
        private void btnThanhToan_Click(object sender, EventArgs e)
        {
            OpenChildForm(new FrmHoaDon());
        }

        // 4. IT
        private void btnHeThong_Click(object sender, EventArgs e)
        {
            OpenChildForm(new FrmThayDoiHeThong()); // Bạn đã có form này
        }

        // 5. Kỹ Thuật
        private void btnBaoTri_Click(object sender, EventArgs e)
        {
            OpenChildForm(new QuanLiBaoTri());
        }

        // 6. Quản Lý
        private void btnNhanSu_Click(object sender, EventArgs e)
        {
            // Có thể mở Form Danh sách nhân viên, trong đó có nút "Thêm nhân viên"
            OpenChildForm(new FrmDanhSachNhanVien());
        }

        private void btnPhanCa_Click(object sender, EventArgs e)
        {
            OpenChildForm(new PhanCongCaTruc()); // Bạn đã có form này
        }

        private void btnDuyetPhep_Click(object sender, EventArgs e)
        {
            OpenChildForm(new FrmPheDuyetDonNghiPhep()); // Bạn đã có form này
        }

        private void btnBaoCaoLoi_Click(object sender, EventArgs e)
        {
            // Quản lý báo lỗi sân -> Gửi dữ liệu vào DB cho Kỹ thuật thấy
            OpenChildForm(new FormReportLoiSan());
        }

        private void btnBaoCaoThongKe_Click(object sender, EventArgs e)
        {
            OpenChildForm(new FrmBaoCao()); // Doanh thu, tồn kho...
        }
    }
}