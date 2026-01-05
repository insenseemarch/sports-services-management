using System;
using System.Windows.Forms;
using SportsServices.Dto; // Nhớ dòng này

namespace SportsServices
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // ==> NẠP DỮ LIỆU GIẢ VÀO BỘ NHỚ
            FakeDatabase.InitData();

            // Chạy form Intro hoặc Login
            Application.Run(new Forms.FrmIntro());
        }
    }
}