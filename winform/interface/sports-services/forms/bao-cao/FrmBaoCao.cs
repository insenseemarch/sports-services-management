using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using SportsServices.Dto;

namespace SportsServices.Forms
{
    public partial class FrmBaoCao : Form
    {
        public FrmBaoCao()
        {
            InitializeComponent(); this.WindowState = FormWindowState.Maximized;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Cấu hình ComboBox
            cboCoSo.Items.Clear();
            cboCoSo.Items.Add("Toàn hệ thống");
            cboCoSo.Items.Add("TP Hồ Chí Minh");
            cboCoSo.Items.Add("Hà Nội");
            cboCoSo.Items.Add("Đà Nẵng");
            cboCoSo.Items.Add("Cần Thơ");

            // Làm đẹp biểu đồ một lần duy nhất
            CauHinhGiaoDienBieuDo(chartDoanhThu);
            CauHinhGiaoDienBieuDo(chartTyLe);

            // Mặc định chọn cái đầu tiên -> Nó sẽ tự gọi hàm SelectedIndexChanged
            cboCoSo.SelectedIndex = 0;
        }

        // Hàm làm đẹp biểu đồ chung (Dùng lại nhiều lần)
        private void CauHinhBieuDo(Chart chart)
        {
            chart.Palette = ChartColorPalette.SeaGreen; // Màu xanh biển đẹp
            chart.ChartAreas[0].AxisX.MajorGrid.Enabled = false; // Tắt lưới dọc
            chart.ChartAreas[0].AxisY.MajorGrid.LineColor = Color.LightGray; // Lưới ngang màu nhạt
            chart.ChartAreas[0].AxisY.LabelStyle.Format = "#,##0"; // Định dạng số tiền
        }

        private void btnXem_Click(object sender, EventArgs e)
        {
            // 1. Nạp số liệu vào 4 thẻ KPI
            LoadKPI();

            // 2. Vẽ biểu đồ dựa trên lựa chọn
           
            VeBieuDoDoanhThu(); // Cột
            VeBieuDoNguonDat(); // Tròn

            // Ẩn bảng nhân viên đi nếu không cần, hoặc hiện doanh thu chi tiết
            dgvChiTiet.DataSource = null;
            
            // Nếu chọn nhân sự thì vẽ biểu đồ khác (bạn có thể tự thêm)
            // Ở đây mình ví dụ hiện bảng chi tiết nhân viên
            LoadBangNhanVien();
        }

        private void cboCoSo_SelectedIndexChanged(object sender, EventArgs e)
        {
            string coSoDuocChon = cboCoSo.Text;

            // Bước 1: Lấy dữ liệu tương ứng với cơ sở
            BaoCaoDto data = LayDuLieuBaoCao(coSoDuocChon);

            // Bước 2: Hiển thị lên 4 thẻ KPI
            lblTongDoanhThu.Text = data.TongDoanhThu.ToString("N0") + " đ";
            lblSoLuotDat.Text = data.SoLuotDat.ToString("N0") + " lượt";
            lblTyLeLapDay.Text = data.TyLeLapDay.ToString("0.0") + " %";
            lblTienMat.Text = data.TienMatDoHuy.ToString("N0") + " đ";

            // Bước 3: Vẽ lại 2 biểu đồ
            //VeBieuDo(chartDoanhThu, data.ListDoanhThu, "Doanh Thu (Triệu đ)");
            //VeBieuDo(chartTyLe, data.ListTyLe, "Tỷ lệ Đặt (%)");
        }


        // --- HÀM GIẢ LẬP DỮ LIỆU (SAU NÀY THAY BẰNG SQL) ---
        private BaoCaoDto LayDuLieuBaoCao(string coSo)
        {
            BaoCaoDto kq = new BaoCaoDto();

            if (coSo.Contains("Cơ sở 1"))
            {
                // Dữ liệu Cơ sở 1
                kq.TongDoanhThu = 150000000;
                kq.SoLuotDat = 120;
                kq.TyLeLapDay = 65.5;
                kq.TienMatDoHuy = 2500000;

                // Biểu đồ Cơ sở 1
                kq.ListDoanhThu.Add(new ChartData { Nhan = "Tháng 9", GiaTri = 40 });
                kq.ListDoanhThu.Add(new ChartData { Nhan = "Tháng 10", GiaTri = 55 });

                kq.ListTyLe.Add(new ChartData { Nhan = "Sáng", GiaTri = 40 });
                kq.ListTyLe.Add(new ChartData { Nhan = "Chiều", GiaTri = 80 });
            }
            else if (coSo.Contains("Cơ sở 2"))
            {
                // Dữ liệu Cơ sở 2
                kq.TongDoanhThu = 280000000;
                kq.SoLuotDat = 300;
                kq.TyLeLapDay = 85.2;
                kq.TienMatDoHuy = 5000000;

                // Biểu đồ Cơ sở 2 (Cao hơn)
                kq.ListDoanhThu.Add(new ChartData { Nhan = "Tháng 9", GiaTri = 80 });
                kq.ListDoanhThu.Add(new ChartData { Nhan = "Tháng 10", GiaTri = 95 });

                kq.ListTyLe.Add(new ChartData { Nhan = "Sáng", GiaTri = 60 });
                kq.ListTyLe.Add(new ChartData { Nhan = "Chiều", GiaTri = 95 });
            }
            else // Toàn hệ thống (Cộng dồn)
            {
                kq.TongDoanhThu = 430000000;
                kq.SoLuotDat = 420;
                kq.TyLeLapDay = 75.3;
                kq.TienMatDoHuy = 7500000;

                kq.ListDoanhThu.Add(new ChartData { Nhan = "Tháng 9", GiaTri = 120 });
                kq.ListDoanhThu.Add(new ChartData { Nhan = "Tháng 10", GiaTri = 150 });

                kq.ListTyLe.Add(new ChartData { Nhan = "Sáng", GiaTri = 50 });
                kq.ListTyLe.Add(new ChartData { Nhan = "Chiều", GiaTri = 88 });
            }

            return kq;
        }

