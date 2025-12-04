using System;
using System.Collections.Generic;
using System.Linq;
using QLTN_LT.DTO;

namespace QLTN_LT.BLL
{
    /// <summary>
    /// Dịch vụ quản lý giỏ hàng
    /// </summary>
    public class CartService
    {
        private List<CartItem> _items;
        public event EventHandler CartChanged;

        public CartService()
        {
            _items = new List<CartItem>();
        }

        /// <summary>
        /// Thêm sản phẩm vào giỏ
        /// </summary>
        public void AddItem(MenuItemDTO product, int quantity = 1)
        {
            if (product == null || quantity <= 0) return;

            var existingItem = _items.FirstOrDefault(i => i.ProductId == product.ItemID);
            
            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {
                _items.Add(new CartItem
                {
                    ProductId = product.ItemID,
                    ProductName = product.ItemName,
                    UnitPrice = product.UnitPrice,
                    Quantity = quantity,
                    Category = product.CategoryName
                });
            }

            OnCartChanged();
        }

        /// <summary>
        /// Cập nhật số lượng sản phẩm
        /// </summary>
        public void UpdateQuantity(int productId, int quantity)
        {
            var item = _items.FirstOrDefault(i => i.ProductId == productId);
            if (item != null)
            {
                if (quantity <= 0)
                {
                    _items.Remove(item);
                }
                else
                {
                    item.Quantity = quantity;
                }
                OnCartChanged();
            }
        }

        /// <summary>
        /// Xóa sản phẩm khỏi giỏ
        /// </summary>
        public void RemoveItem(int productId)
        {
            var item = _items.FirstOrDefault(i => i.ProductId == productId);
            if (item != null)
            {
                _items.Remove(item);
                OnCartChanged();
            }
        }

        /// <summary>
        /// Lấy tất cả sản phẩm trong giỏ
        /// </summary>
        public List<CartItem> GetItems()
        {
            return _items.ToList();
        }

        /// <summary>
        /// Lấy tổng số lượng sản phẩm
        /// </summary>
        public int GetTotalQuantity()
        {
            return _items.Sum(i => i.Quantity);
        }

        /// <summary>
        /// Lấy tổng tiền
        /// </summary>
        public decimal GetTotalAmount()
        {
            return _items.Sum(i => i.Quantity * i.UnitPrice);
        }

        /// <summary>
        /// Lấy tổng tiền sau khi áp dụng chiết khấu
        /// </summary>
        public decimal GetTotalWithDiscount(decimal discountPercent = 0)
        {
            decimal total = GetTotalAmount();
            if (discountPercent > 0 && discountPercent <= 100)
            {
                total = total * (1 - discountPercent / 100);
            }
            return total;
        }

        /// <summary>
        /// Xóa tất cả sản phẩm
        /// </summary>
        public void Clear()
        {
            _items.Clear();
            OnCartChanged();
        }

        /// <summary>
        /// Kiểm tra giỏ có trống không
        /// </summary>
        public bool IsEmpty()
        {
            return _items.Count == 0;
        }

        /// <summary>
        /// Lấy số lượng loại sản phẩm
        /// </summary>
        public int GetItemCount()
        {
            return _items.Count;
        }

        /// <summary>
        /// Lấy sản phẩm theo ID
        /// </summary>
        public CartItem GetItem(int productId)
        {
            return _items.FirstOrDefault(i => i.ProductId == productId);
        }

        /// <summary>
        /// Áp dụng mã giảm giá
        /// </summary>
        public decimal ApplyPromoCode(string promoCode)
        {
            // TODO: Kết nối với database để kiểm tra mã giảm giá
            decimal discount = 0;
            
            switch (promoCode?.ToUpper())
            {
                case "SUMMER10":
                    discount = 10;
                    break;
                case "SUMMER20":
                    discount = 20;
                    break;
                case "VIP":
                    discount = 15;
                    break;
                default:
                    discount = 0;
                    break;
            }

            return discount;
        }

        /// <summary>
        /// Tạo đơn hàng từ giỏ hàng
        /// </summary>
        public OrderDTO CreateOrder(int customerId, string customerName, string customerPhone, string notes = "")
        {
            if (_items.Count == 0) return null;

            var order = new OrderDTO
            {
                CustomerID = customerId,
                CustomerName = customerName,
                OrderDate = DateTime.Now,
                Status = "Pending",
                TotalAmount = GetTotalAmount(),
                OrderDetails = _items.Select(i => new OrderDetailDTO
                {
                    SeafoodID = i.ProductId,
                    SeafoodName = i.ProductName,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice
                }).ToList()
            };

            return order;
        }

        /// <summary>
        /// Lấy thông tin chi tiết giỏ hàng
        /// </summary>
        public CartSummary GetCartSummary()
        {
            return new CartSummary
            {
                Items = GetItems(),
                ItemCount = GetItemCount(),
                TotalQuantity = GetTotalQuantity(),
                TotalAmount = GetTotalAmount(),
                IsEmpty = IsEmpty()
            };
        }

        protected virtual void OnCartChanged()
        {
            CartChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    /// <summary>
    /// Model sản phẩm trong giỏ hàng
    /// </summary>
    public class CartItem
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public string Category { get; set; }

        public decimal Total => Quantity * UnitPrice;
        public decimal GetTotal()
        {
            return Total;
        }
    }

    /// <summary>
    /// Model tóm tắt giỏ hàng
    /// </summary>
    public class CartSummary
    {
        public List<CartItem> Items { get; set; }
        public int ItemCount { get; set; }
        public int TotalQuantity { get; set; }
        public decimal TotalAmount { get; set; }
        public bool IsEmpty { get; set; }
    }
}


