using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using QLTN_LT.BLL;
using QLTN_LT.DTO;
using QRCoder;
using Guna.UI2.WinForms;

using QLTN_LT.GUI.Base;
using QLTN_LT.GUI.Helper;
using QLTN_LT.GUI.Utilities;

namespace QLTN_LT.GUI.Order
{
    public partial class FormPayment : BaseForm
    {
        private readonly OrderBLL _orderBLL;
        private readonly PaymentBLL _paymentBLL;
        private readonly PaymentConfirmationService _confirmationService;
        private int _orderId;
        private OrderDTO _order;
        private List<PaymentMethodDTO> _paymentMethods;
        private System.Windows.Forms.Timer _autoRefreshTimer;
        private bool _isProcessing = false;
        private LoadingOverlay _overlay;

        public FormPayment(int orderId)
        {
            InitializeComponent();
            _orderId = orderId;
            _orderBLL = new OrderBLL();
            _paymentBLL = new PaymentBLL();
            _confirmationService = new PaymentConfirmationService();

            try
            {
                UIHelper.ApplyFormStyle(this);
            }
            catch { }

            // Setup auto-refresh timer
            _autoRefreshTimer = new System.Windows.Forms.Timer();
            _autoRefreshTimer.Interval = 2000; // Refresh every 2 seconds
            _autoRefreshTimer.Tick += AutoRefreshTimer_Tick;

            this.KeyPreview = true;
            this.KeyDown += FormPayment_KeyDown;

            // Overlay
            _overlay = new LoadingOverlay(this, "Đang xử lý...");
        }

        private void FormPayment_Load(object sender, EventArgs e)
        {
            try
            {
                // Setup keyboard navigation
                KeyboardNavigationHelper.RegisterForm(this, new List<Control>
                {
                    cmbPaymentMethod, txtPaymentAmount, btnConfirmPayment, btnCancel
                });

                // Apply theme & responsive
                ThemeHelper.ApplyThemeToForm(this);
                ApplyResponsiveDesign();

                // Setup animations
                AnimationHelper.FadeIn(this, 300);

                LoadOrderData();
                LoadPaymentMethods();
                SetupUI();
                
                // Start auto-refresh timer
                _autoRefreshTimer.Start();
            }
            catch (Exception ex)
            {
                UXInteractionHelper.ShowError("Lỗi", $"Lỗi tải dữ liệu: {ex.Message}");
            }
        }
        
        private void FormPayment_FormClosing(object sender, FormClosingEventArgs e)
        {
            _autoRefreshTimer?.Stop();
            _autoRefreshTimer?.Dispose();
            _overlay?.Dispose();
            KeyboardNavigationHelper.UnregisterForm(this);
        }
        
        private void AutoRefreshTimer_Tick(object sender, EventArgs e)
        {
            // Tự động refresh dữ liệu đơn hàng mỗi 2 giây
            try
            {
                var updatedOrder = _orderBLL.GetById(_orderId);
                if (updatedOrder != null && updatedOrder.TotalAmount != _order.TotalAmount)
                {
                    _order = updatedOrder;
                    LoadOrderData();
                    
                    // Nếu đang hiển thị VietQR, tự động tạo QR Code mới
                    if (pnlVietQR.Visible && cmbPaymentMethod.SelectedItem is PaymentMethodDTO method && method.MethodName == "VietQR")
                    {
                        GenerateVietQRCode();
                    }
                }
            }
            catch
            {
                // Ignore errors in auto-refresh
            }
        }
        
        public void RefreshQRCode()
        {
            if (pnlVietQR.Visible && cmbPaymentMethod.SelectedItem is PaymentMethodDTO method && method.MethodName == "VietQR")
            {
                GenerateVietQRCode();
            }
        }

