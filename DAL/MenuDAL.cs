using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using QLTN_LT.DTO;

namespace QLTN_LT.DAL
{
    public class MenuDAL
    {
        public List<MenuCategoryDTO> GetAllCategories()
        {
            var list = new List<MenuCategoryDTO>();
            const string sql = @"
                SELECT CategoryID, CategoryName, DisplayOrder, IsActive
                FROM dbo.MenuCategories
                ORDER BY DisplayOrder, CategoryName";

            using (var conn = DatabaseHelper.CreateAndOpenConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        list.Add(new MenuCategoryDTO
                        {
                            CategoryID = rd.GetInt32(0),
                            CategoryName = rd.GetString(1),
                            DisplayOrder = rd.GetInt32(2),
                            IsActive = rd.GetBoolean(3)
                        });
                    }
                }
            }
            return list;
        }

        public List<MenuItemDTO> GetAllItems()
        {
            var list = new List<MenuItemDTO>();
            const string sql = @"
                SELECT mi.ItemID, mi.CategoryID, mc.CategoryName, mi.ItemCode, mi.ItemName, 
                       mi.UnitPrice, mi.UnitName, mi.Description, mi.IsAvailable, mi.CreatedAt
                FROM dbo.MenuItems mi
                INNER JOIN dbo.MenuCategories mc ON mi.CategoryID = mc.CategoryID
                WHERE mc.IsActive = 1 AND mi.IsAvailable = 1
                ORDER BY mc.DisplayOrder, mi.ItemName";

            using (var conn = DatabaseHelper.CreateAndOpenConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        list.Add(new MenuItemDTO
                        {
                            ItemID = (int)rd["ItemID"],
                            CategoryID = (int)rd["CategoryID"],
                            CategoryName = rd["CategoryName"].ToString(),
                            ItemCode = rd.IsDBNull(rd.GetOrdinal("ItemCode")) ? null : rd["ItemCode"].ToString(),
                            ItemName = rd["ItemName"].ToString(),
                            UnitPrice = (decimal)rd["UnitPrice"],
                            UnitName = rd.IsDBNull(rd.GetOrdinal("UnitName")) ? null : rd["UnitName"].ToString(),
                            Description = rd.IsDBNull(rd.GetOrdinal("Description")) ? null : rd["Description"].ToString(),
                            IsAvailable = (bool)rd["IsAvailable"],
                            CreatedAt = (System.DateTime)rd["CreatedAt"]
                        });
                    }
                }
            }
            return list;
        }

        public MenuItemDTO GetItemById(int itemId)
        {
            MenuItemDTO item = null;
            const string sql = @"
                SELECT mi.ItemID, mi.CategoryID, mc.CategoryName, mi.ItemCode, mi.ItemName,
                       mi.UnitPrice, mi.UnitName, mi.Description, mi.IsAvailable, mi.CreatedAt
                FROM dbo.MenuItems mi
                INNER JOIN dbo.MenuCategories mc ON mi.CategoryID = mc.CategoryID
                WHERE mi.ItemID = @ItemID";

            using (var conn = DatabaseHelper.CreateAndOpenConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@ItemID", itemId);
                using (var rd = cmd.ExecuteReader())
                {
                    if (rd.Read())
                    {
                        item = new MenuItemDTO
                        {
                            ItemID = (int)rd["ItemID"],
                            CategoryID = (int)rd["CategoryID"],
                            CategoryName = rd["CategoryName"].ToString(),
                            ItemCode = rd.IsDBNull(rd.GetOrdinal("ItemCode")) ? null : rd["ItemCode"].ToString(),
                            ItemName = rd["ItemName"].ToString(),
                            UnitPrice = (decimal)rd["UnitPrice"],
                            UnitName = rd.IsDBNull(rd.GetOrdinal("UnitName")) ? null : rd["UnitName"].ToString(),
                            Description = rd.IsDBNull(rd.GetOrdinal("Description")) ? null : rd["Description"].ToString(),
                            IsAvailable = (bool)rd["IsAvailable"],
                            CreatedAt = (System.DateTime)rd["CreatedAt"]
                        };
                    }
                }
            }

            return item;
        }

        public void InsertItem(MenuItemDTO item)
        {
            const string sql = @"
                INSERT INTO dbo.MenuItems (CategoryID, ItemCode, ItemName, UnitPrice, UnitName, Description, IsAvailable, CreatedAt)
                VALUES (@CategoryID, @ItemCode, @ItemName, @UnitPrice, @UnitName, @Description, @IsAvailable, GETDATE())";

            using (var conn = DatabaseHelper.CreateAndOpenConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@CategoryID", item.CategoryID);
                cmd.Parameters.AddWithValue("@ItemCode", (object)item.ItemCode ?? System.DBNull.Value);
                cmd.Parameters.AddWithValue("@ItemName", item.ItemName);
                cmd.Parameters.AddWithValue("@UnitPrice", item.UnitPrice);
                cmd.Parameters.AddWithValue("@UnitName", (object)item.UnitName ?? System.DBNull.Value);
                cmd.Parameters.AddWithValue("@Description", (object)item.Description ?? System.DBNull.Value);
                cmd.Parameters.AddWithValue("@IsAvailable", item.IsAvailable);
                cmd.ExecuteNonQuery();
            }
        }

        public void UpdateItem(MenuItemDTO item)
        {
            const string sql = @"
                UPDATE dbo.MenuItems
                SET CategoryID = @CategoryID,
                    ItemCode = @ItemCode,
                    ItemName = @ItemName,
                    UnitPrice = @UnitPrice,
                    UnitName = @UnitName,
                    Description = @Description,
                    IsAvailable = @IsAvailable
                WHERE ItemID = @ItemID";

            using (var conn = DatabaseHelper.CreateAndOpenConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@ItemID", item.ItemID);
                cmd.Parameters.AddWithValue("@CategoryID", item.CategoryID);
                cmd.Parameters.AddWithValue("@ItemCode", (object)item.ItemCode ?? System.DBNull.Value);
                cmd.Parameters.AddWithValue("@ItemName", item.ItemName);
                cmd.Parameters.AddWithValue("@UnitPrice", item.UnitPrice);
                cmd.Parameters.AddWithValue("@UnitName", (object)item.UnitName ?? System.DBNull.Value);
                cmd.Parameters.AddWithValue("@Description", (object)item.Description ?? System.DBNull.Value);
                cmd.Parameters.AddWithValue("@IsAvailable", item.IsAvailable);
                cmd.ExecuteNonQuery();
            }
        }

        public void DeleteItem(int itemId)
        {
            const string sql = "DELETE FROM dbo.MenuItems WHERE ItemID = @ItemID";

            using (var conn = DatabaseHelper.CreateAndOpenConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@ItemID", itemId);
                cmd.ExecuteNonQuery();
            }
        }
        public void SetAvailability(int itemId, bool isAvailable)
        {
            const string sql = @"UPDATE dbo.MenuItems SET IsAvailable = @IsAvailable WHERE ItemID = @ItemID";
            using (var conn = DatabaseHelper.CreateAndOpenConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@ItemID", itemId);
                cmd.Parameters.AddWithValue("@IsAvailable", isAvailable);
                cmd.ExecuteNonQuery();
            }
        }
    }
}
