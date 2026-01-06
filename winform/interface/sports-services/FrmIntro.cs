using SportsServices.Dto;
using SportsServices.Forms; // Để gọi FrmDangNhap
using System;
using System.Drawing;
using System.IO;
using System.Reflection.Emit;
using System.Security.Cryptography;
using System.Windows.Forms;
using System.Linq;



namespace SportsServices.Forms
{
    public partial class FrmIntro : Form
    {
        public FrmIntro()
        {
            InitializeComponent();
            if (!this.DesignMode)
            {
                // Tự động căn giữa nội dung khi mở lên
                this.WindowState = FormWindowState.Maximized;
                this.AutoScroll = true;
            }
        }

        // Sự kiện khi bấm nút Đăng nhập ở góc phải
        private void btnLoginHeader_Click(object sender, EventArgs e)
        {
            OpenLogin();
        }

        private void FrmIntro_Load(object sender, EventArgs e)
        {
            string duongDanAnh = @"Bioscapes-Blog-header-1-1080x400.jpg";
        }


        /// <summary>
        /// Hàm vẽ lớp phủ màu đen lên ảnh
        /// </summary>
        private Image TaoLopPhuMo(Image img, int alpha)
        {
            Bitmap bmp = new Bitmap(img.Width, img.Height);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.DrawImage(img, 0, 0, img.Width, img.Height);
                using (Brush brush = new SolidBrush(Color.FromArgb(alpha, 0, 0, 0)))
                {
                    g.FillRectangle(brush, 0, 0, img.Width, img.Height);
                }
            }
            return bmp;
        }


        // Sự kiện khi bấm nút "TÌM SÂN NGAY" ở giữa
        private void btnCta_Click(object sender, EventArgs e)
        {
            OpenLogin();
        }

        private void OpenLogin()
        {
            this.Hide(); // Ẩn trang Intro
            FrmDangNhap frm = new FrmDangNhap();
            frm.ShowDialog(); // Chờ đăng nhập
            this.Show(); // Đóng hẳn ứng dụng sau khi form đăng nhập đóng (nếu user tắt form đăng nhập)
        }
        private void lblLogo_Click(object sender, EventArgs e)
        {

        }

        private void picBanner_Click(object sender, EventArgs e)
        {

        }

        private void dgvKetQua_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void pnlHero_Paint(object sender, PaintEventArgs e)
        {

        }

        private void cboChiNhanh_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {

        }

        private void cboLoaiSan_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void dtpGioDen_ValueChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click_1(object sender, EventArgs e)
        {

        }

        private void pnlLoaiSan_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            pnlHero.AutoScrollPosition = new Point(0, 0);

            // Cuộn tới vị trí của pnlDichVu
            pnlHero.ScrollControlIntoView(pnlDichVu);
        }

        private void pnlCacLoaiSan_Click(object sender, EventArgs e)
        {
            pnlHero.AutoScrollPosition = new Point(0, 0);

            // Cuộn tới vị trí của pnlDichVu
            pnlHero.ScrollControlIntoView(pnlLoaiSan);
        }


        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide(); // Ẩn trang Intro
            FormDangKy frm = new FormDangKy();
            frm.ShowDialog(); // Chờ đăng ký
            this.Show();
        }
    }
}