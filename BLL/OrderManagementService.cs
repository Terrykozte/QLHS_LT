using System;
using System.Collections.Generic;
using System.Linq;
using QLTN_LT.DTO;
using QLTN_LT.DAL;

namespace QLTN_LT.BLL
{
    /// <summary>
    /// Service quản lý đơn hàng toàn diện
    /// </summary>
    public class OrderManagementService
    {
        private readonly OrderDAL _orderDAL = new OrderDAL();
        private readonly MenuDAL _menuDAL = new MenuDAL();
        private readonly PaymentDAL _paymentDAL = new PaymentDAL();
        private readonly TableDAL _tableDAL = new TableDAL();

        /// <summary>
        /// Tạo đơn hàng mới
        /// </summary>
        public Result<OrderDTO> CreateOrder(int branchId, int? tableId, int? createdBy, string note = null)
        {
            try
            {
                var order = new OrderDTO
                {
                    BranchID = branchId,
                    TableID = tableId,
                    OrderNumber = GenerateOrderNumber(),
                    OrderStatus = "Pending",
                    TotalAmount = 0,
                    Note = note,
                    CreatedBy = createdBy,
                    CreatedAt = DateTime.Now,
                    IsReservation = false
                };

                var result = _orderDAL.AddOrder(order);
                return result;
            }
            catch (Exception ex)
            {
                return new Result<OrderDTO> { Success = false, Message = $"Lỗi tạo đơn hàng: {ex.Message}" };
            }
        }

        /// <summary>
        /// Thêm item vào đơn hàng
        /// </summary>
        public Result<bool> AddItemToOrder(long orderId, int itemId, decimal quantity, decimal unitPrice)
        {
            try
            {
                var orderItem = new OrderDetailDTO
                {
                    OrderID = orderId,
                    ItemID = itemId,
                    Quantity = quantity,
                    UnitPrice = unitPrice
                };

                // Lấy tên item
                var menuItem = _menuDAL.GetMenuItemById(itemId);
                if (menuItem != null)
                    orderItem.ItemName = menuItem.ItemName;

                var result = _orderDAL.AddOrderItem(orderItem);
                
                if (result.Success)
                {
                    // Cập nhật tổng tiền
                    UpdateOrderTotal(orderId);
                }

                return result;
            }
            catch (Exception ex)
            {
                return new Result<bool> { Success = false, Message = $"Lỗi thêm item: {ex.Message}" };
            }
        }

        /// <summary>
        /// Cập nhật số lượng item
        /// </summary>
        public Result<bool> UpdateItemQuantity(long orderItemId, decimal newQuantity)
        {
            try
            {
                var result = _orderDAL.UpdateOrderItemQuantity(orderItemId, newQuantity);
                
                if (result.Success)
                {
                    // Lấy order ID và cập nhật tổng tiền
                    var orderItem = _orderDAL.GetOrderItemById(orderItemId);
                    if (orderItem != null)
                        UpdateOrderTotal(orderItem.OrderID);
                }

                return result;
            }
            catch (Exception ex)
            {
                return new Result<bool> { Success = false, Message = $"Lỗi cập nhật số lượng: {ex.Message}" };
            }
        }

        /// <summary>
        /// Xóa item khỏi đơn hàng
        /// </summary>
        public Result<bool> RemoveItemFromOrder(long orderItemId)
        {
            try
            {
                var orderItem = _orderDAL.GetOrderItemById(orderItemId);
                if (orderItem == null)
                    return new Result<bool> { Success = false, Message = "Item không tồn tại" };

                var result = _orderDAL.DeleteOrderItem(orderItemId);
                
                if (result.Success)
                {
                    // Cập nhật tổng tiền
                    UpdateOrderTotal(orderItem.OrderID);
                }

                return result;
            }
            catch (Exception ex)
            {
                return new Result<bool> { Success = false, Message = $"Lỗi xóa item: {ex.Message}" };
            }
        }

        /// <summary>
        /// Cập nhật tổng tiền đơn hàng
        /// </summary>
        public Result<bool> UpdateOrderTotal(long orderId)
        {
            try
            {
                var items = _orderDAL.GetOrderItems(orderId);
                decimal totalAmount = items.Sum(x => x.Quantity * x.UnitPrice);

                var result = _orderDAL.UpdateOrderTotal(orderId, totalAmount);
                return result;
            }
            catch (Exception ex)
            {
                return new Result<bool> { Success = false, Message = $"Lỗi cập nhật tổng tiền: {ex.Message}" };
            }
        }

