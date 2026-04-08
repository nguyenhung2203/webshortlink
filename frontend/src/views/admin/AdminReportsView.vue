<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { AdminService } from '@/api/services'
import { useAuthStore } from '@/stores/auth'
import { BarChart2, Users, Link2, MousePointerClick, TrendingUp } from 'lucide-vue-next'
import WxPageHeader from '@/components/ui/WxPageHeader.vue'

const authStore = useAuthStore()
const reports = ref<any>(null)
const error = ref('')
const loading = ref(true)

async function load() {
  loading.value = true
  try {
    if (!authStore.accessToken) throw new Error('Chưa xác thực.')
    reports.value = await AdminService.getReports(authStore.accessToken)
  } catch (err) {
    // Reports might not have a dedicated endpoint yet - show what we have
    error.value = err instanceof Error ? err.message : 'Không thể tải báo cáo.'
  } finally {
    loading.value = false
  }
}

onMounted(load)
</script>

<template>
  <div class="flex flex-col gap-6">
    <WxPageHeader title="Báo cáo hệ thống" description="Thống kê tổng quan toàn bộ hoạt động của nền tảng." />

    <div v-if="loading" class="text-center py-10 text-on-surface-variant">Đang tải báo cáo...</div>

    <div v-else-if="error" class="bg-surface-container rounded-2xl border border-outline p-8 text-center">
      <BarChart2 :size="40" class="text-on-surface-variant mx-auto mb-3 opacity-40" />
      <p class="text-on-surface-variant text-sm">Chức năng báo cáo nâng cao đang được phát triển.</p>
      <p class="text-on-surface-variant/60 text-xs mt-1">Vui lòng xem dữ liệu tổng quan ở trang Dashboard.</p>
    </div>

    <template v-else-if="reports">
      <!-- Report metrics -->
      <div class="grid grid-cols-2 lg:grid-cols-4 gap-4">
        <div class="bg-surface-container rounded-xl border border-outline p-5 flex flex-col gap-2">
          <Users :size="20" class="text-primary" />
          <p class="text-2xl font-extrabold text-on-surface">{{ reports.totalUsers ?? 0 }}</p>
          <p class="text-xs text-on-surface-variant">Tổng người dùng</p>
        </div>
        <div class="bg-surface-container rounded-xl border border-outline p-5 flex flex-col gap-2">
          <Link2 :size="20" class="text-primary" />
          <p class="text-2xl font-extrabold text-on-surface">{{ reports.totalLinks ?? 0 }}</p>
          <p class="text-xs text-on-surface-variant">Tổng số link</p>
        </div>
        <div class="bg-surface-container rounded-xl border border-outline p-5 flex flex-col gap-2">
          <MousePointerClick :size="20" class="text-primary" />
          <p class="text-2xl font-extrabold text-on-surface">{{ reports.totalClicks ?? 0 }}</p>
          <p class="text-xs text-on-surface-variant">Tổng số click</p>
        </div>
        <div class="bg-surface-container rounded-xl border border-outline p-5 flex flex-col gap-2">
          <TrendingUp :size="20" class="text-primary" />
          <p class="text-2xl font-extrabold text-on-surface">{{ reports.activeSubscriptions ?? 0 }}</p>
          <p class="text-xs text-on-surface-variant">Gói đang hoạt động</p>
        </div>
      </div>

      <!-- Plan breakdown -->
      <div v-if="reports.planBreakdown" class="bg-surface-container rounded-2xl border border-outline p-6">
        <h3 class="font-semibold text-on-surface mb-4">Phân bổ theo gói</h3>
        <div class="flex flex-col gap-3">
          <div v-for="item in reports.planBreakdown" :key="item.planName" class="flex items-center gap-3">
            <span class="text-sm text-on-surface-variant w-16 shrink-0">{{ item.planName }}</span>
            <div class="flex-1 bg-surface-container-high rounded-full h-2">
              <div class="h-2 rounded-full bg-primary transition-all" :style="{ width: `${item.percent}%` }"></div>
            </div>
            <span class="text-sm font-bold text-on-surface w-10 text-right">{{ item.count }}</span>
          </div>
        </div>
      </div>
    </template>
  </div>
</template>
