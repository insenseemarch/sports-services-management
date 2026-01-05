namespace ReportDoanhThu
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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea2 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend2 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.tlpMain = new System.Windows.Forms.TableLayoutPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.dtpDenNgay = new System.Windows.Forms.DateTimePicker();
            this.dtpTuNgay = new System.Windows.Forms.DateTimePicker();
            this.btnXem = new System.Windows.Forms.Button();
            this.lblDenNgay = new System.Windows.Forms.Label();
            this.lblTuNgay = new System.Windows.Forms.Label();
            this.tlpKPI = new System.Windows.Forms.TableLayoutPanel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.lblTongDoanhThu = new System.Windows.Forms.Label();
            this.lblDoanhThu = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.lblSoLuotDat = new System.Windows.Forms.Label();
            this.lblSoDat = new System.Windows.Forms.Label();
            this.panel4 = new System.Windows.Forms.Panel();
            this.lblTyLeLapDay = new System.Windows.Forms.Label();
            this.lblTyLe = new System.Windows.Forms.Label();
            this.panel5 = new System.Windows.Forms.Panel();
            this.lblTienMat = new System.Windows.Forms.Label();
            this.lblTienMatDH = new System.Windows.Forms.Label();
            this.tlpCharts = new System.Windows.Forms.TableLayoutPanel();
            this.chartDoanhThu = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.chartTyLe = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.dgvChiTiet = new System.Windows.Forms.DataGridView();
            this.lblBaoCao = new System.Windows.Forms.Label();
            this.cboCoSo = new System.Windows.Forms.ComboBox();
            this.lblCoSo = new System.Windows.Forms.Label();
            this.tlpMain.SuspendLayout();
            this.panel1.SuspendLayout();
            this.tlpKPI.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel4.SuspendLayout();
            this.panel5.SuspendLayout();
            this.tlpCharts.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chartDoanhThu)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartTyLe)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvChiTiet)).BeginInit();
            this.SuspendLayout();
            // 
            // tlpMain
            // 
            this.tlpMain.ColumnCount = 1;
            this.tlpMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpMain.Controls.Add(this.panel1, 0, 0);
            this.tlpMain.Controls.Add(this.tlpKPI, 0, 1);
            this.tlpMain.Controls.Add(this.tlpCharts, 0, 2);
            this.tlpMain.Controls.Add(this.dgvChiTiet, 0, 3);
            this.tlpMain.Location = new System.Drawing.Point(12, 104);
            this.tlpMain.Name = "tlpMain";
            this.tlpMain.RowCount = 4;
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 120F));
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpMain.Size = new System.Drawing.Size(1465, 665);
            this.tlpMain.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.dtpDenNgay);
            this.panel1.Controls.Add(this.dtpTuNgay);
            this.panel1.Controls.Add(this.btnXem);
            this.panel1.Controls.Add(this.lblDenNgay);
            this.panel1.Controls.Add(this.lblTuNgay);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Font = new System.Drawing.Font("Bahnschrift", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1465, 60);
            this.panel1.TabIndex = 0;
            // 
            // dtpDenNgay
            // 
            this.dtpDenNgay.Font = new System.Drawing.Font("Bahnschrift", 13.8F);
            this.dtpDenNgay.Location = new System.Drawing.Point(734, 15);
            this.dtpDenNgay.Name = "dtpDenNgay";
            this.dtpDenNgay.Size = new System.Drawing.Size(358, 35);
            this.dtpDenNgay.TabIndex = 4;
            // 
            // dtpTuNgay
            // 
            this.dtpTuNgay.Font = new System.Drawing.Font("Bahnschrift", 13.8F);
            this.dtpTuNgay.Location = new System.Drawing.Point(160, 16);
            this.dtpTuNgay.Name = "dtpTuNgay";
            this.dtpTuNgay.Size = new System.Drawing.Size(399, 35);
            this.dtpTuNgay.TabIndex = 3;
            // 
            // btnXem
            // 
            this.btnXem.BackColor = System.Drawing.Color.DarkGreen;
            this.btnXem.Font = new System.Drawing.Font("Bahnschrift", 14F, System.Drawing.FontStyle.Bold);
            this.btnXem.ForeColor = System.Drawing.Color.Ivory;
            this.btnXem.Location = new System.Drawing.Point(1195, 12);
            this.btnXem.Name = "btnXem";
            this.btnXem.Size = new System.Drawing.Size(183, 42);
            this.btnXem.TabIndex = 2;
            this.btnXem.Text = "Xem Báo Cáo";
            this.btnXem.UseVisualStyleBackColor = false;
            this.btnXem.Click += new System.EventHandler(this.btnXem_Click);
            // 
            // lblDenNgay
            // 
            this.lblDenNgay.AutoSize = true;
            this.lblDenNgay.Font = new System.Drawing.Font("Bahnschrift", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDenNgay.ForeColor = System.Drawing.Color.Green;
            this.lblDenNgay.Location = new System.Drawing.Point(608, 19);
            this.lblDenNgay.Name = "lblDenNgay";
            this.lblDenNgay.Size = new System.Drawing.Size(120, 28);
            this.lblDenNgay.TabIndex = 1;
            this.lblDenNgay.Text = "Đến Ngày: ";
            // 
            // lblTuNgay
            // 
            this.lblTuNgay.AutoSize = true;
            this.lblTuNgay.Font = new System.Drawing.Font("Bahnschrift", 14F, System.Drawing.FontStyle.Bold);
            this.lblTuNgay.ForeColor = System.Drawing.Color.Green;
            this.lblTuNgay.Location = new System.Drawing.Point(44, 19);
            this.lblTuNgay.Name = "lblTuNgay";
            this.lblTuNgay.Size = new System.Drawing.Size(110, 29);
            this.lblTuNgay.TabIndex = 0;
            this.lblTuNgay.Text = "Từ Ngày: ";
            // 
            // tlpKPI
            // 
            this.tlpKPI.ColumnCount = 4;
            this.tlpKPI.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tlpKPI.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tlpKPI.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tlpKPI.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tlpKPI.Controls.Add(this.panel2, 0, 0);
            this.tlpKPI.Controls.Add(this.panel3, 1, 0);
            this.tlpKPI.Controls.Add(this.panel4, 2, 0);
            this.tlpKPI.Controls.Add(this.panel5, 3, 0);
            this.tlpKPI.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpKPI.Location = new System.Drawing.Point(3, 63);
            this.tlpKPI.Name = "tlpKPI";
            this.tlpKPI.RowCount = 1;
            this.tlpKPI.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpKPI.Size = new System.Drawing.Size(1459, 114);
            this.tlpKPI.TabIndex = 1;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.SeaGreen;
            this.panel2.Controls.Add(this.lblTongDoanhThu);
            this.panel2.Controls.Add(this.lblDoanhThu);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(5, 5);
            this.panel2.Margin = new System.Windows.Forms.Padding(5);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(354, 104);
            this.panel2.TabIndex = 0;
            // 
            // lblTongDoanhThu
            // 
            this.lblTongDoanhThu.AutoSize = true;
            this.lblTongDoanhThu.Font = new System.Drawing.Font("Bahnschrift", 13.8F);
            this.lblTongDoanhThu.Location = new System.Drawing.Point(91, 54);
            this.lblTongDoanhThu.Name = "lblTongDoanhThu";
            this.lblTongDoanhThu.Size = new System.Drawing.Size(71, 28);
            this.lblTongDoanhThu.TabIndex = 1;
            this.lblTongDoanhThu.Text = "label1";
            // 
            // lblDoanhThu
            // 
            this.lblDoanhThu.AutoSize = true;
            this.lblDoanhThu.Font = new System.Drawing.Font("Bahnschrift", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDoanhThu.Location = new System.Drawing.Point(3, 9);
            this.lblDoanhThu.Name = "lblDoanhThu";
            this.lblDoanhThu.Size = new System.Drawing.Size(146, 28);
            this.lblDoanhThu.TabIndex = 0;
            this.lblDoanhThu.Text = "DOANH THU: ";
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.RoyalBlue;
            this.panel3.Controls.Add(this.lblSoLuotDat);
            this.panel3.Controls.Add(this.lblSoDat);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(369, 5);
            this.panel3.Margin = new System.Windows.Forms.Padding(5);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(354, 104);
            this.panel3.TabIndex = 1;
            // 
            // lblSoLuotDat
            // 
            this.lblSoLuotDat.AutoSize = true;
            this.lblSoLuotDat.Font = new System.Drawing.Font("Bahnschrift", 13.8F);
            this.lblSoLuotDat.Location = new System.Drawing.Point(129, 54);
            this.lblSoLuotDat.Name = "lblSoLuotDat";
            this.lblSoLuotDat.Size = new System.Drawing.Size(71, 28);
            this.lblSoLuotDat.TabIndex = 2;
            this.lblSoLuotDat.Text = "label1";
            // 
            // lblSoDat
            // 
            this.lblSoDat.AutoSize = true;
            this.lblSoDat.Font = new System.Drawing.Font("Bahnschrift", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSoDat.Location = new System.Drawing.Point(3, 9);
            this.lblSoDat.Name = "lblSoDat";
            this.lblSoDat.Size = new System.Drawing.Size(99, 28);
            this.lblSoDat.TabIndex = 1;
            this.lblSoDat.Text = "SỐ ĐẶT: ";
            // 
            // panel4
            // 
            this.panel4.BackColor = System.Drawing.Color.DarkOrange;
            this.panel4.Controls.Add(this.lblTyLeLapDay);
            this.panel4.Controls.Add(this.lblTyLe);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel4.Location = new System.Drawing.Point(733, 5);
            this.panel4.Margin = new System.Windows.Forms.Padding(5);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(354, 104);
            this.panel4.TabIndex = 2;
            // 
            // lblTyLeLapDay
            // 
            this.lblTyLeLapDay.AutoSize = true;
            this.lblTyLeLapDay.Font = new System.Drawing.Font("Bahnschrift", 13.8F);
            this.lblTyLeLapDay.Location = new System.Drawing.Point(148, 54);
            this.lblTyLeLapDay.Name = "lblTyLeLapDay";
            this.lblTyLeLapDay.Size = new System.Drawing.Size(71, 28);
            this.lblTyLeLapDay.TabIndex = 3;
            this.lblTyLeLapDay.Text = "label1";
            // 
            // lblTyLe
            // 
            this.lblTyLe.AutoSize = true;
            this.lblTyLe.Font = new System.Drawing.Font("Bahnschrift", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTyLe.Location = new System.Drawing.Point(3, 9);
            this.lblTyLe.Name = "lblTyLe";
            this.lblTyLe.Size = new System.Drawing.Size(174, 28);
            this.lblTyLe.TabIndex = 2;
            this.lblTyLe.Text = "TỶ LỆ LẤP ĐẦY: ";
            // 
            // panel5
            // 
            this.panel5.BackColor = System.Drawing.Color.Crimson;
            this.panel5.Controls.Add(this.lblTienMat);
            this.panel5.Controls.Add(this.lblTienMatDH);
            this.panel5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel5.Location = new System.Drawing.Point(1097, 5);
            this.panel5.Margin = new System.Windows.Forms.Padding(5);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(357, 104);
            this.panel5.TabIndex = 3;
            // 
            // lblTienMat
            // 
            this.lblTienMat.AutoSize = true;
            this.lblTienMat.Font = new System.Drawing.Font("Bahnschrift", 13.8F);
            this.lblTienMat.Location = new System.Drawing.Point(166, 54);
            this.lblTienMat.Name = "lblTienMat";
            this.lblTienMat.Size = new System.Drawing.Size(71, 28);
            this.lblTienMat.TabIndex = 4;
            this.lblTienMat.Text = "label1";
            // 
            // lblTienMatDH
            // 
            this.lblTienMatDH.AutoSize = true;
            this.lblTienMatDH.Font = new System.Drawing.Font("Bahnschrift", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTienMatDH.Location = new System.Drawing.Point(3, 9);
            this.lblTienMatDH.Name = "lblTienMatDH";
            this.lblTienMatDH.Size = new System.Drawing.Size(203, 28);
            this.lblTienMatDH.TabIndex = 3;
            this.lblTienMatDH.Text = "TIỀN MẤT DO HỦY: ";
            // 
            // tlpCharts
            // 
            this.tlpCharts.ColumnCount = 2;
            this.tlpCharts.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 65F));
            this.tlpCharts.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 35F));
            this.tlpCharts.Controls.Add(this.chartDoanhThu, 0, 0);
            this.tlpCharts.Controls.Add(this.chartTyLe, 1, 0);
            this.tlpCharts.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpCharts.Location = new System.Drawing.Point(3, 183);
            this.tlpCharts.Name = "tlpCharts";
            this.tlpCharts.RowCount = 1;
            this.tlpCharts.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpCharts.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 236F));
            this.tlpCharts.Size = new System.Drawing.Size(1459, 236);
            this.tlpCharts.TabIndex = 2;
            // 
            // chartDoanhThu
            // 
            chartArea1.Name = "ChartArea1";
            this.chartDoanhThu.ChartAreas.Add(chartArea1);
            this.chartDoanhThu.Dock = System.Windows.Forms.DockStyle.Fill;
            legend1.Name = "Legend1";
            this.chartDoanhThu.Legends.Add(legend1);
            this.chartDoanhThu.Location = new System.Drawing.Point(5, 5);
            this.chartDoanhThu.Margin = new System.Windows.Forms.Padding(5);
            this.chartDoanhThu.Name = "chartDoanhThu";
            series1.ChartArea = "ChartArea1";
            series1.Legend = "Legend1";
            series1.Name = "Series1";
            this.chartDoanhThu.Series.Add(series1);
            this.chartDoanhThu.Size = new System.Drawing.Size(938, 226);
            this.chartDoanhThu.TabIndex = 0;
            this.chartDoanhThu.Text = "chart1";
            // 
            // chartTyLe
            // 
            chartArea2.Name = "ChartArea1";
            this.chartTyLe.ChartAreas.Add(chartArea2);
            this.chartTyLe.Dock = System.Windows.Forms.DockStyle.Fill;
            legend2.Name = "Legend1";
            this.chartTyLe.Legends.Add(legend2);
            this.chartTyLe.Location = new System.Drawing.Point(953, 5);
            this.chartTyLe.Margin = new System.Windows.Forms.Padding(5);
            this.chartTyLe.Name = "chartTyLe";
            series2.ChartArea = "ChartArea1";
            series2.Legend = "Legend1";
            series2.Name = "Series1";
            this.chartTyLe.Series.Add(series2);
            this.chartTyLe.Size = new System.Drawing.Size(501, 226);
            this.chartTyLe.TabIndex = 1;
            this.chartTyLe.Text = "chart1";
            // 
            // dgvChiTiet
            // 
            this.dgvChiTiet.BackgroundColor = System.Drawing.Color.White;
            this.dgvChiTiet.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvChiTiet.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvChiTiet.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvChiTiet.Location = new System.Drawing.Point(10, 432);
            this.dgvChiTiet.Margin = new System.Windows.Forms.Padding(10);
            this.dgvChiTiet.Name = "dgvChiTiet";
            this.dgvChiTiet.RowHeadersWidth = 51;
            this.dgvChiTiet.RowTemplate.Height = 24;
            this.dgvChiTiet.Size = new System.Drawing.Size(1445, 223);
            this.dgvChiTiet.TabIndex = 3;
            // 
            // lblBaoCao
            // 
            this.lblBaoCao.AutoSize = true;
            this.lblBaoCao.Font = new System.Drawing.Font("Bahnschrift", 25.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblBaoCao.ForeColor = System.Drawing.Color.ForestGreen;
            this.lblBaoCao.Location = new System.Drawing.Point(538, 9);
            this.lblBaoCao.Name = "lblBaoCao";
            this.lblBaoCao.Size = new System.Drawing.Size(412, 52);
            this.lblBaoCao.TabIndex = 1;
            this.lblBaoCao.Text = "BÁO CÁO THỐNG KÊ";
            // 
            // cboCoSo
            // 
            this.cboCoSo.Font = new System.Drawing.Font("Bahnschrift", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cboCoSo.FormattingEnabled = true;
            this.cboCoSo.Location = new System.Drawing.Point(616, 65);
            this.cboCoSo.Name = "cboCoSo";
            this.cboCoSo.Size = new System.Drawing.Size(256, 36);
            this.cboCoSo.TabIndex = 2;
            this.cboCoSo.SelectedIndexChanged += new System.EventHandler(this.cboCoSo_SelectedIndexChanged);
            // 
            // lblCoSo
            // 
            this.lblCoSo.AutoSize = true;
            this.lblCoSo.Font = new System.Drawing.Font("Bahnschrift", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCoSo.Location = new System.Drawing.Point(455, 65);
            this.lblCoSo.Name = "lblCoSo";
            this.lblCoSo.Size = new System.Drawing.Size(83, 28);
            this.lblCoSo.TabIndex = 3;
            this.lblCoSo.Text = "Cơ Sở: ";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Cornsilk;
            this.ClientSize = new System.Drawing.Size(1489, 781);
            this.Controls.Add(this.lblCoSo);
            this.Controls.Add(this.cboCoSo);
            this.Controls.Add(this.lblBaoCao);
            this.Controls.Add(this.tlpMain);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.tlpMain.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.tlpKPI.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.panel5.ResumeLayout(false);
            this.panel5.PerformLayout();
            this.tlpCharts.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.chartDoanhThu)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartTyLe)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvChiTiet)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tlpMain;
        private System.Windows.Forms.Label lblBaoCao;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label lblTuNgay;
        private System.Windows.Forms.Label lblDenNgay;
        private System.Windows.Forms.DateTimePicker dtpDenNgay;
        private System.Windows.Forms.DateTimePicker dtpTuNgay;
        private System.Windows.Forms.Button btnXem;
        private System.Windows.Forms.TableLayoutPanel tlpKPI;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.Label lblDoanhThu;
        private System.Windows.Forms.Label lblSoDat;
        private System.Windows.Forms.Label lblTyLe;
        private System.Windows.Forms.ComboBox cboCoSo;
        private System.Windows.Forms.Label lblCoSo;
        private System.Windows.Forms.Label lblTienMatDH;
        private System.Windows.Forms.TableLayoutPanel tlpCharts;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartDoanhThu;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartTyLe;
        private System.Windows.Forms.DataGridView dgvChiTiet;
        private System.Windows.Forms.Label lblTongDoanhThu;
        private System.Windows.Forms.Label lblSoLuotDat;
        private System.Windows.Forms.Label lblTyLeLapDay;
        private System.Windows.Forms.Label lblTienMat;
    }
}

