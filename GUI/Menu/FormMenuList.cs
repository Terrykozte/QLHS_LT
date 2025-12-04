using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using QLTN_LT.BLL;
using QLTN_LT.DTO;
using QLTN_LT.GUI.Base;
using QLTN_LT.GUI.Helper;

namespace QLTN_LT.GUI.Menu
{
    public partial class FormMenuList : BaseForm
    {
        private readonly MenuBLL _bll = new MenuBLL();
        private List<MenuItemDTO> _allItems = new List<MenuItemDTO>();
        private Timer _searchDebounceTimer;

        public FormMenuList()
        {
            InitializeComponent();

            // UX & Styling
            this.KeyPreview = true;
            this.KeyDown += FormMenuList_KeyDown;
            try
            {
                UIHelper.ApplyFormStyle(this);
                if (dgvMenu != null) UIHelper.ApplyGridStyle(dgvMenu);
                if (btnAdd != null) UIHelper.ApplyGunaButtonStyle(btnAdd, isPrimary: true);
                if (btnReload != null) UIHelper.ApplyGunaButtonStyle(btnReload, isPrimary: false);
            }
            catch { }

            // Debounce search
            _searchDebounceTimer = new Timer { Interval = 350 };
            _searchDebounceTimer.Tick += (s, e) => { _searchDebounceTimer.Stop(); ApplyFilter(); };

            // Events
            this.Load += FormMenuList_Load;
            if (txtSearch != null)
            {
                txtSearch.TextChanged += (s, e) => { _searchDebounceTimer.Stop(); _searchDebounceTimer.Start(); };
                txtSearch.KeyDown += txtSearch_KeyDown;
            }
            if (cboCategory != null) cboCategory.SelectedIndexChanged += cboCategory_SelectedIndexChanged;
            if (btnAdd != null) btnAdd.Click += btnAdd_Click;
            if (btnReload != null) btnReload.Click += btnReload_Click;
            if (dgvMenu != null) dgvMenu.CellDoubleClick += dgvMenu_CellDoubleClick;
        }

        private void FormMenuList_Load(object sender, EventArgs e)
        {
            try
            {
                ConfigureGrid();
                LoadData();
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex, "FormMenuList_Load");
            }
        }

        private void ConfigureGrid()
        {
            try
            {
                if (dgvMenu == null) return;
                dgvMenu.AutoGenerateColumns = false;
                dgvMenu.Columns.Clear();
                dgvMenu.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "ItemID", HeaderText = "ID", Width = 60 });
                dgvMenu.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "ItemName", HeaderText = "TÊN MÓN", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
                dgvMenu.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "CategoryName", HeaderText = "DANH MỤC", Width = 150 });
                dgvMenu.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "UnitPrice", HeaderText = "GIÁ", Width = 120, DefaultCellStyle = new DataGridViewCellStyle { Format = "N0", Alignment = DataGridViewContentAlignment.MiddleRight } });
                dgvMenu.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Status", HeaderText = "TRẠNG THÁI", Width = 120 });
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex, "ConfigureGrid");
            }
        }

        private void LoadData()
        {
            try
            {
                Wait(true);
                _allItems = _bll.GetAllItems() ?? new List<MenuItemDTO>();
                PopulateCategoryFilter();
                ApplyFilter();
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex, "LoadData");
            }
            finally { Wait(false); }
        }

        private void PopulateCategoryFilter()
        {
            try
            {
                if (cboCategory == null) return;
                var categories = _allItems.Select(item => item.CategoryName).Distinct().Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
                categories.Insert(0, "Tất cả");
                cboCategory.DataSource = categories;
                cboCategory.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex, "PopulateCategoryFilter");
            }
        }

        private void ApplyFilter()
        {
            try
            {
                string selectedCategory = cboCategory?.SelectedItem?.ToString() ?? "Tất cả";
                string keyword = txtSearch?.Text?.Trim().ToLower() ?? string.Empty;

                var filtered = _allItems.AsEnumerable();

                if (selectedCategory != "Tất cả")
                    filtered = filtered.Where(item => string.Equals(item.CategoryName, selectedCategory, StringComparison.OrdinalIgnoreCase));

                if (!string.IsNullOrEmpty(keyword))
                    filtered = filtered.Where(item => (item.ItemName?.ToLower().Contains(keyword) ?? false) || (item.Description?.ToLower().Contains(keyword) ?? false));

                var list = filtered.ToList();
                if (dgvMenu != null) dgvMenu.DataSource = list;
                if (lblPageInfo != null) lblPageInfo.Text = $"Tổng: {list.Count} món";
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex, "ApplyFilter");
            }
        }

        private void cboCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplyFilter();
        }

        private void txtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                ApplyFilter();
                e.SuppressKeyPress = true;
            }
            else if (e.KeyCode == Keys.Escape)
            {
                txtSearch.Clear();
                e.SuppressKeyPress = true;
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                using (var form = new FormMenuEdit(0)) // 0 => new item
                {
                    if (form.ShowDialog(this) == DialogResult.OK)
                    {
                        LoadData();
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex, "btnAdd_Click");
            }
        }

        private void btnReload_Click(object sender, EventArgs e)
        {
            try
            {
                txtSearch?.Clear();
                LoadData();
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex, "btnReload_Click");
            }
        }

        private void dgvMenu_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex >= 0 && dgvMenu?.CurrentRow != null)
                {
                    var selectedItem = dgvMenu.CurrentRow.DataBoundItem as MenuItemDTO;
                    if (selectedItem == null) return;
                    using (var form = new FormMenuEdit(selectedItem.ItemID))
                    {
                        if (form.ShowDialog(this) == DialogResult.OK)
                        {
                            LoadData();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex, "dgvMenu_CellDoubleClick");
            }
        }

        private void FormMenuList_KeyDown(object sender, KeyEventArgs e)
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
                else if (e.KeyCode == Keys.Enter && dgvMenu?.CurrentRow != null)
                {
                    dgvMenu_CellDoubleClick(sender, new DataGridViewCellEventArgs(0, dgvMenu.CurrentRow.Index));
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex, "FormMenuList_KeyDown");
            }
        }

        protected override void CleanupResources()
        {
            try
            {
                _searchDebounceTimer?.Stop();
                _searchDebounceTimer?.Dispose();
                _allItems?.Clear();
            }
            catch { }
            finally { base.CleanupResources(); }
        }
    }
}
