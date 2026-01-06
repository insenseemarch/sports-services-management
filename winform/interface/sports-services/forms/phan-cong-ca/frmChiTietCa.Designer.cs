namespace SportsServices.Forms
{
    partial class frmChiTietCa
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
            this.lblTieuDe = new System.Windows.Forms.Label();
            this.lbQuanLy = new System.Windows.Forms.Label();
            this.txtQuanLy = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.rtbNhanVien = new System.Windows.Forms.RichTextBox();
            this.btnLuu = new System.Windows.Forms.Button();
            this.btnDong = new System.Windows.Forms.Button();
            this.Title = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblTieuDe
            // 
            this.lblTieuDe.AutoSize = true;
            this.lblTieuDe.Font = new System.Drawing.Font("Bahnschrift", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTieuDe.Location = new System.Drawing.Point(184, 66);
            this.lblTieuDe.Name = "lblTieuDe";
            this.lblTieuDe.Size = new System.Drawing.Size(100, 24);
            this.lblTieuDe.TabIndex = 0;
            this.lblTieuDe.Text = "Ca + Ngày";
            this.lblTieuDe.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbQuanLy
            // 
            this.lbQuanLy.AutoSize = true;
            this.lbQuanLy.Font = new System.Drawing.Font("Bahnschrift", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbQuanLy.Location = new System.Drawing.Point(16, 170);
            this.lbQuanLy.Name = "lbQuanLy";
            this.lbQuanLy.Size = new System.Drawing.Size(144, 24);
            this.lbQuanLy.TabIndex = 1;
            this.lbQuanLy.Text = "Người Quản Lý:";
            // 
            // txtQuanLy
            // 
            this.txtQuanLy.Font = new System.Drawing.Font("Bahnschrift", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtQuanLy.Location = new System.Drawing.Point(265, 170);
            this.txtQuanLy.Name = "txtQuanLy";
            this.txtQuanLy.Size = new System.Drawing.Size(396, 32);
            this.txtQuanLy.TabIndex = 2;
            this.txtQuanLy.TextChanged += new System.EventHandler(this.txtQuanLy_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Bahnschrift", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(16, 282);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(202, 24);
            this.label1.TabIndex = 3;
            this.label1.Text = "Danh sách Nhân viên:";
            // 
            // rtbNhanVien
            // 
            this.rtbNhanVien.Font = new System.Drawing.Font("Bahnschrift", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtbNhanVien.Location = new System.Drawing.Point(265, 286);
            this.rtbNhanVien.Name = "rtbNhanVien";
            this.rtbNhanVien.Size = new System.Drawing.Size(396, 96);
            this.rtbNhanVien.TabIndex = 4;
            this.rtbNhanVien.Text = "";
            // 
            // btnLuu
            // 
            this.btnLuu.BackColor = System.Drawing.Color.DarkGreen;
            this.btnLuu.Font = new System.Drawing.Font("Bahnschrift", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnLuu.ForeColor = System.Drawing.Color.White;
            this.btnLuu.Location = new System.Drawing.Point(577, 478);
            this.btnLuu.Name = "btnLuu";
            this.btnLuu.Size = new System.Drawing.Size(102, 41);
            this.btnLuu.TabIndex = 5;
            this.btnLuu.Text = "Lưu";
            this.btnLuu.UseVisualStyleBackColor = false;
            this.btnLuu.Click += new System.EventHandler(this.btnLuu_Click_1);
            // 
            // btnDong
            // 
            this.btnDong.BackColor = System.Drawing.Color.DarkGreen;
            this.btnDong.Font = new System.Drawing.Font("Bahnschrift", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDong.ForeColor = System.Drawing.Color.White;
            this.btnDong.Location = new System.Drawing.Point(424, 478);
            this.btnDong.Name = "btnDong";
            this.btnDong.Size = new System.Drawing.Size(102, 41);
            this.btnDong.TabIndex = 6;
            this.btnDong.Text = "Đóng";
            this.btnDong.UseVisualStyleBackColor = false;
            this.btnDong.Click += new System.EventHandler(this.btnDong_Click);
            // 
            // Title
            // 
            this.Title.AutoSize = true;
            this.Title.BackColor = System.Drawing.Color.Cornsilk;
            this.Title.Font = new System.Drawing.Font("Bahnschrift", 19.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Title.ForeColor = System.Drawing.Color.ForestGreen;
            this.Title.Location = new System.Drawing.Point(229, 9);
            this.Title.Name = "Title";
            this.Title.Size = new System.Drawing.Size(264, 40);
            this.Title.TabIndex = 7;
            this.Title.Text = "CHI TIẾT CA LÀM";
            // 
            // frmChiTietCa
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Cornsilk;
            this.ClientSize = new System.Drawing.Size(715, 531);
            this.Controls.Add(this.Title);
            this.Controls.Add(this.btnDong);
            this.Controls.Add(this.btnLuu);
            this.Controls.Add(this.rtbNhanVien);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtQuanLy);
            this.Controls.Add(this.lbQuanLy);
            this.Controls.Add(this.lblTieuDe);
            this.Name = "frmChiTietCa";
            this.Text = "frmChiTietCa";
            this.Load += new System.EventHandler(this.frmChiTietCa_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblTieuDe;
        private System.Windows.Forms.Label lbQuanLy;
        private System.Windows.Forms.TextBox txtQuanLy;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RichTextBox rtbNhanVien;
        private System.Windows.Forms.Button btnLuu;
        private System.Windows.Forms.Button btnDong;
        private System.Windows.Forms.Label Title;
    }
}