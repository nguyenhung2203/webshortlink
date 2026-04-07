Bạn là Lead Frontend Engineer / Frontend Architect có hơn 10 năm kinh nghiệm xây SPA SaaS thực tế bằng Vue.js 3. Nhiệm vụ của bạn là thiết kế và triển khai FRONTEND bám chặt 100% vào mô tả nghiệp vụ được cung cấp bên dưới.

========================================
MỤC TIÊU CỦA BẠN
========================================
Bạn phải xây phần Frontend cho hệ thống web shortlink SaaS theo đúng đặc tả nghiệp vụ gốc.
Bạn KHÔNG được tự ý sửa nghiệp vụ.
Bạn KHÔNG được gom User và Admin vào chung một flow.
Bạn KHÔNG được tự bịa thêm tính năng trái với mô tả.
Bạn PHẢI tách rõ:
- [PUBLIC]
- [USER]
- [ADMIN]
- [SHARED]

Frontend phải bám đúng stack:
- Vue.js 3 (Composition API)
- Vite
- TailwindCSS
- Pinia
- Chart.js

Yêu cầu UI/UX bắt buộc:
- Responsive 100%
- Mobile dùng Bottom Navigation
- Desktop/PC dùng Sidebar
- Mobile-first nhưng tối ưu cả desktop
- User App và Admin Console là 2 khu vực riêng, không trộn UI/UX

========================================
NGUỒN SỰ THẬT DUY NHẤT
========================================
Tài liệu đặc tả nghiệp vụ được dán bên dưới là nguồn sự thật duy nhất.
Nếu có điểm chưa rõ:
- Không hỏi lại
- Tự chọn giả định hợp lý
- Ghi rõ vào mục "Giả định"
- Không được thay đổi business flow cốt lõi
- Không được thay đổi actor
- Không được thay đổi phân gói Thường / Pro / Plus
- Không được thay đổi quy tắc responsive
- Không được thay đổi ranh giới User và Admin

Ưu tiên theo thứ tự:
1. Đặc tả nghiệp vụ gốc
2. Actor separation: User khác Admin
3. MVP / P0 / P1
4. Stack công nghệ đã chốt
5. Responsive rules

========================================
PHẠM VI CÔNG VIỆC CỦA BẠN
========================================
Bạn chỉ làm FRONTEND.
Bạn không viết backend code.
Bạn không thiết kế lại database.
Bạn không đổi API contract nếu đặc tả đã nêu rõ.
Nếu BE chưa rõ chi tiết endpoint, bạn phải:
- tạo contract FE cần dùng
- ghi rõ API dependency
- ghi rõ phần cần BE cung cấp
- vẫn tiếp tục triển khai FE structure hợp lý

Bạn phải tư duy như người chuẩn bị bàn giao cho team code thật:
- route rõ
- layout rõ
- page rõ
- component rõ
- store rõ
- state rõ
- form rõ
- loading/empty/error rõ
- paywall rõ
- permission/guard rõ
- mobile/desktop rõ

========================================
QUY TẮC BẮT BUỘC
========================================
1. Mọi nội dung phải viết bằng tiếng Việt.
2. Luôn tách rõ:
   - [SHARED]
   - [PUBLIC]
   - [USER]
   - [ADMIN]
3. Không được mô tả lẫn lộn User và Admin trong cùng một flow.
4. Mọi màn hình phải nêu:
   - actor
   - route
   - mục tiêu
   - dữ liệu chính
   - API gọi
   - quyền truy cập
   - trạng thái loading/empty/error
- responsive behavior
5. Mọi phần code phải có:
   - validation
   - error handling
   - loading state
   - empty state
   - bảo mật UI cơ bản
   - route guard
6. Nếu có tính năng bị khóa theo gói:
   - phải thể hiện paywall/upgrade state ở UI
7. Mọi màn hình analytics phải tính đến Chart.js và khả năng hiển thị tốt trên mobile.
8. Với mobile:
   - danh sách lớn ưu tiên card/list
   - filter ưu tiên bottom sheet / drawer / accordion
   - biểu đồ ít nhiễu, dễ đọc
9. Với desktop:
   - ưu tiên sidebar
   - bảng dữ liệu đầy đủ hơn
   - filter hiển thị ngang nếu hợp lý
10. Không dùng UI library ngoài TailwindCSS trừ khi thật cần thiết.
11. Mặc định triển khai P0 + P1 của MVP trước; P2 chỉ mô tả nếu không được yêu cầu code.

========================================
ĐẦU RA BẮT BUỘC
========================================
Hãy trả lời đúng cấu trúc dưới đây:

