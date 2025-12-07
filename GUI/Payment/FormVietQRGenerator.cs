using System;
using System.Drawing;
using System.Windows.Forms;
using System.Threading.Tasks;
using Guna.UI2.WinForms;
using QLTN_LT.BLL;
using QLTN_LT.DTO;
using QLTN_LT.GUI.Base;
using QLTN_LT.GUI.Helper;
using QLTN_LT.GUI.Utilities;

namespace QLTN_LT.GUI.Payment
{
    /// <summary>
    /// Form tạo mã QR VietQR
    /// </summary>
    public partial class FormVietQRGenerator : BaseForm
    {
        private VietQRIntegrationService _vietQRService = new VietQRIntegrationService();
        private OrderDTO _currentOrder;
        private PictureBox _pictureBoxQR;
        private Label _lblQRStatus;
        private TextBox _txtAccountNo;
        private TextBox _txtAccountName;
        private NumericUpDown _numAmount;
        private TextBox _txtDescription;
        private ComboBox _cmbTemplate;
        private Button _btnGenerateQuickLink;
        private Button _btnGenerateAPI;
        private Button _btnCopyLink;
        private Button _btnDownload;
        private Label _lblQuickLink;

        public FormVietQRGenerator(OrderDTO order = null)
        {
            // InitializeComponent(); // no designer; UI is built in code
            _currentOrder = order;
            SetupUI();
            ApplyThemeAndResponsive();
        }

        private void SetupUI()
        {
            this.Text = "Tạo Mã QR VietQR";
            this.Size = new Size(900, 700);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = ThemeHelper.GetBackgroundColor();

            // Panel chính
            var pnlMain = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = ThemeHelper.GetBackgroundColor(),
                Padding = new Padding(20)
            };

            // Panel input
            var pnlInput = new Panel
            {
                Dock = DockStyle.Top,
                Height = 300,
                BackColor = ThemeHelper.GetSurfaceColor(),
                Padding = new Padding(15),
                Margin = new Padding(0, 0, 0, 10)
            };

            // Tiêu đề
            var lblTitle = new Label
            {
                Text = "Thông Tin Thanh Toán",
                Dock = DockStyle.Top,
                Height = 30,
                Font = ThemeHelper.GetHeadingFont(14),
                ForeColor = ThemeHelper.GetTextColor()
            };
            pnlInput.Controls.Add(lblTitle);

            // Số tài khoản
            var lblAccountNo = new Label
            {
                Text = "Số Tài Khoản:",
                Dock = DockStyle.Top,
                Height = 20,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = ThemeHelper.GetTextColor(),
                Margin = new Padding(0, 10, 0, 5)
            };
            pnlInput.Controls.Add(lblAccountNo);

            _txtAccountNo = new TextBox
            {
                Dock = DockStyle.Top,
                Height = 35,
                Text = "113366668888",
                BackColor = ThemeHelper.GetSurfaceColor(),
                ForeColor = ThemeHelper.GetTextColor(),
                Font = new Font("Segoe UI", 10),
                BorderStyle = BorderStyle.FixedSingle,
                Margin = new Padding(0, 0, 0, 10)
            };
            pnlInput.Controls.Add(_txtAccountNo);

            // Tên tài khoản
            var lblAccountName = new Label
            {
                Text = "Tên Tài Khoản:",
                Dock = DockStyle.Top,
                Height = 20,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = ThemeHelper.GetTextColor(),
                Margin = new Padding(0, 10, 0, 5)
            };
            pnlInput.Controls.Add(lblAccountName);

            _txtAccountName = new TextBox
            {
                Dock = DockStyle.Top,
                Height = 35,
                Text = "QUY VAC XIN PHONG CHONG COVID",
                BackColor = ThemeHelper.GetSurfaceColor(),
                ForeColor = ThemeHelper.GetTextColor(),
                Font = new Font("Segoe UI", 10),
                BorderStyle = BorderStyle.FixedSingle,
                Margin = new Padding(0, 0, 0, 10)
            };
            pnlInput.Controls.Add(_txtAccountName);

