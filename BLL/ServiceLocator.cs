using System;
using System.Collections.Generic;

namespace QLTN_LT.BLL
{
    /// <summary>
    /// Service Locator - Quản lý các service trong ứng dụng
    /// Giúp dễ dàng truy cập các service từ bất kỳ nơi nào
    /// </summary>
    public static class ServiceLocator
    {
        private static readonly Dictionary<Type, object> _services = new Dictionary<Type, object>();

        // Các service chính
        private static OrderManagementService _orderService;
        private static PaymentManagementService _paymentService;
        private static InventoryManagementService _inventoryService;
        private static MenuBLL _menuService;
        private static SeafoodBLL _seafoodService;
        private static UserBLL _userService;
        private static ReportBLL _reportService;
        private static TableBLL _tableService;

        /// <summary>
        /// Khởi tạo tất cả service
        /// </summary>
        public static void Initialize()
        {
            _orderService = new OrderManagementService();
            _paymentService = new PaymentManagementService();
            _inventoryService = new InventoryManagementService();
            _menuService = new MenuBLL();
            _seafoodService = new SeafoodBLL();
            _userService = new UserBLL();
            _reportService = new ReportBLL();
            _tableService = new TableBLL();

            RegisterService(typeof(OrderManagementService), _orderService);
            RegisterService(typeof(PaymentManagementService), _paymentService);
            RegisterService(typeof(InventoryManagementService), _inventoryService);
            RegisterService(typeof(MenuBLL), _menuService);
            RegisterService(typeof(SeafoodBLL), _seafoodService);
            RegisterService(typeof(UserBLL), _userService);
            RegisterService(typeof(ReportBLL), _reportService);
            RegisterService(typeof(TableBLL), _tableService);
        }

        /// <summary>
        /// Đăng ký service
        /// </summary>
        public static void RegisterService(Type serviceType, object service)
        {
            if (_services.ContainsKey(serviceType))
                _services[serviceType] = service;
            else
                _services.Add(serviceType, service);
        }

        /// <summary>
        /// Lấy service
        /// </summary>
        public static T GetService<T>() where T : class
        {
            var type = typeof(T);
            if (_services.ContainsKey(type))
                return _services[type] as T;
            return null;
        }

        /// <summary>
        /// Lấy OrderManagementService
        /// </summary>
        public static OrderManagementService GetOrderService()
        {
            return _orderService ?? (_orderService = new OrderManagementService());
        }

        /// <summary>
        /// Lấy PaymentManagementService
        /// </summary>
        public static PaymentManagementService GetPaymentService()
        {
            return _paymentService ?? (_paymentService = new PaymentManagementService());
        }

        /// <summary>
        /// Lấy InventoryManagementService
        /// </summary>
        public static InventoryManagementService GetInventoryService()
        {
            return _inventoryService ?? (_inventoryService = new InventoryManagementService());
        }

        /// <summary>
        /// Lấy MenuBLL
        /// </summary>
        public static MenuBLL GetMenuService()
        {
            return _menuService ?? (_menuService = new MenuBLL());
        }

        /// <summary>
        /// Lấy SeafoodBLL
        /// </summary>
        public static SeafoodBLL GetSeafoodService()
        {
            return _seafoodService ?? (_seafoodService = new SeafoodBLL());
        }

        /// <summary>
        /// Lấy UserBLL
        /// </summary>
        public static UserBLL GetUserService()
        {
            return _userService ?? (_userService = new UserBLL());
        }

        /// <summary>
        /// Lấy ReportBLL
        /// </summary>
        public static ReportBLL GetReportService()
        {
            return _reportService ?? (_reportService = new ReportBLL());
        }

        /// <summary>
        /// Lấy TableBLL
        /// </summary>
        public static TableBLL GetTableService()
        {
            return _tableService ?? (_tableService = new TableBLL());
        }

        /// <summary>
        /// Xóa tất cả service
        /// </summary>
        public static void Clear()
        {
            _services.Clear();
            _orderService = null;
            _paymentService = null;
            _inventoryService = null;
            _menuService = null;
            _seafoodService = null;
            _userService = null;
            _reportService = null;
            _tableService = null;
        }

        /// <summary>
        /// Kiểm tra xem service có tồn tại không
        /// </summary>
        public static bool HasService<T>() where T : class
        {
            var type = typeof(T);
            return _services.ContainsKey(type);
        }

        /// <summary>
        /// Lấy tất cả service đã đăng ký
        /// </summary>
        public static Dictionary<Type, object> GetAllServices()
        {
            return new Dictionary<Type, object>(_services);
        }
    }
}

