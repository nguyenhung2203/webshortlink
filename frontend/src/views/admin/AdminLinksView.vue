<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { AdminService } from '@/api/services'
import { useAuthStore } from '@/stores/auth'
import type { ShortLink } from '@/types/api'
import WxPageHeader from '@/components/ui/WxPageHeader.vue'
import WxCard from '@/components/ui/WxCard.vue'
import WxBadge from '@/components/ui/WxBadge.vue'
import WxButton from '@/components/ui/WxButton.vue'

const authStore = useAuthStore()
const links = ref<ShortLink[]>([])
const error = ref('')

async function load() {
  try {
    if (!authStore.token) throw new Error('Chưa xác thực')
    links.value = await AdminService.getLinks(authStore.token)
  } catch (err) {
    error.value = err instanceof Error ? err.message : 'Không thể tải links.'
  }
}

async function toggle(link: ShortLink) {
  const action = link.status === 'Blocked' ? 'unblock' : 'block'

  try {
    if (!authStore.token) throw new Error('Chưa xác thực')
    await AdminService.toggleLinkBlock(authStore.token, link.id, action)
    await load()
  } catch (err) {
    error.value = err instanceof Error ? err.message : 'Không thể cập nhật link.'
  }
}

onMounted(load)
</script>

<template>
  <div class="flex flex-col gap-6">
    <WxPageHeader
      title="Hạ tầng Links"
      subtitle="Giám sát link trên toàn hệ thống, truy xuất chủ sở hữu, lưu lượng và điểm Abuse."
    />

    <p v-if="error" class="text-danger text-sm">{{ error }}</p>

    <div class="grid gap-4">
      <WxCard v-for="link in links" :key="link.id" class="p-5 flex flex-col gap-4">
        <div class="flex justify-between items-start">
          <div class="flex flex-col gap-1">
            <strong class="text-lg text-primary">{{ link.shortUrl }}</strong>
            <p class="text-sm text-on-surface-variant font-mono">Chủ sở hữu: {{ link.owner }}</p>
          </div>
          <WxBadge :variant="link.status === 'Blocked' ? 'error' : link.status === 'Active' ? 'success' : 'warning'">
            {{ link.status === 'Blocked' ? 'Bị chặn' : link.status === 'Active' ? 'Hoạt động' : link.status }}
          </WxBadge>
        </div>

        <div class="flex gap-6 mt-2 text-sm">
          <span class="text-on-surface-variant">Tổng clicks: <strong>{{ link.totalClicks }}</strong></span>
          <span class="text-warning">Bots phát hiện: <strong>{{ link.botClicks }}</strong></span>
          <span class="text-danger flex items-center gap-1">Điểm Abuse (Spam): <strong>{{ link.abuseScore }}</strong></span>
        </div>

        <div class="mt-2">
           <WxButton 
            :variant="link.status === 'Blocked' ? 'success' : 'danger'" 
            @click="toggle(link)"
            size="sm"
          >
            {{ link.status === 'Blocked' ? 'Bỏ chặn Link' : 'Chặn (Ban) Link' }}
          </WxButton>
        </div>
      </WxCard>
    </div>
  </div>
</template>
