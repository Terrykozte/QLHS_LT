using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;

namespace QLTN_LT.GUI.Utilities
{
    /// <summary>
    /// Hỗ trợ keyboard navigation (Tab, Arrow keys, etc.)
    /// </summary>
    public static class KeyboardNavigationHelper
    {
        private static Dictionary<Form, NavigationContext> _navigationContexts = new Dictionary<Form, NavigationContext>();

        private class NavigationContext
        {
            public List<Control> NavigableControls { get; set; } = new List<Control>();
            public int CurrentIndex { get; set; } = -1;
            public Control CurrentControl { get; set; }
        }

        /// <summary>
        /// Đăng ký form để hỗ trợ keyboard navigation
        /// </summary>
        public static void RegisterForm(Form form, List<Control> navigableControls = null)
        {
            if (form == null) return;

            var context = new NavigationContext();
            
            if (navigableControls != null && navigableControls.Count > 0)
            {
                context.NavigableControls = navigableControls;
            }
            else
            {
                // Tự động lấy tất cả control có thể focus
                context.NavigableControls = GetFocusableControls(form);
            }

            if (!_navigationContexts.ContainsKey(form))
            {
                _navigationContexts[form] = context;
            }

            // Setup keyboard event
            form.KeyDown += (s, e) => HandleKeyDown(form, e);
        }

        /// <summary>
        /// Lấy tất cả control có thể focus
        /// </summary>
        private static List<Control> GetFocusableControls(Control parent)
        {
            var controls = new List<Control>();

            foreach (Control control in parent.Controls)
            {
                if (control.CanFocus && control.Visible && control.Enabled)
                {
                    controls.Add(control);
                }

                // Recursive for nested controls
                if (control.HasChildren)
                {
                    controls.AddRange(GetFocusableControls(control));
                }
            }

            return controls;
        }

        /// <summary>
        /// Xử lý keyboard events
        /// </summary>
        private static void HandleKeyDown(Form form, KeyEventArgs e)
        {
            if (!_navigationContexts.ContainsKey(form)) return;

            var context = _navigationContexts[form];
            if (context.NavigableControls.Count == 0) return;

            switch (e.KeyCode)
            {
                case Keys.Tab:
                    e.Handled = true;
                    if (e.Shift)
                        NavigatePrevious(form, context);
                    else
                        NavigateNext(form, context);
                    break;

                case Keys.Up:
                    e.Handled = true;
                    NavigateUp(form, context);
                    break;

                case Keys.Down:
                    e.Handled = true;
                    NavigateDown(form, context);
                    break;

                case Keys.Left:
                    e.Handled = true;
                    NavigateLeft(form, context);
                    break;

                case Keys.Right:
                    e.Handled = true;
                    NavigateRight(form, context);
                    break;

                case Keys.Enter:
                    if (context.CurrentControl is Button btn)
                    {
                        e.Handled = true;
                        btn.PerformClick();
                    }
                    break;

                case Keys.Escape:
                    // Có thể dùng để đóng form hoặc cancel
                    break;
            }
        }

        /// <summary>
        /// Điều hướng đến control tiếp theo
        /// </summary>
        private static void NavigateNext(Form form, NavigationContext context)
        {
            if (context.NavigableControls.Count == 0) return;

            context.CurrentIndex++;
            if (context.CurrentIndex >= context.NavigableControls.Count)
            {
                context.CurrentIndex = 0;
            }

            FocusControl(context.NavigableControls[context.CurrentIndex]);
        }

        /// <summary>
        /// Điều hướng đến control trước đó
        /// </summary>
        private static void NavigatePrevious(Form form, NavigationContext context)
        {
            if (context.NavigableControls.Count == 0) return;

            context.CurrentIndex--;
            if (context.CurrentIndex < 0)
            {
                context.CurrentIndex = context.NavigableControls.Count - 1;
            }

            FocusControl(context.NavigableControls[context.CurrentIndex]);
        }

        /// <summary>
        /// Điều hướng lên
        /// </summary>
        private static void NavigateUp(Form form, NavigationContext context)
        {
            var current = form.ActiveControl;
            if (current == null) return;

            var focusable = GetFocusableControls(form);
            var currentIndex = focusable.IndexOf(current);

            if (currentIndex > 0)
            {
                focusable[currentIndex - 1].Focus();
            }
            else if (focusable.Count > 0)
            {
                focusable[focusable.Count - 1].Focus();
            }
        }

