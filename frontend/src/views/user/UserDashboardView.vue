<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { UserService } from '@/api/services'
import { useAuthStore } from '@/stores/auth'
import type { DashboardMetrics } from '@/types/api'
import WxPageHeader from '@/components/ui/WxPageHeader.vue'
import WxCard from '@/components/ui/WxCard.vue'
import WxStatCard from '@/components/ui/WxStatCard.vue'
import { MousePointerClick, Users, Bot, Link as LinkIcon } from 'lucide-vue-next'

const authStore = useAuthStore()
const loading = ref(true)
const error = ref('')
const data = ref<DashboardMetrics | null>(null)

async function load() {
  loading.value = true
  error.value = ''

  try {
    if (!authStore.token) throw new Error('Chưa xác thực')
    data.value = await UserService.getDashboard(authStore.token)
  } catch (err) {
    error.value = err instanceof Error ? err.message : 'Không thể tải dashboard.'
  } finally {
    loading.value = false
  }
}

onMounted(load)
</script>

<template>
  <div class="flex flex-col gap-6">
    <WxPageHeader
      title="Tổng quan"
      subtitle="Theo dõi tổng quan toàn bộ shortlink của bạn."
    />

    <p v-if="error" class="text-danger">{{ error }}</p>

    <div v-if="loading" class="text-on-surface-variant">Đang tải biểu đồ dữ liệu...</div>

    <template v-else-if="data">
      <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
        <WxStatCard 
          title="Tổng lượt click" 
          :value="data.metrics.totalClicks" 
          :icon="MousePointerClick" 
        />
        <WxStatCard 
          title="Click duy nhất" 
          :value="data.metrics.uniqueClicks" 
          :icon="Users" 
        />
        <WxStatCard 
          title="Click từ Bot" 
          :value="data.metrics.botClicks" 
          :icon="Bot" 
        />
        <WxStatCard 
          title="Link đang hoạt động" 
          :value="data.metrics.activeLinks" 
          :icon="LinkIcon" 
        />
      </div>

      <div class="grid grid-cols-1 lg:grid-cols-2 gap-6">
        <WxCard title="Xu hướng click" subtitle="7 ngày gần đây" class="p-4">
          <div class="flex flex-col gap-3 mt-4">
            <div v-for="point in data.trend" :key="point.date" class="flex items-center gap-4">
              <span class="w-24 text-sm text-on-surface-variant">{{ point.date }}</span>
              <div class="flex-1 bg-surface-container-high h-2 rounded-full overflow-hidden">
                <div class="bg-primary h-full rounded-full" :style="{ width: `${Math.max(2, Math.min(100, point.totalClicks * 5))}%` }"></div>
              </div>
              <strong class="w-8 text-right">{{ point.totalClicks }}</strong>
            </div>
          </div>
        </WxCard>

        <WxCard title="Top links" class="p-4">
          <div class="flex flex-col gap-4 mt-4">
            <div v-for="link in data.topLinks" :key="link.id" class="flex justify-between items-center bg-surface-container-low p-3 rounded-lg border border-outline-variant">
              <div>
                <strong class="text-primary">{{ link.shortUrl }}</strong>
                <p class="text-xs text-on-surface-variant">{{ link.status }}</p>
              </div>
              <div class="text-right">
                <strong>{{ link.totalClicks }}</strong>
                <p class="text-xs text-on-surface-variant">{{ link.uniqueClicks }} duy nhất</p>
              </div>
            </div>
          </div>
        </WxCard>
      </div>
    </template>
  </div>
</template>
