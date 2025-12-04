using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Linq;
using System.Reflection;

namespace QLTN_LT.GUI.Utilities
{
    /// <summary>
    /// Hỗ trợ data binding và caching tối ưu
    /// </summary>
    public static class DataBindingHelper
    {
        private static Dictionary<string, object> _cache = new Dictionary<string, object>();
        private static Dictionary<string, DateTime> _cacheExpiry = new Dictionary<string, DateTime>();
        private const int DEFAULT_CACHE_DURATION_SECONDS = 300; // 5 minutes

        /// <summary>
        /// Bind dữ liệu vào DataGridView
        /// </summary>
        public static void BindDataToGrid<T>(DataGridView dgv, List<T> data, bool autoGenerateColumns = true)
        {
            if (dgv == null || data == null) return;

            try
            {
                dgv.DataSource = null;
                dgv.DataSource = data;
                dgv.AutoGenerateColumns = autoGenerateColumns;

                // Adjust column widths
                foreach (DataGridViewColumn col in dgv.Columns)
                {
                    col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error binding data to grid: {ex.Message}");
            }
        }

        /// <summary>
        /// Bind dữ liệu vào ComboBox
        /// </summary>
        public static void BindDataToComboBox<T>(ComboBox comboBox, List<T> data, 
            string displayMember = null, string valueMember = null)
        {
            if (comboBox == null || data == null) return;

            try
            {
                comboBox.DataSource = null;
                comboBox.DataSource = data;
                
                if (!string.IsNullOrEmpty(displayMember))
                    comboBox.DisplayMember = displayMember;
                
                if (!string.IsNullOrEmpty(valueMember))
                    comboBox.ValueMember = valueMember;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error binding data to combobox: {ex.Message}");
            }
        }

        /// <summary>
        /// Bind dữ liệu vào ListBox
        /// </summary>
        public static void BindDataToListBox<T>(ListBox listBox, List<T> data, 
            string displayMember = null, string valueMember = null)
        {
            if (listBox == null || data == null) return;

            try
            {
                listBox.DataSource = null;
                listBox.DataSource = data;
                
                if (!string.IsNullOrEmpty(displayMember))
                    listBox.DisplayMember = displayMember;
                
                if (!string.IsNullOrEmpty(valueMember))
                    listBox.ValueMember = valueMember;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error binding data to listbox: {ex.Message}");
            }
        }

        /// <summary>
        /// Bind dữ liệu vào FlowLayoutPanel
        /// </summary>
        public static void BindDataToFlowPanel<T>(FlowLayoutPanel panel, List<T> data, 
            Func<T, Control> controlFactory)
        {
            if (panel == null || data == null || controlFactory == null) return;

            try
            {
                panel.Controls.Clear();
                
                foreach (var item in data)
                {
                    var control = controlFactory(item);
                    if (control != null)
                    {
                        panel.Controls.Add(control);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error binding data to flow panel: {ex.Message}");
            }
        }

        /// <summary>
        /// Lấy dữ liệu từ DataGridView
        /// </summary>
        public static List<T> GetDataFromGrid<T>(DataGridView dgv) where T : class, new()
        {
            if (dgv == null) return new List<T>();

            var result = new List<T>();

            try
            {
                foreach (DataGridViewRow row in dgv.Rows)
                {
                    if (row.IsNewRow) continue;

                    var item = new T();
                    
                    foreach (DataGridViewCell cell in row.Cells)
                    {
                        var columnName = dgv.Columns[cell.ColumnIndex].Name;
                        var property = typeof(T).GetProperty(columnName, 
                            BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

                        if (property != null && property.CanWrite)
                        {
                            try
                            {
                                property.SetValue(item, Convert.ChangeType(cell.Value, property.PropertyType));
                            }
                            catch { }
                        }
                    }

                    result.Add(item);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting data from grid: {ex.Message}");
            }

            return result;
        }

        /// <summary>
        /// Lấy dữ liệu từ ComboBox
        /// </summary>
        public static T GetSelectedItem<T>(ComboBox comboBox) where T : class
        {
            if (comboBox == null || comboBox.SelectedItem == null) return null;

            return comboBox.SelectedItem as T;
        }

        /// <summary>
        /// Lấy dữ liệu từ ListBox
        /// </summary>
        public static List<T> GetSelectedItems<T>(ListBox listBox) where T : class
        {
            if (listBox == null) return new List<T>();

            return listBox.SelectedItems.Cast<T>().ToList();
        }

        /// <summary>
        /// Cache dữ liệu
        /// </summary>
        public static void CacheData<T>(string key, T data, int durationSeconds = DEFAULT_CACHE_DURATION_SECONDS)
        {
            if (string.IsNullOrEmpty(key)) return;

            _cache[key] = data;
            _cacheExpiry[key] = DateTime.Now.AddSeconds(durationSeconds);
        }

        /// <summary>
        /// Lấy dữ liệu từ cache
        /// </summary>
        public static T GetCachedData<T>(string key) where T : class
        {
            if (string.IsNullOrEmpty(key) || !_cache.ContainsKey(key)) return null;

            // Check expiry
            if (_cacheExpiry.ContainsKey(key) && DateTime.Now > _cacheExpiry[key])
            {
                _cache.Remove(key);
                _cacheExpiry.Remove(key);
                return null;
            }

            return _cache[key] as T;
        }

        /// <summary>
        /// Kiểm tra cache có tồn tại không
        /// </summary>
        public static bool IsCacheValid(string key)
        {
            if (string.IsNullOrEmpty(key) || !_cache.ContainsKey(key)) return false;

            if (_cacheExpiry.ContainsKey(key) && DateTime.Now > _cacheExpiry[key])
            {
                _cache.Remove(key);
                _cacheExpiry.Remove(key);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Xóa cache
        /// </summary>
        public static void ClearCache(string key = null)
        {
            if (string.IsNullOrEmpty(key))
            {
                _cache.Clear();
                _cacheExpiry.Clear();
            }
            else
            {
                _cache.Remove(key);
                _cacheExpiry.Remove(key);
            }
        }

        /// <summary>
        /// Bind dữ liệu với filtering
        /// </summary>
        public static void BindDataWithFilter<T>(DataGridView dgv, List<T> data, 
            Func<T, bool> filterPredicate, bool autoGenerateColumns = true)
        {
            if (dgv == null || data == null) return;

            var filteredData = data.Where(filterPredicate).ToList();
            BindDataToGrid(dgv, filteredData, autoGenerateColumns);
        }

        /// <summary>
        /// Bind dữ liệu với sorting
        /// </summary>
        public static void BindDataWithSort<T>(DataGridView dgv, List<T> data, 
            Func<T, object> sortKeySelector, bool descending = false, bool autoGenerateColumns = true)
        {
            if (dgv == null || data == null) return;

            var sortedData = descending 
                ? data.OrderByDescending(sortKeySelector).ToList()
                : data.OrderBy(sortKeySelector).ToList();

            BindDataToGrid(dgv, sortedData, autoGenerateColumns);
        }

        /// <summary>
        /// Bind dữ liệu với paging
        /// </summary>
        public static void BindDataWithPaging<T>(DataGridView dgv, List<T> data, 
            int pageNumber, int pageSize, bool autoGenerateColumns = true)
        {
            if (dgv == null || data == null) return;

            var pagedData = data
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            BindDataToGrid(dgv, pagedData, autoGenerateColumns);
        }

        /// <summary>
        /// Bind dữ liệu với search
        /// </summary>
        public static void BindDataWithSearch<T>(DataGridView dgv, List<T> data, 
            string searchTerm, Func<T, string> searchSelector, bool autoGenerateColumns = true)
        {
            if (dgv == null || data == null) return;

            if (string.IsNullOrEmpty(searchTerm))
            {
                BindDataToGrid(dgv, data, autoGenerateColumns);
                return;
            }

            var searchTerm_lower = searchTerm.ToLower();
            var filteredData = data
                .Where(x => searchSelector(x)?.ToLower().Contains(searchTerm_lower) ?? false)
                .ToList();

            BindDataToGrid(dgv, filteredData, autoGenerateColumns);
        }

        /// <summary>
        /// Lấy tổng số trang
        /// </summary>
        public static int GetTotalPages<T>(List<T> data, int pageSize)
        {
            if (data == null || pageSize <= 0) return 0;
            return (int)Math.Ceiling((double)data.Count / pageSize);
        }

        /// <summary>
        /// Bind dữ liệu vào TreeView
        /// </summary>
        public static void BindDataToTreeView<T>(TreeView treeView, List<T> data, 
            Func<T, string> textSelector, Func<T, List<T>> childrenSelector)
        {
            if (treeView == null || data == null) return;

            try
            {
                treeView.Nodes.Clear();

                foreach (var item in data)
                {
                    var node = new TreeNode(textSelector(item)) { Tag = item };
                    AddChildNodes(node, childrenSelector(item), textSelector, childrenSelector);
                    treeView.Nodes.Add(node);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error binding data to treeview: {ex.Message}");
            }
        }

        private static void AddChildNodes<T>(TreeNode parentNode, List<T> children, 
            Func<T, string> textSelector, Func<T, List<T>> childrenSelector)
        {
            if (children == null) return;

            foreach (var child in children)
            {
                var childNode = new TreeNode(textSelector(child)) { Tag = child };
                AddChildNodes(childNode, childrenSelector(child), textSelector, childrenSelector);
                parentNode.Nodes.Add(childNode);
            }
        }

        /// <summary>
        /// Refresh data binding
        /// </summary>
        public static void RefreshBinding(DataGridView dgv)
        {
            if (dgv == null) return;

            try
            {
                dgv.Refresh();
                dgv.InvalidateRect(dgv.DisplayRectangle);
            }
            catch { }
        }

        /// <summary>
        /// Bind dữ liệu vào Panel với custom layout
        /// </summary>
        public static void BindDataToPanel<T>(Panel panel, List<T> data, 
            Func<T, Control> controlFactory, int columns = 1)
        {
            if (panel == null || data == null || controlFactory == null) return;

            try
            {
                panel.Controls.Clear();
                int row = 0;
                int col = 0;
                int controlHeight = 100;
                int controlWidth = panel.Width / columns - 10;

                foreach (var item in data)
                {
                    var control = controlFactory(item);
                    if (control != null)
                    {
                        control.Width = controlWidth;
                        control.Height = controlHeight;
                        control.Left = col * (controlWidth + 10) + 5;
                        control.Top = row * (controlHeight + 10) + 5;

                        panel.Controls.Add(control);

                        col++;
                        if (col >= columns)
                        {
                            col = 0;
                            row++;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error binding data to panel: {ex.Message}");
            }
        }
    }
}

