using System;

namespace QLTN_LT.DTO
{
    public class Customer
    {
        public int CustomerID { get; set; }
        public string CustomerName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string District { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string Notes { get; set; }

        public CustomerDTO()
        {
            CreatedDate = DateTime.Now;
        }

        public override string ToString()
        {
            return $"{CustomerName} - {PhoneNumber}";
        }
    }
}

