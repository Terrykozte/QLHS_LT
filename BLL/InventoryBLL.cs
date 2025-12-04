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

        public List<InventoryStatusDTO> GetInventoryStatus()
        {
            return _repo.GetInventoryStatus();
        }

        public InventoryDTO GetBySeafoodId(int seafoodId)
        {
            if (seafoodId <= 0)
                throw new ArgumentException("SeafoodID phải lớn hơn 0");

            return _repo.GetBySeafoodId(seafoodId);
        }

        public InventoryDTO GetByInventoryId(int inventoryId)
        {
            if (inventoryId <= 0)
                throw new ArgumentException("InventoryID phải lớn hơn 0");

            return _repo.GetByInventoryId(inventoryId);
        }

        public int StockIn(int inventoryId, int quantity, int? supplierId = null, string reason = null, int? createdBy = null)
        {
            if (inventoryId <= 0)
                throw new ArgumentException("InventoryID phải lớn hơn 0");

            if (quantity <= 0)
                throw new ArgumentException("Số lượng nhập phải lớn hơn 0");

            var transaction = new InventoryTransactionDTO
            {
                InventoryID = inventoryId,
                TransactionType = "In",
                Quantity = quantity,
                Reason = reason ?? "Nhập hàng",
                SupplierID = supplierId,
                CreatedBy = createdBy
            };

            return _repo.InsertTransaction(transaction);
        }

        public int StockOut(int inventoryId, int quantity, string reason = null, int? createdBy = null)
        {
            if (inventoryId <= 0)
                throw new ArgumentException("InventoryID phải lớn hơn 0");

            if (quantity <= 0)
                throw new ArgumentException("Số lượng xuất phải lớn hơn 0");

            var transaction = new InventoryTransactionDTO
            {
                InventoryID = inventoryId,
                TransactionType = "Out",
                Quantity = quantity,
                Reason = reason ?? "Xuất hàng",
                CreatedBy = createdBy
            };

            return _repo.InsertTransaction(transaction);
        }

        public int AdjustInventory(int inventoryId, int newQuantity, string reason = null, int? createdBy = null)
        {
            if (inventoryId <= 0)
                throw new ArgumentException("InventoryID phải lớn hơn 0");

            if (newQuantity < 0)
                throw new ArgumentException("Số lượng không được âm");

            var transaction = new InventoryTransactionDTO
            {
                InventoryID = inventoryId,
                TransactionType = "Adjustment",
                Quantity = newQuantity,
                Reason = reason ?? "Điều chỉnh kho",
                CreatedBy = createdBy
            };

            return _repo.InsertTransaction(transaction);
        }

        public List<InventoryTransactionDTO> GetTransactions(int inventoryId, int pageNumber = 1, int pageSize = 50)
        {
            if (inventoryId <= 0)
                throw new ArgumentException("InventoryID phải lớn hơn 0");

            if (pageNumber < 1)
                pageNumber = 1;

            if (pageSize < 1 || pageSize > 500)
                pageSize = 50;

            return _repo.GetTransactions(inventoryId, pageNumber, pageSize);
        }

        public List<InventoryTransactionDTO> GetTransactionsByDateRange(DateTime fromDate, DateTime toDate)
        {
            if (fromDate > toDate)
                throw new ArgumentException("Ngày bắt đầu không được lớn hơn ngày kết thúc");

            return _repo.GetTransactionsByDateRange(fromDate, toDate);
        }

        public int GetTotalTransactionCount(int inventoryId)
        {
            if (inventoryId <= 0)
                throw new ArgumentException("InventoryID phải lớn hơn 0");

            return _repo.GetTotalTransactionCount(inventoryId);
        }

        public List<InventoryStatusDTO> GetLowStockItems()
        {
            var allItems = GetInventoryStatus();
            var lowStockItems = new List<InventoryStatusDTO>();

            foreach (var item in allItems)
            {
                if (item.Status == "Cần nhập" || item.Status == "Thấp")
                    lowStockItems.Add(item);
            }

            return lowStockItems;
        }
    }
}