        private void LoadOrderData()
        {
            _order = _orderBLL.GetById(_orderId);
            if (_order == null)
                throw new Exception("Không tìm thấy đơn hàng");

            lblOrderNumber.Text = $"Đơn hàng: {_order.OrderNumber}";
            lblOrderDate.Text = $"Ngày: {_order.OrderDate:dd/MM/yyyy HH:mm}";
            lblTotalAmount.Text = $"Tổng tiền: {_order.TotalAmount:N0} VNĐ";

            var paidAmount = _paymentBLL.CalculateTotalPaid(_orderId);
            var remaining = _order.TotalAmount - paidAmount;

            lblPaidAmount.Text = $"Đã thanh toán: {paidAmount:N0} VNĐ";
            lblRemainingAmount.Text = $"Còn lại: {remaining:N0} VNĐ";

            txtPaymentAmount.Text = remaining.ToString("N0");
        }

        private void LoadPaymentMethods()
        {
            _paymentMethods = _paymentBLL.GetPaymentMethods();
            cmbPaymentMethod.DataSource = _paymentMethods;
            cmbPaymentMethod.DisplayMember = "MethodName";
            cmbPaymentMethod.ValueMember = "PaymentMethodID";
        }

        private void SetupUI()
        {
            // Setup payment method change event
            cmbPaymentMethod.SelectedIndexChanged += CmbPaymentMethod_SelectedIndexChanged;

            // Hover/click effects for primary buttons
            try
            {
                // Style basic WinForms buttons
                btnConfirmPayment.FlatStyle = FlatStyle.Flat;
                btnConfirmPayment.BackColor = Color.FromArgb(34, 197, 94);
                btnConfirmPayment.ForeColor = Color.White;
                btnConfirmPayment.FlatAppearance.BorderSize = 0;

                btnCancel.FlatStyle = FlatStyle.Flat;
                btnCancel.BackColor = Color.FromArgb(107, 114, 128);
                btnCancel.ForeColor = Color.White;
                btnCancel.FlatAppearance.BorderSize = 0;
            }
            catch { }
        }

        private void ApplyResponsiveDesign()
        {
            try
            {
                // Adjust font sizes
                if (lblOrderNumber != null)
                    lblOrderNumber.Font = new Font(lblOrderNumber.Font.FontFamily,
                        ResponsiveHelper.GetResponsiveFontSize(12, this), FontStyle.Bold);

                if (lblTotalAmount != null)
                    lblTotalAmount.Font = new Font(lblTotalAmount.Font.FontFamily,
                        ResponsiveHelper.GetResponsiveFontSize(14, this), FontStyle.Bold);

                // Adjust button sizes
                if (btnConfirmPayment != null)
                    btnConfirmPayment.Height = ResponsiveHelper.GetResponsiveRowHeight(this, 40);

                if (btnCancel != null)
                    btnCancel.Height = ResponsiveHelper.GetResponsiveRowHeight(this, 40);
            }
            catch { }
        }

