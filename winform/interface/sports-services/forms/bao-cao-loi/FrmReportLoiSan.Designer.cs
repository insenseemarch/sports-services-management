namespace TTTT
{
    partial class FormReportLoiSan
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
            this.grpThongTinSan = new System.Windows.Forms.GroupBox();
            this.dtpThoiGianLoi = new System.Windows.Forms.DateTimePicker();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.lblCoSo = new System.Windows.Forms.Label();
            this.cboSan = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.grpThongTinLoi = new System.Windows.Forms.GroupBox();
            this.btnGuiBaoLoi = new System.Windows.Forms.Button();
            this.picAnhLoi = new System.Windows.Forms.PictureBox();
            this.btnChonAnh = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.txtMoTa = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtTieuDe = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.cboMucDo = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.grpDanhSachBaoLoi = new System.Windows.Forms.GroupBox();
            this.dgvDSBaoLoi = new System.Windows.Forms.DataGridView();
            this.btnTroVe = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.grpThongTinSan.SuspendLayout();
            this.grpThongTinLoi.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picAnhLoi)).BeginInit();
            this.grpDanhSachBaoLoi.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDSBaoLoi)).BeginInit();
            this.SuspendLayout();
            // 
            // grpThongTinSan
            // 
            this.grpThongTinSan.Controls.Add(this.dtpThoiGianLoi);
            this.grpThongTinSan.Controls.Add(this.label2);
            this.grpThongTinSan.Controls.Add(this.label3);
            this.grpThongTinSan.Controls.Add(this.lblCoSo);
            this.grpThongTinSan.Controls.Add(this.cboSan);
            this.grpThongTinSan.Controls.Add(this.label1);
            this.grpThongTinSan.Font = new System.Drawing.Font("Bahnschrift SemiBold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpThongTinSan.Location = new System.Drawing.Point(49, 129);
            this.grpThongTinSan.Name = "grpThongTinSan";
            this.grpThongTinSan.Size = new System.Drawing.Size(604, 175);
            this.grpThongTinSan.TabIndex = 0;
            this.grpThongTinSan.TabStop = false;
            this.grpThongTinSan.Text = "Thông tin sân";
            // 
            // dtpThoiGianLoi
            // 
            this.dtpThoiGianLoi.CustomFormat = "dd/MM/yyyy HH:mm";
            this.dtpThoiGianLoi.Font = new System.Drawing.Font("Bahnschrift", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dtpThoiGianLoi.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpThoiGianLoi.Location = new System.Drawing.Point(261, 125);
            this.dtpThoiGianLoi.Name = "dtpThoiGianLoi";
            this.dtpThoiGianLoi.Size = new System.Drawing.Size(229, 28);
            this.dtpThoiGianLoi.TabIndex = 5;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(24, 125);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(210, 24);
            this.label2.TabIndex = 4;
            this.label2.Text = "Thời gian phát hiện lỗi:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(24, 76);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(49, 24);
            this.label3.TabIndex = 3;
            this.label3.Text = "Sân:";
            // 
            // lblCoSo
            // 
            this.lblCoSo.AutoSize = true;
            this.lblCoSo.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblCoSo.Font = new System.Drawing.Font("Bahnschrift", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCoSo.Location = new System.Drawing.Point(109, 32);
            this.lblCoSo.Name = "lblCoSo";
            this.lblCoSo.Size = new System.Drawing.Size(51, 23);
            this.lblCoSo.TabIndex = 2;
            this.lblCoSo.Text = "temp";
            // 
            // cboSan
            // 
            this.cboSan.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboSan.Font = new System.Drawing.Font("Bahnschrift", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cboSan.FormattingEnabled = true;
            this.cboSan.Location = new System.Drawing.Point(109, 73);
            this.cboSan.Name = "cboSan";
            this.cboSan.Size = new System.Drawing.Size(270, 29);
            this.cboSan.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(24, 32);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(64, 24);
            this.label1.TabIndex = 0;
            this.label1.Text = "Cơ sở:";
            // 
            // grpThongTinLoi
            // 
            this.grpThongTinLoi.Controls.Add(this.btnGuiBaoLoi);
            this.grpThongTinLoi.Controls.Add(this.picAnhLoi);
            this.grpThongTinLoi.Controls.Add(this.btnChonAnh);
            this.grpThongTinLoi.Controls.Add(this.label7);
            this.grpThongTinLoi.Controls.Add(this.txtMoTa);
            this.grpThongTinLoi.Controls.Add(this.label6);
            this.grpThongTinLoi.Controls.Add(this.txtTieuDe);
            this.grpThongTinLoi.Controls.Add(this.label5);
            this.grpThongTinLoi.Controls.Add(this.cboMucDo);
            this.grpThongTinLoi.Controls.Add(this.label4);
            this.grpThongTinLoi.Font = new System.Drawing.Font("Bahnschrift SemiBold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpThongTinLoi.Location = new System.Drawing.Point(49, 324);
            this.grpThongTinLoi.Name = "grpThongTinLoi";
            this.grpThongTinLoi.Size = new System.Drawing.Size(604, 335);
            this.grpThongTinLoi.TabIndex = 1;
            this.grpThongTinLoi.TabStop = false;
            this.grpThongTinLoi.Text = "Thông tin lỗi";
            // 
            // btnGuiBaoLoi
            // 
            this.btnGuiBaoLoi.BackColor = System.Drawing.Color.ForestGreen;
            this.btnGuiBaoLoi.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnGuiBaoLoi.ForeColor = System.Drawing.Color.Transparent;
            this.btnGuiBaoLoi.Location = new System.Drawing.Point(243, 283);
            this.btnGuiBaoLoi.Name = "btnGuiBaoLoi";
            this.btnGuiBaoLoi.Size = new System.Drawing.Size(103, 36);
            this.btnGuiBaoLoi.TabIndex = 9;
            this.btnGuiBaoLoi.Text = "Báo Lỗi";
            this.btnGuiBaoLoi.UseVisualStyleBackColor = false;
            // 
            // picAnhLoi
            // 
            this.picAnhLoi.Location = new System.Drawing.Point(320, 84);
            this.picAnhLoi.Name = "picAnhLoi";
            this.picAnhLoi.Size = new System.Drawing.Size(267, 182);
            this.picAnhLoi.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picAnhLoi.TabIndex = 8;
            this.picAnhLoi.TabStop = false;
            // 
            // btnChonAnh
            // 
            this.btnChonAnh.Location = new System.Drawing.Point(488, 44);
            this.btnChonAnh.Name = "btnChonAnh";
            this.btnChonAnh.Size = new System.Drawing.Size(99, 34);
            this.btnChonAnh.TabIndex = 7;
            this.btnChonAnh.Text = "Tải ảnh";
            this.btnChonAnh.UseVisualStyleBackColor = true;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Bahnschrift", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(316, 49);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(146, 21);
            this.label7.TabIndex = 6;
            this.label7.Text = "Ảnh lỗi (tùy chọn):";
            // 
            // txtMoTa
            // 
            this.txtMoTa.Font = new System.Drawing.Font("Bahnschrift", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtMoTa.Location = new System.Drawing.Point(110, 146);
            this.txtMoTa.Multiline = true;
            this.txtMoTa.Name = "txtMoTa";
            this.txtMoTa.Size = new System.Drawing.Size(200, 120);
            this.txtMoTa.TabIndex = 5;
            this.txtMoTa.TextChanged += new System.EventHandler(this.txtMoTa_TextChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(23, 146);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(65, 24);
            this.label6.TabIndex = 4;
            this.label6.Text = "Mô tả:";
            // 
            // txtTieuDe
            // 
            this.txtTieuDe.Font = new System.Drawing.Font("Bahnschrift", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtTieuDe.Location = new System.Drawing.Point(110, 96);
            this.txtTieuDe.Name = "txtTieuDe";
            this.txtTieuDe.Size = new System.Drawing.Size(200, 28);
            this.txtTieuDe.TabIndex = 3;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(24, 99);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(79, 24);
            this.label5.TabIndex = 2;
            this.label5.Text = "Tiêu đề:";
            // 
            // cboMucDo
            // 
            this.cboMucDo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboMucDo.Font = new System.Drawing.Font("Bahnschrift", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cboMucDo.FormattingEnabled = true;
            this.cboMucDo.Location = new System.Drawing.Point(113, 46);
            this.cboMucDo.Name = "cboMucDo";
            this.cboMucDo.Size = new System.Drawing.Size(121, 29);
            this.cboMucDo.TabIndex = 1;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(25, 49);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(80, 24);
            this.label4.TabIndex = 0;
            this.label4.Text = "Mức độ:";
            // 
            // grpDanhSachBaoLoi
            // 
            this.grpDanhSachBaoLoi.Controls.Add(this.dgvDSBaoLoi);
            this.grpDanhSachBaoLoi.Font = new System.Drawing.Font("Bahnschrift SemiBold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpDanhSachBaoLoi.Location = new System.Drawing.Point(688, 129);
            this.grpDanhSachBaoLoi.Name = "grpDanhSachBaoLoi";
            this.grpDanhSachBaoLoi.Size = new System.Drawing.Size(410, 506);
            this.grpDanhSachBaoLoi.TabIndex = 2;
            this.grpDanhSachBaoLoi.TabStop = false;
            this.grpDanhSachBaoLoi.Text = "Các lỗi đã báo";
            // 
            // dgvDSBaoLoi
            // 
            this.dgvDSBaoLoi.AllowUserToAddRows = false;
            this.dgvDSBaoLoi.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvDSBaoLoi.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvDSBaoLoi.Location = new System.Drawing.Point(3, 28);
            this.dgvDSBaoLoi.Name = "dgvDSBaoLoi";
            this.dgvDSBaoLoi.ReadOnly = true;
            this.dgvDSBaoLoi.RowHeadersWidth = 51;
            this.dgvDSBaoLoi.RowTemplate.Height = 24;
            this.dgvDSBaoLoi.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvDSBaoLoi.Size = new System.Drawing.Size(404, 475);
            this.dgvDSBaoLoi.TabIndex = 0;
            // 
            // btnTroVe
            // 
            this.btnTroVe.BackColor = System.Drawing.Color.ForestGreen;
            this.btnTroVe.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnTroVe.Font = new System.Drawing.Font("Bahnschrift", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnTroVe.ForeColor = System.Drawing.Color.Transparent;
            this.btnTroVe.Location = new System.Drawing.Point(992, 662);
            this.btnTroVe.Name = "btnTroVe";
            this.btnTroVe.Size = new System.Drawing.Size(103, 36);
            this.btnTroVe.TabIndex = 11;
            this.btnTroVe.Text = "Trở Về";
            this.btnTroVe.UseVisualStyleBackColor = false;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Bahnschrift", 25.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.ForeColor = System.Drawing.Color.ForestGreen;
            this.label8.Location = new System.Drawing.Point(425, 9);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(343, 53);
            this.label8.TabIndex = 12;
            this.label8.Text = "Báo Cáo Lỗi Sân";
            this.label8.Click += new System.EventHandler(this.label8_Click);
            // 
            // FormReportLoiSan
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Cornsilk;
            this.ClientSize = new System.Drawing.Size(1157, 748);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.btnTroVe);
            this.Controls.Add(this.grpDanhSachBaoLoi);
            this.Controls.Add(this.grpThongTinLoi);
            this.Controls.Add(this.grpThongTinSan);
            this.Name = "FormReportLoiSan";
            this.Text = "FormReportLoiSan";
            this.grpThongTinSan.ResumeLayout(false);
            this.grpThongTinSan.PerformLayout();
            this.grpThongTinLoi.ResumeLayout(false);
            this.grpThongTinLoi.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picAnhLoi)).EndInit();
            this.grpDanhSachBaoLoi.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvDSBaoLoi)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox grpThongTinSan;
        private System.Windows.Forms.ComboBox cboSan;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DateTimePicker dtpThoiGianLoi;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lblCoSo;
        private System.Windows.Forms.GroupBox grpThongTinLoi;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtMoTa;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtTieuDe;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox cboMucDo;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.PictureBox picAnhLoi;
        private System.Windows.Forms.Button btnChonAnh;
        private System.Windows.Forms.GroupBox grpDanhSachBaoLoi;
        private System.Windows.Forms.DataGridView dgvDSBaoLoi;
        private System.Windows.Forms.Button btnGuiBaoLoi;
        private System.Windows.Forms.Button btnTroVe;
        private System.Windows.Forms.Label label8;
    }
}