namespace SportsServices.Forms
{
    partial class QuanLiBaoTri
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.txtTaiLai = new System.Windows.Forms.Button();
            this.dgvBaoCao = new System.Windows.Forms.DataGridView();
            this.colMaPhieu = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colCoSo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colMaSan = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colMaNV = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colNgayGioBD = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colNgayKTDK = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colNgayKTTT = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colLyDo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colMoTa = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colChiPhi = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colTrangThai = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.txtMaSanCapNhat = new System.Windows.Forms.Label();
            this.gbUpdateSan = new System.Windows.Forms.GroupBox();
            this.btnLuuTrangThai = new System.Windows.Forms.Button();
            this.cboTrangThaiSan = new System.Windows.Forms.ComboBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.txtTrangThaiMoi = new System.Windows.Forms.Label();
            this.gbQuanLi = new System.Windows.Forms.GroupBox();
            ((System.ComponentModel.ISupportInitialize)(this.dgvBaoCao)).BeginInit();
            this.gbUpdateSan.SuspendLayout();
            this.gbQuanLi.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtTaiLai
            // 
            this.txtTaiLai.Location = new System.Drawing.Point(9, 33);
            this.txtTaiLai.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.txtTaiLai.Name = "txtTaiLai";
            this.txtTaiLai.Size = new System.Drawing.Size(104, 34);
            this.txtTaiLai.TabIndex = 0;
            this.txtTaiLai.Text = "Reset";
            this.txtTaiLai.UseVisualStyleBackColor = true;
            this.txtTaiLai.Click += new System.EventHandler(this.button1_Click);
            // 
            // dgvBaoCao
            // 
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.ActiveBorder;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Bahnschrift", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.DarkGreen;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvBaoCao.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvBaoCao.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvBaoCao.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colMaPhieu,
            this.colCoSo,
            this.colMaSan,
            this.colMaNV,
            this.colNgayGioBD,
            this.colNgayKTDK,
            this.colNgayKTTT,
            this.colLyDo,
            this.colMoTa,
            this.colChiPhi,
            this.colTrangThai});
            this.dgvBaoCao.EnableHeadersVisualStyles = false;
            this.dgvBaoCao.Location = new System.Drawing.Point(18, 94);
            this.dgvBaoCao.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.dgvBaoCao.Name = "dgvBaoCao";
            this.dgvBaoCao.RowHeadersWidth = 51;
            this.dgvBaoCao.RowTemplate.Height = 24;
            this.dgvBaoCao.Size = new System.Drawing.Size(1428, 354);
            this.dgvBaoCao.TabIndex = 1;
            this.dgvBaoCao.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellContentClick);
            // 
            // colMaPhieu
            // 
            this.colMaPhieu.HeaderText = "Mã Phiếu";
            this.colMaPhieu.MinimumWidth = 6;
            this.colMaPhieu.Name = "colMaPhieu";
            this.colMaPhieu.Width = 125;
            // 
            // colCoSo
            // 
            this.colCoSo.HeaderText = "Cơ Sở";
            this.colCoSo.MinimumWidth = 6;
            this.colCoSo.Name = "colCoSo";
            this.colCoSo.Width = 125;
            // 
            // colMaSan
            // 
            this.colMaSan.HeaderText = "Mã Sân";
            this.colMaSan.MinimumWidth = 6;
            this.colMaSan.Name = "colMaSan";
            this.colMaSan.Width = 125;
            // 
            // colMaNV
            // 
            this.colMaNV.HeaderText = "Mã NV Bảo trì";
            this.colMaNV.MinimumWidth = 6;
            this.colMaNV.Name = "colMaNV";
            this.colMaNV.Width = 125;
            // 
            // colNgayGioBD
            // 
            this.colNgayGioBD.HeaderText = "Ngày Bắt Đầu";
            this.colNgayGioBD.MinimumWidth = 6;
            this.colNgayGioBD.Name = "colNgayGioBD";
            this.colNgayGioBD.Width = 125;
            // 
            // colNgayKTDK
            // 
            this.colNgayKTDK.HeaderText = "Ngày KT Dự Kiến";
            this.colNgayKTDK.MinimumWidth = 6;
            this.colNgayKTDK.Name = "colNgayKTDK";
            this.colNgayKTDK.Width = 125;
            // 
            // colNgayKTTT
            // 
            this.colNgayKTTT.HeaderText = "Ngày KT Thực Tế";
            this.colNgayKTTT.MinimumWidth = 6;
            this.colNgayKTTT.Name = "colNgayKTTT";
            this.colNgayKTTT.Width = 125;
            // 
            // colLyDo
            // 
            this.colLyDo.HeaderText = "Lý Do";
            this.colLyDo.MinimumWidth = 6;
            this.colLyDo.Name = "colLyDo";
            this.colLyDo.Width = 125;
            // 
            // colMoTa
            // 
            this.colMoTa.HeaderText = "Mô Tả";
            this.colMoTa.MinimumWidth = 6;
            this.colMoTa.Name = "colMoTa";
            this.colMoTa.Width = 125;
            // 
            // colChiPhi
            // 
            this.colChiPhi.HeaderText = "ChiPhi";
            this.colChiPhi.MinimumWidth = 6;
            this.colChiPhi.Name = "colChiPhi";
            this.colChiPhi.Width = 125;
            // 
            // colTrangThai
            // 
            this.colTrangThai.HeaderText = "Trạng Thái";
            this.colTrangThai.Items.AddRange(new object[] {
            "Chưa Ghi Nhận",
            "Đã Ghi Nhận",
            "Đã Hoàn Thành"});
            this.colTrangThai.MinimumWidth = 6;
            this.colTrangThai.Name = "colTrangThai";
            this.colTrangThai.Width = 125;
            // 
            // txtMaSanCapNhat
            // 
            this.txtMaSanCapNhat.AutoSize = true;
            this.txtMaSanCapNhat.Location = new System.Drawing.Point(19, 60);
            this.txtMaSanCapNhat.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.txtMaSanCapNhat.Name = "txtMaSanCapNhat";
            this.txtMaSanCapNhat.Size = new System.Drawing.Size(81, 24);
            this.txtMaSanCapNhat.TabIndex = 2;
            this.txtMaSanCapNhat.Text = "Mã Sân:";
            this.txtMaSanCapNhat.Click += new System.EventHandler(this.txtMaSanCapNhat_Click);
            // 
            // gbUpdateSan
            // 
            this.gbUpdateSan.Controls.Add(this.btnLuuTrangThai);
            this.gbUpdateSan.Controls.Add(this.cboTrangThaiSan);
            this.gbUpdateSan.Controls.Add(this.textBox1);
            this.gbUpdateSan.Controls.Add(this.txtTrangThaiMoi);
            this.gbUpdateSan.Controls.Add(this.txtMaSanCapNhat);
            this.gbUpdateSan.Location = new System.Drawing.Point(14, 468);
            this.gbUpdateSan.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.gbUpdateSan.Name = "gbUpdateSan";
            this.gbUpdateSan.Padding = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.gbUpdateSan.Size = new System.Drawing.Size(1462, 328);
            this.gbUpdateSan.TabIndex = 3;
            this.gbUpdateSan.TabStop = false;
            this.gbUpdateSan.Text = "Cập nhật trạng thái Sân";
            // 
            // btnLuuTrangThai
            // 
            this.btnLuuTrangThai.Location = new System.Drawing.Point(647, 186);
            this.btnLuuTrangThai.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.btnLuuTrangThai.Name = "btnLuuTrangThai";
            this.btnLuuTrangThai.Size = new System.Drawing.Size(126, 34);
            this.btnLuuTrangThai.TabIndex = 6;
            this.btnLuuTrangThai.Text = "Cập Nhật";
            this.btnLuuTrangThai.UseVisualStyleBackColor = true;
            this.btnLuuTrangThai.Click += new System.EventHandler(this.btnLuuTrangThai_Click);
            // 
            // cboTrangThaiSan
            // 
            this.cboTrangThaiSan.Font = new System.Drawing.Font("Bahnschrift", 12F);
            this.cboTrangThaiSan.FormattingEnabled = true;
            this.cboTrangThaiSan.Items.AddRange(new object[] {
            "Trống",
            "Đang bảo trì"});
            this.cboTrangThaiSan.Location = new System.Drawing.Point(1003, 60);
            this.cboTrangThaiSan.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.cboTrangThaiSan.Name = "cboTrangThaiSan";
            this.cboTrangThaiSan.Size = new System.Drawing.Size(421, 32);
            this.cboTrangThaiSan.TabIndex = 5;
            this.cboTrangThaiSan.SelectedIndexChanged += new System.EventHandler(this.cboTrangThaiSan_SelectedIndexChanged);
            // 
            // textBox1
            // 
            this.textBox1.Font = new System.Drawing.Font("Bahnschrift", 12F);
            this.textBox1.Location = new System.Drawing.Point(142, 60);
            this.textBox1.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(449, 32);
            this.textBox1.TabIndex = 4;
            this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // txtTrangThaiMoi
            // 
            this.txtTrangThaiMoi.AutoSize = true;
            this.txtTrangThaiMoi.Location = new System.Drawing.Point(824, 60);
            this.txtTrangThaiMoi.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.txtTrangThaiMoi.Name = "txtTrangThaiMoi";
            this.txtTrangThaiMoi.Size = new System.Drawing.Size(145, 24);
            this.txtTrangThaiMoi.TabIndex = 3;
            this.txtTrangThaiMoi.Text = "Trạng Thái Mới:";
            this.txtTrangThaiMoi.Click += new System.EventHandler(this.txtTrangThaiMoi_Click);
            // 
            // gbQuanLi
            // 
            this.gbQuanLi.BackColor = System.Drawing.Color.Cornsilk;
            this.gbQuanLi.Controls.Add(this.txtTaiLai);
            this.gbQuanLi.Controls.Add(this.dgvBaoCao);
            this.gbQuanLi.Location = new System.Drawing.Point(14, 13);
            this.gbQuanLi.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.gbQuanLi.Name = "gbQuanLi";
            this.gbQuanLi.Padding = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.gbQuanLi.Size = new System.Drawing.Size(1462, 456);
            this.gbQuanLi.TabIndex = 4;
            this.gbQuanLi.TabStop = false;
            this.gbQuanLi.Text = "Quản Lí Bảo Trì";
            // 
            // QuanLiBaoTri
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Cornsilk;
            this.ClientSize = new System.Drawing.Size(1490, 721);
            this.Controls.Add(this.gbQuanLi);
            this.Controls.Add(this.gbUpdateSan);
            this.Font = new System.Drawing.Font("Bahnschrift", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.Color.DarkGreen;
            this.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.Name = "QuanLiBaoTri";
            this.Text = "QuanLiBaoTri";
            this.Load += new System.EventHandler(this.QuanLiBaoTri_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvBaoCao)).EndInit();
            this.gbUpdateSan.ResumeLayout(false);
            this.gbUpdateSan.PerformLayout();
            this.gbQuanLi.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button txtTaiLai;
        private System.Windows.Forms.DataGridView dgvBaoCao;
        private System.Windows.Forms.DataGridViewTextBoxColumn colMaPhieu;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCoSo;
        private System.Windows.Forms.DataGridViewTextBoxColumn colMaSan;
        private System.Windows.Forms.DataGridViewTextBoxColumn colMaNV;
        private System.Windows.Forms.DataGridViewTextBoxColumn colNgayGioBD;
        private System.Windows.Forms.DataGridViewTextBoxColumn colNgayKTDK;
        private System.Windows.Forms.DataGridViewTextBoxColumn colNgayKTTT;
        private System.Windows.Forms.DataGridViewTextBoxColumn colLyDo;
        private System.Windows.Forms.DataGridViewTextBoxColumn colMoTa;
        private System.Windows.Forms.DataGridViewTextBoxColumn colChiPhi;
        private System.Windows.Forms.DataGridViewComboBoxColumn colTrangThai;
        private System.Windows.Forms.Label txtMaSanCapNhat;
        private System.Windows.Forms.GroupBox gbUpdateSan;
        private System.Windows.Forms.ComboBox cboTrangThaiSan;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label txtTrangThaiMoi;
        private System.Windows.Forms.Button btnLuuTrangThai;
        private System.Windows.Forms.GroupBox gbQuanLi;
    }
}