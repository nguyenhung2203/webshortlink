<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { AdminService } from '@/api/services'
import { useAuthStore } from '@/stores/auth'
import type { AdminDashboardDashboard } from '@/types/api'

const authStore = useAuthStore()
const data = ref<AdminDashboardDashboard | null>(null)
const error = ref('')

async function load() {
  try {
    if (!authStore.token) throw new Error('Chưa xác thực')
    data.value = await AdminService.getDashboard(authStore.token)
  } catch (err) {
    error.value = err instanceof Error ? err.message : 'Không thể tải admin dashboard.'
  }
}

onMounted(load)
</script>

<template>
  <div class="flex flex-col gap-6">
    <div class="flex items-center justify-between pb-4 border-b border-outline-variant">
      <div>
        <h2 class="text-2xl font-bold">Tổng quan hệ thống</h2>
        <p class="text-sm text-on-surface-variant mt-1">Theo dõi kinh doanh, sản phẩm và vận hành trong cùng một màn hình.</p>
      </div>
    </div>

    <p v-if="error" class="text-danger text-sm">{{ error }}</p>

    <template v-if="data">
      <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
        <div class="bg-surface-container p-5 rounded-2xl border border-outline-variant shadow-sm flex flex-col gap-1">
          <span class="text-sm text-on-surface-variant font-medium">Tổng Khách hàng</span>
          <strong class="text-3xl font-bold text-primary">{{ data.business.totalUsers }}</strong>
        </div>
        <div class="bg-surface-container p-5 rounded-2xl border border-outline-variant shadow-sm flex flex-col gap-1">
          <span class="text-sm text-on-surface-variant font-medium">Khách hàng trả phí</span>
          <strong class="text-3xl font-bold text-success">{{ data.business.paidUsers }}</strong>
        </div>
        <div class="bg-surface-container p-5 rounded-2xl border border-outline-variant shadow-sm flex flex-col gap-1">
          <span class="text-sm text-on-surface-variant font-medium">Tổng Links hệ thống</span>
          <strong class="text-3xl font-bold text-primary">{{ data.product.totalLinks }}</strong>
        </div>
        <div class="bg-surface-container p-5 rounded-2xl border border-outline-variant shadow-sm flex flex-col gap-1">
          <span class="text-sm text-on-surface-variant font-medium">Tổng lượt tương tác (Clicks)</span>
          <strong class="text-3xl font-bold text-orange-500">{{ data.product.totalClicks }}</strong>
        </div>
      </div>

      <div class="grid grid-cols-1 md:grid-cols-3 gap-6">
        <div class="bg-surface-container p-6 rounded-2xl border border-outline-variant flex flex-col gap-3">
          <h3 class="text-lg font-bold border-b border-outline-variant pb-2">Kinh doanh (Business)</h3>
          <p class="flex justify-between text-sm"><span>Tỷ lệ chuyển đổi:</span> <strong>{{ data.business.conversionRate }}%</strong></p>
          <p class="flex justify-between text-sm"><span>Doanh thu tháng này:</span> <strong class="text-success">{{ data.business.monthlyRevenue.toLocaleString('vi-VN') }}đ</strong></p>
        </div>
        <div class="bg-surface-container p-6 rounded-2xl border border-outline-variant flex flex-col gap-3">
          <h3 class="text-lg font-bold border-b border-outline-variant pb-2">Sản phẩm (Product)</h3>
          <p class="flex justify-between text-sm"><span>Link đang hoạt động:</span> <strong class="text-primary">{{ data.product.activeLinks }}</strong></p>
          <p class="flex justify-between text-sm"><span>Click duy nhất:</span> <strong>{{ data.product.uniqueClicks }}</strong></p>
        </div>
        <div class="bg-surface-container p-6 rounded-2xl border border-outline-variant flex flex-col gap-3">
          <h3 class="text-lg font-bold border-b border-outline-variant pb-2">Vận hành (Operations)</h3>
          <p class="flex justify-between text-sm"><span>Click nghi ngờ Bot:</span> <strong class="text-warning">{{ data.operations.botClicks }}</strong></p>
          <p class="flex justify-between text-sm"><span>Độ trễ hàng đợi:</span> <strong :class="data.operations.queueLagSeconds > 5 ? 'text-danger' : 'text-success'">{{ data.operations.queueLagSeconds }}s</strong></p>
          <p class="flex justify-between text-sm"><span>Tỷ lệ lỗi hệ thống:</span> <strong :class="data.operations.errorRate > 1 ? 'text-danger' : 'text-success'">{{ data.operations.errorRate }}%</strong></p>
        </div>
      </div>
    </template>
  </div>
</template>
