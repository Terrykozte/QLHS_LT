namespace QLTN_LT.GUI.Order
{
    partial class FormPayment
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

        private void InitializeComponent()
        {
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lblOrderNumber = new System.Windows.Forms.Label();
            this.lblOrderDate = new System.Windows.Forms.Label();
            this.lblTotalAmount = new System.Windows.Forms.Label();
            this.lblPaidAmount = new System.Windows.Forms.Label();
            this.lblRemainingAmount = new System.Windows.Forms.Label();
            
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cmbPaymentMethod = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtPaymentAmount = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtNotes = new System.Windows.Forms.TextBox();
            
            this.pnlVietQR = new System.Windows.Forms.Panel();
            this.picQRCode = new System.Windows.Forms.PictureBox();
            this.lblQRStatus = new System.Windows.Forms.Label();
            this.txtPaymentInfo = new System.Windows.Forms.TextBox();
            
            this.btnConfirmPayment = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnExportInvoice = new System.Windows.Forms.Button();
            this.btnExportPdf = new System.Windows.Forms.Button();

            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.pnlVietQR.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picQRCode)).BeginInit();
            this.SuspendLayout();

            // groupBox1
            this.groupBox1.Controls.Add(this.lblOrderNumber);
            this.groupBox1.Controls.Add(this.lblOrderDate);
            this.groupBox1.Controls.Add(this.lblTotalAmount);
            this.groupBox1.Controls.Add(this.lblPaidAmount);
            this.groupBox1.Controls.Add(this.lblRemainingAmount);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(400, 150);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Thông tin đơn hàng";

            // lblOrderNumber
            this.lblOrderNumber.AutoSize = true;
            this.lblOrderNumber.Location = new System.Drawing.Point(10, 25);
            this.lblOrderNumber.Name = "lblOrderNumber";
            this.lblOrderNumber.Size = new System.Drawing.Size(100, 15);
            this.lblOrderNumber.TabIndex = 0;
            this.lblOrderNumber.Text = "Đơn hàng: ORD001";

            // lblOrderDate
            this.lblOrderDate.AutoSize = true;
            this.lblOrderDate.Location = new System.Drawing.Point(10, 45);
            this.lblOrderDate.Name = "lblOrderDate";
            this.lblOrderDate.Size = new System.Drawing.Size(100, 15);
            this.lblOrderDate.TabIndex = 1;
            this.lblOrderDate.Text = "Ngày: 01/01/2024";

            // lblTotalAmount
            this.lblTotalAmount.AutoSize = true;
            this.lblTotalAmount.Location = new System.Drawing.Point(10, 65);
            this.lblTotalAmount.Name = "lblTotalAmount";
            this.lblTotalAmount.Size = new System.Drawing.Size(100, 15);
            this.lblTotalAmount.TabIndex = 2;
            this.lblTotalAmount.Text = "Tổng tiền: 0 VNĐ";

            // lblPaidAmount
            this.lblPaidAmount.AutoSize = true;
            this.lblPaidAmount.Location = new System.Drawing.Point(10, 85);
            this.lblPaidAmount.Name = "lblPaidAmount";
            this.lblPaidAmount.Size = new System.Drawing.Size(100, 15);
            this.lblPaidAmount.TabIndex = 3;
            this.lblPaidAmount.Text = "Đã thanh toán: 0 VNĐ";

            // lblRemainingAmount
            this.lblRemainingAmount.AutoSize = true;
            this.lblRemainingAmount.ForeColor = System.Drawing.Color.Red;
            this.lblRemainingAmount.Location = new System.Drawing.Point(10, 105);
            this.lblRemainingAmount.Name = "lblRemainingAmount";
            this.lblRemainingAmount.Size = new System.Drawing.Size(100, 15);
            this.lblRemainingAmount.TabIndex = 4;
            this.lblRemainingAmount.Text = "Còn lại: 0 VNĐ";

            // groupBox2
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.cmbPaymentMethod);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.txtPaymentAmount);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.txtNotes);
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Location = new System.Drawing.Point(12, 170);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(400, 180);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Thông tin thanh toán";

            // label1
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(80, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "Phương thức:";

            // cmbPaymentMethod
            this.cmbPaymentMethod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPaymentMethod.FormattingEnabled = true;
            this.cmbPaymentMethod.Location = new System.Drawing.Point(100, 22);
            this.cmbPaymentMethod.Name = "cmbPaymentMethod";
            this.cmbPaymentMethod.Size = new System.Drawing.Size(280, 23);
            this.cmbPaymentMethod.TabIndex = 1;

            // label2
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(10, 55);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(80, 15);
            this.label2.TabIndex = 2;
            this.label2.Text = "Số tiền:";

            // txtPaymentAmount
            this.txtPaymentAmount.Location = new System.Drawing.Point(100, 52);
            this.txtPaymentAmount.Name = "txtPaymentAmount";
            this.txtPaymentAmount.Size = new System.Drawing.Size(280, 23);
            this.txtPaymentAmount.TabIndex = 3;
            this.txtPaymentAmount.TextChanged += new System.EventHandler(this.txtPaymentAmount_TextChanged);

            // label3
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(10, 85);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(80, 15);
            this.label3.TabIndex = 4;
            this.label3.Text = "Ghi chú:";

            // txtNotes
            this.txtNotes.Location = new System.Drawing.Point(100, 82);
            this.txtNotes.Multiline = true;
            this.txtNotes.Name = "txtNotes";
            this.txtNotes.Size = new System.Drawing.Size(280, 80);
            this.txtNotes.TabIndex = 5;

            // pnlVietQR
            this.pnlVietQR.Controls.Add(this.picQRCode);
            this.pnlVietQR.Controls.Add(this.lblQRStatus);
            this.pnlVietQR.Controls.Add(this.txtPaymentInfo);
            this.pnlVietQR.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlVietQR.Location = new System.Drawing.Point(420, 12);
            this.pnlVietQR.Name = "pnlVietQR";
            this.pnlVietQR.Size = new System.Drawing.Size(350, 400);
            this.pnlVietQR.TabIndex = 2;
            this.pnlVietQR.Visible = false;

            // picQRCode
            this.picQRCode.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picQRCode.Location = new System.Drawing.Point(10, 25);
            this.picQRCode.Name = "picQRCode";
            this.picQRCode.Size = new System.Drawing.Size(330, 200);
            this.picQRCode.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picQRCode.TabIndex = 0;
            this.picQRCode.TabStop = false;

            // lblQRStatus
            this.lblQRStatus.AutoSize = true;
            this.lblQRStatus.Location = new System.Drawing.Point(10, 230);
            this.lblQRStatus.Name = "lblQRStatus";
            this.lblQRStatus.Size = new System.Drawing.Size(100, 15);
            this.lblQRStatus.TabIndex = 1;
            this.lblQRStatus.Text = "Đang tạo QR Code...";

            // txtPaymentInfo
            this.txtPaymentInfo.Location = new System.Drawing.Point(10, 250);
            this.txtPaymentInfo.Multiline = true;
            this.txtPaymentInfo.Name = "txtPaymentInfo";
            this.txtPaymentInfo.ReadOnly = true;
            this.txtPaymentInfo.Size = new System.Drawing.Size(330, 130);
            this.txtPaymentInfo.TabIndex = 2;

            // btnConfirmPayment
            this.btnConfirmPayment.BackColor = System.Drawing.Color.Green;
            this.btnConfirmPayment.ForeColor = System.Drawing.Color.White;
            this.btnConfirmPayment.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnConfirmPayment.Location = new System.Drawing.Point(12, 370);
            this.btnConfirmPayment.Name = "btnConfirmPayment";
            this.btnConfirmPayment.Size = new System.Drawing.Size(190, 40);
            this.btnConfirmPayment.TabIndex = 3;
            this.btnConfirmPayment.Text = "Xác nhận thanh toán";
            this.btnConfirmPayment.UseVisualStyleBackColor = false;
            this.btnConfirmPayment.Click += new System.EventHandler(this.btnConfirmPayment_Click);

            // btnCancel
            this.btnCancel.BackColor = System.Drawing.Color.Red;
            this.btnCancel.ForeColor = System.Drawing.Color.White;
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(222, 370);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(90, 40);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "Hủy";
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);

            // btnExportInvoice (Excel)
            this.btnExportInvoice.BackColor = System.Drawing.Color.DodgerBlue;
            this.btnExportInvoice.ForeColor = System.Drawing.Color.White;
            this.btnExportInvoice.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExportInvoice.Location = new System.Drawing.Point(322, 370);
            this.btnExportInvoice.Name = "btnExportInvoice";
            this.btnExportInvoice.Size = new System.Drawing.Size(90, 40);
            this.btnExportInvoice.TabIndex = 5;
            this.btnExportInvoice.Text = "Xuất Excel";
            this.btnExportInvoice.UseVisualStyleBackColor = false;
            this.btnExportInvoice.Click += new System.EventHandler(this.btnExportInvoice_Click);

            // btnExportPdf
            this.btnExportPdf = new System.Windows.Forms.Button();
            this.btnExportPdf.BackColor = System.Drawing.Color.MediumVioletRed;
            this.btnExportPdf.ForeColor = System.Drawing.Color.White;
            this.btnExportPdf.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExportPdf.Location = new System.Drawing.Point(422, 370);
            this.btnExportPdf.Name = "btnExportPdf";
            this.btnExportPdf.Size = new System.Drawing.Size(90, 40);
            this.btnExportPdf.TabIndex = 6;
            this.btnExportPdf.Text = "Xuất PDF";
            this.btnExportPdf.UseVisualStyleBackColor = false;
            this.btnExportPdf.Click += new System.EventHandler(this.btnExportPdf_Click);

            // FormPayment
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(780, 420);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.pnlVietQR);
            this.Controls.Add(this.btnConfirmPayment);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnExportInvoice);
            this.Controls.Add(this.btnExportPdf);
            this.Name = "FormPayment";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Thanh toán đơn hàng";
            this.Load += new System.EventHandler(this.FormPayment_Load);

            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.pnlVietQR.ResumeLayout(false);
            this.pnlVietQR.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picQRCode)).EndInit();
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label lblOrderNumber;
        private System.Windows.Forms.Label lblOrderDate;
        private System.Windows.Forms.Label lblTotalAmount;
        private System.Windows.Forms.Label lblPaidAmount;
        private System.Windows.Forms.Label lblRemainingAmount;
        
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbPaymentMethod;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtPaymentAmount;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtNotes;
        
        private System.Windows.Forms.Panel pnlVietQR;
        private System.Windows.Forms.PictureBox picQRCode;
        private System.Windows.Forms.Label lblQRStatus;
        private System.Windows.Forms.TextBox txtPaymentInfo;
        
        private System.Windows.Forms.Button btnConfirmPayment;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnExportInvoice;
        private System.Windows.Forms.Button btnExportPdf;
    }
}

