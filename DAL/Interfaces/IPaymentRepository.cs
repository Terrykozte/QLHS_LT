using System;
using System.Collections.Generic;
using QLTN_LT.DTO;

namespace QLTN_LT.DAL.Interfaces
{
    public interface IPaymentRepository
    {
        int Insert(PaymentDTO payment);
        PaymentDTO GetById(int paymentId);
        List<PaymentDTO> GetByOrderId(int orderId);
        List<PaymentDTO> GetAll(DateTime fromDate, DateTime toDate, string status = null);
        void UpdateStatus(int paymentId, string status);
        List<PaymentMethodDTO> GetPaymentMethods();
    }
}

