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

const title = computed(() => route.name?.toString().replaceAll('-', ' ') ?? 'Admin')

function logout() {
  authStore.logout()
  router.push('/login')
}
</script>

<template>
  <div class="app-shell admin-theme">
    <aside class="sidebar desktop-only">
      <div class="brand">Admin Portal</div>
      <nav class="nav-list">
        <RouterLink v-for="item in navItems" :key="item.to" :to="item.to" class="nav-link">
          {{ item.label }}
        </RouterLink>
      </nav>
      <button class="ghost-button" @click="logout">Đăng xuất</button>
    </aside>

    <div class="app-main">
      <header class="topbar">
        <div>
          <p class="eyebrow">System Admin</p>
          <h1>{{ title }}</h1>
        </div>
        <div class="topbar-meta">
          <span class="badge badge-dark">{{ authStore.user?.role }}</span>
          <span>{{ authStore.user?.email }}</span>
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
