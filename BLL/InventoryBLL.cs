using System;
using System.Collections.Generic;
using System.Linq;
using QLTN_LT.DAL;
using QLTN_LT.DAL.Interfaces;
using QLTN_LT.DTO;

namespace QLTN_LT.BLL
{
    public class InventoryBLL
    {
        private readonly IInventoryRepository _repo;

        public InventoryBLL() : this(new InventoryDAL())
        {
        }

        public InventoryBLL(IInventoryRepository repo)
        {
            _repo = repo;
        }

        public List<InventoryTransactionDTO> GetTransactions(DateTime fromDate, DateTime toDate, string type, string keyword)
        {
            return _repo.GetAll(fromDate, toDate, type, keyword).OrderByDescending(t => t.TransactionDate).ToList();
        }

        public void AddTransaction(InventoryTransactionDTO transaction)
        {
            if (transaction.ItemID <= 0 || (transaction.QuantityIn <= 0 && transaction.QuantityOut <= 0))
            {
                throw new ArgumentException("Sản phẩm và số lượng phải hợp lệ.");
            }
            _repo.Insert(transaction);
        }
    }
}

