using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Data.SqlClient;
using QLTN_LT.DAL;
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
        private System.Windows.Forms.Timer _autoPayTimer;
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
            _autoRefreshTimer.Enabled = false; // Không tự động bật

            // Auto pay timer (tự động xác nhận thanh toán tiền mặt)
            _autoPayTimer = new System.Windows.Forms.Timer();
            _autoPayTimer.Interval = 700; // 0.7s sau khi form sẵn sàng
            _autoPayTimer.Tick += AutoPayTimer_Tick;

            this.KeyPreview = true;
            this.KeyDown += FormPayment_KeyDown;

            // Overlay
            _overlay = new LoadingOverlay(this, "Đang xử lý...");

            // Đăng ký sự kiện đóng form
            this.FormClosing += FormPayment_FormClosing;
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
                
                // Start auto-refresh timer (chỉ sau khi tất cả đã sẵn sàng)
                if (_autoRefreshTimer != null && !_autoRefreshTimer.Enabled)
                {
                    _autoRefreshTimer.Start();
                }
                // Bắt đầu auto-pay sau khi mọi thứ sẵn sàng
                _autoPayTimer?.Start();
            }
            catch (Exception ex)
            {
                UXInteractionHelper.ShowError("Lỗi", $"Lỗi tải dữ liệu: {ex.Message}");
            }
        }
        
        private void FormPayment_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                // Dừng timer ngay lập tức
                if (_autoRefreshTimer != null)
                {
                    _autoRefreshTimer.Stop();
                    _autoRefreshTimer.Tick -= AutoRefreshTimer_Tick;
                    _autoRefreshTimer.Dispose();
                    _autoRefreshTimer = null;
                }

                // Ẩn overlay nếu đang hiển thị
                if (_overlay != null)
                {
                    try { _overlay.Hide(); } catch { }
                    _overlay.Dispose();
                    _overlay = null;
                }

                // Unregister keyboard navigation
            KeyboardNavigationHelper.UnregisterForm(this);

                // Xóa tất cả event handlers
                this.KeyDown -= FormPayment_KeyDown;
                this.KeyPreview = false;

                // Xóa các controls
                if (cmbPaymentMethod != null)
                    cmbPaymentMethod.SelectedIndexChanged -= CmbPaymentMethod_SelectedIndexChanged;

                // Dispose các resources
                if (picQRCode != null && picQRCode.Image != null)
                {
                    picQRCode.Image?.Dispose();
                    picQRCode.Image = null;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in FormPayment_FormClosing: {ex.Message}");
            }
        }
        
        private void AutoRefreshTimer_Tick(object sender, EventArgs e)
        {
            // Tự động refresh dữ liệu đơn hàng mỗi 2 giây và kiểm tra trạng thái thanh toán
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

                // Kiểm tra trạng thái thanh toán qua service
                RefreshPaymentStatus();
            }
            catch
            {
                // Ignore errors in auto-refresh
            }
        }

        private void AutoPayTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                _autoPayTimer?.Stop();

                // Chọn mặc định phương thức Tiền mặt/Cash nếu có
                if (_paymentMethods == null || _paymentMethods.Count == 0)
                {
                    LoadPaymentMethods();
                }
                if (cmbPaymentMethod != null && _paymentMethods != null && _paymentMethods.Count > 0)
                {
                    var cash = _paymentMethods.FirstOrDefault(m =>
                        m.MethodName != null && (m.MethodName.IndexOf("tiền", StringComparison.OrdinalIgnoreCase) >= 0
                                              || m.MethodName.IndexOf("cash", StringComparison.OrdinalIgnoreCase) >= 0));
                    if (cash != null)
                    {
                        for (int i = 0; i < cmbPaymentMethod.Items.Count; i++)
                        {
                            if (cmbPaymentMethod.Items[i] is PaymentMethodDTO pm && pm.PaymentMethodID == cash.PaymentMethodID)
                            {
                                cmbPaymentMethod.SelectedIndex = i;
                                break;
                            }
                        }
                    }
                    else
                    {
                        cmbPaymentMethod.SelectedIndex = 0;
                    }
                }

                // Set số tiền = còn lại
                var paidAmount = _paymentBLL.CalculateTotalPaid(_orderId);
                var remaining = _order.TotalAmount - paidAmount;
                if (remaining <= 0)
                {
                    // Không cần auto-pay nếu không còn dư
                    return;
                }
                txtPaymentAmount.Text = remaining.ToString("N0");

                // Tự động xác nhận thanh toán
                btnConfirmPayment_Click(sender, EventArgs.Empty);
            }
            catch { }
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

                // Sử dụng Quick Link với tài khoản cố định VCB
                const string bankId = "vietcombank"; // hoặc 970407
                const string accountNumber = "1031839610";
                const string accountName = "PHAM HOAI THUONG";

                decimal amount = decimal.Parse(txtPaymentAmount.Text.Replace(",", "").Replace(" VNĐ", ""));
                string description = BuildVietQRDescription(_order);

                var service = new VietQRIntegrationService();
                var quick = service.GenerateQuickLink(accountNumber, accountName, amount, description, bankId, VietQRIntegrationService.QRTemplate.Compact2);
                if (!quick.IsSuccess)
                {
                    lblQRStatus.Text = "✗ Lỗi tạo Quick Link";
                    lblQRStatus.ForeColor = Color.FromArgb(239, 68, 68);
                    return;
                }

                // Tải ảnh QR từ Quick Link
                var bmp = DownloadImageAsync(quick.Data).GetAwaiter().GetResult();
                if (bmp != null && picQRCode != null)
                {
                    picQRCode.Image = bmp;
                    AnimationHelper.ScaleIn(picQRCode, 300);
                }

                // Hiển thị thông tin thanh toán
                if (txtPaymentInfo != null)
                {
                    txtPaymentInfo.Text = $"Ngân hàng: {bankId}\r\nSố tài khoản: {accountNumber}\r\nTên tài khoản: {accountName}\r\nSố tiền: {amount:N0} VNĐ\r\nNội dung: {description}";
                    AnimationHelper.FadeIn(txtPaymentInfo, 300);
                }

                if (lblQRStatus != null)
                {
                    lblQRStatus.Text = "✓ QR Code được tạo thành công";
                    lblQRStatus.ForeColor = Color.FromArgb(34, 197, 94);
                    AnimationHelper.Pulse(lblQRStatus, 400);
                }

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
                    
                    // Dừng timer ngay lập tức
                    if (_autoRefreshTimer != null)
                    {
                        _autoRefreshTimer.Stop();
                    }

                    // Ẩn overlay
                    if (_overlay != null)
                    {
                        try { _overlay.Hide(); } catch { }
                    }
                    
                    // Delay before closing
                    var closeTimer = new Timer { Interval = 1500 };
                    closeTimer.Tick += (s, e2) =>
                    {
                        closeTimer.Stop();
                        closeTimer.Dispose();
                        try
                        {
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                        }
                        catch { }
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

                    // Ẩn overlay nếu đang hiển thị
                    if (_overlay != null)
                    {
                        try { _overlay.Hide(); } catch { }
                    }
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
            try
            {
                // Dừng timer trước khi đóng
                if (_autoRefreshTimer != null)
                {
                    _autoRefreshTimer.Stop();
                }

                // Ẩn overlay
                if (_overlay != null)
                {
                    try { _overlay.Hide(); } catch { }
                }

            this.DialogResult = DialogResult.Cancel;
            this.Close();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in btnCancel_Click: {ex.Message}");
                this.Close();
            }
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
                    if (UIHelper.ShowSaveFileDialog(this, sfd) == DialogResult.OK)
                    {
                        var exporter = new InvoiceExportService();
                        if (exporter.ExportToExcel(_orderId, sfd.FileName))
                        {
                            UIHelper.ShowMessageBox(this, "Xuất Excel thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            UIHelper.ShowMessageBox(this, "Xuất Excel thất bại.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                UIHelper.ShowMessageBox(this, "Lỗi xuất hóa đơn: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                    if (UIHelper.ShowSaveFileDialog(this, sfd) == DialogResult.OK)
                    {
                        var exporter = new InvoiceExportService();
                        if (exporter.ExportToPdf(_orderId, sfd.FileName))
                        {
                            UIHelper.ShowMessageBox(this, "Xuất PDF thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            UIHelper.ShowMessageBox(this, "Xuất PDF thất bại.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                UIHelper.ShowMessageBox(this, "Lỗi xuất PDF: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // NEW: Button Quick Link VietQR
        private async void btnVietQRQuickLink_Click(object sender, EventArgs e)
        {
            try
            {
                _overlay.Show("Đang tạo VietQR (Quick Link)...");

                // Chuẩn bị thông tin VietQR (gán cứng theo yêu cầu)
                const string bankId = "vietcombank"; // hoặc 970407
                const string accountNo = "1031839610";
                const string accountName = "PHAM HOAI THUONG";

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
            // Không dấu + dùng dấu gạch dưới, tối đa 50 ký tự
            string code = order?.OrderNumber ?? DateTime.Now.ToString("yyyyMMddHHmmss");
            // Chuẩn hóa: chỉ chữ/số và _
            var sb = new System.Text.StringBuilder();
            sb.Append("DON_HANG_");
            foreach (var ch in code)
            {
                if (char.IsLetterOrDigit(ch)) sb.Append(char.ToUpperInvariant(ch));
                else sb.Append('_');
            }
            string s = sb.ToString();
            if (s.Length > 50) s = s.Substring(0, 50);
            return s;
        }

        private void RefreshPaymentStatus()
        {
            try
            {
                var status = _confirmationService.GetPaymentStatus(_orderId);
                if (status == null) return;

                if (lblTotalAmount != null)
                    lblTotalAmount.Text = $"Tổng tiền: {status.TotalAmount:N0} VNĐ";
                if (lblPaidAmount != null)
                    lblPaidAmount.Text = $"Đã thanh toán: {status.TotalPaid:N0} VNĐ";
                if (lblRemainingAmount != null)
                    lblRemainingAmount.Text = $"Còn lại: {status.RemainingAmount:N0} VNĐ";

                if (!string.IsNullOrEmpty(status.PaymentStatus) && status.PaymentStatus.Equals("Paid", StringComparison.OrdinalIgnoreCase))
                {
                    if (lblQRStatus != null)
                    {
                        lblQRStatus.Text = "✓ ĐÃ THANH TOÁN THÀNH CÔNG";
                        lblQRStatus.ForeColor = System.Drawing.Color.FromArgb(34, 197, 94);
                        AnimationHelper.Pulse(lblQRStatus, 400);
                    }
                    _autoRefreshTimer?.Stop();
                }
            }
            catch { }
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
                        UIHelper.ShowFormDialog(this, form);
                    }
                    catch { }
                }
                else if (e.Control && e.KeyCode == Keys.F1)
                {
                    // Mở VietQR nâng cao (giữ như tuỳ chọn)
                    try
                    {
                        var form = new QLTN_LT.GUI.Payment.FormVietQRGenerator(_order);
                        UIHelper.ShowFormDialog(this, form);
                    }
                    catch { }
                }
            }
            catch { }
        }
    }
}
