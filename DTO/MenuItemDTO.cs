using System;

namespace QLTN_LT.DTO
{
    public class MenuItemDTO
    {
        public int ItemID { get; set; }
        public int CategoryID { get; set; }
        public string CategoryName { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public decimal UnitPrice { get; set; }
        public string UnitName { get; set; }
        public string Description { get; set; }
        public bool IsAvailable { get; set; }
        public DateTime CreatedAt { get; set; }
        // Optional: đường dẫn ảnh nếu có trong dữ liệu; nếu không có sẽ fallback icon
        public string ImagePath { get; set; }

        // Thuộc tính hiển thị cho lưới
        public string Status => IsAvailable ? "Available" : "Unavailable";
    }
}

