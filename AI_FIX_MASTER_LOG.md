# AI FIX MASTER LOG - Web Shortlink SaaS

> **Mục đích:** Đây là file điều phối sửa lỗi, bổ sung tính năng và bàn giao liên tục cho nhiều AI/engineer làm nối tiếp nhau.
> 
> **Nguyên tắc:** AI nào sửa xong đến đâu thì **phải cập nhật ngay trong file này** để AI sau có thể đọc tiếp và làm tiếp, không bị đứt mạch.

---

## 1) Cách dùng file này

### Nếu bạn là AI/engineer tiếp theo

1. Đọc **toàn bộ file** từ trên xuống dưới trước khi sửa.
2. Không xóa baseline audit ban đầu.
3. Không sửa nghiệp vụ cốt lõi nếu không có chỉ đạo mới.
4. Khi nhận việc, hãy:
   - chọn `FIX-ID` ưu tiên cao nhất còn mở,
   - sửa code,
   - cập nhật lại trạng thái trong bảng backlog,
   - thêm báo cáo vào mục **Nhật ký phiên làm việc**,
   - cập nhật mục **Bàn giao cho AI tiếp theo**.
5. Mọi thay đổi phải nêu rõ:
   - đã sửa gì,
   - file nào bị chạm,
   - API nào thêm/sửa,
   - màn hình nào thêm/sửa,
   - migration/seed nào thêm/sửa,
   - test thủ công đã chạy,
   - phần nào còn tồn đọng.
6. Nếu chưa có bằng chứng thì ghi đúng là **Chưa có bằng chứng**.
7. Không đánh dấu `Done` nếu chỉ sửa UI mà chưa có logic BE tương ứng, hoặc ngược lại.
8. Với tính năng premium, chỉ được đánh dấu `Done` khi **cả FE gating và BE gating** đều đúng.
9. Không hard-code logic theo tên plan nếu có thể dùng feature flag/capability.
10. Phải giữ rõ ranh giới **[PUBLIC] / [USER] / [ADMIN] / [SHARED]**.

---

## 2) Nguồn sự thật của dự án

### 2.1. Đặc tả gốc
Hệ thống là web shortlink SaaS có:
- 2 actor chính: **User** và **Admin**
- 3 gói: **Thường / Pro / Plus**
- Tracking click chi tiết
- Admin dashboard và quản trị hệ thống
- Responsive 100%
- Mobile dùng **Bottom Navigation**
- Desktop dùng **Sidebar**
- Stack:
  - Backend: ASP.NET Core 8 Web API, EF Core, Identity, Redis, Cloudflare Turnstile
  - Frontend: Vue 3, Vite, TailwindCSS, Pinia, Chart.js
  - Database: SQL Server
  - Dùng **Code First**

### 2.2. Baseline audit hiện tại
Baseline dưới đây được tổng hợp từ lần audit gần nhất, dùng làm mốc xuất phát để fix tiếp.

---

## 3) Ràng buộc không được phá vỡ

### 3.1. Actor
- **User** và **Admin** là 2 actor nghiệp vụ riêng.
- Không trộn UI, route, permission, use case của User và Admin.

### 3.2. Responsive
- Mobile: bắt buộc dùng **Bottom Navigation**.
- Desktop/PC: bắt buộc dùng **Sidebar**.
- Danh sách lớn trên mobile phải ưu tiên **card/list**, không để table cuộn ngang vô tội vạ.

### 3.3. Kiến trúc kỹ thuật
- Backend: ASP.NET Core 8 Web API
- Database: SQL Server
- ORM: EF Core **Code First**
- Auth: ASP.NET Core Identity
- Cache/Queue: Redis
- Anti-bot: Cloudflare Turnstile

### 3.4. Phân gói
- Bắt buộc có logic Thường / Pro / Plus rõ ràng.
- Gating không được chỉ nằm ở FE.
- FE chỉ hỗ trợ UX/paywall; BE mới là nơi enforce cuối cùng.

---

## 4) Snapshot baseline hiện tại

## 4.1. Tỷ lệ hoàn thiện tổng quan

| Hạng mục | Mức độ hoàn thiện |
|---|---:|
| Tổng thể | 62% |
| User | 68% |
| Admin | 55% |
| Plan Thường | 40% |
| Plan Pro | 48% |
| Plan Plus | 35% |
| User mobile | 65% |
| User desktop | 72% |
| Admin mobile | 40% |
| Admin desktop | 58% |

## 4.2. Top thiếu sót quan trọng nhất hiện tại

