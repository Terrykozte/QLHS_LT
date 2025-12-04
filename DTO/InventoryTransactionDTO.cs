using System;

namespace QLTN_LT.DTO
{
    public class InventoryTransactionDTO
    {
        public int TransactionID { get; set; }
        public int InventoryID { get; set; }
        public int ItemID { get; set; }
        public string ItemName { get; set; }
        public string SeafoodName { get; set; }
        public int Quantity { get; set; }
        public int QuantityIn { get; set; }
        public int QuantityOut { get; set; }
        public int QuantityRemaining { get; set; }
        public DateTime TransactionDate { get; set; }
        public string TransactionType { get; set; } // 'In' or 'Out'
        public string Reason { get; set; }
        public int? SupplierID { get; set; }
        public string SupplierName { get; set; }
        public string Notes { get; set; }
        public string ProcessedBy { get; set; }
        public int? CreatedBy { get; set; }

        // Aliases for UI binding compatibility
        public string Type { get => TransactionType; set => TransactionType = value; }
        public string ProductName { get => !string.IsNullOrEmpty(SeafoodName) ? SeafoodName : ItemName; set => SeafoodName = value; }
        public string Note { get => !string.IsNullOrEmpty(Notes) ? Notes : Reason; set => Notes = value; }
}
}
