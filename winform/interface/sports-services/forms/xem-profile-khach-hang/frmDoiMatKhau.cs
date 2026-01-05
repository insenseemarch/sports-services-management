using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ThongTinCaNhan
{
    public partial class frmDoiMatKhau : Form
    {
        // Biến để lưu mật khẩu hiện tại từ Form chính truyền sang
        private string _matKhauHienTai;

        // Biến public để Form chính lấy được mật khẩu mới sau khi đổi
        public string MatKhauMoi { get; private set; }

        // Sửa constructor để nhận tham số mật khẩu hiện tại
        public frmDoiMatKhau(string matKhauDangDung)
        {
            InitializeComponent();
            _matKhauHienTai = matKhauDangDung;
        }

        private void txtMatKhauCu_TextChanged(object sender, EventArgs e)
        {

        }

        private void frmDoiMatKhau_Load(object sender, EventArgs e)
        {

        }

        private void btnLuu_Click(object sender, EventArgs e)
        {
            // 1. Kiểm tra xem có nhập đủ không
            if (string.IsNullOrEmpty(txtMatKhauCu.Text) || string.IsNullOrEmpty(txtMatKhauMoi.Text))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin!");
                return;
            }

            // 2. Kiểm tra mật khẩu cũ có đúng không
            if (txtMatKhauCu.Text != _matKhauHienTai)
            {
                MessageBox.Show("Mật khẩu cũ không chính xác!");
                return;
            }

            // 3. Kiểm tra mật khẩu mới và xác nhận có khớp không
            if (txtMatKhauMoi.Text != txtXacNhan.Text)
            {
                MessageBox.Show("Mật khẩu xác nhận không khớp!");
                return;
            }

            // 4. Nếu mọi thứ OK
            MatKhauMoi = txtMatKhauMoi.Text; // Lưu vào biến public
            this.DialogResult = DialogResult.OK; // Báo về là thành công
            this.Close(); // Đóng form
        }

        private void btnHuy_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel; // Báo về là hủy
            this.Close();
        }

        private void chkShowMatKhau_CheckedChanged(object sender, EventArgs e)
        {
            if (txtMatKhauCu.PasswordChar == '*')
            {
                // Đang ẩn -> Chuyển sang hiện
                txtMatKhauCu.PasswordChar = '\0';
                chkShowMatKhau.Checked = true; // (Tuỳ chọn) Đổi tên nút
            }
            else
            {
                // Đang hiện -> Chuyển sang ẩn
                txtMatKhauCu.PasswordChar = '*';
                chkShowMatKhau.Checked = false; // (Tuỳ chọn) Đổi tên nút
            }
        }

        private void chkShowMatKhauMoi_CheckedChanged(object sender, EventArgs e)
        {
            if (txtMatKhauMoi.PasswordChar == '*')
            {
                // Đang ẩn -> Chuyển sang hiện
                txtMatKhauMoi.PasswordChar = '\0';
                chkShowMatKhauMoi.Checked = true; // (Tuỳ chọn) Đổi tên nút
            }
            else
            {
                // Đang hiện -> Chuyển sang ẩn
                txtMatKhauMoi.PasswordChar = '*';
                chkShowMatKhauMoi.Checked = false; // (Tuỳ chọn) Đổi tên nút
            }
        }

        private void chkShowXacNhan_CheckedChanged(object sender, EventArgs e)
        {
            if (txtXacNhan.PasswordChar == '*')
            {
                // Đang ẩn -> Chuyển sang hiện
                txtXacNhan.PasswordChar = '\0';
                chkShowXacNhan.Checked = true; // (Tuỳ chọn) Đổi tên nút
            }
            else
            {
                // Đang hiện -> Chuyển sang ẩn
                txtXacNhan.PasswordChar = '*';
                chkShowXacNhan.Checked = false; // (Tuỳ chọn) Đổi tên nút
            }
        }
    }
}
