# ANTIGRAVITY_TASK.md

## Mục tiêu
Tích hợp bộ UI từ source **WeUI `vue-template`** vào project **`webshortlink/frontend`** để Antigravity có thể đọc đúng cấu trúc và triển khai code tiếp.

---

## Nguồn sử dụng

### UI source
- `weui-source/vue-template`

### Target project
- `webshortlink/frontend`

### Không dùng
- `weui-source/system 001`  
  Lý do: đây là bản **React**, không phù hợp với stack hiện tại của `webshortlink/frontend`.

---

## Kết luận kỹ thuật
Project `webshortlink/frontend` đang dùng:
- Vue 3
- TypeScript
- Vite
- Pinia

Bộ UI phù hợp để tích hợp là:
- `weui-source/vue-template`

Mục tiêu tích hợp là:
- dùng `vue-template` như **UI kit nguồn**
- chỉ lấy các phần **reusable**
- không bê toàn bộ page demo dư thừa
- refactor project đích theo hướng dễ mở rộng cho sản phẩm shortlink

---

## Yêu cầu bắt buộc cho Antigravity

1. **Chỉ dùng bản `vue-template`**
   - Không dùng `system 001`
   - Không copy component từ bản React

2. **Giữ nguyên stack của frontend**
   - Vue 3
   - TypeScript
   - Vite
   - Pinia

3. **Tích hợp theo kiểu UI kit**
   - Lấy component, layout, store, style cần thiết
   - Không bê nguyên toàn bộ template admin demo

4. **Source of truth cho UI**
   - `weui-source/vue-template/src`

5. **Target source**
   - `webshortlink/frontend/src`

---

## Các thư mục cần copy từ UI source

Copy từ:

`weui-source/vue-template/src/`

sang:

`webshortlink/frontend/src/`

### Ưu tiên lấy
- `components/`
- `layouts/`
- `stores/`
- `composables/`
- `types/`
- `assets/styles/`

### Đặc biệt quan trọng
- `components/ui/*`
- `components/layout/*`
- `layouts/DefaultLayout.vue`
- `layouts/AuthLayout.vue`
- `stores/toast.ts`
- `stores/theme.ts`
- `stores/sidebar.ts`
- `assets/styles/tokens.css`

---

## Không nên copy toàn bộ các page demo sau nếu chưa cần
Không ưu tiên bê nguyên các page demo như:
- mailbox
- gallery
- kanban
- ecommerce
- invoice
- các dashboard demo dư thừa
- các page showcase không liên quan trực tiếp đến shortlink product

Mục tiêu là giữ project gọn để Antigravity dễ đọc và code tiếp.

---

## Các component UI dự kiến sẽ dùng
Ưu tiên tận dụng các component `Wx*` từ UI source, ví dụ:
- `WxButton`
- `WxInput`
- `WxModal`
- `WxToast`
- `WxCard`
- `WxDataTable`
- `WxTabs`
- `WxDatePicker` nếu cần
- các layout/navigation component liên quan

---

## Việc cần refactor trong target project

### 1. `frontend/src/main.ts`
Cập nhật để:
- dùng `createPinia()`
- dùng `router`
- import `tokens.css`

Ví dụ mong muốn:

```ts
import { createApp } from 'vue'
import { createPinia } from 'pinia'
import App from './App.vue'
import router from './router'
import './assets/styles/tokens.css'

const app = createApp(App)
app.use(createPinia())
app.use(router)
app.mount('#app')
```

---

### 2. `frontend/src/App.vue`
Biến thành shell tối giản:

```vue
<script setup lang="ts">
import WxToast from '@/components/ui/WxToast.vue'
</script>

<template>
  <RouterView />
  <WxToast />
</template>
```

---

### 3. `frontend/src/router/index.ts`
Refactor router để dùng layout của UI kit.

Yêu cầu tối thiểu:
- `DefaultLayout` cho app chính
- `AuthLayout` cho trang đăng nhập

Ví dụ cấu trúc mong muốn:

```ts
import { createRouter, createWebHistory } from 'vue-router'

const DefaultLayout = () => import('@/layouts/DefaultLayout.vue')
const AuthLayout = () => import('@/layouts/AuthLayout.vue')

const router = createRouter({
  history: createWebHistory(),
  routes: [
    {
      path: '/',
      component: DefaultLayout,
      children: [
        {
          path: '',
          name: 'dashboard',
          component: () => import('@/views/DashboardView.vue'),
          meta: { title: 'Dashboard' },
        },
        {
          path: 'links',
          name: 'links',
          component: () => import('@/views/LinksView.vue'),
          meta: { title: 'Links' },
        },
      ],
    },
    {
      path: '/auth',
      component: AuthLayout,
      children: [
        {
          path: 'login',
          name: 'login',
          component: () => import('@/views/LoginView.vue'),
          meta: { title: 'Login' },
        },
      ],
    },
  ],
})

router.beforeEach((to) => {
  document.title = `${to.meta.title || 'Page'} | WebShortLink`
})

export default router
```

