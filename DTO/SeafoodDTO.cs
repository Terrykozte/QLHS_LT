using System;

namespace QLTN_LT.DTO
{
    public class SeafoodDTO
    {
        public int SeafoodID { get; set; }
        public string SeafoodName { get; set; }
        public int CategoryID { get; set; }
        public string CategoryName { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Price { get => UnitPrice; set => UnitPrice = value; }
        public int Quantity { get; set; }
        public string Unit { get; set; }
        public string Description { get; set; }
        public string ImagePath { get; set; }
        public string Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public SeafoodDTO()
        {
            CreatedDate = DateTime.Now;
            Status = "Active";
            Quantity = 0;
        }

        public override string ToString()
        {
            return $"{SeafoodName} - {UnitPrice:N0} (Tồn: {Quantity})";
        }
    }
}

