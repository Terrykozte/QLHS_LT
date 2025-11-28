using System.Collections.Generic;
using QLTN_LT.DTO;

namespace QLTN_LT.DAL.Interfaces
{
    public interface ISupplierRepository
    {
        List<SupplierDTO> GetAll();
        SupplierDTO GetById(int id);
        void Insert(SupplierDTO supplier);
        void Update(SupplierDTO supplier);
        void Delete(int id);
        List<SupplierDTO> Search(string keyword);
    }
}
