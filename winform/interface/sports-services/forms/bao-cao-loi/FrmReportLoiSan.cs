using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace TTTT
{
    public partial class FormReportLoiSan : Form
    {
        // Lưu ảnh lỗi tạm
        private byte[] _anhLoiBytes;
        private string _anhLoiPath;

        // Bảng tạm giữ danh sách báo lỗi (fake data, chưa dùng DB)
        private DataTable _dsBaoLoi;

        // Tạm giả: người báo lỗi (nhân viên / khách)
        private int _currentUserId = 1;

        // Item đại diện cho 1 sân trong combobox
        private class SanItem
        {
            public int MaSan { get; set; }
            public string TenSan { get; set; }
            public string TenCoSo { get; set; }

            // Hiển thị trong combo: "Cơ sở - Tên sân"
            public override string ToString()
            {
                return $"{TenCoSo} - {TenSan}";
            }
        }

        public FormReportLoiSan()
        {
            InitializeComponent();

            // Gắn event (phòng trường hợp Designer chưa gắn)
            this.Load += FormReportLoiSan_Load;
            cboSan.SelectedIndexChanged += cboSan_SelectedIndexChanged;
            btnChonAnh.Click += btnChonAnh_Click;
            btnGuiBaoLoi.Click += btnGuiBaoLoi_Click;
            btnTroVe.Click += btnTroVe_Click;
        }

        private void FormReportLoiSan_Load(object sender, EventArgs e)
        {
            KhoiTaoBangBaoLoiTrongRam();
            CauHinhGridBaoLoi();
            LoadDanhSachSanFake();
            LoadDanhSachMucDo();
            KhoiTaoMacDinhForm();
        }

        // ==========================================
        // 1. Khởi tạo DataTable tạm để test UI
        // ==========================================
        private void KhoiTaoBangBaoLoiTrongRam()
        {
            _dsBaoLoi = new DataTable();
            _dsBaoLoi.Columns.Add("Thời gian");
            _dsBaoLoi.Columns.Add("Sân");
            _dsBaoLoi.Columns.Add("Cơ sở");
            _dsBaoLoi.Columns.Add("Mức độ");
            _dsBaoLoi.Columns.Add("Tiêu đề");
            _dsBaoLoi.Columns.Add("Mô tả");

            // Fake 1–2 dòng cho đẹp (có thể bỏ nếu không muốn)
            _dsBaoLoi.Rows.Add(
                DateTime.Now.AddHours(-5).ToString("dd/MM/yyyy HH:mm"),
                "Sân 5 người #1",
                "CS1 - Quận 5",
                "Nhẹ",
                "Lưới rách nhẹ",
                "Lưới góc phải bị rách nhẹ."
            );

            dgvDSBaoLoi.DataSource = _dsBaoLoi;
        }

        // Cấu hình DataGridView
        private void CauHinhGridBaoLoi()
        {
            dgvDSBaoLoi.AutoGenerateColumns = true; // dùng cột từ DataTable
            dgvDSBaoLoi.RowHeadersVisible = false;
            dgvDSBaoLoi.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvDSBaoLoi.MultiSelect = false;
            dgvDSBaoLoi.ReadOnly = true;
        }

        // ==========================================
        // 2. Fake danh sách sân (chưa dùng DB)
        // ==========================================
        private void LoadDanhSachSanFake()
        {
            cboSan.Items.Clear();

            // Bạn có thể thay tên CS / sân theo hệ thống của nhóm
            cboSan.Items.Add(new SanItem
            {
                MaSan = 1,
                TenSan = "Sân 5 người #1",
                TenCoSo = "CS1 - Quận 5"
            });
            cboSan.Items.Add(new SanItem
            {
                MaSan = 2,
                TenSan = "Sân 7 người #1",
                TenCoSo = "CS1 - Quận 5"
            });
            cboSan.Items.Add(new SanItem
            {
                MaSan = 3,
                TenSan = "Sân 11 người #1",
                TenCoSo = "CS2 - Thủ Đức"
            });

            if (cboSan.Items.Count > 0)
                cboSan.SelectedIndex = 0;
        }

        // Khi chọn sân → auto hiện tên cơ sở
        private void cboSan_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboSan.SelectedItem is SanItem san)
                lblCoSo.Text = san.TenCoSo;
            else
                lblCoSo.Text = "";
        }

        // ==========================================
        // 3. Load mức độ lỗi
        // ==========================================
        private void LoadDanhSachMucDo()
        {
            cboMucDo.Items.Clear();
            cboMucDo.Items.Add("Nhẹ");
            cboMucDo.Items.Add("Trung bình");
            cboMucDo.Items.Add("Nặng");
            if (cboMucDo.Items.Count > 0)
                cboMucDo.SelectedIndex = 0;
        }

        private void KhoiTaoMacDinhForm()
        {
            dtpThoiGianLoi.Value = DateTime.Now;
            txtTieuDe.Clear();
            txtMoTa.Clear();
            picAnhLoi.Image = null;
            _anhLoiBytes = null;
            _anhLoiPath = null;

            if (cboSan.Items.Count > 0)
                cboSan.SelectedIndex = 0;
            if (cboMucDo.Items.Count > 0)
                cboMucDo.SelectedIndex = 0;
        }

        // ==========================================
        // 4. Chọn ảnh lỗi
        // ==========================================
        private void btnChonAnh_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Title = "Chọn ảnh lỗi sân (tùy chọn)";
                ofd.Filter = "Ảnh (*.jpg;*.jpeg;*.png)|*.jpg;*.jpeg;*.png|Tất cả file (*.*)|*.*";

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    _anhLoiPath = ofd.FileName;
                    _anhLoiBytes = File.ReadAllBytes(ofd.FileName);

                    picAnhLoi.Image = Image.FromFile(ofd.FileName);
                    picAnhLoi.SizeMode = PictureBoxSizeMode.Zoom;
                }
            }
        }

        // ==========================================
        // 5. Kiểm tra dữ liệu trước khi gửi
        // ==========================================
        private bool ValidateForm()
        {
            if (cboSan.SelectedIndex < 0)
            {
                MessageBox.Show("Vui lòng chọn sân.", "Thiếu thông tin",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtMoTa.Text))
            {
                MessageBox.Show("Vui lòng nhập mô tả lỗi sân.", "Thiếu thông tin",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        // ==========================================
        // 6. Gửi báo lỗi (chỉ thêm vào DataTable tạm)
        // ==========================================
        private void btnGuiBaoLoi_Click(object sender, EventArgs e)
        {
            if (!ValidateForm())
                return;

            if (!(cboSan.SelectedItem is SanItem san))
            {
                MessageBox.Show("Lỗi hệ thống: không xác định được sân.",
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            DateTime thoiGian = dtpThoiGianLoi.Value;
            string mucDo = cboMucDo.SelectedItem?.ToString() ?? "";
            string tieuDe = txtTieuDe.Text.Trim();
            string moTa = txtMoTa.Text.Trim();

            // Thêm vào DataTable tạm để hiển thị trong dgvDSBaoLoi
            _dsBaoLoi.Rows.Add(
                thoiGian.ToString("dd/MM/yyyy HH:mm"),
                san.TenSan,
                san.TenCoSo,
                mucDo,
                string.IsNullOrEmpty(tieuDe) ? "(Không có tiêu đề)" : tieuDe,
                moTa
            );

            MessageBox.Show("Đã ghi nhận báo lỗi (demo, chưa lưu DB).",
                "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

            KhoiTaoMacDinhForm();
        }

        private void btnNhapLai_Click(object sender, EventArgs e)
        {
            KhoiTaoMacDinhForm();
        }

        private void btnTroVe_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // Nếu Designer lỡ tạo mấy handler rỗng thì để cũng không sao
        private void txtMoTa_TextChanged(object sender, EventArgs e) { }

        private void label8_Click(object sender, EventArgs e)
        {

        }
    }
}
