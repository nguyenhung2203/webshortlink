<script setup lang="ts">
import { computed, onMounted, ref, onUnmounted } from 'vue'
import { RouterLink, RouterView, useRoute, useRouter } from 'vue-router'
import { Home, Link as LinkIcon, BarChart2, CreditCard, User, LogOut, Globe, Search as SearchIcon, Command, ArrowRight } from 'lucide-vue-next'
import { useAuthStore } from '@/stores/auth'

const route = useRoute()
const router = useRouter()
const authStore = useAuthStore()

onMounted(() => {
  authStore.refreshSession()
})

const navItems = [
  { to: '/app/dashboard', label: 'Tổng quan', shortLabel: 'Tổng quan', icon: Home },
  { to: '/app/links', label: 'Quản lý Link', shortLabel: 'Links', icon: LinkIcon },
  { to: '/app/analytics', label: 'Phân tích', shortLabel: 'Phân tích', icon: BarChart2 },
  { to: '/app/domains', label: 'Tên miền', shortLabel: 'Domain', icon: Globe },
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
    case 'user-checkout':
      return 'Thanh toán VietQR'
    case 'user-payments':
      return 'Lịch sử thanh toán'
    case 'user-domains':
      return 'Tên miền riêng'
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

async function logout() {
  await authStore.logout()
  router.push('/auth/login')
}

// === COMMAND PALETTE FEATURE ===
const isCommandOpen = ref(false)
const commandSearch = ref('')
const commandInputRef = ref<HTMLInputElement | null>(null)

function handleKeyDown(e: KeyboardEvent) {
  if ((e.ctrlKey || e.metaKey) && e.key.toLowerCase() === 'k') {
    e.preventDefault()
    isCommandOpen.value = !isCommandOpen.value
    if (isCommandOpen.value) {
      setTimeout(() => { commandInputRef.value?.focus() }, 50)
    } else {
      commandSearch.value = ''
    }
  }
  if (e.key === 'Escape' && isCommandOpen.value) {
    isCommandOpen.value = false
    commandSearch.value = ''
  }
}

onMounted(() => {
  window.addEventListener('keydown', handleKeyDown)
})
onUnmounted(() => {
  window.removeEventListener('keydown', handleKeyDown)
})

function executeCommand() {
  if (!commandSearch.value.trim()) return
  const q = commandSearch.value.trim()
  
  if (q.startsWith('http://') || q.startsWith('https://')) {
    // Tự động chuyển qua trang tạo link nếu gõ link
    router.push({ name: 'user-create-link', query: { url: q } })
  } else {
    // Tìm kiếm trong Links
    router.push({ name: 'user-links', query: { search: q } })
  }
  
  isCommandOpen.value = false
  commandSearch.value = ''
}
// ===============================
</script>

<template>
  <div class="flex h-screen overflow-hidden bg-surface text-on-surface">
    <aside class="hidden w-64 flex-col border-r border-outline-variant bg-surface-container transition-all duration-300 md:flex">
      <div class="header-gradient flex h-14 items-center border-b border-outline-variant px-6 text-white">
        <span class="text-xl font-bold tracking-wider">WeShort</span>
        <span class="ml-2 text-xs font-medium text-white/70">by Wemake</span>
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
      <header class="z-10 flex h-14 shrink-0 items-center justify-between border-b border-outline-variant bg-surface-container px-4 md:px-6 shadow-sm gap-2">
        <div class="min-w-0 flex-1">
          <p class="hidden text-xs text-on-surface-variant md:block">Khu vực người dùng</p>
          <h1 class="text-lg font-bold truncate">{{ title }}</h1>
        </div>
        <div class="flex items-center gap-2 md:gap-3 shrink-0">
          <span 
            v-if="authStore.user?.currentPlanId === 2"
            class="rounded-full bg-primary/20 border border-primary/30 px-3 py-1 text-xs font-black text-primary uppercase drop-shadow-sm tracking-widest"
          >
            PRO
          </span>
          <span 
            v-else-if="authStore.user?.currentPlanId === 3"
            class="rounded-full bg-amber-500/20 border border-amber-500/30 px-3 py-1 text-xs font-black text-amber-600 uppercase drop-shadow-sm tracking-widest"
          >
            PLUS
          </span>
          <span 
            v-else
            class="rounded-full bg-gray-200 border border-gray-300 px-3 py-1 text-xs font-bold text-gray-500 uppercase tracking-widest"
          >
            FREE
          </span>
          <span class="text-sm font-bold text-on-surface hidden sm:inline ml-1 truncate max-w-[150px]">{{ authStore.user?.fullName || authStore.user?.email }}</span>
          <button @click="logout" class="md:hidden flex items-center justify-center p-1.5 text-red-500 hover:bg-red-50 rounded" title="Đăng xuất">
            <LogOut :size="20" />
          </button>
        </div>
      </header>

      <main class="flex-1 overflow-y-auto bg-surface p-4 md:p-6">
        <RouterView />
      </main>

      <nav class="z-10 flex h-14 shrink-0 border-t border-outline-variant bg-surface-container shadow-[0_-4px_6px_-1px_rgba(0,0,0,0.1)] md:hidden overflow-x-auto" style="scrollbar-width: none;">
        <RouterLink
          v-for="item in navItems"
          :key="item.to"
          :to="item.to"
          class="flex flex-col items-center justify-center gap-1 min-w-[72px] flex-shrink-0 text-[10px] font-medium transition-colors"
          :class="isNavActive(item.to) ? 'text-primary' : 'text-on-surface-variant hover:text-on-surface'"
        >
          <component :is="item.icon" :size="20" />
          {{ item.shortLabel }}
        </RouterLink>
      </nav>
    </div>

    <!-- COMMAND PALETTE MODAL -->
    <div v-if="isCommandOpen" class="fixed inset-0 z-[999] flex items-start justify-center pt-[15vh] px-4 sm:px-0">
      <!-- Backdrop -->
      <div class="fixed inset-0 bg-slate-900/40 backdrop-blur-sm transition-opacity" @click="isCommandOpen = false"></div>
      
      <!-- Command Box -->
      <div 
        class="relative w-full max-w-xl scale-100 overflow-hidden rounded-xl bg-white shadow-2xl ring-1 ring-black/5 transition-all"
        style="animation: slideDown 0.2s cubic-bezier(0.16, 1, 0.3, 1);"
      >
        <div class="flex items-center border-b border-slate-100 px-4 py-3">
          <SearchIcon class="h-5 w-5 text-slate-400" />
          <input
            ref="commandInputRef"
            v-model="commandSearch"
            type="text"
            class="ml-3 flex-1 appearance-none bg-transparent outline-none placeholder:text-slate-400 text-slate-900 sm:text-sm"
            placeholder="Dán link dài để rút gọn ngay, hoặc tìm kiếm tag..."
            @keyup.enter="executeCommand"
          />
          <kbd class="hidden sm:flex items-center gap-1 rounded bg-slate-100 px-2 py-1 text-xs font-semibold text-slate-500">
            ESC
          </kbd>
        </div>
        
        <div class="max-h-80 overflow-y-auto px-4 py-3">
          <p class="text-xs font-semibold text-slate-500 uppercase tracking-wider mb-2">Gợi ý hành động</p>
          <div 
            class="flex items-center justify-between rounded-lg px-3 py-2 cursor-pointer hover:bg-blue-50 group"
            @click="router.push('/app/links/create'); isCommandOpen = false"
          >
            <div class="flex items-center gap-3">
              <div class="flex h-8 w-8 items-center justify-center rounded-md bg-blue-100 text-blue-600">
                <LinkIcon :size="16" />
              </div>
              <span class="text-sm font-medium text-slate-700 group-hover:text-blue-700">Tạo mới Link gốc</span>
            </div>
            <ArrowRight :size="14" class="text-slate-400 group-hover:text-blue-500" />
          </div>
          <div 
            class="flex items-center justify-between rounded-lg px-3 py-2 cursor-pointer hover:bg-blue-50 group mt-1"
            @click="router.push('/app/analytics'); isCommandOpen = false"
          >
            <div class="flex items-center gap-3">
              <div class="flex h-8 w-8 items-center justify-center rounded-md bg-emerald-100 text-emerald-600">
                <BarChart2 :size="16" />
              </div>
              <span class="text-sm font-medium text-slate-700 group-hover:text-blue-700">Xem Báo cáo Tổng thể</span>
            </div>
            <ArrowRight :size="14" class="text-slate-400 group-hover:text-blue-500" />
          </div>
        </div>

        <div class="bg-slate-50 px-4 py-3 border-t border-slate-100 text-xs text-slate-500 flex justify-between items-center">
          <span class="flex items-center gap-1"><Command :size="12"/> Spotlight <span class="bg-slate-200 text-slate-700 px-1 rounded ml-1">Ctrl</span>+<span class="bg-slate-200 text-slate-700 px-1 rounded">K</span></span>
          <span>Dùng Enter để xác nhận</span>
        </div>
      </div>
    </div>

  </div>
</template>

<style scoped>
@keyframes slideDown {
  from { opacity: 0; transform: translateY(-10px) scale(0.98); }
  to { opacity: 1; transform: translateY(0) scale(1); }
}
</style>