1. Email verification có UI/API nhưng logic BE mâu thuẫn: đăng ký đã auto confirm.
2. Forgot/reset password có UI/API nhưng chưa có bằng chứng gửi email/token thật.
3. Feature gating theo plan còn rất mỏng; nhiều tính năng premium đang mở cho mọi gói hoặc chỉ chặn ở FE.
4. Link rules có API nhưng chưa có UI; đồng thời seed plan không có `targeting.advanced` nên API gần như không usable.
5. Admin UI thiếu các màn trọng yếu: user detail, link detail, reports, security/ops, plans/settings.
6. Custom domain có API nhưng thiếu màn quản lý domain phía FE; verify DNS chưa thật.
7. Payment history có API nhưng chưa có màn hình FE riêng.
8. Responsive Admin mobile chưa đạt: danh sách lớn còn table cuộn ngang.
9. Ops metrics trên admin overview còn hard-code, chưa phải telemetry thật.
10. Startup/seeding vẫn có rủi ro do cách seeder runtime chưa an toàn.

---

## 5) Mục tiêu hoàn thành tối thiểu

## 5.1. MVP đúng nghiệp vụ
Để được coi là **MVP đúng đặc tả**, tối thiểu phải đạt:

### [PUBLIC]
- Đăng ký
- Đăng nhập
- Verify email thực sự
- Forgot/reset password thực sự
- Pricing page
- Protected link password page
- Redirect error pages: not found / expired / disabled / click limit

### [USER]
- Dashboard
- Links list
- Create link
- Link detail
- Analytics cơ bản
- Billing / Plan
- Payment history
- Domains management
- Profile / Security
- Link rules UI cơ bản nếu BE đã expose rules
- Paywall rõ cho tính năng premium

### [ADMIN]
- Admin overview
- Users list
- User detail
- Links list
- Link detail
- Reports basic
- Security / Ops basic
- Audit logs
- Quản lý plan user

### [BACKEND]
- Auth chuẩn với verify email + reset password thật
- Plan feature đủ và enforce ở BE
- Redirect flow không block analytics
- Analytics async + aggregate stats
- Domains usable
- Admin API usable
- Code First migrations ổn định
- Redis cache/queue hoạt động

---

## 6) Quy tắc đánh dấu trạng thái

Dùng một trong các trạng thái sau cho từng `FIX-ID`:
- `TODO`
- `IN_PROGRESS`
- `BLOCKED`
- `DONE`
- `PARTIAL`
- `NOT_APPLICABLE`

### Chỉ được đánh dấu `DONE` khi có đủ bằng chứng
- FE route/màn hình nếu có UI
- BE API/service nếu có logic
- Permission đúng
- Manual test hoặc smoke test đã chạy
- Nếu liên quan plan: có cả FE gating và BE gating
- Nếu liên quan Code First: migration/seed/config đã cập nhật nếu cần

---

## 7) Master Fix Board

## 7.1. Critical blockers

| FIX-ID | Nhóm | Actor | Priority | Trạng thái | Mô tả ngắn | Đóng khi nào |
|---|---|---|---|---|---|---|
| FIX-001 | Auth | PUBLIC/USER | Critical | TODO | Verify email hiện hữu nhưng BE đang auto confirm khi register | Register không auto confirm; verify email flow chạy thật |
| FIX-002 | Auth | PUBLIC/USER | Critical | TODO | Forgot/reset password chưa có luồng gửi token/email thật | Có token delivery thật + FE hoàn tất flow |
| FIX-003 | Plans | SHARED | Critical | TODO | Feature matrix Thường/Pro/Plus chưa đủ và chưa thống nhất | Có bảng feature canonical + seed + mapping rõ |
| FIX-004 | Plan Gating | USER | Critical | TODO | Premium features chưa được enforce ở BE | Password/expiration/click-limit/custom-domain/analytics advanced bị chặn đúng theo plan |
| FIX-005 | Link Rules | USER | Critical | TODO | Rules API chưa usable, thiếu feature seed + thiếu update UI/BE | Rules usable end-to-end |
| FIX-006 | Admin Core | ADMIN | Critical | TODO | Thiếu các màn admin trọng yếu | Có user detail, link detail, reports, security/ops cơ bản |
| FIX-007 | Public UX | PUBLIC | Critical | TODO | Thiếu pricing, password page, redirect error pages | Public pages đầy đủ và route hoạt động |

## 7.2. High priority