            // Số tiền
            var lblAmount = new Label
            {
                Text = "Số Tiền (VND):",
                Dock = DockStyle.Top,
                Height = 20,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = ThemeHelper.GetTextColor(),
                Margin = new Padding(0, 10, 0, 5)
            };
            pnlInput.Controls.Add(lblAmount);

            _numAmount = new NumericUpDown
            {
                Dock = DockStyle.Top,
                Height = 35,
                Value = _currentOrder?.TotalAmount ?? 0,
                Minimum = 0,
                Maximum = 9999999999999,
                BackColor = ThemeHelper.GetSurfaceColor(),
                ForeColor = ThemeHelper.GetTextColor(),
                Font = new Font("Segoe UI", 10),
                Margin = new Padding(0, 0, 0, 10)
            };
            pnlInput.Controls.Add(_numAmount);

            // Nội dung
            var lblDescription = new Label
            {
                Text = "Nội Dung Chuyển Khoản:",
                Dock = DockStyle.Top,
                Height = 20,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = ThemeHelper.GetTextColor(),
                Margin = new Padding(0, 10, 0, 5)
            };
            pnlInput.Controls.Add(lblDescription);

            _txtDescription = new TextBox
            {
                Dock = DockStyle.Top,
                Height = 35,
                Text = _currentOrder != null ? $"DH{_currentOrder.OrderNumber}" : "Ung Ho Quy Vac Xin",
                BackColor = ThemeHelper.GetSurfaceColor(),
                ForeColor = ThemeHelper.GetTextColor(),
                Font = new Font("Segoe UI", 10),
                BorderStyle = BorderStyle.FixedSingle,
                Margin = new Padding(0, 0, 0, 10)
            };
            pnlInput.Controls.Add(_txtDescription);

            // Template
            var lblTemplate = new Label
            {
                Text = "Template:",
                Dock = DockStyle.Top,
                Height = 20,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = ThemeHelper.GetTextColor(),
                Margin = new Padding(0, 10, 0, 5)
            };
            pnlInput.Controls.Add(lblTemplate);

            _cmbTemplate = new ComboBox
            {
                Dock = DockStyle.Top,
                Height = 35,
                BackColor = ThemeHelper.GetSurfaceColor(),
                ForeColor = ThemeHelper.GetTextColor(),
                Font = new Font("Segoe UI", 10),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Margin = new Padding(0, 0, 0, 10)
            };
            _cmbTemplate.Items.AddRange(new object[] { "Compact2 (540x640)", "Compact (540x540)", "QR Only (480x480)", "Print (600x776)" });
            _cmbTemplate.SelectedIndex = 0;
            pnlInput.Controls.Add(_cmbTemplate);

            pnlMain.Controls.Add(pnlInput);

            // Panel buttons
            var pnlButtons = new Panel
            {
                Dock = DockStyle.Top,
                Height = 50,
                BackColor = ThemeHelper.GetSurfaceColor(),
                Padding = new Padding(15),
                Margin = new Padding(0, 0, 0, 10)
            };

            _btnGenerateQuickLink = new Button
            {
                Text = "Tạo Quick Link",
                Dock = DockStyle.Left,
                Width = 150,
                Height = 40,
                BackColor = ThemeHelper.Colors.Primary,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Margin = new Padding(0, 0, 10, 0)
            };
            _btnGenerateQuickLink.Click += BtnGenerateQuickLink_Click;
            pnlButtons.Controls.Add(_btnGenerateQuickLink);

            _btnGenerateAPI = new Button
            {
                Text = "Tạo QR (API)",
                Dock = DockStyle.Left,
                Width = 150,
                Height = 40,
                BackColor = ThemeHelper.Colors.Secondary,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Margin = new Padding(0, 0, 10, 0)
            };
            _btnGenerateAPI.Click += BtnGenerateAPI_Click;
            pnlButtons.Controls.Add(_btnGenerateAPI);

