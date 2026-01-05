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
    public partial class FormDatSan : Form
    {
        // Lớp nhỏ để giữ thông tin sân (theo CSDL: SAN, LOAISAN, COSO)
        private class SanInfo
        {
            public string MaSan { get; set; }
            public string TenSan { get; set; }
            public string CoSo { get; set; }      // TenCS
            public string LoaiSan { get; set; }   // TenLS
            public int SucChua { get; set; }
            public decimal GiaGio { get; set; }
        }

        private List<SanInfo> _allSan;    // tất cả sân (fake)
        private SanInfo _sanDangChon;     // sân đang chọn

        public FormDatSan()
        {
            InitializeComponent(); this.WindowState = FormWindowState.Maximized;

            // Gắn event
            this.Load += FormDatSan_Load;
            btnTimSan.Click += btnTimSan_Click;
            dgvSanTrong.CellClick += dgvSanTrong_CellClick;     // click 1 dòng để chọn sân
            btnDatSan.Click += btnDatSan_Click;
        }

        private void FormDatSan_Load(object sender, EventArgs e)
        {
            InitControls();
            InitGrid();
            TaoFakeSan();
            LoadCombos();
            HienThiDanhSachSan(_allSan);   // ban đầu show hết
        }

        //================= KHỞI TẠO CONTROL =================
        private void InitControls()
        {
            // Ngày thuê: hôm nay
            dtpNgayThue.Value = DateTime.Today;

            // Giờ bắt đầu
            dtpGioBatDau.Format = DateTimePickerFormat.Custom;
            dtpGioBatDau.CustomFormat = "HH:mm";   // chỉ giờ:phút 24h
            dtpGioBatDau.ShowUpDown = true;
            dtpGioBatDau.Value = DateTime.Today.AddHours(18);

            // Giờ kết thúc
            dtpGioKetThuc.Format = DateTimePickerFormat.Custom;
            dtpGioKetThuc.CustomFormat = "HH:mm";
            dtpGioKetThuc.ShowUpDown = true;
            dtpGioKetThuc.Value = DateTime.Today.AddHours(20);

            // Hình thức thanh toán (bám theo đề: có SportsServices.Forms, HinhThucTT)
            cboHinhThucTT.Items.Clear();
            cboHinhThucTT.Items.Add("Tiền mặt tại quầy");
            cboHinhThucTT.Items.Add("Chuyển khoản ngân hàng");
            cboHinhThucTT.Items.Add("Thanh toán online (QR)");
            if (cboHinhThucTT.Items.Count > 0) cboHinhThucTT.SelectedIndex = 0;

            lblSanDaChon.Text = "Chưa chọn";
        }

        //================= KHỞI TẠO GRID =================
        private void InitGrid()
        {
            dgvSanTrong.AutoGenerateColumns = false;
            dgvSanTrong.RowHeadersVisible = false;
            dgvSanTrong.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvSanTrong.MultiSelect = false;
            dgvSanTrong.Columns.Clear();

            // 0: Mã sân (ẩn)
            var colMaSan = new DataGridViewTextBoxColumn
            {
                Name = "colMaSan",
                HeaderText = "Mã sân",
                Visible = false
            };

            // 1: Tên sân
            var colTenSan = new DataGridViewTextBoxColumn
            {
                Name = "colTenSan",
                HeaderText = "Tên sân",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                ReadOnly = true
            };

            // 2: Cơ sở
            var colCoSo = new DataGridViewTextBoxColumn
            {
                Name = "colCoSo",
                HeaderText = "Cơ sở",
                Width = 160,
                ReadOnly = true
            };

            // 3: Loại sân
            var colLoaiSan = new DataGridViewTextBoxColumn
            {
                Name = "colLoaiSan",
                HeaderText = "Loại sân",
                Width = 110,
                ReadOnly = true
            };

            // 4: Sức chứa
            var colSucChua = new DataGridViewTextBoxColumn
            {
                Name = "colSucChua",
                HeaderText = "Sức chứa",
                Width = 80,
                ReadOnly = true
            };

            // 5: Giá/giờ
            var colGiaGio = new DataGridViewTextBoxColumn
            {
                Name = "colGiaGio",
                HeaderText = "Giá/giờ",
                Width = 100,
                ReadOnly = true
            };
            colGiaGio.DefaultCellStyle.Format = "N0"; // 250,000

            dgvSanTrong.Columns.AddRange(colMaSan, colTenSan, colCoSo, colLoaiSan, colSucChua, colGiaGio);
        }

        //================= FAKE DATA SÂN (dựa trên CSDL: COSO, LOAISAN, SAN) =================
        private void TaoFakeSan()
        {
            _allSan = new List<SanInfo>
            {
                new SanInfo { MaSan = "CS1-S5A", TenSan = "Sân 5A", CoSo = "Cơ sở 1 - Quận 1", LoaiSan = "Sân 5 người",  SucChua = 10, GiaGio = 200000 },
                new SanInfo { MaSan = "CS1-S5B", TenSan = "Sân 5B", CoSo = "Cơ sở 1 - Quận 1", LoaiSan = "Sân 5 người",  SucChua = 10, GiaGio = 220000 },
                new SanInfo { MaSan = "CS1-S7A", TenSan = "Sân 7A", CoSo = "Cơ sở 1 - Quận 1", LoaiSan = "Sân 7 người",  SucChua = 14, GiaGio = 300000 },
                new SanInfo { MaSan = "CS2-S7B", TenSan = "Sân 7B", CoSo = "Cơ sở 2 - Quận 7", LoaiSan = "Sân 7 người",  SucChua = 14, GiaGio = 320000 },
                new SanInfo { MaSan = "CS2-S5C", TenSan = "Sân 5C", CoSo = "Cơ sở 2 - Quận 7", LoaiSan = "Sân 5 người",  SucChua = 10, GiaGio = 210000 },
                new SanInfo { MaSan = "CS3-S11", TenSan = "Sân 11", CoSo = "Cơ sở 3 - Thủ Đức", LoaiSan = "Sân 11 người", SucChua = 22, GiaGio = 500000 }
            };
        }

        //================= ĐỔ DỮ LIỆU VÀO COMBOBOX =================
        private void LoadCombos()
        {
            // Cơ sở từ danh sách sân
            var dsCoSo = _allSan.Select(s => s.CoSo).Distinct().ToList();
            cboCoSo.Items.Clear();
            foreach (var cs in dsCoSo) cboCoSo.Items.Add(cs);
            if (cboCoSo.Items.Count > 0) cboCoSo.SelectedIndex = 0;

            // Loại sân
            var dsLoaiSan = _allSan.Select(s => s.LoaiSan).Distinct().ToList();
            cboLoaiSan.Items.Clear();
            foreach (var ls in dsLoaiSan) cboLoaiSan.Items.Add(ls);
            if (cboLoaiSan.Items.Count > 0) cboLoaiSan.SelectedIndex = 0;
        }

        //================= HIỂN THỊ DANH SÁCH SÂN TRONG GRID =================
        private void HienThiDanhSachSan(IEnumerable<SanInfo> ds)
        {
            dgvSanTrong.Rows.Clear();
            foreach (var s in ds)
            {
                dgvSanTrong.Rows.Add(s.MaSan, s.TenSan, s.CoSo, s.LoaiSan, s.SucChua, s.GiaGio);
            }
            _sanDangChon = null;
            lblSanDaChon.Text = "Chưa chọn";
        }

        //================= SỰ KIỆN: TÌM SÂN =================
        private void btnTimSan_Click(object sender, EventArgs e)
        {
            // hiện tại giả lập: chỉ lọc theo Cơ sở + Loại sân,
            // còn ngày + giờ chỉ hiển thị để user chọn, sau này check trùng lịch sau.
            string coSoChon = cboCoSo.SelectedItem?.ToString();
            string loaiSanChon = cboLoaiSan.SelectedItem?.ToString();

            var dsLoc = _allSan.AsEnumerable();

            if (!string.IsNullOrEmpty(coSoChon))
                dsLoc = dsLoc.Where(s => s.CoSo == coSoChon);

            if (!string.IsNullOrEmpty(loaiSanChon))
                dsLoc = dsLoc.Where(s => s.LoaiSan == loaiSanChon);

            HienThiDanhSachSan(dsLoc);
        }

        //================= SỰ KIỆN: CLICK 1 DÒNG => CHỌN SÂN =================
        private void dgvSanTrong_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            var row = dgvSanTrong.Rows[e.RowIndex];
            string maSan = row.Cells["colMaSan"].Value?.ToString();

            _sanDangChon = _allSan.FirstOrDefault(s => s.MaSan == maSan);

            if (_sanDangChon != null)
            {
                lblSanDaChon.Text = $"{_sanDangChon.TenSan} ({_sanDangChon.CoSo} - {_sanDangChon.LoaiSan})";
            }
        }

        //================= SỰ KIỆN: ĐẶT SÂN =================
        private void btnDatSan_Click(object sender, EventArgs e)
        {
            if (_sanDangChon == null)
            {
                MessageBox.Show("Vui lòng chọn một sân trong danh sách bên phải.",
                                "Chưa chọn sân", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (cboHinhThucTT.SelectedItem == null)
            {
                MessageBox.Show("Vui lòng chọn hình thức thanh toán.",
                                "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DateTime ngay = dtpNgayThue.Value.Date;
            DateTime gioBD = dtpGioBatDau.Value;
            DateTime gioKT = dtpGioKetThuc.Value;
            string hinhThuc = cboHinhThucTT.SelectedItem.ToString();

            // Demo: hiển thị thông tin phiếu đặt sân (bám sát PHIEUDATSAN + SportsServices.Forms)
            string msg =
                $"Đặt sân thành công!\n\n" +
                $"Sân: {_sanDangChon.TenSan} ({_sanDangChon.CoSo} - {_sanDangChon.LoaiSan})\n" +
                $"Ngày: {ngay:dd/MM/yyyy}\n" +
                $"Giờ: {gioBD:HH:mm} - {gioKT:HH:mm}\n" +
                $"Giá/giờ: {_sanDangChon.GiaGio:N0}đ\n" +
                $"Hình thức thanh toán: {hinhThuc}";

            MessageBox.Show(msg, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // Các handler cũ nếu muốn giữ lại thì để trống cũng được
        private void label3_Click(object sender, EventArgs e) { }
        private void dateTimePicker1_ValueChanged(object sender, EventArgs e) { }
    }
}