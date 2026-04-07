<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { LinkService } from '@/api/services'
import { useAuthStore } from '@/stores/auth'
import type { ShortLink } from '@/types/api'
import WxPageHeader from '@/components/ui/WxPageHeader.vue'
import WxButton from '@/components/ui/WxButton.vue'
import WxCard from '@/components/ui/WxCard.vue'
import WxBadge from '@/components/ui/WxBadge.vue'
import WxEmptyState from '@/components/ui/WxEmptyState.vue'

const authStore = useAuthStore()
const links = ref<ShortLink[]>([])
const loading = ref(true)
const error = ref('')

async function load() {
  loading.value = true
  error.value = ''

  try {
    if (!authStore.accessToken) throw new Error('Chưa xác thực')
    links.value = await LinkService.list(authStore.accessToken)
  } catch (err) {
    error.value = err instanceof Error ? err.message : 'Không thể tải danh sách link.'
  } finally {
    loading.value = false
  }
}

async function toggleStatus(link: ShortLink) {
  try {
    if (!authStore.accessToken) throw new Error('Chưa xác thực')
    await LinkService.updateStatus(authStore.accessToken, link.id, link.status !== 'Active')
    await load()
  } catch (err) {
    error.value = err instanceof Error ? err.message : 'Không thể cập nhật trạng thái.'
  }
}

onMounted(load)
</script>

<template>
  <div class="flex flex-col gap-6">
      <WxPageHeader
      title="Danh sách Link"
      subtitle="Quản lý shortlink và trạng thái chuyển hướng."
    >
      <template #actions>
        <WxButton variant="primary" to="/app/links/create">Tạo Link mới</WxButton>
      </template>
    </WxPageHeader>

    <p v-if="error" class="text-danger">{{ error }}</p>

    <div v-if="loading" class="text-on-surface-variant text-center py-8">Đang tải danh sách link...</div>

    <WxEmptyState
      v-else-if="links.length === 0"
      title="Chưa có shortlink nào"
      description="Bạn chưa tạo danh sách link nào. Hãy bắt đầu bằng cách rút gọn URL đầu tiên của bạn."
    >
      <template #action>
        <WxButton variant="primary" to="/app/links/create">Tạo Link ngay</WxButton>
      </template>
    </WxEmptyState>

    <div v-else class="grid gap-4">
      <WxCard v-for="link in links" :key="link.id" class="p-4 flex flex-col gap-4">
        <div class="flex justify-between items-start">
          <div>
            <div class="font-bold text-lg text-primary">{{ link.shortUrl }}</div>
            <div class="text-sm text-on-surface-variant mt-1">{{ link.originalUrl }}</div>
          </div>
          <WxBadge :variant="link.status === 'Active' ? 'success' : 'warning'">
            {{ link.status === 'Active' ? 'Hoạt động' : link.status === 'Paused' ? 'Tạm dừng' : link.status }}
          </WxBadge>
        </div>

        <div class="flex gap-4 text-sm text-on-surface-variant">
          <span>Tổng click: <strong>{{ link.totalClicks }}</strong></span>
          <span>Click duy nhất: <strong>{{ link.uniqueClicks }}</strong></span>
          <span>Bot: <strong>{{ link.botClicks }}</strong></span>
        </div>

        <div class="flex gap-3 mt-2">
          <WxButton 
            variant="ghost" 
            :to="`/app/links/${link.id}`"
          >
            Chi tiết
          </WxButton>
          <WxButton 
            variant="secondary" 
            :href="`http://localhost:5130/go/${link.slug}`" 
            target="_blank"
          >
            Mở xem thử
          </WxButton>
          <WxButton 
            :variant="link.status === 'Active' ? 'danger' : 'success'" 
            @click="toggleStatus(link)"
          >
            {{ link.status === 'Active' ? 'Tạm dừng Link' : 'Kích hoạt lại' }}
          </WxButton>
        </div>
      </WxCard>
    </div>
  </div>
</template>