        private void CmbPaymentMethod_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
        {
            var selectedMethod = (PaymentMethodDTO)cmbPaymentMethod.SelectedItem;
            
            // Show/Hide VietQR controls based on selected method
            if (selectedMethod.MethodName == "VietQR")
                {
                    if (!pnlVietQR.Visible)
            {
                pnlVietQR.Visible = true;
                        AnimationHelper.FadeIn(pnlVietQR, 300);
                    }
                GenerateVietQRCode();
            }
            else
                {
                    if (pnlVietQR.Visible)
                    {
                        AnimationHelper.FadeOut(pnlVietQR, 200, () =>
            {
                pnlVietQR.Visible = false;
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in payment method selection: {ex.Message}");
            }
        }

        private void GenerateVietQRCode()
        {
            try
            {
                // Show loading state
                if (lblQRStatus != null)
                {
                    lblQRStatus.Text = "Đang tạo QR Code...";
                    lblQRStatus.ForeColor = Color.FromArgb(59, 130, 246);
                }

                // Lấy thông tin tài khoản VietQR từ App.config (ưu tiên) hoặc mặc định VietinBank
                string bankCode = ConfigurationManager.AppSettings["VietQR_BankId"] ?? "970415"; // VietinBank
                string accountNumber = ConfigurationManager.AppSettings["VietQR_AccountNo"] ?? "113366668888";
                string accountName = ConfigurationManager.AppSettings["VietQR_AccountName"] ?? "VIETQR ACCOUNT";
                
                decimal amount = decimal.Parse(txtPaymentAmount.Text.Replace(",", "").Replace(" VNĐ", ""));
                string description = BuildVietQRDescription(_order);

                var vietQRService = new VietQRService(bankCode, accountNumber, accountName, amount, description);
                
                // Tạo QR Code bằng QRCoder (dạng text EMVCo từ VietQRService)
                string qrData = vietQRService.GenerateQRCode();
                
                using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
                {
                    QRCodeData qrCodeData = qrGenerator.CreateQrCode(qrData, QRCodeGenerator.ECCLevel.Q);
                    using (QRCode qrCode = new QRCode(qrCodeData))
                    {
                        Bitmap qrCodeImage = qrCode.GetGraphic(10);
                        
                        // Animate QR code display
                        if (picQRCode != null)
                        {
                        picQRCode.Image = qrCodeImage;
                            AnimationHelper.ScaleIn(picQRCode, 300);
                        }
                    }
                }

                // Hiển thị thông tin thanh toán
                if (txtPaymentInfo != null)
                {
                txtPaymentInfo.Text = vietQRService.GeneratePaymentInfo();
                    AnimationHelper.FadeIn(txtPaymentInfo, 300);
                }

                // Show success status
                if (lblQRStatus != null)
                {
                    lblQRStatus.Text = "✓ QR Code được tạo thành công";
                    lblQRStatus.ForeColor = Color.FromArgb(34, 197, 94);
                    AnimationHelper.Pulse(lblQRStatus, 400);
                }

                // Show success toast
                UXInteractionHelper.ShowToast(this, "QR Code được tạo thành công!", 2000,
                    Color.FromArgb(34, 197, 94), Color.White);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error generating VietQR: {ex.Message}");
                
                if (lblQRStatus != null)
                {
                    lblQRStatus.Text = "✗ Lỗi tạo QR Code";
                    lblQRStatus.ForeColor = Color.FromArgb(239, 68, 68);
                    AnimationHelper.Shake(lblQRStatus, 300);
                }

                UXInteractionHelper.ShowError("Lỗi", $"Lỗi tạo QR Code: {ex.Message}");
            }
        }

        private void btnConfirmPayment_Click(object sender, EventArgs e)
        {
            try
            {
                if (_isProcessing) return;

                if (!ValidatePaymentInput())
                    return;

                _isProcessing = true;
                
                // Show loading state (generic)
                var _origText = btnConfirmPayment.Text;
                btnConfirmPayment.Enabled = false;
                btnConfirmPayment.Text = "Đang xử lý...";
                btnConfirmPayment.Cursor = Cursors.WaitCursor;

                var selectedMethod = (PaymentMethodDTO)cmbPaymentMethod.SelectedItem;
                decimal paymentAmount = decimal.Parse(txtPaymentAmount.Text.Replace(",", "").Replace(" VNĐ", ""));

                // Sử dụng PaymentConfirmationService để xác nhận thanh toán
                var response = _confirmationService.ConfirmPaymentManual(
                    _orderId,
                    selectedMethod.PaymentMethodID,
                    paymentAmount,
                    $"TXN_ORD{_orderId}_{DateTime.Now:yyyyMMddHHmmss}"
                );

                if (response.Success)
                {
                    // Show success animation
                    AnimationHelper.Pulse(btnConfirmPayment, 300);
                    
                    // Show success toast
                    UXInteractionHelper.ShowToast(this, 
                        $"Thanh toán thành công!\nMã giao dịch: {response.TransactionCode}",
                        3000,
                        Color.FromArgb(34, 197, 94),
                        Color.White);
                    
                    // Delay before closing
                    var closeTimer = new Timer { Interval = 1500 };
                    closeTimer.Tick += (s, e2) =>
                    {
                        closeTimer.Stop();
                        closeTimer.Dispose();
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                    };
                    closeTimer.Start();
                }
                else
                {
                    // Show error animation
                    AnimationHelper.Shake(btnConfirmPayment, 300);
                    
                    UXInteractionHelper.ShowError("Lỗi Thanh Toán", $"Lỗi: {response.Message}");
                }
            }
            catch (Exception ex)
            {
                AnimationHelper.Shake(btnConfirmPayment, 300);
                UXInteractionHelper.ShowError("Lỗi", $"Lỗi xử lý thanh toán: {ex.Message}");
            }
            finally
            {
                _isProcessing = false;
                try
                {
                    btnConfirmPayment.Enabled = true;
                    btnConfirmPayment.Text = "Xác Nhận Thanh Toán";
                    btnConfirmPayment.Cursor = Cursors.Hand;
                }
                catch { }
            }
        }

        private bool ValidatePaymentInput()
        {
            if (string.IsNullOrWhiteSpace(txtPaymentAmount.Text))
            {
                UXInteractionHelper.ShowWarning("Cảnh báo", "Vui lòng nhập số tiền thanh toán");
                AnimationHelper.Shake(txtPaymentAmount, 300);
                return false;
            }

            if (!decimal.TryParse(txtPaymentAmount.Text.Replace(",", "").Replace(" VNĐ", ""), out decimal amount))
            {
                UXInteractionHelper.ShowWarning("Cảnh báo", "Số tiền không hợp lệ");
                AnimationHelper.Shake(txtPaymentAmount, 300);
                return false;
            }

            if (amount <= 0)
            {
                UXInteractionHelper.ShowWarning("Cảnh báo", "Số tiền phải lớn hơn 0");
                AnimationHelper.Shake(txtPaymentAmount, 300);
                return false;
            }

            var remaining = decimal.Parse(lblRemainingAmount.Text.Replace("Còn lại: ", "").Replace(" VNĐ", ""));
            if (amount > remaining)
            {
                UXInteractionHelper.ShowWarning("Cảnh báo", $"Số tiền thanh toán không được vượt quá {remaining:N0} VNĐ");
                AnimationHelper.Shake(txtPaymentAmount, 300);
                return false;
            }

            return true;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void btnExportInvoice_Click(object sender, EventArgs e)
        {
            try
            {
                using (var sfd = new SaveFileDialog())
                {
                    sfd.Title = "Xuất hóa đơn (Excel)";
                    sfd.Filter = "Excel (*.xls)|*.xls|All files (*.*)|*.*";
                    sfd.FileName = $"HoaDon_{_orderId}_{DateTime.Now:yyyyMMdd_HHmmss}.xls";
                    if (sfd.ShowDialog(this) == DialogResult.OK)
                    {
                        var exporter = new InvoiceExportService();
                        if (exporter.ExportToExcel(_orderId, sfd.FileName))
                        {
                            MessageBox.Show("Xuất Excel thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show("Xuất Excel thất bại.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi xuất hóa đơn: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void txtPaymentAmount_TextChanged(object sender, EventArgs e)
        {
            // Format currency
            if (decimal.TryParse(txtPaymentAmount.Text.Replace(",", ""), out decimal value))
            {
                txtPaymentAmount.Text = value.ToString("N0");
                txtPaymentAmount.Select(txtPaymentAmount.Text.Length, 0);
            }
        }

        private void btnExportPdf_Click(object sender, EventArgs e)
        {
            try
            {
                using (var sfd = new SaveFileDialog())
                {
                    sfd.Title = "Xuất hóa đơn (PDF)";
                    sfd.Filter = "PDF (*.pdf)|*.pdf|All files (*.*)|*.*";
                    sfd.FileName = $"HoaDon_{_orderId}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
                    if (sfd.ShowDialog(this) == DialogResult.OK)
                    {
                        var exporter = new InvoiceExportService();
                        if (exporter.ExportToPdf(_orderId, sfd.FileName))
                        {
                            MessageBox.Show("Xuất PDF thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show("Xuất PDF thất bại.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi xuất PDF: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // NEW: Button Quick Link VietQR
        private async void btnVietQRQuickLink_Click(object sender, EventArgs e)
        {
            try
            {
                _overlay.Show("Đang tạo VietQR (Quick Link)...");

                // Chuẩn bị thông tin VietQR
                string bankId = ConfigurationManager.AppSettings["VietQR_BankId"] ?? "970415"; // VietinBank
                string accountNo = ConfigurationManager.AppSettings["VietQR_AccountNo"] ?? "113366668888";
                string accountName = ConfigurationManager.AppSettings["VietQR_AccountName"] ?? "VIETQR ACCOUNT";

                decimal amount = decimal.Parse(txtPaymentAmount.Text.Replace(",", "").Replace(" VNĐ", ""));
                string desc = BuildVietQRDescription(_order);

                var service = new VietQRIntegrationService();
                var quick = service.GenerateQuickLink(accountNo, accountName, amount, desc, bankId, VietQRIntegrationService.QRTemplate.Compact2);

                if (!quick.IsSuccess)
                {
                    _overlay.Hide();
                    UXInteractionHelper.ShowError("VietQR", quick.Message);
                    return;
                }

                // Hiển thị panel VietQR nếu đang ẩn
                if (!pnlVietQR.Visible)
                {
                    pnlVietQR.Visible = true;
                    AnimationHelper.FadeIn(pnlVietQR, 250);
                }

                lblQRStatus.Text = "Đang tải ảnh QR...";
                lblQRStatus.ForeColor = Color.FromArgb(59, 130, 246);

                // Tải ảnh Quick Link
                var bmp = await DownloadImageAsync(quick.Data);
                if (bmp != null)
                {
                    picQRCode.Image = bmp;
                    AnimationHelper.ScaleIn(picQRCode, 280);
                    lblQRStatus.Text = "✓ Quick Link đã sẵn sàng";
                    lblQRStatus.ForeColor = ThemeHelper.Colors.Success;
                    txtPaymentInfo.Text = $"Ngân hàng: {bankId}\r\nSố TK: {accountNo}\r\nTên TK: {accountName}\r\nSố tiền: {amount:N0} VNĐ\r\nNội dung: {desc}\r\nTemplate: compact2\r\nURL: {quick.Data}";
                }
                else
                {
                    lblQRStatus.Text = "✗ Không tải được ảnh QR";
                    lblQRStatus.ForeColor = ThemeHelper.Colors.Danger;
                }

                _overlay.Hide();
                UXInteractionHelper.ShowToast(this, "Tạo VietQR (Quick Link) thành công!", 1800, ThemeHelper.Colors.Success, Color.White);
            }
            catch (Exception ex)
            {
                _overlay.Hide();
                UXInteractionHelper.ShowError("VietQR", ex.Message);
            }
        }

        private async Task<Bitmap> DownloadImageAsync(string url)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var bytes = await client.GetByteArrayAsync(url);
                    using (var ms = new MemoryStream(bytes))
                    {
                        return new Bitmap(ms);
                    }
                }
            }
            catch { return null; }
        }

        private string BuildVietQRDescription(OrderDTO order)
        {
            // Mô tả tối đa 25 ký tự theo quy định
            // Mặc định: TT_yyyyMMdd_suffix6
            string suffix = order?.OrderNumber;
            if (!string.IsNullOrEmpty(suffix) && suffix.Length > 6)
                suffix = suffix.Substring(suffix.Length - 6);
            string s = $"TT_{(order?.OrderDate ?? DateTime.Now):yyyyMMdd}_{suffix}";
            if (s.Length > 25) s = s.Substring(0, 25);
            return s;
        }

        private void FormPayment_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.F5)
                {
                    LoadOrderData();
                    RefreshQRCode();
                    e.Handled = true;
                }
                else if (e.KeyCode == Keys.Enter)
                {
                    btnConfirmPayment_Click(sender, EventArgs.Empty);
                    e.Handled = true;
                }
                else if (e.KeyCode == Keys.Escape)
                {
                    this.Close();
                    e.Handled = true;
                }
                else if (e.Control && e.KeyCode == Keys.Q)
                {
                    btnVietQRQuickLink_Click(sender, EventArgs.Empty);
                    e.Handled = true;
                }
                else if (e.KeyCode == Keys.F1)
                {
                    // Mở Hướng dẫn phím tắt
                    try
                    {
                        var form = new QLTN_LT.GUI.Helper.FormShortcuts();
                        form.ShowDialog(this);
                    }
                    catch { }
                }
                else if (e.Control && e.KeyCode == Keys.F1)
                {
                    // Mở VietQR nâng cao (giữ như tuỳ chọn)
                    try
                    {
                        var form = new QLTN_LT.GUI.Payment.FormVietQRGenerator(_order);
                        form.ShowDialog(this);
                    }
                    catch { }
                }
            }
            catch { }
        }
    }
}
