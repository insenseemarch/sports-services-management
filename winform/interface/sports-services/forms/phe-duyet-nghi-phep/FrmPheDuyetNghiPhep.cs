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
    public partial class Form1 : Form // Nếu form bạn tên khác thì sửa chữ Form1 này
    {
        public Form1()
        {
            InitializeComponent();
        }

        // 1. KHI FORM VỪA CHẠY
        private void Form1_Load(object sender, EventArgs e)
        {
            KhoDonPhep.TaoDuLieuMau();

            // Nạp Combobox
            cboChiNhanh.Items.Clear();
            cboChiNhanh.Items.Add("Tất cả");
            cboChiNhanh.Items.Add("Thành Phố Hồ Chí Minh");
            cboChiNhanh.Items.Add("Hà Nội");
            cboChiNhanh.Items.Add("Đà Nẵng");
            cboChiNhanh.Items.Add("Cần Thơ");
            cboChiNhanh.SelectedIndex = 0;

            // Cấu hình bảng
            dgvDanhSachDon.AutoGenerateColumns = false; // QUAN TRỌNG: Tắt tự sinh cột
            dgvDanhSachDon.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            SetButtonState(false);
        }

        // 2. HÀM LOAD DỮ LIỆU (QUAN TRỌNG NHẤT)
        private void LoadLuoi()
        {
            string chiNhanh = cboChiNhanh.Text;
            var listHienThi = KhoDonPhep.DanhSachDon;

            if (chiNhanh != "Tất cả")
            {
                listHienThi = KhoDonPhep.DanhSachDon.Where(x => x.ChiNhanh == chiNhanh).ToList();
            }

            dgvDanhSachDon.DataSource = null;
            dgvDanhSachDon.AutoGenerateColumns = false;
            dgvDanhSachDon.DataSource = listHienThi;

            ToMauTrangThai();
        }

        private void cboChiNhanh_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadLuoi();
            ResetChiTiet();
        }
        // 4. KHI CLICK VÀO BẢNG -> HIỆN CHI TIẾT & BẮT RÀNG BUỘC
        private void dgvDanhSachDon_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            // 1. Lấy Mã Đơn từ cột đầu tiên (Hoặc cột bạn đặt tên là colMaDon)
            // Cách an toàn nhất là lấy theo DataBoundItem (dữ liệu gốc của dòng)
            var donDuocChon = dgvDanhSachDon.Rows[e.RowIndex].DataBoundItem as DonNghiPhep;

            if (donDuocChon != null)
            {
                // 2. Điền dữ liệu vào ô chi tiết
                txtMaDon.Text = donDuocChon.MaDon;

                // Ở đây ta hiển thị Tên (dù trên lưới chỉ hiện Mã) -> Rất chuyên nghiệp
                txtTenNV.Text = donDuocChon.TenNV;

                txtLyDo.Text = donDuocChon.Lydo;

                txtNguoiThayThe.Text = donDuocChon.TenNVThayThe;

                // 3. Xử lý nút bấm
                if (donDuocChon.TrangThai == "Chờ duyệt")
                {
                    SetButtonState(true);
                }
                else
                {
                    SetButtonState(false);
                }
            }
        }

        private void btnDuyet_Click(object sender, EventArgs e)
        {
            XuLyDon("Đã duyệt");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            XuLyDon("Từ chối");
        }

        private void XuLyDon(string trangThaiMoi)
        {
            string maDon = txtMaDon.Text;
            var don = KhoDonPhep.DanhSachDon.FirstOrDefault(x => x.MaDon == maDon);

            if (don != null)
            {
                don.TrangThai = trangThaiMoi;
                MessageBox.Show("Đã cập nhật: " + trangThaiMoi);
                LoadLuoi();
                SetButtonState(false);
            }
        }

        private void SetButtonState(bool enable)
        {
            btnDuyet.Enabled = enable;
            btnTuChoi.Enabled = enable;

            btnDuyet.BackColor = enable ? Color.ForestGreen : Color.Gray;
            btnTuChoi.BackColor = enable ? Color.Crimson : Color.Gray;
        }

        private void ToMauTrangThai()
        {
            foreach (DataGridViewRow row in dgvDanhSachDon.Rows)
            {
                // Kiểm tra null trước khi ToString()
                if (row.Cells["colTrangThai"].Value != null)
                {
                    string tt = row.Cells["colTrangThai"].Value.ToString();
                    if (tt == "Đã duyệt") row.DefaultCellStyle.BackColor = Color.LightGreen;
                    else if (tt == "Từ chối") row.DefaultCellStyle.BackColor = Color.LightPink;
                }
            }
        }

        private void ResetChiTiet()
        {
            txtMaDon.Text = "";
            txtTenNV.Text = "";
            txtLyDo.Text = "";
            txtNguoiThayThe.Text = "";
            SetButtonState(false);
        }
    }
}
