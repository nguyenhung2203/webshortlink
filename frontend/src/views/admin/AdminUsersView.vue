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

    await AdminService.getUsers(authStore.token)
  } catch (err) {
    error.value = err instanceof Error ? err.message : 'Không thể tải danh sách khách hàng.'
  }
}

onMounted(load)
</script>

<template>
  <div class="flex flex-col gap-6">
    <WxPageHeader
      title="Khách hàng"
      description="Màn hình này đã được giữ route và layout, nhưng data admin hiện đang blocked ở backend."
    />

    <WxEmptyState
      title="Chưa thể tải danh sách khách hàng"
      :description="error || 'Backend chưa expose /api/admin/users nên frontend không gọi API giả.'"
    />
  </div>
</template>
