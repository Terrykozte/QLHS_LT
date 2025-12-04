using System;

namespace QLTN_LT.DTO
{
    public class PaymentDTO
    {
        public int PaymentID { get; set; }
        public int OrderID { get; set; }
        public int PaymentMethodID { get; set; }
        public string MethodName { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public string TransactionCode { get; set; }
        public string QRCode { get; set; }
        public string BankAccount { get; set; }
        public string BankName { get; set; }
        public string Status { get; set; } // Pending, Completed, Failed, Cancelled
        public string Notes { get; set; }
    }
}

