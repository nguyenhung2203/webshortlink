<script setup lang="ts">
import { computed } from 'vue'
import { RouterLink, RouterView, useRoute, useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth'
import { LayoutDashboard, Users, Link as LinkIcon, LogOut, ClipboardList } from 'lucide-vue-next'

const route = useRoute()
const router = useRouter()
const authStore = useAuthStore()

const navItems = [
  { to: '/admin', label: 'Tổng quan', shortLabel: 'Tổng quan', icon: LayoutDashboard },
  { to: '/admin/users', label: 'Khách hàng', shortLabel: 'Khách hàng', icon: Users },
  { to: '/admin/links', label: 'Quản lý Link', shortLabel: 'Links', icon: LinkIcon },
  { to: '/admin/audit', label: 'Nhật ký (Audit)', shortLabel: 'Nhật ký', icon: ClipboardList },
]

const title = computed(() => route.name?.toString().replaceAll('-', ' ') ?? 'Admin')

function logout() {
  authStore.logout()
  router.push('/auth/login')
}
</script>

<template>
  <div class="flex h-screen bg-surface-container-high text-on-surface overflow-hidden">
    <!-- Sidebar / Desktop -->
    <aside class="hidden md:flex w-64 flex-col bg-surface border-r border-outline-variant transition-all duration-300">
      <div class="h-14 flex items-center px-6 border-b border-outline-variant bg-slate-800 text-white">
        <span class="font-bold text-lg tracking-wider">Trang Quản Trị</span>
      </div>
      <nav class="flex-1 overflow-y-auto p-4 flex flex-col gap-1">
        <RouterLink 
          v-for="item in navItems" 
          :key="item.to" 
          :to="item.to" 
          class="flex items-center gap-3 px-3 py-2.5 rounded-lg text-sm font-medium transition-colors"
          :class="route.path === item.to || (item.to !== '/admin' && route.path.startsWith(item.to)) || (item.to === '/admin' && route.path === '/admin') ? 'bg-primary/20 text-primary' : 'text-on-surface hover:bg-surface-container-low'"
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
      <header class="h-14 flex shrink-0 items-center justify-between px-6 bg-surface border-b border-outline-variant shadow-sm z-10">
        <div>
          <p class="text-xs text-on-surface-variant hidden md:block">Quản trị Hệ thống</p>
          <h1 class="text-lg font-bold capitalize">{{ title }}</h1>
        </div>
        <div class="flex items-center gap-3">
          <span class="px-2 py-0.5 rounded bg-slate-800 text-white text-xs font-semibold">{{ authStore.user?.role }}</span>
          <span class="text-sm font-medium">{{ authStore.user?.email }}</span>
        </div>
      </header>

      <main class="flex-1 overflow-y-auto p-4 md:p-6 bg-surface-container-low">
        <RouterView />
      </main>

      <!-- Bottom Nav / Mobile -->
      <nav class="md:hidden flex h-14 shrink-0 bg-surface border-t border-outline-variant shadow-[0_-4px_6px_-1px_rgba(0,0,0,0.1)] z-10">
        <RouterLink 
          v-for="item in navItems" 
          :key="item.to" 
          :to="item.to" 
          class="flex-1 flex flex-col items-center justify-center gap-1 text-[10px] font-medium transition-colors"
          :class="route.path === item.to || (item.to !== '/admin' && route.path.startsWith(item.to)) || (item.to === '/admin' && route.path === '/admin') ? 'text-primary' : 'text-on-surface hover:text-primary'"
        >
          <component :is="item.icon" :size="20" />
          {{ item.shortLabel }}
        </RouterLink>
      </nav>
    </div>
  </div>
</template>
