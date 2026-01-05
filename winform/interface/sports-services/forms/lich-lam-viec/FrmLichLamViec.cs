using System;
using System.Drawing;
using System.Windows.Forms;

namespace TTTT
{
    public partial class FormLichLamViec : Form
    {
        private bool _calendarInitialized = false;

        // Lưới lịch: 6 tuần × 7 ngày
        private Panel[,] _dayCells = new Panel[6, 7];
        private Label[,] _dayLabels = new Label[6, 7];
        // key: ngày cụ thể, value: trạng thái "Lam" / "Nghi"
        private readonly System.Collections.Generic.Dictionary<DateTime, string> _lichLamViec
            = new System.Collections.Generic.Dictionary<DateTime, string>();

        private readonly Color _mauNgayThuong = Color.White;
        private readonly Color _mauNgayLamViec = Color.LightGreen;
        private readonly Color _mauNgayNghi = Color.LightPink;


        private int _thangDangXem;
        private int _namDangXem;

        public FormLichLamViec()
        {
            InitializeComponent();

            this.Load += FormLichLamViec_Load;
            dtpThang.ValueChanged += dtpThang_ValueChanged;
            chkXinNghi.CheckedChanged += chkXinNghi_CheckedChanged;
            btnGuiDon.Click += btnGuiDon_Click;
            btnTroVe.Click += btnTroVe_Click;
        }

        /// <summary>
        /// Fake lịch làm việc cho tháng đang xem (chỉ để test giao diện).
        /// Ví dụ: ngày 5, 12, 19, 26 là ngày nghỉ; còn lại ngày thường coi là ngày làm.
        /// </summary>
        private void TaoFakeLichLamViecChoThang()
        {
            _lichLamViec.Clear();

            int daysInMonth = DateTime.DaysInMonth(_namDangXem, _thangDangXem);
            for (int day = 1; day <= daysInMonth; day++)
            {
                var d = new DateTime(_namDangXem, _thangDangXem, day);

                // ví dụ: Chủ nhật là ngày nghỉ
                if (d.DayOfWeek == DayOfWeek.Sunday)
                {
                    _lichLamViec[d] = "Nghi";
                }
                else
                {
                    // các ngày còn lại là ngày có ca làm
                    _lichLamViec[d] = "Lam";
                }
            }

            // thêm vài ngày nghỉ phép đặc biệt (demo)
            var dNghiDacBiet = new DateTime(_namDangXem, _thangDangXem, 15);
            _lichLamViec[dNghiDacBiet] = "Nghi";
        }


        private void FormLichLamViec_Load(object sender, EventArgs e)
        {
            // set tháng ban đầu cho dtpThang
            dtpThang.Value = DateTime.Today;
            _thangDangXem = dtpThang.Value.Month;
            _namDangXem = dtpThang.Value.Year;

            InitCalendarGrid();       // tạo 6x7 ô ngày
            _calendarInitialized = true;
            FillCalendarDays();

            // setup vùng xin nghỉ
            dtpNghiTu.Format = DateTimePickerFormat.Custom;
            dtpNghiTu.CustomFormat = "dd/MM/yyyy";
            dtpNghiDen.Format = DateTimePickerFormat.Custom;
            dtpNghiDen.CustomFormat = "dd/MM/yyyy";

            chkXinNghi.Checked = false;
            SetXinNghiEnabled(false);

            // cấu hình DataGridView Đơn đã gửi
            InitGridDonNghi();
        }

        private void dtpThang_ValueChanged(object sender, EventArgs e)
        {
            if (!_calendarInitialized) return;

            _thangDangXem = dtpThang.Value.Month;
            _namDangXem = dtpThang.Value.Year;

            FillCalendarDays();
        }

        /// <summary>
        /// Tạo 6x7 ô Panel + Label ngày cho tblCalendar.
        /// Hàng 0 của tblCalendar là header (Sun,Mon,...), từ hàng 1 đến 6 là ngày.
        /// </summary>
        private void InitCalendarGrid()
        {
            tblCalendar.SuspendLayout();

            // Xoá control cũ ở các hàng 1..6 nếu có
            for (int row = 1; row < tblCalendar.RowCount; row++)
            {
                for (int col = 0; col < tblCalendar.ColumnCount; col++)
                {
                    var ctl = tblCalendar.GetControlFromPosition(col, row);
                    if (ctl != null)
                        tblCalendar.Controls.Remove(ctl);
                }
            }

            // Tạo mới 6×7 ô
            for (int r = 0; r < 6; r++)
            {
                for (int c = 0; c < 7; c++)
                {
                    var cellPanel = new Panel
                    {
                        BorderStyle = BorderStyle.FixedSingle,
                        Margin = new Padding(1),
                        Dock = DockStyle.Fill
                    };

                    var lblDay = new Label
                    {
                        AutoSize = false,
                        Dock = DockStyle.Fill,
                        TextAlign = ContentAlignment.TopLeft,
                        Font = new Font(Font.FontFamily, 9, FontStyle.Regular),
                        Margin = new Padding(2)
                    };

                    cellPanel.Controls.Add(lblDay);

                    // row+1 vì hàng 0 là header
                    tblCalendar.Controls.Add(cellPanel, c, r + 1);

                    _dayCells[r, c] = cellPanel;
                    _dayLabels[r, c] = lblDay;
                }
            }

            tblCalendar.ResumeLayout();
        }

