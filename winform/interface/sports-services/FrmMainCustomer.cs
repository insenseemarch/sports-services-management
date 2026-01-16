using SportsServices.Dto; // Để dùng class TaiKhoan
using System;
using System.Drawing;
using System.Windows.Forms;
using SportsServices.Forms;

// Đảm bảo namespace trùng với project của bạn
namespace SportsServices
{
    public partial class FrmMainCustomer : Form
    {
        // Biến lưu form con hiện tại
        private Form currentChildForm;
        private TaiKhoan _currentUser; // Lưu thông tin người đăng nhập

        // Constructor nhận vào tài khoản từ màn hình đăng nhập
        public FrmMainCustomer(TaiKhoan user)
        {
            InitializeComponent();
            _currentUser = user;

            // Cài đặt giao diện
            this.Text = "ViệtSports - Cổng thông tin khách hàng";
            this.WindowState = FormWindowState.Maximized; // Mở toàn màn hình

            // Hiển thị tên
            lblXinChao.Text = "Xin kính chào quý khách: " + (_currentUser != null ? _currentUser.HoTen : "Khách");
        }

        // Hàm dùng chung để mở Form con lồng vào Panel
        private void OpenChildForm(Form childForm)
        {
            // Nếu có form cũ đang mở thì đóng lại
            if (currentChildForm != null)
            {
                currentChildForm.Close();
            }

            currentChildForm = childForm;

            // Thiết lập để form con hiển thị như một control
            childForm.TopLevel = false;
            childForm.FormBorderStyle = FormBorderStyle.None;
            childForm.Dock = DockStyle.Fill;

            // Thêm vào panel
            panelDesktop.Controls.Add(childForm);
            panelDesktop.Tag = childForm;
            childForm.BringToFront();
            childForm.Show();
        }

        // --- XỬ LÝ SỰ KIỆN CÁC NÚT MENU ---

        private void btnDatSan_Click(object sender, EventArgs e)
        {
            // Mở form Đặt Sân
            OpenChildForm(new FormDatSan());
            // Highlight nút (tuỳ chọn): Đổi màu nền nút này để biết đang chọn
        }

        private void btnDatDichVu_Click(object sender, EventArgs e)
        {
            // Mở form Đặt Dịch Vụ
            OpenChildForm(new FormDatDichVu());
        }

        private void btnThayDoi_Click(object sender, EventArgs e)
        {
            // Mở form Thay Đổi / Lịch sử
            OpenChildForm(new FormDoiSan());
        }

        private void btnCaNhan_Click(object sender, EventArgs e)
        {
            // Mở form Profile
            // Giả sử FrmXemProfileKhachHang có constructor nhận Username hoặc đối tượng khách
            // Nếu form profile của bạn chưa nhận tham số, hãy sửa nó sau.
            // Tạm thời gọi constructor mặc định:
            OpenChildForm(new FrmXemProfileKhachHang());
        }

        private void btnDangXuat_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Bạn có chắc chắn muốn đăng xuất?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                this.Close(); // Đóng form này để quay về login
            }
        }

        // Sự kiện khi form load (Mặc định mở trang Đặt sân đầu tiên)
        private void FrmMainCustomer_Load(object sender, EventArgs e)
        {
            btnDatSan_Click(sender, e);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenChildForm(new FormDatSan());
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenChildForm(new FormDoiSan());
        }

        private void button3_Click(object sender, EventArgs e)
        {
            OpenChildForm(new FrmXemProfileKhachHang());
        }

        private void button5_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Bạn có chắc chắn muốn đăng xuất?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                this.Close(); // Đóng form này để quay về login
            }
        }
    }
}