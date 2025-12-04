using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Guna.UI2.WinForms;
using QLTN_LT.BLL;
using QLTN_LT.DTO;
using QLTN_LT.GUI.Base;
using QLTN_LT.GUI.Helper;

namespace QLTN_LT.GUI.Seafood
{
    public partial class FormSeafoodManagement : BaseForm
    {
        private SeafoodBLL _seafoodBLL;
        private CategoryBLL _categoryBLL;
        private List<SeafoodDTO> _allSeafood;
        private List<CategoryDTO> _categories;
        private Timer _searchDebounce;

        public FormSeafoodManagement()
        {
            InitializeComponent();
            _seafoodBLL = new SeafoodBLL();
            _categoryBLL = new CategoryBLL();

            // Style
            try
            {
                UIHelper.ApplyFormStyle(this);
                if (dgvSeafood != null) UIHelper.ApplyGridStyle(dgvSeafood);
                if (btnAdd != null) UIHelper.ApplyGunaButtonStyle(btnAdd, true);
                if (btnEdit != null) UIHelper.ApplyGunaButtonStyle(btnEdit, false);
                if (btnDelete != null) UIHelper.ApplyGunaButtonStyle(btnDelete, false);
                if (btnRefresh != null) UIHelper.ApplyGunaButtonStyle(btnRefresh, false);
            }
            catch { }
        }

        private void FormSeafoodManagement_Load(object sender, EventArgs e)
        {
            this.KeyPreview = true;
            this.KeyDown += FormSeafoodManagement_KeyDown;

            _searchDebounce = new Timer { Interval = 350 };
            _searchDebounce.Tick += (s, e2) => { _searchDebounce.Stop(); RefreshDataGrid(); };

            LoadCategories();
            LoadSeafood();
            SetupEventHandlers();
        }

        private void ApplyStyles()
        {
            this.BackColor = Color.FromArgb(245, 245, 245);
            pnlTop.BackColor = Color.White;
            pnlList.BackColor = Color.White;
            pnlDetail.BackColor = Color.White;

            btnAdd.FillColor = Color.FromArgb(34, 197, 94);
            btnEdit.FillColor = Color.FromArgb(59, 130, 246);
            btnDelete.FillColor = Color.FromArgb(239, 68, 68);
            btnRefresh.FillColor = Color.FromArgb(107, 114, 128);
        }

        private void LoadCategories()
        {
            try
            {
                _categories = _categoryBLL.GetAll().ToList();
                cmbCategory.Items.Clear();
                cmbCategory.Items.Add("Tất cả");
                foreach (var cat in _categories)
                {
                    cmbCategory.Items.Add(cat.CategoryName);
                }
                cmbCategory.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải danh mục: " + ex.Message);
            }
        }

        private void LoadSeafood()
        {
            try
            {
                Wait(true);
                _allSeafood = _seafoodBLL.GetAll()?.ToList() ?? new List<SeafoodDTO>();
                RefreshDataGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải sản phẩm: " + ex.Message);
            }
            finally { Wait(false); }
        }