| FIX-ID | Nhóm | Actor | Priority | Trạng thái | Mô tả ngắn | Đóng khi nào |
|---|---|---|---|---|---|---|
| FIX-008 | Domains | USER | High | TODO | Có API domain nhưng thiếu UI và verify DNS thật | Có domains screen + verify flow + default domain |
| FIX-009 | Billing UI | USER | High | TODO | Payment history có API nhưng thiếu màn FE | Có payment history screen usable |
| FIX-010 | Admin Mobile UX | ADMIN | High | TODO | Table mobile còn cuộn ngang, chưa card/list phù hợp | Admin mobile list usable thật |
| FIX-011 | User Links UX | USER | High | TODO | Links list thiếu search/filter/sort/pagination tốt | List usable ở mobile và desktop |
| FIX-012 | Admin Metrics | ADMIN | High | TODO | Ops metrics hard-code | Admin overview lấy telemetry thật |
| FIX-013 | Seeder Runtime | SHARED | High | TODO | Rủi ro seeding/runtime bootstrap | Seeder an toàn, idempotent, rõ ràng |
| FIX-014 | Admin Plan UI | ADMIN | High | TODO | BE có đổi plan user nhưng FE chưa có UI | Admin đổi plan user được từ UI |
| FIX-015 | Admin Detail UI | ADMIN | High | TODO | Thiếu user detail và link detail UI | 2 màn detail hoạt động |

## 7.3. Medium / next

| FIX-ID | Nhóm | Actor | Priority | Trạng thái | Mô tả ngắn | Đóng khi nào |
|---|---|---|---|---|---|---|
| FIX-016 | Auth UX | PUBLIC | Medium | TODO | FE chưa tận dụng refresh token/session tối ưu | Auto refresh/session restore rõ ràng |
| FIX-017 | Admin Search/Filter | ADMIN | Medium | TODO | List admin thiếu search/filter/sort | Usable tốt trên desktop/mobile |
| FIX-018 | Analytics Retention | USER/PLAN | Medium | TODO | Retention theo plan chưa rõ | Enforce retention theo plan |
| FIX-019 | Export/Reports | USER/ADMIN | Medium | TODO | Export/report nâng cao chưa có | Có theo scope đã chốt |
| FIX-020 | Anti-bot | PUBLIC/USER | Medium | TODO | Turnstile hiện chưa bật thật ở các điểm nhạy cảm | Register/login/forgot/create suspicious có verify |
| FIX-021 | Rate Limiting | SHARED | Medium | TODO | Chưa granular theo auth/user/admin/redirect | Có policies phù hợp |
| FIX-022 | Observability | ADMIN/OPS | Medium | TODO | Monitoring và dashboard ops còn mỏng | Có telemetry/log/health phù hợp |

---

## 8) Chi tiết từng fix item

## FIX-001 - Verify email thật, bỏ auto-confirm

- **Nhóm:** Auth
- **Actor:** [PUBLIC], [USER]
- **Priority:** Critical
- **Hiện trạng:**
  - Có UI/API verify email.
  - Nhưng đăng ký đang auto confirm hoặc để account active ngay, làm verify email gần như vô nghĩa.
- **Cần sửa:**
  - Register không được auto confirm email.
  - Phải tạo token verify thật.
  - Phải có gửi email hoặc ít nhất adapter/service gửi mail thật để flow khép kín.
  - FE verify page phải phản ánh đúng trạng thái thành công/thất bại/hết hạn token.
- **Chạm FE:** Register, VerifyEmailView, auth store, auth service.
- **Chạm BE:** Auth service/controller, email token generation, email sender, identity setup.
- **Điều kiện đóng:**
  - Register tạo account chưa confirm.
  - Verify endpoint confirm thành công.
  - Login bị chặn hoặc cảnh báo đúng nếu email chưa confirm.
  - Có manual test đầy đủ.
- **Cập nhật gần nhất:** Chưa sửa.

## FIX-002 - Forgot/reset password thật

- **Nhóm:** Auth
- **Actor:** [PUBLIC], [USER]
- **Priority:** Critical
- **Hiện trạng:**
  - Có UI/API nhưng chưa có bằng chứng email/token delivery thật.
- **Cần sửa:**
  - Tạo token reset chuẩn.
  - Gửi email/reset link thật hoặc tối thiểu cắm mail adapter có thể test local.
  - FE reset page phải tiêu thụ token đúng.
- **Chạm FE:** ForgotPasswordView, ResetPasswordView, auth service.
- **Chạm BE:** Auth controller/service, mail sender, token generation/validation.
- **Điều kiện đóng:**
  - Có full flow yêu cầu reset -> nhận token -> reset thành công.
  - Có case token sai/hết hạn.
- **Cập nhật gần nhất:** Chưa sửa.

