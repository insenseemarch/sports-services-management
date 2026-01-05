using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PhanCongCaTruc
{
    public partial class frmChiTietCa : Form
    {
        // 1. Các biến để hứng dữ liệu từ Form Lịch truyền sang
        private string _tenCa;
        private DateTime _ngay;

        // Biến này để lưu bản ghi đang thao tác (nếu đã có)
        private ThongTinCa _dataHienTai;
        private string _chiNhanh;

        // 2. Sửa Constructor để nhận tham số (Tên ca và Ngày)
        public frmChiTietCa(string tenCa, DateTime ngay, string chiNhanh)
        {
            InitializeComponent();
            _tenCa = tenCa;
            _ngay = ngay;
            _chiNhanh = chiNhanh; // <-- Lưu lại
        }

        // 3. Khi Form vừa mở lên (Sự kiện Load)
        private void frmChiTietCa_Load(object sender, EventArgs e)
        {
            lblTieuDe.Text = _tenCa.ToUpper() + " - NGÀY " + _ngay.ToString("dd/MM/yyyy");

            _dataHienTai = KhoDuLieu.DanhSachPhanCong.FirstOrDefault(x =>
            x.TenCa == _tenCa &&
            x.Ngay == _ngay &&
            x.ChiNhanh == _chiNhanh); // <-- Thêm điều kiện

            if (_dataHienTai != null)
            {
                // Nếu CÓ dữ liệu cũ -> Hiện lên
                txtQuanLy.Text = _dataHienTai.NguoiQuanLy;
                rtbNhanVien.Text = _dataHienTai.DanhSachNV;
            }
            else
            {
                // Nếu CHƯA có dữ liệu -> Để trống
                txtQuanLy.Text = "";
                rtbNhanVien.Text = "";

                // (MẸO UX) Tooltip hướng dẫn: 
                // Bạn có thể dùng thuộc tính PlaceholderText (nếu dùng .NET mới) 
                // hoặc gán tooltips để user biết cách nhập.
            }
        }

        private void txtQuanLy_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnLuu_Click_1(object sender, EventArgs e)
        {
            // Validate: Không nhập gì thì nhắc nhở (Tùy chọn)
            if (string.IsNullOrWhiteSpace(txtQuanLy.Text) && string.IsNullOrWhiteSpace(rtbNhanVien.Text))
            {
                var hoi = MessageBox.Show("Thông tin đang trống, bạn có chắc muốn lưu không?", "Xác nhận", MessageBoxButtons.YesNo);
                if (hoi == DialogResult.No) return;
            }

            // --- BƯỚC 1: KIỂM TRA ĐỊNH DẠNG (VALIDATION) ---

            // 1.1. Kiểm tra Quản lý (Bắt buộc phải có dấu "-")
            // Ví dụ đúng: "QL01 - Nguyễn Văn A"
            if (!string.IsNullOrWhiteSpace(txtQuanLy.Text) && !txtQuanLy.Text.Contains("-"))
            {
                MessageBox.Show("Tên Quản lý phải có định dạng: Mã - Tên\nVí dụ: QL01 - Nguyễn Văn A", "Sai định dạng");
                txtQuanLy.Focus(); // Đưa con trỏ về ô này để sửa
                return; // Dừng lại, không lưu
            }

            // 1.2. Kiểm tra Danh sách nhân viên
            // Duyệt qua từng dòng trong ô nhập, dòng nào có chữ mà thiếu dấu "-" là báo lỗi ngay
            foreach (string line in rtbNhanVien.Lines)
            {
                if (!string.IsNullOrWhiteSpace(line) && !line.Contains("-"))
                {
                    MessageBox.Show("Nhân viên '" + line + "' sai định dạng!\nVui lòng nhập: Mã - Tên", "Sai định dạng");
                    return;
                }
            }

            // 1. Nếu chưa có trong kho thì Tạo mới (Add)
            if (_dataHienTai == null)
            {
                _dataHienTai = new ThongTinCa();
                _dataHienTai.TenCa = _tenCa;
                _dataHienTai.Ngay = _ngay;
                _dataHienTai.ChiNhanh = _chiNhanh;
                KhoDuLieu.DanhSachPhanCong.Add(_dataHienTai);
            }

            // 2. Cập nhật thông tin từ ô nhập vào biến
            _dataHienTai.NguoiQuanLy = txtQuanLy.Text;
            _dataHienTai.DanhSachNV = rtbNhanVien.Text;

            // 3. Thông báo và Đóng form
            MessageBox.Show("Đã lưu phân công thành công!", "Thông báo");
            this.Close(); // Đóng form để quay về lịch
        }

        private void btnDong_Click(object sender, EventArgs e)
        {
            // Đóng form mà không làm gì cả
            this.Close();
        }
    }
}
