namespace TTTT
{
    partial class FormLichLamViec
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
            this.label1 = new System.Windows.Forms.Label();
            this.dtpThang = new System.Windows.Forms.DateTimePicker();
            this.tblCalendar = new System.Windows.Forms.TableLayoutPanel();
            this.lblSat = new System.Windows.Forms.Label();
            this.lblFri = new System.Windows.Forms.Label();
            this.lblThu = new System.Windows.Forms.Label();
            this.lblWed = new System.Windows.Forms.Label();
            this.lblTue = new System.Windows.Forms.Label();
            this.lblMon = new System.Windows.Forms.Label();
            this.lblSun = new System.Windows.Forms.Label();
            this.grpLichLamViec = new System.Windows.Forms.GroupBox();
            this.grpXinNghiPhep = new System.Windows.Forms.GroupBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtNhanVienThayThe = new System.Windows.Forms.TextBox();
            this.btnGuiDon = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.txtLyDo = new System.Windows.Forms.TextBox();
            this.dtpNghiDen = new System.Windows.Forms.DateTimePicker();
            this.dtpNghiTu = new System.Windows.Forms.DateTimePicker();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.chkXinNghi = new System.Windows.Forms.CheckBox();
            this.grpDonNghi = new System.Windows.Forms.GroupBox();
            this.dgvDonDaGui = new System.Windows.Forms.DataGridView();
            this.btnTroVe = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.tblCalendar.SuspendLayout();
            this.grpLichLamViec.SuspendLayout();
            this.grpXinNghiPhep.SuspendLayout();
            this.grpDonNghi.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDonDaGui)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Bahnschrift", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(80, 74);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(69, 24);
            this.label1.TabIndex = 0;
            this.label1.Text = "Tháng:";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // dtpThang
            // 
            this.dtpThang.CustomFormat = "MM/yyyy";
            this.dtpThang.Font = new System.Drawing.Font("Bahnschrift", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dtpThang.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpThang.Location = new System.Drawing.Point(161, 74);
            this.dtpThang.Name = "dtpThang";
            this.dtpThang.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.dtpThang.Size = new System.Drawing.Size(200, 28);
            this.dtpThang.TabIndex = 1;
            this.dtpThang.ValueChanged += new System.EventHandler(this.dtpThang_ValueChanged);
            // 
            // tblCalendar
            // 
            this.tblCalendar.ColumnCount = 7;
            this.tblCalendar.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 14.28571F));
            this.tblCalendar.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 14.28571F));
            this.tblCalendar.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 14.28571F));
            this.tblCalendar.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 14.28571F));
            this.tblCalendar.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 14.28571F));
            this.tblCalendar.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 14.28571F));
            this.tblCalendar.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 14.28571F));
            this.tblCalendar.Controls.Add(this.lblSat, 6, 0);
            this.tblCalendar.Controls.Add(this.lblFri, 5, 0);
            this.tblCalendar.Controls.Add(this.lblThu, 4, 0);
            this.tblCalendar.Controls.Add(this.lblWed, 3, 0);
            this.tblCalendar.Controls.Add(this.lblTue, 2, 0);
            this.tblCalendar.Controls.Add(this.lblMon, 1, 0);
            this.tblCalendar.Controls.Add(this.lblSun, 0, 0);
            this.tblCalendar.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tblCalendar.Font = new System.Drawing.Font("Bahnschrift", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tblCalendar.Location = new System.Drawing.Point(3, 28);
            this.tblCalendar.Name = "tblCalendar";
            this.tblCalendar.RowCount = 7;
            this.tblCalendar.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28571F));
            this.tblCalendar.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28571F));
            this.tblCalendar.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28571F));
            this.tblCalendar.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28571F));
            this.tblCalendar.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28571F));
            this.tblCalendar.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28571F));
            this.tblCalendar.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28571F));
            this.tblCalendar.Size = new System.Drawing.Size(962, 360);
            this.tblCalendar.TabIndex = 2;
            this.tblCalendar.Paint += new System.Windows.Forms.PaintEventHandler(this.tblCalendar_Paint);
            // 
            // lblSat
            // 
            this.lblSat.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblSat.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblSat.Font = new System.Drawing.Font("Bahnschrift", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSat.Location = new System.Drawing.Point(822, 0);
            this.lblSat.Margin = new System.Windows.Forms.Padding(0);
            this.lblSat.Name = "lblSat";
            this.lblSat.Size = new System.Drawing.Size(140, 51);
            this.lblSat.TabIndex = 6;
            this.lblSat.Text = "Saturday";
            this.lblSat.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblFri
            // 
            this.lblFri.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblFri.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblFri.Font = new System.Drawing.Font("Bahnschrift", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFri.Location = new System.Drawing.Point(685, 0);
            this.lblFri.Margin = new System.Windows.Forms.Padding(0);
            this.lblFri.Name = "lblFri";
            this.lblFri.Size = new System.Drawing.Size(137, 51);
            this.lblFri.TabIndex = 5;
            this.lblFri.Text = "Friday";
            this.lblFri.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblThu
            // 
            this.lblThu.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblThu.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblThu.Font = new System.Drawing.Font("Bahnschrift", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblThu.Location = new System.Drawing.Point(548, 0);
            this.lblThu.Margin = new System.Windows.Forms.Padding(0);
            this.lblThu.Name = "lblThu";
            this.lblThu.Size = new System.Drawing.Size(137, 51);
            this.lblThu.TabIndex = 4;
            this.lblThu.Text = "Thursday";
            this.lblThu.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblWed
            // 
            this.lblWed.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblWed.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblWed.Font = new System.Drawing.Font("Bahnschrift", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblWed.Location = new System.Drawing.Point(411, 0);
            this.lblWed.Margin = new System.Windows.Forms.Padding(0);
            this.lblWed.Name = "lblWed";
            this.lblWed.Size = new System.Drawing.Size(137, 51);
            this.lblWed.TabIndex = 3;
            this.lblWed.Text = "Wednesday";
            this.lblWed.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblTue
            // 
            this.lblTue.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblTue.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblTue.Font = new System.Drawing.Font("Bahnschrift", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTue.Location = new System.Drawing.Point(274, 0);
            this.lblTue.Margin = new System.Windows.Forms.Padding(0);
            this.lblTue.Name = "lblTue";
            this.lblTue.Size = new System.Drawing.Size(137, 51);
            this.lblTue.TabIndex = 2;
            this.lblTue.Text = "Tuesday";
            this.lblTue.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblMon
            // 
            this.lblMon.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblMon.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblMon.Font = new System.Drawing.Font("Bahnschrift", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMon.Location = new System.Drawing.Point(137, 0);
            this.lblMon.Margin = new System.Windows.Forms.Padding(0);
            this.lblMon.Name = "lblMon";
            this.lblMon.Size = new System.Drawing.Size(137, 51);
            this.lblMon.TabIndex = 1;
            this.lblMon.Text = "Monday";
            this.lblMon.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblSun
            // 
            this.lblSun.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblSun.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblSun.Font = new System.Drawing.Font("Bahnschrift", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSun.Location = new System.Drawing.Point(0, 0);
            this.lblSun.Margin = new System.Windows.Forms.Padding(0);
            this.lblSun.Name = "lblSun";
            this.lblSun.Size = new System.Drawing.Size(137, 51);
            this.lblSun.TabIndex = 0;
            this.lblSun.Text = "Sunday";
            this.lblSun.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblSun.Click += new System.EventHandler(this.lblSun_Click);
            // 
            // grpLichLamViec
            // 
            this.grpLichLamViec.Controls.Add(this.tblCalendar);
            this.grpLichLamViec.Font = new System.Drawing.Font("Bahnschrift", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpLichLamViec.Location = new System.Drawing.Point(84, 105);
            this.grpLichLamViec.Name = "grpLichLamViec";
            this.grpLichLamViec.Size = new System.Drawing.Size(968, 391);
            this.grpLichLamViec.TabIndex = 2;
            this.grpLichLamViec.TabStop = false;
            this.grpLichLamViec.Text = "Lịch làm việc trong tháng";
            // 
            // grpXinNghiPhep
            // 
            this.grpXinNghiPhep.Controls.Add(this.label5);
            this.grpXinNghiPhep.Controls.Add(this.txtNhanVienThayThe);
            this.grpXinNghiPhep.Controls.Add(this.btnGuiDon);
            this.grpXinNghiPhep.Controls.Add(this.label4);
            this.grpXinNghiPhep.Controls.Add(this.txtLyDo);
            this.grpXinNghiPhep.Controls.Add(this.dtpNghiDen);
            this.grpXinNghiPhep.Controls.Add(this.dtpNghiTu);
            this.grpXinNghiPhep.Controls.Add(this.label3);
            this.grpXinNghiPhep.Controls.Add(this.label2);
            this.grpXinNghiPhep.Font = new System.Drawing.Font("Bahnschrift", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpXinNghiPhep.Location = new System.Drawing.Point(84, 530);
            this.grpXinNghiPhep.Name = "grpXinNghiPhep";
            this.grpXinNghiPhep.Size = new System.Drawing.Size(543, 185);
            this.grpXinNghiPhep.TabIndex = 3;
            this.grpXinNghiPhep.TabStop = false;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Bahnschrift SemiBold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(23, 61);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(182, 24);
            this.label5.TabIndex = 8;
            this.label5.Text = "Nhân viên thay thế:";
            // 
            // txtNhanVienThayThe
            // 
            this.txtNhanVienThayThe.Location = new System.Drawing.Point(211, 58);
            this.txtNhanVienThayThe.Name = "txtNhanVienThayThe";
            this.txtNhanVienThayThe.Size = new System.Drawing.Size(154, 28);
            this.txtNhanVienThayThe.TabIndex = 7;
            // 
            // btnGuiDon
            // 
            this.btnGuiDon.BackColor = System.Drawing.Color.ForestGreen;
            this.btnGuiDon.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnGuiDon.Font = new System.Drawing.Font("Bahnschrift", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnGuiDon.ForeColor = System.Drawing.Color.Transparent;
            this.btnGuiDon.Location = new System.Drawing.Point(225, 144);
            this.btnGuiDon.Name = "btnGuiDon";
            this.btnGuiDon.Size = new System.Drawing.Size(85, 35);
            this.btnGuiDon.TabIndex = 6;
            this.btnGuiDon.Text = "Gửi Đơn";
            this.btnGuiDon.UseVisualStyleBackColor = false;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Bahnschrift SemiBold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(23, 96);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(63, 24);
            this.label4.TabIndex = 5;
            this.label4.Text = "Lý do:";
            // 
            // txtLyDo
            // 
            this.txtLyDo.Location = new System.Drawing.Point(92, 96);
            this.txtLyDo.Multiline = true;
            this.txtLyDo.Name = "txtLyDo";
            this.txtLyDo.Size = new System.Drawing.Size(407, 40);
            this.txtLyDo.TabIndex = 4;
            this.txtLyDo.TextChanged += new System.EventHandler(this.txtLyDo_TextChanged);
            // 
            // dtpNghiDen
            // 
            this.dtpNghiDen.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpNghiDen.Location = new System.Drawing.Point(408, 21);
            this.dtpNghiDen.Name = "dtpNghiDen";
            this.dtpNghiDen.Size = new System.Drawing.Size(129, 28);
            this.dtpNghiDen.TabIndex = 3;
            // 
            // dtpNghiTu
            // 
            this.dtpNghiTu.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpNghiTu.Location = new System.Drawing.Point(156, 24);
            this.dtpNghiTu.Name = "dtpNghiTu";
            this.dtpNghiTu.Size = new System.Drawing.Size(129, 28);
            this.dtpNghiTu.TabIndex = 2;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Bahnschrift SemiBold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(291, 24);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(98, 24);
            this.label3.TabIndex = 1;
            this.label3.Text = "Đến ngày:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Bahnschrift SemiBold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(23, 24);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(127, 24);
            this.label2.TabIndex = 0;
            this.label2.Text = "Nghỉ từ ngày:";
            // 
            // chkXinNghi
            // 
            this.chkXinNghi.AutoSize = true;
            this.chkXinNghi.Font = new System.Drawing.Font("Bahnschrift", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkXinNghi.Location = new System.Drawing.Point(84, 502);
            this.chkXinNghi.Name = "chkXinNghi";
            this.chkXinNghi.Size = new System.Drawing.Size(151, 28);
            this.chkXinNghi.TabIndex = 4;
            this.chkXinNghi.Text = "Xin nghỉ phép";
            this.chkXinNghi.UseVisualStyleBackColor = true;
            this.chkXinNghi.CheckedChanged += new System.EventHandler(this.chkXinNghi_CheckedChanged);
            // 
            // grpDonNghi
            // 
            this.grpDonNghi.Controls.Add(this.dgvDonDaGui);
            this.grpDonNghi.Font = new System.Drawing.Font("Bahnschrift", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpDonNghi.Location = new System.Drawing.Point(633, 518);
            this.grpDonNghi.Name = "grpDonNghi";
            this.grpDonNghi.Size = new System.Drawing.Size(416, 188);
            this.grpDonNghi.TabIndex = 7;
            this.grpDonNghi.TabStop = false;
            this.grpDonNghi.Text = "Đơn đã gửi";
            // 
            // dgvDonDaGui
            // 
            this.dgvDonDaGui.AllowUserToAddRows = false;
            this.dgvDonDaGui.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvDonDaGui.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvDonDaGui.Location = new System.Drawing.Point(3, 28);
            this.dgvDonDaGui.Name = "dgvDonDaGui";
            this.dgvDonDaGui.ReadOnly = true;
            this.dgvDonDaGui.RowHeadersWidth = 51;
            this.dgvDonDaGui.RowTemplate.Height = 24;
            this.dgvDonDaGui.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvDonDaGui.Size = new System.Drawing.Size(410, 157);
            this.dgvDonDaGui.TabIndex = 0;
            // 
            // btnTroVe
            // 
            this.btnTroVe.BackColor = System.Drawing.Color.ForestGreen;
            this.btnTroVe.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnTroVe.Font = new System.Drawing.Font("Bahnschrift", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnTroVe.ForeColor = System.Drawing.Color.Transparent;
            this.btnTroVe.Location = new System.Drawing.Point(965, 66);
            this.btnTroVe.Name = "btnTroVe";
            this.btnTroVe.Size = new System.Drawing.Size(87, 38);
            this.btnTroVe.TabIndex = 8;
            this.btnTroVe.Text = "Trở Về";
            this.btnTroVe.UseVisualStyleBackColor = false;
            this.btnTroVe.Click += new System.EventHandler(this.btnTroVe_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.BackColor = System.Drawing.Color.Cornsilk;
            this.label6.Font = new System.Drawing.Font("Bahnschrift", 25.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.ForeColor = System.Drawing.Color.ForestGreen;
            this.label6.Location = new System.Drawing.Point(460, 9);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(299, 53);
            this.label6.TabIndex = 9;
            this.label6.Text = "Lịch Làm Việc";
            // 
            // FormLichLamViec
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Cornsilk;
            this.ClientSize = new System.Drawing.Size(1157, 748);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.btnTroVe);
            this.Controls.Add(this.grpDonNghi);
            this.Controls.Add(this.chkXinNghi);
            this.Controls.Add(this.grpXinNghiPhep);
            this.Controls.Add(this.grpLichLamViec);
            this.Controls.Add(this.dtpThang);
            this.Controls.Add(this.label1);
            this.Name = "FormLichLamViec";
            this.Text = "FormLichLamViec";
            this.Load += new System.EventHandler(this.FormLichLamViec_Load);
            this.tblCalendar.ResumeLayout(false);
            this.grpLichLamViec.ResumeLayout(false);
            this.grpXinNghiPhep.ResumeLayout(false);
            this.grpXinNghiPhep.PerformLayout();
            this.grpDonNghi.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvDonDaGui)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DateTimePicker dtpThang;
        private System.Windows.Forms.TableLayoutPanel tblCalendar;
        private System.Windows.Forms.Label lblSat;
        private System.Windows.Forms.Label lblFri;
        private System.Windows.Forms.Label lblThu;
        private System.Windows.Forms.Label lblWed;
        private System.Windows.Forms.Label lblTue;
        private System.Windows.Forms.Label lblMon;
        private System.Windows.Forms.Label lblSun;
        private System.Windows.Forms.GroupBox grpLichLamViec;
        private System.Windows.Forms.GroupBox grpXinNghiPhep;
        private System.Windows.Forms.CheckBox chkXinNghi;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnGuiDon;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtLyDo;
        private System.Windows.Forms.DateTimePicker dtpNghiDen;
        private System.Windows.Forms.DateTimePicker dtpNghiTu;
        private System.Windows.Forms.GroupBox grpDonNghi;
        private System.Windows.Forms.DataGridView dgvDonDaGui;
        private System.Windows.Forms.Button btnTroVe;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtNhanVienThayThe;
        private System.Windows.Forms.Label label6;
    }
}