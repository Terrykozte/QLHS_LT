using System;

namespace QLTN_LT.DTO
{
    public class InventoryTransactionDTO
    {
        public int InventoryID { get; set; }
        public int ItemID { get; set; }
        public string ItemName { get; set; }
        public int QuantityIn { get; set; }
        public int QuantityOut { get; set; }
        public int QuantityRemaining { get; set; }
        public DateTime TransactionDate { get; set; }
        public string TransactionType { get; set; } // 'In' or 'Out'
        public string Notes { get; set; }
        public string ProcessedBy { get; set; }
    }
}

