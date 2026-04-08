<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { AdminService } from '@/api/services'
import { useAuthStore } from '@/stores/auth'
import type { AdminDashboardDashboard } from '@/types/api'
import WxPageHeader from '@/components/ui/WxPageHeader.vue'
import { Users, Link2, MousePointerClick, TrendingUp, ShieldAlert, Zap } from 'lucide-vue-next'
import { RouterLink } from 'vue-router'

const authStore = useAuthStore()
const data = ref<AdminDashboardDashboard | null>(null)
const error = ref('')
const loading = ref(true)

async function load() {
  loading.value = true
  try {
    if (!authStore.accessToken) throw new Error('Chưa xác thực.')
    data.value = await AdminService.getDashboard(authStore.accessToken)
  } catch (err) {
    error.value = err instanceof Error ? err.message : 'Không thể tải dashboard admin.'
  } finally {
    loading.value = false
  }
}

onMounted(load)
</script>

<template>
  <div class="flex flex-col gap-6">
    <WxPageHeader title="Tổng quan hệ thống" description="Số liệu tổng quan về hoạt động trên hệ thống WeShort." />

    <div v-if="error" class="text-danger bg-danger/10 rounded-xl p-4 text-sm">{{ error }}</div>
    <div v-if="loading" class="text-center py-12 text-on-surface-variant">Đang tải...</div>

    <template v-else-if="data">
      <!-- Top stats row -->
      <div class="grid grid-cols-2 lg:grid-cols-4 gap-4">
        <div class="bg-surface-container rounded-2xl border border-outline p-5 flex flex-col gap-2">
          <Users :size="18" class="text-primary" />
          <p class="text-2xl font-extrabold text-on-surface">{{ data.business.totalUsers }}</p>
          <p class="text-xs text-on-surface-variant">Tổng người dùng</p>
        </div>
        <div class="bg-surface-container rounded-2xl border border-outline p-5 flex flex-col gap-2">
          <Zap :size="18" class="text-warning" />
          <p class="text-2xl font-extrabold text-on-surface">{{ data.business.paidUsers }}</p>
          <p class="text-xs text-on-surface-variant">User trả phí</p>
        </div>
        <div class="bg-surface-container rounded-2xl border border-outline p-5 flex flex-col gap-2">
          <Link2 :size="18" class="text-primary" />
          <p class="text-2xl font-extrabold text-on-surface">{{ data.product.totalLinks }}</p>
          <p class="text-xs text-on-surface-variant">Tổng số link</p>
        </div>
        <div class="bg-surface-container rounded-2xl border border-outline p-5 flex flex-col gap-2">
          <MousePointerClick :size="18" class="text-primary" />
          <p class="text-2xl font-extrabold text-on-surface">{{ data.product.totalClicks }}</p>
          <p class="text-xs text-on-surface-variant">Tổng số click</p>
        </div>
      </div>

      <!-- Detail sections -->
      <div class="grid grid-cols-1 md:grid-cols-3 gap-4">
        <!-- Business -->
        <div class="bg-surface-container rounded-2xl border border-outline p-5">
          <div class="flex items-center gap-2 mb-4">
            <TrendingUp :size="16" class="text-primary" />
            <h3 class="font-semibold text-on-surface">Doanh thu & Users</h3>
          </div>
          <ul class="space-y-2.5 text-sm">
            <li class="flex justify-between items-center">
              <span class="text-on-surface-variant">Tỷ lệ chuyển đổi</span>
              <strong class="text-on-surface">{{ data.business.conversionRate }}%</strong>
            </li>
            <li class="flex justify-between items-center">
              <span class="text-on-surface-variant">MRR tháng này</span>
              <strong class="text-success">${{ data.business.monthlyRevenue ?? 0 }}</strong>
            </li>
          </ul>
        </div>

        <!-- Product -->
        <div class="bg-surface-container rounded-2xl border border-outline p-5">
          <div class="flex items-center gap-2 mb-4">
            <Link2 :size="16" class="text-primary" />
            <h3 class="font-semibold text-on-surface">Sản phẩm (Links)</h3>
          </div>
          <ul class="space-y-2.5 text-sm">
            <li class="flex justify-between items-center">
              <span class="text-on-surface-variant">Link đang hoạt động</span>
              <strong class="text-on-surface">{{ data.product.activeLinks }}</strong>
            </li>
            <li class="flex justify-between items-center">
              <span class="text-on-surface-variant">Unique Clicks</span>
              <strong class="text-on-surface">{{ data.product.uniqueClicks }}</strong>
            </li>
          </ul>
        </div>

        <!-- Operations -->
        <div class="bg-surface-container rounded-2xl border border-outline p-5">
          <div class="flex items-center gap-2 mb-4">
            <ShieldAlert :size="16" class="text-danger" />
            <h3 class="font-semibold text-on-surface">Vận hành</h3>
          </div>
          <ul class="space-y-2.5 text-sm">
            <li class="flex justify-between items-center">
              <span class="text-on-surface-variant">Bot Clicks</span>
              <strong class="text-danger">{{ data.operations.botClicks }}</strong>
            </li>
            <li class="flex justify-between items-center">
              <span class="text-on-surface-variant">Clicks đáng ngờ</span>
              <strong class="text-warning">{{ data.operations.suspiciousClicks }}</strong>
            </li>
            <li class="flex justify-between items-center">
              <span class="text-on-surface-variant">Tỉ lệ lỗi</span>
              <strong :class="data.operations.errorRate > 5 ? 'text-danger' : 'text-on-surface'">{{ data.operations.errorRate }}%</strong>
            </li>
            <li class="flex justify-between items-center">
              <span class="text-on-surface-variant">Queue Lag</span>
              <strong :class="data.operations.queueLagSeconds > 30 ? 'text-warning' : 'text-on-surface'">{{ data.operations.queueLagSeconds }}s</strong>
            </li>
          </ul>
        </div>
      </div>

      <!-- Quick nav -->
      <div class="flex flex-wrap gap-3 pt-2">
        <RouterLink to="/admin/users" class="text-sm font-medium text-primary hover:underline">→ Xem danh sách user</RouterLink>
        <RouterLink to="/admin/links" class="text-sm font-medium text-primary hover:underline">→ Xem danh sách link</RouterLink>
        <RouterLink to="/admin/reports" class="text-sm font-medium text-primary hover:underline">→ Báo cáo</RouterLink>
        <RouterLink to="/admin/security" class="text-sm font-medium text-primary hover:underline">→ Bảo mật</RouterLink>
      </div>
    </template>
  </div>
</template>
