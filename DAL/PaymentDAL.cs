using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using QLTN_LT.DTO;
using QLTN_LT.DAL.Interfaces;

namespace QLTN_LT.DAL
{
    public class PaymentDAL : IPaymentRepository
    {
        public int Insert(PaymentDTO payment)
        {
            const string sql = "sp_InsertPayment";
            using (var conn = DatabaseHelper.CreateConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@OrderID", payment.OrderID);
                cmd.Parameters.AddWithValue("@PaymentMethodID", payment.PaymentMethodID);
                cmd.Parameters.AddWithValue("@Amount", payment.Amount);
                cmd.Parameters.AddWithValue("@TransactionCode", string.IsNullOrEmpty(payment.TransactionCode) ? (object)DBNull.Value : payment.TransactionCode);
                cmd.Parameters.AddWithValue("@QRCode", string.IsNullOrEmpty(payment.QRCode) ? (object)DBNull.Value : payment.QRCode);
                cmd.Parameters.AddWithValue("@BankAccount", string.IsNullOrEmpty(payment.BankAccount) ? (object)DBNull.Value : payment.BankAccount);
                cmd.Parameters.AddWithValue("@BankName", string.IsNullOrEmpty(payment.BankName) ? (object)DBNull.Value : payment.BankName);
                cmd.Parameters.AddWithValue("@Status", payment.Status ?? "Pending");
                conn.Open();
                return (int)cmd.ExecuteScalar();
            }
        }

        public PaymentDTO GetById(int paymentId)
        {
            PaymentDTO payment = null;
            const string sql = @"
                SELECT p.PaymentID, p.OrderID, pm.PaymentMethodID, pm.MethodName, p.Amount, 
                       p.PaymentDate, p.TransactionCode, p.QRCode, p.BankAccount, p.BankName, p.Status, p.Notes
                FROM [dbo].[Payments] p
                INNER JOIN [dbo].[PaymentMethods] pm ON p.PaymentMethodID = pm.PaymentMethodID
                WHERE p.PaymentID = @PaymentID";

            using (var conn = DatabaseHelper.CreateConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@PaymentID", paymentId);
                conn.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    if (rd.Read())
                    {
                        payment = new PaymentDTO
                        {
                            PaymentID = rd.GetInt32(0),
                            OrderID = rd.GetInt32(1),
                            PaymentMethodID = rd.GetInt32(2),
                            MethodName = rd.GetString(3),
                            Amount = rd.GetDecimal(4),
                            PaymentDate = rd.GetDateTime(5),
                            TransactionCode = rd.IsDBNull(6) ? null : rd.GetString(6),
                            QRCode = rd.IsDBNull(7) ? null : rd.GetString(7),
                            BankAccount = rd.IsDBNull(8) ? null : rd.GetString(8),
                            BankName = rd.IsDBNull(9) ? null : rd.GetString(9),
                            Status = rd.GetString(10),
                            Notes = rd.IsDBNull(11) ? null : rd.GetString(11)
                        };
                    }
                }
            }
            return payment;
        }

        public List<PaymentDTO> GetByOrderId(int orderId)
        {
            var list = new List<PaymentDTO>();
            const string sql = "sp_GetPaymentsByOrderId";

            using (var conn = DatabaseHelper.CreateConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@OrderID", orderId);
                conn.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        list.Add(new PaymentDTO
                        {
                            PaymentID = rd.GetInt32(0),
                            OrderID = rd.GetInt32(1),
                            MethodName = rd.GetString(2),
                            Amount = rd.GetDecimal(3),
                            PaymentDate = rd.GetDateTime(4),
                            TransactionCode = rd.IsDBNull(5) ? null : rd.GetString(5),
                            Status = rd.GetString(6)
                        });
                    }
                }
            }
            return list;
        }

        public List<PaymentDTO> GetAll(DateTime fromDate, DateTime toDate, string status = null)
        {
            var list = new List<PaymentDTO>();
            const string sql = @"
                SELECT p.PaymentID, p.OrderID, pm.MethodName, p.Amount, p.PaymentDate, 
                       p.TransactionCode, p.Status
                FROM [dbo].[Payments] p
                INNER JOIN [dbo].[PaymentMethods] pm ON p.PaymentMethodID = pm.PaymentMethodID
                WHERE p.PaymentDate >= @FromDate AND p.PaymentDate <= @ToDate
                  AND (@Status IS NULL OR p.Status = @Status)
                ORDER BY p.PaymentDate DESC";

            using (var conn = DatabaseHelper.CreateConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@FromDate", fromDate);
                cmd.Parameters.AddWithValue("@ToDate", toDate);
                cmd.Parameters.AddWithValue("@Status", string.IsNullOrEmpty(status) ? (object)DBNull.Value : status);
                conn.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        list.Add(new PaymentDTO
                        {
                            PaymentID = rd.GetInt32(0),
                            OrderID = rd.GetInt32(1),
                            MethodName = rd.GetString(2),
                            Amount = rd.GetDecimal(3),
                            PaymentDate = rd.GetDateTime(4),
                            TransactionCode = rd.IsDBNull(5) ? null : rd.GetString(5),
                            Status = rd.GetString(6)
                        });
                    }
                }
            }
            return list;
        }

        public void UpdateStatus(int paymentId, string status)
        {
            const string sql = "sp_UpdatePaymentStatus";
            using (var conn = DatabaseHelper.CreateConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@PaymentID", paymentId);
                cmd.Parameters.AddWithValue("@Status", status);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public List<PaymentMethodDTO> GetPaymentMethods()
        {
            var list = new List<PaymentMethodDTO>();
            const string sql = "SELECT PaymentMethodID, MethodName, Description, IsActive FROM [dbo].[PaymentMethods] WHERE IsActive = 1";

            using (var conn = DatabaseHelper.CreateConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                conn.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        list.Add(new PaymentMethodDTO
                        {
                            PaymentMethodID = rd.GetInt32(0),
                            MethodName = rd.GetString(1),
                            Description = rd.IsDBNull(2) ? null : rd.GetString(2),
                            IsActive = rd.GetBoolean(3)
                        });
                    }
                }
            }
            return list;
        }
    }
}

