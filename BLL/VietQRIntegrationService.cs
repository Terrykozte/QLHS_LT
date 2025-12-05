using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using QLTN_LT.DTO;

namespace QLTN_LT.BLL
{
    /// <summary>
    /// Service tích hợp VietQR để tạo mã QR thanh toán
    /// </summary>
    public class VietQRIntegrationService
    {
        // VietQR API endpoints
        private const string VIETQR_API_BASE = "https://api.vietqr.io/v2";
        private const string VIETQR_QUICK_LINK_BASE = "https://img.vietqr.io/image";

        // VietQR credentials (cần cấu hình từ My VietQR)
        private readonly string _clientId;
        private readonly string _apiKey;

        // Bank information
        private const string DEFAULT_BANK_ID = "970415"; // VietinBank
        private const string DEFAULT_BANK_NAME = "VietinBank";

        // Template options
        public enum QRTemplate
        {
            Compact2,  // 540x640 - Bao gồm QR, logo, thông tin chuyển khoản
            Compact,   // 540x540 - QR kèm logo
            QrOnly,    // 480x480 - QR đơn giản
            Print      // 600x776 - QR + logo + đầy đủ thông tin
        }

        public VietQRIntegrationService(string clientId = null, string apiKey = null)
        {
            // Lấy từ config hoặc tham số
            _clientId = clientId ?? GetConfigValue("VietQR_ClientId");
            _apiKey = apiKey ?? GetConfigValue("VietQR_ApiKey");
        }

        /// <summary>
        /// Tạo Quick Link (URL) cho mã QR
        /// </summary>
        public Result<string> GenerateQuickLink(
            string accountNo,
            string accountName,
            decimal amount,
            string description,
            string bankId = DEFAULT_BANK_ID,
            QRTemplate template = QRTemplate.Compact2)
        {
            try
            {
                // Validate input
                if (string.IsNullOrWhiteSpace(accountNo))
                    return new Result<string> { IsSuccess = false, Message = "Số tài khoản không được để trống" };

                if (string.IsNullOrWhiteSpace(accountName))
                    return new Result<string> { IsSuccess = false, Message = "Tên tài khoản không được để trống" };

                if (amount <= 0)
                    return new Result<string> { IsSuccess = false, Message = "Số tiền phải lớn hơn 0" };

                // Chuẩn hóa dữ liệu
                accountNo = accountNo.Trim();
                accountName = NormalizeVietnamese(accountName).ToUpper();
                description = NormalizeVietnamese(description ?? "");
                bankId = bankId.Trim();

                // Tạo Quick Link
                string templateName = GetTemplateName(template);
                string quickLink = $"{VIETQR_QUICK_LINK_BASE}/{bankId}-{accountNo}-{templateName}.png";

                // Thêm parameters
                var parameters = new List<string>();
                parameters.Add($"amount={amount}");
                
                if (!string.IsNullOrWhiteSpace(description))
                    parameters.Add($"addInfo={Uri.EscapeDataString(description)}");
                
                if (!string.IsNullOrWhiteSpace(accountName))
                    parameters.Add($"accountName={Uri.EscapeDataString(accountName)}");

                if (parameters.Count > 0)
                    quickLink += "?" + string.Join("&", parameters);

                return new Result<string> { IsSuccess = true, Data = quickLink };
            }
            catch (Exception ex)
            {
                return new Result<string> { IsSuccess = false, Message = $"Lỗi tạo Quick Link: {ex.Message}" };
            }
        }

