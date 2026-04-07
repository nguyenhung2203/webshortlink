<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { AdminService } from '@/api/services'
import { useAuthStore } from '@/stores/auth'
import type { AdminDashboardDashboard } from '@/types/api'
import WxPageHeader from '@/components/ui/WxPageHeader.vue'

const authStore = useAuthStore()
const data = ref<AdminDashboardDashboard | null>(null)
const error = ref('')

async function load() {
  try {
    if (!authStore.accessToken) {
      throw new Error('Chưa xác thực.')
    }
    data.value = await AdminService.getDashboard(authStore.accessToken)
  } catch (err) {
    error.value = err instanceof Error ? err.message : 'Không thể tải dashboard admin.'
  }
}

onMounted(load)
</script>

<template>
  <div class="flex flex-col gap-6">
    <WxPageHeader
      title="Tổng quan hệ thống"
      description="Số liệu tổng quan về hoạt động trên hệ thống WebShortlink."
    />

    <div v-if="error" class="text-red-500 bg-red-50 p-4 rounded-md">
      {{ error }}
    </div>

    <div v-else-if="data" class="grid grid-cols-1 md:grid-cols-3 gap-6">
      <div class="bg-white p-6 rounded-xl border border-gray-200">
        <h3 class="font-bold text-lg mb-4 text-gray-800">Doanh thu & Users</h3>
        <ul class="space-y-3 text-sm text-gray-600">
          <li class="flex justify-between"><span>Tổng số user:</span> <strong class="text-gray-900">{{ data.business.totalUsers }}</strong></li>
          <li class="flex justify-between"><span>User trả phí:</span> <strong class="text-gray-900">{{ data.business.paidUsers }}</strong></li>
          <li class="flex justify-between"><span>Tỷ lệ chuyển đổi:</span> <strong class="text-gray-900">{{ data.business.conversionRate }}%</strong></li>
          <li class="flex justify-between"><span>Doanh thu tháng (MRR):</span> <strong class="text-gray-900 text-green-600">${{ data.business.monthlyRevenue }}</strong></li>
        </ul>
      </div>

      <div class="bg-white p-6 rounded-xl border border-gray-200">
        <h3 class="font-bold text-lg mb-4 text-gray-800">Sản phẩm (Links)</h3>
        <ul class="space-y-3 text-sm text-gray-600">
          <li class="flex justify-between"><span>Tổng link:</span> <strong class="text-gray-900">{{ data.product.totalLinks }}</strong></li>
          <li class="flex justify-between"><span>Link đang hoạt động:</span> <strong class="text-gray-900">{{ data.product.activeLinks }}</strong></li>
          <li class="flex justify-between"><span>Tổng Click:</span> <strong class="text-gray-900">{{ data.product.totalClicks }}</strong></li>
          <li class="flex justify-between"><span>Unique Clicks:</span> <strong class="text-gray-900">{{ data.product.uniqueClicks }}</strong></li>
        </ul>
      </div>

      <div class="bg-white p-6 rounded-xl border border-gray-200">
        <h3 class="font-bold text-lg mb-4 text-gray-800">Vận hành</h3>
        <ul class="space-y-3 text-sm text-gray-600">
          <li class="flex justify-between"><span>Bot Clicks:</span> <strong class="text-gray-900">{{ data.operations.botClicks }}</strong></li>
          <li class="flex justify-between"><span>Clicks đáng ngờ:</span> <strong class="text-gray-900">{{ data.operations.suspiciousClicks }}</strong></li>
          <li class="flex justify-between"><span>Tỉ lệ lỗi:</span> <strong class="text-gray-900">{{ data.operations.errorRate }}%</strong></li>
          <li class="flex justify-between"><span>Queue Lag (giây):</span> <strong class="text-gray-900">{{ data.operations.queueLagSeconds }}s</strong></li>
        </ul>
      </div>
    </div>
  </div>
</template>