        /// <summary>
        /// Cập nhật trạng thái đơn hàng
        /// </summary>
        public Result<bool> UpdateOrderStatus(long orderId, string newStatus)
        {
            try
            {
                // Kiểm tra trạng thái hợp lệ
                var validStatuses = new[] { "Pending", "Processing", "Completed", "Cancelled", "Reserved" };
                if (!validStatuses.Contains(newStatus))
                    return new Result<bool> { Success = false, Message = "Trạng thái không hợp lệ" };

                var result = _orderDAL.UpdateOrderStatus(orderId, newStatus);
                return result;
            }
            catch (Exception ex)
            {
                return new Result<bool> { Success = false, Message = $"Lỗi cập nhật trạng thái: {ex.Message}" };
            }
        }

        /// <summary>
        /// Hoàn tất đơn hàng
        /// </summary>
        public Result<bool> CompleteOrder(long orderId)
        {
            try
            {
                var order = _orderDAL.GetOrderById(orderId);
                if (order == null)
                    return new Result<bool> { Success = false, Message = "Đơn hàng không tồn tại" };

                // Cập nhật trạng thái
                var result = _orderDAL.UpdateOrderStatus(orderId, "Completed");
                
                if (result.Success)
                {
                    // Cập nhật thời gian hoàn tất
                    _orderDAL.UpdateOrderCompletedTime(orderId, DateTime.Now);
                }

                return result;
            }
            catch (Exception ex)
            {
                return new Result<bool> { Success = false, Message = $"Lỗi hoàn tất đơn hàng: {ex.Message}" };
            }
        }

        /// <summary>
        /// Hủy đơn hàng
        /// </summary>
        public Result<bool> CancelOrder(long orderId, string reason = null)
        {
            try
            {
                var order = _orderDAL.GetOrderById(orderId);
                if (order == null)
                    return new Result<bool> { Success = false, Message = "Đơn hàng không tồn tại" };

                if (order.OrderStatus == "Completed")
                    return new Result<bool> { Success = false, Message = "Không thể hủy đơn hàng đã hoàn tất" };

                var result = _orderDAL.UpdateOrderStatus(orderId, "Cancelled");
                return result;
            }
            catch (Exception ex)
            {
                return new Result<bool> { Success = false, Message = $"Lỗi hủy đơn hàng: {ex.Message}" };
            }
        }

        /// <summary>
        /// Lấy danh sách đơn hàng theo trạng thái
        /// </summary>
        public Result<List<OrderDTO>> GetOrdersByStatus(string status, DateTime? fromDate = null, DateTime? toDate = null)
        {
            try
            {
                var result = _orderDAL.GetOrdersByStatus(status, fromDate, toDate);
                return result;
            }
            catch (Exception ex)
            {
                return new Result<List<OrderDTO>> { Success = false, Message = $"Lỗi lấy danh sách đơn hàng: {ex.Message}" };
            }
        }

        /// <summary>
        /// Lấy danh sách đơn hàng theo bàn
        /// </summary>
        public Result<List<OrderDTO>> GetOrdersByTable(int tableId)
        {
            try
            {
                var result = _orderDAL.GetOrdersByTable(tableId);
                return result;
            }
            catch (Exception ex)
            {
                return new Result<List<OrderDTO>> { Success = false, Message = $"Lỗi lấy danh sách đơn hàng: {ex.Message}" };
            }
        }

        /// <summary>
        /// Lấy danh sách đơn hàng theo chi nhánh
        /// </summary>
        public Result<List<OrderDTO>> GetOrdersByBranch(int branchId, DateTime? fromDate = null, DateTime? toDate = null)
        {
            try
            {
                var result = _orderDAL.GetOrdersByBranch(branchId, fromDate, toDate);
                return result;
            }
            catch (Exception ex)
            {
                return new Result<List<OrderDTO>> { Success = false, Message = $"Lỗi lấy danh sách đơn hàng: {ex.Message}" };
            }
        }

        /// <summary>
        /// Tìm kiếm đơn hàng
        /// </summary>
        public Result<List<OrderDTO>> SearchOrders(string keyword, DateTime? fromDate = null, DateTime? toDate = null)
        {
            try
            {
                var result = _orderDAL.SearchOrders(keyword, fromDate, toDate);
                return result;
            }
            catch (Exception ex)
            {
                return new Result<List<OrderDTO>> { Success = false, Message = $"Lỗi tìm kiếm đơn hàng: {ex.Message}" };
            }
        }

