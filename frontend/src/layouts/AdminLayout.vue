<script setup lang="ts">
import { computed } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth'
import { LayoutDashboard, Users, Link as LinkIcon, LogOut, ClipboardList, BarChart2, ShieldCheck, CreditCard, TerminalSquare, Search, Globe, Package } from 'lucide-vue-next'

const route = useRoute()
const router = useRouter()
const authStore = useAuthStore()

const navItems = [
  { to: '/admin', label: 'Cổng thông tin', shortLabel: 'Overview', icon: LayoutDashboard },
  { to: '/admin/users', label: 'Khách hàng', shortLabel: 'Users', icon: Users },
  { to: '/admin/links', label: 'Hệ thống Link', shortLabel: 'Links', icon: LinkIcon },
  { to: '/admin/domains', label: 'Tên miền', shortLabel: 'Domains', icon: Globe },
  { to: '/admin/plans', label: 'Gói & Tính năng', shortLabel: 'Plans', icon: Package },
  { to: '/admin/payments', label: 'Giao dịch', shortLabel: 'Payments', icon: CreditCard },
  { to: '/admin/reports', label: 'Báo cáo', shortLabel: 'Reports', icon: BarChart2 },
  { to: '/admin/security', label: 'Bảo mật', shortLabel: 'Security', icon: ShieldCheck },
  { to: '/admin/audit', label: 'Nhật ký (Audit)', shortLabel: 'Audit', icon: ClipboardList },
]

async function logout() {
  await authStore.logout()
  router.push('/auth/login')
}

// Lấy thông tin page title từ active route name
const pageTitle = computed(() => {
  const map: Record<string, string> = {
    'admin-dashboard': 'Cổng tổng quan',
    'admin-users': 'Tài khoản & Khách hàng',
    'admin-user-detail': 'Chi tiết Khách hàng',
    'admin-links': 'Quản lý Liên kết',
    'admin-link-detail': 'Cấu hình Liên kết',
    'admin-domains': 'Kiểm duyệt Tên miền',
    'admin-plans': 'Quản lý Gói & Tính năng',
    'admin-payments': 'Đối soát Giao dịch',
    'admin-reports': 'Phân tích & Báo cáo',
    'admin-security': 'Phòng thủ & Bảo mật',
    'admin-audit': 'Nhật ký hệ thống',
  }
  return map[route.name as string] || 'Hệ thống Quản trị'
})

</script>

