using System;

namespace QLTN_LT.DTO
{
    public class BranchDTO
    {
        public int BranchID { get; set; }
        public string BranchCode { get; set; }
        public string BranchName { get; set; }
        public string Address { get; set; }
        public bool IsActive { get; set; }
    }
}

