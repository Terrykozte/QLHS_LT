using System.Collections.Generic;
using System.Data.SqlClient;
using QLTN_LT.DTO;

namespace QLTN_LT.DAL
{
    public class MenuDAL
    {
        public List<MenuItemDTO> GetAllItems()
        {
            var list = new List<MenuItemDTO>();
            const string sql = @"
                SELECT mi.ItemID, mi.CategoryID, mc.CategoryName, mi.ItemCode, mi.ItemName, 
                       mi.UnitPrice, mi.UnitName, mi.Description, mi.IsAvailable, mi.CreatedAt
                FROM hs.MenuItems mi
                JOIN hs.MenuCategories mc ON mi.CategoryID = mc.CategoryID
                ORDER BY mc.DisplayOrder, mi.ItemName";

            using (var conn = DatabaseHelper.CreateConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                conn.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        list.Add(new MenuItemDTO
                        {
                            ItemID = (int)rd["ItemID"],
                            CategoryID = (int)rd["CategoryID"],
                            CategoryName = rd["CategoryName"].ToString(),
                            ItemCode = rd["ItemCode"].ToString(),
                            ItemName = rd["ItemName"].ToString(),
                            UnitPrice = (decimal)rd["UnitPrice"],
                            UnitName = rd["UnitName"].ToString(),
                            Description = rd["Description"].ToString(),
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
                FROM hs.MenuItems mi
                JOIN hs.MenuCategories mc ON mi.CategoryID = mc.CategoryID
                WHERE mi.ItemID = @ItemID";

            using (var conn = DatabaseHelper.CreateConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@ItemID", itemId);
                conn.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    if (rd.Read())
                    {
                        item = new MenuItemDTO
                        {
                            ItemID = (int)rd["ItemID"],
                            CategoryID = (int)rd["CategoryID"],
                            CategoryName = rd["CategoryName"].ToString(),
                            ItemCode = rd["ItemCode"].ToString(),
                            ItemName = rd["ItemName"].ToString(),
                            UnitPrice = (decimal)rd["UnitPrice"],
                            UnitName = rd["UnitName"].ToString(),
                            Description = rd["Description"].ToString(),
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
                INSERT INTO hs.MenuItems (CategoryID, ItemCode, ItemName, UnitPrice, UnitName, Description, IsAvailable, CreatedAt)
                VALUES (@CategoryID, @ItemCode, @ItemName, @UnitPrice, @UnitName, @Description, @IsAvailable, GETDATE())";

            using (var conn = DatabaseHelper.CreateConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@CategoryID", item.CategoryID);
                cmd.Parameters.AddWithValue("@ItemCode", (object)item.ItemCode ?? string.Empty);
                cmd.Parameters.AddWithValue("@ItemName", item.ItemName);
                cmd.Parameters.AddWithValue("@UnitPrice", item.UnitPrice);
                cmd.Parameters.AddWithValue("@UnitName", (object)item.UnitName ?? string.Empty);
                cmd.Parameters.AddWithValue("@Description", (object)item.Description ?? string.Empty);
                cmd.Parameters.AddWithValue("@IsAvailable", item.IsAvailable);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void UpdateItem(MenuItemDTO item)
        {
            const string sql = @"
                UPDATE hs.MenuItems
                SET CategoryID = @CategoryID,
                    ItemCode = @ItemCode,
                    ItemName = @ItemName,
                    UnitPrice = @UnitPrice,
                    UnitName = @UnitName,
                    Description = @Description,
                    IsAvailable = @IsAvailable
                WHERE ItemID = @ItemID";

            using (var conn = DatabaseHelper.CreateConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@ItemID", item.ItemID);
                cmd.Parameters.AddWithValue("@CategoryID", item.CategoryID);
                cmd.Parameters.AddWithValue("@ItemCode", (object)item.ItemCode ?? string.Empty);
                cmd.Parameters.AddWithValue("@ItemName", item.ItemName);
                cmd.Parameters.AddWithValue("@UnitPrice", item.UnitPrice);
                cmd.Parameters.AddWithValue("@UnitName", (object)item.UnitName ?? string.Empty);
                cmd.Parameters.AddWithValue("@Description", (object)item.Description ?? string.Empty);
                cmd.Parameters.AddWithValue("@IsAvailable", item.IsAvailable);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void DeleteItem(int itemId)
        {
            const string sql = "DELETE FROM hs.MenuItems WHERE ItemID = @ItemID";

            using (var conn = DatabaseHelper.CreateConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@ItemID", itemId);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}
