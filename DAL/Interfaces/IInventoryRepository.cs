using System;
using System.Collections.Generic;
using QLTN_LT.DTO;

namespace QLTN_LT.DAL.Interfaces
{
    public interface IInventoryRepository
    {
        List<InventoryTransactionDTO> GetAll(DateTime fromDate, DateTime toDate, string type, string keyword);
        void Insert(InventoryTransactionDTO transaction);
    }
}
