using System;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace QLTN_LT.BLL
{
    /// <summary>
    /// VietQR Service - Tích hợp thanh toán QR Code
    /// Tham khảo: https://github.com/vietqr/vietqr-node
    /// </summary>
    public class VietQRService
    {
        private readonly string _bankCode;
        private readonly string _accountNumber;
        private readonly string _accountName;
        private readonly decimal _amount;
        private readonly string _description;

        public VietQRService(string bankCode, string accountNumber, string accountName, decimal amount, string description)
        {
            _bankCode = bankCode ?? throw new ArgumentNullException(nameof(bankCode));
            _accountNumber = accountNumber ?? throw new ArgumentNullException(nameof(accountNumber));
            _accountName = accountName ?? throw new ArgumentNullException(nameof(accountName));
            _amount = amount;
            _description = description ?? "Thanh toan don hang";
        }

        /// <summary>
        /// Tạo QR Code từ thông tin thanh toán
        /// </summary>
        public string GenerateQRCode()
        {
            try
            {
                // Format: https://vietqr.io/api/generate
                // Tham số: bankCode, accountNo, accountName, amount, description, addInfo
                var qrUrl = BuildQRUrl();
                return qrUrl;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi tạo QR Code: {ex.Message}");
            }
        }

        /// <summary>
        /// Xây dựng URL QR Code từ VietQR API
        /// </summary>
        private string BuildQRUrl()
        {
            // Mã ngân hàng VietQR (ví dụ: 970422 cho Techcombank)
            // Danh sách mã ngân hàng: https://vietqr.io/api/banks
            
            var baseUrl = "https://api.vietqr.io/v2/generate";
            
            // Encode thông tin
            var encodedAccountName = Uri.EscapeDataString(_accountName);
            var encodedDescription = Uri.EscapeDataString(_description);
            
            // Xây dựng URL với tham số
            var qrUrl = $"{baseUrl}?" +
                       $"bankCode={_bankCode}&" +
                       $"accountNo={_accountNumber}&" +
                       $"accountName={encodedAccountName}&" +
                       $"amount={(long)_amount}&" +
                       $"description={encodedDescription}";

            return qrUrl;
        }

        /// <summary>
        /// Lấy dữ liệu QR Code dưới dạng Base64 hoặc URL
        /// </summary>
        public async Task<QRCodeResponse> GetQRCodeAsync()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var url = BuildQRUrl();
                    var response = await client.GetAsync(url);
                    
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        
                        // Parse JSON response
                        // Giả sử API trả về JSON với trường "data" chứa URL hoặc Base64
                        return new QRCodeResponse
                        {
                            Success = true,
                            QRUrl = url,
                            Message = "Tạo QR Code thành công"
                        };
                    }
                    else
                    {
                        return new QRCodeResponse
                        {
                            Success = false,
                            Message = $"Lỗi API: {response.StatusCode}"
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                return new QRCodeResponse
                {
                    Success = false,
                    Message = $"Lỗi: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// Tạo chuỗi thông tin thanh toán cho QR Code
        /// </summary>
        public string GeneratePaymentInfo()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Ngân hàng: {_bankCode}");
            sb.AppendLine($"Số tài khoản: {_accountNumber}");
            sb.AppendLine($"Tên tài khoản: {_accountName}");
            sb.AppendLine($"Số tiền: {_amount:N0} VNĐ");
            sb.AppendLine($"Nội dung: {_description}");
            return sb.ToString();
        }

        /// <summary>
        /// Lấy danh sách các ngân hàng hỗ trợ VietQR
        /// </summary>
        public static async Task<List<BankInfo>> GetSupportedBanksAsync()
        {
            var banks = new List<BankInfo>
            {
                new BankInfo { Code = "970422", Name = "Techcombank" },
                new BankInfo { Code = "970407", Name = "Vietcombank" },
                new BankInfo { Code = "970403", Name = "Agribank" },
                new BankInfo { Code = "970416", Name = "BIDV" },
                new BankInfo { Code = "970418", Name = "MB Bank" },
                new BankInfo { Code = "970428", Name = "Eximbank" },
                new BankInfo { Code = "970432", Name = "Sacombank" },
                new BankInfo { Code = "970433", Name = "Saigonbank" },
                new BankInfo { Code = "970434", Name = "Kienlongbank" },
                new BankInfo { Code = "970435", Name = "Baoviethbank" },
                new BankInfo { Code = "970436", Name = "Phuongdongbank" },
                new BankInfo { Code = "970437", Name = "Oceanbank" },
                new BankInfo { Code = "970438", Name = "Vietinbank" },
                new BankInfo { Code = "970440", Name = "TMCP Quân Đội" },
                new BankInfo { Code = "970441", Name = "Ngân hàng Công Thương" },
                new BankInfo { Code = "970443", Name = "Ngân hàng Phát triển" },
                new BankInfo { Code = "970449", Name = "ACB" },
                new BankInfo { Code = "970450", Name = "Bảo Việt" },
                new BankInfo { Code = "970451", Name = "Tiên Phong" },
                new BankInfo { Code = "970452", Name = "Thương Mại" },
                new BankInfo { Code = "970453", Name = "Quốc Tế" },
                new BankInfo { Code = "970454", Name = "Kỹ Thương" },
                new BankInfo { Code = "970455", Name = "Công Nghiệp" },
                new BankInfo { Code = "970456", Name = "Ngoại Thương" }
            };

            return await Task.FromResult(banks);
        }
    }

    public class QRCodeResponse
    {
        public bool Success { get; set; }
        public string QRUrl { get; set; }
        public string QRBase64 { get; set; }
        public string Message { get; set; }
    }

    public class BankInfo
    {
        public string Code { get; set; }
        public string Name { get; set; }
    }
}

