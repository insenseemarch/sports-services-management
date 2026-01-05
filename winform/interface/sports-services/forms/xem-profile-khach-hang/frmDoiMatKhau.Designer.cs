namespace ThongTinCaNhan
{
    partial class frmDoiMatKhau
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
            this.txtMatKhauCu = new System.Windows.Forms.TextBox();
            this.lbMatKhauCu = new System.Windows.Forms.Label();
            this.titleDoiMatKhau = new System.Windows.Forms.Label();
            this.lbMatKhauMoi = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtMatKhauMoi = new System.Windows.Forms.TextBox();
            this.txtXacNhan = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.btnLuu = new System.Windows.Forms.Button();
            this.btnHuy = new System.Windows.Forms.Button();
            this.chkShowMatKhau = new System.Windows.Forms.CheckBox();
            this.chkShowMatKhauMoi = new System.Windows.Forms.CheckBox();
            this.chkShowXacNhan = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // txtMatKhauCu
            // 
            this.txtMatKhauCu.Font = new System.Drawing.Font("Bahnschrift", 12F);
            this.txtMatKhauCu.Location = new System.Drawing.Point(288, 129);
            this.txtMatKhauCu.Name = "txtMatKhauCu";
            this.txtMatKhauCu.PasswordChar = '*';
            this.txtMatKhauCu.Size = new System.Drawing.Size(380, 32);
            this.txtMatKhauCu.TabIndex = 0;
            this.txtMatKhauCu.TextChanged += new System.EventHandler(this.txtMatKhauCu_TextChanged);
            // 
            // lbMatKhauCu
            // 
            this.lbMatKhauCu.AutoSize = true;
            this.lbMatKhauCu.Font = new System.Drawing.Font("Bahnschrift", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbMatKhauCu.ForeColor = System.Drawing.Color.DarkGreen;
            this.lbMatKhauCu.Location = new System.Drawing.Point(12, 137);
            this.lbMatKhauCu.Name = "lbMatKhauCu";
            this.lbMatKhauCu.Size = new System.Drawing.Size(128, 24);
            this.lbMatKhauCu.TabIndex = 1;
            this.lbMatKhauCu.Text = "Mật Khẩu Cũ:";
            // 
            // titleDoiMatKhau
            // 
            this.titleDoiMatKhau.AutoSize = true;
            this.titleDoiMatKhau.Font = new System.Drawing.Font("Microsoft Sans Serif", 26F, System.Drawing.FontStyle.Bold);
            this.titleDoiMatKhau.ForeColor = System.Drawing.Color.ForestGreen;
            this.titleDoiMatKhau.Location = new System.Drawing.Point(224, 39);
            this.titleDoiMatKhau.Name = "titleDoiMatKhau";
            this.titleDoiMatKhau.Size = new System.Drawing.Size(351, 52);
            this.titleDoiMatKhau.TabIndex = 2;
            this.titleDoiMatKhau.Text = "ĐỔI MẬT KHẤU";
            // 
            // lbMatKhauMoi
            // 
            this.lbMatKhauMoi.AutoSize = true;
            this.lbMatKhauMoi.Font = new System.Drawing.Font("Bahnschrift", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbMatKhauMoi.ForeColor = System.Drawing.Color.DarkGreen;
            this.lbMatKhauMoi.Location = new System.Drawing.Point(12, 215);
            this.lbMatKhauMoi.Name = "lbMatKhauMoi";
            this.lbMatKhauMoi.Size = new System.Drawing.Size(137, 24);
            this.lbMatKhauMoi.TabIndex = 3;
            this.lbMatKhauMoi.Text = "Mật Khẩu Mới:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Bahnschrift", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.DarkGreen;
            this.label2.Location = new System.Drawing.Point(12, 287);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(226, 24);
            this.label2.TabIndex = 4;
            this.label2.Text = "Nhập Lại Mật Khẩu Mới: ";
            // 
            // txtMatKhauMoi
            // 
            this.txtMatKhauMoi.Font = new System.Drawing.Font("Bahnschrift", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtMatKhauMoi.Location = new System.Drawing.Point(288, 207);
            this.txtMatKhauMoi.Name = "txtMatKhauMoi";
            this.txtMatKhauMoi.PasswordChar = '*';
            this.txtMatKhauMoi.Size = new System.Drawing.Size(380, 32);
            this.txtMatKhauMoi.TabIndex = 5;
            // 
            // txtXacNhan
            // 
            this.txtXacNhan.Font = new System.Drawing.Font("Bahnschrift", 12F);
            this.txtXacNhan.Location = new System.Drawing.Point(288, 284);
            this.txtXacNhan.Name = "txtXacNhan";
            this.txtXacNhan.PasswordChar = '*';
            this.txtXacNhan.Size = new System.Drawing.Size(380, 32);
            this.txtXacNhan.TabIndex = 6;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Bahnschrift", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.Red;
            this.label1.Location = new System.Drawing.Point(146, 132);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(19, 24);
            this.label1.TabIndex = 7;
            this.label1.Text = "*";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Bahnschrift", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.Color.Red;
            this.label3.Location = new System.Drawing.Point(146, 207);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(19, 24);
            this.label3.TabIndex = 8;
            this.label3.Text = "*";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Bahnschrift", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.Color.Red;
            this.label4.Location = new System.Drawing.Point(229, 284);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(19, 24);
            this.label4.TabIndex = 9;
            this.label4.Text = "*";
            // 
            // btnLuu
            // 
            this.btnLuu.Font = new System.Drawing.Font("Bahnschrift", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnLuu.ForeColor = System.Drawing.Color.DarkGreen;
            this.btnLuu.Location = new System.Drawing.Point(713, 396);
            this.btnLuu.Name = "btnLuu";
            this.btnLuu.Size = new System.Drawing.Size(75, 42);
            this.btnLuu.TabIndex = 10;
            this.btnLuu.Text = "Lưu";
            this.btnLuu.UseVisualStyleBackColor = true;
            this.btnLuu.Click += new System.EventHandler(this.btnLuu_Click);
            // 
            // btnHuy
            // 
            this.btnHuy.Font = new System.Drawing.Font("Bahnschrift", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnHuy.ForeColor = System.Drawing.Color.DarkGreen;
            this.btnHuy.Location = new System.Drawing.Point(593, 396);
            this.btnHuy.Name = "btnHuy";
            this.btnHuy.Size = new System.Drawing.Size(75, 42);
            this.btnHuy.TabIndex = 11;
            this.btnHuy.Text = "Hủy";
            this.btnHuy.UseVisualStyleBackColor = true;
            this.btnHuy.Click += new System.EventHandler(this.btnHuy_Click);
            // 
            // chkShowMatKhau
            // 
            this.chkShowMatKhau.AutoSize = true;
            this.chkShowMatKhau.Font = new System.Drawing.Font("Bahnschrift", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkShowMatKhau.Location = new System.Drawing.Point(699, 128);
            this.chkShowMatKhau.Name = "chkShowMatKhau";
            this.chkShowMatKhau.Size = new System.Drawing.Size(81, 28);
            this.chkShowMatKhau.TabIndex = 12;
            this.chkShowMatKhau.Text = "Show";
            this.chkShowMatKhau.UseVisualStyleBackColor = true;
            this.chkShowMatKhau.CheckedChanged += new System.EventHandler(this.chkShowMatKhau_CheckedChanged);
            // 
            // chkShowMatKhauMoi
            // 
            this.chkShowMatKhauMoi.AutoSize = true;
            this.chkShowMatKhauMoi.Font = new System.Drawing.Font("Bahnschrift", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkShowMatKhauMoi.Location = new System.Drawing.Point(699, 209);
            this.chkShowMatKhauMoi.Name = "chkShowMatKhauMoi";
            this.chkShowMatKhauMoi.Size = new System.Drawing.Size(81, 28);
            this.chkShowMatKhauMoi.TabIndex = 13;
            this.chkShowMatKhauMoi.Text = "Show";
            this.chkShowMatKhauMoi.UseVisualStyleBackColor = true;
            this.chkShowMatKhauMoi.CheckedChanged += new System.EventHandler(this.chkShowMatKhauMoi_CheckedChanged);
            // 
            // chkShowXacNhan
            // 
            this.chkShowXacNhan.AutoSize = true;
            this.chkShowXacNhan.Font = new System.Drawing.Font("Bahnschrift", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkShowXacNhan.Location = new System.Drawing.Point(699, 284);
            this.chkShowXacNhan.Name = "chkShowXacNhan";
            this.chkShowXacNhan.Size = new System.Drawing.Size(81, 28);
            this.chkShowXacNhan.TabIndex = 14;
            this.chkShowXacNhan.Text = "Show";
            this.chkShowXacNhan.UseVisualStyleBackColor = true;
            this.chkShowXacNhan.CheckedChanged += new System.EventHandler(this.chkShowXacNhan_CheckedChanged);
            // 
            // frmDoiMatKhau
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Cornsilk;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.chkShowXacNhan);
            this.Controls.Add(this.chkShowMatKhauMoi);
            this.Controls.Add(this.chkShowMatKhau);
            this.Controls.Add(this.btnHuy);
            this.Controls.Add(this.btnLuu);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtXacNhan);
            this.Controls.Add(this.txtMatKhauMoi);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lbMatKhauMoi);
            this.Controls.Add(this.titleDoiMatKhau);
            this.Controls.Add(this.lbMatKhauCu);
            this.Controls.Add(this.txtMatKhauCu);
            this.Name = "frmDoiMatKhau";
            this.Text = "frmDoiMatKhau";
            this.Load += new System.EventHandler(this.frmDoiMatKhau_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtMatKhauCu;
        private System.Windows.Forms.Label lbMatKhauCu;
        private System.Windows.Forms.Label titleDoiMatKhau;
        private System.Windows.Forms.Label lbMatKhauMoi;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtMatKhauMoi;
        private System.Windows.Forms.TextBox txtXacNhan;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnLuu;
        private System.Windows.Forms.Button btnHuy;
        private System.Windows.Forms.CheckBox chkShowMatKhau;
        private System.Windows.Forms.CheckBox chkShowMatKhauMoi;
        private System.Windows.Forms.CheckBox chkShowXacNhan;
    }
}