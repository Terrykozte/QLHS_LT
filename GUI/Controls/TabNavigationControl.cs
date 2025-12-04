using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Guna.UI2.WinForms;
using QLTN_LT.GUI.Utilities;

namespace QLTN_LT.GUI.Controls
{
    /// <summary>
    /// Custom tab navigation control
    /// </summary>
    public partial class TabNavigationControl : UserControl
    {
        private Guna2Panel pnlTabContainer;
        private FlowLayoutPanel flowTabs;
        private Guna2Panel pnlContent;
        private Dictionary<string, Guna2Button> _tabs = new Dictionary<string, Guna2Button>();
        private Dictionary<string, Control> _tabContents = new Dictionary<string, Control>();
        private string _activeTabKey = "";
        private Color _activeTabColor = Color.FromArgb(59, 130, 246);
        private Color _inactiveTabColor = Color.FromArgb(107, 114, 128);

        public event EventHandler TabChanged;

        public TabNavigationControl()
        {
            InitializeComponent();
            SetupUI();
        }

        private void SetupUI()
        {
            this.BackColor = Color.Transparent;
            this.Size = new Size(600, 400);

            // Tab container
            pnlTabContainer = new Guna2Panel
            {
                Dock = DockStyle.Top,
                Height = 50,
                FillColor = Color.FromArgb(17, 24, 39),
                BorderRadius = 0,
                Padding = new Padding(10, 5, 10, 0)
            };

            // Flow layout for tabs
            flowTabs = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                AutoScroll = true,
                BackColor = Color.Transparent,
                WrapContents = false
            };

            pnlTabContainer.Controls.Add(flowTabs);

            // Content panel
            pnlContent = new Guna2Panel
            {
                Dock = DockStyle.Fill,
                FillColor = Color.FromArgb(31, 41, 55),
                BorderRadius = 0,
                Padding = new Padding(10)
            };

            this.Controls.Add(pnlContent);
            this.Controls.Add(pnlTabContainer);
        }

        /// <summary>
        /// Thêm tab mới
        /// </summary>
        public void AddTab(string key, string displayText, Control content = null)
        {
            if (_tabs.ContainsKey(key)) return;

            // Create tab button
            var tabButton = new Guna2Button
            {
                Text = displayText,
                Size = new Size(120, 40),
                FillColor = Color.Transparent,
                ForeColor = _inactiveTabColor,
                Font = new Font("Segoe UI", 10),
                BorderRadius = 5,
                Cursor = Cursors.Hand,
                Tag = key,
                Margin = new Padding(5, 0, 5, 0)
            };

            tabButton.Click += (s, e) => SelectTab(key);

            flowTabs.Controls.Add(tabButton);
            _tabs[key] = tabButton;

            if (content != null)
            {
                _tabContents[key] = content;
            }

            // Select first tab
            if (_activeTabKey == "")
            {
                SelectTab(key);
            }
        }

        /// <summary>
        /// Chọn tab
        /// </summary>
        public void SelectTab(string key)
        {
            if (!_tabs.ContainsKey(key)) return;

            // Deactivate previous tab
            if (!string.IsNullOrEmpty(_activeTabKey) && _tabs.ContainsKey(_activeTabKey))
            {
                var prevTab = _tabs[_activeTabKey];
                prevTab.ForeColor = _inactiveTabColor;
                prevTab.FillColor = Color.Transparent;

                // Hide previous content
                if (_tabContents.ContainsKey(_activeTabKey))
                {
                    var prevContent = _tabContents[_activeTabKey];
                    if (prevContent != null)
                    {
                        AnimationHelper.FadeOut(prevContent, 200, () =>
                        {
                            prevContent.Visible = false;
                        });
                    }
                }
            }

            // Activate new tab
            _activeTabKey = key;
            var activeTab = _tabs[key];
            activeTab.ForeColor = Color.White;
            activeTab.FillColor = _activeTabColor;

            // Show new content
            if (_tabContents.ContainsKey(key))
            {
                var content = _tabContents[key];
                if (content != null)
                {
                    pnlContent.Controls.Clear();
                    content.Dock = DockStyle.Fill;
                    pnlContent.Controls.Add(content);
                    content.Visible = true;
                    AnimationHelper.FadeIn(content, 200);
                }
            }

            TabChanged?.Invoke(this, new EventArgs());
        }

        /// <summary>
        /// Lấy tab hiện tại
        /// </summary>
        public string GetActiveTab()
        {
            return _activeTabKey;
        }

        /// <summary>
        /// Xóa tab
        /// </summary>
        public void RemoveTab(string key)
        {
            if (!_tabs.ContainsKey(key)) return;

            flowTabs.Controls.Remove(_tabs[key]);
            _tabs.Remove(key);
            _tabContents.Remove(key);

            if (_activeTabKey == key && _tabs.Count > 0)
            {
                SelectTab(_tabs.Keys.GetEnumerator().Current);
            }
        }

        /// <summary>
        /// Xóa tất cả tabs
        /// </summary>
        public void ClearTabs()
        {
            flowTabs.Controls.Clear();
            _tabs.Clear();
            _tabContents.Clear();
            _activeTabKey = "";
        }

        /// <summary>
        /// Cập nhật tab content
        /// </summary>
        public void UpdateTabContent(string key, Control newContent)
        {
            if (_tabContents.ContainsKey(key))
            {
                _tabContents[key] = newContent;
                
                if (_activeTabKey == key)
                {
                    SelectTab(key);
                }
            }
        }

        /// <summary>
        /// Đặt màu cho tab
        /// </summary>
        public void SetTabColors(Color activeColor, Color inactiveColor)
        {
            _activeTabColor = activeColor;
            _inactiveTabColor = inactiveColor;

            // Update existing tabs
            foreach (var tab in _tabs.Values)
            {
                if (tab.Tag?.ToString() == _activeTabKey)
                {
                    tab.ForeColor = Color.White;
                    tab.FillColor = _activeTabColor;
                }
                else
                {
                    tab.ForeColor = _inactiveTabColor;
                    tab.FillColor = Color.Transparent;
                }
            }
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.ResumeLayout(false);
        }
    }
}

