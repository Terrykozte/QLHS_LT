using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using QLTN_LT.BLL;
using QLTN_LT.DTO;
using QLTN_LT.GUI.Base;
using QLTN_LT.GUI.Helper;

namespace QLTN_LT.GUI.Table
{
    public partial class FormTableList : BaseForm
    {
        private TableBLL _bll = new TableBLL();
        private List<TableDTO> _allData = new List<TableDTO>();
        private Timer _searchDebounceTimer;

        public FormTableList()
        {
            InitializeComponent();

            // UX & Styling
            this.KeyPreview = true;
            this.KeyDown += FormTableList_KeyDown;
            try
            {
                UIHelper.ApplyFormStyle(this);
                if (dgvTable != null) UIHelper.ApplyGridStyle(dgvTable);
                if (btnAdd != null) UIHelper.ApplyGunaButtonStyle(btnAdd, isPrimary: true);
                if (btnGenerateQR != null) UIHelper.ApplyGunaButtonStyle(btnGenerateQR, isPrimary: false);
            }
            catch { }

            // Debounce search
            _searchDebounceTimer = new Timer { Interval = 350 };
            _searchDebounceTimer.Tick += (s, e) => { _searchDebounceTimer.Stop(); LoadData(); };

            // Events
            this.Load += FormTableList_Load;
            if (txtSearch != null)
            {
                txtSearch.TextChanged += (s, e) => { _searchDebounceTimer.Stop(); _searchDebounceTimer.Start(); };
                txtSearch.KeyDown += TxtSearch_KeyDown;
            }
            if (btnAdd != null) btnAdd.Click += BtnAdd_Click;
            if (btnGenerateQR != null) btnGenerateQR.Click += btnGenerateQr_Click;
            if (dgvTable != null) dgvTable.CellDoubleClick += DgvTable_CellDoubleClick;
        }

        private void FormTableList_Load(object sender, EventArgs e)
        {
            try
            {
                ConfigureGrid();
                LoadData();
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex, "FormTableList_Load");
            }
        }

        private void ConfigureGrid()
        {
            try
            {
                if (dgvTable == null) return;

                dgvTable.AutoGenerateColumns = false;
                dgvTable.Columns.Clear();

                dgvTable.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "TableID", HeaderText = "ID", Width = 60 });
                dgvTable.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "TableName", HeaderText = "TÊN BÀN", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
                dgvTable.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Status", HeaderText = "TRẠNG THÁI", Width = 150 });
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex, "ConfigureGrid");
            }
        }

        private void DgvTable_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex < 0 || dgvTable?.Rows.Count <= e.RowIndex) return;
                if (!(dgvTable.Rows[e.RowIndex].DataBoundItem is TableDTO table)) return;

                using (var form = new FormTableEdit(table.TableID))
                {
                    if (form.ShowDialog(this) == DialogResult.OK)
                    {
                        LoadData();
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex, "DgvTable_CellDoubleClick");
            }
        }

        private void LoadData()
        {
            try
            {
                Wait(true);
                var keyword = txtSearch?.Text?.Trim().ToLower() ?? string.Empty;
                _allData = _bll.GetAll() ?? new List<TableDTO>();

                var data = string.IsNullOrEmpty(keyword)
                    ? _allData
                    : _allData.Where(t => (t.TableName?.ToLower().Contains(keyword) ?? false) || (t.Status?.ToLower().Contains(keyword) ?? false)).ToList();

                if (dgvTable != null) dgvTable.DataSource = data;
                if (lblPageInfo != null) lblPageInfo.Text = $"Tổng: {data.Count} bàn";
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex, "LoadData");
            }
            finally { Wait(false); }
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                using (var form = new FormTableAdd())
                {
                    if (form.ShowDialog(this) == DialogResult.OK)
                    {
                        LoadData();
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex, "BtnAdd_Click");
            }
        }

        private void btnGenerateQr_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvTable?.CurrentRow == null)
                {
                    MessageBox.Show("Vui lòng chọn một bàn để tạo mã QR.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var selected = dgvTable.CurrentRow.DataBoundItem as TableDTO;
                if (selected == null) return;
                using (var f = new FormGenerateQR(selected.TableName))
                {
                    f.ShowDialog(this);
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex, "btnGenerateQr_Click");
            }
        }

        // Designer-bound alias (case-sensitive in designer)
        private void BtnGenerateQr_Click(object sender, EventArgs e)
        {
            btnGenerateQr_Click(sender, e);
        }

        private void TxtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                LoadData();
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Escape)
            {
                txtSearch.Clear();
                e.Handled = true;
            }
        }

        private void FormTableList_KeyDown(object sender, KeyEventArgs e)
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
                    BtnAdd_Click(sender, EventArgs.Empty);
                    e.Handled = true;
                }
                else if (e.KeyCode == Keys.Enter && dgvTable?.CurrentRow != null)
                {
                    DgvTable_CellDoubleClick(sender, new DataGridViewCellEventArgs(0, dgvTable.CurrentRow.Index));
                    e.Handled = true;
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
                _bll = null;
            }
            catch { }
            finally { base.CleanupResources(); }
        }
    }
}
