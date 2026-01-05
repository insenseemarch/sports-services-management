using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SportsServices.Forms;

namespace SportsServices
{
    public partial class Form1 : Form
    {
        private void btnOpenDangKy_Click(object sender, EventArgs e)
        {
            FrmDangKy frm = new FrmDangKy(); // Bây giờ mới new được
            frm.Show();
        }
    }
}