## FIX-003 - Chuẩn hóa plan feature matrix

- **Nhóm:** Plans / Monetization
- **Actor:** [SHARED]
- **Priority:** Critical
- **Hiện trạng:**
  - Seed có một số feature nhưng thiếu nhiều feature spec.
  - Chưa có canonical matrix duy nhất làm nguồn sự thật.
- **Cần sửa:**
  - Chốt danh sách feature/capability chuẩn cho Thường / Pro / Plus.
  - Mỗi feature phải có key ổn định.
  - Seed plan_features đồng bộ với nghiệp vụ.
- **Gợi ý feature keys tối thiểu:**
  - `links.max_count`
  - `links.custom_slug`
  - `links.password_protection`
  - `links.expiration`
  - `links.click_limit`
  - `domains.custom`
  - `domains.max_count`
  - `analytics.advanced`
  - `analytics.retention_days`
  - `rules.targeting_advanced`
  - `reports.export`
  - `api.access`
  - `webhook.access`
- **Điều kiện đóng:**
  - Có bảng feature canonical.
  - Seed/migration cập nhật.
  - FE/BE đều đọc từ nguồn thống nhất.
- **Cập nhật gần nhất:** Chưa sửa.

## FIX-004 - Enforce plan gating ở backend

- **Nhóm:** Plan Gating
- **Actor:** [USER]
- **Priority:** Critical
- **Hiện trạng:**
  - Một số feature premium đang mở cho mọi plan hoặc chỉ bị chặn ở FE.
- **Cần sửa:**
  - BE phải chặn thật cho các tính năng premium.
  - FE chỉ hiển thị paywall và CTA nâng cấp.
- **Các feature cần enforce ngay:**
  - Password protection
  - Expiration
  - Click limit
  - Custom domain
  - Analytics advanced
  - Advanced rules/targeting
- **Điều kiện đóng:**
  - User plan thấp gọi API bị từ chối đúng.
  - FE hiển thị paywall đúng.
  - Có test ít nhất cho Free vs Pro/Plus.
- **Cập nhật gần nhất:** Chưa sửa.

## FIX-005 - Link Rules usable end-to-end

- **Nhóm:** Link Rules
- **Actor:** [USER]
- **Priority:** Critical
- **Hiện trạng:**
  - BE có rules API nhưng thiếu update rule.
  - FE chưa có rules UI.
  - Seed plan thiếu `rules.targeting_advanced` hoặc tương đương.
- **Cần sửa:**
  - Hoàn chỉnh API rules: get/create/update/delete nếu scope cần.
  - Có UI quản lý rule cho link.
  - Gắn gating theo plan phù hợp.
- **Điều kiện đóng:**
  - User phù hợp plan có thể tạo/sửa/xóa rule.
  - User plan thấp thấy paywall hợp lý.
- **Cập nhật gần nhất:** Chưa sửa.

## FIX-006 - Admin core screens

- **Nhóm:** Admin Console
- **Actor:** [ADMIN]
- **Priority:** Critical
- **Hiện trạng:**
  - Thiếu User Detail, Link Detail, Reports, Security/Ops cơ bản.
- **Cần sửa:**
  - Thêm route, page, service, store, UI cho các màn trọng yếu.
  - Tách rõ admin shell khỏi user shell.
- **Điều kiện đóng:**
  - Có route và UI usable cho các màn trên.
  - Có gắn API đúng actor `/api/admin/*`.
- **Cập nhật gần nhất:** Chưa sửa.

## FIX-007 - Public pages đầy đủ

- **Nhóm:** Public UX
- **Actor:** [PUBLIC]
- **Priority:** Critical
- **Hiện trạng:**
  - Thiếu pricing page.
  - Thiếu protected link password page FE.
  - Thiếu error pages cho redirect state.
- **Cần sửa:**
  - Pricing page
  - Password submit page
  - Redirect error pages: not found / expired / disabled / click limit
- **Điều kiện đóng:**
  - Public route đầy đủ.
  - Flow resolve/password/error khép kín.
- **Cập nhật gần nhất:** Chưa sửa.

## FIX-008 - Domains module end-to-end

- **Nhóm:** Domains
- **Actor:** [USER]
- **Priority:** High
- **Hiện trạng:**
  - Có API domains nhưng chưa có màn FE riêng.
  - Verify DNS chưa thật.
  - Chưa có default domain rõ ràng.
- **Cần sửa:**
  - Domains list/create/delete/verify/default ở FE.
  - DNS verify logic thực tế hơn.
  - Bổ sung default domain nếu chưa có.
