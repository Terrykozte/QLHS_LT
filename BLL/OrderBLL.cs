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

        public OrderBLL() : this(new OrderDAL())
        {
        }

        public OrderBLL(IOrderRepository repo)
        {
            _repo = repo;
        }

                public List<OrderDTO> GetAll(DateTime fromDate, DateTime toDate, string status, string keyword)
        {
            return _repo.GetAll(fromDate, toDate, status, keyword);
        }

        public OrderDTO GetById(int id)
        {
            return _repo.GetById(id);
        }

        public List<OrderDetailDTO> GetDetails(int orderId)
        {
            return _repo.GetDetails(orderId);
        }

        public void CancelOrder(int orderId)
        {
            _repo.UpdateStatus(orderId, "Cancelled");
        }

        public int Create(OrderDTO order)
        {
            if (order.OrderDetails == null || order.OrderDetails.Count == 0)
            {
                throw new ArgumentException("Đơn hàng phải có ít nhất một sản phẩm.");
            }

            int newOrderId = _repo.Insert(order);
            foreach (var detail in order.OrderDetails)
            {
                detail.OrderID = newOrderId;
                _repo.AddDetail(detail);
            }
            return newOrderId;
        }
    }
}

