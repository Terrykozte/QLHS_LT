using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using QLTN_LT.BLL;
using QLTN_LT.DTO;
using QRCoder;

using QLTN_LT.GUI.Base;
using QLTN_LT.GUI.Helper;

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
        }

        private void FormPayment_Load(object sender, EventArgs e)
        {
            try
            {
                LoadOrderData();
                LoadPaymentMethods();
                SetupUI();
                
                // Start auto-refresh timer
                _autoRefreshTimer.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải dữ liệu: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private void FormPayment_FormClosing(object sender, FormClosingEventArgs e)
        {
            _autoRefreshTimer?.Stop();
            _autoRefreshTimer?.Dispose();
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
        }

        private void CmbPaymentMethod_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selectedMethod = (PaymentMethodDTO)cmbPaymentMethod.SelectedItem;
            
            // Show/Hide VietQR controls based on selected method
            if (selectedMethod.MethodName == "VietQR")
            {
                pnlVietQR.Visible = true;
                GenerateVietQRCode();
            }
            else
            {
                pnlVietQR.Visible = false;
            }
        }

        private void GenerateVietQRCode()
        {
            try
            {
                // Thông tin tài khoản ngân hàng (cần cấu hình từ settings)
                string bankCode = "970422"; // Techcombank
                string accountNumber = "0123456789"; // Số tài khoản
                string accountName = "NHA HANG HAI SAN"; // Tên tài khoản
                
                decimal amount = decimal.Parse(txtPaymentAmount.Text.Replace(",", "").Replace(" VNĐ", ""));
                string description = $"Thanh toan don hang {_order.OrderNumber}";

                var vietQRService = new VietQRService(bankCode, accountNumber, accountName, amount, description);
                
                // Tạo QR Code bằng QRCoder
                string qrData = vietQRService.GenerateQRCode();
                
                using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
                {
                    QRCodeData qrCodeData = qrGenerator.CreateQrCode(qrData, QRCodeGenerator.ECCLevel.Q);
                    using (QRCode qrCode = new QRCode(qrCodeData))
                    {
                        Bitmap qrCodeImage = qrCode.GetGraphic(10);
                        picQRCode.Image = qrCodeImage;
                    }
                }

                // Hiển thị thông tin thanh toán
                txtPaymentInfo.Text = vietQRService.GeneratePaymentInfo();
                lblQRStatus.Text = "QR Code được tạo thành công";
                lblQRStatus.ForeColor = Color.Green;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tạo QR Code: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                lblQRStatus.Text = "Lỗi tạo QR Code";
                lblQRStatus.ForeColor = Color.Red;
            }
        }

        private void btnConfirmPayment_Click(object sender, EventArgs e)
        {
            try
            {
                if (!ValidatePaymentInput())
                    return;

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
                    MessageBox.Show(
                        $"Thanh toán thành công!\n\nMã giao dịch: {response.TransactionCode}\nSố tiền: {response.Amount:N0} VNĐ",
                        "Thông báo",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                    
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MessageBox.Show(
                        $"Lỗi thanh toán: {response.Message}",
                        "Lỗi",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi xử lý thanh toán: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool ValidatePaymentInput()
        {
            if (string.IsNullOrWhiteSpace(txtPaymentAmount.Text))
            {
                MessageBox.Show("Vui lòng nhập số tiền thanh toán", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (!decimal.TryParse(txtPaymentAmount.Text.Replace(",", "").Replace(" VNĐ", ""), out decimal amount))
            {
                MessageBox.Show("Số tiền không hợp lệ", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (amount <= 0)
            {
                MessageBox.Show("Số tiền phải lớn hơn 0", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            var remaining = decimal.Parse(lblRemainingAmount.Text.Replace("Còn lại: ", "").Replace(" VNĐ", ""));
            if (amount > remaining)
            {
                MessageBox.Show($"Số tiền thanh toán không được vượt quá {remaining:N0} VNĐ", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
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
            }
            catch { }
        }
    }
}

