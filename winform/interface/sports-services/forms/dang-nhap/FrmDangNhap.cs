using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace SportsServices.Forms
{
    public partial class FrmDangNhap : Form
    {
        public FrmDangNhap()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen; // Hiện giữa màn hình
        }

        private void FrmDangNhap_Load(object sender, EventArgs e)
        {
            // Mặc định focus vào ô username
            txtUsername.Focus();
        }

        private void btnDangNhap_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text.Trim();

            // 1. Validate dữ liệu
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ Tên đăng nhập và Mật khẩu!",
                                "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 2. Kiểm tra đăng nhập với CSDL (Khớp với bảng TAI_KHOAN bên FrmDangKy)
            // Lưu ý: Nếu bạn có hàm DbHelper.ExecuteQuery hoặc GetDataTable thì dùng nó nhé.
            // Ở đây mình viết code SQL thuần để đảm bảo chạy được.
            try
            {
                // Giả sử DbHelper.ConnectionString là chuỗi kết nối của bạn
                // Nếu DbHelper chưa public chuỗi này, bạn hãy thay bằng chuỗi kết nối thật
                string query = "SELECT Count(*) FROM TAI_KHOAN WHERE Username = @User AND PasswordHash = @Pass";

                // Cách gọi qua DbHelper (nếu có hàm trả về giá trị):
                // int count = (int)DbHelper.ExecuteScalar(query, ...);

                // Hoặc viết trực tiếp để test:
                // using (SqlConnection conn = new SqlConnection("CHUOI_KET_NOI_CUA_BAN"))
                // {
                //     conn.Open();
                //     SqlCommand cmd = new SqlCommand(query, conn);
                //     cmd.Parameters.AddWithValue("@User", username);
                //     cmd.Parameters.AddWithValue("@Pass", password);
                //     int count = (int)cmd.ExecuteScalar();
                // }

                // --- LOGIC GIẢ LẬP ĐỂ BẠN CHẠY TEST GIAO DIỆN ---
                // (Xóa đoạn if này và dùng code SQL thật ở trên khi đã kết nối DB)
                bool loginSuccess = false;

                // Check admin cứng hoặc check DB
                if (username == "admin" && password == "admin") loginSuccess = true;

                // Nếu DB trả về > 0 dòng thì loginSuccess = true;

                if (loginSuccess)
                {
                    MessageBox.Show("Đăng nhập thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    this.Hide(); // Ẩn form đăng nhập

                    // Mở Form Main (Lưu ý tên class Form chính của bạn, vd: Form1 hoặc FrmMain)
                    Form1 frmMain = new Form1();
                    frmMain.ShowDialog();

                    this.Close(); // Đóng hẳn khi form main tắt
                }
                else
                {
                    MessageBox.Show("Sai tên đăng nhập hoặc mật khẩu!", "Lỗi đăng nhập", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi kết nối: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
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