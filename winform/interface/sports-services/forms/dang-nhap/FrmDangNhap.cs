using SportsServices.Dto;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Forms;

namespace SportsServices.Forms
{
    public partial class FrmDangNhap : Form
    {
        public FrmDangNhap()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen; // Hiện giữa màn hình
            this.WindowState = FormWindowState.Maximized;
        }

        private void FrmDangNhap_Load(object sender, EventArgs e)
        {
            // Mặc định focus vào ô username
            txtUsername.Focus();
        }

        private void btnDangNhap_Click(object sender, EventArgs e)
        {
            string u = txtUsername.Text;
            string p = txtPassword.Text;

            // 1. Tìm kiếm trong FakeDatabase
            // Tìm user khớp username và password
            var user = FakeDatabase.TaiKhoans.FirstOrDefault(t => t.Username == u && t.Password == p);

            if (user != null)
            {
                MessageBox.Show($"Xin chào {user.Role}: {user.HoTen}", "Đăng nhập thành công");

                this.Hide(); // Ẩn form đăng nhập

                // 2. PHÂN QUYỀN ĐIỀU HƯỚNG
                if (user.Role == "KHACH_HANG")
                {
                    // Mở giao diện cho Khách
                    FrmMainCustomer frmKhach = new FrmMainCustomer(user);
                    this.Hide();
                    frmKhach.ShowDialog();
                    this.Show();
                }
                else if (user.Role == "LE_TAN" || user.Role == "QUAN_LY" || user.Role == "THU_NGAN" || user.Role == "IT" || user.Role == "KY_THUAT" || user.Role == "HLV")
                {
                    // Mở giao diện cho Nhân viên/Quản lý
                    // Ví dụ bạn có FrmMainStaff
                    // Nếu chưa có, tạm thời mở Form1 nhưng hiển thị khác, hoặc mở 1 form khác
                    // Ví dụ:
                    // FrmQuanLy frmAdmin = new FrmQuanLy();
                    // frmAdmin.ShowDialog();
                    MessageBox.Show("Đang mở giao diện Quản lý");
                    FrmMainStaff frmAdmin = new FrmMainStaff(user); // Tạm thời mở Form1
                    frmAdmin.ShowDialog();
                }

                // Khi Form chính đóng lại thì hiện lại Login hoặc thoát luôn tuỳ logic
                this.Close();
            }
            else
            {
                MessageBox.Show("Sai tên đăng nhập hoặc mật khẩu!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show("Bạn có muốn thoát chương trình?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dr == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        private void btnDangKy_Click(object sender, EventArgs e)
        {
            // Mở form đăng ký (FormDangKy là class bạn đã có trong file FrmDangKy.cs)
            FormDangKy frm = new FormDangKy();
            this.Hide();
            frm.ShowDialog();
            this.Show(); // Hiện lại form đăng nhập sau khi tắt form đăng ký
        }

        private void chkShowPass_CheckedChanged(object sender, EventArgs e)
        {
            // Logic ẩn hiện mật khẩu giống bên Đăng ký
            txtPassword.UseSystemPasswordChar = !chkShowPass.Checked;
        }
    }
}