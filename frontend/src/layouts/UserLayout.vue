<script setup lang="ts">
import { computed } from 'vue'
import { RouterLink, RouterView, useRoute, useRouter } from 'vue-router'
import { Home, Link as LinkIcon, BarChart2, CreditCard, User, LogOut } from 'lucide-vue-next'
import { useAuthStore } from '@/stores/auth'

const route = useRoute()
const router = useRouter()
const authStore = useAuthStore()

const navItems = [
  { to: '/app/dashboard', label: 'Tổng quan', shortLabel: 'Tổng quan', icon: Home },
  { to: '/app/links', label: 'Quản lý Link', shortLabel: 'Links', icon: LinkIcon },
  { to: '/app/analytics', label: 'Phân tích', shortLabel: 'Phân tích', icon: BarChart2 },
  { to: '/app/billing', label: 'Gói dịch vụ', shortLabel: 'Dịch vụ', icon: CreditCard },
  { to: '/app/profile', label: 'Tài khoản', shortLabel: 'Tài khoản', icon: User },
] as const

const title = computed(() => {
  const name = route.name?.toString() ?? ''

  switch (name) {
    case 'user-dashboard':
      return 'Tổng quan'
    case 'user-links':
      return 'Quản lý Link'
    case 'user-create-link':
      return 'Tạo Link mới'
    case 'user-link-detail':
      return 'Chi tiết Link'
    case 'user-analytics':
      return 'Phân tích'
    case 'user-billing':
      return 'Gói dịch vụ'
    case 'user-profile':
      return 'Tài khoản'
    default:
      return 'Khu vực người dùng'
  }
})

function isNavActive(path: string) {
  if (path === '/app/links') {
    return route.path === '/app/links' || route.path.startsWith('/app/links/')
  }

  return route.path === path
}

function logout() {
  authStore.logout()
  router.push('/auth/login')
}
</script>

<template>
  <div class="flex h-screen overflow-hidden bg-surface text-on-surface">
    <aside class="hidden w-64 flex-col border-r border-outline-variant bg-surface-container transition-all duration-300 md:flex">
      <div class="header-gradient flex h-14 items-center border-b border-outline-variant px-6 text-white">
        <span class="text-lg font-bold tracking-wider">Shortlink SaaS</span>
      </div>

      <nav class="flex flex-1 flex-col gap-1 overflow-y-auto p-4">
        <RouterLink
          v-for="item in navItems"
          :key="item.to"
          :to="item.to"
          class="flex items-center gap-3 rounded-lg px-3 py-2.5 text-sm font-medium transition-colors"
          :class="isNavActive(item.to) ? 'bg-primary/10 text-primary' : 'text-on-surface-variant hover:bg-surface-container-high hover:text-on-surface'"
        >
          <component :is="item.icon" :size="20" />
          {{ item.label }}
        </RouterLink>
      </nav>

      <div class="border-t border-outline-variant p-4">
        <button
          class="flex w-full items-center gap-3 rounded-lg px-3 py-2.5 text-sm font-medium text-danger transition-colors hover:bg-danger/10"
          @click="logout"
        >
          <LogOut :size="20" />
          Đăng xuất
        </button>
      </div>
    </aside>

    <div class="flex min-w-0 flex-1 flex-col">
      <header class="z-10 flex h-14 shrink-0 items-center justify-between border-b border-outline-variant bg-surface-container px-6 shadow-sm">
        <div>
          <p class="hidden text-xs text-on-surface-variant md:block">Khu vực người dùng</p>
          <h1 class="text-lg font-bold">{{ title }}</h1>
        </div>
        <div class="flex items-center gap-3">
          <span class="rounded-full bg-primary/10 px-2 py-0.5 text-xs font-semibold text-primary">
            Plan #{{ authStore.user?.currentPlanId || 'Basic' }}
          </span>
          <span class="text-sm font-medium">{{ authStore.user?.fullName || authStore.user?.email }}</span>
        </div>
      </header>

      <main class="flex-1 overflow-y-auto bg-surface p-4 md:p-6">
        <RouterView />
      </main>

      <nav class="z-10 flex h-14 shrink-0 border-t border-outline-variant bg-surface-container shadow-[0_-4px_6px_-1px_rgba(0,0,0,0.1)] md:hidden">
        <RouterLink
          v-for="item in navItems"
          :key="item.to"
          :to="item.to"
          class="flex flex-1 flex-col items-center justify-center gap-1 text-[10px] font-medium transition-colors"
          :class="isNavActive(item.to) ? 'text-primary' : 'text-on-surface-variant hover:text-on-surface'"
        >
          <component :is="item.icon" :size="20" />
          {{ item.shortLabel }}
        </RouterLink>
      </nav>
    </div>
  </div>
</template>