        /// <summary>
        /// Điền số ngày trong tháng hiện tại vào lưới 6x7.
        /// </summary>
        private void FillCalendarDays()
        {
            // tạo fake lịch cho tháng đang xem (demo)
            TaoFakeLichLamViecChoThang();

            // Xoá nội dung và reset màu cũ
            for (int r = 0; r < 6; r++)
            {
                for (int c = 0; c < 7; c++)
                {
                    _dayLabels[r, c].Text = "";
                    _dayCells[r, c].BackColor = _mauNgayThuong;
                }
            }

            DateTime firstOfMonth = new DateTime(_namDangXem, _thangDangXem, 1);
            int daysInMonth = DateTime.DaysInMonth(_namDangXem, _thangDangXem);

            int startCol = (int)firstOfMonth.DayOfWeek; // Sunday=0, Monday=1,...
            int row = 0;
            int col = startCol;

            for (int day = 1; day <= daysInMonth; day++)
            {
                _dayLabels[row, col].Text = day.ToString();

                // ngày hiện tại
                var d = new DateTime(_namDangXem, _thangDangXem, day);

                // xem ngày này là ngày làm hay ngày nghỉ
                if (_lichLamViec.TryGetValue(d, out string trangThai))
                {
                    if (trangThai == "Lam")
                    {
                        _dayCells[row, col].BackColor = _mauNgayLamViec;   // tô xanh
                    }
                    else if (trangThai == "Nghi")
                    {
                        _dayCells[row, col].BackColor = _mauNgayNghi;      // tô hồng/đỏ
                    }
                }

                col++;
                if (col >= 7)
                {
                    col = 0;
                    row++;
                    if (row >= 6) break;
                }
            }
        }


        private void btnTroVe_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void chkXinNghi_CheckedChanged(object sender, EventArgs e)
        {
            SetXinNghiEnabled(chkXinNghi.Checked);
        }

        private void SetXinNghiEnabled(bool enabled)
        {
            grpXinNghiPhep.Enabled = enabled;
            grpDonNghi.Enabled = enabled;
        }

        /// <summary>
        /// Cấu hình DataGridView "Đơn đã gửi"
        /// </summary>
        private void InitGridDonNghi()
        {
            dgvDonDaGui.AutoGenerateColumns = false;
            dgvDonDaGui.RowHeadersVisible = false;
            dgvDonDaGui.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvDonDaGui.MultiSelect = false;
            dgvDonDaGui.Columns.Clear();

            var colTuNgay = new DataGridViewTextBoxColumn
            {
                Name = "colTuNgay",
                HeaderText = "Từ ngày",
                Width = 80,
                ReadOnly = true
            };

            var colDenNgay = new DataGridViewTextBoxColumn
            {
                Name = "colDenNgay",
                HeaderText = "Đến ngày",
                Width = 80,
                ReadOnly = true
            };

            var colNhanVien = new DataGridViewTextBoxColumn
            {
                Name = "colNhanVien",
                HeaderText = "Nhân viên thay thế",
                Width = 120,
                ReadOnly = true
            };

            var colLyDo = new DataGridViewTextBoxColumn
            {
                Name = "colLyDo",
                HeaderText = "Lý do",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                ReadOnly = true
            };

            var colTrangThai = new DataGridViewTextBoxColumn
            {
                Name = "colTrangThai",
                HeaderText = "Trạng thái",
                Width = 90,
                ReadOnly = true
            };

            dgvDonDaGui.Columns.AddRange(colTuNgay, colDenNgay, colNhanVien, colLyDo, colTrangThai);
        }

        /// <summary>
        /// Fake gửi đơn xin nghỉ: kiểm tra dữ liệu + thêm dòng vào DataGridView
        /// </summary>
        private void btnGuiDon_Click(object sender, EventArgs e)
        {
            if (!chkXinNghi.Checked)
            {
                MessageBox.Show("Hãy tick 'Xin nghỉ phép' trước khi gửi đơn.",
                                "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            DateTime tu = dtpNghiTu.Value.Date;
            DateTime den = dtpNghiDen.Value.Date;

            if (den < tu)
            {
                MessageBox.Show("Ngày kết thúc phải lớn hơn hoặc bằng ngày bắt đầu.",
                                "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtLyDo.Text))
            {
                MessageBox.Show("Vui lòng nhập lý do xin nghỉ.",
                                "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string nvThay = string.IsNullOrWhiteSpace(txtNhanVienThayThe.Text)
                ? "(chưa sắp xếp)"
                : txtNhanVienThayThe.Text.Trim();

            // Thêm 1 dòng vào DataGridView
            dgvDonDaGui.Rows.Add(
                tu.ToString("dd/MM/yyyy"),
                den.ToString("dd/MM/yyyy"),
                nvThay,
                txtLyDo.Text.Trim(),
                "Chờ duyệt"    // fake trạng thái
            );

            MessageBox.Show("Đã gửi đơn xin nghỉ (fake).",
                            "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);

            // Clear form xin nghỉ cho lần kế tiếp
            txtLyDo.Clear();
            txtNhanVienThayThe.Clear();
            dtpNghiTu.Value = DateTime.Today;
            dtpNghiDen.Value = DateTime.Today;
        }

        // Các handler rỗng khác có thể giữ hoặc xoá cũng được
        private void label1_Click(object sender, EventArgs e) { }
        private void label2_Click(object sender, EventArgs e) { }
        private void lblSun_Click(object sender, EventArgs e) { }
        private void tblCalendar_Paint(object sender, PaintEventArgs e) { }
        private void txtLyDo_TextChanged(object sender, EventArgs e) { }
    }
}
