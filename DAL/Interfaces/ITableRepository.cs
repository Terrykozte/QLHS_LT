using System.Collections.Generic;
using QLTN_LT.DTO;

namespace QLTN_LT.DAL.Interfaces
{
    public interface ITableRepository
    {
                List<TableDTO> GetAll();
        TableDTO GetById(int id);
        void Insert(TableDTO table);
        void Update(TableDTO table);
        void Delete(int id);
        List<TableDTO> Search(string keyword);
    }
}