        /// <summary>
        /// Tạo mã QR thông qua API VietQR (v2/generate)
        /// </summary>
        public async Task<Result<VietQRResponse>> GenerateQRCodeAsync(
            string accountNo,
            string accountName,
            int acqId,
            decimal amount,
            string addInfo = "",
            string format = "text",
            QRTemplate template = QRTemplate.Compact2)
        {
            try
            {
                // Validate credentials
                if (string.IsNullOrWhiteSpace(_clientId) || string.IsNullOrWhiteSpace(_apiKey))
                    return new Result<VietQRResponse> 
                    { 
                        IsSuccess = false, 
                        Message = "VietQR credentials không được cấu hình. Vui lòng cấu hình Client ID và API Key." 
                    };

                // Validate input
                if (string.IsNullOrWhiteSpace(accountNo))
                    return new Result<VietQRResponse> { IsSuccess = false, Message = "Số tài khoản không được để trống" };

                if (amount <= 0)
                    return new Result<VietQRResponse> { IsSuccess = false, Message = "Số tiền phải lớn hơn 0" };

                // Chuẩn hóa dữ liệu
                accountNo = accountNo.Trim();
                accountName = NormalizeVietnamese(accountName ?? "").ToUpper();
                addInfo = NormalizeVietnamese(addInfo ?? "");

                // Tạo request body
                var requestBody = new
                {
                    accountNo = accountNo,
                    accountName = accountName,
                    acqId = acqId,
                    amount = (long)amount,
                    addInfo = addInfo,
                    format = format,
                    template = GetTemplateName(template)
                };

                using (var client = new HttpClient())
                {
                    // Thêm headers
                    client.DefaultRequestHeaders.Add("x-client-id", _clientId);
                    client.DefaultRequestHeaders.Add("x-api-key", _apiKey);

                    // Tạo request
                    var json = JsonConvert.SerializeObject(requestBody);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    // Gửi request
                    var response = await client.PostAsync($"{VIETQR_API_BASE}/generate", content);

                    if (!response.IsSuccessStatusCode)
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        return new Result<VietQRResponse> 
                        { 
                            IsSuccess = false, 
                            Message = $"VietQR API error: {response.StatusCode} - {errorContent}" 
                        };
                    }

                    // Parse response
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var vietQRResponse = JsonConvert.DeserializeObject<VietQRResponse>(responseContent);

                    if (vietQRResponse?.Code != "00")
                    {
                        return new Result<VietQRResponse> 
                        { 
                            IsSuccess = false, 
                            Message = $"VietQR error: {vietQRResponse?.Desc}" 
                        };
                    }

                    return new Result<VietQRResponse> { IsSuccess = true, Data = vietQRResponse };
                }
            }
            catch (Exception ex)
            {
                return new Result<VietQRResponse> { IsSuccess = false, Message = $"Lỗi tạo mã QR: {ex.Message}" };
            }
        }

        /// <summary>
        /// Tạo mã QR cho đơn hàng
        /// </summary>
        public async Task<Result<VietQRResponse>> GenerateOrderQRCodeAsync(
            OrderDTO order,
            string accountNo,
            string accountName,
            int acqId,
            QRTemplate template = QRTemplate.Compact2)
        {
            try
            {
                if (order == null)
                    return new Result<VietQRResponse> { IsSuccess = false, Message = "Đơn hàng không tồn tại" };

                // Tạo nội dung chuyển khoản từ số đơn hàng
                string addInfo = $"DH{order.OrderNumber}";

                return await GenerateQRCodeAsync(
                    accountNo,
                    accountName,
                    acqId,
                    order.TotalAmount,
                    addInfo,
                    "text",
                    template
                );
            }
            catch (Exception ex)
            {
                return new Result<VietQRResponse> { IsSuccess = false, Message = $"Lỗi: {ex.Message}" };
            }
        }

        /// <summary>
        /// Tạo Quick Link cho đơn hàng
        /// </summary>
        public Result<string> GenerateOrderQuickLink(
            OrderDTO order,
            string accountNo,
            string accountName,
            string bankId = DEFAULT_BANK_ID,
            QRTemplate template = QRTemplate.Compact2)
        {
            try
            {
                if (order == null)
                    return new Result<string> { IsSuccess = false, Message = "Đơn hàng không tồn tại" };

                // Tạo nội dung chuyển khoản từ số đơn hàng
                string description = $"DH{order.OrderNumber}";

                return GenerateQuickLink(
                    accountNo,
                    accountName,
                    order.TotalAmount,
                    description,
                    bankId,
                    template
                );
            }
            catch (Exception ex)
            {
                return new Result<string> { IsSuccess = false, Message = $"Lỗi: {ex.Message}" };
            }
        }

        /// <summary>
        /// Lấy danh sách ngân hàng hỗ trợ
        /// </summary>
        public async Task<Result<List<BankDTO>>> GetBanksAsync()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var response = await client.GetAsync($"{VIETQR_API_BASE}/banks");

                    if (!response.IsSuccessStatusCode)
                        return new Result<List<BankDTO>> 
                        { 
                            IsSuccess = false, 
                            Message = $"Lỗi lấy danh sách ngân hàng: {response.StatusCode}" 
                        };

                    var content = await response.Content.ReadAsStringAsync();
                    var banksResponse = JsonConvert.DeserializeObject<BanksResponse>(content);

                    if (banksResponse?.Code != "00")
                        return new Result<List<BankDTO>> 
                        { 
                            IsSuccess = false, 
                            Message = $"Lỗi: {banksResponse?.Desc}" 
                        };