        /// <summary>
        /// Lấy chi tiết đơn hàng
        /// </summary>
        public Result<OrderDTO> GetOrderDetails(long orderId)
        {
            try
            {
                var order = _orderDAL.GetOrderById(orderId);
                if (order == null)
                    return new Result<OrderDTO> { Success = false, Message = "Đơn hàng không tồn tại" };

                // Lấy items
                order.Items = _orderDAL.GetOrderItems(orderId);

                // Lấy payments
                order.Payments = _paymentDAL.GetPaymentsByOrderId(orderId);

                return new Result<OrderDTO> { Success = true, Data = order };
            }
            catch (Exception ex)
            {
                return new Result<OrderDTO> { Success = false, Message = $"Lỗi lấy chi tiết đơn hàng: {ex.Message}" };
            }
        }

        /// <summary>
        /// Tạo đơn hàng từ menu
        /// </summary>
        public Result<OrderDTO> CreateOrderFromMenu(int branchId, int? tableId, List<MenuItemDTO> items, int? createdBy, string note = null)
        {
            try
            {
                // Tạo đơn hàng
                var createResult = CreateOrder(branchId, tableId, createdBy, note);
                if (!createResult.Success)
                    return createResult;

                var order = createResult.Data;

                // Thêm items
                foreach (var item in items)
                {
                    AddItemToOrder(order.OrderID, item.ItemID, item.Quantity ?? 1, item.UnitPrice);
                }

                // Cập nhật tổng tiền
                UpdateOrderTotal(order.OrderID);

                return new Result<OrderDTO> { Success = true, Data = order };
            }
            catch (Exception ex)
            {
                return new Result<OrderDTO> { Success = false, Message = $"Lỗi tạo đơn hàng từ menu: {ex.Message}" };
            }
        }

        /// <summary>
        /// Tính toán doanh thu
        /// </summary>
        public Result<decimal> CalculateRevenue(DateTime fromDate, DateTime toDate)
        {
            try
            {
                var result = _orderDAL.CalculateRevenue(fromDate, toDate);
                return result;
            }
            catch (Exception ex)
            {
                return new Result<decimal> { Success = false, Message = $"Lỗi tính toán doanh thu: {ex.Message}" };
            }
        }

        /// <summary>
        /// Lấy thống kê đơn hàng
        /// </summary>
        public Result<Dictionary<string, int>> GetOrderStatistics(DateTime fromDate, DateTime toDate)
        {
            try
            {
                var stats = new Dictionary<string, int>();
                
                var pending = _orderDAL.GetOrdersByStatus("Pending", fromDate, toDate);
                var processing = _orderDAL.GetOrdersByStatus("Processing", fromDate, toDate);
                var completed = _orderDAL.GetOrdersByStatus("Completed", fromDate, toDate);
                var cancelled = _orderDAL.GetOrdersByStatus("Cancelled", fromDate, toDate);

                stats["Pending"] = pending.Data?.Count ?? 0;
                stats["Processing"] = processing.Data?.Count ?? 0;
                stats["Completed"] = completed.Data?.Count ?? 0;
                stats["Cancelled"] = cancelled.Data?.Count ?? 0;

                return new Result<Dictionary<string, int>> { Success = true, Data = stats };
            }
            catch (Exception ex)
            {
                return new Result<Dictionary<string, int>> { Success = false, Message = $"Lỗi lấy thống kê: {ex.Message}" };
            }
        }

        /// <summary>
        /// Tạo số đơn hàng duy nhất
        /// </summary>
        private string GenerateOrderNumber()
        {
            return "ORD" + DateTime.Now.ToString("yyyyMMddHHmmss");
        }

        /// <summary>
        /// Kiểm tra xem đơn hàng có thể thanh toán không
        /// </summary>
        public Result<bool> CanPayOrder(long orderId)
        {
            try
            {
                var order = _orderDAL.GetOrderById(orderId);
                if (order == null)
                    return new Result<bool> { Success = false, Message = "Đơn hàng không tồn tại" };

                if (order.OrderStatus == "Cancelled")
                    return new Result<bool> { Success = false, Message = "Không thể thanh toán đơn hàng đã hủy" };

                if (order.TotalAmount <= 0)
                    return new Result<bool> { Success = false, Message = "Đơn hàng không có item" };

                return new Result<bool> { Success = true, Data = true };
            }
            catch (Exception ex)
            {
                return new Result<bool> { Success = false, Message = $"Lỗi kiểm tra: {ex.Message}" };
            }
        }

        /// <summary>
        /// Lấy danh sách đơn hàng chưa thanh toán
        /// </summary>
        public Result<List<OrderDTO>> GetUnpaidOrders(int branchId)
        {
            try
            {
                var result = _orderDAL.GetUnpaidOrders(branchId);
                return result;
            }
            catch (Exception ex)
            {
                return new Result<List<OrderDTO>> { Success = false, Message = $"Lỗi lấy danh sách: {ex.Message}" };
            }
        }
    }
}

