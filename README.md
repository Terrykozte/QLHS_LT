# 📦 MainSystem – Quản Lý Cửa Hàng Hải Sản Lẩu Tươi

## 🎯 Mục Đích
Hệ thống hỗ trợ quản lý toàn diện cho cửa hàng hải sản lẩu tươi, bao gồm quản lý đơn hàng, kho, thực đơn, khách hàng, nhà cung cấp, nhân viên và báo cáo doanh thu.

---

## ✨ Tính Năng Chính

### 📊 Dashboard
- Tổng quan doanh thu  
- Thống kê đơn hàng – khách hàng  
- Món bán chạy  
- Cảnh báo hàng sắp hết hạn

### 🍤 Quản Lý Hải Sản
- Thêm / Sửa / Xóa  
- Phân loại theo danh mục  
- Theo dõi giá  
- Quản lý nhà cung cấp

### 📋 Quản Lý Đơn Hàng
- Tạo đơn hàng  
- Chọn bàn ăn  
- Chọn hải sản  
- Thanh toán  
- Lịch sử đơn hàng

### 📦 Quản Lý Kho
- Nhập – Xuất kho  
- Lịch sử giao dịch  
- Báo cáo tồn kho

### 📖 Quản Lý Thực Đơn
- Tạo thực đơn  
- Chọn hải sản  
- Cập nhật giá  
- Phân loại theo danh mục

### 👥 Quản Lý Khách Hàng
- Thêm / Sửa / Xóa  
- Lịch sử mua hàng  
- Thông tin liên hệ

### 🏢 Nhà Cung Cấp
- Quản lý danh sách  
- Lịch sử nhập hàng  

### 👤 Quản Lý Người Dùng
- Tạo tài khoản  
- Phân quyền vai trò  
- Quản lý truy cập

### 📊 Báo Cáo
- Báo cáo doanh thu  
- Báo cáo kho  
- Thống kê theo tháng

---

## 🏗️ Kiến Trúc Hệ Thống

```
GUI (Form WinForms)
   ↓
BLL (Business Logic Layer)
   ↓
DAL (Data Access Layer)
   ↓
Database (SQL Server)
```

---

## 🔐 Quyền Truy Cập

| Vai Trò | Quyền |
|--------|--------|
| **Admin** | Toàn quyền hệ thống |
| **Staff** | Quyền giới hạn theo nghiệp vụ |
| **Viewer** | Chỉ xem Dashboard |

---

## 📁 Cấu Trúc Thư Mục

```
QLHS_LT/
├── GUI/            # Giao diện
├── BLL/            # Xử lý nghiệp vụ
├── DAL/            # Truy cập dữ liệu
├── DTO/            # Đối tượng dữ liệu
└── Properties/
```

---

## 🚀 Bắt Đầu

### Yêu Cầu
- Visual Studio 2019+  
- .NET Framework 4.7.2+  
- SQL Server 2016+  
- Guna UI2 WinForms  

### Cài Đặt
1. Clone dự án  
2. Mở `QLHS_LT.sln`  
3. Restore NuGet Packages  
4. Cập nhật chuỗi kết nối ở `ConnectionSettings.cs`  
5. Import database `finalqlhs.sql`  
6. Build & Run  

**Chuỗi kết nối mẫu:**
```csharp
public static string ConnectionString =
    "Server=YOUR_SERVER;Database=QLHS_LT;User Id=sa;Password=YOUR_PASSWORD;";
```

---

## 📚 Tài Liệu Dự Án

- `ARCHITECTURE.md` – Kiến trúc hệ thống  
- `INTEGRATION_GUIDE.md` – Hướng dẫn tích hợp  
- `QUICK_REFERENCE.md` – Tham chiếu nhanh  
- `FIXES_APPLIED.md` – Danh sách sửa lỗi  
- `SUMMARY.md` – Tóm tắt tổng quan  

---

## 📞 Liên Hệ / Hỗ Trợ
- **Người phụ trách:** Phạm Hoài Thương  
- **Số điện thoại:** 0369874654  

---

## 📝 Phiên Bản
**Version:** 1.0  
**Cập nhật gần nhất:** 2025-12-07
