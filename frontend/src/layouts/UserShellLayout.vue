<script setup lang="ts">
import { computed } from 'vue'
import { RouterLink, RouterView, useRoute, useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth'

const route = useRoute()
const router = useRouter()
const authStore = useAuthStore()

const navItems = [
  { to: '/app/dashboard', label: 'Dashboard', shortLabel: 'Home' },
  { to: '/app/links', label: 'Links', shortLabel: 'Links' },
  { to: '/app/links/create', label: 'Create Link', shortLabel: 'Create' },
  { to: '/app/analytics', label: 'Analytics', shortLabel: 'Stats' },
  { to: '/app/billing', label: 'Billing', shortLabel: 'More' },
]

const title = computed(() => navItems.find((item) => route.path === item.to || route.path.startsWith(`${item.to}/`))?.label ?? 'Profile')

function logout() {
  authStore.logout()
  router.push('/login')
}
</script>

<template>
  <div class="app-shell">
    <aside class="sidebar desktop-only">
      <div class="brand-card">
        <div class="brand-mark">S</div>
        <div>
          <div class="brand-title">Shortlink SaaS</div>
          <p class="brand-subtitle">User Workspace</p>
        </div>
      </div>

      <nav class="nav-list">
        <RouterLink v-for="item in navItems" :key="item.to" :to="item.to" class="nav-link">
          {{ item.label }}
        </RouterLink>
        <RouterLink class="nav-link" to="/app/profile">Profile</RouterLink>
      </nav>

      <div class="sidebar-footer glass-dark">
        <div class="sidebar-user">
          <span class="sidebar-avatar">{{ authStore.user?.fullName?.slice(0, 1) }}</span>
          <div>
            <strong>{{ authStore.user?.fullName }}</strong>
            <p>Plan #{{ authStore.user?.currentPlanId }}</p>
          </div>
        </div>
        <button class="ghost-button ghost-button-light" @click="logout">Đăng xuất</button>
      </div>
    </aside>

    <div class="app-main">
      <section class="shell-banner header-gradient">
        <div class="shell-banner-content">
          <div>
            <p class="banner-overline">WEMAKE STYLE</p>
            <h1 class="banner-title">Shortlink Control Center</h1>
            <p class="banner-copy">Theo dõi chiến dịch, quản lý shortlink và xem tăng trưởng theo thời gian thực.</p>
          </div>
          <div class="banner-pills">
            <span class="banner-pill">Build OK</span>
            <span class="banner-pill">Plan #{{ authStore.user?.currentPlanId }}</span>
          </div>
        </div>
      </section>

      <header class="topbar">
        <div>
          <p class="eyebrow">User Workspace</p>
          <h2>{{ title }}</h2>
        </div>
        <div class="topbar-meta">
          <span class="badge">Plan #{{ authStore.user?.currentPlanId }}</span>
          <span class="muted strong-text">{{ authStore.user?.fullName }}</span>
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
