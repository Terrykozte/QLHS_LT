using System;

namespace QLTN_LT.DTO
{
    public class InventoryDTO
    {
        public int InventoryID { get; set; }
        public int SeafoodID { get; set; }
        public string SeafoodName { get; set; }
        public int Quantity { get; set; }
        public int ReorderLevel { get; set; }
        public string Status { get; set; } // Cần nhập, Thấp, Bình thường
        public DateTime LastUpdated { get; set; }
    }
}
