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
    public partial class FrmThayDoiHeThong : Form
    {
        public FrmThayDoiHeThong()
        {
            InitializeComponent(); this.WindowState = FormWindowState.Maximized;
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            cboLoaiSan.Items.Add("Sân bóng đá mini");
            cboLoaiSan.Items.Add("Sân cầu lông");
            cboLoaiSan.Items.Add("Sân tennis");
            cboLoaiSan.Items.Add("Sân bóng rổ");
            cboLoaiSan.Items.Add("Sân futsal");
            cboLoaiSan.DropDownStyle = ComboBoxStyle.DropDownList;
            cboLoaiSan.SelectedIndex = 0;

            cboCoSo.Items.Add("Hà Nội");
            cboCoSo.Items.Add("Đà Nẵng");
            cboCoSo.Items.Add("TP.HCM");
            cboCoSo.Items.Add("Cần Thơ");
            cboCoSo.DropDownStyle = ComboBoxStyle.DropDownList;
            cboCoSo.SelectedIndex = 0;

            dtpMoCua.Format = DateTimePickerFormat.Time;
            dtpMoCua.ShowUpDown = true;

            dtpDongCua.Format = DateTimePickerFormat.Time;
            dtpDongCua.ShowUpDown = true;
        }

        private void btnLuuTatCa_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Đã lưu tất cả thay đổi!", "Thông báo");
        }

        private void btnKhoiPhuc_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Đã khôi phục mặc định!", "Thông báo");
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
