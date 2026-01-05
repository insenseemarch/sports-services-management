using SportsServices.Dto;
using SportsServices.Forms; // Import form thêm nếu cần
using System;
using System.Linq;
using System.Windows.Forms;

namespace SportsServices.Forms
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            SetupColumns();
            LoadCoSo();
            // Mặc định load hết hoặc load theo cơ sở đầu tiên
            if (cboCoSo.Items.Count > 0)
            {
                cboCoSo.SelectedIndex = 0;
                LoadNhanVien(cboCoSo.SelectedValue.ToString());
            }
        }

        // Tạo cột cho GridView
        private void SetupColumns()
        {
            dgvNhanVien.AutoGenerateColumns = false;
            dgvNhanVien.Columns.Clear();

            // Add cột thủ công để kiểm soát giao diện
            dgvNhanVien.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "MaNV", HeaderText = "Mã NV" });
            dgvNhanVien.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "HoTen", HeaderText = "Họ Tên", Width = 200 });
            dgvNhanVien.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "GioiTinh", HeaderText = "Giới Tính" });
            dgvNhanVien.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "ChucVu", HeaderText = "Chức Vụ" });
            dgvNhanVien.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "SDT", HeaderText = "SĐT" });

            // Format lương có dấu phẩy
            var colLuong = new DataGridViewTextBoxColumn { DataPropertyName = "LuongCB", HeaderText = "Lương CB" };
            colLuong.DefaultCellStyle.Format = "N0";
            dgvNhanVien.Columns.Add(colLuong);

            // Nút Xóa
            var colDel = new DataGridViewButtonColumn { Name = "Xoa", HeaderText = "", Text = "Xóa", UseColumnTextForButtonValue = true };
            dgvNhanVien.Columns.Add(colDel);
        }

        private void LoadCoSo()
        {
            // Lấy từ kho dữ liệu chung
            cboCoSo.DataSource = FakeDatabase.CoSos;
            cboCoSo.DisplayMember = "TenCoSo";
            cboCoSo.ValueMember = "MaCoSo";
        }

        private void LoadNhanVien(string maCoSo)
        {
            // Lọc từ kho dữ liệu chung
            var list = FakeDatabase.NhanViens.Where(x => x.MaCoSo == maCoSo).ToList();
            dgvNhanVien.DataSource = list;
        }

        private void cboCoSo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboCoSo.SelectedValue is string maCoSo)
            {
                LoadNhanVien(maCoSo);
            }
        }

        // Xử lý nút Thêm
        private void btnThem_Click(object sender, EventArgs e)
        {
            // Code mở form thêm (như bạn đã làm)
            // Lưu ý: Khi lưu bên form kia, nhớ Add vào FakeDatabase.NhanViens
            // Sau đó gọi lại LoadNhanVien() ở đây là danh sách tự cập nhật
        }

        // Xử lý nút Xóa
        private void dgvNhanVien_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && dgvNhanVien.Columns[e.ColumnIndex].Name == "Xoa")
            {
                var item = dgvNhanVien.Rows[e.RowIndex].DataBoundItem as NhanVien;
                if (item != null)
                {
                    if (MessageBox.Show($"Xóa {item.HoTen}?", "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        // Xóa trong kho dữ liệu chung
                        FakeDatabase.NhanViens.Remove(item);
                        // Load lại lưới
                        LoadNhanVien(cboCoSo.SelectedValue.ToString());
                    }
                }
            }
        }
    }
}