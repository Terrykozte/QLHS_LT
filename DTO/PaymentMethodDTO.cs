using System;

namespace QLTN_LT.DTO
{
    public class PaymentMethodDTO
    {
        public int PaymentMethodID { get; set; }
        public string MethodName { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
    }
}

