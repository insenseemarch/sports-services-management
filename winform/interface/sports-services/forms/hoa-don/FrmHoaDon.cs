using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HoaDon
{
    public partial class Form1 : Form
    {
        decimal giaThueSan = 0;
        decimal phanTramGiam = 0;
        decimal thanhTienDV = 0;
        public Form1()
        {
            InitializeComponent();
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            cboLoaiSanDat.Items.Add("Sân bóng đá mini");
            cboLoaiSanDat.Items.Add("Sân cầu lông");
            cboLoaiSanDat.Items.Add("Sân tennis");
            cboLoaiSanDat.Items.Add("Sân bóng rổ");
            cboLoaiSanDat.Items.Add("Sân futsal");

            cboLoaiSanDat.DropDownStyle = ComboBoxStyle.DropDownList;

            dataGridView1.Rows.Add("Huấn luyện viên", 1, 500000, 1000000);
            dataGridView1.Rows.Add("Vợt cầu lông", 2, 100000, 200000);

            cboUuDai.Items.Add("Không có");
            cboUuDai.Items.Add("HAPPY HOUR - Giảm 15%");
            cboUuDai.Items.Add("PNVN - Giảm 20%");
            cboUuDai.Items.Add("SILVER MEMBER - Giảm 5%");
            cboUuDai.Items.Add("GOLD MEMBER - Giảm 10%");
            cboUuDai.Items.Add("PLATINUM MEMBER - Giảm 20%");

            cboUuDai.DropDownStyle = ComboBoxStyle.DropDownList;
            cboUuDai.SelectedIndex = 0;

            cboThanhToan.Items.Add("Tiền mặt");
            cboThanhToan.Items.Add("Chuyển khoản");
            cboThanhToan.DropDownStyle = ComboBoxStyle.DropDownList;
            cboThanhToan.SelectedIndex = 0;

            TinhTienDichVu();
            TinhThanhTien();

        }

        private void cboLoaiSanDat_SelectedIndexChanged(object sender, EventArgs e)
        {
            cboMaSanDat.Items.Clear();

            switch (cboLoaiSanDat.SelectedItem.ToString())
            {
                case "Sân bóng đá mini":
                    cboMaSanDat.Items.Add("BD01");
                    cboMaSanDat.Items.Add("BD02");
                    cboMaSanDat.Items.Add("BD03");
                    break;

                case "Sân cầu lông":
                    cboMaSanDat.Items.Add("CL01");
                    cboMaSanDat.Items.Add("CL02");
                    cboMaSanDat.Items.Add("CL03");
                    break;

                case "Sân tennis":
                    cboMaSanDat.Items.Add("TN01");
                    cboMaSanDat.Items.Add("TN02");
                    break;

                case "Sân bóng rổ":
                    cboMaSanDat.Items.Add("BR01");
                    cboMaSanDat.Items.Add("BR02");
                    break;

                case "Sân futsal":
                    cboMaSanDat.Items.Add("FS01");
                    cboMaSanDat.Items.Add("FS02");
                    break;
            }

            if (cboMaSanDat.Items.Count > 0)
                cboMaSanDat.SelectedIndex = 0;
        }

        private void cboMaSanDat_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            TinhTienDichVu();
        }

        private void TinhTienDichVu()
        { 
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.Cells[3].Value != null)
                {
                    decimal val = decimal.Parse(
                        row.Cells[3].Value.ToString(),
                        System.Globalization.NumberStyles.AllowThousands
                    );
                    thanhTienDV += val;
                }

            }
            TinhThanhTien();
        }
        private void cboUuDai_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (cboUuDai.SelectedItem.ToString())
            {
                case "Không có": phanTramGiam = 0; break;
                case "HAPPY HOUR - Giảm 15%": phanTramGiam = 0.15m; break;
                case "PNVN - Giảm 20%": phanTramGiam = 0.20m; break;
                case "SILVER MEMBER - Giảm 5%": phanTramGiam = 0.05m; break;
                case "GOLD MEMBER - Giảm 10%": phanTramGiam = 0.10m; break;
                case "PLATINUM MEMBER - Giảm 20%": phanTramGiam = 0.20m; break;
            }

            TinhThanhTien();
        }
        private void txtGiaThueSan_TextChanged(object sender, EventArgs e)
        {
            decimal giaThueSan = decimal.Parse(txtGiaThueSan.Text, System.Globalization.NumberStyles.AllowThousands);
            TinhThanhTien();
        }
        private void TinhThanhTien()
        {
            decimal tongTien = giaThueSan + thanhTienDV;

            decimal giam = 0;

            if (cboUuDai.SelectedItem.ToString().Contains("15%"))
                giam = 0.15m;
            else if (cboUuDai.SelectedItem.ToString().Contains("20%"))
                giam = 0.20m;
            else if (cboUuDai.SelectedItem.ToString().Contains("10%"))
                giam = 0.10m;
            else if (cboUuDai.SelectedItem.ToString().Contains("5%"))
                giam = 0.05m;

            decimal thanhToan = tongTien * (1 - giam);

            txtThanhTienThanhToan.Text = thanhToan.ToString("#,###");

        }
    }
}
