using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq; // Cần dùng LINQ để tìm trong List
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SportsServices.Dto; // Để dùng FakeDatabase và TaiKhoan

namespace SportsServices.Forms
{
    public partial class FrmXemProfileNhanVien : Form
    {
        private TaiKhoan _currentUser; // Tài khoản đang đăng nhập
        private NhanVien _nhanVienChiTiet; // Thông tin chi tiết nhân viên

        // Constructor nhận vào tài khoản từ FormMain
        public FrmXemProfileNhanVien(TaiKhoan user)
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;
            _currentUser = user;
        }

        // Constructor mặc định (để tránh lỗi Designer, nhưng ít dùng)
        public FrmXemProfileNhanVien()
        {
            InitializeComponent();
        }

        private void FrmXemProfileNhanVien_Load(object sender, EventArgs e)
        {
            if (_currentUser == null) return;

            LoadThongTinNhanVien();
        }

        private void LoadThongTinNhanVien()
        {
            // 1. Hiển thị thông tin Tài khoản
            txtTenDangNhap.Text = _currentUser.Username;
            txtPass.Text = _currentUser.Password; // Mật khẩu hiện tại

            // 2. Tìm thông tin chi tiết trong FakeDatabase.NhanViens
            // Logic: Tìm người có HoTen trùng với HoTen của tài khoản
            _nhanVienChiTiet = FakeDatabase.NhanViens.FirstOrDefault(nv => nv.HoTen == _currentUser.HoTen);

            if (_nhanVienChiTiet != null)
            {
                // Nếu tìm thấy trong danh sách nhân viên
                txtHoTen.Text = _nhanVienChiTiet.HoTen;
                dtpNgaySinh.Value = _nhanVienChiTiet.NgaySinh;

                // Kiểm tra control txtCCCD có tồn tại không, nếu không thì bỏ qua
                if (IsControlAvailable(nameof(txtCCCD))) txtCCCD.Text = _nhanVienChiTiet.CCCD;

                txtSDT.Text = _nhanVienChiTiet.SDT;
                txtDiaChi.Text = _nhanVienChiTiet.DiaChi;

                // Hiển thị lương (nếu có TextBox txtLuongCoBan)
                // Lưu ý: Lương thường để ReadOnly = true vì nhân viên không tự sửa lương được
                if (IsControlAvailable("txtLuongCoBan"))
                    Controls["txtLuongCoBan"].Text = _nhanVienChiTiet.LuongCB.ToString("N0") + " VNĐ";
            }
            else
            {
                // Trường hợp user mới tạo chưa có trong danh sách nhân viên chi tiết
                txtHoTen.Text = _currentUser.HoTen;
                txtDiaChi.Text = "Chưa cập nhật";
                txtSDT.Text = "Chưa cập nhật";
            }
        }

        // Hàm phụ trợ kiểm tra xem TextBox có tồn tại trên form không (tránh lỗi)
        private bool IsControlAvailable(string name)
        {
            return this.Controls.Find(name, true).Length > 0;
        }

        private void btnShowHide_CheckedChanged(object sender, EventArgs e)
        {
            // Logic ẩn hiện mật khẩu
            if (txtPass.PasswordChar == '*')
            {
                txtPass.PasswordChar = '\0'; // Hiện
            }
            else
            {
                txtPass.PasswordChar = '*'; // Ẩn
            }
        }

        private void btnDoiMatKhau_Click(object sender, EventArgs e)
        {
            string matKhauHienTai = txtPass.Text;

            // Mở form đổi mật khẩu dành cho Nhân viên
            frmDoiMatKhauNV frm = new frmDoiMatKhauNV(matKhauHienTai);

            if (frm.ShowDialog() == DialogResult.OK)
            {
                // 1. Cập nhật trên giao diện
                txtPass.Text = frm.MatKhauMoi;

                // 2. Cập nhật vào biến session
                _currentUser.Password = frm.MatKhauMoi;

                // 3. Cập nhật vào FakeDatabase (List TaiKhoans)
                var userInDb = FakeDatabase.TaiKhoans.FirstOrDefault(t => t.Username == _currentUser.Username);
                if (userInDb != null)
                {
                    userInDb.Password = frm.MatKhauMoi;
                }

                MessageBox.Show("Đổi mật khẩu thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnLuu_Click(object sender, EventArgs e)
        {
            // Validate dữ liệu
            if (string.IsNullOrEmpty(txtHoTen.Text) || string.IsNullOrEmpty(txtSDT.Text))
            {
                MessageBox.Show("Vui lòng nhập họ tên và số điện thoại.", "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // 1. Cập nhật vào biến cục bộ _nhanVienChiTiet (nếu đã kết nối được)
                if (_nhanVienChiTiet != null)
                {
                    _nhanVienChiTiet.HoTen = txtHoTen.Text;
                    _nhanVienChiTiet.NgaySinh = dtpNgaySinh.Value;
                    _nhanVienChiTiet.SDT = txtSDT.Text;
                    _nhanVienChiTiet.DiaChi = txtDiaChi.Text;
                    if (IsControlAvailable("txtCCCD")) _nhanVienChiTiet.CCCD = txtCCCD.Text;
                }
                else
                {
                    // Nếu chưa có thì tạo mới (Optional - tuỳ logic)
                    // Ở đây chỉ cập nhật tên hiển thị
                }

                // 2. Cập nhật tên vào Tài khoản (để hiển thị Xin chào ở form chính đúng tên mới)
                _currentUser.HoTen = txtHoTen.Text;
                var userInDb = FakeDatabase.TaiKhoans.FirstOrDefault(t => t.Username == _currentUser.Username);
                if (userInDb != null) userInDb.HoTen = txtHoTen.Text;

                MessageBox.Show("Cập nhật thông tin thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi cập nhật: " + ex.Message);
            }
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            // Chỉ đóng form này, không thoát cả ứng dụng (Application.Exit)
            // Vì form này nằm trong Panel của FrmMainStaff
            this.Close();
        }

        // Tính tuổi tự động khi chọn ngày sinh
        private void dtpNgaySinh_ValueChanged(object sender, EventArgs e)
        {
            DateTime ngaySinh = dtpNgaySinh.Value;
            int tuoi = DateTime.Now.Year - ngaySinh.Year;
            if (DateTime.Now < ngaySinh.AddYears(tuoi)) tuoi--;

            // Nếu có label hiển thị tuổi thì gán vào đây
            // lblTuoi.Text = tuoi.ToString();
        }

        // Các event thừa (Do copy paste hoặc click nhầm designer), cứ để trống
        private void label1_Click(object sender, EventArgs e) { }
        private void txtLuongCoBan_TextChanged(object sender, EventArgs e) { }
        private void label2_Click(object sender, EventArgs e) { }
        private void Form1_Load(object sender, EventArgs e) { } // Hàm load cũ
    }
}