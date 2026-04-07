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

    await AdminService.getLinks(authStore.token)
  } catch (err) {
    error.value = err instanceof Error ? err.message : 'Không thể tải hạ tầng links.'
  }
}

onMounted(load)
</script>

<template>
  <div class="flex flex-col gap-6">
    <WxPageHeader
      title="Hạ tầng Links"
      description="Frontend không giả lập admin data khi backend chưa mở route /api/admin/links."
    />

    <WxEmptyState
      title="Admin links đang bị khoá"
      :description="error || 'Trang này đã sẵn sàng về UI, nhưng backend chưa expose danh sách và thao tác admin links.'"
    />
  </div>
</template>
