using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using QLTN_LT.BLL;
using QLTN_LT.DTO;

namespace QLTN_LT.GUI.Menu
{
    public partial class FormMenuList : Form
    {
        private readonly MenuBLL _bll = new MenuBLL();
        private List<MenuItemDTO> _allItems = new List<MenuItemDTO>();

        public FormMenuList()
        {
            InitializeComponent();
        }

        private void FormMenuList_Load(object sender, EventArgs e)
        {
            ConfigureGrid();
            LoadData();
            dgvMenu.CellDoubleClick += dgvMenu_CellDoubleClick;
        }

        private void ConfigureGrid()
        {
            dgvMenu.AutoGenerateColumns = false;
            dgvMenu.Columns.Clear();

            dgvMenu.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "ItemID", HeaderText = "ID", Width = 60 });
            dgvMenu.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "ItemName", HeaderText = "TÊN MÓN", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
            dgvMenu.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "CategoryName", HeaderText = "DANH MỤC", Width = 150 });
            dgvMenu.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Price", HeaderText = "GIÁ", Width = 120, DefaultCellStyle = new DataGridViewCellStyle { Format = "N0", Alignment = DataGridViewContentAlignment.MiddleRight } });
            dgvMenu.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Status", HeaderText = "TRẠNG THÁI", Width = 120 });
        }

        private void LoadData()
        {
            _allItems = _bll.GetAllItems();
            PopulateCategoryFilter();
            ApplyFilter();
        }

        private void PopulateCategoryFilter()
        {
            var categories = _allItems.Select(item => item.CategoryName).Distinct().ToList();
            categories.Insert(0, "Tất cả");
            cboCategory.DataSource = categories;
        }

        private void ApplyFilter()
        {
            string selectedCategory = cboCategory.SelectedItem?.ToString() ?? "Tất cả";
            string keyword = txtSearch.Text.ToLower();

            var filteredList = _allItems;

            if (selectedCategory != "Tất cả")
            {
                filteredList = filteredList.Where(item => item.CategoryName == selectedCategory).ToList();
            }

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                filteredList = filteredList.Where(item => item.ItemName.ToLower().Contains(keyword)).ToList();
            }

            dgvMenu.DataSource = filteredList;
            lblPageInfo.Text = $"Showing {filteredList.Count} entries";
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
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            using (var form = new FormMenuEdit(0)) // Pass 0 for new item
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    LoadData();
                }
            }
        }

        private void btnReload_Click(object sender, EventArgs e)
        {
            txtSearch.Clear();
            LoadData();
        }

        private void dgvMenu_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                if (dgvMenu.CurrentRow == null) return;

                var selectedItem = (MenuItemDTO)dgvMenu.CurrentRow.DataBoundItem;
                using (var form = new FormMenuEdit(selectedItem.ItemID))
                {
                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        LoadData();
                    }
                }
            }
        }
    }
}
