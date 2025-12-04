namespace QLTN_LT.GUI.Menu
{
    partial class FormMenuQR
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.pnlMain = new Guna.UI2.WinForms.Guna2Panel();
            this.pnlLeft = new Guna.UI2.WinForms.Guna2Panel();
            this.txtSearch = new Guna.UI2.WinForms.Guna2TextBox();
            this.lblFound = new System.Windows.Forms.Label();
            this.dgvMenu = new Guna.UI2.WinForms.Guna2DataGridView();
            this.pnlRight = new Guna.UI2.WinForms.Guna2Panel();
            this.lblCart = new System.Windows.Forms.Label();
            this.dgvCart = new Guna.UI2.WinForms.Guna2DataGridView();
            this.pnlBottom = new Guna.UI2.WinForms.Guna2Panel();
            this.nudQty = new Guna.UI2.WinForms.Guna2NumericUpDown();
            this.btnAddToCart = new Guna.UI2.WinForms.Guna2Button();
            this.lblTotal = new System.Windows.Forms.Label();
            this.btnGenerateQR = new Guna.UI2.WinForms.Guna2Button();
            this.picQR = new System.Windows.Forms.PictureBox();
            this.txtQRInfo = new Guna.UI2.WinForms.Guna2TextBox();

            this.pnlMain.SuspendLayout();
            this.pnlLeft.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMenu)).BeginInit();
            this.pnlRight.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCart)).BeginInit();
            this.pnlBottom.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudQty)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picQR)).BeginInit();
            this.SuspendLayout();

            // pnlMain
            this.pnlMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlMain.Padding = new System.Windows.Forms.Padding(10);
            this.pnlMain.Controls.Add(this.pnlRight);
            this.pnlMain.Controls.Add(this.pnlLeft);

            // pnlLeft
            this.pnlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnlLeft.Width = 520;
            this.pnlLeft.Padding = new System.Windows.Forms.Padding(0, 0, 10, 0);
            this.pnlLeft.Controls.Add(this.dgvMenu);
            this.pnlLeft.Controls.Add(this.lblFound);
            this.pnlLeft.Controls.Add(this.txtSearch);

            // txtSearch
            this.txtSearch.PlaceholderText = "Tìm kiếm món...";
            this.txtSearch.Location = new System.Drawing.Point(0, 0);
            this.txtSearch.Size = new System.Drawing.Size(510, 40);
            this.txtSearch.TextChanged += new System.EventHandler(this.txtSearch_TextChanged);

            // lblFound
            this.lblFound.AutoSize = true;
            this.lblFound.Location = new System.Drawing.Point(5, 45);
            this.lblFound.Text = "Tìm thấy 0 món";

            // dgvMenu
            this.dgvMenu.Location = new System.Drawing.Point(0, 65);
            this.dgvMenu.Size = new System.Drawing.Size(510, 520);
            this.dgvMenu.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                | System.Windows.Forms.AnchorStyles.Left)
                | System.Windows.Forms.AnchorStyles.Right)));

            // pnlRight
            this.pnlRight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlRight.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.pnlRight.Controls.Add(this.txtQRInfo);
            this.pnlRight.Controls.Add(this.picQR);
            this.pnlRight.Controls.Add(this.btnGenerateQR);
            this.pnlRight.Controls.Add(this.lblTotal);
            this.pnlRight.Controls.Add(this.btnAddToCart);
            this.pnlRight.Controls.Add(this.nudQty);
            this.pnlRight.Controls.Add(this.dgvCart);
            this.pnlRight.Controls.Add(this.lblCart);
            this.pnlRight.Resize += new System.EventHandler(this.pnlRight_Resize);

            // lblCart
            this.lblCart.AutoSize = true;
            this.lblCart.Location = new System.Drawing.Point(10, 0);
            this.lblCart.Text = "Giỏ hàng";

            // dgvCart
            this.dgvCart.Location = new System.Drawing.Point(10, 20);
            this.dgvCart.Size = new System.Drawing.Size(430, 300);
            this.dgvCart.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                | System.Windows.Forms.AnchorStyles.Right)));

            // nudQty
            this.nudQty.Location = new System.Drawing.Point(10, 330);
            this.nudQty.Minimum = new decimal(new int[] {1,0,0,0});
            this.nudQty.Maximum = new decimal(new int[] {999,0,0,0});
            this.nudQty.Value = new decimal(new int[] {1,0,0,0});
            this.nudQty.Size = new System.Drawing.Size(80, 36);

            // btnAddToCart
            this.btnAddToCart.Text = "Thêm";
            this.btnAddToCart.Location = new System.Drawing.Point(100, 330);
            this.btnAddToCart.Size = new System.Drawing.Size(80, 36);
            this.btnAddToCart.Click += new System.EventHandler(this.btnAddToCart_Click);

            // lblTotal
            this.lblTotal.AutoSize = true;
            this.lblTotal.Location = new System.Drawing.Point(10, 380);
            this.lblTotal.Text = "Tổng: 0 VNĐ";

            // btnGenerateQR
            this.btnGenerateQR.Text = "Tạo mã QR";
            this.btnGenerateQR.Location = new System.Drawing.Point(300, 370);
            this.btnGenerateQR.Size = new System.Drawing.Size(140, 40);
            this.btnGenerateQR.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnGenerateQR.Click += new System.EventHandler(this.btnGenerateQR_Click);

            // picQR
            this.picQR.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picQR.Location = new System.Drawing.Point(10, 420);
            this.picQR.Size = new System.Drawing.Size(300, 220);
            this.picQR.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picQR.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left))));

            // txtQRInfo
            this.txtQRInfo.Location = new System.Drawing.Point(320, 420);
            this.txtQRInfo.Multiline = true;
            this.txtQRInfo.ReadOnly = true;
            this.txtQRInfo.Size = new System.Drawing.Size(200, 220);
            this.txtQRInfo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right))));

            // FormMenuQR
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(980, 600);
            this.Controls.Add(this.pnlMain);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormMenuQR";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Menu QR";

            this.pnlMain.ResumeLayout(false);
            this.pnlLeft.ResumeLayout(false);
            this.pnlLeft.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMenu)).EndInit();
            this.pnlRight.ResumeLayout(false);
            this.pnlRight.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCart)).EndInit();
            this.pnlBottom.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.nudQty)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picQR)).EndInit();
            this.ResumeLayout(false);
        }

        private Guna.UI2.WinForms.Guna2Panel pnlMain;
        private Guna.UI2.WinForms.Guna2Panel pnlLeft;
        private Guna.UI2.WinForms.Guna2TextBox txtSearch;
        private System.Windows.Forms.Label lblFound;
        private Guna.UI2.WinForms.Guna2DataGridView dgvMenu;
        private Guna.UI2.WinForms.Guna2Panel pnlRight;
        private System.Windows.Forms.Label lblCart;
        private Guna.UI2.WinForms.Guna2DataGridView dgvCart;
        private Guna.UI2.WinForms.Guna2Panel pnlBottom;
        private Guna.UI2.WinForms.Guna2NumericUpDown nudQty;
        private Guna.UI2.WinForms.Guna2Button btnAddToCart;
        private System.Windows.Forms.Label lblTotal;
        private Guna.UI2.WinForms.Guna2Button btnGenerateQR;
        private System.Windows.Forms.PictureBox picQR;
        private Guna.UI2.WinForms.Guna2TextBox txtQRInfo;
    }
}


