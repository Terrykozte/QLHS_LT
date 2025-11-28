using System.Drawing;
using System.Windows.Forms;
using QRCoder;

namespace QLTN_LT.GUI.Table
{
    public partial class FormGenerateQR : Form
    {
        public FormGenerateQR(string qrData)
        {
            InitializeComponent();
            GenerateQrCode(qrData);
        }

        private void GenerateQrCode(string data)
        {
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(data, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);
            Bitmap qrCodeImage = qrCode.GetGraphic(20);
            picQR.Image = qrCodeImage;
            lblData.Text = data;
        }
    }
}

