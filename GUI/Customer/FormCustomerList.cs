using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using System.Text;
using System.Drawing;
using System.Diagnostics;
using QLTN_LT.BLL;
using QLTN_LT.DTO;
using QLTN_LT.GUI.Base;
using QLTN_LT.GUI.Helper;
using QLTN_LT.GUI.Order;

namespace QLTN_LT.GUI.Customer
{
    /// <summary>
    /// FormCustomerList - Danh sách khách hàng với ListEnhancer
    /// Tích hợp: debounce search, sort header, multi-select, batch delete, export, context menu, phím tắt
    /// Cải thiện: UI/UX, layout chuẩn, xử lý smooth, hạn chế lỗi double-close, clean code OOP
    /// </summary>
    public partial class FormCustomerList : BaseForm
    {
        #region Fields

        private readonly CustomerBLL _bll = new CustomerBLL();
        private ListEnhancer _listEnhancer;
        private int _currentPage = 1;
        private int _pageSize = 10;
        private int _totalRecords = 0;
        private List<CustomerDTO> _allData = new List<CustomerDTO>();
        private Label _lblEmptyState;
        private Label _lblLoadingIndicator;
        private Button _btnQuickOrder;
        private bool _isLoading = false;
        private bool _isDisposed = false;

        #endregion

        #region Constructor & Initialization

        public FormCustomerList()
        {
            InitializeComponent();
            InitializeFormSettings();
            InitializeListEnhancer();
            InitializeEventHandlers();
        }

        private void InitializeFormSettings()
        {
            this.KeyPreview = true;
            this.DoubleBuffered = true;
            try
            {
                UIHelper.ApplyFormStyle(this);
            }
            catch { }
        }

        private void InitializeListEnhancer()
        {
            try
            {
                _listEnhancer = new ListEnhancer(dgvCustomer, txtSearch, lblPageInfo, _pageSize);

                // Setup callbacks
                _listEnhancer.SetLoadDataCallback(page => { _currentPage = page; LoadData(); });
                _listEnhancer.SetRefreshCallback(() => LoadData());
                _listEnhancer.SetDeleteCallback(id => DeleteCustomer(id));
                _listEnhancer.SetBatchDeleteCallback(ids => BatchDeleteCustomers(ids));

                // Setup event handlers
                _listEnhancer.OnColumnSort = (columnName, ascending) => ApplySorting(columnName, ascending);
                _listEnhancer.OnEditClick = () => EditCurrentCustomer();
                _listEnhancer.OnDeleteRequested = (id) => ConfirmAndDeleteCustomer(id);
                _listEnhancer.OnBatchDeleteRequested = (ids) => ConfirmAndBatchDeleteCustomers(ids);
                _listEnhancer.OnCopySuccess = () => ShowInfo("✅ Đã sao chép thông tin khách hàng!");
                _listEnhancer.OnExportCurrentPage = () => ExportCurrentPage();
                _listEnhancer.OnExportSelected = () => ExportSelected();
                _listEnhancer.OnExportAll = () => ExportAll();
                _listEnhancer.OnAddNew = () => AddNewCustomer();
                _listEnhancer.OnImport = () => ImportFromFile();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error initializing ListEnhancer: {ex.Message}");
            }
        }

        private void InitializeEventHandlers()
        {
            this.Load += FormCustomerList_Load;
            this.KeyDown += FormCustomerList_KeyDown;
            this.FormClosing += FormCustomerList_FormClosing;

            if (dgvCustomer != null)
            {
                dgvCustomer.CellContentClick += DgvCustomer_CellContentClick;
                dgvCustomer.CellDoubleClick += DgvCustomer_CellDoubleClick;
                dgvCustomer.CellMouseEnter += DgvCustomer_CellMouseEnter;
                dgvCustomer.CellMouseLeave += DgvCustomer_CellMouseLeave;
                dgvCustomer.CellPainting += DgvCustomer_CellPainting;
            }

            if (btnAdd != null) btnAdd.Click += (s, e) => AddNewCustomer();
            if (btnImport != null) btnImport.Click += (s, e) => ImportFromFile();
            if (btnExport != null) btnExport.Click += (s, e) => ExportAll();
            if (btnPrevious != null) btnPrevious.Click += (s, e) => PreviousPage();
            if (btnNext != null) btnNext.Click += (s, e) => NextPage();
        }

