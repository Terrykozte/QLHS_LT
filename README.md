# MainSystem - Quản Lý Cửa Hàng Hải Sản Lẩu Tươi

## 🎯 Mục Đích

Hệ thống quản lý toàn diện cho cửa hàng hải sản lẩu tươi, bao gồm:
- Quản lý đơn hàng
- Quản lý kho hàng
- Quản lý thực đơn
- Quản lý khách hàng
- Quản lý nhân viên
- Báo cáo doanh thu

## ✨ Tính Năng Chính

### 📊 Dashboard
- Hiển thị tổng quan doanh thu
- Thống kê đơn hàng
- Thống kê khách hàng
- Top 5 món bán chạy
- Hàng sắp hết hạn

### 🍤 Quản Lý Hải Sản
- Thêm/Sửa/Xóa hải sản
- Phân loại theo danh mục
- Quản lý nhà cung cấp
- Theo dõi giá cả

### 📋 Quản Lý Đơn Hàng
- Tạo đơn hàng
- Chọn bàn ăn
- Chọn hải sản
- Thanh toán
- Lịch sử đơn hàng

### 📦 Quản Lý Kho
- Nhập kho
- Xuất kho
- Theo dõi giao dịch
- Báo cáo kho

### 📖 Quản Lý Thực Đơn
- Tạo thực đơn
- Chọn hải sản
- Cập nhật giá
- Phân loại theo danh mục

### 👥 Quản Lý Khách Hàng
- Thêm/Sửa/Xóa khách hàng
- Lịch sử mua hàng
- Thông tin liên hệ

### 🏢 Quản Lý Nhà Cung Cấp
- Thêm/Sửa/Xóa nhà cung cấp
- Thông tin liên hệ
- Lịch sử giao dịch

### 👤 Quản Lý Người Dùng
- Tạo tài khoản
- Phân quyền
- Quản lý vai trò

### 📊 Báo Cáo
- Báo cáo doanh thu
- Báo cáo kho hàng
- Báo cáo hàng tháng

## 🏗️ Kiến Trúc

```
GUI Layer (Giao Diện)
    ↓
BLL Layer (Xử Lý Nghiệp Vụ)
    ↓
DAL Layer (Truy Cập Dữ Liệu)
    ↓
Database (SQL Server)
```

## 🔐 Quyền Truy Cập

| Vai Trò | Quyền |
|---------|-------|
| Admin | Truy cập toàn bộ |
| Staff | Truy cập giới hạn |
| Khác | Chỉ Dashboard |

## 📁 Cấu Trúc Thư Mục

```
QLHS_LT/
├── GUI/              # Giao diện người dùng
├── BLL/              # Xử lý nghiệp vụ
├── DAL/              # Truy cập dữ liệu
├── DTO/              # Đối tượng dữ liệu
└── Properties/       # Cài đặt dự án
```

## [object Object]ắt Đầu

### Yêu Cầu
- Visual Studio 2019+
- .NET Framework 4.7.2+
- SQL Server 2016+
- Guna UI2 WinForms

### Cài Đặt
1. Clone dự án
2. Mở `QLHS_LT.sln` trong Visual Studio
3. Khôi phục NuGet packages
4. Cập nhật chuỗi kết nối trong `ConnectionSettings.cs`
5. Chạy migration database
6. Biên dịch và chạy

### Cấu Hình Database
```csharp
// DAL/ConnectionSettings.cs
public static string ConnectionString = 
    "Server=YOUR_SERVER;Database=QLHS_LT;User Id=sa;Password=YOUR_PASSWORD;";
```

## 📚 Tài Liệu

### Hướng Dẫn Chính
- **[INTEGRATION_GUIDE.md](INTEGRATION_GUIDE.md)** - Hướng dẫn tích hợp chi tiết
- **[QUICK_REFERENCE.md](QUICK_REFERENCE.md)** - Tham chiếu nhanh
- **[ARCHITECTURE.md](ARCHITECTURE.md)** - Kiến trúc hệ thống
- **[FIXES_APPLIED.md](FIXES_APPLIED.md)** - Chi tiết sửa chữa
- **[SUMMARY.md](SUMMARY.md)** - Tóm tắt

### Hướng Dẫn Phát Triển
1. Đọc `ARCHITECTURE.md` để hiểu kiến trúc
2. Đọc `INTEGRATION_GUIDE.md` để hiểu cách tích hợp
3. Đọc `QUICK_REFERENCE.md` để tham khảo nhanh

## 🔧 Phát Triển

### Thêm Trang Mới
1. Tạo form kế thừa từ `BaseForm`
2. Thêm button vào `FormMain`
3. Cập nhật `CreateFormForButton()`
4. Cập nhật `HasAccessForButton()`
5. Thêm tooltip

### Thêm Tính Năng Mới
1. Tạo DTO trong `DTO/`
2. Tạo DAL trong `DAL/`
3. Tạo BLL trong `BLL/`
4. Tạo Form trong `GUI/`
5. Cập nhật `FormMain`

## [object Object]ỡ Rối

### Lỗi Thường Gặp

**Lỗi: "Lỗi hiển thị trang"**
- Kiểm tra: Form có kế thừa BaseForm không?
- Kiểm tra: InitializeComponent() có được gọi không?

**Lỗi: "Bạn không có quyền truy cập"**
- Kiểm tra: Vai trò người dùng có đúng không?
- Kiểm tra: HasAccessForButton() có logic đúng không?

**Lỗi: "Không thể kết nối DB"**
- Kiểm tra: Chuỗi kết nối có đúng không?
- Kiểm tra: SQL Server có chạy không?

### Debug Mode
```csharp
System.Diagnostics.Debug.WriteLine($"Debug: {message}");
```

## 📊 Luồng Sử Dụng

### Tạo Đơn Hàng
```
1. Dashboard → Đơn Hàng
2. Chọn Bàn Ăn
3. Chọn Hải Sản
4. Nhập Số Lượng
5. Xác Nhận
6. Thanh Toán
```

### Quản Lý Kho
```
1. Dashboard → Kho Hàng
2. Chọn: Nhập Kho / Xuất Kho
3. Chọn Hải Sản
4. Nhập Số Lượng
5. Xác Nhận
```

## 🎨 Styling

### Màu Sắc
- **Primary**: #3B82F6 (Blue)
- **Success**: #16A34A (Green)
- **Danger**: #EF4444 (Red)
- **Background**: #F9FAFB (Gray)

### Font
- **Tiêu đề**: Segoe UI, 14pt, Bold
- **Nội dung**: Segoe UI, 10pt

## 📞 Hỗ Trợ

Nếu gặp vấn đề:
1. Kiểm tra tài liệu
2. Xem Debug Output
3. Kiểm tra Exception Handler
4. Liên hệ với nhóm phát triển

## 📝 Lịch Sử Thay Đổi

### v1.0 (2025-12-04)
- ✅ Sửa chữa lỗi FormDashboard
- ✅ Tạo tài liệu hướng dẫn
- ✅ Đảm bảo tất cả các trang liên kết
- ✅ Kiểm tra chất lượng

## 📄 Giấy Phép

Dự án này được phát triển cho mục đích quản lý cửa hàng.

## 👨[object Object] Giả

- **Nhóm Phát Triển**: QLHS_LT Team
- **Cập Nhật Cuối**: 2025-12-04

## 🙏 Cảm Ơn

Cảm ơn bạn đã sử dụng MainSystem!

---

**Phiên bản**: 1.0  
**Trạng thái**: ✅ Hoàn thành & Sẵn Sàng  
**Cập nhật**: 2025-12-04
