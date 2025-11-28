using System;
using System.Collections.Generic;
using System.Windows.Forms;
using QLTN_LT.BLL;
using QLTN_LT.DTO;

namespace QLTN_LT.GUI.Table
{
    public partial class FormTableList : Form
    {
        private readonly TableBLL _bll = new TableBLL();

        public FormTableList()
        {
            InitializeComponent();
        }

        private void FormTableList_Load(object sender, EventArgs e)
        {
            ConfigureGrid();
            LoadData();
        }

        private void ConfigureGrid()
        {
            dgvTable.AutoGenerateColumns = false;
            dgvTable.Columns.Clear();

            dgvTable.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "TableID", HeaderText = "ID", Width = 60 });
            dgvTable.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "TableName", HeaderText = "TÊN BÀN", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
            dgvTable.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Status", HeaderText = "TRẠNG THÁI", Width = 150 });
            
            dgvTable.CellDoubleClick += DgvTable_CellDoubleClick;
        }

        private void DgvTable_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            
            if (!(dgvTable.Rows[e.RowIndex].DataBoundItem is TableDTO table)) return;

            using (var form = new FormTableEdit(table.TableID))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    LoadData();
                }
            }
        }

        private void LoadData()
        {
            try
            {
                var keyword = txtSearch.Text.Trim();
                var data = string.IsNullOrEmpty(keyword) ? _bll.GetAll() : _bll.Search(keyword);

                dgvTable.DataSource = data;
                lblPageInfo.Text = $"Showing {data.Count} entries";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải dữ liệu: {ex.Message}");
            }
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            using (var form = new FormTableAdd())
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    LoadData();
                }
            }
        }

        private void BtnGenerateQr_Click(object sender, EventArgs e)
        {
            if (dgvTable.CurrentRow == null)
            {
                MessageBox.Show("Vui lòng chọn một bàn để tạo mã QR.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var selected = (TableDTO)dgvTable.CurrentRow.DataBoundItem;
            using (var f = new FormGenerateQR(selected.TableName))
            {
                f.ShowDialog(this);
            }
        }

        private void TxtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                LoadData();
            }
        }
    }
}