- **Điều kiện đóng:**
  - User Pro/Plus quản lý domain được từ UI.
- **Cập nhật gần nhất:** Chưa sửa.

## FIX-009 - Payment history screen

- **Nhóm:** Billing UI
- **Actor:** [USER]
- **Priority:** High
- **Hiện trạng:**
  - Có API thanh toán nhưng chưa có màn FE riêng.
- **Cần sửa:**
  - Tạo screen payment history.
  - Nối API đúng.
  - Thêm empty/error/loading states.
- **Điều kiện đóng:**
  - User xem được lịch sử thanh toán từ UI.
- **Cập nhật gần nhất:** Chưa sửa.

## FIX-010 - Admin mobile UX

- **Nhóm:** Responsive / Admin UX
- **Actor:** [ADMIN]
- **Priority:** High
- **Hiện trạng:**
  - Users/Links list trên mobile vẫn thiên về table cuộn ngang.
- **Cần sửa:**
  - Mobile đổi sang card/list hoặc hybrid responsive table dùng được.
  - Filter/search không phá UX nhỏ màn hình.
- **Điều kiện đóng:**
  - Admin mobile thao tác users/links không cần cuộn ngang khó chịu.
- **Cập nhật gần nhất:** Chưa sửa.

## FIX-011 - User links list UX

- **Nhóm:** User UX
- **Actor:** [USER]
- **Priority:** High
- **Hiện trạng:**
  - List có core data nhưng thiếu search/filter/sort/pagination và empty state rõ.
- **Cần sửa:**
  - Nâng cấp trải nghiệm list cả mobile và desktop.
- **Điều kiện đóng:**
  - Links list usable cho nhiều dữ liệu.
- **Cập nhật gần nhất:** Chưa sửa.

## FIX-012 - Telemetry thật cho admin overview

- **Nhóm:** Admin Ops
- **Actor:** [ADMIN]
- **Priority:** High
- **Hiện trạng:**
  - Một số metrics đang hard-code.
- **Cần sửa:**
  - Lấy dữ liệu thật hoặc ẩn/đánh dấu rõ experimental nếu chưa có telemetry.
- **Điều kiện đóng:**
  - Không còn metric hard-code giả lập trên admin overview.
- **Cập nhật gần nhất:** Chưa sửa.

## FIX-013 - Seeder runtime an toàn

- **Nhóm:** Startup / Data
- **Actor:** [SHARED]
- **Priority:** High
- **Hiện trạng:**
  - Còn rủi ro ở bootstrap/seed trên môi trường sạch.
- **Cần sửa:**
  - Tách migration/seed rõ ràng, idempotent, an toàn runtime.
- **Điều kiện đóng:**
  - Project boot sạch trên DB mới mà không gây side effects nguy hiểm.
- **Cập nhật gần nhất:** Chưa sửa.

## FIX-014 - Admin đổi plan user từ UI

- **Nhóm:** Admin Billing/User Management
- **Actor:** [ADMIN]
- **Priority:** High
- **Hiện trạng:**
  - BE có endpoint đổi plan nhưng FE chưa có UI.
- **Cần sửa:**
  - Thêm action đổi plan trong admin users list hoặc user detail.
- **Điều kiện đóng:**
  - Admin đổi plan từ UI được và phản ánh đúng sau refresh.
- **Cập nhật gần nhất:** Chưa sửa.

## FIX-015 - Admin detail screens

- **Nhóm:** Admin Detail UI
- **Actor:** [ADMIN]
- **Priority:** High
- **Hiện trạng:**
  - Có API nhưng chưa có 2 màn detail chính.
- **Cần sửa:**
  - User detail screen
  - Link detail screen
- **Điều kiện đóng:**
  - Có route + page + data load + actions phù hợp.
- **Cập nhật gần nhất:** Chưa sửa.

---

## 9) Coverage checklist theo actor

## 9.1. [USER]

