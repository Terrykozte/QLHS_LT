using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace QLTN_LT.GUI.Helper
{
    /// <summary>
    /// Centralized exception handling for the application.
    /// Provides logging and user-friendly error messages.
    /// </summary>
    public static class ExceptionHandler
    {
        private static readonly string LogDirectory = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory, "Logs");

        static ExceptionHandler()
        {
            try
            {
                Directory.CreateDirectory(LogDirectory);
            }
            catch { }
        }

        /// <summary>
        /// Handles an exception with logging and user notification.
        /// </summary>
        public static void Handle(Exception ex, string context = "", bool showDialog = true)
        {
            if (ex == null) return;

            try
            {
                // Log to file
                LogToFile(ex, context);

                // Log to debug
                Debug.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {context}: {ex.Message}");

                // Show user-friendly message
                if (showDialog)
                {
                    string userMessage = GetUserFriendlyMessage(ex);
                    MessageBox.Show(
                        userMessage,
                        "Lỗi",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
            catch
            {
                // Fail silently to prevent infinite loops
            }
        }

        /// <summary>
        /// Handles an exception and returns a result object.
        /// </summary>
        public static DTO.Result HandleWithResult(Exception ex, string context = "")
        {
            Handle(ex, context, showDialog: false);
            return new DTO.Result
            {
                IsSuccess = false,
                Message = GetUserFriendlyMessage(ex)
            };
        }

        /// <summary>
        /// Logs exception to file.
        /// </summary>
        private static void LogToFile(Exception ex, string context)
        {
            try
            {
                string logFile = Path.Combine(LogDirectory, $"error_{DateTime.Now:yyyy-MM-dd}.log");
                string logMessage = FormatLogMessage(ex, context);
                File.AppendAllText(logFile, logMessage + Environment.NewLine);
            }
            catch
            {
                // Fail silently
            }
        }

        /// <summary>
        /// Formats exception for logging.
        /// </summary>
        private static string FormatLogMessage(Exception ex, string context)
        {
            return $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {context}" + Environment.NewLine +
                   $"Type: {ex.GetType().Name}" + Environment.NewLine +
                   $"Message: {ex.Message}" + Environment.NewLine +
                   $"StackTrace: {ex.StackTrace}" + Environment.NewLine +
                   "---";
        }

        /// <summary>
        /// Gets user-friendly error message based on exception type.
        /// </summary>
        private static string GetUserFriendlyMessage(Exception ex)
        {
            if (ex == null)
                return "Đã xảy ra lỗi không xác định.";

            // Check inner exception first
            if (ex.InnerException != null)
                return GetUserFriendlyMessage(ex.InnerException);

            // Map exception types to user messages
            if (ex is ArgumentNullException)
                return "Dữ liệu bắt buộc không được để trống.";

            if (ex is ArgumentException)
                return "Dữ liệu nhập không hợp lệ: " + ex.Message;

            if (ex is InvalidOperationException)
                return "Thao tác không hợp lệ: " + ex.Message;

            if (ex is System.Data.SqlClient.SqlException sqlEx)
            {
                if (sqlEx.Message.IndexOf("timeout", StringComparison.OrdinalIgnoreCase) >= 0)
                    return "Kết nối cơ sở dữ liệu hết thời gian chờ. Vui lòng thử lại.";
                if (sqlEx.Message.IndexOf("connection", StringComparison.OrdinalIgnoreCase) >= 0)
                    return "Không thể kết nối đến cơ sở dữ liệu. Vui lòng kiểm tra kết nối.";
                return "Lỗi cơ sở dữ liệu: " + sqlEx.Message;
            }

            if (ex is System.Net.Http.HttpRequestException)
                return "Lỗi kết nối mạng. Vui lòng kiểm tra kết nối internet.";

            if (ex is UnauthorizedAccessException)
                return "Bạn không có quyền thực hiện thao tác này.";

            if (ex is FileNotFoundException)
                return "Tệp không tìm thấy.";

            if (ex is DirectoryNotFoundException)
                return "Thư mục không tìm thấy.";

            if (ex is NotImplementedException)
                return "Chức năng này chưa được triển khai.";

            // Default message
            return "Đã xảy ra lỗi: " + ex.Message;
        }

        /// <summary>
        /// Safely executes an action with exception handling.
        /// </summary>
        public static void TrySafe(Action action, string context = "")
        {
            try
            {
                action?.Invoke();
            }
            catch (Exception ex)
            {
                Handle(ex, context);
            }
        }

        /// <summary>
        /// Safely executes a function with exception handling.
        /// </summary>
        public static T TrySafe<T>(Func<T> func, T defaultValue = default, string context = "")
        {
            try
            {
                return func != null ? func() : defaultValue;
            }
            catch (Exception ex)
            {
                Handle(ex, context, showDialog: false);
                return defaultValue;
            }
        }
    }
}

