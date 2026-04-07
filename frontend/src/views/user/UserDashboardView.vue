<script setup lang="ts">
import { onMounted, ref, computed } from 'vue'
import { UserService } from '@/api/services'
import { useAuthStore } from '@/stores/auth'
import type { DashboardMetrics } from '@/types/api'
import WxPageHeader from '@/components/ui/WxPageHeader.vue'
import WxCard from '@/components/ui/WxCard.vue'
import WxStatCard from '@/components/ui/WxStatCard.vue'
import { MousePointerClick, Users, Bot, Link as LinkIcon } from 'lucide-vue-next'
import {
  Chart as ChartJS,
  Title,
  Tooltip,
  Legend,
  CategoryScale,
  LinearScale,
  LineElement,
  PointElement
} from 'chart.js'
import { Line } from 'vue-chartjs'

ChartJS.register(CategoryScale, LinearScale, PointElement, LineElement, Title, Tooltip, Legend)

const authStore = useAuthStore()
const loading = ref(true)
const error = ref('')
const data = ref<DashboardMetrics | null>(null)

const chartData = computed(() => {
  if (!data.value) return { labels: [], datasets: [] }
  return {
    labels: data.value.trends.map((p) => p.bucket),
    datasets: [
      {
        label: 'Tổng click',
        backgroundColor: '#0ea5e9',
        borderColor: '#0ea5e9',
        data: data.value.trends.map((p) => p.totalClicks),
        tension: 0.4,
        fill: false,
      },
    ],
  }
})

const chartOptions = {
  responsive: true,
  maintainAspectRatio: false,
  plugins: { legend: { display: false } },
  scales: {
    y: { beginAtZero: true, grid: { color: 'rgba(150,150,150,0.1)' } },
    x: { grid: { display: false } },
  },
}

async function load() {
  loading.value = true
  error.value = ''
  try {
    if (!authStore.accessToken) throw new Error('Chưa xác thực')
    data.value = await UserService.getDashboard(authStore.accessToken)
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

    <p v-if="error && !loading" class="text-danger">{{ error }}</p>
    <div v-if="loading" class="text-on-surface-variant">Đang tải biểu đồ dữ liệu...</div>

    <template v-else-if="data">
      <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
        <WxStatCard title="Tổng lượt click"       :value="data.totalClicks"  :icon="MousePointerClick" />
        <WxStatCard title="Click duy nhất"         :value="data.uniqueClicks" :icon="Users" />
        <WxStatCard title="Click từ Bot"           :value="data.botClicks"    :icon="Bot" />
        <WxStatCard title="Link đang hoạt động"    :value="data.activeLinks"  :icon="LinkIcon" />
      </div>

      <div class="grid grid-cols-1 lg:grid-cols-2 gap-6">
        <WxCard title="Xu hướng click" subtitle="7 ngày gần đây" class="p-4">
          <div class="mt-4 h-64 w-full">
            <Line :data="chartData" :options="chartOptions" />
          </div>
        </WxCard>

        <WxCard title="Top links" class="p-4">
          <div class="flex flex-col gap-4 mt-4">
            <div
              v-for="link in data.topLinks"
              :key="link.linkId"
              class="flex justify-between items-center bg-surface-container-low p-3 rounded-lg border border-outline-variant"
            >
              <div>
                <strong class="text-primary truncate block max-w-xs">{{ link.shortUrl }}</strong>
                <p class="text-xs text-on-surface-variant mt-0.5">{{ link.status }}</p>
              </div>
              <div class="text-right">
                <strong class="text-on-surface">{{ link.totalClicks }}</strong>
                <p class="text-xs text-on-surface-variant mt-0.5">{{ link.uniqueClicks }} duy nhất</p>
              </div>
            </div>
          </div>
        </WxCard>
      </div>
    </template>
  </div>
</template>