        private void RefreshDataGrid()
        {
            try
            {
                var filtered = _allSeafood.AsEnumerable();

                // Filter by category
                if (cmbCategory != null && cmbCategory.SelectedIndex > 0)
                {
                    string category = cmbCategory.SelectedItem.ToString();
                    filtered = filtered.Where(s => string.Equals(s.CategoryName, category, StringComparison.OrdinalIgnoreCase));
                }

                // Filter by search
                if (!string.IsNullOrWhiteSpace(txtSearch?.Text))
                {
                    string search = txtSearch.Text.ToLower();
                    filtered = filtered.Where(s =>
                        (s.SeafoodName?.ToLower().Contains(search) ?? false) ||
                        (s.Description?.ToLower().Contains(search) ?? false) ||
                        (s.CategoryName?.ToLower().Contains(search) ?? false)
                    );
                }

                dgvSeafood.DataSource = null;
                dgvSeafood.DataSource = filtered.ToList();

                // Format columns
                if (dgvSeafood.Columns.Count > 0)
                {
                    if (dgvSeafood.Columns["SeafoodID"] != null)
                        dgvSeafood.Columns["SeafoodID"].HeaderText = "ID";
                    if (dgvSeafood.Columns["SeafoodName"] != null)
                        dgvSeafood.Columns["SeafoodName"].HeaderText = "Tên sản phẩm";
                    if (dgvSeafood.Columns["CategoryName"] != null)
                        dgvSeafood.Columns["CategoryName"].HeaderText = "Danh mục";
                    if (dgvSeafood.Columns["UnitPrice"] != null)
                    {
                        dgvSeafood.Columns["UnitPrice"].HeaderText = "Giá (VNĐ)";
                        dgvSeafood.Columns["UnitPrice"].DefaultCellStyle.Format = "N0";
                        dgvSeafood.Columns["UnitPrice"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    }
                    if (dgvSeafood.Columns["Status"] != null)
                        dgvSeafood.Columns["Status"].HeaderText = "Trạng thái";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi cập nhật danh sách: " + ex.Message);
            }
        }

        private void SetupEventHandlers()
        {
            if (cmbCategory != null) cmbCategory.SelectedIndexChanged += (s, e) => RefreshDataGrid();
            if (txtSearch != null) txtSearch.TextChanged += (s, e) => { _searchDebounce.Stop(); _searchDebounce.Start(); };
            if (dgvSeafood != null) dgvSeafood.SelectionChanged += (s, e) => DisplaySelectedSeafood();
            if (btnAdd != null) btnAdd.Click += (s, e) => AddSeafood();
            if (btnEdit != null) btnEdit.Click += (s, e) => EditSeafood();
            if (btnDelete != null) btnDelete.Click += (s, e) => DeleteSeafood();
            if (btnRefresh != null) btnRefresh.Click += (s, e) => LoadSeafood();
        }

        private void DisplaySelectedSeafood()
        {
            try
            {
                if (dgvSeafood == null || dgvSeafood.SelectedRows.Count == 0)
                {
                    ClearDetail();
                    return;
                }

                var item = dgvSeafood.SelectedRows[0].DataBoundItem as SeafoodDTO;
                if (item != null)
                {
                    lblName.Text = item.SeafoodName;
                    lblCategory.Text = item.CategoryName;
                    lblPrice.Text = $"{item.UnitPrice:N0} VNĐ";
                    lblDescription.Text = item.Description;
                    lblStatus.Text = item.Status;
                    lblStatus.ForeColor = (item.Status ?? "").Contains("còn", StringComparison.OrdinalIgnoreCase) ? Color.Green : Color.Red;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi hiển thị chi tiết: " + ex.Message);
            }
        }

        private void ClearDetail()
        {
            lblName.Text = "Chưa chọn";
            lblCategory.Text = "";
            lblPrice.Text = "";
            lblDescription.Text = "";
            lblStatus.Text = "";
        }

        private void AddSeafood()
        {
            try
            {
                using (var form = new FormSeafoodAdd())
                {
                    if (form.ShowDialog(this) == DialogResult.OK)
                    {
                        LoadSeafood();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
        }

        private void EditSeafood()
        {
            try
            {
                if (dgvSeafood?.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Vui lòng chọn sản phẩm để sửa");
                    return;
                }

                var item = dgvSeafood.SelectedRows[0].DataBoundItem as SeafoodDTO;
                if (item == null) return;

                using (var form = new FormSeafoodEdit(item))
                {
                    if (form.ShowDialog(this) == DialogResult.OK)
                    {
                        LoadSeafood();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
        }

        private void DeleteSeafood()
        {
            try
            {
                if (dgvSeafood?.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Vui lòng chọn sản phẩm để xóa");
                    return;
                }

                var item = dgvSeafood.SelectedRows[0].DataBoundItem as SeafoodDTO;
                if (item == null) return;

                if (!ShowConfirm("Bạn có chắc muốn xóa sản phẩm này?", "Xác nhận"))
                    return;

                _seafoodBLL.Delete(item.SeafoodID);
                ShowInfo("Xóa sản phẩm thành công");
                LoadSeafood();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
        }

        private void FormSeafoodManagement_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.F5)
                {
                    LoadSeafood();
                    e.Handled = true;
                }
                else if (e.Control && e.KeyCode == Keys.N)
                {
                    AddSeafood();
                    e.Handled = true;
                }
                else if (e.KeyCode == Keys.Enter)
                {
                    EditSeafood();
                    e.Handled = true;
                }
                else if (e.KeyCode == Keys.Delete)
                {
                    DeleteSeafood();
                    e.Handled = true;
                }
            }
            catch { }
        }

        protected override void CleanupResources()
        {
            try
            {
                _searchDebounce?.Stop();
                _searchDebounce?.Dispose();
                _allSeafood?.Clear();
                _categories?.Clear();
                _seafoodBLL = null;
                _categoryBLL = null;
            }
            catch { }
            finally { base.CleanupResources(); }
        }
    }
}