            _btnCopyLink = new Button
            {
                Text = "Copy Link",
                Dock = DockStyle.Left,
                Width = 120,
                Height = 40,
                BackColor = ThemeHelper.Colors.Info,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Margin = new Padding(0, 0, 10, 0)
            };
            _btnCopyLink.Click += BtnCopyLink_Click;
            pnlButtons.Controls.Add(_btnCopyLink);

            _btnDownload = new Button
            {
                Text = "Tải Xuống",
                Dock = DockStyle.Left,
                Width = 120,
                Height = 40,
                BackColor = ThemeHelper.Colors.Success,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            _btnDownload.Click += BtnDownload_Click;
            pnlButtons.Controls.Add(_btnDownload);

            pnlMain.Controls.Add(pnlButtons);

            // Panel QR
            var pnlQR = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = ThemeHelper.GetSurfaceColor(),
                Padding = new Padding(15),
                AutoScroll = true
            };

            _pictureBoxQR = new PictureBox
            {
                Width = 540,
                Height = 640,
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            pnlQR.Controls.Add(_pictureBoxQR);

            _lblQRStatus = new Label
            {
                Text = "Chưa tạo mã QR",
                Dock = DockStyle.Bottom,
                Height = 30,
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = ThemeHelper.GetTextSecondaryColor(),
                Font = new Font("Segoe UI", 10)
            };
            pnlQR.Controls.Add(_lblQRStatus);

            _lblQuickLink = new Label
            {
                Text = "",
                Dock = DockStyle.Bottom,
                Height = 60,
                TextAlign = ContentAlignment.TopLeft,
                ForeColor = ThemeHelper.GetTextColor(),
                Font = new Font("Segoe UI", 9),
                AutoSize = false,
                // WordWrap not supported in WinForms Label; using AutoSize=false + Dock fill
                Margin = new Padding(0, 10, 0, 0)
            };
            pnlQR.Controls.Add(_lblQuickLink);

            pnlMain.Controls.Add(pnlQR);

            this.Controls.Add(pnlMain);
        }

        private void ApplyThemeAndResponsive()
        {
            ThemeHelper.ApplyThemeToForm(this);
            ResponsiveDesignHelper.ApplyResponsiveDesignToForm(this);
        }

        private void BtnGenerateQuickLink_Click(object sender, EventArgs e)
        {
            try
            {
                var result = _vietQRService.GenerateQuickLink(
                    _txtAccountNo.Text,
                    _txtAccountName.Text,
                    (decimal)_numAmount.Value,
                    _txtDescription.Text,
                    "970415",
                    GetSelectedTemplate()
                );

                if (result.IsSuccess)
                {
                    _lblQuickLink.Text = $"Quick Link:\n{result.Data}";
                    _lblQRStatus.Text = "✓ Quick Link được tạo thành công";
                    _lblQRStatus.ForeColor = ThemeHelper.Colors.Success;
                    
                    // Hiển thị ảnh từ URL
                    LoadQRImageFromUrl(result.Data);
                }
                else
                {
                    FormManagementHelper.ShowErrorMessage(result.Message);
                }
            }
            catch (Exception ex)
            {
                FormManagementHelper.ShowErrorMessage($"Lỗi: {ex.Message}");
            }
        }