| Hạng mục | Trạng thái baseline | Ghi chú |
|---|---|---|
| Register | PARTIAL | Verify email flow sai bản chất |
| Login | DONE | Core dùng được |
| Forgot password | PARTIAL | Thiếu email/token delivery thật |
| Reset password | PARTIAL | Phụ thuộc token delivery thật |
| Verify email | PARTIAL | Auto confirm làm mất ý nghĩa |
| View profile | DONE | Đủ MVP |
| Update profile | DONE | Cần validation tốt hơn nếu muốn polish |
| Change password | DONE | MVP đủ |
| View plans | DONE | Có API/UI |
| View subscription | DONE | Có |
| Upgrade plan | DONE | Demo-level |
| Payment history | PARTIAL | Có API, thiếu FE screen |
| Create link | DONE | Core flow chạy |
| Links list | PARTIAL | Thiếu UX cho nhiều dữ liệu |
| Link detail | DONE | Có |
| Edit link | PARTIAL | Thiếu UI edit rõ |
| Delete link | DONE | Có |
| Toggle link status | DONE | Có |
| Link rules create/delete | PARTIAL | Có API nhưng thiếu usable FE + plan |
| Link rules update | TODO | Chưa đủ |
| Analytics overview | DONE | Có |
| Analytics per link | DONE | Gating advanced còn sai |
| Domains | PARTIAL | Có API, thiếu FE đầy đủ |
| Verify domain | PARTIAL | Chưa verify DNS thật |
| Default domain | TODO | Chưa rõ |
| Paywall/upgrade state | PARTIAL | Mới rời rạc |

## 9.2. [ADMIN]

| Hạng mục | Trạng thái baseline | Ghi chú |
|---|---|---|
| Admin login | DONE | Có |
| Admin session | PARTIAL | FE chưa tận dụng nhiều |
| Overview dashboard | PARTIAL | Có nhưng metrics còn hard-code |
| Users list | DONE | Có core list |
| User detail | TODO | Thiếu UI |
| Lock/unlock user | DONE | Có |
| Change user plan | PARTIAL | BE có, FE thiếu |
| Links list toàn hệ thống | DONE | Có |
| Link detail | TODO | Thiếu UI |
| Enable/disable link | DONE | Có |
| Reports | PARTIAL | Có API basic, thiếu UI |
| Security/Ops | PARTIAL | Có API basic, thiếu UI |
| Audit logs | DONE | Có |
| Plans/Settings | TODO | Chưa có đủ |
| Admin mobile usable | PARTIAL | Còn table cuộn ngang |

---

## 10) Coverage checklist theo plan

| Tính năng | Thường | Pro | Plus | Baseline | Việc cần làm |
|---|---|---|---|---|---|
| Số lượng link | Có giới hạn | Cao hơn | Cao nhất | PARTIAL | Xác minh FE/BE thống nhất |
| Custom domain | Không | Có | Có nhiều hơn | PARTIAL | Bổ sung FE + verify + default |
| Password protection | Không nên mở | Có | Có | TODO | Chặn ở BE |
| Expiration | Không nên mở | Có | Có | TODO | Chặn ở BE |
| Click limit | Không nên mở | Có | Có | TODO | Chặn ở BE |
| Analytics advanced | Không | Có | Có | TODO | Chặn response hoặc field ở BE |
| Analytics retention | Ngắn | Dài hơn | Dài nhất | TODO | Enforce thật |
| Geo/device targeting | Không | Có | Có nâng cao | TODO | Seed feature + rules UI/BE |
| Export data | Không | Có | Có nâng cao | TODO | Chưa có |
| API/Webhook | Không | Có giới hạn | Có nhiều hơn | TODO | Chưa có |
| QR code | Tùy chiến lược | Có | Có | TODO | Chưa thấy bằng chứng |

---

## 11) Screen/route checklist kỳ vọng

## 11.1. [PUBLIC]

| Screen | Baseline | Ghi chú |
|---|---|---|
| Landing | Chưa có bằng chứng | Nếu ngoài scope thì ghi rõ |
| Pricing | TODO | Cần bổ sung |
| Login | DONE | Có |
| Register | DONE | Có |
| Forgot password | DONE | Có |
| Reset password | DONE | Có |
| Verify email | DONE | Có UI nhưng logic BE sai |
| Protected link password page | TODO | Thiếu |
| Redirect error page - Not Found | TODO | Thiếu |
| Redirect error page - Expired | TODO | Thiếu |
| Redirect error page - Disabled | TODO | Thiếu |
| Redirect error page - Click limit | TODO | Thiếu |

## 11.2. [USER]

| Screen | Baseline | Ghi chú |
|---|---|---|
| Dashboard | DONE | Có |
| Links List | PARTIAL | Thiếu UX nâng cao |
| Create Link | DONE | Có |
| Link Detail | DONE | Có |
| Link Analytics | DONE | Gating advanced chưa đúng |
| Domains | TODO | Thiếu màn riêng |
| Billing / Plan | DONE | Có |
| Payment History | TODO | Thiếu |
| Profile | DONE | Có |
| Security | DONE | Có |
| Link Rules | TODO | Thiếu |

## 11.3. [ADMIN]

