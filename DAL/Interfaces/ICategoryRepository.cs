using System.Collections.Generic;
using QLTN_LT.DTO;

namespace QLTN_LT.DAL.Interfaces
{
    public interface ICategoryRepository
    {
                List<CategoryDTO> GetAll();
        CategoryDTO GetById(int id);
        void Insert(CategoryDTO category);
        void Update(CategoryDTO category);
        void Delete(int id);
        List<CategoryDTO> Search(string keyword);
    }
}

