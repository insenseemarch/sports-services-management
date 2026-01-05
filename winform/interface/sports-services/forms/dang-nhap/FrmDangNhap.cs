using SportsServices.Dto;
using SportsServices.Forms; // Để gọi Form1 (Dashboard)
using System;
using System.Windows.Forms;

namespace SportsServices.Forms
{
    public partial class FrmDangNhap : Form
    {
        public FrmDangNhap()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string user = txtUsername.Text;

            // Check xem có nhân viên nào trùng Mã NV không (Giả sử Mã NV là username)
            // Hoặc bạn hardcode tạm admin
            if (user == "admin")
            {
                new Form1("QuanLy").ShowDialog();
            }
            else
            {
                // Tìm trong fake DB
                var nv = FakeDatabase.NhanViens.FirstOrDefault(x => x.MaNV == user);
                if (nv != null)
                {
                    MessageBox.Show($"Xin chào {nv.HoTen}!");
                    new Form1("NhanVien").ShowDialog();
                }
                else
                {
                    MessageBox.Show("Sai tài khoản!");
                }
            }
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            // Mở form đăng ký bạn đã có sẵn
            FrmDangKy frm = new FrmDangKy();
            frm.ShowDialog();
        }
    }
}