| Screen | Baseline | Ghi chú |
|---|---|---|
| Admin Overview | PARTIAL | Metrics chưa thật |
| Admin Users | DONE | Có |
| Admin User Detail | TODO | Thiếu |
| Admin Links | DONE | Có |
| Admin Link Detail | TODO | Thiếu |
| Admin Reports | TODO | Thiếu UI |
| Admin Security/Ops | TODO | Thiếu UI |
| Admin Audit Logs | DONE | Có |
| Admin Plans/Settings | TODO | Thiếu |

---

## 12) API checklist kỳ vọng

## 12.1. [AUTH / PUBLIC]

| Endpoint/nhóm | Baseline | Ghi chú |
|---|---|---|
| Register | DONE | Logic verify email cần sửa |
| Login | DONE | Có |
| Refresh token | DONE | FE chưa tận dụng tối ưu |
| Logout | DONE | Có |
| Session | DONE | Có |
| Forgot password | PARTIAL | Thiếu email/token thật |
| Reset password | PARTIAL | Thiếu end-to-end thật |
| Verify email | PARTIAL | Auto confirm làm sai flow |
| Resolve short code | DONE | Có |
| Submit password for protected link | PARTIAL | BE có, FE page thiếu |
| Redirect endpoint | DONE | Có |

## 12.2. [USER API]

| Endpoint/nhóm | Baseline | Ghi chú |
|---|---|---|
| Profile | DONE | Có |
| Security change password | DONE | Có |
| Links CRUD | DONE/PARTIAL | Edit UI còn thiếu |
| Links status patch | DONE | Có |
| Link rules | PARTIAL | Thiếu update + UI |
| Analytics overview | DONE | Có |
| Analytics per link | DONE | Gating advanced sai |
| Plans | DONE | Namespace hơi lẫn |
| Subscription | DONE | Có |
| Payments | PARTIAL | FE thiếu screen |
| Billing upgrade | DONE | Demo only |
| Domains | PARTIAL | UI + DNS verify + default domain thiếu |

## 12.3. [ADMIN API]

| Endpoint/nhóm | Baseline | Ghi chú |
|---|---|---|
| Admin session | DONE | FE chưa dùng nhiều |
| Admin overview | PARTIAL | Metrics hard-code |
| Users list/detail | PARTIAL | Detail UI thiếu |
| User status change | DONE | Có |
| User plan change | PARTIAL | FE thiếu UI |
| Links list/detail | PARTIAL | Detail UI thiếu |
| Link enable/disable | DONE | Có |
| Reports basic | PARTIAL | UI thiếu |
| Security basic | PARTIAL | UI thiếu |
| Audit logs | DONE | Có |

---

## 13) Định nghĩa báo cáo sau mỗi lần sửa

Sau mỗi lần sửa, AI/engineer **phải thêm một block mới** vào mục **Nhật ký phiên làm việc** theo format này:

```md
### SESSION-XXX
- Người thực hiện:
- Thời gian:
- FIX-ID đã xử lý:
- Trạng thái sau khi sửa:
- Tóm tắt thay đổi:
- File FE đã sửa:
- File BE đã sửa:
- Migration/seed/config đã sửa:
- API thêm/sửa/xóa:
- Route/màn hình thêm/sửa/xóa:
- Test thủ công đã chạy:
- Kết quả test:
- Phần còn tồn đọng:
- Hướng bàn giao cho AI tiếp theo:
```

---

## 14) Nhật ký phiên làm việc

### SESSION-000 (Baseline audit)
- Người thực hiện: Auditor/Baseline
- Thời gian: Baseline trước khi bắt đầu đợt fix tiếp theo
- FIX-ID đã xử lý: Chưa fix, chỉ audit
- Trạng thái sau khi sửa: Không áp dụng
- Tóm tắt thay đổi: Thiết lập baseline coverage, xác định gap chính, lập backlog fix
- File FE đã sửa: Chưa có
- File BE đã sửa: Chưa có
- Migration/seed/config đã sửa: Chưa có
- API thêm/sửa/xóa: Chưa có
- Route/màn hình thêm/sửa/xóa: Chưa có
- Test thủ công đã chạy: Dùng bằng chứng audit có sẵn
- Kết quả test: Xác nhận dự án mới đạt mức demo hẹp, chưa đạt đầy đủ đặc tả
- Phần còn tồn đọng: Xem toàn bộ `FIX-ID`
- Hướng bàn giao cho AI tiếp theo:
  1. Xử lý `FIX-001` và `FIX-002` trước
  2. Sau đó chốt `FIX-003` và `FIX-004`
  3. Rồi làm `FIX-005`, `FIX-006`, `FIX-007`

