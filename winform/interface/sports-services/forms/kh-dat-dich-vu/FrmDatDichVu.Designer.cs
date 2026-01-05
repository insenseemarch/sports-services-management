namespace TTTT
{
    partial class FormDatDichVu
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            this.lblTieuDe = new System.Windows.Forms.Label();
            this.lblThongTinPhieu = new System.Windows.Forms.Label();
            this.dgdDichVu = new System.Windows.Forms.DataGridView();
            this.Chon = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.MaDV = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TenDV = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DonGia = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SoLuong = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lblTongTien = new System.Windows.Forms.Label();
            this.btnLuuDichVu = new System.Windows.Forms.Button();
            this.btnBoQua = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgdDichVu)).BeginInit();
            this.SuspendLayout();
            // 
            // lblTieuDe
            // 
            this.lblTieuDe.AutoSize = true;
            this.lblTieuDe.Font = new System.Drawing.Font("Bahnschrift", 25.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTieuDe.ForeColor = System.Drawing.Color.ForestGreen;
            this.lblTieuDe.Location = new System.Drawing.Point(348, 21);
            this.lblTieuDe.Name = "lblTieuDe";
            this.lblTieuDe.Size = new System.Drawing.Size(447, 52);
            this.lblTieuDe.TabIndex = 0;
            this.lblTieuDe.Text = "Đặt Dịch Vụ Kèm Theo";
            // 
            // lblThongTinPhieu
            // 
            this.lblThongTinPhieu.AutoSize = true;
            this.lblThongTinPhieu.Font = new System.Drawing.Font("Bahnschrift SemiBold", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblThongTinPhieu.ForeColor = System.Drawing.Color.DarkOliveGreen;
            this.lblThongTinPhieu.Location = new System.Drawing.Point(445, 85);
            this.lblThongTinPhieu.Name = "lblThongTinPhieu";
            this.lblThongTinPhieu.Size = new System.Drawing.Size(254, 36);
            this.lblThongTinPhieu.TabIndex = 1;
            this.lblThongTinPhieu.Text = "Phiếu đặt sân: #...";
            // 
            // dgdDichVu
            // 
            this.dgdDichVu.AllowUserToAddRows = false;
            this.dgdDichVu.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgdDichVu.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dgdDichVu.BackgroundColor = System.Drawing.SystemColors.ControlDarkDark;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgdDichVu.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.dgdDichVu.ColumnHeadersHeight = 27;
            this.dgdDichVu.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Chon,
            this.MaDV,
            this.TenDV,
            this.DonGia,
            this.SoLuong});
            this.dgdDichVu.GridColor = System.Drawing.Color.Black;
            this.dgdDichVu.Location = new System.Drawing.Point(118, 145);
            this.dgdDichVu.Name = "dgdDichVu";
            this.dgdDichVu.RowHeadersVisible = false;
            this.dgdDichVu.RowHeadersWidth = 51;
            this.dgdDichVu.RowTemplate.Height = 24;
            this.dgdDichVu.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgdDichVu.Size = new System.Drawing.Size(930, 396);
            this.dgdDichVu.TabIndex = 2;
            this.dgdDichVu.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgdDichVu_CellContentClick);
            // 
            // Chon
            // 
            this.Chon.HeaderText = "Chọn";
            this.Chon.MinimumWidth = 6;
            this.Chon.Name = "Chon";
            this.Chon.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Chon.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.Chon.Width = 77;
            // 
            // MaDV
            // 
            this.MaDV.HeaderText = "Mã Dịch Vụ";
            this.MaDV.MinimumWidth = 6;
            this.MaDV.Name = "MaDV";
            this.MaDV.Visible = false;
            this.MaDV.Width = 126;
            // 
            // TenDV
            // 
            this.TenDV.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.TenDV.HeaderText = "Tên Dịch Vụ";
            this.TenDV.MinimumWidth = 6;
            this.TenDV.Name = "TenDV";
            // 
            // DonGia
            // 
            dataGridViewCellStyle4.Format = "N0";
            this.DonGia.DefaultCellStyle = dataGridViewCellStyle4;
            this.DonGia.HeaderText = "Đơn Giá";
            this.DonGia.MinimumWidth = 6;
            this.DonGia.Name = "DonGia";
            this.DonGia.Width = 99;
            // 
            // SoLuong
            // 
            this.SoLuong.HeaderText = "Số Lượng";
            this.SoLuong.MinimumWidth = 6;
            this.SoLuong.Name = "SoLuong";
            this.SoLuong.Width = 109;
            // 
            // lblTongTien
            // 
            this.lblTongTien.AutoSize = true;
            this.lblTongTien.Font = new System.Drawing.Font("Bahnschrift", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTongTien.ForeColor = System.Drawing.Color.ForestGreen;
            this.lblTongTien.Location = new System.Drawing.Point(112, 556);
            this.lblTongTien.Name = "lblTongTien";
            this.lblTongTien.Size = new System.Drawing.Size(226, 28);
            this.lblTongTien.TabIndex = 3;
            this.lblTongTien.Text = " Tổng tiền dịch vụ: 0đ";
            // 
            // btnLuuDichVu
            // 
            this.btnLuuDichVu.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnLuuDichVu.BackColor = System.Drawing.Color.ForestGreen;
            this.btnLuuDichVu.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLuuDichVu.Font = new System.Drawing.Font("Bahnschrift", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnLuuDichVu.ForeColor = System.Drawing.Color.Transparent;
            this.btnLuuDichVu.Location = new System.Drawing.Point(374, 652);
            this.btnLuuDichVu.Name = "btnLuuDichVu";
            this.btnLuuDichVu.Size = new System.Drawing.Size(164, 42);
            this.btnLuuDichVu.TabIndex = 4;
            this.btnLuuDichVu.Text = "Lưu Dịch Vụ";
            this.btnLuuDichVu.UseVisualStyleBackColor = false;
            // 
            // btnBoQua
            // 
            this.btnBoQua.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBoQua.BackColor = System.Drawing.Color.Red;
            this.btnBoQua.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBoQua.Font = new System.Drawing.Font("Bahnschrift", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnBoQua.ForeColor = System.Drawing.Color.Transparent;
            this.btnBoQua.Location = new System.Drawing.Point(689, 652);
            this.btnBoQua.Name = "btnBoQua";
            this.btnBoQua.Size = new System.Drawing.Size(117, 42);
            this.btnBoQua.TabIndex = 5;
            this.btnBoQua.Text = "Bỏ Qua";
            this.btnBoQua.UseVisualStyleBackColor = false;
            // 
            // FormDatDichVu
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Cornsilk;
            this.ClientSize = new System.Drawing.Size(1157, 748);
            this.Controls.Add(this.btnBoQua);
            this.Controls.Add(this.btnLuuDichVu);
            this.Controls.Add(this.lblTongTien);
            this.Controls.Add(this.dgdDichVu);
            this.Controls.Add(this.lblThongTinPhieu);
            this.Controls.Add(this.lblTieuDe);
            this.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "FormDatDichVu";
            this.Text = "FormDatDichVu";
            ((System.ComponentModel.ISupportInitialize)(this.dgdDichVu)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblTieuDe;
        private System.Windows.Forms.Label lblThongTinPhieu;
        private System.Windows.Forms.DataGridView dgdDichVu;
        private System.Windows.Forms.Label lblTongTien;
        private System.Windows.Forms.Button btnLuuDichVu;
        private System.Windows.Forms.Button btnBoQua;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Chon;
        private System.Windows.Forms.DataGridViewTextBoxColumn MaDV;
        private System.Windows.Forms.DataGridViewTextBoxColumn TenDV;
        private System.Windows.Forms.DataGridViewTextBoxColumn DonGia;
        private System.Windows.Forms.DataGridViewTextBoxColumn SoLuong;
    }
}