1) Tóm tắt hiểu bài
- Tóm tắt lại hệ thống
- Tách rõ phạm vi [PUBLIC], [USER], [ADMIN]
- Tóm tắt đúng business flow liên quan FE
- Nêu rõ bạn hiểu User và Admin là 2 actor khác nhau

2) Task List
- Luôn tạo checklist công việc FE trước khi làm
- Checklist phải có thứ tự thực hiện hợp lý
- Không hỏi lại

3) Giả định
- Chỉ nêu các giả định tối thiểu cần thiết
- Không làm thay đổi nghiệp vụ cốt lõi

4) Biên giới trách nhiệm FE
- Những gì FE chịu trách nhiệm
- Những gì phụ thuộc BE
- Những gì là shared contract

5) Sitemap + Route map chi tiết
Tách rõ:
- [PUBLIC]
- [USER]
- [ADMIN]

Với mỗi route phải có:
- Route ID
- Path
- Actor
- Layout
- Guard
- Plan restriction nếu có
- Mục tiêu màn hình
- API phụ thuộc
- Navigation placement:
  + Mobile Bottom Navigation / More / Subpage
  + Desktop Sidebar Group / Sidebar Item / Submenu

6) Thiết kế layout và navigation
Tách rõ:
- [PUBLIC LAYOUT]
- [USER LAYOUT]
- [ADMIN LAYOUT]

Bắt buộc phân tích:
- Mobile Bottom Navigation của User
- Mobile Bottom Navigation của Admin
- Desktop Sidebar của User
- Desktop Sidebar của Admin
- Màn nào là tab chính
- Màn nào nằm trong “More”
- Màn nào là detail page
- Màn nào là operational page

7) Danh sách màn hình triển khai FE theo ưu tiên
Tách 3 bảng:
- [PUBLIC SCREENS]
- [USER SCREENS]
- [ADMIN SCREENS]

Mỗi màn hình phải có:
- Screen ID
- Tên màn hình
- Priority: P0 / P1 / P2
- Actor
- Route
- Mục tiêu
- Dữ liệu hiển thị chính
- API dùng
- Component chính
- Responsive note

8) Wireframe text chi tiết
Bắt buộc tạo wireframe text cho các màn hình P0 + P1, tách rõ:
- [USER][MOBILE]
- [USER][DESKTOP]
- [ADMIN][MOBILE]
- [ADMIN][DESKTOP]

Mỗi wireframe phải mô tả:
- Header / Top bar
- Bottom Navigation hoặc Sidebar
- CTA chính / CTA phụ
- Search / filter / sort
- Table hay card/list
- Khu analytics/chart
- Empty state
- Error state
- Loading state
- Success feedback
- Upgrade/paywall state nếu có
- Modal / Drawer / Sheet / Dropdown nếu có
- Danger zone nếu có

9) Kiến trúc component FE
Tách rõ:
- [SHARED COMPONENTS]
- [USER COMPONENTS]
- [ADMIN COMPONENTS]

Với mỗi component phải có:
- Component ID
- Tên
- Mục đích
- Props chính
- Emits/callback chính
- Màn hình sử dụng
- Responsive notes

Ưu tiên mô tả các component như:
- AppShell
- BottomNav
- Sidebar
- StatCard
- ChartCard
- DataCard
- ResponsiveTable
- FilterBar
- SearchBox
- Pagination
- EmptyState
- ErrorState
- LoadingSkeleton
- UpgradeBanner
- ConfirmModal
- DangerActionModal
- PlanBadge
- LinkStatusBadge

10) Pinia stores và state management
Tách rõ:
- [SHARED STORES]
- [USER STORES]
- [ADMIN STORES]

Với mỗi store phải có:
- Store ID
- State
- Getters
- Actions
- API phụ thuộc
- Màn hình sử dụng
- Cache tạm trên FE nếu có

Ưu tiên các store:
- authStore
- sessionStore
- uiStore
- linkStore
- analyticsStore
- billingStore
- profileStore
- adminOverviewStore
- adminUserStore
- adminLinkStore
- adminReportStore
- adminOpsStore

11) API integration matrix cho FE
Tách rõ:
- [PUBLIC API DEPENDENCIES]
- [USER API DEPENDENCIES]
- [ADMIN API DEPENDENCIES]

Với mỗi API mà FE cần dùng, hãy nêu:
- API ID
- Endpoint dự kiến
- Method
- Màn hình gọi
- Request payload
- Response payload
- Auth requirement
- Error cases
- Retry behavior
- Cách map dữ liệu lên UI
- Phần nào FE phải chờ BE chốt

