using System;
using System.Collections.Generic;
using System.Linq;
using QLTN_LT.DAL;
using QLTN_LT.DTO;

namespace QLTN_LT.BLL
{
    public class MenuBLL
    {
        private readonly MenuDAL _dal = new MenuDAL();

        public List<MenuItemDTO> GetAllItems()
        {
            return _dal.GetAllItems();
        }

        public MenuItemDTO GetItemById(int itemId)
        {
            return _dal.GetItemById(itemId);
        }

        public void InsertItem(MenuItemDTO item)
        {
            ValidateItem(item, requireId: false);
            _dal.InsertItem(item);
        }

        public void UpdateItem(MenuItemDTO item)
        {
            ValidateItem(item, requireId: true);
            _dal.UpdateItem(item);
        }

        public void DeleteItem(int itemId)
        {
            if (itemId <= 0) throw new ArgumentException("ItemID khA_ng h_p l_.");
            _dal.DeleteItem(itemId);
        }

        public Dictionary<string, List<MenuItemDTO>> GetGroupedMenu()
        {
            var allItems = GetAllItems();
            return allItems
                .GroupBy(item => item.CategoryName)
                .ToDictionary(g => g.Key, g => g.ToList());
        }

        private static void ValidateItem(MenuItemDTO item, bool requireId)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            if (requireId && item.ItemID <= 0) throw new ArgumentException("ItemID khA_ng h_p l_.");
            if (item.CategoryID <= 0) throw new ArgumentException("Danh m_c khA_ng h_p l_.");
            if (string.IsNullOrWhiteSpace(item.ItemName)) throw new ArgumentException("TA_n m_n khA_ng _A_c _A_nh.");
            if (item.UnitPrice < 0) throw new ArgumentException("GiA_khA_ng h_p l_.");
        }
    }
}
