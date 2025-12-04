using System;
using System.Collections.Generic;
using QLTN_LT.DAL;
using QLTN_LT.DAL.Interfaces;
using QLTN_LT.DTO;

namespace QLTN_LT.BLL
{
    public class PaymentBLL
    {
        private readonly PaymentDAL _repo;

        public PaymentBLL() 
        {
            _repo = new PaymentDAL();
        }

        public int CreatePayment(PaymentDTO payment)
        {
            if (payment == null)
                throw new ArgumentNullException(nameof(payment));

            if (payment.OrderID <= 0)
                throw new ArgumentException("OrderID phải lớn hơn 0");

            if (payment.Amount <= 0)
                throw new ArgumentException("Số tiền phải lớn hơn 0");

            return _repo.Insert(payment);
        }

        public PaymentDTO GetPaymentById(int paymentId)
        {
            if (paymentId <= 0)
                throw new ArgumentException("PaymentID phải lớn hơn 0");

            return _repo.GetById(paymentId);
        }

        public List<PaymentDTO> GetPaymentsByOrderId(int orderId)
        {
            if (orderId <= 0)
                throw new ArgumentException("OrderID phải lớn hơn 0");

            return _repo.GetByOrderId(orderId);
        }

        public List<PaymentDTO> GetAllPayments(DateTime fromDate, DateTime toDate, string status = null)
        {
            if (fromDate > toDate)
                throw new ArgumentException("Ngày bắt đầu không được lớn hơn ngày kết thúc");

            return _repo.GetAll(fromDate, toDate, status);
        }

        public void UpdatePaymentStatus(int paymentId, string status)
        {
            if (paymentId <= 0)
                throw new ArgumentException("PaymentID phải lớn hơn 0");

            if (string.IsNullOrWhiteSpace(status))
                throw new ArgumentException("Status không được để trống");

            _repo.UpdateStatus(paymentId, status);
        }

        public List<PaymentMethodDTO> GetPaymentMethods()
        {
            return _repo.GetPaymentMethods();
        }

        public decimal CalculateTotalPaid(int orderId)
        {
            var payments = GetPaymentsByOrderId(orderId);
            decimal total = 0;
            foreach (var payment in payments)
            {
                if (payment.Status == "Completed")
                    total += payment.Amount;
            }
            return total;
        }
    }
}

