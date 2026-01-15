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
    public partial class FormDoiSan : Form
    {
        // ======== Fake model phiếu đặt sân =========
        private class PhieuDatFake
        {
            public int MaPhieu { get; set; }
            public string San { get; set; }
            public string CoSo { get; set; }
            public DateTime Ngay { get; set; }
            public TimeSpan GioBD { get; set; }
            public TimeSpan GioKT { get; set; }
            public string TrangThai { get; set; }   // "Đang đặt", "Đã huỷ"...
        }

        private List<PhieuDatFake> _dsPhieu;
        private PhieuDatFake _phieuDangChon;

        public FormDoiSan()
        {
            InitializeComponent(); this.WindowState = FormWindowState.Maximized;

            this.Load += FormDoiSan_Load;
            dgvDanhSach.CellClick += dgvDanhSach_CellClick;
            rdoHuyDat.CheckedChanged += ModeChanged;
            rdoDoiLich.CheckedChanged += ModeChanged;
            btnApDungThayDoi.Click += btnApDungThayDoi_Click;
        }

        private void FormDoiSan_Load(object sender, EventArgs e)
        {
            InitGrid();
            TaoFakePhieu();
            HienThiDanhSach(_dsPhieu);

            // setup DateTimePicker chỉ hiện dd/MM/yyyy và HH:mm
            dtpNgayMoi.Format = DateTimePickerFormat.Custom;
            dtpNgayMoi.CustomFormat = "dd/MM/yyyy";

            dtpGioBatDauMoi.Format = DateTimePickerFormat.Custom;
            dtpGioBatDauMoi.CustomFormat = "HH:mm";
            dtpGioBatDauMoi.ShowUpDown = true;

            dtpGioKetThucMoi.Format = DateTimePickerFormat.Custom;
            dtpGioKetThucMoi.CustomFormat = "HH:mm";
            dtpGioKetThucMoi.ShowUpDown = true;

            // mặc định chọn Dời lịch
            rdoDoiLich.Checked = true;
            ModeChanged(null, null);
        }

        // ============ cấu hình DataGridView ===============
        private void InitGrid()
        {
            dgvDanhSach.AutoGenerateColumns = false;
            dgvDanhSach.RowHeadersVisible = false;
            dgvDanhSach.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvDanhSach.MultiSelect = false;
            dgvDanhSach.Columns.Clear();

            var colMa = new DataGridViewTextBoxColumn
            {
                Name = "colMa",
                HeaderText = "Mã phiếu",
                Width = 70,
                ReadOnly = true
            };

            var colSan = new DataGridViewTextBoxColumn
            {
                Name = "colSan",
                HeaderText = "Sân",
                Width = 130,
                ReadOnly = true
            };

            var colCoSo = new DataGridViewTextBoxColumn
            {
                Name = "colCoSo",
                HeaderText = "Cơ sở",
                Width = 130,
                ReadOnly = true
            };

            var colNgay = new DataGridViewTextBoxColumn
            {
                Name = "colNgay",
                HeaderText = "Ngày",
                Width = 90,
                ReadOnly = true
            };

            var colGio = new DataGridViewTextBoxColumn
            {
                Name = "colGio",
                HeaderText = "Giờ",
                Width = 90,
                ReadOnly = true
            };

            var colTT = new DataGridViewTextBoxColumn
            {
                Name = "colTrangThai",
                HeaderText = "Trạng thái",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                ReadOnly = true
            };

            dgvDanhSach.Columns.AddRange(colMa, colSan, colCoSo, colNgay, colGio, colTT);
        }

        // ============ Fake data phiếu đặt sân ===============
        private void TaoFakePhieu()
        {
            _dsPhieu = new List<PhieuDatFake>
            {
                new PhieuDatFake
                {
                    MaPhieu = 101,
                    San = "Sân 5A",
                    CoSo = "Cơ sở 1",
                    Ngay = DateTime.Today,
                    GioBD = new TimeSpan(18, 0, 0),
                    GioKT = new TimeSpan(20, 0, 0),
                    TrangThai = "Đang đặt"
                },
                new PhieuDatFake
                {
                    MaPhieu = 102,
                    San = "Sân 7B",
                    CoSo = "Cơ sở 2",
                    Ngay = DateTime.Today.AddDays(1),
                    GioBD = new TimeSpan(19, 0, 0),
                    GioKT = new TimeSpan(21, 0, 0),
                    TrangThai = "Đang đặt"
                },
                new PhieuDatFake
                {
                    MaPhieu = 103,
                    San = "Sân 11",
                    CoSo = "Cơ sở 3",
                    Ngay = DateTime.Today.AddDays(2),
                    GioBD = new TimeSpan(16, 0, 0),
                    GioKT = new TimeSpan(18, 0, 0),
                    TrangThai = "Đang đặt"
                }
            };
        }

        private void HienThiDanhSach(IEnumerable<PhieuDatFake> ds)
        {
            dgvDanhSach.Rows.Clear();

            foreach (var p in ds)
            {
                dgvDanhSach.Rows.Add(
                    p.MaPhieu,
                    p.San,
                    p.CoSo,
                    p.Ngay.ToString("dd/MM/yyyy"),
                    $"{p.GioBD:hh\\:mm} - {p.GioKT:hh\\:mm}",
                    p.TrangThai
                );
            }

            _phieuDangChon = null;
            lblMaDatSan.Text = "Mã đặt sân: -";
            lblSan.Text = "Sân: -";
            lblCoSo.Text = "Cơ sở: -";
            lblThoiGianCu.Text = "Thời gian cũ: -";
            lblTrangThai.Text = "Trạng thái: -";
        }

        // ============ click chọn 1 đơn trong danh sách ============
        private void dgvDanhSach_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            // lấy mã phiếu từ cột đầu tiên
            int ma = Convert.ToInt32(dgvDanhSach.Rows[e.RowIndex].Cells["colMa"].Value);
            _phieuDangChon = _dsPhieu.FirstOrDefault(p => p.MaPhieu == ma);

            if (_phieuDangChon != null)
            {
                lblMaDatSan.Text = $"Mã đặt sân: {_phieuDangChon.MaPhieu}";
                lblSan.Text = $"Sân: {_phieuDangChon.San}";
                lblCoSo.Text = $"Cơ sở: {_phieuDangChon.CoSo}";
                lblThoiGianCu.Text =
                    $"Thời gian cũ: {_phieuDangChon.Ngay:dd/MM/yyyy} " +
                    $"{_phieuDangChon.GioBD:hh\\:mm} - {_phieuDangChon.GioKT:hh\\:mm}";
                lblTrangThai.Text = $"Trạng thái: {_phieuDangChon.TrangThai}";

                // đồng thời set mặc định khung “ngày/giờ mới”
                dtpNgayMoi.Value = _phieuDangChon.Ngay;
                dtpGioBatDauMoi.Value = DateTime.Today.Date + _phieuDangChon.GioBD;
                dtpGioKetThucMoi.Value = DateTime.Today.Date + _phieuDangChon.GioKT;
            }
        }

        // ============ bật/tắt phần đổi lịch tuỳ radio ============
        private void ModeChanged(object sender, EventArgs e)
        {
            bool doiLich = rdoDoiLich.Checked;

            dtpNgayMoi.Enabled = doiLich;
            dtpGioBatDauMoi.Enabled = doiLich;
            dtpGioKetThucMoi.Enabled = doiLich;
        }

        // ============ nút Áp dụng thay đổi (demo) ============
        private void btnApDungThayDoi_Click(object sender, EventArgs e)
        {
            if (_phieuDangChon == null)
            {
                MessageBox.Show("Vui lòng chọn một đơn đặt sân ở danh sách bên trái.",
                                "Chưa chọn đơn", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (rdoHuyDat.Checked)
            {
                MessageBox.Show(
                    $"Bạn đã HUỶ đơn #{_phieuDangChon.MaPhieu}.\n" +
                    $"Sân: {_phieuDangChon.San} - {_phieuDangChon.CoSo}\n" +
                    $"Thời gian cũ: {_phieuDangChon.Ngay:dd/MM/yyyy} " +
                    $"{_phieuDangChon.GioBD:hh\\:mm}-{_phieuDangChon.GioKT:hh\\:mm}",
                    "Huỷ đơn (demo)", MessageBoxButtons.OK, MessageBoxIcon.Information
                );
            }
            else // Dời lịch
            {
                DateTime ngayMoi = dtpNgayMoi.Value.Date;
                DateTime gioBDMoi = dtpGioBatDauMoi.Value;
                DateTime gioKTMoi = dtpGioKetThucMoi.Value;

                MessageBox.Show(
                    $"Bạn đã DỜI đơn #{_phieuDangChon.MaPhieu}.\n\n" +
                    $"Cũ: {_phieuDangChon.San} - {_phieuDangChon.CoSo}\n" +
                    $"    {_phieuDangChon.Ngay:dd/MM/yyyy} " +
                    $"{_phieuDangChon.GioBD:hh\\:mm}-{_phieuDangChon.GioKT:hh\\:mm}\n" +
                    $"Mới: {ngayMoi:dd/MM/yyyy} {gioBDMoi:HH:mm}-{gioKTMoi:HH:mm}",
                    "Dời lịch (demo)", MessageBoxButtons.OK, MessageBoxIcon.Information
                );
            }
        }

        // 2 handler auto sinh, giữ trống cũng được
        private void label2_Click(object sender, EventArgs e) { }
        private void lblMaDatSan_Click(object sender, EventArgs e) { }

        private void grpTTHienTai_Enter(object sender, EventArgs e)
        {

        }
    }
}
