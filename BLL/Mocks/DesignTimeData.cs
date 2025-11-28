using System; 
using System.Collections.Generic; 
using QLTN_LT.DTO; 

namespace QLTN_LT.BLL.Mocks
{
    public static class DesignTimeData
    {
        // Bật chế độ mock để phát triển UI trước khi kết nối DB
                public static bool UseMock = false;

        public static List<CategoryDTO> Categories()
        {
            return new List<CategoryDTO>
            {
                new CategoryDTO { CategoryID = 1, CategoryName = "Cá", Description = "Các loại cá tươi" },
                new CategoryDTO { CategoryID = 2, CategoryName = "Tôm", Description = "Các loại tôm tươi" },
                new CategoryDTO { CategoryID = 3, CategoryName = "Cua", Description = "Các loại cua tươi" },
                new CategoryDTO { CategoryID = 4, CategoryName = "Mực", Description = "Các loại mực tươi" },
                new CategoryDTO { CategoryID = 5, CategoryName = "Sò", Description = "Các loại sò tươi" }
            };
        }

        public static List<SeafoodDTO> Seafoods()
        {
            return new List<SeafoodDTO>
            {
                new SeafoodDTO { SeafoodID = 1, SeafoodName = "Cá Basa", CategoryID = 1, CategoryName = "Cá", UnitPrice = 85000, Quantity = 50, Status = "Active" },
                new SeafoodDTO { SeafoodID = 2, SeafoodName = "Cá Tra", CategoryID = 1, CategoryName = "Cá", UnitPrice = 75000, Quantity = 40, Status = "Active" },
                new SeafoodDTO { SeafoodID = 3, SeafoodName = "Tôm Sú", CategoryID = 2, CategoryName = "Tôm", UnitPrice = 250000, Quantity = 30, Status = "Active" },
                new SeafoodDTO { SeafoodID = 4, SeafoodName = "Tôm Thẻ", CategoryID = 2, CategoryName = "Tôm", UnitPrice = 180000, Quantity = 45, Status = "Active" },
                new SeafoodDTO { SeafoodID = 5, SeafoodName = "Cua Hoàng Đế", CategoryID = 3, CategoryName = "Cua", UnitPrice = 350000, Quantity = 20, Status = "Active" },
                new SeafoodDTO { SeafoodID = 6, SeafoodName = "Mực Ống", CategoryID = 4, CategoryName = "Mực", UnitPrice = 120000, Quantity = 35, Status = "Active" }
            };
        }

        public static List<CustomerDTO> Customers()
        {
            return new List<CustomerDTO>
            {
                new CustomerDTO { CustomerID = 1, CustomerName = "Nguyễn Văn A", PhoneNumber = "0901234567", Email = "a@example.com", City = "TP.HCM" },
                new CustomerDTO { CustomerID = 2, CustomerName = "Trần Thị B", PhoneNumber = "0912345678", Email = "b@example.com", City = "TP.HCM" },
                new CustomerDTO { CustomerID = 3, CustomerName = "Phạm Văn C", PhoneNumber = "0923456789", Email = "c@example.com", City = "TP.HCM" }
            };
        }
    }
}
