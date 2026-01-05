using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SportsServices.Forms
{
    public partial class FrmXemProfileNhanVien : Form
    {
        public FrmXemProfileNhanVien()
        {
            InitializeComponent(); this.WindowState = FormWindowState.Maximized;
        }

        // Giả sử đây là ID của khách hàng đang đăng nhập
        private string currentNVien = "KH001";

        private void Form1_Load(object sender, EventArgs e)
        {
            LoadThongTinKhachHang(currentNVien);
        }

        private void LoadThongTinKhachHang(string maKH)
        {
            // --- MÔ PHỎNG DỮ LIỆU TỪ SQL (Thay đoạn này bằng query SQL thật của bạn) ---
            // Bảng TÀI KHOẢN
            string tenDangNhap = "nguyenvana123";
            DateTime ngayDangKy = new DateTime(2023, 1, 15);

            // Bảng KHÁCH HÀNG
            string hoTen = "Nguyễn Văn A";
            DateTime ngaySinh = new DateTime(2000, 5, 20);
            string cccd = "079200001234";
            string sdt = "0909123456";
            string email = "nguyenvana@gmail.com";
            string diaChi = "123 Đường ABC, TP.HCM";
            bool isHSSV = true;
            // --------------------------------------------------------------------------

            // Đổ dữ liệu vào các Control
            // Nhóm tài khoản
            txtPass.Text = "123";
            txtTenDangNhap.Text = tenDangNhap;
            dtpNgayDangKy.Value = ngayDangKy;


            // Nhóm cá nhân
            txtHoTen.Text = hoTen;
            dtpNgaySinh.Value = ngaySinh;
            txtCCCD.Text = cccd;
            txtSDT.Text = sdt;
            txtEmail.Text = email;
            txtDiaChi.Text = diaChi;
        }
        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void dtpNgaySinh_ValueChanged(object sender, EventArgs e)
        {
            // 1. Tính tuổi chính xác
            DateTime ngaySinh = dtpNgaySinh.Value;
            DateTime homNay = DateTime.Now;

            int tuoi = homNay.Year - ngaySinh.Year;

            // Kiểm tra xem đã tới sinh nhật năm nay chưa, nếu chưa thì trừ đi 1 tuổi
            if (homNay < ngaySinh.AddYears(tuoi))
            {
                tuoi--;
            }
        }

        private void btnShowHide_CheckedChanged(object sender, EventArgs e)
        {
            // '\0' là ký tự rỗng (null character), nghĩa là không che gì cả -> Hiện mật khẩu
            // '*' là ký tự dùng để che -> Ẩn mật khẩu

            if (txtPass.PasswordChar == '*')
            {
                // Đang ẩn -> Chuyển sang hiện
                txtPass.PasswordChar = '\0';
                btnShowHide.Checked = true; // (Tuỳ chọn) Đổi tên nút
            }
            else
            {
                // Đang hiện -> Chuyển sang ẩn
                txtPass.PasswordChar = '*';
                btnShowHide.Checked = false; // (Tuỳ chọn) Đổi tên nút
            }
        }

        private void btnDoiMatKhau_Click(object sender, EventArgs e)
        {
            // Lấy mật khẩu đang có trên ô text (hoặc từ biến lưu trữ của bạn)
            string matKhauHienTai = txtPass.Text;

            // Khởi tạo form đổi mật khẩu và truyền mật khẩu hiện tại vào
            frmDoiMatKhauKhach frm = new frmDoiMatKhauKhach(matKhauHienTai);

            // Hiện form lên dưới dạng Dialog (người dùng phải xử lý xong mới quay lại được form chính)
            if (frm.ShowDialog() == DialogResult.OK)
            {
                // Nếu người dùng bấm "Lưu" thành công
                // Cập nhật mật khẩu mới lên giao diện
                txtPass.Text = frm.MatKhauMoi;

                // Cập nhật vào CSDL (Database) nếu có
                // UpdatePasswordToDatabase(frm.MatKhauMoi);

                MessageBox.Show("Đổi mật khẩu thành công!");
            }
            else
            {
                // Trường hợp người dùng bấm Hủy hoặc tắt form thì không làm gì cả
            }
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            // Hiện hộp thoại xác nhận
            DialogResult hoi = MessageBox.Show("Bạn có chắc chắn muốn thoát chương trình không?", "Xác nhận thoát", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            // Nếu người dùng chọn Yes thì mới thoát
            if (hoi == DialogResult.Yes)
            {
                Application.Exit(); // Lệnh này sẽ tắt toàn bộ chương trình
            }
        }

        private void btnLuu_Click(object sender, EventArgs e)
        {
            // BƯỚC 1: Kiểm tra dữ liệu nhập vào (Validate)
            if (string.IsNullOrEmpty(txtHoTen.Text) || string.IsNullOrEmpty(txtSDT.Text))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ Họ tên và Số điện thoại!", "Thông báo");
                return; // Dừng lại, không làm tiếp
            }

            try
            {
                // BƯỚC 2: Thực hiện Lưu vào Database (SQL)
                // Ví dụ: UpdateThongTin(txtHoTen.Text, dtpNgaySinh.Value, ...);

                // --- VIẾT CODE SQL UPDATE CỦA BẠN Ở ĐÂY ---
                // string sql = "UPDATE KhachHang SET HoTen = @HoTen, NgaySinh = @NgaySinh ... WHERE ID = ...";
                // ... thực thi câu lệnh ...
                // --------------------------------------------


                // BƯỚC 3: Thông báo thành công
                MessageBox.Show("Cập nhật thông tin thành công!", "Thông báo");

                // BƯỚC 4: Refresh (Tải lại) thông tin
                // Gọi lại hàm load dữ liệu ban đầu để giao diện hiển thị thông tin mới nhất
                LoadSportsServices();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Có lỗi xảy ra: " + ex.Message);
            }
        }
        private void LoadSportsServices()
        {
            // Code lấy dữ liệu từ SQL và gán vào các TextBox, DateTimePicker...
            // Ví dụ:
            // txtHoTen.Text = dataTuSQL["HoTen"].ToString();
            // dtpNgaySinh.Value = ...
        }

        private void txtLuongCoBan_TextChanged(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
    }
}
