<script setup lang="ts">
import { computed } from 'vue'
import { RouterLink, RouterView, useRoute, useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth'

const route = useRoute()
const router = useRouter()
const authStore = useAuthStore()

const navItems = [
  { to: '/admin', label: 'Overview', shortLabel: 'Overview' },
  { to: '/admin/users', label: 'Users', shortLabel: 'Users' },
  { to: '/admin/links', label: 'Links', shortLabel: 'Links' },
  { to: '/admin/audit', label: 'Audit', shortLabel: 'More' },
]

const title = computed(() => navItems.find((item) => route.path === item.to || route.path.startsWith(`${item.to}/`))?.label ?? 'Admin')

function logout() {
  authStore.logout()
  router.push('/login')
}
</script>

<template>
  <div class="app-shell admin-theme">
    <aside class="sidebar desktop-only">
      <div class="brand-card">
        <div class="brand-mark">A</div>
        <div>
          <div class="brand-title">Admin Portal</div>
          <p class="brand-subtitle">Operations Console</p>
        </div>
      </div>

      <nav class="nav-list">
        <RouterLink v-for="item in navItems" :key="item.to" :to="item.to" class="nav-link">
          {{ item.label }}
        </RouterLink>
      </nav>

      <div class="sidebar-footer glass-dark">
        <div class="sidebar-user">
          <span class="sidebar-avatar">{{ authStore.user?.email?.slice(0, 1)?.toUpperCase() }}</span>
          <div>
            <strong>{{ authStore.user?.role }}</strong>
            <p>{{ authStore.user?.email }}</p>
          </div>
        </div>
        <button class="ghost-button ghost-button-light" @click="logout">Đăng xuất</button>
      </div>
    </aside>

    <div class="app-main">
      <section class="shell-banner header-gradient admin-banner">
        <div class="shell-banner-content">
          <div>
            <p class="banner-overline">OPERATIONS</p>
            <h1 class="banner-title">System Governance Console</h1>
            <p class="banner-copy">Theo dõi tăng trưởng, kiểm soát abuse và vận hành toàn hệ thống SaaS trong một màn hình.</p>
          </div>
          <div class="banner-pills">
            <span class="banner-pill">Security</span>
            <span class="banner-pill">Audit Ready</span>
          </div>
        </div>
      </section>

      <header class="topbar">
        <div>
          <p class="eyebrow">System Admin</p>
          <h2>{{ title }}</h2>
        </div>
        <div class="topbar-meta">
          <span class="badge badge-dark">{{ authStore.user?.role }}</span>
          <span class="muted strong-text">{{ authStore.user?.email }}</span>
        </div>
      </header>

      <main class="page-container">
        <RouterView />
      </main>

      <nav class="bottom-nav mobile-only">
        <RouterLink v-for="item in navItems" :key="item.to" :to="item.to" class="bottom-nav-link">
          {{ item.shortLabel }}
        </RouterLink>
      </nav>
    </div>
  </div>
</template>
