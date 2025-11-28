using System;
using System.Collections.Generic;

namespace QLTN_LT.DTO
{
    public class OrderDTO
    {
        public int OrderID { get; set; }
        public string OrderNumber { get; set; }
        public int? CustomerID { get; set; }
        public string CustomerName { get; set; }
        public int? TableID { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
        public List<OrderDetailDTO> OrderDetails { get; set; }
    }
}
