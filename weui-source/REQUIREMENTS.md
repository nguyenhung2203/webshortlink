# REQUIREMENTS — React UI Showcase → Vue 3 + TypeScript + Tailwind Template

## 1. Mục tiêu dự án
Chuyển dự án UI hiện tại từ React + TypeScript + Vite thành một template UI chuẩn thị trường dùng Vue 3 + TypeScript, có thể sử dụng thật cho dashboard/admin panel hoặc làm nền cho các sản phẩm web nội bộ/SaaS.

Đây không phải tác vụ port JSX đơn thuần.
Đây là tác vụ refactor + redesign architecture + chuẩn hóa design system + template hóa để dùng thật.

---

## 2. Hiện trạng
Dự án hiện tại là một UI showcase / design-system demo, có các đặc điểm:

- React + TypeScript + Vite
- Tailwind CSS
- Nhiều component và pattern bị dồn vào một file lớn `src/App.tsx`
- Có light/dark mode
- Có token và nhiều section demo
- Có nhiều component hữu ích, nhưng cấu trúc hiện tại chưa phù hợp để dùng như template production-ready

---

## 3. Công nghệ đích bắt buộc
Template sau khi chuyển phải sử dụng:

- Vue 3
- TypeScript
- Vite
- Vue Router
- Pinia
- Tailwind CSS

---

## 4. Nguyên tắc triển khai
Phải tuân thủ các nguyên tắc sau:

1. Không được chuyển đổi theo kiểu cơ học từ JSX sang Vue.
2. Phải tái cấu trúc kiến trúc để phù hợp với Vue template production-ready.
3. Phải giữ tinh thần thiết kế hiện tại ở mức hợp lý:
   - hiện đại
   - premium
   - enterprise
   - dark-mode capable
   - có điểm nhấn gradient nhẹ
4. Phải ưu tiên khả năng tái sử dụng, bảo trì, mở rộng lâu dài.
5. Không được dồn logic vào một file lớn như hiện trạng.
6. Không được tạo quá nhiều micro-component vô nghĩa.
7. Không được phụ thuộc nặng vào UI library bên ngoài nếu không thật sự cần.

---

## 5. Yêu cầu về styling
- Tailwind CSS là hệ thống styling chính của toàn bộ template.
- Không dùng CSS rời rạc tràn lan cho từng page/component nếu không thật sự cần.
- Design tokens phải được tổ chức bằng CSS variables để hỗ trợ light/dark theme.
- Tailwind phải tích hợp tốt với semantic tokens.
- Hạn chế inline style.
- Hạn chế hardcode màu trực tiếp trong component.
- Ưu tiên utility class rõ ràng, nhất quán, dễ maintain.
- Dự án phải là Tailwind-first template, không phải Vue port + CSS chắp vá.

---

## 6. Yêu cầu theme
- Có light mode và dark mode
- Theme phải dựa trên CSS variables + Tailwind
- Không nhân đôi style thủ công cho từng theme
- Có semantic tokens rõ ràng, ví dụ:
  - background
  - foreground
  - border
  - muted
  - primary
  - success
  - warning
  - danger
  - info

---

## 7. Mục tiêu kiến trúc
Sau khi chuyển, dự án phải có:

- Kiến trúc thư mục Vue hợp lý
- Layout tách riêng
- Router thật
- Pages thật
- Reusable components chuẩn
- State management hợp lý
- Design tokens rõ ràng
- Theme hoạt động đúng
- Có thể dùng làm nền cho phát triển sản phẩm tiếp theo

---

## 8. Phạm vi component cần chuẩn hóa
Bao gồm nhưng không giới hạn:

- WxButton
- WxInput
- WxPasswordInput
- WxTextarea
- WxInputNumber
- WxSelect
- WxMultiSelect
- WxDropdown
- WxSwitch
- WxRadio
- WxCheckbox
- WxTabs
- WxAvatar
- WxTag
- WxBadge
- WxSpinner
- WxProgressBar
- WxPagination
- WxSkeleton
- WxCard
- WxEmptyState
- WxAlert
- WxStatCard
- WxFilterBar
- WxFormSection
- WxBreadcrumb
- WxStepper
- WxPageHeader
- WxFileUpload
- WxDatePicker
- WxSlider
- WxDrawer
- WxTimeline
- WxTreeView
- WxToast
- WxModal
- WxConfirmDialog
- WxDataTable
- WxSideNav

---

## 9. Yêu cầu về page/template
Template phải có tối thiểu các trang sau:

- Dashboard Overview
- Form Elements
- Data Table
- Feedback / Alerts / Toasts
- Dialogs / Drawers
- File Upload
- Charts / Analytics
- Settings
- Login
- Register
- Forgot Password
- Tokens / Design System
- Not Found

Các page này phải nhìn như một sản phẩm thật, không phải chỉ là showcase linh tinh.

---

## 10. Yêu cầu component API
Mỗi component reusable cần có:

- Props rõ ràng
- Emits rõ ràng
- Slots hợp lý
- Variants
- Sizes
- Disabled/loading/error states nếu phù hợp
- Accessibility cơ bản
- Dark mode support
- TypeScript typing rõ ràng
- Naming nhất quán

---

## 11. Yêu cầu state management
Phải dùng Pinia ở mức hợp lý, không lạm dụng.

Ưu tiên cho:
- theme mode
- sidebar state
- toast state
- modal/drawer global state nếu cần
- UI app preferences

Không nên đưa toàn bộ local form state vào store nếu không cần.

---

## 12. Yêu cầu router/navigation
- Dùng Vue Router
- Có nested routes hợp lý
- Có side navigation thật
- Có breadcrumb mapping
- Có route meta/title cơ bản
- Có route cho auth pages
- Có route 404

---

## 13. Yêu cầu tài liệu
Tất cả tài liệu phân tích, mapping, kế hoạch, báo cáo phải lưu trong:

`docs/vue-ui-template-migration/`

Tài liệu phải:
- ngắn gọn
- có cấu trúc
- hữu ích
- không lan man
- đủ để agent và dev khác đọc là hiểu hướng triển khai

---

## 14. Tiêu chí hoàn thành
Dự án được xem là hoàn thành khi đáp ứng đủ:

1. Có kiến trúc Vue rõ ràng
2. Có layouts, router, stores cơ bản
3. Có pages mẫu hoạt động
4. Có reusable components đủ dùng
5. Có dark mode / light mode hoạt động
6. Có Tailwind-first styling + CSS variable tokens
7. Có mapping từ React cũ sang Vue mới
8. Có tài liệu triển khai và báo cáo cuối
9. Không còn kiểu monolithic `App.tsx` như bản cũ
10. Kết quả nhìn như template thị trường có thể dùng thật

---

## 15. Quy định ra quyết định khi thiếu thông tin
Nếu thiếu thông tin, được phép:
- tự chọn mặc định hợp lý
- ghi rõ giả định
- tiếp tục triển khai

Không dừng lại để hỏi xác nhận cho các quyết định nhỏ.
