# WebShortlink System

Dự án WebShortlink là nền tảng rút gọn tên miền với chức năng quản lý người dùng (Role-based), phân tích luồng truy cập (Analytics), và chứng thực tên miền tùy chỉnh (Custom Domain).

## 1. Yêu cầu môi trường
- **Backend:** .NET 8 SDK
- **Frontend:** Node.js (Version >= 18)
- **Database:** SQL Server (hoặc SQL LocalDB)
- **Cache/Queue:** Redis (Tùy chọn, cần thiết để hệ thống Analytics đạt hiệu năng tối đa)

## 2. Quản lý cấu hình & Bí mật (Secrets)
Kho lưu trữ (repository) không chứa bất kỳ secret thực tế nào (kể cả cho dev). Bạn cần tự thiết lập cấu hình:

- **Local / Development:** 
  Chỉnh sửa file `backend/backend/appsettings.Development.json`. Thay thế các giá trị placeholder (`REPLACE_...`) như `Jwt:SigningKey`, `Smtp:Password` bằng cấu hình thật của bạn, hoặc sử dụng `dotnet user-secrets`.
  
- **Production:** 
  Đọc file `backend/backend/.env.example` để biết cấu trúc cấu hình. Biến thiết lập qua tham số môi trường:
  - `Jwt__SigningKey`: Phải có độ dài tối thiểu 32 ký tự.
  - `Smtp__Password`: Mật khẩu gửi email.
  - `ConnectionStrings__SqlServer`: Chuỗi kết nối DB.

## 3. Chạy Backend
Mở terminal tại thư mục backend:
```bash
cd backend/backend
# Tùy chọn: thiết lập user-secrets nếu không muốn sửa trực tiếp appsettings.Development.json
dotnet run
```
**Lưu ý khởi động Production:**
- **Seeding Fail-Fast:** Nếu cơ sở dữ liệu lỗi đồng bộ (schema migration) khi chạy production, ứng dụng sẽ `Fail-Fast` (tắt tiến trình) thay vì chạy với dữ liệu lỗi.
- **Admin Bootstrap:** Để ứng dụng tự tạo tài khoản Admin gốc trong database trống, hãy cung cấp 2 biến hệ thống: `App__AdminEmail` và `App__AdminPassword`. Nếu bỏ trống `AdminPassword`, bước này an toàn bị bỏ qua. Bắt buộc đổi Password sau lần login đầu.

## 4. Chạy Frontend
Cấu hình `.env` từ `.env.example`, trỏ `VITE_API_BASE_URL` tới địa chỉ Backend API.
```bash
cd frontend
npm install
npm run dev
```

## 5. Domain Verification
Giải pháp chứng thực Custom Domain đang sử dụng là **HTTP Tĩnh (Well-Known URI)**:
Ứng dụng kiểm tra sự tồn tại của file `/.well-known/webshortlink-verification.txt` chứa token tại custom domain (Ưu tiên quét HTTPS trước, HTTP sau; timeout 10s). Quản trị viên nên biết rằng phương pháp này chưa hỗ trợ xác thực qua DNS TXT.