<template>
  <div class="admin-wrapper" style="display: flex; height: 100vh; background-color: #f1f5f9; overflow: hidden;">
    
    <!-- Sidebar Desktop -->
    <aside class="admin-sidebar" style="width: 260px; background-color: #0f172a; color: #f8fafc; display: flex; flex-direction: column; flex-shrink: 0; box-shadow: 4px 0 24px rgba(0,0,0,0.1); z-index: 20;">
      <!-- Logo -->
      <div style="height: 64px; display: flex; items-center; justify-content: flex-start; padding: 0 1.5rem; border-bottom: 1px solid #1e293b; align-items: center; gap: 0.75rem;">
        <TerminalSquare :size="24" style="color: #38bdf8;" />
        <span style="font-weight: 800; font-size: 1.1rem; letter-spacing: 0.5px; color: #f8fafc;">WS<span style="color: #38bdf8;">/</span>ADMIN</span>
      </div>

      <!-- Navigation -->
      <div style="flex: 1; overflow-y: auto; padding: 1.5rem 1rem; display: flex; flex-direction: column; gap: 0.25rem;">
        <div style="font-size: 0.7rem; font-weight: 700; color: #64748b; text-transform: uppercase; letter-spacing: 1px; padding: 0 0.5rem 0.5rem;">Modules</div>
        
        <RouterLink 
          v-for="item in navItems" 
          :key="item.to" 
          :to="item.to" 
          style="display: flex; align-items: center; gap: 0.75rem; padding: 0.75rem 1rem; border-radius: 8px; font-size: 0.9rem; font-weight: 500; text-decoration: none; transition: all 0.2s;"
          :style="route.path === item.to || (item.to !== '/admin' && route.path.startsWith(item.to)) || (item.to === '/admin' && route.path === '/admin') ? 'background-color: #1e293b; border-left: 3px solid #38bdf8; color: #f8fafc;' : 'color: #94a3b8; border-left: 3px solid transparent;'"
        >
          <component :is="item.icon" :size="18" :style="route.path === item.to || (item.to !== '/admin' && route.path.startsWith(item.to)) || (item.to === '/admin' && route.path === '/admin') ? 'color: #38bdf8;' : ''" />
          {{ item.label }}
        </RouterLink>
      </div>

      <!-- Footer Info -->
      <div style="padding: 1.5rem 1rem; border-top: 1px solid #1e293b;">
        <button 
          @click="logout"
          style="width: 100%; display: flex; align-items: center; gap: 0.75rem; padding: 0.75rem 1rem; border-radius: 8px; font-size: 0.9rem; font-weight: 500; color: #ef4444; background: transparent; border: 0; cursor: pointer; text-align: left; transition: background 0.2s;"
          onmouseover="this.style.backgroundColor='#1e293b'"
          onmouseout="this.style.backgroundColor='transparent'"
        >
          <LogOut :size="18" /> Ngắt kết nối
        </button>
      </div>
    </aside>

    <!-- Main Workspace -->
    <div style="flex: 1; display: flex; flex-direction: column; min-width: 0;">
      
      <!-- Topbar -->
      <header style="height: 64px; background: #ffffff; border-bottom: 1px solid #e2e8f0; display: flex; align-items: center; justify-content: space-between; padding: 0 2rem; flex-shrink: 0; z-index: 10;">
        
        <!-- Breadcrumb / Title -->
        <div style="display: flex; align-items: center; gap: 0.75rem;">
          <h1 style="margin: 0; font-size: 1.25rem; font-weight: 800; color: #0f172a; letter-spacing: -0.02em;">{{ pageTitle }}</h1>
          <span style="color: #cbd5e1;">/</span>
          <span style="font-size: 0.85rem; font-weight: 600; color: #64748b;">Hệ thống lõi</span>
        </div>

        <!-- System Status & Account -->
        <div style="display: flex; align-items: center; gap: 1.5rem;">
          <div style="display: flex; align-items: center; gap: 0.5rem; background: #ecfdf5; border: 1px solid #a7f3d0; padding: 0.35rem 0.75rem; border-radius: 999px;">
            <div style="width: 8px; height: 8px; border-radius: 50%; background: #10b981; animation: pulse 2s infinite;"></div>
            <span style="font-size: 0.75rem; font-weight: 700; color: #059669; text-transform: uppercase;">System Ops Nominal</span>
          </div>

          <div style="display: flex; align-items: center; gap: 0.75rem; border-left: 1px solid #e2e8f0; padding-left: 1.5rem;">
            <div style="text-align: right;">
              <div style="font-size: 0.85rem; font-weight: 700; color: #0f172a;">{{ authStore.user?.role }} Account</div>
              <div style="font-size: 0.75rem; color: #64748b;">{{ authStore.user?.email }}</div>
            </div>
            <div style="width: 36px; height: 36px; border-radius: 8px; background: #38bdf8; color: white; display: grid; place-items: center; font-weight: 800; font-size: 1rem;">
              {{ (authStore.user?.email || 'A').charAt(0).toUpperCase() }}
            </div>
          </div>
        </div>
      </header>

      <!-- Content Area -->
      <main style="flex: 1; overflow-y: auto; padding: 2rem; position: relative;">
        <!-- Container for max width scaling -->
        <div style="max-width: 1400px; margin: 0 auto;">
          <RouterView />
        </div>
      </main>

    </div>

  </div>
</template>

<style scoped>
  @media (max-width: 768px) {
    .admin-sidebar {
      display: none !important;
    }
  }
</style>
