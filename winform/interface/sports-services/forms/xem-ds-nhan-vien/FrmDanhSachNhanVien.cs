using DanhSachNhanVien.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace DanhSachNhanVien
{
    public partial class Form1 : Form
    {
        private List<CoSo> danhSachCoSo = new List<CoSo>();
        private List<NhanVien> danhSachNhanVien = new List<NhanVien>();

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            TaoDuLieuMau();
            SetupColumns();
            LoadCoSo();
        }

        // =============================
        // 1. TẠO CỘT CHUẨN CHO BẢNG
        // =============================
        private void SetupColumns()
        {
            dgvNhanVien.AutoGenerateColumns = false;
            dgvNhanVien.Columns.Clear();

            dgvNhanVien.Columns.Add("MaNV", "Mã NV");
            dgvNhanVien.Columns.Add("HoTen", "Họ tên");
            dgvNhanVien.Columns.Add("NgaySinh", "Ngày sinh");
            dgvNhanVien.Columns.Add("GioiTinh", "Giới tính");
            dgvNhanVien.Columns.Add("CCCD", "CCCD");
            dgvNhanVien.Columns.Add("SDT", "SĐT");
            dgvNhanVien.Columns.Add("DiaChi", "Địa chỉ");
            dgvNhanVien.Columns.Add("ChucVu", "Chức vụ");
            dgvNhanVien.Columns.Add("LuongCB", "Lương CB");

            // nút xoá
            var colDelete = new DataGridViewButtonColumn();
            colDelete.Name = "Xoa";
            colDelete.HeaderText = "";
            colDelete.Text = "X";
            colDelete.UseColumnTextForButtonValue = true;
            dgvNhanVien.Columns.Add(colDelete);
        }

        // =============================
        // 2. LOAD DATA MẪU
        // =============================
        private void TaoDuLieuMau()
        {
            danhSachCoSo = new List<CoSo>
            {
                new CoSo { MaCoSo = "CS1", TenCoSo = "Hà Nội" },
                new CoSo { MaCoSo = "CS2", TenCoSo = "TP.HCM" },
                new CoSo { MaCoSo = "CS3", TenCoSo = "Đà Nẵng" },
                new CoSo { MaCoSo = "CS4", TenCoSo = "Cần Thơ" }
            };

            danhSachNhanVien = new List<NhanVien>
            {
                new NhanVien { MaNV = "NV01", HoTen = "Nguyễn Văn A", NgaySinh = new DateTime(1999,1,1), GioiTinh="Nam", CCCD="012345", SDT="0901111", DiaChi="Long An", ChucVu="Nhân viên thu ngân", LuongCB=5000, MaCoSo = "CS2"},
                new NhanVien { MaNV = "NV02", HoTen = "Trần Thị B", NgaySinh = new DateTime(2000,5,10), GioiTinh="Nữ", CCCD="098765", SDT="0902222", DiaChi="Tiền Giang", ChucVu="Quản lý", LuongCB=8000, MaCoSo = "CS2"},
                new NhanVien { MaNV = "NV03", HoTen = "Lê Văn C", NgaySinh = new DateTime(1998,3,20), GioiTinh="Nam", CCCD="076543", SDT="0903333", DiaChi="Bắc Ninh", ChucVu="Nhân viên lễ tân", LuongCB=5500, MaCoSo = "CS1"},
                new NhanVien { MaNV = "NV04", HoTen = "Nguyễn Việt Cường", NgaySinh = new DateTime(2005,1,1), GioiTinh="Nam", CCCD="012345", SDT="0903333", DiaChi="Gia Lai", ChucVu="Nhân viên bảo trì", LuongCB=5500, MaCoSo = "CS3"},
                new NhanVien { MaNV = "NV05", HoTen = "Phạm Nguyễn Thế Khôi", NgaySinh = new DateTime(2005,1,1), GioiTinh="Nam", CCCD="0127245", SDT="0903333", DiaChi="Phú Yên", ChucVu="Nhân viên bảo trì", LuongCB=4000, MaCoSo = "CS3"},
                new NhanVien { MaNV = "NV06", HoTen = "Lê Hải Sơn", NgaySinh = new DateTime(2005,1,1), GioiTinh="Nam", CCCD="012345", SDT="0903333", DiaChi="Bình Phước", ChucVu="Nhân viên lễ tân", LuongCB=5000, MaCoSo = "CS2"},
                new NhanVien { MaNV = "NV07", HoTen = "Hồ Khổng Tuyết Như", NgaySinh = new DateTime(2005,1,1), GioiTinh="Nữ", CCCD="012345", SDT="0903333", DiaChi="Bến Tre", ChucVu="Nhân viên lễ tân", LuongCB=5000, MaCoSo = "CS4"},
                new NhanVien { MaNV = "NV08", HoTen = "Trần Thị Thủy Tiên", NgaySinh = new DateTime(2005,1,1), GioiTinh="Nữ", CCCD="012345", SDT="0903333", DiaChi="Long An", ChucVu="Nhân viên thu ngân", LuongCB=5000, MaCoSo = "CS4"}
            };
        }

        // =============================
        // 3. LOAD CƠ SỞ (COMBOBOX)
        // =============================
        private void LoadCoSo()
        {
            cboCoSo.DataSource = danhSachCoSo;
            cboCoSo.DisplayMember = "TenCoSo";
            cboCoSo.ValueMember = "MaCoSo";
            cboCoSo.SelectedIndex = 0;
        }

        // =============================
        // 4. LOAD NHÂN VIÊN THEO CƠ SỞ
        // =============================
        private void LoadNhanVienTheoCoSo(string maCoSo)
        {
            dgvNhanVien.Rows.Clear();

            var list = danhSachNhanVien
                .Where(nv => nv.MaCoSo == maCoSo)
                .ToList();

            foreach (var nv in list)
            {
                dgvNhanVien.Rows.Add(
                    nv.MaNV,
                    nv.HoTen,
                    nv.NgaySinh.ToString("dd/MM/yyyy"),
                    nv.GioiTinh,
                    nv.CCCD,
                    nv.SDT,
                    nv.DiaChi,
                    nv.ChucVu,
                    nv.LuongCB
                );
            }
        }

        // =============================
        // 5. SỰ KIỆN CHỌN CƠ SỞ
        // =============================
        private void cboCoSo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboCoSo.SelectedValue == null) return;

            string maCoSo = cboCoSo.SelectedValue.ToString();
            LoadNhanVienTheoCoSo(maCoSo);
        }

        // =============================
        // 6. XOÁ THEO DÒNG
        // =============================
        private void dgvNhanVien_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            if (dgvNhanVien.Columns[e.ColumnIndex].Name == "Xoa")
            {
                string maNV = dgvNhanVien.Rows[e.RowIndex].Cells["MaNV"].Value.ToString();

                var confirm = MessageBox.Show($"Xóa nhân viên {maNV} ?",
                                              "Xác nhận", MessageBoxButtons.YesNo);

                if (confirm == DialogResult.Yes)
                {
                    var nv = danhSachNhanVien.FirstOrDefault(x => x.MaNV == maNV);
                    if (nv != null)
                        danhSachNhanVien.Remove(nv);

                    LoadNhanVienTheoCoSo(cboCoSo.SelectedValue.ToString());
                }
            }
        }

        // =============================
        // 7. MỞ FORM THÊM
        // =============================
        private void btnThem_Click(object sender, EventArgs e)
        {
            using (var f = new ThemNhanVien())
            {
                f.LoadChiNhanh(danhSachCoSo);
                var result = f.ShowDialog();

                if (result == DialogResult.OK)
                {
                    var nv = f.NhanVienMoi;
                    nv.MaCoSo = cboCoSo.SelectedValue.ToString();

                    danhSachNhanVien.Add(nv);
                    LoadNhanVienTheoCoSo(nv.MaCoSo);
                }
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {
        }

    }
}
