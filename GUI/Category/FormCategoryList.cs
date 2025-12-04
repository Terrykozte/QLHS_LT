using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using QLTN_LT.DTO;
using QLTN_LT.BLL;
using System.Diagnostics;
using QLTN_LT.GUI.Base;
using QLTN_LT.GUI.Helper;

namespace QLTN_LT.GUI.Category
{
    public partial class FormCategoryList : BaseForm
    {
        private CategoryBLL _categoryBLL;
        private Timer _searchDebounceTimer;
        private List<CategoryDTO> _allData = new List<CategoryDTO>();

        public FormCategoryList()
        {
            InitializeComponent();
            _categoryBLL = new CategoryBLL();

            // UX
            this.KeyPreview = true;
            this.KeyDown += FormCategoryList_KeyDown;

            // Debounce search
            _searchDebounceTimer = new Timer { Interval = 400 };
            _searchDebounceTimer.Tick += (s, e) =>
            {
                _searchDebounceTimer.Stop();
                ApplyFilters();
            };

            // Styling
            try
            {
                UIHelper.ApplyFormStyle(this);
                if (dgvCategory != null) UIHelper.ApplyGridStyle(dgvCategory);
                if (btnAdd != null) UIHelper.ApplyGunaButtonStyle(btnAdd, isPrimary: true);
            }
            catch { }

            // Events
            this.Load += FormCategoryList_Load;
            if (txtSearch != null) txtSearch.TextChanged += txtSearch_TextChanged;
            if (dgvCategory != null) dgvCategory.CellDoubleClick += dgvCategory_CellDoubleClick;
        }

        private void FormCategoryList_Load(object sender, EventArgs e)
        {
            try
            {
                LoadData();
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex, "FormCategoryList_Load");
            }
        }

        private void LoadData()
        {
            try
            {
                Wait(true);
                _allData = _categoryBLL.GetAll() ?? new List<CategoryDTO>();
                ApplyFilters();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading category data: {ex.Message}");
                MessageBox.Show($"Lỗi tải dữ liệu: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Wait(false);
            }
        }

        private void ApplyFilters()
        {
            try
            {
                var list = _allData;

                // Apply search filter
                string keyword = txtSearch?.Text?.Trim().ToLower() ?? string.Empty;
                if (!string.IsNullOrEmpty(keyword))
                {
                    list = list.Where(c =>
                        (c.CategoryName?.ToLower().Contains(keyword) ?? false) ||
                        (c.Description?.ToLower().Contains(keyword) ?? false)
                    ).ToList();
                }

                if (dgvCategory != null)
                {
                    dgvCategory.DataSource = list;
                    ConfigureColumns();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error applying filters: {ex.Message}");
            }
        }

        private void ConfigureColumns()
        {
            try
            {
                if (dgvCategory == null) return;

                if (dgvCategory.AutoGenerateColumns)
                {
                    dgvCategory.AutoGenerateColumns = false;
                    dgvCategory.Columns.Clear();

                    dgvCategory.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "CategoryID", HeaderText = "ID", Width = 60 });
                    dgvCategory.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "CategoryName", HeaderText = "Tên danh mục", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
                    dgvCategory.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Description", HeaderText = "Mô tả", Width = 240 });
                    dgvCategory.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Status", HeaderText = "Trạng thái", Width = 120 });
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error configuring columns: {ex.Message}");
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                using (var form = new FormCategoryAdd())
                {
                    if (form.ShowDialog(this) == DialogResult.OK)
                    {
                        LoadData();
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error opening add form: {ex.Message}");
                MessageBox.Show($"Lỗi mở form thêm: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgvCategory_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex >= 0 && dgvCategory?.Rows.Count > e.RowIndex)
                {
                    var item = dgvCategory.Rows[e.RowIndex].DataBoundItem as CategoryDTO;
                    if (item != null)
                    {
                        using (var form = new FormCategoryEdit(item))
                        {
                            if (form.ShowDialog(this) == DialogResult.OK)
                            {
                                LoadData();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error opening edit form: {ex.Message}");
                MessageBox.Show($"Lỗi mở form sửa: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            _searchDebounceTimer.Stop();
            _searchDebounceTimer.Start();
        }

        private void FormCategoryList_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.F5)
                {
                    LoadData();
                    e.Handled = true;
                }
                else if (e.Control && e.KeyCode == Keys.N)
                {
                    btnAdd_Click(sender, EventArgs.Empty);
                    e.Handled = true;
                }
                else if (e.KeyCode == Keys.Enter && dgvCategory?.CurrentRow != null)
                {
                    var item = dgvCategory.CurrentRow.DataBoundItem as CategoryDTO;
                    if (item != null)
                    {
                        using (var form = new FormCategoryEdit(item))
                        {
                            if (form.ShowDialog(this) == DialogResult.OK)
                                LoadData();
                        }
                        e.Handled = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"KeyDown error: {ex.Message}");
            }
        }

        protected override void CleanupResources()
        {
            try
            {
                _searchDebounceTimer?.Stop();
                _searchDebounceTimer?.Dispose();
                _allData?.Clear();
                _categoryBLL = null;
            }
            catch { }
            finally
            {
                base.CleanupResources();
            }
        }
    }
}
