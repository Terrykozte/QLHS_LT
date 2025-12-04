using System;
using System.Windows.Forms;
using Guna.UI2.WinForms;
using System.Diagnostics;

namespace QLTN_LT.GUI.Helper
{
    /// <summary>
    /// Helper class for displaying user messages with consistent styling and logging.
    /// </summary>
    public static class UserMessage
    {
        private static readonly string LogSource = "UserMessage";

        /// <summary>
        /// Shows an error message to the user.
        /// </summary>
        public static void ShowError(string message, string title = "Lỗi")
        {
            try
            {
                LogMessage($"[ERROR] {title}: {message}", EventLogEntryType.Error);
                MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error showing error message: {ex.Message}");
            }
        }

        /// <summary>
        /// Shows an information message to the user.
        /// </summary>
        public static void ShowInfo(string message, string title = "Thông báo")
        {
            try
            {
                LogMessage($"[INFO] {title}: {message}", EventLogEntryType.Information);
                MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error showing info message: {ex.Message}");
            }
        }

        /// <summary>
        /// Shows a warning message to the user.
        /// </summary>
        public static void ShowWarning(string message, string title = "Cảnh báo")
        {
            try
            {
                LogMessage($"[WARNING] {title}: {message}", EventLogEntryType.Warning);
                MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error showing warning message: {ex.Message}");
            }
        }

        /// <summary>
        /// Shows a confirmation dialog to the user.
        /// </summary>
        public static bool ShowConfirm(string message, string title = "Xác nhận")
        {
            try
            {
                LogMessage($"[CONFIRM] {title}: {message}", EventLogEntryType.Information);
                DialogResult result = MessageBox.Show(message, title, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                return result == DialogResult.Yes;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error showing confirm dialog: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Shows a success message to the user.
        /// </summary>
        public static void ShowSuccess(string message, string title = "Thành công")
        {
            try
            {
                LogMessage($"[SUCCESS] {title}: {message}", EventLogEntryType.SuccessAudit);
                MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error showing success message: {ex.Message}");
            }
        }

        /// <summary>
        /// Logs a message to the debug output.
        /// </summary>
        private static void LogMessage(string message, EventLogEntryType type)
        {
            try
            {
                string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                string logMessage = $"[{timestamp}] {LogSource}: {message}";
                Debug.WriteLine(logMessage);
            }
            catch
            {
                // Silently fail if logging fails
            }
        }
    }
}
