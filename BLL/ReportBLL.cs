using System;
using System.Collections.Generic;
using QLTN_LT.DAL;
using QLTN_LT.DTO;

namespace QLTN_LT.BLL
{
    public class ReportBLL
    {
        private readonly ReportDAL _dal = new ReportDAL();

        public List<DailyRevenueDTO> GetDailyRevenue(DateTime fromDate, DateTime toDate)
        {
            {
                return _dal.GetDailyRevenue(fromDate, toDate);
            }
        }

        public int GetOrderCount(DateTime fromDate, DateTime toDate)
        {
            return _dal.GetOrderCount(fromDate, toDate);
        }

        public int GetNewCustomersCount(DateTime fromDate, DateTime toDate)
        {
            return _dal.GetNewCustomersCount(fromDate, toDate);
        }

                public List<InventoryStatusDTO> GetInventoryStatusReport()
        {
            return _dal.GetInventoryStatusReport();
        }

        public List<TopSellingItemDTO> GetTopSellingItems(int topCount, DateTime fromDate, DateTime toDate)
        {
            return _dal.GetTopSellingItems(topCount, fromDate, toDate);
        }
    }
}

