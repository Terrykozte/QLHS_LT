using System;
using System.Collections.Generic;
using QLTN_LT.DTO;

namespace QLTN_LT.DAL.Interfaces
{
    public interface IOrderRepository
    {
        List<OrderDTO> GetAll(DateTime fromDate, DateTime toDate, string status, string keyword);
        OrderDTO GetById(int id);
        int Insert(OrderDTO order);
        void UpdateStatus(int id, string status);
        void AddDetail(OrderDetailDTO detail);
        List<OrderDetailDTO> GetDetails(int orderId);
    }
}