        #endregion

        #region Form Load & Cleanup

        private void FormCustomerList_Load(object sender, EventArgs e)
        {
            if (_isDisposed) return;
            try
            {
                ConfigureGrid();
                BuildEmptyState();
                BuildLoadingIndicator();
                BuildQuickOrderButton();
                LoadData();
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex, "FormCustomerList_Load");
            }
        }

        private void FormCustomerList_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Prevent double-close issues
            if (_isDisposed)
            {
                e.Cancel = true;
                return;
            }
        }

        protected override void CleanupResources()
        {
            if (_isDisposed) return;
            _isDisposed = true;

            try
            {
                // Xóa event handlers
                if (dgvCustomer != null)
                {
                    dgvCustomer.CellContentClick -= DgvCustomer_CellContentClick;
                    dgvCustomer.CellDoubleClick -= DgvCustomer_CellDoubleClick;
                    dgvCustomer.CellMouseEnter -= DgvCustomer_CellMouseEnter;
                    dgvCustomer.CellMouseLeave -= DgvCustomer_CellMouseLeave;
                    dgvCustomer.CellPainting -= DgvCustomer_CellPainting;
                }

                // Xóa ListEnhancer
                _listEnhancer?.Dispose();
                _listEnhancer = null;

                // Xóa dữ liệu
                _allData?.Clear();
                _allData = null;

                // Xóa controls
                _lblEmptyState?.Dispose();
                _lblEmptyState = null;

                _lblLoadingIndicator?.Dispose();
                _lblLoadingIndicator = null;

                _btnQuickOrder?.Dispose();
                _btnQuickOrder = null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in CleanupResources: {ex.Message}");
            }
            finally
            {
                base.CleanupResources();
            }
        }

        #endregion

        #region Grid Configuration

        private void ConfigureGrid()
        {
            try
            {
                if (dgvCustomer == null) return;

                dgvCustomer.AutoGenerateColumns = false;
                dgvCustomer.Columns.Clear();

                // Grid styling - Modern & Professional
                dgvCustomer.EnableHeadersVisualStyles = false;
                dgvCustomer.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = Color.FromArgb(41, 128, 185),
                    ForeColor = Color.White,
                    Font = new Font("Segoe UI", 10, FontStyle.Bold),
                    Alignment = DataGridViewContentAlignment.MiddleCenter,
                    Padding = new Padding(5),
                    WrapMode = DataGridViewTriState.False
                };
                dgvCustomer.ColumnHeadersHeight = 45;
                dgvCustomer.RowTemplate.Height = 38;
                dgvCustomer.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 248, 250);
                dgvCustomer.DefaultCellStyle.SelectionBackColor = Color.FromArgb(52, 152, 219);
                dgvCustomer.DefaultCellStyle.SelectionForeColor = Color.White;
                dgvCustomer.DefaultCellStyle.Font = new Font("Segoe UI", 9);
                dgvCustomer.DefaultCellStyle.Padding = new Padding(5);
                dgvCustomer.GridColor = Color.FromArgb(220, 220, 220);
                dgvCustomer.AllowUserToAddRows = false;
                dgvCustomer.AllowUserToDeleteRows = false;
                dgvCustomer.AllowUserToResizeRows = false;
                dgvCustomer.ReadOnly = false;
                dgvCustomer.MultiSelect = false;
                dgvCustomer.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

                // Selection CheckBox Column
                var chkCol = new DataGridViewCheckBoxColumn
                {
                    HeaderText = "✓",
                    Width = 40,
                    ReadOnly = false,
                    Name = "colCheck",
                    ThreeState = false
                };
                chkCol.TrueValue = true;
                chkCol.FalseValue = false;
                chkCol.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dgvCustomer.Columns.Add(chkCol);

                // Row number (STT)
                var sttCol = new DataGridViewTextBoxColumn
                {
                    Name = "colSTT",
                    HeaderText = "STT",
                    Width = 50,
                    ReadOnly = true
                };
                dgvCustomer.Columns.Add(sttCol);

                // ID Column
                var idCol = new DataGridViewTextBoxColumn
                {
                    Name = "CustomerID",
                    DataPropertyName = "CustomerID",
                    HeaderText = "ID",
                    Width = 60,
                    ReadOnly = true
                };
                dgvCustomer.Columns.Add(idCol);

                // Name Column
                var nameCol = new DataGridViewTextBoxColumn
                {
                    Name = "CustomerName",
                    DataPropertyName = "CustomerName",
                    HeaderText = "TÊN KHÁCH HÀNG",
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                    ReadOnly = true
                };
                dgvCustomer.Columns.Add(nameCol);

                // Phone Column
                var phoneCol = new DataGridViewTextBoxColumn
                {
                    Name = "PhoneNumber",
                    DataPropertyName = "PhoneNumber",
                    HeaderText = "SỐ ĐIỆN THOẠI",
                    Width = 150,
                    ReadOnly = true
                };
                dgvCustomer.Columns.Add(phoneCol);

                // Address Column
                var addressCol = new DataGridViewTextBoxColumn
                {
                    Name = "Address",
                    DataPropertyName = "Address",
                    HeaderText = "ĐỊA CHỈ",
                    Width = 250,
                    ReadOnly = true
                };
                dgvCustomer.Columns.Add(addressCol);

                // Action Button Columns
                var editBtn = new DataGridViewButtonColumn
                {
                    HeaderText = "THAO TÁC",
                    Text = "✏️ Sửa",
                    UseColumnTextForButtonValue = true,
                    Name = "colEdit",
                    Width = 100,
                    DefaultCellStyle = new DataGridViewCellStyle
                    {
                        BackColor = Color.FromArgb(52, 152, 219),
                        ForeColor = Color.White,
                        Font = new Font("Segoe UI", 9, FontStyle.Bold),
                        Alignment = DataGridViewContentAlignment.MiddleCenter,
                        Padding = new Padding(3)
                    }
                };
                dgvCustomer.Columns.Add(editBtn);

                var orderBtn = new DataGridViewButtonColumn
                {
                    HeaderText = "",
                    Text = "🛒 Đặt đơn",
                    UseColumnTextForButtonValue = true,
                    Name = "colOrder",
                    Width = 110,
                    DefaultCellStyle = new DataGridViewCellStyle
                    {
                        BackColor = Color.FromArgb(46, 204, 113),
                        ForeColor = Color.White,
                        Font = new Font("Segoe UI", 9, FontStyle.Bold),
                        Alignment = DataGridViewContentAlignment.MiddleCenter,
                        Padding = new Padding(3)
                    }
                };
                dgvCustomer.Columns.Add(orderBtn);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error configuring grid: {ex.Message}");
            }
        }

        #endregion

        #region Grid Events

        private void DgvCustomer_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

                var colName = dgvCustomer.Columns[e.ColumnIndex].Name;
                if (colName == "colEdit")
                {
                    EditCustomerAtRow(e.RowIndex);
                }
                else if (colName == "colOrder")
                {
                    CreateOrderForCustomerAtRow(e.RowIndex);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in cell content click: {ex.Message}");
                MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DgvCustomer_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
                {
                    EditCustomerAtRow(e.RowIndex);
                }
            }
            catch { }
        }

        private void DgvCustomer_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
                {
                    dgvCustomer.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.FromArgb(220, 240, 255);
                }
            }
            catch { }
        }

        private void DgvCustomer_CellMouseLeave(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex >= 0)
                {
                    dgvCustomer.Rows[e.RowIndex].DefaultCellStyle.BackColor =
                        e.RowIndex % 2 == 0 ? Color.White : Color.FromArgb(245, 248, 250);
                }
            }
            catch { }
        }

        private void DgvCustomer_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            try
            {
                if (e.ColumnIndex >= 0 && dgvCustomer.Columns[e.ColumnIndex].Name == "colEdit" && e.RowIndex >= 0)
                {
                    e.PaintBackground(e.ClipBounds, false);

                    var rect = e.CellBounds;
                    var buttonRect = new Rectangle(rect.X + 5, rect.Y + 5, rect.Width - 10, rect.Height - 10);

                    using (var brush = new SolidBrush(Color.FromArgb(52, 152, 219)))
                    {
                        e.Graphics.FillRectangle(brush, buttonRect);
                    }

                    using (var pen = new Pen(Color.FromArgb(41, 128, 185), 1))
                    {
                        e.Graphics.DrawRectangle(pen, buttonRect);
                    }

                    var textSize = e.Graphics.MeasureString(e.Value?.ToString() ?? "", e.CellStyle.Font);
                    var textX = buttonRect.X + (buttonRect.Width - (int)textSize.Width) / 2;
                    var textY = buttonRect.Y + (buttonRect.Height - (int)textSize.Height) / 2;

                    e.Graphics.DrawString(e.Value?.ToString() ?? "", e.CellStyle.Font, Brushes.White, textX, textY);
                    e.Handled = true;
                }
            }
            catch { }
        }

        #endregion

        #region Data Loading & Filtering

        private void LoadData()
        {
            if (_isLoading || _isDisposed) return;
            _isLoading = true;

            try
            {
                Wait(true);
                ShowLoadingState(true);

                _allData = _bll.GetAll() ?? new List<CustomerDTO>();

                // Filter with case-insensitive & accent-insensitive search
                string keyword = _listEnhancer.GetSearchKeyword();
                string kwNorm = ListEnhancer.RemoveDiacritics(keyword).ToLowerInvariant();

                var filteredData = _allData.FindAll(x =>
                {
                    if (string.IsNullOrEmpty(kwNorm)) return true;
                    string name = ListEnhancer.RemoveDiacritics(x.CustomerName ?? string.Empty).ToLowerInvariant();
                    string phone = ListEnhancer.RemoveDiacritics(x.PhoneNumber ?? string.Empty).ToLowerInvariant();
                    string addr = ListEnhancer.RemoveDiacritics(x.Address ?? string.Empty).ToLowerInvariant();
                    return name.Contains(kwNorm) || phone.Contains(kwNorm) || addr.Contains(kwNorm);
                });

                _totalRecords = filteredData.Count;
                UpdatePaginationButtons();

                // Paging (client-side)
                var pagedData = filteredData
                    .Skip((_currentPage - 1) * _pageSize)
                    .Take(_pageSize)
                    .ToList();

                if (dgvCustomer != null)
                {
                    dgvCustomer.DataSource = pagedData;
                }

                // Update page info
                UpdatePageInfo(keyword);
                UpdateEmptyState(pagedData.Count == 0);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading data: {ex.Message}");
                MessageBox.Show($"❌ Lỗi tải dữ liệu: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Wait(false);
                ShowLoadingState(false);
                _isLoading = false;
            }
        }

        private void UpdatePageInfo(string keyword)
        {
            try
            {
                if (lblPageInfo == null) return;

                if (_totalRecords == 0)
                {
                    lblPageInfo.Text = "📊 Tổng cộng: 0 khách hàng";
                }
                else
                {
                    int from = (_currentPage - 1) * _pageSize + 1;
                    int to = Math.Min(_currentPage * _pageSize, _totalRecords);
                    int totalPages = (int)Math.Ceiling((double)_totalRecords / _pageSize);
                    string searchInfo = string.IsNullOrEmpty(keyword) ? "" : $" (Tìm: '{keyword}')";
                    lblPageInfo.Text = $"📊 Hiển thị {from} - {to} / {_totalRecords} khách hàng | Trang {_currentPage}/{totalPages}{searchInfo}";
                }

                _listEnhancer.SetPaginationInfo(_currentPage, _pageSize, _totalRecords);
            }
            catch { }
        }

        private void UpdatePaginationButtons()
        {
            try
            {
                if (btnPrevious != null)
                    btnPrevious.Enabled = _currentPage > 1 && !_isLoading;

                if (btnNext != null)
                    btnNext.Enabled = _currentPage * _pageSize < _totalRecords && !_isLoading;
            }
            catch { }
        }

        #endregion

        #region Sorting

        private void ApplySorting(string columnName, bool ascending)
        {
            try
            {
                if (_allData == null || _allData.Count == 0) return;

                switch (columnName)
                {
                    case "CustomerID":
                        _allData = ascending
                            ? _allData.OrderBy(x => x.CustomerID).ToList()
                            : _allData.OrderByDescending(x => x.CustomerID).ToList();
                        break;
                    case "CustomerName":
                        _allData = ascending
                            ? _allData.OrderBy(x => x.CustomerName).ToList()
                            : _allData.OrderByDescending(x => x.CustomerName).ToList();
                        break;
                    case "PhoneNumber":
                        _allData = ascending
                            ? _allData.OrderBy(x => x.PhoneNumber).ToList()
                            : _allData.OrderByDescending(x => x.PhoneNumber).ToList();
                        break;
                    case "Address":
                        _allData = ascending
                            ? _allData.OrderBy(x => x.Address).ToList()
                            : _allData.OrderByDescending(x => x.Address).ToList();
                        break;
                }

                _currentPage = 1;
                LoadData();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error applying sorting: {ex.Message}");
            }
        }

        #endregion

        #region Customer Operations

        private void AddNewCustomer()
        {
            if (_isLoading) return;
            try
            {
                using (var form = new FormCustomerAdd())
                {
                    if (UIHelper.ShowFormDialog(this, form) == DialogResult.OK)
                    {
                        _currentPage = 1;
                        LoadData();
                        ShowInfo("✅ Thêm khách hàng thành công!");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error opening add form: {ex.Message}");
                MessageBox.Show($"❌ Lỗi mở form thêm: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void EditCurrentCustomer()
        {
            if (dgvCustomer?.CurrentRow == null)
            {
                ShowWarning("Vui lòng chọn khách hàng để sửa.");
                return;
            }
            EditCustomerAtRow(dgvCustomer.CurrentRow.Index);
        }

        private void EditCustomerAtRow(int rowIndex)
        {
            try
            {
                if (rowIndex < 0 || rowIndex >= dgvCustomer.Rows.Count) return;

                var cellValue = dgvCustomer.Rows[rowIndex].Cells["CustomerID"].Value;
                if (cellValue == null || !int.TryParse(cellValue.ToString(), out int customerId)) return;

                using (var form = new FormCustomerEdit(customerId))
                {
                    if (UIHelper.ShowFormDialog(this, form) == DialogResult.OK)
                    {
                        LoadData();
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex, "EditCustomerAtRow");
            }
        }

        private bool DeleteCustomer(int customerId)
        {
            try
            {
                Wait(true);
                _bll.Delete(customerId);
                ShowInfo("✅ Xóa khách hàng thành công!");
                _listEnhancer.OnDeleteSuccess();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Lỗi xóa: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            finally
            {
                Wait(false);
            }
        }

        private void ConfirmAndDeleteCustomer(int customerId)
        {
            try
            {
                var customer = _allData.FirstOrDefault(x => x.CustomerID == customerId);
                string customerName = customer?.CustomerName ?? "khách hàng này";

                if (!ShowConfirm($"🗑️ Bạn có chắc muốn xóa '{customerName}'?\n\nHành động này không thể hoàn tác!", "Xác nhận xóa"))
                    return;

                DeleteCustomer(customerId);
            }
            catch { }
        }

        private bool BatchDeleteCustomers(List<int> customerIds)
        {
            try
            {
                if (customerIds == null || customerIds.Count == 0) return false;

                if (!ShowConfirm($"🗑️ Bạn có chắc muốn xóa {customerIds.Count} khách hàng đã chọn?\n\nHành động này không thể hoàn tác!", "Xác nhận xóa"))
                    return false;

                Wait(true);
                int ok = 0, fail = 0;
                foreach (var id in customerIds)
                {
                    try
                    {
                        _bll.Delete(id);
                        ok++;
                    }
                    catch
                    {
                        fail++;
                    }
                }
                ShowInfo($"✅ Đã xóa {ok} | ❌ Lỗi {fail}");
                _listEnhancer.OnBatchDeleteSuccess();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Lỗi xóa: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            finally
            {
                Wait(false);
            }
        }

        private void ConfirmAndBatchDeleteCustomers(List<int> customerIds)
        {
            BatchDeleteCustomers(customerIds);
        }

        #endregion

        #region Order Operations

        private void CreateOrderForCurrentCustomer()
        {
            try
            {
                if (dgvCustomer?.CurrentRow == null)
                {
                    ShowWarning("Vui lòng chọn khách hàng để tạo đơn hàng.");
                    return;
                }

                CreateOrderForCustomerAtRow(dgvCustomer.CurrentRow.Index);
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex, "CreateOrderForCurrentCustomer");
            }
        }

        private void CreateOrderForCustomerAtRow(int rowIndex)
        {
            try
            {
                if (rowIndex < 0 || rowIndex >= dgvCustomer.Rows.Count) return;

                var row = dgvCustomer.Rows[rowIndex];
                int id = 0;
                int.TryParse(row.Cells["CustomerID"].Value?.ToString(), out id);

                var customer = new CustomerDTO
                {
                    CustomerID = id,
                    CustomerName = row.Cells["CustomerName"].Value?.ToString(),
                    PhoneNumber = row.Cells["PhoneNumber"].Value?.ToString(),
                    Address = row.Cells["Address"].Value?.ToString(),
                };

                using (var form = new FormOrderCreate(customer))
                {
                    UIHelper.ShowFormDialog(this, form);
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex, "CreateOrderForCustomerAtRow");
            }
        }

        #endregion

        #region Import/Export

        private void ImportFromFile()
        {
            if (_isLoading) return;
            try
            {
                using (var ofd = new OpenFileDialog())
                {
                    ofd.Filter = "CSV Files (*.csv)|*.csv|Excel Files (*.xls;*.xlsx)|*.xls;*.xlsx|All Files (*.*)|*.*";
                    ofd.Title = "Chọn file để nhập dữ liệu";

                    if (UIHelper.ShowOpenFileDialog(this, ofd) == DialogResult.OK)
                    {
                        ImportDataFromFile(ofd.FileName);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Lỗi nhập dữ liệu: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ImportDataFromFile(string filePath)
        {
            try
            {
                Wait(true);
                int importedCount = 0;
                int errorCount = 0;

                if (filePath.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
                {
                    var lines = File.ReadAllLines(filePath, Encoding.UTF8);
                    if (lines.Length <= 1)
                    {
                        MessageBox.Show("❌ File CSV trống hoặc không có dữ liệu!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    for (int i = 1; i < lines.Length; i++)
                    {
                        try
                        {
                            var parts = lines[i].Split(',');
                            if (parts.Length < 3) continue;

                            var customer = new CustomerDTO
                            {
                                CustomerName = parts[1].Trim(),
                                PhoneNumber = parts[2].Trim(),
                                Address = parts.Length > 3 ? parts[3].Trim() : ""
                            };

                            if (ValidateCustomer(customer))
                            {
                                _bll.Insert(customer);
                                importedCount++;
                            }
                            else
                            {
                                errorCount++;
                            }
                        }
                        catch
                        {
                            errorCount++;
                        }
                    }
                }
                else
                {
                    MessageBox.Show("⚠️ Hiện tại chỉ hỗ trợ import từ file CSV.\n\nVui lòng chuyển đổi file Excel sang CSV.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                Wait(false);
                MessageBox.Show($"✅ Nhập dữ liệu thành công!\n\n✓ Thêm: {importedCount} khách hàng\n✗ Lỗi: {errorCount} dòng", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadData();
            }
            catch (Exception ex)
            {
                Wait(false);
                MessageBox.Show($"❌ Lỗi: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool ValidateCustomer(CustomerDTO customer)
        {
            if (string.IsNullOrWhiteSpace(customer.CustomerName)) return false;
            if (string.IsNullOrWhiteSpace(customer.PhoneNumber)) return false;
            if (customer.PhoneNumber.Length < 9) return false;
            return true;
        }

        private void ExportCurrentPage()
        {
            try
            {
                var current = dgvCustomer?.DataSource as IEnumerable<CustomerDTO> ?? Enumerable.Empty<CustomerDTO>();
                DoExport(current, "TrangHienTai");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Lỗi xuất: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ExportSelected()
        {
            try
            {
                var selectedIds = _listEnhancer.GetSelectedIds();
                if (selectedIds.Count == 0)
                {
                    MessageBox.Show("⚠️ Chưa có mục nào được chọn!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                var selected = _allData.Where(x => selectedIds.Contains(x.CustomerID));
                DoExport(selected, "DaChon");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Lỗi xuất: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ExportAll()
        {
            try
            {
                DoExport(_allData, "TatCa");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Lỗi xuất: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DoExport(IEnumerable<CustomerDTO> list, string baseFileName)
        {
            try
            {
                using (var sfd = new SaveFileDialog())
                {
                    sfd.Filter = "Excel (*.xls)|*.xls|CSV (*.csv)|*.csv";
                    sfd.FileName = $"{baseFileName}_{DateTime.Now:yyyyMMdd_HHmmss}.xls";

                    if (UIHelper.ShowSaveFileDialog(this, sfd) == DialogResult.OK)
                    {
                        Wait(true);
                        if (sfd.FileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
                        {
                            var lines = new List<string> { "ID,Tên,SĐT,Địa chỉ" };
                            foreach (var c in list)
                            {
                                lines.Add($"{c.CustomerID},{ListEnhancer.EscapeCsv(c.CustomerName)},{ListEnhancer.EscapeCsv(c.PhoneNumber)},{ListEnhancer.EscapeCsv(c.Address)}");
                            }
                            File.WriteAllLines(sfd.FileName, lines, Encoding.UTF8);
                        }
                        else
                        {
                            var sb = new StringBuilder();
                            sb.AppendLine("<html><head><meta http-equiv='Content-Type' content='text/html; charset=utf-8' /><style>body{font-family:Segoe UI;margin:20px;background:#f9f9f9;} h3{color:#2980b9;border-bottom:2px solid #2980b9;padding-bottom:10px;} table{border-collapse:collapse;width:100%;background:white;box-shadow:0 2px 4px rgba(0,0,0,0.1);} th{background:#2980b9;color:white;padding:12px;text-align:left;font-weight:bold;} td{border:1px solid #ddd;padding:10px;} tr:nth-child(even){background:#f5f5f5;} tr:hover{background:#e8f4f8;transition:background 0.3s;} p{color:#666;font-size:12px;}</style></head><body>");
                            sb.AppendLine($"<h3>📋 Danh sách khách hàng ({list.Count()} bản ghi)</h3>");
                            sb.AppendLine($"<p>Xuất lúc: {DateTime.Now:dd/MM/yyyy HH:mm:ss}</p>");
                            sb.AppendLine("<table border='1' cellspacing='0' cellpadding='6'>");
                            sb.AppendLine("<tr><th>ID</th><th>Tên khách hàng</th><th>SĐT</th><th>Địa chỉ</th></tr>");
                            foreach (var c in list)
                            {
                                sb.AppendLine($"<tr><td>{c.CustomerID}</td><td>{ListEnhancer.HtmlEncode(c.CustomerName)}</td><td>{ListEnhancer.HtmlEncode(c.PhoneNumber)}</td><td>{ListEnhancer.HtmlEncode(c.Address)}</td></tr>");
                            }
                            sb.AppendLine("</table></body></html>");
                            File.WriteAllText(sfd.FileName, sb.ToString(), Encoding.UTF8);
                        }
                        Wait(false);
                        MessageBox.Show($"✅ Xuất danh sách khách hàng thành công!\n\nFile: {Path.GetFileName(sfd.FileName)}", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                Wait(false);
                MessageBox.Show($"❌ Lỗi xuất: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region Pagination

        private void PreviousPage()
        {
            if (_currentPage > 1 && !_isLoading)
            {
                _currentPage--;
                LoadData();
            }
        }

        private void NextPage()
        {
            if (_currentPage * _pageSize < _totalRecords && !_isLoading)
            {
                _currentPage++;
                LoadData();
            }
        }

        #endregion

        #region UI Helpers

        private void BuildEmptyState()
        {
            _lblEmptyState = new Label
            {
                Text = "📭 Không có dữ liệu khách hàng\n\nNhấn '➕ Thêm khách hàng' để bắt đầu",
                AutoSize = false,
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill,
                ForeColor = Color.FromArgb(150, 150, 150),
                Font = new Font("Segoe UI", 12, FontStyle.Regular),
                Visible = false,
                BackColor = Color.FromArgb(250, 250, 250)
            };
            this.Controls.Add(_lblEmptyState);
            _lblEmptyState.BringToFront();
        }

        private void BuildLoadingIndicator()
        {
            _lblLoadingIndicator = new Label
            {
                Text = "⏳ Đang tải dữ liệu...",
                AutoSize = false,
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill,
                ForeColor = Color.FromArgb(52, 152, 219),
                Font = new Font("Segoe UI", 11, FontStyle.Regular),
                BackColor = Color.FromArgb(240, 248, 255),
                Visible = false
            };
            this.Controls.Add(_lblLoadingIndicator);
            _lblLoadingIndicator.BringToFront();
        }

        private void BuildQuickOrderButton()
        {
            try
            {
                _btnQuickOrder = new Button
                {
                    Text = "🛒 Tạo đơn",
                    Width = 110,
                    Height = 34,
                    BackColor = Color.FromArgb(46, 204, 113),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Font = new Font("Segoe UI", 9, FontStyle.Bold),
                    Anchor = AnchorStyles.Top | AnchorStyles.Right
                };
                _btnQuickOrder.FlatAppearance.BorderSize = 0;
                _btnQuickOrder.Location = new Point(this.ClientSize.Width - _btnQuickOrder.Width - 20, 15);
                _btnQuickOrder.Click += (s, e) => CreateOrderForCurrentCustomer();
                this.Controls.Add(_btnQuickOrder);
                _btnQuickOrder.BringToFront();

                this.Resize += (s, e) =>
                {
                    if (_btnQuickOrder != null && !_isDisposed)
                    {
                        _btnQuickOrder.Location = new Point(this.ClientSize.Width - _btnQuickOrder.Width - 20, 15);
                    }
                };
            }
            catch { }
        }

        private void UpdateEmptyState(bool isEmpty)
        {
            if (_lblEmptyState == null) return;
            _lblEmptyState.Visible = isEmpty;
        }

        private void ShowLoadingState(bool isLoading)
        {
            if (_lblLoadingIndicator == null) return;
            _lblLoadingIndicator.Visible = isLoading;
        }

        #endregion

        #region Keyboard Shortcuts

        private void FormCustomerList_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                _listEnhancer?.HandleFormKeyDown(e);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"KeyDown error: {ex.Message}");
            }
        }

        #endregion
    }
}

