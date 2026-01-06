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
    public partial class PhanCongCaTruc : Form
    {
        // Biến lưu ngày Thứ 2 của tuần hiện tại
        DateTime _ngayDauTuan;

        public PhanCongCaTruc()
        {
            InitializeComponent();
        }

        private void PhanCongCaTruc_Load(object sender, EventArgs e)
        {
            // ---- BƯỚC KHỞI TẠO BAN ĐẦU ----
            // 1. Tạo dữ liệu mẫu
            KhoDuLieu.TaoDuLieuMau();

            // 2. Vẽ tiêu đề ngày
            HienThiNgayLenHeader();

            // 3. Vẽ các dòng Ca và đổ dữ liệu lên
            LoadDuLieuLenLuoi();

            

            // ---- TÙY CHỈNH GIAO DIỆN DGV ----

            // 1. Cho phép chữ trong ô tự xuống dòng nếu dài quá
            dgvLichTuan.DefaultCellStyle.WrapMode = DataGridViewTriState.True;

            // 2. Tự động giãn chiều cao của dòng (Row Height) để chứa đủ chữ
            dgvLichTuan.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;

            // (Tùy chọn) Căn giữa nội dung cho đẹp
            dgvLichTuan.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            // Chặn lỗi click
            dgvLichTuan.ReadOnly = true; // Không cho sửa tay trên lưới -> Click lúc nào cũng ăn
            dgvLichTuan.SelectionMode = DataGridViewSelectionMode.CellSelect; // Click là chọn cả ô
            dgvLichTuan.MultiSelect = false; // Chỉ cho chọn 1 ô 1 lúc cho đỡ lỗi
        }

        private void LoadDuLieuLenLuoi()
        {
            dgvLichTuan.Rows.Clear(); // Xóa sạch bảng cũ đi vẽ lại từ đầu

            string[] cacCa = { "Ca Sáng (7h-11h)", "Ca Chiều (13h-17h)", "Ca Tối (18h-22h)" };

            string chiNhanhHienTai = cboChiNhanh.Text;

            foreach (string tenCa in cacCa)
            {
                int rowIndex = dgvLichTuan.Rows.Add();
                dgvLichTuan.Rows[rowIndex].Cells[0].Value = tenCa;

                // Quét qua 7 ngày
                for (int i = 1; i <= 7; i++)
                {
                    if (dgvLichTuan.Columns[i].Tag == null) continue;

                    DateTime ngayCot = (DateTime)dgvLichTuan.Columns[i].Tag;

                    // Tìm xem có dữ liệu vừa lưu không
                    var caTruc = KhoDuLieu.DanhSachPhanCong.FirstOrDefault(x =>
                    x.TenCa == tenCa &&
                    x.Ngay.Date == ngayCot.Date &&
                    x.ChiNhanh == chiNhanhHienTai); // <--- THÊM ĐIỀU KIỆN NÀY

                    if (caTruc != null)
                    {
                        // --- NẾU CÓ DỮ LIỆU THÌ HIỆN RA ĐÂY ---
                        // Kiểm tra xem có tên quản lý không
                        if (!string.IsNullOrEmpty(caTruc.NguoiQuanLy))
                        {
                            dgvLichTuan.Rows[rowIndex].Cells[i].Value = caTruc.NguoiQuanLy;
                            dgvLichTuan.Rows[rowIndex].Cells[i].Style.BackColor = Color.LightYellow; // Tô màu vàng cho dễ thấy
                        }
                        else
                        {
                            // Nếu có danh sách NV mà chưa có quản lý thì hiện số lượng NV
                            dgvLichTuan.Rows[rowIndex].Cells[i].Value = "Đã có NV";
                            dgvLichTuan.Rows[rowIndex].Cells[i].Style.BackColor = Color.LightGreen;
                        }
                    }
                    else
                    {
                        dgvLichTuan.Rows[rowIndex].Cells[i].Value = "(Trống)";
                    }
                }
            }
        }

        // Sự kiện CellClick (Sửa lại đoạn gọi Form)
        private void dgvLichTuan_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex <= 0) return;

            DateTime ngayDuocChon = (DateTime)dgvLichTuan.Columns[e.ColumnIndex].Tag;
            string tenCa = dgvLichTuan.Rows[e.RowIndex].Cells[0].Value.ToString();

            // Lấy thêm tên chi nhánh đang chọn
            string chiNhanh = cboChiNhanh.Text;

            // Truyền thêm chi nhánh sang Form con
            frmChiTietCa frm = new frmChiTietCa(tenCa, ngayDuocChon, chiNhanh);

            frm.ShowDialog();
            LoadDuLieuLenLuoi();
        }

        private void HienThiNgayLenHeader()
        {
            // Tìm ngày Thứ 2 của tuần này
            _ngayDauTuan = LayNgayThu2(DateTime.Now);

            // Duyệt từ Cột 1 (Thứ 2) đến Cột 7 (Chủ Nhật)
            // Lưu ý: Cột 0 là "Ca Trực" nên mình bỏ qua, bắt đầu từ i = 1
            for (int i = 1; i <= 7; i++)
            {
                // Tính ngày: Cột 1 là +0 ngày, Cột 2 là +1 ngày...
                DateTime ngayCuaCot = _ngayDauTuan.AddDays(i - 1);

                // Lấy tên thứ cũ đang có trên Design (Ví dụ: "Thứ 2")
                string tenThu = dgvLichTuan.Columns[i].HeaderText;

                // Nếu tên thứ chưa có chữ ngày tháng thì mới cộng vào
                // Đề phòng load lại nhiều lần nó cộng dồn
                if (!tenThu.Contains("\n"))
                {
                    dgvLichTuan.Columns[i].HeaderText = tenThu + "\n" + ngayCuaCot.ToString("dd/MM");
                }

                // QUAN TRỌNG: Lưu ngày thật vào Tag của cột để tí nữa click lấy ra dùng
                dgvLichTuan.Columns[i].Tag = ngayCuaCot;
            }
        }

        private void TaoCacCaTrucMau()
        {
            // Thêm 3 dòng mẫu đại diện cho 3 ca
            dgvLichTuan.Rows.Add("Ca Sáng (7h-11h)");
            dgvLichTuan.Rows.Add("Ca Chiều (13h-17h)");
            dgvLichTuan.Rows.Add("Ca Tối (18h-22h)");
        }

        // Hàm tính ngày thứ 2 đầu tuần
        private DateTime LayNgayThu2(DateTime dt)
        {
            int diff = (7 + (dt.DayOfWeek - DayOfWeek.Monday)) % 7;
            return dt.AddDays(-1 * diff).Date;
        }

        private void dgvLichTuan_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // 1. Chặn lỗi: Nếu click vào tiêu đề hoặc click vào cột "Ca Trực" (Cột 0) thì không làm gì
            if (e.RowIndex < 0 || e.ColumnIndex <= 0) return;

            // 2. Lấy ngày từ cột vừa click (Đã lưu trong Tag ở bước trên)
            if (dgvLichTuan.Columns[e.ColumnIndex].Tag == null) return; // Kiểm tra an toàn

            DateTime ngayDuocChon = (DateTime)dgvLichTuan.Columns[e.ColumnIndex].Tag;
            string tenCa = dgvLichTuan.Rows[e.RowIndex].Cells[0].Value.ToString();

            // 3. Hiển thị Form chi tiết
            // (Lúc này bạn cần tạo Form chi tiết như mình hướng dẫn ở bài trước rồi nhé)
            // Ví dụ:
            // frmChiTietNgay frm = new frmChiTietNgay(ngayDuocChon);
            // frm.ShowDialog();

            // Tạm thời test bằng MessageBox xem chạy đúng ngày chưa:
            MessageBox.Show("Bạn đang chọn xem: " + tenCa + "\nNgày: " + ngayDuocChon.ToString("dd/MM/yyyy"));
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadDuLieuLenLuoi();
        }
    }
}
