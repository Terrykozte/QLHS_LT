using System;

namespace QLTN_LT.DTO
{
    public class CategoryDTO
    {
        public int CategoryID { get; set; }
        public string CategoryName { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string Status { get; set; }

        public CategoryDTO()
        {
            CreatedDate = DateTime.Now;
            Status = "Active";
        }

        public override string ToString()
        {
            return CategoryName;
        }
    }
}

