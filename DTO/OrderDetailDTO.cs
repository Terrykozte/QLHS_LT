namespace QLTN_LT.DTO
{
    public class OrderDetailDTO
    {
        public int OrderDetailID { get; set; }
        public int OrderID { get; set; }
        public int SeafoodID { get; set; }
        public string SeafoodName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice => Quantity * UnitPrice;
    }
}

