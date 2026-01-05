using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TTTT
{
    public partial class FormDatDichVu : Form
    {
        public FormDatDichVu()
        {
            InitializeComponent();

            // Gắn event (nếu trong Designer đã gắn rồi thì cũng không sao)
            this.Load += FormDatDichVu_Load;
            dgdDichVu.CellValueChanged += dgdDichVu_CellValueChanged;
            dgdDichVu.CurrentCellDirtyStateChanged += dgdDichVu_CurrentCellDirtyStateChanged;
        }

        // =============== LOAD FAKE DATA ===============
        private void FormDatDichVu_Load(object sender, EventArgs e)
        {
            dgdDichVu.AutoGenerateColumns = false;
            dgdDichVu.RowHeadersVisible = false;   // ẩn cột mũi tên
            dgdDichVu.Rows.Clear();

            var dsFake = new[]
            {
                new { Ma = 1, Ten = "Thuê bóng",        DonGia = 20000 },
                new { Ma = 2, Ten = "Nước suối (chai)", DonGia = 10000 },
                new { Ma = 3, Ten = "Khăn lạnh",        DonGia = 5000  },
                new { Ma = 4, Ten = "Áo bib",           DonGia = 30000 }
            };

            foreach (var dv in dsFake)
            {
                // Giả định: các cột lần lượt là
                // 0: Chọn, 1: MãDV, 2: TênDV, 3: Đơn giá, 4: Số lượng
                dgdDichVu.Rows.Add(false, dv.Ma, dv.Ten, dv.DonGia, 0);
            }

            CapNhatTongTien();
        }

        // =============== TÍNH TỔNG TIỀN ===============
        private void CapNhatTongTien()
        {
            decimal tong = 0;

            foreach (DataGridViewRow row in dgdDichVu.Rows)
            {
                if (row.IsNewRow) continue;

                // cột 0: checkbox "Chọn"
                bool chon = false;
                if (row.Cells[0].Value is bool b)
                    chon = b;
                if (!chon) continue;

                // cột 3: Đơn giá
                decimal donGia = 0;
                decimal.TryParse(Convert.ToString(row.Cells[3].Value), out donGia);

                // cột 4: Số lượng
                int soLuong = 0;
                int.TryParse(Convert.ToString(row.Cells[4].Value), out soLuong);

                tong += donGia * soLuong;
            }

            lblTongTien.Text = $"Tổng tiền dịch vụ: {tong:N0}đ";
        }

        // commit ngay khi click checkbox / thay đổi ô để CellValueChanged chạy
        private void dgdDichVu_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dgdDichVu.IsCurrentCellDirty)
                dgdDichVu.CommitEdit(DataGridViewDataErrorContexts.Commit);
        }

        private void dgdDichVu_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            CapNhatTongTien();
        }

        // nếu Designer còn gắn CellContentClick thì giữ hàm rỗng này
        private void dgdDichVu_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
        }
    }
}
