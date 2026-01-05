namespace SportsServices.Forms
{
    partial class FrmPheDuyetDonNghiPhep
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
            this.dgvDanhSachDon = new System.Windows.Forms.DataGridView();
            this.colMaDon = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colMaNV = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colNgayNghi = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colCaNghi = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colMaThayThe = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colTrangThai = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lblDanhSachDon = new System.Windows.Forms.Label();
            this.grpChiTiet = new System.Windows.Forms.GroupBox();
            this.txtNguoiThayThe = new System.Windows.Forms.TextBox();
            this.txtMaDon = new System.Windows.Forms.TextBox();
            this.lblMaDon = new System.Windows.Forms.Label();
            this.btnTuChoi = new System.Windows.Forms.Button();
            this.btnDuyet = new System.Windows.Forms.Button();
            this.txtLyDo = new System.Windows.Forms.TextBox();
            this.txtTenNV = new System.Windows.Forms.TextBox();
            this.lblLiDo = new System.Windows.Forms.Label();
            this.lblTenNguoiThayThe = new System.Windows.Forms.Label();
            this.lblTenNguoiNghi = new System.Windows.Forms.Label();
            this.lblChiNhanh = new System.Windows.Forms.Label();
            this.cboChiNhanh = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDanhSachDon)).BeginInit();
            this.grpChiTiet.SuspendLayout();
            this.SuspendLayout();
            // 
            // dgvDanhSachDon
            // 
            this.dgvDanhSachDon.AllowUserToAddRows = false;
            this.dgvDanhSachDon.AllowUserToDeleteRows = false;
            this.dgvDanhSachDon.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvDanhSachDon.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colMaDon,
            this.colMaNV,
            this.colNgayNghi,
            this.colCaNghi,
            this.colMaThayThe,
            this.colTrangThai});
            this.dgvDanhSachDon.Location = new System.Drawing.Point(12, 113);
            this.dgvDanhSachDon.MultiSelect = false;
            this.dgvDanhSachDon.Name = "dgvDanhSachDon";
            this.dgvDanhSachDon.ReadOnly = true;
            this.dgvDanhSachDon.RowHeadersWidth = 51;
            this.dgvDanhSachDon.RowTemplate.Height = 24;
            this.dgvDanhSachDon.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvDanhSachDon.Size = new System.Drawing.Size(1323, 192);
            this.dgvDanhSachDon.TabIndex = 0;
            this.dgvDanhSachDon.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvDanhSachDon_CellClick);
            // 
            // colMaDon
            // 
            this.colMaDon.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colMaDon.DataPropertyName = "MaDon";
            this.colMaDon.HeaderText = "Mã Đơn";
            this.colMaDon.MinimumWidth = 6;
            this.colMaDon.Name = "colMaDon";
            this.colMaDon.ReadOnly = true;
            // 
            // colMaNV
            // 
            this.colMaNV.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colMaNV.DataPropertyName = "MaNV";
            this.colMaNV.HeaderText = "Mã Nhân Viên Xin Nghỉ";
            this.colMaNV.MinimumWidth = 6;
            this.colMaNV.Name = "colMaNV";
            this.colMaNV.ReadOnly = true;
            // 
            // colNgayNghi
            // 
            this.colNgayNghi.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colNgayNghi.DataPropertyName = "NgayNghi";
            this.colNgayNghi.HeaderText = "Ngày Nghỉ";
            this.colNgayNghi.MinimumWidth = 6;
            this.colNgayNghi.Name = "colNgayNghi";
            this.colNgayNghi.ReadOnly = true;
            // 
            // colCaNghi
            // 
            this.colCaNghi.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colCaNghi.DataPropertyName = "CaNghi";
            this.colCaNghi.HeaderText = "Ca Nghỉ";
            this.colCaNghi.MinimumWidth = 6;
            this.colCaNghi.Name = "colCaNghi";
            this.colCaNghi.ReadOnly = true;
            // 
            // colMaThayThe
            // 
            this.colMaThayThe.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colMaThayThe.DataPropertyName = "MaNVThayThe";
            this.colMaThayThe.HeaderText = "Mã Nhân Viên Thay Thế";
            this.colMaThayThe.MinimumWidth = 6;
            this.colMaThayThe.Name = "colMaThayThe";
            this.colMaThayThe.ReadOnly = true;
            // 
            // colTrangThai
            // 
            this.colTrangThai.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colTrangThai.DataPropertyName = "TrangThai";
            this.colTrangThai.HeaderText = "Trạng Thái";
            this.colTrangThai.MinimumWidth = 6;
            this.colTrangThai.Name = "colTrangThai";
            this.colTrangThai.ReadOnly = true;
            this.colTrangThai.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            // 
            // lblDanhSachDon
            // 
            this.lblDanhSachDon.AutoSize = true;
            this.lblDanhSachDon.BackColor = System.Drawing.Color.Cornsilk;
            this.lblDanhSachDon.Font = new System.Drawing.Font("Bahnschrift", 25.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDanhSachDon.ForeColor = System.Drawing.Color.ForestGreen;
            this.lblDanhSachDon.Location = new System.Drawing.Point(382, 9);
            this.lblDanhSachDon.Name = "lblDanhSachDon";
            this.lblDanhSachDon.Size = new System.Drawing.Size(545, 52);
            this.lblDanhSachDon.TabIndex = 1;
            this.lblDanhSachDon.Text = "DANH SÁCH ĐƠN XIN NGHỈ";
            // 
            // grpChiTiet
            // 
            this.grpChiTiet.Controls.Add(this.txtNguoiThayThe);
            this.grpChiTiet.Controls.Add(this.txtMaDon);
            this.grpChiTiet.Controls.Add(this.lblMaDon);
            this.grpChiTiet.Controls.Add(this.btnTuChoi);
            this.grpChiTiet.Controls.Add(this.btnDuyet);
            this.grpChiTiet.Controls.Add(this.txtLyDo);
            this.grpChiTiet.Controls.Add(this.txtTenNV);
            this.grpChiTiet.Controls.Add(this.lblLiDo);
            this.grpChiTiet.Controls.Add(this.lblTenNguoiThayThe);
            this.grpChiTiet.Controls.Add(this.lblTenNguoiNghi);
            this.grpChiTiet.Font = new System.Drawing.Font("Bahnschrift", 19.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpChiTiet.ForeColor = System.Drawing.Color.DarkGreen;
            this.grpChiTiet.Location = new System.Drawing.Point(12, 325);
            this.grpChiTiet.Name = "grpChiTiet";
            this.grpChiTiet.Size = new System.Drawing.Size(1323, 333);
            this.grpChiTiet.TabIndex = 2;
            this.grpChiTiet.TabStop = false;
            this.grpChiTiet.Text = "Thông Tin Chi Tiết Đơn";
            // 
            // txtNguoiThayThe
            // 
            this.txtNguoiThayThe.Font = new System.Drawing.Font("Bahnschrift", 12F);
            this.txtNguoiThayThe.Location = new System.Drawing.Point(844, 102);
            this.txtNguoiThayThe.Name = "txtNguoiThayThe";
            this.txtNguoiThayThe.ReadOnly = true;
            this.txtNguoiThayThe.Size = new System.Drawing.Size(349, 32);
            this.txtNguoiThayThe.TabIndex = 10;
            // 
            // txtMaDon
            // 
            this.txtMaDon.Font = new System.Drawing.Font("Bahnschrift", 12F);
            this.txtMaDon.Location = new System.Drawing.Point(212, 52);
            this.txtMaDon.Name = "txtMaDon";
            this.txtMaDon.ReadOnly = true;
            this.txtMaDon.Size = new System.Drawing.Size(349, 32);
            this.txtMaDon.TabIndex = 9;
            // 
            // lblMaDon
            // 
            this.lblMaDon.AutoSize = true;
            this.lblMaDon.Font = new System.Drawing.Font("Bahnschrift", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMaDon.Location = new System.Drawing.Point(32, 55);
            this.lblMaDon.Name = "lblMaDon";
            this.lblMaDon.Size = new System.Drawing.Size(83, 24);
            this.lblMaDon.TabIndex = 8;
            this.lblMaDon.Text = "Mã Đơn:";
            // 
            // btnTuChoi
            // 
            this.btnTuChoi.BackColor = System.Drawing.Color.Red;
            this.btnTuChoi.Font = new System.Drawing.Font("Bahnschrift", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnTuChoi.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.btnTuChoi.Location = new System.Drawing.Point(900, 228);
            this.btnTuChoi.Name = "btnTuChoi";
            this.btnTuChoi.Size = new System.Drawing.Size(129, 49);
            this.btnTuChoi.TabIndex = 7;
            this.btnTuChoi.Text = "Từ Chối";
            this.btnTuChoi.UseVisualStyleBackColor = false;
            this.btnTuChoi.Click += new System.EventHandler(this.button1_Click);
            // 
            // btnDuyet
            // 
            this.btnDuyet.BackColor = System.Drawing.Color.DarkGreen;
            this.btnDuyet.Font = new System.Drawing.Font("Bahnschrift", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDuyet.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.btnDuyet.Location = new System.Drawing.Point(1128, 228);
            this.btnDuyet.Name = "btnDuyet";
            this.btnDuyet.Size = new System.Drawing.Size(129, 49);
            this.btnDuyet.TabIndex = 6;
            this.btnDuyet.Text = "Đồng Ý";
            this.btnDuyet.UseVisualStyleBackColor = false;
            this.btnDuyet.Click += new System.EventHandler(this.btnDuyet_Click);
            // 
            // txtLyDo
            // 
            this.txtLyDo.Font = new System.Drawing.Font("Bahnschrift", 12F);
            this.txtLyDo.Location = new System.Drawing.Point(212, 152);
            this.txtLyDo.Multiline = true;
            this.txtLyDo.Name = "txtLyDo";
            this.txtLyDo.ReadOnly = true;
            this.txtLyDo.Size = new System.Drawing.Size(561, 175);
            this.txtLyDo.TabIndex = 5;
            // 
            // txtTenNV
            // 
            this.txtTenNV.Font = new System.Drawing.Font("Bahnschrift", 12F);
            this.txtTenNV.Location = new System.Drawing.Point(212, 102);
            this.txtTenNV.Name = "txtTenNV";
            this.txtTenNV.ReadOnly = true;
            this.txtTenNV.Size = new System.Drawing.Size(349, 32);
            this.txtTenNV.TabIndex = 3;
            // 
            // lblLiDo
            // 
            this.lblLiDo.AutoSize = true;
            this.lblLiDo.Font = new System.Drawing.Font("Bahnschrift", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLiDo.Location = new System.Drawing.Point(32, 155);
            this.lblLiDo.Name = "lblLiDo";
            this.lblLiDo.Size = new System.Drawing.Size(121, 24);
            this.lblLiDo.TabIndex = 2;
            this.lblLiDo.Text = "Lí Do Nghỉ:   ";
            // 
            // lblTenNguoiThayThe
            // 
            this.lblTenNguoiThayThe.AutoSize = true;
            this.lblTenNguoiThayThe.Font = new System.Drawing.Font("Bahnschrift", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTenNguoiThayThe.Location = new System.Drawing.Point(621, 110);
            this.lblTenNguoiThayThe.Name = "lblTenNguoiThayThe";
            this.lblTenNguoiThayThe.Size = new System.Drawing.Size(198, 24);
            this.lblTenNguoiThayThe.TabIndex = 1;
            this.lblTenNguoiThayThe.Text = "Tên Người Thay Thế:  ";
            // 
            // lblTenNguoiNghi
            // 
            this.lblTenNguoiNghi.AutoSize = true;
            this.lblTenNguoiNghi.Font = new System.Drawing.Font("Bahnschrift", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTenNguoiNghi.Location = new System.Drawing.Point(32, 105);
            this.lblTenNguoiNghi.Name = "lblTenNguoiNghi";
            this.lblTenNguoiNghi.Size = new System.Drawing.Size(155, 24);
            this.lblTenNguoiNghi.TabIndex = 0;
            this.lblTenNguoiNghi.Text = "Tên Người Nghỉ: ";
            // 
            // lblChiNhanh
            // 
            this.lblChiNhanh.AutoSize = true;
            this.lblChiNhanh.Font = new System.Drawing.Font("Bahnschrift", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblChiNhanh.ForeColor = System.Drawing.Color.DarkGreen;
            this.lblChiNhanh.Location = new System.Drawing.Point(220, 70);
            this.lblChiNhanh.Name = "lblChiNhanh";
            this.lblChiNhanh.Size = new System.Drawing.Size(106, 24);
            this.lblChiNhanh.TabIndex = 3;
            this.lblChiNhanh.Text = "Chi Nhánh:";
            // 
            // cboChiNhanh
            // 
            this.cboChiNhanh.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboChiNhanh.Font = new System.Drawing.Font("Bahnschrift", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cboChiNhanh.FormattingEnabled = true;
            this.cboChiNhanh.Location = new System.Drawing.Point(391, 70);
            this.cboChiNhanh.Name = "cboChiNhanh";
            this.cboChiNhanh.Size = new System.Drawing.Size(537, 32);
            this.cboChiNhanh.TabIndex = 4;
            this.cboChiNhanh.SelectedIndexChanged += new System.EventHandler(this.cboChiNhanh_SelectedIndexChanged);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Cornsilk;
            this.ClientSize = new System.Drawing.Size(1347, 670);
            this.Controls.Add(this.cboChiNhanh);
            this.Controls.Add(this.lblChiNhanh);
            this.Controls.Add(this.grpChiTiet);
            this.Controls.Add(this.lblDanhSachDon);
            this.Controls.Add(this.dgvDanhSachDon);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvDanhSachDon)).EndInit();
            this.grpChiTiet.ResumeLayout(false);
            this.grpChiTiet.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvDanhSachDon;
        private System.Windows.Forms.Label lblDanhSachDon;
        private System.Windows.Forms.GroupBox grpChiTiet;
        private System.Windows.Forms.Label lblChiNhanh;
        private System.Windows.Forms.ComboBox cboChiNhanh;
        private System.Windows.Forms.Label lblTenNguoiThayThe;
        private System.Windows.Forms.Label lblTenNguoiNghi;
        private System.Windows.Forms.Button btnTuChoi;
        private System.Windows.Forms.Button btnDuyet;
        private System.Windows.Forms.TextBox txtLyDo;
        private System.Windows.Forms.TextBox txtTenNV;
        private System.Windows.Forms.Label lblLiDo;
        private System.Windows.Forms.TextBox txtNguoiThayThe;
        private System.Windows.Forms.TextBox txtMaDon;
        private System.Windows.Forms.Label lblMaDon;
        private System.Windows.Forms.DataGridViewTextBoxColumn colMaDon;
        private System.Windows.Forms.DataGridViewTextBoxColumn colMaNV;
        private System.Windows.Forms.DataGridViewTextBoxColumn colNgayNghi;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCaNghi;
        private System.Windows.Forms.DataGridViewTextBoxColumn colMaThayThe;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTrangThai;
    }
}

