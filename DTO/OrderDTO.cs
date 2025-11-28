using System;

namespace QLTN_LT.DTO
{
    public class Order
    {
        public int OrderID { get; set; }
        public string OrderNumber { get; set; }
        public int CustomerID { get; set; }
        public string CustomerName { get; set; }
        public int TableID { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
    }
}
