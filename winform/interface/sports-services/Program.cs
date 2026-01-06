using System;
using System.Windows.Forms;
using SportsServices.Dto;

namespace SportsServices
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            FakeDatabase.InitData();

            // 1. Chạy Intro trước
            Forms.FrmIntro intro = new Forms.FrmIntro();
            // Nếu Intro chạy xong và trả về OK (hoặc chỉ cần ShowDialog xong)
            intro.ShowDialog();

            // 2. Sau khi Intro tắt thì mới chạy Form Đăng Nhập là Form chính
            Application.Run(new Forms.FrmDangNhap());
        }
    }
}