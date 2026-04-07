<template>
  <div class="landing-shell">
    <section class="hero-card">
      <p class="eyebrow">Shortlink SaaS MVP</p>
      <h1>Rút gọn link, theo dõi click, quản trị gói dịch vụ trong một hệ thống.</h1>
      <p class="muted">
        Bản code hiện tại ưu tiên đi hết luồng sản phẩm: đăng nhập, tạo link, xem analytics,
        nâng cấp gói và quản trị admin.
      </p>

      <div class="hero-actions">
        <RouterLink class="primary-button" to="/login">Đăng nhập demo</RouterLink>
        <RouterLink class="secondary-button" to="/register">Tạo tài khoản</RouterLink>
      </div>

      <div class="credential-grid">
        <div class="panel">
          <h3>User demo</h3>
          <p><strong>Email:</strong> user@demo.local</p>
          <p><strong>Password:</strong> Demo123!</p>
        </div>
        <div class="panel">
          <h3>Admin demo</h3>
          <p><strong>Email:</strong> admin@demo.local</p>
          <p><strong>Password:</strong> Admin123!</p>
        </div>
      </div>
    </section>
  </div>
</template>

<script setup lang="ts">
import { RouterLink } from 'vue-router'
</script>
