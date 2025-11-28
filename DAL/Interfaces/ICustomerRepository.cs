using System.Collections.Generic;
using QLTN_LT.DTO;

namespace QLTN_LT.DAL.Interfaces
{
    public interface ICustomerRepository
    {
        List<CustomerDTO> GetAll();
        CustomerDTO GetById(int id);
        void Insert(CustomerDTO customer);
        void Update(CustomerDTO customer);
        void Delete(int id);
        List<CustomerDTO> Search(string keyword);
    }
}

