<script setup lang="ts">
import { computed } from 'vue'
import { RouterLink, RouterView, useRoute, useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth'
import { Home, Link as LinkIcon, PlusCircle, BarChart2, CreditCard, User, LogOut } from 'lucide-vue-next'

const route = useRoute()
const router = useRouter()
const authStore = useAuthStore()

const navItems = [
  { to: '/app/dashboard', label: 'Tổng quan', shortLabel: 'Tổng quan', icon: Home },
  { to: '/app/links', label: 'Quản lý Link', shortLabel: 'Links', icon: LinkIcon },
  { to: '/app/links/create', label: 'Tạo Link mới', shortLabel: 'Tạo mới', icon: PlusCircle },
  { to: '/app/billing', label: 'Gói dịch vụ', shortLabel: 'Dịch vụ', icon: CreditCard },
  { to: '/app/profile', label: 'Tài khoản', shortLabel: 'Tài khoản', icon: User },
]

const title = computed(() => {
  const t = route.name?.toString().replaceAll('-', ' ') ?? 'User'
  if (t === 'user dashboard') return 'Tổng quan'
  if (t === 'user links') return 'Quản lý Link'
  if (t === 'user create link') return 'Tạo Link mới'
  if (t === 'user billing') return 'Gói dịch vụ'
  if (t === 'user profile') return 'Tài khoản'
  return t
})

function logout() {
  authStore.logout()
  router.push('/auth/login')
}
</script>

<template>
  <div class="flex h-screen bg-surface text-on-surface overflow-hidden">
    <!-- Sidebar / Desktop -->
    <aside class="hidden md:flex w-64 flex-col bg-surface-container border-r border-outline-variant transition-all duration-300">
      <div class="h-14 flex items-center px-6 border-b border-outline-variant header-gradient text-white">
        <span class="font-bold text-lg tracking-wider">Shortlink SaaS</span>
      </div>
      <nav class="flex-1 overflow-y-auto p-4 flex flex-col gap-1">
        <RouterLink 
          v-for="item in navItems" 
          :key="item.to" 
          :to="item.to" 
          class="flex items-center gap-3 px-3 py-2.5 rounded-lg text-sm font-medium transition-colors"
          :class="route.path === item.to ? 'bg-primary/10 text-primary' : 'text-on-surface-variant hover:bg-surface-container-high hover:text-on-surface'"
        >
          <component :is="item.icon" :size="20" />
          {{ item.label }}
        </RouterLink>
      </nav>
      <div class="p-4 border-t border-outline-variant">
        <button 
          class="flex w-full items-center gap-3 px-3 py-2.5 rounded-lg text-sm font-medium text-danger hover:bg-danger/10 transition-colors" 
          @click="logout"
        >
          <LogOut :size="20" />
          Đăng xuất
        </button>
      </div>
    </aside>

    <!-- Main Content -->
    <div class="flex-1 flex flex-col min-w-0">
      <header class="h-14 flex shrink-0 items-center justify-between px-6 bg-surface-container border-b border-outline-variant shadow-sm z-10">
        <div>
          <p class="text-xs text-on-surface-variant hidden md:block">Khu vực Người dùng</p>
          <h1 class="text-lg font-bold capitalize">{{ title }}</h1>
        </div>
        <div class="flex items-center gap-3">
          <span class="px-2 py-0.5 rounded-full bg-primary/10 text-primary text-xs font-semibold">Plan #{{ authStore.user?.currentPlanId || 'Basic' }}</span>
          <span class="text-sm font-medium">{{ authStore.user?.fullName || authStore.user?.email }}</span>
        </div>
      </header>

      <main class="flex-1 overflow-y-auto p-4 md:p-6 bg-surface">
        <RouterView />
      </main>

      <!-- Bottom Nav / Mobile -->
      <nav class="md:hidden flex h-14 shrink-0 bg-surface-container border-t border-outline-variant shadow-[0_-4px_6px_-1px_rgba(0,0,0,0.1)] z-10">
        <RouterLink 
          v-for="item in navItems" 
          :key="item.to" 
          :to="item.to" 
          class="flex-1 flex flex-col items-center justify-center gap-1 text-[10px] font-medium transition-colors"
          :class="route.path === item.to ? 'text-primary' : 'text-on-surface-variant hover:text-on-surface'"
        >
          <component :is="item.icon" :size="20" />
          {{ item.shortLabel }}
        </RouterLink>
      </nav>
    </div>
  </div>
</template>
