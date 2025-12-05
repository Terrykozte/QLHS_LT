using System;
using System.Collections.Generic;
using System.Linq;
using QLTN_LT.DTO;
using QLTN_LT.DAL;

namespace QLTN_LT.BLL
{
    /// <summary>
    /// Service quản lý kho toàn diện
    /// </summary>
    public class InventoryManagementService
    {
        private readonly InventoryDAL _inventoryDAL = new InventoryDAL();
        private readonly SeafoodDAL _seafoodDAL = new SeafoodDAL();

        /// <summary>
        /// Nhập kho
        /// </summary>
        public Result<bool> StockIn(int seafoodId, int quantity, int? supplierId = null, string reason = null, int? createdBy = null)
        {
            try
            {
                // Kiểm tra sản phẩm
                var seafood = _seafoodDAL.GetSeafoodById(seafoodId);
                if (seafood == null)
                    return new Result<bool> { Success = false, Message = "Sản phẩm không tồn tại" };

                // Kiểm tra số lượng
                if (quantity <= 0)
                    return new Result<bool> { Success = false, Message = "Số lượng phải lớn hơn 0" };

                // Tạo ghi nhận giao dịch
                var transaction = new InventoryTransactionDTO
                {
                    SeafoodID = seafoodId,
                    TransactionType = "In",
                    Quantity = quantity,
                    Reason = reason ?? "Nhập kho",
                    SupplierID = supplierId,
                    TransactionDate = DateTime.Now,
                    CreatedBy = createdBy
                };

                var result = _inventoryDAL.AddInventoryTransaction(transaction);
                
                if (result.Success)
                {
                    // Cập nhật tồn kho
                    UpdateInventoryQuantity(seafoodId);
                }

                return result;
            }
            catch (Exception ex)
            {
                return new Result<bool> { Success = false, Message = $"Lỗi nhập kho: {ex.Message}" };
            }
        }

        /// <summary>
        /// Xuất kho
        /// </summary>
        public Result<bool> StockOut(int seafoodId, int quantity, string reason = null, int? createdBy = null)
        {
            try
            {
                // Kiểm tra sản phẩm
                var seafood = _seafoodDAL.GetSeafoodById(seafoodId);
                if (seafood == null)
                    return new Result<bool> { Success = false, Message = "Sản phẩm không tồn tại" };

                // Kiểm tra số lượng
                if (quantity <= 0)
                    return new Result<bool> { Success = false, Message = "Số lượng phải lớn hơn 0" };

                // Kiểm tra tồn kho
                var inventory = _inventoryDAL.GetInventoryBySeafoodId(seafoodId);
                if (inventory == null || inventory.Quantity < quantity)
                    return new Result<bool> { Success = false, Message = "Tồn kho không đủ" };

                // Tạo ghi nhận giao dịch
                var transaction = new InventoryTransactionDTO
                {
                    SeafoodID = seafoodId,
                    TransactionType = "Out",
                    Quantity = quantity,
                    Reason = reason ?? "Xuất kho",
                    TransactionDate = DateTime.Now,
                    CreatedBy = createdBy
                };

                var result = _inventoryDAL.AddInventoryTransaction(transaction);
                
                if (result.Success)
                {
                    // Cập nhật tồn kho
                    UpdateInventoryQuantity(seafoodId);
                }

                return result;
            }
            catch (Exception ex)
            {
                return new Result<bool> { Success = false, Message = $"Lỗi xuất kho: {ex.Message}" };
            }
        }

