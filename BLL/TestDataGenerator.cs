using System;
using System.Collections.Generic;
using QLTN_LT.DTO;
using QLTN_LT.DAL;

namespace QLTN_LT.BLL
{
    /// <summary>
    /// Lớp để tạo dữ liệu test cho Menu
    /// Sử dụng để test API VietQR và các tính năng khác
    /// </summary>
    public class TestDataGenerator
    {
        private readonly MenuDAL _menuDAL;
        private readonly CategoryDAL _categoryDAL;

        public TestDataGenerator()
        {
            _menuDAL = new MenuDAL();
            _categoryDAL = new CategoryDAL();
        }

        /// <summary>
        /// Tạo tất cả dữ liệu test
        /// </summary>
        public void GenerateAllTestData()
        {
            try
            {
                GenerateMenuCategories();
                GenerateMenuItems();
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tạo dữ liệu test: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Tạo danh mục thực đơn test
        /// </summary>
        private void GenerateMenuCategories()
        {
            var categories = new List<MenuCategoryDTO>
            {
                new MenuCategoryDTO { CategoryName = "Hải Sản Tươi", DisplayOrder = 1, IsActive = true },
                new MenuCategoryDTO { CategoryName = "Cua & Tôm", DisplayOrder = 2, IsActive = true },
                new MenuCategoryDTO { CategoryName = "Cá", DisplayOrder = 3, IsActive = true },
                new MenuCategoryDTO { CategoryName = "Mực & Cá Mực", DisplayOrder = 4, IsActive = true },
                new MenuCategoryDTO { CategoryName = "Đặc Biệt", DisplayOrder = 5, IsActive = true }
            };

            foreach (var category in categories)
            {
                try
                {
                    // Kiểm tra danh mục đã tồn tại chưa
                    var existing = _categoryDAL.GetByName(category.CategoryName);
                    if (existing == null)
                    {
                        _categoryDAL.Insert(category);
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Lỗi tạo danh mục {category.CategoryName}: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Tạo các món ăn test
        /// </summary>
        private void GenerateMenuItems()
        {
            var items = new List<MenuItemDTO>
            {
                // Danh mục: Hải Sản Tươi
                new MenuItemDTO
                {
                    ItemCode = "HS001",
                    ItemName = "Tôm Sú Nướng",
                    UnitPrice = 250000,
                    UnitName = "Phần",
                    Description = "Tôm Sú tươi nướng với gia vị đặc biệt",
                    IsAvailable = true,
                    CategoryName = "Hải Sản Tươi"
                },
                new MenuItemDTO
                {
                    ItemCode = "HS002",
                    ItemName = "Cua Hoàng Đế Hấp",
                    UnitPrice = 350000,
                    UnitName = "Phần",
                    Description = "Cua hoàng đế tươi hấp với nước dùng",
                    IsAvailable = true,
                    CategoryName = "Hải Sản Tươi"
                },
                new MenuItemDTO
                {
                    ItemCode = "HS003",
                    ItemName = "Sò Điệp Nướng Bơ",
                    UnitPrice = 280000,
                    UnitName = "Phần",
                    Description = "Sò điệp tươi nướng với bơ tỏi",
                    IsAvailable = true,
                    CategoryName = "Hải Sản Tươi"
                },
                new MenuItemDTO
                {
                    ItemCode = "HS004",
                    ItemName = "Hàu Nướng Muối Ớt",
                    UnitPrice = 200000,
                    UnitName = "Phần",
                    Description = "Hàu tươi nướng với muối ớt",
                    IsAvailable = true,
                    CategoryName = "Hải Sản Tươi"
                },

                // Danh mục: Cua & Tôm
                new MenuItemDTO
                {
                    ItemCode = "CT001",
                    ItemName = "Tôm Hùm Nướng Chanh",
                    UnitPrice = 450000,
                    UnitName = "Phần",
                    Description = "Tôm hùm nướng với chanh tươi",
                    IsAvailable = true,
                    CategoryName = "Cua & Tôm"
                },
                new MenuItemDTO
                {
                    ItemCode = "CT002",
                    ItemName = "Cua Biển Hấp Rượu",
                    UnitPrice = 380000,
                    UnitName = "Phần",
                    Description = "Cua biển hấp với rượu nấu",
                    IsAvailable = true,
                    CategoryName = "Cua & Tôm"
                },
                new MenuItemDTO
                {
                    ItemCode = "CT003",
                    ItemName = "Tôm Sú Xào Tỏi",
                    UnitPrice = 220000,
                    UnitName = "Phần",
                    Description = "Tôm sú xào với tỏi thơm",
                    IsAvailable = true,
                    CategoryName = "Cua & Tôm"
                },
                new MenuItemDTO
                {
                    ItemCode = "CT004",
                    ItemName = "Cua Cà Chua Xốt",
                    UnitPrice = 320000,
                    UnitName = "Phần",
                    Description = "Cua xào với cà chua và xốt đặc biệt",
                    IsAvailable = true,
                    CategoryName = "Cua & Tôm"
                },

                // Danh mục: Cá
                new MenuItemDTO
                {
                    ItemCode = "CA001",
                    ItemName = "Cá Hồi Nướng",
                    UnitPrice = 380000,
                    UnitName = "Phần",
                    Description = "Cá hồi tươi nướng với gia vị",
                    IsAvailable = true,
                    CategoryName = "Cá"
                },
                new MenuItemDTO
                {
                    ItemCode = "CA002",
                    ItemName = "Cá Bớp Hấp Gừng",
                    UnitPrice = 280000,
                    UnitName = "Phần",
                    Description = "Cá bớp hấp với gừng tươi",
                    IsAvailable = true,
                    CategoryName = "Cá"
                },
                new MenuItemDTO
                {
                    ItemCode = "CA003",
                    ItemName = "Cá Chim Chiên Giòn",
                    UnitPrice = 250000,
                    UnitName = "Phần",
                    Description = "Cá chim chiên giòn vàng",
                    IsAvailable = true,
                    CategoryName = "Cá"
                },
                new MenuItemDTO
                {
                    ItemCode = "CA004",
                    ItemName = "Cá Tầm Nướng Muối",
                    UnitPrice = 420000,
                    UnitName = "Phần",
                    Description = "Cá tầm nướng với muối",
                    IsAvailable = true,
                    CategoryName = "Cá"
                },

                // Danh mục: Mực & Cá Mực
                new MenuItemDTO
                {
                    ItemCode = "MU001",
                    ItemName = "Mực Nướng Muối Ớt",
                    UnitPrice = 280000,
                    UnitName = "Phần",
                    Description = "Mực tươi nướng với muối ớt",
                    IsAvailable = true,
                    CategoryName = "Mực & Cá Mực"
                },
                new MenuItemDTO
                {
                    ItemCode = "MU002",
                    ItemName = "Cá Mực Xào Dừa",
                    UnitPrice = 320000,
                    UnitName = "Phần",
                    Description = "Cá mực xào với nước cốt dừa",
                    IsAvailable = true,
                    CategoryName = "Mực & Cá Mực"
                },
                new MenuItemDTO
                {
                    ItemCode = "MU003",
                    ItemName = "Mực Nhồi Thịt Nướng",
                    UnitPrice = 350000,
                    UnitName = "Phần",
                    Description = "Mực nhồi thịt nướng",
                    IsAvailable = true,
                    CategoryName = "Mực & Cá Mực"
                },

                // Danh mục: Đặc Biệt
                new MenuItemDTO
                {
                    ItemCode = "DB001",
                    ItemName = "Lẩu Hải Sản Tổng Hợp",
                    UnitPrice = 550000,
                    UnitName = "Phần",
                    Description = "Lẩu với tôm, cua, mực, cá",
                    IsAvailable = true,
                    CategoryName = "Đặc Biệt"
                },
                new MenuItemDTO
                {
                    ItemCode = "DB002",
                    ItemName = "Bàn Hải Sản Hoàng Gia",
                    UnitPrice = 1200000,
                    UnitName = "Bàn",
                    Description = "Tôm hùm, cua hoàng đế, cá hồi, mực",
                    IsAvailable = true,
                    CategoryName = "Đặc Biệt"
                },
                new MenuItemDTO
                {
                    ItemCode = "DB003",
                    ItemName = "Cơm Chiên Hải Sản",
                    UnitPrice = 180000,
                    UnitName = "Phần",
                    Description = "Cơm chiên với tôm, cua, mực",
                    IsAvailable = true,
                    CategoryName = "Đặc Biệt"
                }
            };

            foreach (var item in items)
            {
                try
                {
                    // Kiểm tra item đã tồn tại chưa
                    var existing = _menuDAL.GetItemById(item.ItemID);
                    if (existing == null)
                    {
                        _menuDAL.InsertItem(item);
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Lỗi tạo món {item.ItemName}: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Xóa tất cả dữ liệu test
        /// </summary>
        public void DeleteAllTestData()
        {
            try
            {
                var items = _menuDAL.GetAllItems();
                foreach (var item in items)
                {
                    if (item.ItemCode != null && 
                        (item.ItemCode.StartsWith("HS") || 
                         item.ItemCode.StartsWith("CT") || 
                         item.ItemCode.StartsWith("CA") || 
                         item.ItemCode.StartsWith("MU") || 
                         item.ItemCode.StartsWith("DB")))
                    {
                        _menuDAL.DeleteItem(item.ItemID);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi xóa dữ liệu test: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Lấy thông tin thống kê dữ liệu test
        /// </summary>
        public Dictionary<string, int> GetTestDataStatistics()
        {
            var stats = new Dictionary<string, int>();
            try
            {
                var items = _menuDAL.GetAllItems();
                var categories = _menuDAL.GetAllCategories();

                stats["TotalItems"] = items.Count;
                stats["TotalCategories"] = categories.Count;

                foreach (var category in categories)
                {
                    int count = 0;
                    foreach (var item in items)
                    {
                        if (item.CategoryID == category.CategoryID)
                            count++;
                    }
                    stats[$"Category_{category.CategoryName}"] = count;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Lỗi lấy thống kê: {ex.Message}");
            }

            return stats;
        }
    }
}


