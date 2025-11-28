namespace QLTN_LT.GUI.Table
{
    partial class FormGenerateQR
    {
        private System.ComponentModel.IContainer components = null;
        private Guna.UI2.WinForms.Guna2PictureBox picQR;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblData;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.picQR = new Guna.UI2.WinForms.Guna2PictureBox();
            this.lblData = new Guna.UI2.WinForms.Guna2HtmlLabel();
            ((System.ComponentModel.ISupportInitialize)(this.picQR)).BeginInit();
            this.SuspendLayout();
            // 
            // picQR
            // 
            this.picQR.ImageRotate = 0F;
            this.picQR.Location = new System.Drawing.Point(50, 25);
            this.picQR.Name = "picQR";
            this.picQR.Size = new System.Drawing.Size(300, 300);
            this.picQR.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picQR.TabIndex = 0;
            this.picQR.TabStop = false;
            // 
            // lblData
            // 
            this.lblData.BackColor = System.Drawing.Color.Transparent;
            this.lblData.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblData.Location = new System.Drawing.Point(50, 340);
            this.lblData.Name = "lblData";
            this.lblData.Size = new System.Drawing.Size(62, 23);
            this.lblData.TabIndex = 1;
            this.lblData.Text = "QR Data";
            this.lblData.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // FormGenerateQR
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(400, 400);
            this.Controls.Add(this.lblData);
            this.Controls.Add(this.picQR);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormGenerateQR";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Mã QR Code";
            ((System.ComponentModel.ISupportInitialize)(this.picQR)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}

