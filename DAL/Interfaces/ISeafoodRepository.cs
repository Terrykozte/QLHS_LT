using System.Collections.Generic;
using QLTN_LT.DTO;

namespace QLTN_LT.DAL.Interfaces
{
    public interface ISeafoodRepository
    {
        List<SeafoodDTO> GetAll();
        SeafoodDTO GetById(int id);
        void Insert(SeafoodDTO seafood);
        void Update(SeafoodDTO seafood);
        void Delete(int id);
        List<SeafoodDTO> Search(string keyword);
    }
}

