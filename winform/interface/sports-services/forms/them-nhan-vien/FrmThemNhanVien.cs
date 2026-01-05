using SportsServices.Dto;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;

namespace SportsServices.Forms
{
    public partial class ThemNhanVien : Form
    {
        public NhanVien NhanVienMoi { get; private set; }

        public ThemNhanVien()
        {
            InitializeComponent(); this.WindowState = FormWindowState.Maximized;
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            NhanVienMoi = new NhanVien
            {
                MaNV = txtMaNV.Text.Trim(),
                HoTen = txtHoTen.Text.Trim(),
                NgaySinh = dtpNgaySinh.Value.Date,
                GioiTinh = txtGioiTinh.Text,
                CCCD = txtCCCD.Text.Trim(),
                SDT = txtSDT.Text.Trim(),
                DiaChi = txtDiaChi.Text.Trim(),
                ChucVu = txtChucVu.Text.Trim(),
                LuongCB = decimal.Parse(txtLuongCB.Text.Trim())
                // MaCoSo set bên Form1
            };

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void ThemNhanVien_Load(object sender, EventArgs e)
        {
            txtGioiTinh.Items.Add("Nam");
            txtGioiTinh.Items.Add("Nữ");
            txtGioiTinh.Items.Add("Khác");

            txtGioiTinh.DropDownStyle = ComboBoxStyle.DropDownList; // Không cho người dùng gõ

            txtChucVu.Items.Add("Nhân viên quản lý");
            txtChucVu.Items.Add("Nhân viên lễ tân");
            txtChucVu.Items.Add("Nhân viên kỹ thuật");
            txtChucVu.Items.Add("Nhân viên tin học");
            txtChucVu.DropDownStyle = ComboBoxStyle.DropDownList;

        }

        public void LoadChiNhanh(List<CoSo> coSos)
        {
            comboCN.DataSource = coSos;
            comboCN.DisplayMember = "TenCoSo";
            comboCN.ValueMember = "MaCoSo";

            comboCN.DropDownStyle = ComboBoxStyle.DropDownList;
        }

        private void label10_Click(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void labelSDT_Click(object sender, EventArgs e)
        {

        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void label9_Click(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void labelDiaChi_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void txtGioiTinh_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboCN_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void btnTaiFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "Chọn file cần tải";
            ofd.Filter = "PDF File|*.pdf|Ảnh|*.jpg;*.png|Word|*.docx|Tất cả|*.*";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                string duongDanFile = ofd.FileName;
                MessageBox.Show("Đã chọn: " + duongDanFile);
            }
        }

        private void btnTaiAnh_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "Chọn ảnh thẻ ứng viên";
            ofd.Filter = "Ảnh (*.jpg; *.jpeg; *.png)|*.jpg;*.jpeg;*.png";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                picAnhThe.Image = Image.FromFile(ofd.FileName);
            }
        }
    }
}
