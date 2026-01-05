using System;
using System.Drawing;
using System.Windows.Forms;
using SportsServices.Forms; // Để gọi FrmDangNhap

namespace SportsServices.Forms
{
    public partial class FrmIntro : Form
    {
        public FrmIntro()
        {
            InitializeComponent();
            // Tự động căn giữa nội dung khi mở lên
            CenterContent();
        }

        // Sự kiện khi bấm nút Đăng nhập ở góc phải
        private void btnLoginHeader_Click(object sender, EventArgs e)
        {
            OpenLogin();
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
            this.Close(); // Đóng hẳn ứng dụng sau khi form đăng nhập đóng (nếu user tắt form đăng nhập)
        }

        // Hàm giúp nội dung luôn nằm giữa màn hình dù màn hình to hay nhỏ
        private void CenterContent()
        {
            if (pnlHero != null && pnlContentCenter != null)
            {
                pnlContentCenter.Left = (pnlHero.Width - pnlContentCenter.Width) / 2;
                pnlContentCenter.Top = (pnlHero.Height - pnlContentCenter.Height) / 2;
            }
        }

        // Khi thay đổi kích thước cửa sổ, tự động căn giữa lại
        private void pnlHero_SizeChanged(object sender, EventArgs e)
        {
            CenterContent();
        }
    }
}