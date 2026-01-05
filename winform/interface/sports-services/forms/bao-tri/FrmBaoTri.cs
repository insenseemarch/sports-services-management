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
    public partial class QuanLiBaoTri : Form
    {
        public QuanLiBaoTri()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void QuanLiBaoTri_Load(object sender, EventArgs e)
        {
            // Thêm các lựa chọn vào ComboBox Trạng thái mới
            cboTrangThaiSan.Items.Clear();
            cboTrangThaiSan.Items.Add("Trống");
            cboTrangThaiSan.Items.Add("Đang bảo trì");
            cboTrangThaiSan.SelectedIndex = 0; // Chọn mặc định cái đầu tiên

            // Thêm dữ liệu mẫu vào bảng (Giả lập dữ liệu từ Database)
            // Lưu ý: Các cột này phải khớp với thứ tự bạn tạo trong Edit Columns
            dgvBaoCao.Rows.Add("1", "Cơ sở A", "S001", "NV_MT_01", "2025-11-21 09:00",
                       "2025-11-23", "2025-11-25", "Bảo trì định kỳ",
                       "Đèn sân hư", "1,500,000", ""); //
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void txtMaSanCapNhat_Click(object sender, EventArgs e)
        {
            string maSan = txtMaSanCapNhat.Text.Trim();
            string trangThaiMoi = cboTrangThaiSan.SelectedItem?.ToString();

            // Kiểm tra dữ liệu đầu vào
            if (string.IsNullOrEmpty(maSan))
            {
                MessageBox.Show("Vui lòng chọn mã sân từ bảng hoặc nhập vào!", "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // --- PHẦN XỬ LÝ UPDATE ---

            // CÁCH 1: Cập nhật giả lập (Chỉ hiển thị trên bảng để test giao diện)
            bool timThay = false;
            foreach (DataGridViewRow row in dgvBaoCao.Rows)
            {
                // Kiểm tra cột Mã Sân (giả sử cột index 2)
                if (row.Cells[2].Value?.ToString() == maSan)
                {
                    // Update cột trạng thái phiếu (giả sử cột cuối cùng index 10)
                    // Logic của bạn: Update Sân, nhưng ở đây mình hiển thị tạm lên bảng cho bạn thấy đổi màu
                    MessageBox.Show($"Đã cập nhật sân {maSan} sang trạng thái: {trangThaiMoi}", "Thành công");
                    timThay = true;
                    break;
                }
            }

            if (!timThay)
            {
                // Nếu không thấy trên bảng thì vẫn báo thành công (giả lập update xuống DB)
                MessageBox.Show($"Đã gửi lệnh cập nhật sân {maSan} thành: {trangThaiMoi}", "Thành công");
            }
        }

        private void txtTrangThaiMoi_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void cboTrangThaiSan_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        private void dgvBaoCao_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            // Code này giúp chương trình không bị crash hay hiện bảng lỗi
            // khi dữ liệu ComboBox bị sai lệch.
            e.Cancel = true;
        }

        private void btnLuuTrangThai_Click(object sender, EventArgs e)
        {

        }
    }
}
