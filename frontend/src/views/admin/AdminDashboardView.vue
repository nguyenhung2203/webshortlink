<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { AdminService } from '@/api/services'
import { useAuthStore } from '@/stores/auth'
import WxEmptyState from '@/components/ui/WxEmptyState.vue'
import WxPageHeader from '@/components/ui/WxPageHeader.vue'

const authStore = useAuthStore()
const error = ref('')

async function load() {
  try {
    if (!authStore.token) {
      throw new Error('Chưa xác thực.')
    }

    await AdminService.getDashboard(authStore.token)
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
      description="Trang admin đã được dựng shell, nhưng dữ liệu thật đang bị backend chặn."
    />

    <WxEmptyState
      title="Admin API chưa sẵn sàng"
      :description="error || 'Backend chưa expose /api/admin/dashboard, nên frontend chỉ có thể hiện trạng thái blocked an toàn.'"
    />
  </div>
</template>