        private async void BtnGenerateAPI_Click(object sender, EventArgs e)
        {
            try
            {
                _lblQRStatus.Text = "Đang tạo mã QR...";
                _lblQRStatus.ForeColor = ThemeHelper.Colors.Warning;
                this.Enabled = false;

                var result = await _vietQRService.GenerateQRCodeAsync(
                    _txtAccountNo.Text,
                    _txtAccountName.Text,
                    970415,
                    (decimal)_numAmount.Value,
                    _txtDescription.Text,
                    "text",
                    GetSelectedTemplate()
                );

                this.Enabled = true;

                if (result.IsSuccess)
                {
                    _lblQRStatus.Text = "✓ Mã QR được tạo thành công";
                    _lblQRStatus.ForeColor = ThemeHelper.Colors.Success;

                    if (!string.IsNullOrWhiteSpace(result.Data.Data?.QrDataURL))
                    {
                        LoadQRImageFromDataURL(result.Data.Data.QrDataURL);
                    }
                }
                else
                {
                    FormManagementHelper.ShowErrorMessage(result.Message);
                    _lblQRStatus.Text = "✗ Lỗi tạo mã QR";
                    _lblQRStatus.ForeColor = ThemeHelper.Colors.Danger;
                }
            }
            catch (Exception ex)
            {
                this.Enabled = true;
                FormManagementHelper.ShowErrorMessage($"Lỗi: {ex.Message}");
                _lblQRStatus.Text = "✗ Lỗi";
                _lblQRStatus.ForeColor = ThemeHelper.Colors.Danger;
            }
        }

        private void BtnCopyLink_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(_lblQuickLink.Text))
                {
                    FormManagementHelper.ShowWarningMessage("Vui lòng tạo Quick Link trước");
                    return;
                }

                string link = _lblQuickLink.Text.Replace("Quick Link:\n", "");
                Clipboard.SetText(link);
                FormManagementHelper.ShowSuccessMessage("Đã copy link vào clipboard");
            }
            catch (Exception ex)
            {
                FormManagementHelper.ShowErrorMessage($"Lỗi: {ex.Message}");
            }
        }

        private void BtnDownload_Click(object sender, EventArgs e)
        {
            try
            {
                if (_pictureBoxQR.Image == null)
                {
                    FormManagementHelper.ShowWarningMessage("Vui lòng tạo mã QR trước");
                    return;
                }

                var saveDialog = new SaveFileDialog
                {
                    Filter = "PNG Image|*.png|JPEG Image|*.jpg|All Files|*.*",
                    FileName = $"VietQR_{DateTime.Now:yyyyMMdd_HHmmss}.png"
                };

                if (QLTN_LT.GUI.Helper.UIHelper.ShowSaveFileDialog(this, saveDialog) == DialogResult.OK)
                {
                    _pictureBoxQR.Image.Save(saveDialog.FileName);
                    FormManagementHelper.ShowSuccessMessage($"Đã lưu mã QR tại:\n{saveDialog.FileName}");
                }
            }
            catch (Exception ex)
            {
                FormManagementHelper.ShowErrorMessage($"Lỗi: {ex.Message}");
            }
        }

        private void LoadQRImageFromUrl(string url)
        {
            try
            {
                using (var client = new System.Net.Http.HttpClient())
                {
                    var imageData = client.GetByteArrayAsync(url).Result;
                    using (var ms = new System.IO.MemoryStream(imageData))
                    {
                        _pictureBoxQR.Image = Image.FromStream(ms);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Lỗi tải ảnh: {ex.Message}");
            }
        }

        private void LoadQRImageFromDataURL(string dataUrl)
        {
            try
            {
                if (dataUrl.StartsWith("data:image"))
                {
                    var base64 = dataUrl.Split(',')[1];
                    var imageData = Convert.FromBase64String(base64);
                    using (var ms = new System.IO.MemoryStream(imageData))
                    {
                        _pictureBoxQR.Image = Image.FromStream(ms);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Lỗi tải ảnh: {ex.Message}");
            }
        }

        private VietQRIntegrationService.QRTemplate GetSelectedTemplate()
        {
            return _cmbTemplate.SelectedIndex switch
            {
                0 => VietQRIntegrationService.QRTemplate.Compact2,
                1 => VietQRIntegrationService.QRTemplate.Compact,
                2 => VietQRIntegrationService.QRTemplate.QrOnly,
                3 => VietQRIntegrationService.QRTemplate.Print,
                _ => VietQRIntegrationService.QRTemplate.Compact2
            };
        }
    }
}