                    return new Result<List<BankDTO>> { IsSuccess = true, Data = banksResponse.Data };
                }
            }
            catch (Exception ex)
            {
                return new Result<List<BankDTO>> { IsSuccess = false, Message = $"Lỗi: {ex.Message}" };
            }
        }

        /// <summary>
        /// Chuẩn hóa tiếng Việt (loại bỏ dấu)
        /// </summary>
        private string NormalizeVietnamese(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return "";

            // Loại bỏ dấu tiếng Việt
            var vietnameseChars = new Dictionary<char, char>
            {
                {'à', 'a'}, {'á', 'a'}, {'ả', 'a'}, {'ã', 'a'}, {'ạ', 'a'},
                {'ă', 'a'}, {'ằ', 'a'}, {'ắ', 'a'}, {'ẳ', 'a'}, {'ẵ', 'a'}, {'ặ', 'a'},
                {'â', 'a'}, {'ầ', 'a'}, {'ấ', 'a'}, {'ẩ', 'a'}, {'ẫ', 'a'}, {'ậ', 'a'},
                {'đ', 'd'},
                {'è', 'e'}, {'é', 'e'}, {'ẻ', 'e'}, {'ẽ', 'e'}, {'ẹ', 'e'},
                {'ê', 'e'}, {'ề', 'e'}, {'ế', 'e'}, {'ể', 'e'}, {'ễ', 'e'}, {'ệ', 'e'},
                {'ì', 'i'}, {'í', 'i'}, {'ỉ', 'i'}, {'ĩ', 'i'}, {'ị', 'i'},
                {'ò', 'o'}, {'ó', 'o'}, {'ỏ', 'o'}, {'õ', 'o'}, {'ọ', 'o'},
                {'ô', 'o'}, {'ồ', 'o'}, {'ố', 'o'}, {'ổ', 'o'}, {'ỗ', 'o'}, {'ộ', 'o'},
                {'ơ', 'o'}, {'ờ', 'o'}, {'ớ', 'o'}, {'ở', 'o'}, {'ỡ', 'o'}, {'ợ', 'o'},
                {'ù', 'u'}, {'ú', 'u'}, {'ủ', 'u'}, {'ũ', 'u'}, {'ụ', 'u'},
                {'ư', 'u'}, {'ừ', 'u'}, {'ứ', 'u'}, {'ử', 'u'}, {'ữ', 'u'}, {'ự', 'u'},
                {'ỳ', 'y'}, {'ý', 'y'}, {'ỷ', 'y'}, {'ỹ', 'y'}, {'ỵ', 'y'}
            };

            var result = new StringBuilder();
            foreach (char c in input)
            {
                if (vietnameseChars.ContainsKey(c))
                    result.Append(vietnameseChars[c]);
                else
                    result.Append(c);
            }

            return result.ToString();
        }

        /// <summary>
        /// Lấy tên template từ enum
        /// </summary>
        private string GetTemplateName(QRTemplate template)
        {
            return template switch
            {
                QRTemplate.Compact2 => "compact2",
                QRTemplate.Compact => "compact",
                QRTemplate.QrOnly => "qr_only",
                QRTemplate.Print => "print",
                _ => "compact2"
            };
        }

        /// <summary>
        /// Lấy giá trị từ config
        /// </summary>
        private string GetConfigValue(string key)
        {
            try
            {
                var config = System.Configuration.ConfigurationManager.AppSettings[key];
                return config ?? "";
            }
            catch
            {
                return "";
            }
        }
    }

    /// <summary>
    /// Response từ VietQR API
    /// </summary>
    public class VietQRResponse
    {
        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("desc")]
        public string Desc { get; set; }

        [JsonProperty("data")]
        public VietQRData Data { get; set; }
    }

    /// <summary>
    /// Dữ liệu VietQR
    /// </summary>
    public class VietQRData
    {
        [JsonProperty("qrCode")]
        public string QrCode { get; set; }

        [JsonProperty("qrDataURL")]
        public string QrDataURL { get; set; }
    }

    /// <summary>
    /// Response danh sách ngân hàng
    /// </summary>
    public class BanksResponse
    {
        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("desc")]
        public string Desc { get; set; }

        [JsonProperty("data")]
        public List<BankDTO> Data { get; set; }
    }

    /// <summary>
    /// Thông tin ngân hàng
    /// </summary>
    public class BankDTO
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("bin")]
        public string Bin { get; set; }

        [JsonProperty("shortName")]
        public string ShortName { get; set; }

        [JsonProperty("logo")]
        public string Logo { get; set; }
    }
}

