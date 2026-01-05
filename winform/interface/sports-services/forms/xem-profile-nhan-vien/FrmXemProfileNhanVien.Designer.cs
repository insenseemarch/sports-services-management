namespace ThongTinCaNhan
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lbLuongCoBan = new System.Windows.Forms.Label();
            this.txtLuongCoBan = new System.Windows.Forms.TextBox();
            this.gbThongTin = new System.Windows.Forms.GroupBox();
            this.cboVaiTro = new System.Windows.Forms.ComboBox();
            this.btnShowHide = new System.Windows.Forms.CheckBox();
            this.btnDoiMatKhau = new System.Windows.Forms.Button();
            this.lbChucVu = new System.Windows.Forms.Label();
            this.dtpNgayDangKy = new System.Windows.Forms.DateTimePicker();
            this.lbNgayDK = new System.Windows.Forms.Label();
            this.txtPass = new System.Windows.Forms.TextBox();
            this.lbMK = new System.Windows.Forms.Label();
            this.lbTenDN = new System.Windows.Forms.Label();
            this.txtTenDangNhap = new System.Windows.Forms.TextBox();
            this.gbThongTinCaNhan = new System.Windows.Forms.GroupBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.dausao1 = new System.Windows.Forms.Label();
            this.btnThoat = new System.Windows.Forms.Button();
            this.btnLuu = new System.Windows.Forms.Button();
            this.txtDiaChi = new System.Windows.Forms.TextBox();
            this.lbDiaChi = new System.Windows.Forms.Label();
            this.txtEmail = new System.Windows.Forms.TextBox();
            this.lbEmail = new System.Windows.Forms.Label();
            this.txtSDT = new System.Windows.Forms.TextBox();
            this.lbSDT = new System.Windows.Forms.Label();
            this.txtCCCD = new System.Windows.Forms.TextBox();
            this.lbCCCD = new System.Windows.Forms.Label();
            this.dtpNgaySinh = new System.Windows.Forms.DateTimePicker();
            this.lbNamSinh = new System.Windows.Forms.Label();
            this.txtHoTen = new System.Windows.Forms.TextBox();
            this.lbHoTen = new System.Windows.Forms.Label();
            this.gbThongTin.SuspendLayout();
            this.gbThongTinCaNhan.SuspendLayout();
            this.SuspendLayout();
            // 
            // lbLuongCoBan
            // 
            this.lbLuongCoBan.AutoSize = true;
            this.lbLuongCoBan.Location = new System.Drawing.Point(691, 62);
            this.lbLuongCoBan.Name = "lbLuongCoBan";
            this.lbLuongCoBan.Size = new System.Drawing.Size(202, 24);
            this.lbLuongCoBan.TabIndex = 0;
            this.lbLuongCoBan.Text = "Lương Cơ Bản (VND): ";
            this.lbLuongCoBan.Click += new System.EventHandler(this.label1_Click);
            // 
            // txtLuongCoBan
            // 
            this.txtLuongCoBan.Font = new System.Drawing.Font("Bahnschrift", 12F);
            this.txtLuongCoBan.Location = new System.Drawing.Point(899, 62);
            this.txtLuongCoBan.Name = "txtLuongCoBan";
            this.txtLuongCoBan.ReadOnly = true;
            this.txtLuongCoBan.Size = new System.Drawing.Size(281, 32);
            this.txtLuongCoBan.TabIndex = 1;
            this.txtLuongCoBan.Text = "7.000.000";
            this.txtLuongCoBan.TextChanged += new System.EventHandler(this.txtLuongCoBan_TextChanged);
            // 
            // gbThongTin
            // 
            this.gbThongTin.Controls.Add(this.cboVaiTro);
            this.gbThongTin.Controls.Add(this.btnShowHide);
            this.gbThongTin.Controls.Add(this.btnDoiMatKhau);
            this.gbThongTin.Controls.Add(this.lbChucVu);
            this.gbThongTin.Controls.Add(this.txtLuongCoBan);
            this.gbThongTin.Controls.Add(this.lbLuongCoBan);
            this.gbThongTin.Controls.Add(this.dtpNgayDangKy);
            this.gbThongTin.Controls.Add(this.lbNgayDK);
            this.gbThongTin.Controls.Add(this.txtPass);
            this.gbThongTin.Controls.Add(this.lbMK);
            this.gbThongTin.Controls.Add(this.lbTenDN);
            this.gbThongTin.Controls.Add(this.txtTenDangNhap);
            this.gbThongTin.Font = new System.Drawing.Font("Bahnschrift", 12F, System.Drawing.FontStyle.Bold);
            this.gbThongTin.Location = new System.Drawing.Point(12, 32);
            this.gbThongTin.Name = "gbThongTin";
            this.gbThongTin.Size = new System.Drawing.Size(1228, 286);
            this.gbThongTin.TabIndex = 2;
            this.gbThongTin.TabStop = false;
            this.gbThongTin.Text = "Thông Tin Nhân Viên";
            // 
            // cboVaiTro
            // 
            this.cboVaiTro.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboVaiTro.Font = new System.Drawing.Font("Bahnschrift", 12F);
            this.cboVaiTro.FormattingEnabled = true;
            this.cboVaiTro.Items.AddRange(new object[] {
            "Quản Lí",
            "",
            "Kỹ Thuật",
            "",
            "Thu Ngân",
            "",
            "Lễ Tân"});
            this.cboVaiTro.Location = new System.Drawing.Point(899, 210);
            this.cboVaiTro.Name = "cboVaiTro";
            this.cboVaiTro.Size = new System.Drawing.Size(281, 32);
            this.cboVaiTro.TabIndex = 10;
            // 
            // btnShowHide
            // 
            this.btnShowHide.AutoSize = true;
            this.btnShowHide.Location = new System.Drawing.Point(493, 142);
            this.btnShowHide.Name = "btnShowHide";
            this.btnShowHide.Size = new System.Drawing.Size(73, 28);
            this.btnShowHide.TabIndex = 9;
            this.btnShowHide.Text = "Hiện";
            this.btnShowHide.UseVisualStyleBackColor = true;
            this.btnShowHide.CheckedChanged += new System.EventHandler(this.btnShowHide_CheckedChanged);
            // 
            // btnDoiMatKhau
            // 
            this.btnDoiMatKhau.Location = new System.Drawing.Point(572, 136);
            this.btnDoiMatKhau.Name = "btnDoiMatKhau";
            this.btnDoiMatKhau.Size = new System.Drawing.Size(103, 31);
            this.btnDoiMatKhau.TabIndex = 8;
            this.btnDoiMatKhau.Text = "Thay đổi";
            this.btnDoiMatKhau.UseVisualStyleBackColor = true;
            this.btnDoiMatKhau.Click += new System.EventHandler(this.btnDoiMatKhau_Click);
            // 
            // lbChucVu
            // 
            this.lbChucVu.AutoSize = true;
            this.lbChucVu.Location = new System.Drawing.Point(691, 213);
            this.lbChucVu.Name = "lbChucVu";
            this.lbChucVu.Size = new System.Drawing.Size(88, 24);
            this.lbChucVu.TabIndex = 6;
            this.lbChucVu.Text = "Chức Vụ:";
            // 
            // dtpNgayDangKy
            // 
            this.dtpNgayDangKy.Font = new System.Drawing.Font("Bahnschrift", 12F);
            this.dtpNgayDangKy.Location = new System.Drawing.Point(205, 210);
            this.dtpNgayDangKy.Name = "dtpNgayDangKy";
            this.dtpNgayDangKy.Size = new System.Drawing.Size(316, 32);
            this.dtpNgayDangKy.TabIndex = 5;
            // 
            // lbNgayDK
            // 
            this.lbNgayDK.AutoSize = true;
            this.lbNgayDK.Location = new System.Drawing.Point(12, 213);
            this.lbNgayDK.Name = "lbNgayDK";
            this.lbNgayDK.Size = new System.Drawing.Size(136, 24);
            this.lbNgayDK.TabIndex = 4;
            this.lbNgayDK.Text = "Ngày Đăng Kí:";
            // 
            // txtPass
            // 
            this.txtPass.Font = new System.Drawing.Font("Bahnschrift", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtPass.Location = new System.Drawing.Point(205, 136);
            this.txtPass.Name = "txtPass";
            this.txtPass.PasswordChar = '*';
            this.txtPass.ReadOnly = true;
            this.txtPass.Size = new System.Drawing.Size(255, 32);
            this.txtPass.TabIndex = 3;
            // 
            // lbMK
            // 
            this.lbMK.AutoSize = true;
            this.lbMK.Location = new System.Drawing.Point(12, 142);
            this.lbMK.Name = "lbMK";
            this.lbMK.Size = new System.Drawing.Size(100, 24);
            this.lbMK.TabIndex = 2;
            this.lbMK.Text = "Mật Khẩu:";
            // 
            // lbTenDN
            // 
            this.lbTenDN.AutoSize = true;
            this.lbTenDN.Location = new System.Drawing.Point(12, 65);
            this.lbTenDN.Name = "lbTenDN";
            this.lbTenDN.Size = new System.Drawing.Size(150, 24);
            this.lbTenDN.TabIndex = 0;
            this.lbTenDN.Text = "Tên Đăng Nhập:";
            // 
            // txtTenDangNhap
            // 
            this.txtTenDangNhap.Font = new System.Drawing.Font("Bahnschrift", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtTenDangNhap.Location = new System.Drawing.Point(205, 62);
            this.txtTenDangNhap.Name = "txtTenDangNhap";
            this.txtTenDangNhap.ReadOnly = true;
            this.txtTenDangNhap.Size = new System.Drawing.Size(401, 32);
            this.txtTenDangNhap.TabIndex = 1;
            // 
            // gbThongTinCaNhan
            // 
            this.gbThongTinCaNhan.Controls.Add(this.label6);
            this.gbThongTinCaNhan.Controls.Add(this.label5);
            this.gbThongTinCaNhan.Controls.Add(this.label3);
            this.gbThongTinCaNhan.Controls.Add(this.label2);
            this.gbThongTinCaNhan.Controls.Add(this.dausao1);
            this.gbThongTinCaNhan.Controls.Add(this.btnThoat);
            this.gbThongTinCaNhan.Controls.Add(this.btnLuu);
            this.gbThongTinCaNhan.Controls.Add(this.txtDiaChi);
            this.gbThongTinCaNhan.Controls.Add(this.lbDiaChi);
            this.gbThongTinCaNhan.Controls.Add(this.txtEmail);
            this.gbThongTinCaNhan.Controls.Add(this.lbEmail);
            this.gbThongTinCaNhan.Controls.Add(this.txtSDT);
            this.gbThongTinCaNhan.Controls.Add(this.lbSDT);
            this.gbThongTinCaNhan.Controls.Add(this.txtCCCD);
            this.gbThongTinCaNhan.Controls.Add(this.lbCCCD);
            this.gbThongTinCaNhan.Controls.Add(this.dtpNgaySinh);
            this.gbThongTinCaNhan.Controls.Add(this.lbNamSinh);
            this.gbThongTinCaNhan.Controls.Add(this.txtHoTen);
            this.gbThongTinCaNhan.Controls.Add(this.lbHoTen);
            this.gbThongTinCaNhan.Font = new System.Drawing.Font("Bahnschrift", 12F, System.Drawing.FontStyle.Bold);
            this.gbThongTinCaNhan.Location = new System.Drawing.Point(12, 338);
            this.gbThongTinCaNhan.Name = "gbThongTinCaNhan";
            this.gbThongTinCaNhan.Size = new System.Drawing.Size(1240, 344);
            this.gbThongTinCaNhan.TabIndex = 4;
            this.gbThongTinCaNhan.TabStop = false;
            this.gbThongTinCaNhan.Text = "Thông tin Cá Nhân";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.ForeColor = System.Drawing.Color.Crimson;
            this.label6.Location = new System.Drawing.Point(809, 62);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(19, 24);
            this.label6.TabIndex = 27;
            this.label6.Text = "*";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.ForeColor = System.Drawing.Color.Crimson;
            this.label5.Location = new System.Drawing.Point(738, 142);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(19, 24);
            this.label5.TabIndex = 26;
            this.label5.Text = "*";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.ForeColor = System.Drawing.Color.Crimson;
            this.label3.Location = new System.Drawing.Point(153, 234);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(19, 24);
            this.label3.TabIndex = 24;
            this.label3.Text = "*";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.Color.Crimson;
            this.label2.Location = new System.Drawing.Point(96, 147);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(19, 24);
            this.label2.TabIndex = 23;
            this.label2.Text = "*";
            this.label2.Click += new System.EventHandler(this.label2_Click);
            // 
            // dausao1
            // 
            this.dausao1.AutoSize = true;
            this.dausao1.ForeColor = System.Drawing.Color.Crimson;
            this.dausao1.Location = new System.Drawing.Point(96, 64);
            this.dausao1.Name = "dausao1";
            this.dausao1.Size = new System.Drawing.Size(19, 24);
            this.dausao1.TabIndex = 21;
            this.dausao1.Text = "*";
            // 
            // btnThoat
            // 
            this.btnThoat.Location = new System.Drawing.Point(523, 289);
            this.btnThoat.Name = "btnThoat";
            this.btnThoat.Size = new System.Drawing.Size(101, 34);
            this.btnThoat.TabIndex = 20;
            this.btnThoat.Text = "Thoát";
            this.btnThoat.UseVisualStyleBackColor = true;
            this.btnThoat.Click += new System.EventHandler(this.btnThoat_Click);
            // 
            // btnLuu
            // 
            this.btnLuu.Location = new System.Drawing.Point(652, 289);
            this.btnLuu.Name = "btnLuu";
            this.btnLuu.Size = new System.Drawing.Size(127, 34);
            this.btnLuu.TabIndex = 19;
            this.btnLuu.Text = "Cập Nhật";
            this.btnLuu.UseVisualStyleBackColor = true;
            this.btnLuu.Click += new System.EventHandler(this.btnLuu_Click);
            // 
            // txtDiaChi
            // 
            this.txtDiaChi.Font = new System.Drawing.Font("Bahnschrift", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtDiaChi.Location = new System.Drawing.Point(894, 231);
            this.txtDiaChi.Name = "txtDiaChi";
            this.txtDiaChi.Size = new System.Drawing.Size(286, 32);
            this.txtDiaChi.TabIndex = 14;
            // 
            // lbDiaChi
            // 
            this.lbDiaChi.AutoSize = true;
            this.lbDiaChi.Location = new System.Drawing.Point(691, 234);
            this.lbDiaChi.Name = "lbDiaChi";
            this.lbDiaChi.Size = new System.Drawing.Size(78, 24);
            this.lbDiaChi.TabIndex = 13;
            this.lbDiaChi.Text = "Địa Chỉ:";
            // 
            // txtEmail
            // 
            this.txtEmail.Font = new System.Drawing.Font("Bahnschrift", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtEmail.Location = new System.Drawing.Point(894, 139);
            this.txtEmail.Name = "txtEmail";
            this.txtEmail.Size = new System.Drawing.Size(286, 32);
            this.txtEmail.TabIndex = 12;
            // 
            // lbEmail
            // 
            this.lbEmail.AutoSize = true;
            this.lbEmail.Location = new System.Drawing.Point(691, 146);
            this.lbEmail.Name = "lbEmail";
            this.lbEmail.Size = new System.Drawing.Size(66, 24);
            this.lbEmail.TabIndex = 11;
            this.lbEmail.Text = "Email:";
            // 
            // txtSDT
            // 
            this.txtSDT.Font = new System.Drawing.Font("Bahnschrift", 12F);
            this.txtSDT.Location = new System.Drawing.Point(894, 59);
            this.txtSDT.Name = "txtSDT";
            this.txtSDT.Size = new System.Drawing.Size(286, 32);
            this.txtSDT.TabIndex = 10;
            // 
            // lbSDT
            // 
            this.lbSDT.AutoSize = true;
            this.lbSDT.Location = new System.Drawing.Point(691, 62);
            this.lbSDT.Name = "lbSDT";
            this.lbSDT.Size = new System.Drawing.Size(137, 24);
            this.lbSDT.TabIndex = 9;
            this.lbSDT.Text = "Số Điện Thoại:";
            // 
            // txtCCCD
            // 
            this.txtCCCD.Font = new System.Drawing.Font("Bahnschrift", 12F);
            this.txtCCCD.Location = new System.Drawing.Point(205, 231);
            this.txtCCCD.Name = "txtCCCD";
            this.txtCCCD.Size = new System.Drawing.Size(401, 32);
            this.txtCCCD.TabIndex = 8;
            // 
            // lbCCCD
            // 
            this.lbCCCD.AutoSize = true;
            this.lbCCCD.Location = new System.Drawing.Point(12, 234);
            this.lbCCCD.Name = "lbCCCD";
            this.lbCCCD.Size = new System.Drawing.Size(160, 24);
            this.lbCCCD.TabIndex = 7;
            this.lbCCCD.Text = "Số CCCD/CMND: ";
            // 
            // dtpNgaySinh
            // 
            this.dtpNgaySinh.Font = new System.Drawing.Font("Bahnschrift", 12F);
            this.dtpNgaySinh.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpNgaySinh.Location = new System.Drawing.Point(205, 146);
            this.dtpNgaySinh.Name = "dtpNgaySinh";
            this.dtpNgaySinh.Size = new System.Drawing.Size(255, 32);
            this.dtpNgaySinh.TabIndex = 6;
            this.dtpNgaySinh.ValueChanged += new System.EventHandler(this.dtpNgaySinh_ValueChanged);
            // 
            // lbNamSinh
            // 
            this.lbNamSinh.AutoSize = true;
            this.lbNamSinh.Location = new System.Drawing.Point(12, 146);
            this.lbNamSinh.Name = "lbNamSinh";
            this.lbNamSinh.Size = new System.Drawing.Size(103, 24);
            this.lbNamSinh.TabIndex = 3;
            this.lbNamSinh.Text = "Ngày sinh:";
            // 
            // txtHoTen
            // 
            this.txtHoTen.Font = new System.Drawing.Font("Bahnschrift", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtHoTen.Location = new System.Drawing.Point(205, 56);
            this.txtHoTen.Name = "txtHoTen";
            this.txtHoTen.Size = new System.Drawing.Size(401, 32);
            this.txtHoTen.TabIndex = 2;
            // 
            // lbHoTen
            // 
            this.lbHoTen.AutoSize = true;
            this.lbHoTen.Location = new System.Drawing.Point(12, 63);
            this.lbHoTen.Name = "lbHoTen";
            this.lbHoTen.Size = new System.Drawing.Size(102, 24);
            this.lbHoTen.TabIndex = 1;
            this.lbHoTen.Text = "Họ và Tên:";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Cornsilk;
            this.ClientSize = new System.Drawing.Size(1264, 694);
            this.Controls.Add(this.gbThongTin);
            this.Controls.Add(this.gbThongTinCaNhan);
            this.Font = new System.Drawing.Font("Bahnschrift", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.Color.DarkGreen;
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.gbThongTin.ResumeLayout(false);
            this.gbThongTin.PerformLayout();
            this.gbThongTinCaNhan.ResumeLayout(false);
            this.gbThongTinCaNhan.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Label lbLuongCoBan;
        private System.Windows.Forms.TextBox txtLuongCoBan;
        private System.Windows.Forms.GroupBox gbThongTin;
        private System.Windows.Forms.Label lbTenDN;
        private System.Windows.Forms.TextBox txtTenDangNhap;
        private System.Windows.Forms.DateTimePicker dtpNgayDangKy;
        private System.Windows.Forms.Label lbNgayDK;
        private System.Windows.Forms.TextBox txtPass;
        private System.Windows.Forms.Label lbMK;
        private System.Windows.Forms.Label lbChucVu;
        private System.Windows.Forms.GroupBox gbThongTinCaNhan;
        private System.Windows.Forms.Label lbNamSinh;
        private System.Windows.Forms.TextBox txtHoTen;
        private System.Windows.Forms.Label lbHoTen;
        private System.Windows.Forms.Label lbDiaChi;
        private System.Windows.Forms.TextBox txtEmail;
        private System.Windows.Forms.Label lbEmail;
        private System.Windows.Forms.TextBox txtSDT;
        private System.Windows.Forms.Label lbSDT;
        private System.Windows.Forms.TextBox txtCCCD;
        private System.Windows.Forms.Label lbCCCD;
        private System.Windows.Forms.DateTimePicker dtpNgaySinh;
        private System.Windows.Forms.TextBox txtDiaChi;
        private System.Windows.Forms.Button btnDoiMatKhau;
        private System.Windows.Forms.Button btnThoat;
        private System.Windows.Forms.Button btnLuu;
        private System.Windows.Forms.Label dausao1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox btnShowHide;
        private System.Windows.Forms.ComboBox cboVaiTro;
    }
}