---

## Các page thật cần tạo cho dự án
Tạo ít nhất 3 page thật, không phải page demo vô nghĩa:

1. `DashboardView.vue`
2. `LinksView.vue`
3. `LoginView.vue`

### Yêu cầu riêng cho `LinksView`
Phải dùng các component từ UI kit, ví dụ:
- `WxButton`
- `WxInput`
- `WxCard`
- `WxDataTable`
- `WxModal` hoặc `WxToast` nếu phù hợp

---

## Cấu trúc thư mục mong muốn sau khi tích hợp

```txt
frontend/src/
  assets/
    styles/
      tokens.css
  components/
    ui/
    layout/
  layouts/
    DefaultLayout.vue
    AuthLayout.vue
  views/
    DashboardView.vue
    LinksView.vue
    LoginView.vue
  router/
    index.ts
  stores/
  composables/
  types/
  App.vue
  main.ts
```

---

## Dependency cần kiểm tra và bổ sung
Antigravity cần kiểm tra và cài thêm nếu thiếu:

- `lucide-vue-next`
- `@floating-ui/vue`
- `tailwindcss`
- `@tailwindcss/vite`

Ngoài ra cần kiểm tra:
- alias `@` trong `vite.config.ts`
- cấu hình Tailwind
- cấu hình import style/token
- tương thích version Vue/Pinia/Vite

---

## Nguyên tắc tích hợp
1. Ưu tiên **khung + component + style tokens**
2. Không bê nguyên toàn bộ demo pages
3. Không làm code rác chỉ để “cho chạy”
4. Giữ code sạch, có thể mở rộng tiếp cho shortlink SaaS
5. Cấu trúc phải đủ rõ để AI khác tiếp tục làm được

---

## Kết quả mong muốn
Sau khi hoàn tất, project phải đạt được các điều kiện sau:

- chạy được bằng `npm run dev`
- giao diện đồng bộ theo UI kit
- có layout rõ ràng
- có router rõ ràng
- có 3 page thật:
  - Dashboard
  - Links
  - Login
- page Links đã dùng component `Wx*`
- codebase gọn, readable, dễ phát triển tiếp

---

## Checklist implementation cho Antigravity

### Bước 1
Đọc 2 source:
- `weui-source/vue-template`
- `webshortlink/frontend`

### Bước 2
Xác nhận stack tương thích:
- Vue 3
- TypeScript
- Vite
- Pinia

### Bước 3
Copy phần reusable từ `vue-template/src` sang `webshortlink/frontend/src`

### Bước 4
Refactor:
- `main.ts`
- `App.vue`
- `router/index.ts`

### Bước 5
Tạo các page:
- `DashboardView.vue`
- `LinksView.vue`
- `LoginView.vue`

### Bước 6
Gắn UI kit vào page thật, đặc biệt là `LinksView.vue`

### Bước 7
Kiểm tra dependency và sửa lỗi build nếu có

### Bước 8
Đảm bảo app chạy được bằng:
```bash
npm run dev
```

---

## Prompt ngắn để chạy lại cho Antigravity
Có thể dùng nguyên văn prompt sau:

```text
Đọc 2 source code trong workspace:
1. weui-source/vue-template
2. webshortlink/frontend

Nhiệm vụ:
- Tích hợp UI kit từ weui-source/vue-template vào webshortlink/frontend
- Chỉ dùng bản Vue template, không dùng bản React
- Copy các phần reusable cần thiết: components, layouts, stores, composables, types, tokens.css
- Refactor App.vue, main.ts, router/index.ts
- Tạo giao diện thật cho 3 trang: Dashboard, Links, Login
- Dùng các component Wx* trong page Links
- Không bê toàn bộ page demo thừa
- Đảm bảo project chạy được bằng npm run dev
- Giữ code sạch, dễ mở rộng
```

---

## Ghi chú cuối
Nếu cần mở rộng tiếp sau bước tích hợp đầu tiên, ưu tiên thứ tự:
1. Links CRUD
2. Analytics UI
3. User profile
4. Billing/Plan
5. Admin console tách riêng sau

MVP trước mắt chỉ cần tích hợp UI kit sạch, đúng stack, và dựng được khung frontend chuẩn để code tiếp.