        // --- HÀM VẼ BIỂU ĐỒ CHUNG ---
        private void VeBieuDo(Chart chart, List<ChartData> data, string tenSeries)
        {
            chart.Series.Clear();

            // Tạo Series mới
            Series series = new Series(tenSeries);
            series.ChartType = SeriesChartType.Column; // Dạng cột
            series.IsValueShownAsLabel = true; // Hiện số trên đầu cột

            // Đổ dữ liệu vào
            foreach (var item in data)
            {
                series.Points.AddXY(item.Nhan, item.GiaTri);
            }

            chart.Series.Add(series);
        }

        // --- HÀM LÀM ĐẸP GIAO DIỆN BIỂU ĐỒ ---
        private void CauHinhGiaoDienBieuDo(Chart chart)
        {
            chart.Palette = ChartColorPalette.SeaGreen; // Màu xanh hiện đại
            chart.ChartAreas[0].AxisX.MajorGrid.Enabled = false; // Tắt lưới dọc
            chart.ChartAreas[0].AxisY.MajorGrid.LineColor = Color.LightGray; // Lưới ngang nhạt
        }
        private void LoadKPI()
        {
            var data = KhoDuLieuBaoCao.GetSoLieuKPI();

            // Gán dữ liệu và định dạng tiền tệ (N0 là định dạng số có dấu phẩy: 150,000)
            lblTongDoanhThu.Text = data.TongDoanhThu.ToString("N0") + " đ";
            lblTyLeLapDay.Text = data.TyLeLapDay + "%";
            lblSoLuotDat.Text = data.TongLuotDat + " lượt";
            lblTienMat.Text = data.TienMatDoHuy.ToString("N0") + " đ";
        }

        private void VeBieuDoDoanhThu()
        {
            var listDoanhThu = KhoDuLieuBaoCao.GetDoanhThu();

            chartDoanhThu.Series.Clear();
            chartDoanhThu.Titles.Clear();
            chartDoanhThu.Titles.Add("DOANH THU THEO THÁNG");

            Series series = chartDoanhThu.Series.Add("DoanhThu");
            series.ChartType = SeriesChartType.Column; // Biểu đồ Cột
            series.IsValueShownAsLabel = true;

            foreach (var item in listDoanhThu)
            {
                series.Points.AddXY(item.Thang, item.SoTien);
            }
        }

        private void VeBieuDoNguonDat()
        {
            var listNguon = KhoDuLieuBaoCao.GetNguonDat();

            chartTyLe.Series.Clear();
            chartTyLe.Titles.Clear();
            chartTyLe.Titles.Add("TỶ LỆ NGUỒN KHÁCH");

            Series series = chartTyLe.Series.Add("Nguon");
            series.ChartType = SeriesChartType.Doughnut; // Biểu đồ bánh Donut (nhìn sang hơn Pie)
            series.IsValueShownAsLabel = true;
            series.Label = "#PERCENT"; // Hiện % trên biểu đồ
            series.LegendText = "#VALX"; // Hiện tên chú thích bên cạnh

            foreach (var item in listNguon)
            {
                series.Points.AddXY(item.Nguon, item.SoLuong);
            }
        }

        private void LoadBangNhanVien()
        {
            var listNV = KhoDuLieuBaoCao.GetNhanVien();

            dgvChiTiet.DataSource = null;
            dgvChiTiet.DataSource = listNV;

            // Đổi tên cột cho đẹp
            dgvChiTiet.Columns["MaNV"].HeaderText = "Mã NV";
            dgvChiTiet.Columns["TenNV"].HeaderText = "Họ và Tên";
            dgvChiTiet.Columns["TongGioLam"].HeaderText = "Tổng Giờ";
            dgvChiTiet.Columns["LuongTamTinh"].HeaderText = "Lương Tạm Tính";
            dgvChiTiet.Columns["LuongTamTinh"].DefaultCellStyle.Format = "N0";

            dgvChiTiet.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }
    }


    // Class chứa dữ liệu cho 1 điểm biểu đồ
    public class ChartData
    {
        public string Nhan { get; set; } // Ví dụ: Tháng 1, Tháng 2...
        public double GiaTri { get; set; }
    }

    // Class chứa toàn bộ báo cáo
    public class BaoCaoDto
    {
        // 4 Chỉ số KPI
        public decimal TongDoanhThu { get; set; }
        public int SoLuotDat { get; set; }
        public double TyLeLapDay { get; set; }
        public decimal TienMatDoHuy { get; set; }

        // Dữ liệu cho 2 biểu đồ
        public List<ChartData> ListDoanhThu { get; set; } = new List<ChartData>();
        public List<ChartData> ListTyLe { get; set; } = new List<ChartData>();
    }
}
