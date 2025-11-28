namespace QLTN_LT.DTO
{
    public class TopSellingItemDTO
    {
        public int ItemID { get; set; }
        public string ItemName { get; set; }
        public string CategoryName { get; set; }
        public decimal TotalQuantitySold { get; set; }
        public decimal TotalRevenue { get; set; }
    }
}

