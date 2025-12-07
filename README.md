# MainSystem — Quản lý cửa hàng hải sản/lẩu tươi

Mục tiêu của dự án: cung cấp một hệ thống WinForms hoàn chỉnh để vận hành cửa hàng (bán hàng theo bàn/mang đi), quản lý kho, thực đơn, khách hàng, nhà cung cấp và báo cáo.

---
## 1) Mục tiêu & phạm vi
- Quản lý đơn hàng: chọn món, giỏ hàng, tạo đơn, thanh toán (tiền mặt/chuyển khoản/VietQR), xuất hóa đơn.
- Quản lý thực đơn: danh mục, món (giá, mô tả, trạng thái), tạo mã QR menu (tùy chọn).
- Quản lý kho: nhập/xuất, lịch sử giao dịch, tồn kho, mức cảnh báo.
- Quản lý khách hàng/nhà cung cấp: thông tin, lịch sử liên quan.
- Báo cáo: doanh thu, kho, báo cáo tháng.
- Phân quyền: Admin/Staff (RBAC đơn giản trong GUI).

---
## 2) Kiến trúc lớp (Layered Architecture)
```
GUI (WinForms)  →  BLL (Business Logic)  →  DAL (Data Access)  →  Database (SQL Server)
          ↑                  ↓
        DTO (Data Transfer Objects)
```
- GUI: Tất cả Form kế thừa BaseForm để có style, xử lý chung, cleanup tài nguyên.
- BLL: Xử lý nghiệp vụ, tính toán, xác nhận thanh toán, cập nhật trạng thái đơn… Không phụ thuộc cụ thể UI.
- DAL: Truy cập DB (ADO.NET), gọi stored procedures hoặc câu lệnh SQL, ánh xạ sang DTO.
- DTO: Các lớp dữ liệu thuần (POCO) trao đổi giữa các tầng.

---
## 3) Cấu trúc thư mục
```
QLHS_LT/
├─ GUI/                 # Tầng giao diện (WinForms)
│  ├─ Main/             # FormMain (sidebar, điều hướng)
│  ├─ Dashboard/        # Tổng quan
│  ├─ Menu/             # FormMenuList, FormMenuQR
│  ├─ Order/            # Tạo/DS/Chi tiết/Thanh toán
│  ├─ Inventory/        # Kho (DS, nhập, xuất, giao dịch)
│  ├─ Customer/         # Danh sách/Thêm/Sửa
│  ├─ Supplier/         # Danh sách/Thêm/Sửa
│  ├─ Table/            # Bàn ăn, QR bàn
│  ├─ Category/         # Danh mục món
│  ├─ Report/           # Báo cáo doanh thu/kho/tháng
│  ├─ Authentication/   # Đăng nhập, cấu hình
│  ├─ Utilities|Helper|Base|Controls
│  └─ Payment/          # Công cụ VietQR nâng cao
├─ BLL/                 # Business logic (dịch vụ)
├─ DAL/                 # Data access (ADO.NET/Stored Proc)
├─ DTO/                 # Đối tượng dữ liệu (POCO)
└─ Properties/, Program.cs
```

---
## 4) Bản đồ lớp chính (Class Map — rút gọn)
- GUI/Base
  - BaseForm: Form cơ sở (style, fade-in, Wait, ESC, cleanup…)
- GUI/Utilities & Helper (trích yếu):
  - UIHelper: style control + dialog an toàn (ShowFormDialog/ShowSaveFileDialog/ShowPrintDialog).
  - KeyboardNavigationHelper, ResponsiveHelper, AnimationHelper, UXInteractionHelper…
  - LoadingOverlay, ResponsiveDesignHelper…
- GUI/Main
  - FormMain: điều hướng sidebar, RBAC, mở child form trong panel.
- GUI/Order
  - FormOrderCreate: chọn món → giỏ hàng → tạo đơn (hỗ trợ Khách lẻ) → hỏi thanh toán ngay.
  - FormOrderList: danh sách đơn, context menu "Thanh toán", Ctrl+P; xuất CSV/Excel-HTML.
  - FormOrderDetail: chi tiết đơn.
  - FormPayment: xác nhận thanh toán (Cash/Transfer/VietQR), auto-refresh trạng thái, export hóa đơn (PDF/Excel-HTML).
