using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SportsServices.Forms
{
    public partial class FormDangKy : Form
    {
        private byte[] _giayToBytes;      // dữ liệu ảnh thẻ SV / giấy tờ
        private string _giayToFileName;   // đường dẫn file (nếu cần)

        public FormDangKy()
        {
            InitializeComponent(); this.WindowState = FormWindowState.Maximized;
        }

        private void FormDangKy_Load(object sender, EventArgs e)
        {
            // Khi mới mở form: chưa tick HSSV => không cho chọn ảnh
            btnTaiGiayTo.Enabled = false;
            _giayToBytes = null;
            _giayToFileName = null;
            picGiayTo.Image = null;
        }

        /// <summary>
        /// Kiểm tra dữ liệu người dùng nhập
        /// </summary>
        private bool ValidateForm()
        {
            // Các trường bắt buộc
            if (string.IsNullOrWhiteSpace(txtHoTen.Text) ||
                string.IsNullOrWhiteSpace(txtUsername.Text) ||
                string.IsNullOrWhiteSpace(txtPassword.Text) ||
                string.IsNullOrWhiteSpace(txtConfirmPassword.Text) ||
                string.IsNullOrWhiteSpace(txtSDT.Text) ||
                string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ các thông tin bắt buộc.",
                                "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // Mật khẩu nhập lại
            if (txtPassword.Text != txtConfirmPassword.Text)
            {
                MessageBox.Show("Mật khẩu nhập lại không khớp.",
                                "Sai mật khẩu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // Nếu chọn HSSV thì bắt buộc phải có ảnh
            if (chkHSSV.Checked && _giayToBytes == null)
            {
                MessageBox.Show("Bạn đã chọn HSSV. Vui lòng chọn ảnh thẻ sinh viên để nhận ưu đãi.",
                                "Thiếu giấy tờ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Chọn ảnh thẻ sinh viên / giấy tờ
        /// </summary>
        private void btnTaiGiayTo_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Title = "Chọn ảnh thẻ sinh viên / giấy tờ HSSV";
                ofd.Filter = "Ảnh (*.jpg;*.jpeg;*.png)|*.jpg;*.jpeg;*.png|Tất cả file (*.*)|*.*";

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    _giayToFileName = ofd.FileName;
                    _giayToBytes = File.ReadAllBytes(ofd.FileName);

                    picGiayTo.Image = Image.FromFile(ofd.FileName);
                }
            }
        }

        /// <summary>
        /// Xử lý nút Đăng ký
        /// </summary>
        private void btnDangKy_Click(object sender, EventArgs e)
        {
            // 1. Kiểm tra dữ liệu
            if (!ValidateForm())
                return;

            // 2. Câu lệnh SQL (tùy bạn đã tạo bảng KHACH_HANG như thế nào)
            string sql = @"
                INSERT INTO KHACH_HANG
                    (HoTen, NgaySinh, CMND, SDT, Email, DiaChi, IsHSSV, AnhGiayTo)
                VALUES
                    (@HoTen, @NgaySinh, @CMND, @SDT, @Email, @DiaChi, @IsHSSV, @AnhGiayTo);

                INSERT INTO TAI_KHOAN(Username, PasswordHash, MaKhachHang)
                VALUES (@Username, @Password, SCOPE_IDENTITY());
            ";

            var p = new[]
            {
                new SqlParameter("@HoTen", txtHoTen.Text),
                new SqlParameter("@NgaySinh", dtpNgaySinh.Value),
                // Đảm bảo TextBox CCCD của bạn tên là txtCCCD, nếu không thì đổi cho khớp
                new SqlParameter("@CMND", (object)txtCCCD.Text ?? DBNull.Value),
                new SqlParameter("@SDT", txtSDT.Text),
                new SqlParameter("@Email", txtEmail.Text),
                new SqlParameter("@DiaChi", (object)txtDiaChi.Text ?? DBNull.Value),

                new SqlParameter("@IsHSSV", chkHSSV.Checked),
                new SqlParameter("@AnhGiayTo", (object)_giayToBytes ?? DBNull.Value),

                new SqlParameter("@Username", txtUsername.Text),
                new SqlParameter("@Password", txtPassword.Text) // demo: sau này nên hash
            };

            try
            {
                DbHelper.ExecuteNonQuery(sql, p);
                MessageBox.Show("Đăng ký thành công!", "Thành công",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi đăng ký: " + ex.Message,
                                "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnHuy_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // Khi tick / bỏ tick HSSV
        private void chkHSSV_CheckedChanged(object sender, EventArgs e)
        {
            // Chỉ cho chọn ảnh khi đã tick HSSV
            btnTaiGiayTo.Enabled = chkHSSV.Checked;

            if (!chkHSSV.Checked)
            {
                // Bỏ tick: xóa ảnh và dữ liệu
                _giayToBytes = null;
                _giayToFileName = null;
                picGiayTo.Image = null;
            }
        }

        // Các event thừa VS tạo, cứ để trống cũng không sao
        private void textBox1_TextChanged(object sender, EventArgs e) { }
        private void textBox2_TextChanged(object sender, EventArgs e) { }
        private void textBox3_TextChanged(object sender, EventArgs e) { }
        private void textBox4_TextChanged(object sender, EventArgs e) { }
        private void textBox5_TextChanged(object sender, EventArgs e) { }
        private void textBox6_TextChanged(object sender, EventArgs e) { }
        private void textBox7_TextChanged(object sender, EventArgs e) { }
        private void label1_Click(object sender, EventArgs e) { }
        private void label2_Click(object sender, EventArgs e) { }
        private void label7_Click(object sender, EventArgs e) { }
        private void groupBox1_Enter(object sender, EventArgs e) { }
        private void picGiayTo_Click(object sender, EventArgs e) { }
        private void dtpNgaySinh_ValueChanged(object sender, EventArgs e) { }
        private void btnChonGiayTo_Click(object sender, EventArgs e) { }
        private void picGiayTo_Click_1(object sender, EventArgs e) { }

        private void label2_Click_1(object sender, EventArgs e)
        {

        }

        private void label11_Click(object sender, EventArgs e)
        {

        }

        private void label12_Click(object sender, EventArgs e)
        {

        }

        private void groupBox1_Enter_1(object sender, EventArgs e)
        {

        }

        private void dtpNgaySinh_ValueChanged_1(object sender, EventArgs e)
        {

        }

        private void chkShowPass_CheckedChanged(object sender, EventArgs e)
        {
            txtPassword.UseSystemPasswordChar = !chkShowPass.Checked;
        }

        private void chkShowConfirm_CheckedChanged(object sender, EventArgs e)
        {
            txtConfirmPassword.UseSystemPasswordChar = !chkShowPass.Checked;
        }
    }
}