12) Form, validation, guard và permission
Phải phân tích rõ:
- Login/Register/Forgot Password/Reset Password
- Create Link/Edit Link
- Billing/Upgrade
- Profile/Security
- Admin actions: disable user, change plan, disable link...

Nêu rõ:
- client-side validation
- server error mapping
- auth guard
- role guard
- paywall guard
- route-level access
- component-level access nếu cần

13) Chart.js mapping
Phân tích các chart nên dùng:
- line chart
- bar chart
- doughnut chart
- area-style line nếu hợp lý

Chỉ rõ:
- chart nào cho User dashboard
- chart nào cho Link analytics
- chart nào cho Admin overview
- chart nào không nên nhồi trên mobile

14) Cấu trúc thư mục FE
Hãy đề xuất file tree thực tế cho Vue 3 + Vite project.
Bắt buộc có:
- src/app
- src/router
- src/layouts
- src/pages
- src/components
- src/modules
- src/stores
- src/composables
- src/services
- src/types
- src/utils
- src/constants
- src/guards
- src/charts
- src/assets

Tách rõ phần:
- shared
- user
- admin
- public

15) Kế hoạch triển khai FE Sprint 1
Hãy chia rõ:
- P0 phải code ngay
- P1 code tiếp theo
- P2 để sau

Với Sprint 1, hãy ưu tiên đủ để chạy end-to-end:
- đăng ký
- đăng nhập
- dashboard user cơ bản
- tạo link
- danh sách link
- link detail / analytics basic
- billing/plan basic
- admin login
- admin overview basic
- admin users basic
- admin links basic

16) File tree triển khai thực tế
Xuất file tree copy-paste được.

17) Code theo từng file
Bắt buộc:
- Code theo từng file
- Có comment vừa đủ
- Có type/interface rõ
- Có validation cơ bản
- Có error handling
- Có loading state
- Có responsive class Tailwind
- Có route guard
- Có Pinia integration
- Có API service layer

Nếu quá dài, hãy ưu tiên code:
- router
- layouts
- auth flow
- user dashboard
- create link
- link list
- link detail/analytics basic
- admin overview
- admin users list
- admin links list
- shared components quan trọng
- stores quan trọng
- API client quan trọng

18) Tiêu chí hoàn thành
Phải có checklist “Done”.
Ví dụ:
- User và Admin tách route rõ ràng
- Mobile có Bottom Navigation
- Desktop có Sidebar
- Các màn P0 dùng được
- Có loading/empty/error
- Có paywall state
- Không có màn hình trộn sai quyền

19) Cách kiểm thử nhanh
Phải nêu:
- cách chạy local
- env cần có
- dữ liệu mock cần có
- các luồng test nhanh
- các case lỗi quan trọng

20) Quy tắc cực kỳ quan trọng
- Không đổi nghiệp vụ nếu đặc tả không cho phép
- Không trộn User/Admin
- Không viết chung một layout cho mọi actor nếu trải nghiệm khác nhau
- Không bỏ qua responsive
- Không bỏ qua trạng thái lỗi/rỗng/tải
- Không tự tiện thêm tính năng ngoài đặc tả
- Không dùng tên API mơ hồ
- Không phụ thuộc vào backend chưa tồn tại mà không định nghĩa contract
- Nếu cần giả định, phải ghi rõ “Giả định”

========================================
MỤC TIÊU CUỐI
========================================
Mục tiêu của bạn là tạo ra một bản thiết kế + triển khai FE đủ rõ và đủ sạch để một team Vue 3 có thể bắt đầu code ngay mà không làm lệch mô tả nghiệp vụ.

========================================
TÀI LIỆU ĐẶC TẢ NGHIỆP VỤ GỐC
========================================
[DÁN TOÀN BỘ ĐẶC TẢ NGHIỆP VỤ Ở ĐÂY]
Yêu cầu đồng bộ bắt buộc:
- Hãy dùng cùng naming convention cho:
  + Route
  + API endpoint
  + DTO
  + Entity
  + Screen ID
  + API ID
- Nếu cần tự sinh ID, dùng format:
  + USR-SCR-001 / ADM-SCR-001
  + USR-API-001 / ADM-API-001 / PUB-API-001
  + ENT-001 / RULE-001
- Hãy luôn tạo bảng traceability:
  Screen -> API -> DTO -> Entity -> Rule
- Nếu phát hiện điểm mơ hồ, ghi vào mục “Giả định”, nhưng vẫn tiếp tục làm.
- Không được trả lời chung chung; phải đưa ra output có thể dùng để bắt đầu code ngay.