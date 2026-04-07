<template>
  <div class="min-h-screen bg-surface flex items-center justify-center p-4">
    <section class="max-w-3xl w-full bg-surface-container rounded-2xl shadow-xl border border-outline-variant p-8 md:p-12">
      <p class="text-primary font-bold uppercase tracking-wider text-sm mb-2">Shortlink SaaS MVP</p>
      <h1 class="text-3xl md:text-5xl font-extrabold text-on-surface mb-6 leading-tight">Rút gọn link, theo dõi click, quản trị gói dịch vụ trong một hệ thống.</h1>
      <p class="text-on-surface-variant text-lg mb-8 leading-relaxed">
        Bản code hiện tại ưu tiên đi hết luồng sản phẩm: đăng nhập, tạo link, xem analytics,
        nâng cấp gói và quản trị admin.
      </p>

      <div class="flex flex-wrap gap-4 mt-6">
        <WxButton variant="primary" to="/auth/login" size="lg">Đăng nhập User</WxButton>
        <WxButton variant="cta" to="/auth/admin" size="lg">Đăng nhập Admin</WxButton>
        <WxButton variant="secondary" to="/auth/register" size="lg">Tạo tài khoản</WxButton>
      </div>

      <div class="grid grid-cols-1 md:grid-cols-2 gap-6 mt-12 bg-surface-container-low p-6 rounded-xl border border-outline-variant">
        <div>
          <h3 class="font-bold text-lg mb-2 text-on-surface">👉 User demo (Khách hàng)</h3>
          <p class="text-on-surface-variant font-mono text-sm"><strong>Email:</strong> user@demo.local</p>
          <p class="text-on-surface-variant font-mono text-sm"><strong>Password:</strong> Demo123!</p>
        </div>
        <div>
          <h3 class="font-bold text-lg mb-2 text-on-surface">👉 Admin demo (Quản trị viên)</h3>
          <p class="text-on-surface-variant font-mono text-sm"><strong>Email:</strong> admin@demo.local</p>
          <p class="text-on-surface-variant font-mono text-sm"><strong>Password:</strong> Admin123!</p>
        </div>
      </div>
    </section>
  </div>
</template>

<script setup lang="ts">
import { RouterLink } from 'vue-router'
import WxButton from '@/components/ui/WxButton.vue'
</script>
