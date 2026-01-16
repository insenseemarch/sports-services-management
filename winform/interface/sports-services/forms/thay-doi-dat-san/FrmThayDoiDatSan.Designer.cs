namespace SportsServices.Forms
{
    partial class FormDoiSan
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
            this.grpDonDaDat = new System.Windows.Forms.GroupBox();
            this.dgvDanhSach = new System.Windows.Forms.DataGridView();
            this.grpTTHienTai = new System.Windows.Forms.GroupBox();
            this.lblTrangThai = new System.Windows.Forms.Label();
            this.lblThoiGianCu = new System.Windows.Forms.Label();
            this.lblCoSo = new System.Windows.Forms.Label();
            this.lblSan = new System.Windows.Forms.Label();
            this.lblMaDatSan = new System.Windows.Forms.Label();
            this.grpThaiDoi = new System.Windows.Forms.GroupBox();
            this.btnApDungThayDoi = new System.Windows.Forms.Button();
            this.dtpGioKetThucMoi = new System.Windows.Forms.DateTimePicker();
            this.dtpGioBatDauMoi = new System.Windows.Forms.DateTimePicker();
            this.dtpNgayMoi = new System.Windows.Forms.DateTimePicker();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.rdoHuyDat = new System.Windows.Forms.RadioButton();
            this.rdoDoiLich = new System.Windows.Forms.RadioButton();
            this.label4 = new System.Windows.Forms.Label();
            this.grpDonDaDat.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDanhSach)).BeginInit();
            this.grpTTHienTai.SuspendLayout();
            this.grpThaiDoi.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpDonDaDat
            // 
            this.grpDonDaDat.Controls.Add(this.dgvDanhSach);
            this.grpDonDaDat.Font = new System.Drawing.Font("Bahnschrift SemiBold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpDonDaDat.Location = new System.Drawing.Point(78, 120);
            this.grpDonDaDat.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.grpDonDaDat.Name = "grpDonDaDat";
            this.grpDonDaDat.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.grpDonDaDat.Size = new System.Drawing.Size(548, 526);
            this.grpDonDaDat.TabIndex = 0;
            this.grpDonDaDat.TabStop = false;
            this.grpDonDaDat.Text = "Danh sách sân đã đặt";
            // 
            // dgvDanhSach
            // 
            this.dgvDanhSach.AllowUserToAddRows = false;
            this.dgvDanhSach.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvDanhSach.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvDanhSach.Location = new System.Drawing.Point(3, 27);
            this.dgvDanhSach.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.dgvDanhSach.Name = "dgvDanhSach";
            this.dgvDanhSach.ReadOnly = true;
            this.dgvDanhSach.RowHeadersWidth = 51;
            this.dgvDanhSach.RowTemplate.Height = 24;
            this.dgvDanhSach.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvDanhSach.Size = new System.Drawing.Size(542, 497);
            this.dgvDanhSach.TabIndex = 0;
            // 
            // grpTTHienTai
            // 
            this.grpTTHienTai.Controls.Add(this.lblTrangThai);
            this.grpTTHienTai.Controls.Add(this.lblThoiGianCu);
            this.grpTTHienTai.Controls.Add(this.lblCoSo);
            this.grpTTHienTai.Controls.Add(this.lblSan);
            this.grpTTHienTai.Controls.Add(this.lblMaDatSan);
            this.grpTTHienTai.Font = new System.Drawing.Font("Bahnschrift SemiBold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpTTHienTai.Location = new System.Drawing.Point(632, 120);
            this.grpTTHienTai.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.grpTTHienTai.Name = "grpTTHienTai";
            this.grpTTHienTai.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.grpTTHienTai.Size = new System.Drawing.Size(445, 241);
            this.grpTTHienTai.TabIndex = 1;
            this.grpTTHienTai.TabStop = false;
            this.grpTTHienTai.Text = "Thông tin hiện tại";
            this.grpTTHienTai.Enter += new System.EventHandler(this.grpTTHienTai_Enter);
            // 
            // lblTrangThai
            // 
            this.lblTrangThai.AutoSize = true;
            this.lblTrangThai.Font = new System.Drawing.Font("Bahnschrift", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTrangThai.Location = new System.Drawing.Point(28, 202);
            this.lblTrangThai.Name = "lblTrangThai";
            this.lblTrangThai.Size = new System.Drawing.Size(120, 24);
            this.lblTrangThai.TabIndex = 4;
            this.lblTrangThai.Text = "Trạng thái: -";
            // 
            // lblThoiGianCu
            // 
            this.lblThoiGianCu.AutoSize = true;
            this.lblThoiGianCu.Font = new System.Drawing.Font("Bahnschrift", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblThoiGianCu.Location = new System.Drawing.Point(28, 161);
            this.lblThoiGianCu.Name = "lblThoiGianCu";
            this.lblThoiGianCu.Size = new System.Drawing.Size(136, 24);
            this.lblThoiGianCu.TabIndex = 3;
            this.lblThoiGianCu.Text = "Thời gian cũ: -";
            // 
            // lblCoSo
            // 
            this.lblCoSo.AutoSize = true;
            this.lblCoSo.Font = new System.Drawing.Font("Bahnschrift", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCoSo.Location = new System.Drawing.Point(28, 122);
            this.lblCoSo.Name = "lblCoSo";
            this.lblCoSo.Size = new System.Drawing.Size(79, 24);
            this.lblCoSo.TabIndex = 2;
            this.lblCoSo.Text = "Cơ sở: -";
            // 
            // lblSan
            // 
            this.lblSan.AutoSize = true;
            this.lblSan.Font = new System.Drawing.Font("Bahnschrift", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSan.Location = new System.Drawing.Point(28, 82);
            this.lblSan.Name = "lblSan";
            this.lblSan.Size = new System.Drawing.Size(64, 24);
            this.lblSan.TabIndex = 1;
            this.lblSan.Text = "Sân: -";
            this.lblSan.Click += new System.EventHandler(this.label2_Click);
            // 
            // lblMaDatSan
            // 
            this.lblMaDatSan.AutoSize = true;
            this.lblMaDatSan.Font = new System.Drawing.Font("Bahnschrift", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMaDatSan.Location = new System.Drawing.Point(28, 41);
            this.lblMaDatSan.Name = "lblMaDatSan";
            this.lblMaDatSan.Size = new System.Drawing.Size(128, 24);
            this.lblMaDatSan.TabIndex = 0;
            this.lblMaDatSan.Text = "Mã đặt sân: -";
            this.lblMaDatSan.Click += new System.EventHandler(this.lblMaDatSan_Click);
            // 
            // grpThaiDoi
            // 
            this.grpThaiDoi.Controls.Add(this.btnApDungThayDoi);
            this.grpThaiDoi.Controls.Add(this.dtpGioKetThucMoi);
            this.grpThaiDoi.Controls.Add(this.dtpGioBatDauMoi);
            this.grpThaiDoi.Controls.Add(this.dtpNgayMoi);
            this.grpThaiDoi.Controls.Add(this.label3);
            this.grpThaiDoi.Controls.Add(this.label2);
            this.grpThaiDoi.Controls.Add(this.label1);
            this.grpThaiDoi.Controls.Add(this.rdoHuyDat);
            this.grpThaiDoi.Controls.Add(this.rdoDoiLich);
            this.grpThaiDoi.Font = new System.Drawing.Font("Bahnschrift SemiBold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpThaiDoi.Location = new System.Drawing.Point(632, 365);
            this.grpThaiDoi.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.grpThaiDoi.Name = "grpThaiDoi";
            this.grpThaiDoi.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.grpThaiDoi.Size = new System.Drawing.Size(445, 281);
            this.grpThaiDoi.TabIndex = 2;
            this.grpThaiDoi.TabStop = false;
            this.grpThaiDoi.Text = "Thay đổi đơn";
            // 
            // btnApDungThayDoi
            // 
            this.btnApDungThayDoi.BackColor = System.Drawing.Color.ForestGreen;
            this.btnApDungThayDoi.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnApDungThayDoi.ForeColor = System.Drawing.Color.Transparent;
            this.btnApDungThayDoi.Location = new System.Drawing.Point(120, 229);
            this.btnApDungThayDoi.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnApDungThayDoi.Name = "btnApDungThayDoi";
            this.btnApDungThayDoi.Size = new System.Drawing.Size(221, 37);
            this.btnApDungThayDoi.TabIndex = 8;
            this.btnApDungThayDoi.Text = "Áp Dụng Thay Đổi";
            this.btnApDungThayDoi.UseVisualStyleBackColor = false;
            // 
            // dtpGioKetThucMoi
            // 
            this.dtpGioKetThucMoi.CustomFormat = "HH:mm";
            this.dtpGioKetThucMoi.Font = new System.Drawing.Font("Bahnschrift", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dtpGioKetThucMoi.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpGioKetThucMoi.Location = new System.Drawing.Point(191, 184);
            this.dtpGioKetThucMoi.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.dtpGioKetThucMoi.Name = "dtpGioKetThucMoi";
            this.dtpGioKetThucMoi.Size = new System.Drawing.Size(127, 32);
            this.dtpGioKetThucMoi.TabIndex = 7;
            // 
            // dtpGioBatDauMoi
            // 
            this.dtpGioBatDauMoi.CustomFormat = "HH:mm";
            this.dtpGioBatDauMoi.Font = new System.Drawing.Font("Bahnschrift", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dtpGioBatDauMoi.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpGioBatDauMoi.Location = new System.Drawing.Point(191, 138);
            this.dtpGioBatDauMoi.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.dtpGioBatDauMoi.Name = "dtpGioBatDauMoi";
            this.dtpGioBatDauMoi.Size = new System.Drawing.Size(127, 32);
            this.dtpGioBatDauMoi.TabIndex = 6;
            // 
            // dtpNgayMoi
            // 
            this.dtpNgayMoi.Font = new System.Drawing.Font("Bahnschrift", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dtpNgayMoi.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpNgayMoi.Location = new System.Drawing.Point(143, 93);
            this.dtpNgayMoi.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.dtpNgayMoi.Name = "dtpNgayMoi";
            this.dtpNgayMoi.Size = new System.Drawing.Size(127, 32);
            this.dtpNgayMoi.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(28, 190);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(160, 24);
            this.label3.TabIndex = 4;
            this.label3.Text = "Giờ kết thúc mới:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(28, 144);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(154, 24);
            this.label2.TabIndex = 3;
            this.label2.Text = "Giờ bắt đầu mới:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(27, 99);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(99, 24);
            this.label1.TabIndex = 2;
            this.label1.Text = "Ngày mới:";
            // 
            // rdoHuyDat
            // 
            this.rdoHuyDat.AutoSize = true;
            this.rdoHuyDat.Font = new System.Drawing.Font("Bahnschrift SemiBold", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rdoHuyDat.Location = new System.Drawing.Point(31, 30);
            this.rdoHuyDat.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.rdoHuyDat.Name = "rdoHuyDat";
            this.rdoHuyDat.Size = new System.Drawing.Size(89, 25);
            this.rdoHuyDat.TabIndex = 1;
            this.rdoHuyDat.TabStop = true;
            this.rdoHuyDat.Text = "Hủy đặt";
            this.rdoHuyDat.UseVisualStyleBackColor = true;
            // 
            // rdoDoiLich
            // 
            this.rdoDoiLich.AutoSize = true;
            this.rdoDoiLich.Font = new System.Drawing.Font("Bahnschrift SemiBold", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rdoDoiLich.Location = new System.Drawing.Point(31, 61);
            this.rdoDoiLich.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.rdoDoiLich.Name = "rdoDoiLich";
            this.rdoDoiLich.Size = new System.Drawing.Size(86, 25);
            this.rdoDoiLich.TabIndex = 0;
            this.rdoDoiLich.TabStop = true;
            this.rdoDoiLich.Text = "Dời lịch";
            this.rdoDoiLich.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Bahnschrift", 25.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.Color.ForestGreen;
            this.label4.Location = new System.Drawing.Point(362, 25);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(493, 53);
            this.label4.TabIndex = 3;
            this.label4.Text = "Điều Chỉnh Đơn Đặt Sân";
            // 
            // FormDoiSan
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Cornsilk;
            this.ClientSize = new System.Drawing.Size(1157, 748);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.grpThaiDoi);
            this.Controls.Add(this.grpTTHienTai);
            this.Controls.Add(this.grpDonDaDat);
            this.Cursor = System.Windows.Forms.Cursors.Default;
            this.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "FormDoiSan";
            this.Text = "FormDoiSan";
            this.grpDonDaDat.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvDanhSach)).EndInit();
            this.grpTTHienTai.ResumeLayout(false);
            this.grpTTHienTai.PerformLayout();
            this.grpThaiDoi.ResumeLayout(false);
            this.grpThaiDoi.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox grpDonDaDat;
        private System.Windows.Forms.DataGridView dgvDanhSach;
        private System.Windows.Forms.GroupBox grpTTHienTai;
        private System.Windows.Forms.Label lblTrangThai;
        private System.Windows.Forms.Label lblThoiGianCu;
        private System.Windows.Forms.Label lblCoSo;
        private System.Windows.Forms.Label lblSan;
        private System.Windows.Forms.Label lblMaDatSan;
        private System.Windows.Forms.GroupBox grpThaiDoi;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RadioButton rdoHuyDat;
        private System.Windows.Forms.RadioButton rdoDoiLich;
        private System.Windows.Forms.Button btnApDungThayDoi;
        private System.Windows.Forms.DateTimePicker dtpGioKetThucMoi;
        private System.Windows.Forms.DateTimePicker dtpGioBatDauMoi;
        private System.Windows.Forms.DateTimePicker dtpNgayMoi;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label4;
    }
}