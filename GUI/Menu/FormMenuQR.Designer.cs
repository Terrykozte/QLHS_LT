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
            this.pnlRight.Controls.Add(this.btnCreateOrder);
            this.pnlRight.Controls.Add(this.lblTable);
            this.pnlRight.Controls.Add(this.nudTableNo);
            this.pnlRight.Controls.Add(this.lblBranch);
            this.pnlRight.Controls.Add(this.txtBranchCode);
            this.pnlRight.Controls.Add(this.lblReserve);
            this.pnlRight.Controls.Add(this.chkReserve);
            this.pnlRight.Controls.Add(this.lblReserveTime);
            this.pnlRight.Controls.Add(this.dtpReserveTime);
            this.pnlRight.Controls.Add(this.lblGuests);
            this.pnlRight.Controls.Add(this.nudGuests);
            this.pnlRight.Controls.Add(this.lblContactName);
            this.pnlRight.Controls.Add(this.txtContactName);
            this.pnlRight.Controls.Add(this.lblContactPhone);
            this.pnlRight.Controls.Add(this.txtContactPhone);
            this.pnlRight.Controls.Add(this.lblContactNote);
            this.pnlRight.Controls.Add(this.txtContactNote);
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

            // lblReserve
            this.lblReserve = new System.Windows.Forms.Label();
            this.lblReserve.AutoSize = true;
            this.lblReserve.Location = new System.Drawing.Point(10, 410);
            this.lblReserve.Text = "Đặt trước:";

            // chkReserve
            this.chkReserve = new Guna.UI2.WinForms.Guna2CheckBox();
            this.chkReserve.Location = new System.Drawing.Point(80, 410);
            this.chkReserve.Size = new System.Drawing.Size(60, 20);

            // lblReserveTime
            this.lblReserveTime = new System.Windows.Forms.Label();
            this.lblReserveTime.AutoSize = true;
            this.lblReserveTime.Location = new System.Drawing.Point(150, 410);
            this.lblReserveTime.Text = "Thời gian:";

            // dtpReserveTime
            this.dtpReserveTime = new Guna.UI2.WinForms.Guna2DateTimePicker();
            this.dtpReserveTime.Location = new System.Drawing.Point(150, 430);
            this.dtpReserveTime.Size = new System.Drawing.Size(180, 36);

            // lblGuests
            this.lblGuests = new System.Windows.Forms.Label();
            this.lblGuests.AutoSize = true;
            this.lblGuests.Location = new System.Drawing.Point(340, 410);
            this.lblGuests.Text = "Số khách:";

            // nudGuests
            this.nudGuests = new Guna.UI2.WinForms.Guna2NumericUpDown();
            this.nudGuests.Location = new System.Drawing.Point(340, 430);
            this.nudGuests.Minimum = new decimal(new int[] {0,0,0,0});
            this.nudGuests.Maximum = new decimal(new int[] {999,0,0,0});
            this.nudGuests.Value = new decimal(new int[] {0,0,0,0});
            this.nudGuests.Size = new System.Drawing.Size(60, 36);

            // lblContactName
            this.lblContactName = new System.Windows.Forms.Label();
            this.lblContactName.AutoSize = true;
            this.lblContactName.Location = new System.Drawing.Point(10, 470);
            this.lblContactName.Text = "Tên liên hệ:";

            // txtContactName
            this.txtContactName = new Guna.UI2.WinForms.Guna2TextBox();
            this.txtContactName.Location = new System.Drawing.Point(10, 490);
            this.txtContactName.Size = new System.Drawing.Size(140, 36);

            // lblContactPhone
            this.lblContactPhone = new System.Windows.Forms.Label();
            this.lblContactPhone.AutoSize = true;
            this.lblContactPhone.Location = new System.Drawing.Point(160, 470);
            this.lblContactPhone.Text = "SĐT liên hệ:";

            // txtContactPhone
            this.txtContactPhone = new Guna.UI2.WinForms.Guna2TextBox();
            this.txtContactPhone.Location = new System.Drawing.Point(160, 490);
            this.txtContactPhone.Size = new System.Drawing.Size(140, 36);

            // lblContactNote
            this.lblContactNote = new System.Windows.Forms.Label();
            this.lblContactNote.AutoSize = true;
            this.lblContactNote.Location = new System.Drawing.Point(10, 530);
            this.lblContactNote.Text = "Ghi chú:";

            // txtContactNote
            this.txtContactNote = new Guna.UI2.WinForms.Guna2TextBox();
            this.txtContactNote.Location = new System.Drawing.Point(10, 550);
            this.txtContactNote.Multiline = true;
            this.txtContactNote.Size = new System.Drawing.Size(290, 60);

            // lblBranch
            this.lblBranch = new System.Windows.Forms.Label();
            this.lblBranch.AutoSize = true;
            this.lblBranch.Location = new System.Drawing.Point(310, 470);
            this.lblBranch.Text = "BranchCode";

            // txtBranchCode
            this.txtBranchCode = new Guna.UI2.WinForms.Guna2TextBox();
            this.txtBranchCode.Location = new System.Drawing.Point(310, 490);
            this.txtBranchCode.Size = new System.Drawing.Size(80, 36);
            this.txtBranchCode.PlaceholderText = "BR01";

            // lblTable
            this.lblTable = new System.Windows.Forms.Label();
            this.lblTable.AutoSize = true;
            this.lblTable.Location = new System.Drawing.Point(400, 470);
            this.lblTable.Text = "Số bàn";

            // nudTableNo
            this.nudTableNo = new Guna.UI2.WinForms.Guna2NumericUpDown();
            this.nudTableNo.Location = new System.Drawing.Point(400, 490);
            this.nudTableNo.Minimum = new decimal(new int[] {0,0,0,0});
            this.nudTableNo.Maximum = new decimal(new int[] {999,0,0,0});
            this.nudTableNo.Value = new decimal(new int[] {0,0,0,0});
            this.nudTableNo.Size = new System.Drawing.Size(80, 36);

            // btnCreateOrder
            this.btnCreateOrder = new Guna.UI2.WinForms.Guna2Button();
            this.btnCreateOrder.Text = "Tạo đơn hàng";
            this.btnCreateOrder.Location = new System.Drawing.Point(310, 550);
            this.btnCreateOrder.Size = new System.Drawing.Size(120, 36);
            this.btnCreateOrder.Click += new System.EventHandler(this.btnCreateOrder_Click);

            // btnGenerateQR
            this.btnGenerateQR.Text = "Tạo mã QR";
            this.btnGenerateQR.Location = new System.Drawing.Point(400, 550);
            this.btnGenerateQR.Size = new System.Drawing.Size(140, 36);
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
        private System.Windows.Forms.Label lblBranch;
        private System.Windows.Forms.Label lblTable;
        private Guna.UI2.WinForms.Guna2TextBox txtBranchCode;
        private Guna.UI2.WinForms.Guna2NumericUpDown nudTableNo;
        private Guna.UI2.WinForms.Guna2Button btnCreateOrder;
        private Guna.UI2.WinForms.Guna2Button btnGenerateQR;
        private System.Windows.Forms.PictureBox picQR;
        private Guna.UI2.WinForms.Guna2TextBox txtQRInfo;
        private System.Windows.Forms.Label lblReserve;
        private Guna.UI2.WinForms.Guna2CheckBox chkReserve;
        private System.Windows.Forms.Label lblReserveTime;
        private Guna.UI2.WinForms.Guna2DateTimePicker dtpReserveTime;
        private System.Windows.Forms.Label lblGuests;
        private Guna.UI2.WinForms.Guna2NumericUpDown nudGuests;
        private System.Windows.Forms.Label lblContactName;
        private Guna.UI2.WinForms.Guna2TextBox txtContactName;
        private System.Windows.Forms.Label lblContactPhone;
        private Guna.UI2.WinForms.Guna2TextBox txtContactPhone;
        private System.Windows.Forms.Label lblContactNote;
        private Guna.UI2.WinForms.Guna2TextBox txtContactNote;
    }
}


