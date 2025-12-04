using System;

namespace QLTN_LT.DTO
{
    public class InventoryStatusDTO
    {
        public int InventoryID { get; set; }
        public int SeafoodID { get; set; }
        public string SeafoodName { get; set; }
        public int Quantity { get; set; }
        public int ReorderLevel { get; set; }
        public string Status { get; set; }
        public DateTime LastUpdated { get; set; }
        public int ItemID { get; set; }
        public string ItemName { get; set; }
        public int QuantityRemaining { get; set; }
    }
}