---

## 15) Bàn giao cho AI tiếp theo

### Ưu tiên xử lý đề xuất

#### Đợt 1 - Blocker cứng
1. `FIX-001` - Verify email thật
2. `FIX-002` - Forgot/reset password thật
3. `FIX-003` - Chuẩn hóa feature matrix
4. `FIX-004` - Enforce plan gating ở BE

#### Đợt 2 - Đúng đặc tả nghiệp vụ
5. `FIX-005` - Link Rules usable
6. `FIX-006` - Admin core screens
7. `FIX-007` - Public pages đầy đủ
8. `FIX-008` - Domains end-to-end

#### Đợt 3 - Hoàn thiện UX/Admin
9. `FIX-009` - Payment history
10. `FIX-010` - Admin mobile UX
11. `FIX-011` - User links UX
12. `FIX-012` - Telemetry thật
13. `FIX-013` - Seeder runtime
14. `FIX-014` - Admin đổi plan user từ UI
15. `FIX-015` - Admin detail screens

### Cách chốt một fix đúng chuẩn

Một `FIX-ID` chỉ được đổi sang `DONE` khi có đủ:
- Code FE/BE tương ứng
- Manual test cơ bản
- Nếu có DB thay đổi: migration/seed/config tương ứng
- Nếu có actor khác nhau: route và permission đúng actor
- Nếu có plan: FE gating + BE gating cùng đúng
- Nếu có responsive: mobile + desktop đều dùng được

### Những lỗi hay gặp cần tránh khi sửa tiếp
- Chỉ sửa FE mà quên chặn BE
- Chỉ sửa BE mà quên thêm route/screen/CTA
- Thêm premium feature nhưng không map theo plan feature chuẩn
- Hard-code `planId > 1` hoặc tương đương
- Thêm admin UI nhưng lại gọi user API
- Để table admin mobile cuộn ngang quá khó dùng
- Dùng số giả lập/hard-code cho metric vận hành mà không gắn nhãn rõ
- Sửa auth nhưng không test lại register/login/verify/reset đủ vòng

---

## 16) Báo cáo tổng hợp nhanh để cập nhật sau mỗi đợt fix

> Sau mỗi đợt fix lớn, hãy cập nhật lại bảng này.

| Hạng mục | Baseline | Hiện tại | Ghi chú |
|---|---:|---:|---|
| Tổng thể | 62% | 62% | Chưa cập nhật sau baseline |
| User | 68% | 68% | Chưa cập nhật sau baseline |
| Admin | 55% | 55% | Chưa cập nhật sau baseline |
| Thường | 40% | 40% | Chưa cập nhật sau baseline |
| Pro | 48% | 48% | Chưa cập nhật sau baseline |
| Plus | 35% | 35% | Chưa cập nhật sau baseline |
| User mobile | 65% | 65% | Chưa cập nhật sau baseline |
| User desktop | 72% | 72% | Chưa cập nhật sau baseline |
| Admin mobile | 40% | 40% | Chưa cập nhật sau baseline |
| Admin desktop | 58% | 58% | Chưa cập nhật sau baseline |

---

## 17) Danh sách file dự kiến dễ bị chạm khi fix

> Đây là nhóm file/thư mục có khả năng phải sửa nhiều. AI sau cần tự đối chiếu với codebase thực tế.

### FE
- `src/router/*`
- `src/layouts/*`
- `src/views/public/*`
- `src/views/user/*`
- `src/views/admin/*`
- `src/stores/*`
- `src/services/*`
- `src/components/shared/*`
- `src/components/user/*`
- `src/components/admin/*`

### BE
- `Api/Controllers/*`
- `Application/*`
- `Infrastructure/*`
- `Persistence/*` hoặc `Data/*`
- `Domain/Entities/*`
- `Program.cs`
- `ApplicationDbContext.cs`
- `Configurations/*`
- `Migrations/*`
- `Seed/*` hoặc `ApplicationDbSeeder.cs`
- `Workers/*`

---

## 18) Kết luận vận hành của file này

File này là **sổ tay fix liên tục** cho dự án.

AI/engineer tiếp theo không nên bắt đầu bằng việc tự đoán dự án đang ở đâu. Hãy dùng chính file này để:
- biết current state,
- biết ưu tiên sửa,
- biết điều kiện đóng task,
- biết cần báo cáo lại ở đâu.

> **Quy tắc cuối cùng:** Không được sửa code xong rồi bỏ đi mà không cập nhật file này.
> AI sau chỉ làm nhanh và đúng nếu AI trước để lại dấu vết đủ rõ.
