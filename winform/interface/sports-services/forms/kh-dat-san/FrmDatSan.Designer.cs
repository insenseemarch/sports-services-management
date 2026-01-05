namespace TTTT
{
    partial class FormDatSan
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
            this.grpTimSan = new System.Windows.Forms.GroupBox();
            this.btnTimSan = new System.Windows.Forms.Button();
            this.dtpGioKetThuc = new System.Windows.Forms.DateTimePicker();
            this.label5 = new System.Windows.Forms.Label();
            this.dtpGioBatDau = new System.Windows.Forms.DateTimePicker();
            this.dtpNgayThue = new System.Windows.Forms.DateTimePicker();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.cboLoaiSan = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cboCoSo = new System.Windows.Forms.ComboBox();
            this.dgvSanTrong = new System.Windows.Forms.DataGridView();
            this.grpTTDS = new System.Windows.Forms.GroupBox();
            this.btnDatSan = new System.Windows.Forms.Button();
            this.cboHinhThucTT = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.lblSanDaChon = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.grpTimSan.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSanTrong)).BeginInit();
            this.grpTTDS.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpTimSan
            // 
            this.grpTimSan.Controls.Add(this.btnTimSan);
            this.grpTimSan.Controls.Add(this.dtpGioKetThuc);
            this.grpTimSan.Controls.Add(this.label5);
            this.grpTimSan.Controls.Add(this.dtpGioBatDau);
            this.grpTimSan.Controls.Add(this.dtpNgayThue);
            this.grpTimSan.Controls.Add(this.label4);
            this.grpTimSan.Controls.Add(this.label3);
            this.grpTimSan.Controls.Add(this.label2);
            this.grpTimSan.Controls.Add(this.cboLoaiSan);
            this.grpTimSan.Controls.Add(this.label1);
            this.grpTimSan.Controls.Add(this.cboCoSo);
            this.grpTimSan.Font = new System.Drawing.Font("Bahnschrift", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpTimSan.Location = new System.Drawing.Point(85, 141);
            this.grpTimSan.Name = "grpTimSan";
            this.grpTimSan.Size = new System.Drawing.Size(432, 496);
            this.grpTimSan.TabIndex = 0;
            this.grpTimSan.TabStop = false;
            this.grpTimSan.Text = "Tìm sân trống";
            // 
            // btnTimSan
            // 
            this.btnTimSan.BackColor = System.Drawing.Color.ForestGreen;
            this.btnTimSan.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnTimSan.Font = new System.Drawing.Font("Bahnschrift", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnTimSan.ForeColor = System.Drawing.Color.Transparent;
            this.btnTimSan.Location = new System.Drawing.Point(150, 427);
            this.btnTimSan.Name = "btnTimSan";
            this.btnTimSan.Size = new System.Drawing.Size(116, 45);
            this.btnTimSan.TabIndex = 13;
            this.btnTimSan.Text = "Tìm sân";
            this.btnTimSan.UseVisualStyleBackColor = false;
            // 
            // dtpGioKetThuc
            // 
            this.dtpGioKetThuc.CustomFormat = "HH:mm";
            this.dtpGioKetThuc.Font = new System.Drawing.Font("Bahnschrift", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dtpGioKetThuc.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpGioKetThuc.Location = new System.Drawing.Point(166, 343);
            this.dtpGioKetThuc.Name = "dtpGioKetThuc";
            this.dtpGioKetThuc.ShowUpDown = true;
            this.dtpGioKetThuc.Size = new System.Drawing.Size(92, 32);
            this.dtpGioKetThuc.TabIndex = 12;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.BackColor = System.Drawing.Color.Transparent;
            this.label5.Font = new System.Drawing.Font("Bahnschrift SemiBold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(40, 349);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(122, 24);
            this.label5.TabIndex = 11;
            this.label5.Text = "Giờ kết thúc:";
            // 
            // dtpGioBatDau
            // 
            this.dtpGioBatDau.CustomFormat = "HH:mm";
            this.dtpGioBatDau.Font = new System.Drawing.Font("Bahnschrift", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dtpGioBatDau.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpGioBatDau.Location = new System.Drawing.Point(166, 273);
            this.dtpGioBatDau.Name = "dtpGioBatDau";
            this.dtpGioBatDau.ShowUpDown = true;
            this.dtpGioBatDau.Size = new System.Drawing.Size(92, 32);
            this.dtpGioBatDau.TabIndex = 10;
            // 
            // dtpNgayThue
            // 
            this.dtpNgayThue.Font = new System.Drawing.Font("Bahnschrift", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dtpNgayThue.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpNgayThue.Location = new System.Drawing.Point(166, 200);
            this.dtpNgayThue.Name = "dtpNgayThue";
            this.dtpNgayThue.Size = new System.Drawing.Size(145, 32);
            this.dtpNgayThue.TabIndex = 8;
            this.dtpNgayThue.ValueChanged += new System.EventHandler(this.dateTimePicker1_ValueChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.BackColor = System.Drawing.Color.Transparent;
            this.label4.Font = new System.Drawing.Font("Bahnschrift SemiBold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(40, 280);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(116, 24);
            this.label4.TabIndex = 7;
            this.label4.Text = "Giờ bắt đầu:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.Font = new System.Drawing.Font("Bahnschrift SemiBold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(40, 207);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(106, 24);
            this.label3.TabIndex = 5;
            this.label3.Text = "Ngày thuê:";
            this.label3.Click += new System.EventHandler(this.label3_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Font = new System.Drawing.Font("Bahnschrift SemiBold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(40, 131);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(92, 24);
            this.label2.TabIndex = 3;
            this.label2.Text = "Loại Sân:";
            // 
            // cboLoaiSan
            // 
            this.cboLoaiSan.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboLoaiSan.Font = new System.Drawing.Font("Bahnschrift", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cboLoaiSan.FormattingEnabled = true;
            this.cboLoaiSan.Location = new System.Drawing.Point(142, 128);
            this.cboLoaiSan.Name = "cboLoaiSan";
            this.cboLoaiSan.Size = new System.Drawing.Size(225, 32);
            this.cboLoaiSan.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("Bahnschrift SemiBold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(40, 59);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(69, 24);
            this.label1.TabIndex = 1;
            this.label1.Text = "Cơ sở: ";
            // 
            // cboCoSo
            // 
            this.cboCoSo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboCoSo.Font = new System.Drawing.Font("Bahnschrift", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cboCoSo.FormattingEnabled = true;
            this.cboCoSo.Location = new System.Drawing.Point(114, 56);
            this.cboCoSo.Name = "cboCoSo";
            this.cboCoSo.Size = new System.Drawing.Size(253, 32);
            this.cboCoSo.TabIndex = 0;
            // 
            // dgvSanTrong
            // 
            this.dgvSanTrong.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dgvSanTrong.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvSanTrong.Location = new System.Drawing.Point(544, 155);
            this.dgvSanTrong.Name = "dgvSanTrong";
            this.dgvSanTrong.ReadOnly = true;
            this.dgvSanTrong.RowHeadersWidth = 51;
            this.dgvSanTrong.RowTemplate.Height = 24;
            this.dgvSanTrong.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvSanTrong.Size = new System.Drawing.Size(519, 252);
            this.dgvSanTrong.TabIndex = 1;
            this.dgvSanTrong.Tag = "";
            // 
            // grpTTDS
            // 
            this.grpTTDS.Controls.Add(this.btnDatSan);
            this.grpTTDS.Controls.Add(this.cboHinhThucTT);
            this.grpTTDS.Controls.Add(this.label7);
            this.grpTTDS.Controls.Add(this.lblSanDaChon);
            this.grpTTDS.Controls.Add(this.label6);
            this.grpTTDS.Font = new System.Drawing.Font("Bahnschrift", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpTTDS.Location = new System.Drawing.Point(544, 421);
            this.grpTTDS.Name = "grpTTDS";
            this.grpTTDS.Size = new System.Drawing.Size(519, 216);
            this.grpTTDS.TabIndex = 2;
            this.grpTTDS.TabStop = false;
            this.grpTTDS.Text = "Thông tin đặt sân";
            // 
            // btnDatSan
            // 
            this.btnDatSan.BackColor = System.Drawing.Color.ForestGreen;
            this.btnDatSan.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDatSan.Font = new System.Drawing.Font("Bahnschrift", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDatSan.ForeColor = System.Drawing.Color.Transparent;
            this.btnDatSan.Location = new System.Drawing.Point(203, 147);
            this.btnDatSan.Name = "btnDatSan";
            this.btnDatSan.Size = new System.Drawing.Size(137, 45);
            this.btnDatSan.TabIndex = 14;
            this.btnDatSan.Text = "Đặt sân";
            this.btnDatSan.UseVisualStyleBackColor = false;
            // 
            // cboHinhThucTT
            // 
            this.cboHinhThucTT.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboHinhThucTT.Font = new System.Drawing.Font("Bahnschrift", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cboHinhThucTT.FormattingEnabled = true;
            this.cboHinhThucTT.Location = new System.Drawing.Point(236, 93);
            this.cboHinhThucTT.Name = "cboHinhThucTT";
            this.cboHinhThucTT.Size = new System.Drawing.Size(238, 32);
            this.cboHinhThucTT.TabIndex = 14;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.BackColor = System.Drawing.Color.Transparent;
            this.label7.Font = new System.Drawing.Font("Bahnschrift SemiBold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(24, 96);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(202, 24);
            this.label7.TabIndex = 16;
            this.label7.Text = "Hình thức thanh toán:";
            // 
            // lblSanDaChon
            // 
            this.lblSanDaChon.AutoSize = true;
            this.lblSanDaChon.Font = new System.Drawing.Font("Bahnschrift", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSanDaChon.Location = new System.Drawing.Point(159, 49);
            this.lblSanDaChon.Name = "lblSanDaChon";
            this.lblSanDaChon.Size = new System.Drawing.Size(104, 24);
            this.lblSanDaChon.TabIndex = 15;
            this.lblSanDaChon.Text = "Chưa chọn";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.BackColor = System.Drawing.Color.Transparent;
            this.label6.Font = new System.Drawing.Font("Bahnschrift SemiBold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(24, 49);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(124, 24);
            this.label6.TabIndex = 14;
            this.label6.Text = "Sân đã chọn:";
            // 
            // label8
            // 
            this.label8.Font = new System.Drawing.Font("Bahnschrift", 25.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.ForeColor = System.Drawing.Color.ForestGreen;
            this.label8.Location = new System.Drawing.Point(433, 36);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(301, 58);
            this.label8.TabIndex = 0;
            this.label8.Text = "Đặt Sân";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // FormDatSan
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Cornsilk;
            this.ClientSize = new System.Drawing.Size(1157, 748);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.grpTTDS);
            this.Controls.Add(this.dgvSanTrong);
            this.Controls.Add(this.grpTimSan);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "FormDatSan";
            this.Text = "FormDatSan";
            this.Load += new System.EventHandler(this.FormDatSan_Load);
            this.grpTimSan.ResumeLayout(false);
            this.grpTimSan.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSanTrong)).EndInit();
            this.grpTTDS.ResumeLayout(false);
            this.grpTTDS.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox grpTimSan;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cboCoSo;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cboLoaiSan;
        private System.Windows.Forms.DateTimePicker dtpNgayThue;
        private System.Windows.Forms.DateTimePicker dtpGioBatDau;
        private System.Windows.Forms.Button btnTimSan;
        private System.Windows.Forms.DateTimePicker dtpGioKetThuc;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.DataGridView dgvSanTrong;
        private System.Windows.Forms.GroupBox grpTTDS;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label lblSanDaChon;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button btnDatSan;
        private System.Windows.Forms.ComboBox cboHinhThucTT;
        private System.Windows.Forms.Label label8;
    }
}