using System;
using System.Collections.Generic;
using QLTN_LT.DTO;

namespace QLTN_LT.DAL.Interfaces
{
    public interface IInventoryRepository
    {
        List<InventoryStatusDTO> GetInventoryStatus();
        InventoryDTO GetBySeafoodId(int seafoodId);
        InventoryDTO GetByInventoryId(int inventoryId);
        int InsertTransaction(InventoryTransactionDTO transaction);
        List<InventoryTransactionDTO> GetTransactions(int inventoryId, int pageNumber = 1, int pageSize = 50);
        List<InventoryTransactionDTO> GetTransactionsByDateRange(DateTime fromDate, DateTime toDate);
        int GetTotalTransactionCount(int inventoryId);
    }
}
