using System;

namespace QLTN_LT.DTO
{
    public class DailyRevenueDTO
    {
        public DateTime OrderDate { get; set; }
        public int OrderCount { get; set; }
        public decimal TotalRevenue { get; set; }
    }
}

