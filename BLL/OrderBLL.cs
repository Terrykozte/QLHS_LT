using System;
using System.Collections.Generic;
using QLTN_LT.DAL;
using QLTN_LT.DAL.Interfaces;
using QLTN_LT.DTO;

namespace QLTN_LT.BLL
{
    public class OrderBLL
    {
        private readonly IOrderRepository _repo;
        private readonly PaymentBLL _paymentBLL;

        public OrderBLL() : this(new OrderDAL(), new PaymentBLL())
        {
        }

        public OrderBLL(IOrderRepository repo, PaymentBLL paymentBLL = null)
        {
            _repo = repo;
            _paymentBLL = paymentBLL ?? new PaymentBLL();
        }

        public List<OrderDTO> GetAll(DateTime fromDate, DateTime toDate, string status, string keyword)
        {
            if (fromDate > toDate)
                throw new ArgumentException("Ngày bắt đầu không được lớn hơn ngày kết thúc");

            return _repo.GetAll(fromDate, toDate, status, keyword);
        }

        public OrderDTO GetById(int id)
        {
            if (id <= 0)
                throw new ArgumentException("OrderID phải lớn hơn 0");

            return _repo.GetById(id);
        }

        public List<OrderDetailDTO> GetDetails(int orderId)
        {
            if (orderId <= 0)
                throw new ArgumentException("OrderID phải lớn hơn 0");

            return _repo.GetDetails(orderId);
        }

        public void CancelOrder(int orderId)
        {
            if (orderId <= 0)
                throw new ArgumentException("OrderID phải lớn hơn 0");

            _repo.UpdateStatus(orderId, "Cancelled");
        }

        public void CompleteOrder(int orderId)
        {
            if (orderId <= 0)
                throw new ArgumentException("OrderID phải lớn hơn 0");

            _repo.UpdateStatus(orderId, "Completed");
        }

        public int Create(OrderDTO order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            if (order.OrderDetails == null || order.OrderDetails.Count == 0)
            {
                throw new ArgumentException("Đơn hàng phải có ít nhất một sản phẩm.");
            }

            if (order.TotalAmount <= 0)
            {
                throw new ArgumentException("Tổng tiền phải lớn hơn 0.");
            }

            int newOrderId = _repo.Insert(order);
            foreach (var detail in order.OrderDetails)
            {
                detail.OrderID = newOrderId;
                _repo.AddDetail(detail);
            }
            return newOrderId;
        }

        public decimal GetOrderTotal(int orderId)
        {
            var order = GetById(orderId);
            return order?.TotalAmount ?? 0;
        }

        public decimal GetRemainingAmount(int orderId)
        {
            var orderTotal = GetOrderTotal(orderId);
            var paidAmount = _paymentBLL.CalculateTotalPaid(orderId);
            return orderTotal - paidAmount;
        }

        public string GetPaymentStatus(int orderId)
        {
            var remaining = GetRemainingAmount(orderId);
            if (remaining <= 0)
                return "Paid";
            else if (remaining < GetOrderTotal(orderId))
                return "Partial";
            else
                return "Unpaid";
        }
    }
}

