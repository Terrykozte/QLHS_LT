namespace QLTN_LT.DTO
{
    public class TableDTO
    {
        public int TableID { get; set; }
        public string TableName { get; set; }
        public int BranchID { get; set; }
        public string BranchName { get; set; }
        public int TableNumber { get; set; }
        public string QrData { get; set; }
        public int Capacity { get; set; }
        public string Status { get; set; }
        public bool IsActive { get; set; }
    }
}