        /// <summary>
        /// Điều hướng xuống
        /// </summary>
        private static void NavigateDown(Form form, NavigationContext context)
        {
            var current = form.ActiveControl;
            if (current == null) return;

            var focusable = GetFocusableControls(form);
            var currentIndex = focusable.IndexOf(current);

            if (currentIndex < focusable.Count - 1)
            {
                focusable[currentIndex + 1].Focus();
            }
            else if (focusable.Count > 0)
            {
                focusable[0].Focus();
            }
        }

        /// <summary>
        /// Điều hướng trái
        /// </summary>
        private static void NavigateLeft(Form form, NavigationContext context)
        {
            var current = form.ActiveControl;
            if (current == null) return;

            // Nếu là DataGridView, di chuyển sang cột trái
            if (current is DataGridView dgv && dgv.CurrentCell != null)
            {
                int colIndex = dgv.CurrentCell.ColumnIndex - 1;
                if (colIndex >= 0)
                {
                    dgv.CurrentCell = dgv.Rows[dgv.CurrentCell.RowIndex].Cells[colIndex];
                }
            }
            else
            {
                // Mặc định, di chuyển đến control trước
                NavigatePrevious(form, context);
            }
        }

        /// <summary>
        /// Điều hướng phải
        /// </summary>
        private static void NavigateRight(Form form, NavigationContext context)
        {
            var current = form.ActiveControl;
            if (current == null) return;

            // Nếu là DataGridView, di chuyển sang cột phải
            if (current is DataGridView dgv && dgv.CurrentCell != null)
            {
                int colIndex = dgv.CurrentCell.ColumnIndex + 1;
                if (colIndex < dgv.Columns.Count)
                {
                    dgv.CurrentCell = dgv.Rows[dgv.CurrentCell.RowIndex].Cells[colIndex];
                }
            }
            else
            {
                // Mặc định, di chuyển đến control tiếp theo
                NavigateNext(form, context);
            }
        }

        /// <summary>
        /// Focus vào một control và highlight nó
        /// </summary>
        private static void FocusControl(Control control)
        {
            if (control == null) return;

            try
            {
                control.Focus();
                
                // Highlight effect
                if (control is TextBox textBox)
                {
                    textBox.SelectAll();
                }
            }
            catch { }
        }

        /// <summary>
        /// Unregister form
        /// </summary>
        public static void UnregisterForm(Form form)
        {
            if (form != null && _navigationContexts.ContainsKey(form))
            {
                _navigationContexts.Remove(form);
            }
        }

        /// <summary>
        /// Hỗ trợ arrow keys cho DataGridView
        /// </summary>
        public static void EnableDataGridViewArrowNavigation(DataGridView dgv)
        {
            if (dgv == null) return;

            dgv.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Up || e.KeyCode == Keys.Down || 
                    e.KeyCode == Keys.Left || e.KeyCode == Keys.Right)
                {
                    // Allow default behavior
                    e.Handled = false;
                }
            };
        }

        /// <summary>
        /// Hỗ trợ Tab key cho DataGridView
        /// </summary>
        public static void EnableDataGridViewTabNavigation(DataGridView dgv)
        {
            if (dgv == null) return;

            dgv.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Tab)
                {
                    e.Handled = true;
                    
                    if (dgv.CurrentCell == null) return;

                    int nextCol = e.Shift 
                        ? dgv.CurrentCell.ColumnIndex - 1 
                        : dgv.CurrentCell.ColumnIndex + 1;

                    if (nextCol < 0 || nextCol >= dgv.Columns.Count)
                    {
                        // Move to next/previous row
                        int nextRow = e.Shift 
                            ? dgv.CurrentCell.RowIndex - 1 
                            : dgv.CurrentCell.RowIndex + 1;

                        if (nextRow >= 0 && nextRow < dgv.Rows.Count)
                        {
                            nextCol = e.Shift ? dgv.Columns.Count - 1 : 0;
                            dgv.CurrentCell = dgv.Rows[nextRow].Cells[nextCol];
                        }
                    }
                    else
                    {
                        dgv.CurrentCell = dgv.Rows[dgv.CurrentCell.RowIndex].Cells[nextCol];
                    }
                }
            };
        }
    }
}

