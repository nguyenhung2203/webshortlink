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

const title = computed(() => route.name?.toString().replaceAll('-', ' ') ?? 'User')

function logout() {
  authStore.logout()
  router.push('/login')
}
</script>

<template>
  <div class="app-shell">
    <aside class="sidebar desktop-only">
      <div class="brand">Shortlink SaaS</div>
      <nav class="nav-list">
        <RouterLink v-for="item in navItems" :key="item.to" :to="item.to" class="nav-link">
          {{ item.label }}
        </RouterLink>
        <RouterLink class="nav-link" to="/app/profile">Profile</RouterLink>
      </nav>
      <button class="ghost-button" @click="logout">Đăng xuất</button>
    </aside>

    <div class="app-main">
      <header class="topbar">
        <div>
          <p class="eyebrow">User Workspace</p>
          <h1>{{ title }}</h1>
        </div>
        <div class="topbar-meta">
          <span class="badge">Plan #{{ authStore.user?.currentPlanId }}</span>
          <span>{{ authStore.user?.fullName }}</span>
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