- GUI/Menu
  - FormMenuList: CRUD món; export CSV/Excel-HTML.
  - FormMenuQR: Bố cục dọc (trái danh sách món dạng card cuộn, phải tổng & QR).
- GUI/Inventory
  - FormInventoryManagement: DS tồn, giao dịch, nhập/xuất nhanh, export.
  - FormInventoryList, FormStockIn, FormStockOut, FormInventoryTransaction.
- GUI/Customer / Supplier / Category / Seafood / Table
  - Danh sách: context menu (Sửa/Xóa/Export…); phím tắt (F5, Enter, Delete, Ctrl+E, Ctrl+N); empty state; dialog an toàn.
  - Thêm/Sửa: kế thừa FormTemplate (Lưu/Hủy đóng ngay, không xuất hiện hộp thoại lồng gây phải đóng 2 lần).
- BLL (tiêu biểu)
  - OrderBLL, PaymentBLL, PaymentConfirmationService, VietQRIntegrationService
  - InventoryBLL, SupplierBLL, CustomerBLL, CategoryBLL, SeafoodBLL, TableBLL
  - CartService (giỏ hàng), MonthlyReportService, ReportBLL…
- DAL
  - DatabaseHelper, các Repository (OrderDAL, PaymentDAL, InventoryDAL, …)
- DTO
  - OrderDTO, OrderDetailDTO, PaymentDTO, CustomerDTO, SupplierDTO, SeafoodDTO, CategoryDTO, TableDTO, …

---
## 5) Quy ước & quyết định kỹ thuật quan trọng
- Dialog an toàn: luôn dùng UIHelper.ShowFormDialog/ShowSaveFileDialog/ShowPrintDialog để tránh tình trạng "đóng 2 lần".
- FormTemplate: bỏ hỏi lại khi Lưu/Hủy; Lưu/Hủy → đóng ngay (DialogResult OK/Cancel) để tránh dialog lồng.
- ESC chỉ đóng dialog thật: BaseForm chỉ xử lý ESC khi TopLevel = true; child forms trong Main không bị ảnh hưởng.
- Export: ưu tiên CSV; Excel-HTML (mở được bằng Excel) cho in ấn nhanh mà không cần thêm thư viện.
- RBAC đơn giản ở GUI (Admin/Staff); có thể mở rộng ở BLL/DAL nếu cần.

---
## 6) Thiết lập & chạy
Yêu cầu: Visual Studio 2019+, .NET Framework 4.7.2+, SQL Server 2016+, Guna UI2 WinForms.

Bước cài đặt:
1) Mở `QLHS_LT.sln` bằng Visual Studio
2) Khôi phục NuGet packages
3) Cấu hình kết nối DB trong `DAL/ConnectionSettings.cs` (hoặc qua FormConfig trong ứng dụng)
4) Đảm bảo DB có sẵn schema/tables/stored procedures tương ứng
5) Build & Run (F5)

Chuỗi kết nối ví dụ:
```csharp
// DAL/ConnectionSettings.cs
public static string ConnectionString = "Server=YOUR_SERVER;Database=QLHS_LT;User Id=sa;Password=YOUR_PASSWORD;";
```

---
## 7) Luồng chính khuyến nghị (tham khảo)
- Bán hàng: Menu → chọn món → Order Create (giỏ hàng) → Lưu đơn → Thanh toán (FormPayment).
- Thanh toán nhanh: Order List → chọn đơn → Chuột phải "Thanh toán" (Ctrl+P) → FormPayment.
- Menu QR: chọn món (trái) → tạo QR (phải) → (tùy chọn) tạo đơn & hỏi thanh toán.

---
## 8) Hỗ trợ
- Điện thoại: 0369874654
- Liên hệ: Phạm Hoài Thương
- IG/FB: Terrykozte
- Email sinh viên: 52300262@student.tdtu.edu.vn
- Email cá nhân: binmin81@gmail.com
- Email công việc: phamhoaithuong1408@gmail.com

(Thông tin trên là kênh hỗ trợ – mọi góp ý/báo lỗi vui lòng mô tả rõ ràng tính năng, bước thao tác và ảnh chụp màn hình.)