        /// <summary>
        /// Điều chỉnh kho
        /// </summary>
        public Result<bool> AdjustInventory(int seafoodId, int newQuantity, string reason = null, int? createdBy = null)
        {
            try
            {
                // Kiểm tra sản phẩm
                var seafood = _seafoodDAL.GetSeafoodById(seafoodId);
                if (seafood == null)
                    return new Result<bool> { Success = false, Message = "Sản phẩm không tồn tại" };

                // Kiểm tra số lượng
                if (newQuantity < 0)
                    return new Result<bool> { Success = false, Message = "Số lượng không thể âm" };

                // Tạo ghi nhận giao dịch
                var transaction = new InventoryTransactionDTO
                {
                    SeafoodID = seafoodId,
                    TransactionType = "Adjustment",
                    Quantity = newQuantity,
                    Reason = reason ?? "Điều chỉnh kho",
                    TransactionDate = DateTime.Now,
                    CreatedBy = createdBy
                };

                var result = _inventoryDAL.AddInventoryTransaction(transaction);
                
                if (result.Success)
                {
                    // Cập nhật tồn kho
                    var inventory = _inventoryDAL.GetInventoryBySeafoodId(seafoodId);
                    if (inventory != null)
                    {
                        inventory.Quantity = newQuantity;
                        inventory.LastUpdated = DateTime.Now;
                        _inventoryDAL.UpdateInventory(inventory);
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                return new Result<bool> { Success = false, Message = $"Lỗi điều chỉnh kho: {ex.Message}" };
            }
        }

        /// <summary>
        /// Cập nhật tồn kho từ giao dịch
        /// </summary>
        private void UpdateInventoryQuantity(int seafoodId)
        {
            try
            {
                var inventory = _inventoryDAL.GetInventoryBySeafoodId(seafoodId);
                if (inventory == null) return;

                // Lấy tất cả giao dịch
                var transactions = _inventoryDAL.GetTransactionsBySeafoodId(seafoodId);
                if (transactions.Data == null || transactions.Data.Count == 0) return;

                // Tính tổng
                int totalQuantity = 0;
                foreach (var trans in transactions.Data)
                {
                    if (trans.TransactionType == "In")
                        totalQuantity += trans.Quantity;
                    else if (trans.TransactionType == "Out")
                        totalQuantity -= trans.Quantity;
                    else if (trans.TransactionType == "Adjustment")
                        totalQuantity = trans.Quantity;
                }

                inventory.Quantity = Math.Max(0, totalQuantity);
                inventory.LastUpdated = DateTime.Now;
                _inventoryDAL.UpdateInventory(inventory);
            }
            catch { }
        }

        /// <summary>
        /// Lấy trạng thái kho
        /// </summary>
        public Result<InventoryStatusDTO> GetInventoryStatus(int seafoodId)
        {
            try
            {
                var inventory = _inventoryDAL.GetInventoryBySeafoodId(seafoodId);
                if (inventory == null)
                    return new Result<InventoryStatusDTO> { Success = false, Message = "Sản phẩm không tồn tại" };

                var status = new InventoryStatusDTO
                {
                    SeafoodID = seafoodId,
                    Quantity = inventory.Quantity,
                    ReorderLevel = inventory.ReorderLevel,
                    Status = GetInventoryStatusString(inventory.Quantity, inventory.ReorderLevel)
                };

                return new Result<InventoryStatusDTO> { Success = true, Data = status };
            }
            catch (Exception ex)
            {
                return new Result<InventoryStatusDTO> { Success = false, Message = $"Lỗi lấy trạng thái kho: {ex.Message}" };
            }
        }

        /// <summary>
        /// Lấy danh sách sản phẩm cần nhập
        /// </summary>
        public Result<List<InventoryStatusDTO>> GetLowStockItems()
        {
            try
            {
                var result = _inventoryDAL.GetLowStockItems();
                return result;
            }
            catch (Exception ex)
            {
                return new Result<List<InventoryStatusDTO>> { Success = false, Message = $"Lỗi lấy danh sách: {ex.Message}" };
            }
        }

        /// <summary>
        /// Lấy danh sách hết hàng
        /// </summary>
        public Result<List<InventoryStatusDTO>> GetOutOfStockItems()
        {
            try
            {
                var result = _inventoryDAL.GetOutOfStockItems();
                return result;
            }
            catch (Exception ex)
            {
                return new Result<List<InventoryStatusDTO>> { Success = false, Message = $"Lỗi lấy danh sách: {ex.Message}" };
            }
        }

        /// <summary>
        /// Lấy lịch sử giao dịch
        /// </summary>
        public Result<List<InventoryTransactionDTO>> GetTransactionHistory(int seafoodId, DateTime? fromDate = null, DateTime? toDate = null)
        {
            try
            {
                var result = _inventoryDAL.GetTransactionHistory(seafoodId, fromDate, toDate);
                return result;
            }
            catch (Exception ex)
            {
                return new Result<List<InventoryTransactionDTO>> { Success = false, Message = $"Lỗi lấy lịch sử: {ex.Message}" };
            }
        }

        /// <summary>
        /// Lấy báo cáo kho
        /// </summary>
        public Result<List<InventoryDTO>> GetInventoryReport()
        {
            try
            {
                var result = _inventoryDAL.GetAllInventory();
                return result;
            }
            catch (Exception ex)
            {
                return new Result<List<InventoryDTO>> { Success = false, Message = $"Lỗi lấy báo cáo: {ex.Message}" };
            }
        }

        /// <summary>
        /// Tính giá trị kho
        /// </summary>
        public Result<decimal> CalculateInventoryValue()
        {
            try
            {
                var inventories = _inventoryDAL.GetAllInventory();
                if (!inventories.Success || inventories.Data == null)
                    return new Result<decimal> { Success = false, Message = "Lỗi lấy dữ liệu kho" };

                decimal totalValue = 0;
                foreach (var inv in inventories.Data)
                {
                    var seafood = _seafoodDAL.GetSeafoodById(inv.SeafoodID);
                    if (seafood != null)
                        totalValue += inv.Quantity * seafood.UnitPrice;
                }

                return new Result<decimal> { Success = true, Data = totalValue };
            }
            catch (Exception ex)
            {
                return new Result<decimal> { Success = false, Message = $"Lỗi tính giá trị kho: {ex.Message}" };
            }
        }

        /// <summary>
        /// Lấy thống kê kho
        /// </summary>
        public Result<Dictionary<string, object>> GetInventoryStatistics()
        {
            try
            {
                var stats = new Dictionary<string, object>();

                var inventories = _inventoryDAL.GetAllInventory();
                if (!inventories.Success || inventories.Data == null)
                    return new Result<Dictionary<string, object>> { Success = false, Message = "Lỗi lấy dữ liệu" };

                // Tổng số sản phẩm
                stats["TotalItems"] = inventories.Data.Count;

                // Tổng tồn kho
                stats["TotalQuantity"] = inventories.Data.Sum(x => x.Quantity);

                // Sản phẩm hết hàng
                stats["OutOfStock"] = inventories.Data.Count(x => x.Quantity == 0);

                // Sản phẩm cần nhập
                stats["LowStock"] = inventories.Data.Count(x => x.Quantity > 0 && x.Quantity <= x.ReorderLevel);

                // Giá trị kho
                var valueResult = CalculateInventoryValue();
                stats["InventoryValue"] = valueResult.Data;

                return new Result<Dictionary<string, object>> { Success = true, Data = stats };
            }
            catch (Exception ex)
            {
                return new Result<Dictionary<string, object>> { Success = false, Message = $"Lỗi lấy thống kê: {ex.Message}" };
            }
        }

        /// <summary>
        /// Đặt mức cảnh báo kho
        /// </summary>
        public Result<bool> SetReorderLevel(int seafoodId, int reorderLevel)
        {
            try
            {
                if (reorderLevel < 0)
                    return new Result<bool> { Success = false, Message = "Mức cảnh báo không thể âm" };

                var inventory = _inventoryDAL.GetInventoryBySeafoodId(seafoodId);
                if (inventory == null)
                    return new Result<bool> { Success = false, Message = "Sản phẩm không tồn tại" };

                inventory.ReorderLevel = reorderLevel;
                var result = _inventoryDAL.UpdateInventory(inventory);
                return result;
            }
            catch (Exception ex)
            {
                return new Result<bool> { Success = false, Message = $"Lỗi đặt mức cảnh báo: {ex.Message}" };
            }
        }

        /// <summary>
        /// Lấy chuỗi trạng thái kho
        /// </summary>
        private string GetInventoryStatusString(int quantity, int reorderLevel)
        {
            if (quantity == 0) return "Hết hàng";
            if (quantity <= reorderLevel) return "Cần nhập";
            if (quantity <= reorderLevel * 2) return "Thấp";
            return "Bình thường";
        }

        /// <summary>
        /// Kiểm tra xem sản phẩm có đủ tồn kho không
        /// </summary>
        public Result<bool> HasSufficientStock(int seafoodId, int requiredQuantity)
        {
            try
            {
                var inventory = _inventoryDAL.GetInventoryBySeafoodId(seafoodId);
                if (inventory == null)
                    return new Result<bool> { Success = false, Message = "Sản phẩm không tồn tại" };

                bool hasSufficientStock = inventory.Quantity >= requiredQuantity;
                return new Result<bool> { Success = true, Data = hasSufficientStock };
            }
            catch (Exception ex)
            {
                return new Result<bool> { Success = false, Message = $"Lỗi kiểm tra tồn kho: {ex.Message}" };
            }
        }

        /// <summary>
        /// Lấy danh sách sản phẩm sắp hết hàng
        /// </summary>
        public Result<List<InventoryStatusDTO>> GetExpiringItems(int daysThreshold = 7)
        {
            try
            {
                // Lấy sản phẩm có tồn kho thấp
                var lowStockResult = GetLowStockItems();
                if (!lowStockResult.Success)
                    return new Result<List<InventoryStatusDTO>> { Success = false, Message = lowStockResult.Message };

                return lowStockResult;
            }
            catch (Exception ex)
            {
                return new Result<List<InventoryStatusDTO>> { Success = false, Message = $"Lỗi lấy danh sách: {ex.Message}" };
            }
        }
    }
}

