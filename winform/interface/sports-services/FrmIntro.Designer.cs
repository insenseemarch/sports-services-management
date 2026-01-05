namespace SportsServices.Forms
{
    partial class FrmIntro
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmIntro));
            this.pnlHeader = new System.Windows.Forms.Panel();
            this.btnLoginHeader = new System.Windows.Forms.Button();
            this.lblLogo = new System.Windows.Forms.Label();
            this.pnlHero = new System.Windows.Forms.Panel();
            this.pnlContentCenter = new System.Windows.Forms.Panel();
            this.btnCta = new System.Windows.Forms.Button();
            this.lblSubTitle = new System.Windows.Forms.Label();
            this.lblMainTitle = new System.Windows.Forms.Label();
            this.pnlHeader.SuspendLayout();
            this.pnlHero.SuspendLayout();
            this.pnlContentCenter.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlHeader
            // 
            this.pnlHeader.BackColor = System.Drawing.Color.White;
            this.pnlHeader.Controls.Add(this.btnLoginHeader);
            this.pnlHeader.Controls.Add(this.lblLogo);
            this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(1200, 70);
            this.pnlHeader.TabIndex = 0;
            // 
            // btnLoginHeader
            // 
            this.btnLoginHeader.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnLoginHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(167)))), ((int)(((byte)(69)))));
            this.btnLoginHeader.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnLoginHeader.FlatAppearance.BorderSize = 0;
            this.btnLoginHeader.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLoginHeader.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnLoginHeader.ForeColor = System.Drawing.Color.White;
            this.btnLoginHeader.Location = new System.Drawing.Point(1050, 15);
            this.btnLoginHeader.Name = "btnLoginHeader";
            this.btnLoginHeader.Size = new System.Drawing.Size(120, 40);
            this.btnLoginHeader.TabIndex = 1;
            this.btnLoginHeader.Text = "Đăng nhập";
            this.btnLoginHeader.UseVisualStyleBackColor = false;
            this.btnLoginHeader.Click += new System.EventHandler(this.btnLoginHeader_Click);
            // 
            // lblLogo
            // 
            this.lblLogo.AutoSize = true;
            this.lblLogo.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold);
            this.lblLogo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(167)))), ((int)(((byte)(69)))));
            this.lblLogo.Location = new System.Drawing.Point(20, 15);
            this.lblLogo.Name = "lblLogo";
            this.lblLogo.Size = new System.Drawing.Size(262, 41);
            this.lblLogo.TabIndex = 0;
            this.lblLogo.Text = "SPORTS SERVICE";
            // 
            // pnlHero
            // 
            // LƯU Ý: Bạn cần thêm hình nền vào Resources và bỏ comment dòng dưới
            // this.pnlHero.BackgroundImage = global::SportsServices.Properties.Resources.hinh_san_bong; 
            this.pnlHero.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pnlHero.BackColor = System.Drawing.Color.WhiteSmoke; // Màu dự phòng nếu chưa có ảnh
            this.pnlHero.Controls.Add(this.pnlContentCenter);
            this.pnlHero.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlHero.Location = new System.Drawing.Point(0, 70);
            this.pnlHero.Name = "pnlHero";
            this.pnlHero.Size = new System.Drawing.Size(1200, 630);
            this.pnlHero.TabIndex = 1;
            this.pnlHero.SizeChanged += new System.EventHandler(this.pnlHero_SizeChanged);
            // 
            // pnlContentCenter
            // 
            this.pnlContentCenter.BackColor = System.Drawing.Color.Transparent; // Trong suốt để thấy ảnh nền
            this.pnlContentCenter.Controls.Add(this.btnCta);
            this.pnlContentCenter.Controls.Add(this.lblSubTitle);
            this.pnlContentCenter.Controls.Add(this.lblMainTitle);
            this.pnlContentCenter.Location = new System.Drawing.Point(300, 150);
            this.pnlContentCenter.Name = "pnlContentCenter";
            this.pnlContentCenter.Size = new System.Drawing.Size(600, 300);
            this.pnlContentCenter.TabIndex = 0;
            // 
            // btnCta
            // 
            this.btnCta.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(193)))), ((int)(((byte)(7)))));
            this.btnCta.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCta.FlatAppearance.BorderSize = 0;
            this.btnCta.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCta.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.btnCta.ForeColor = System.Drawing.Color.Black;
            this.btnCta.Location = new System.Drawing.Point(180, 180);
            this.btnCta.Name = "btnCta";
            this.btnCta.Size = new System.Drawing.Size(240, 60);
            this.btnCta.TabIndex = 2;
            this.btnCta.Text = "TÌM SÂN NGAY";
            this.btnCta.UseVisualStyleBackColor = false;
            this.btnCta.Click += new System.EventHandler(this.btnCta_Click);
            // 
            // lblSubTitle
            // 
            this.lblSubTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblSubTitle.Font = new System.Drawing.Font("Segoe UI", 14F);
            this.lblSubTitle.ForeColor = System.Drawing.Color.DimGray;
            this.lblSubTitle.Location = new System.Drawing.Point(0, 80);
            this.lblSubTitle.Name = "lblSubTitle";
            this.lblSubTitle.Size = new System.Drawing.Size(600, 60);
            this.lblSubTitle.TabIndex = 1;
            this.lblSubTitle.Text = "Hệ thống quản lý và đặt sân thể thao hàng đầu.\r\nNhanh chóng - Tiện lợi - Dễ dàng.";
            this.lblSubTitle.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // lblMainTitle
            // 
            this.lblMainTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblMainTitle.Font = new System.Drawing.Font("Segoe UI", 28F, System.Drawing.FontStyle.Bold);
            this.lblMainTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(37)))), ((int)(((byte)(41)))));
            this.lblMainTitle.Location = new System.Drawing.Point(0, 0);
            this.lblMainTitle.Name = "lblMainTitle";
            this.lblMainTitle.Size = new System.Drawing.Size(600, 80);
            this.lblMainTitle.TabIndex = 0;
            this.lblMainTitle.Text = "CHƠI THỂ THAO THẢ GA";
            this.lblMainTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // FrmIntro
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1200, 700);
            this.Controls.Add(this.pnlHero);
            this.Controls.Add(this.pnlHeader);
            this.Name = "FrmIntro";
            this.Text = "Welcome to Sports Service";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.pnlHeader.ResumeLayout(false);
            this.pnlHeader.PerformLayout();
            this.pnlHero.ResumeLayout(false);
            this.pnlContentCenter.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlHeader;
        private System.Windows.Forms.Label lblLogo;
        private System.Windows.Forms.Button btnLoginHeader;
        private System.Windows.Forms.Panel pnlHero;
        private System.Windows.Forms.Panel pnlContentCenter;
        private System.Windows.Forms.Label lblMainTitle;
        private System.Windows.Forms.Label lblSubTitle;
        private System.Windows.Forms.Button btnCta;
    }
}