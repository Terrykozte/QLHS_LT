using System;
using System.Windows.Forms;
using QLTN_LT.BLL;
using QLTN_LT.GUI.Base;
using QLTN_LT.GUI.Helper;

namespace QLTN_LT.GUI.Helper
{
    public partial class FormTestDataGenerator : BaseForm
    {
        private TestDataGenerator _testDataGenerator;

        public FormTestDataGenerator()
        {
            InitializeComponent();
            _testDataGenerator = new TestDataGenerator();
            
            try
            {
                UIHelper.ApplyFormStyle(this);
            }
            catch { }
        }

        private void FormTestDataGenerator_Load(object sender, EventArgs e)
        {
            try
            {
                lblTitle.Text = "🧪 Công Cụ Tạo Dữ Liệu Test";
                lblDescription.Text = "Tạo dữ liệu test Menu để test API VietQR\n\n" +
                    "✅ 5 danh mục hải sản\n" +
                    "✅ 18 món ăn với giá test\n" +
                    "✅ Dữ liệu đầy đủ cho testing";

                UpdateStatistics();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnGenerateData_Click(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show(
                    "Bạn có chắc muốn tạo dữ liệu test?\n\n" +
                    "Sẽ tạo:\n" +
                    "• 5 danh mục\n" +
                    "• 18 món ăn\n\n" +
                    "Dữ liệu cũ sẽ không bị xóa.",
                    "Xác Nhận",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    Wait(true);
                    _testDataGenerator.GenerateAllTestData();
                    Wait(false);

                    MessageBox.Show(
                        "✅ Tạo dữ liệu test thành công!\n\n" +
                        "Bạn có thể kiểm tra dữ liệu trong:\n" +
                        "• Menu QR\n" +
                        "• Danh sách Menu",
                        "Thành Công",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    UpdateStatistics();
                }
            }
            catch (Exception ex)
            {
                Wait(false);
                MessageBox.Show($"❌ Lỗi: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDeleteData_Click(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show(
                    "⚠️ Bạn có chắc muốn xóa tất cả dữ liệu test?\n\n" +
                    "Chỉ xóa dữ liệu có mã:\n" +
                    "HS***, CT***, CA***, MU***, DB***",
                    "Xác Nhận Xóa",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    Wait(true);
                    _testDataGenerator.DeleteAllTestData();
                    Wait(false);

                    MessageBox.Show(
                        "✅ Xóa dữ liệu test thành công!",
                        "Thành Công",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    UpdateStatistics();
                }
            }
            catch (Exception ex)
            {
                Wait(false);
                MessageBox.Show($"❌ Lỗi: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            UpdateStatistics();
        }

        private void UpdateStatistics()
        {
            try
            {
                var stats = _testDataGenerator.GetTestDataStatistics();
                
                var message = "📊 Thống Kê Dữ Liệu:\n\n";
                message += $"Tổng Danh Mục: {stats["TotalCategories"]}\n";
                message += $"Tổng Món Ăn: {stats["TotalItems"]}\n\n";
                message += "Chi Tiết Từng Danh Mục:\n";

                foreach (var key in stats.Keys)
                {
                    if (key.StartsWith("Category_"))
                    {
                        var categoryName = key.Replace("Category_", "");
                        message += $"• {categoryName}: {stats[key]} món\n";
                    }
                }

                lblStatistics.Text = message;
            }
            catch (Exception ex)
            {
                lblStatistics.Text = $"❌ Lỗi: {ex.Message}";
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        protected override void CleanupResources()
        {
            try
            {
                _testDataGenerator = null;
            }
            catch { }
            finally
            {
                base.CleanupResources();
            }
        }
    }
}











