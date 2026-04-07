<script setup lang="ts">
import { onMounted, ref, computed } from 'vue'
import { UserService, LinkService } from '@/api/services'
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
  const trends = data.value.trends || []
  return {
    labels: trends.map((p) => p.bucket),
    datasets: [
      {
        label: 'Tổng click',
        backgroundColor: '#0ea5e9',
        borderColor: '#0ea5e9',
        data: trends.map((p) => p.totalClicks),
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
    
    const token = authStore.accessToken
    const links = await LinkService.list(token)
    
    if (links.length === 0) {
      data.value = {
        totalClicks: 0,
        uniqueClicks: 0,
        botClicks: 0,
        activeLinks: 0,
        trends: [],
        topLinks: []
      }
      return
    }

    // Frontend-only aggregation from working endpoints
    let totalBotClicks = 0
    const trendsMap: Record<string, { bucket: string, totalClicks: number, uniqueClicks: number, botClicks: number }> = {}
    
    // Lấy analytics cho từng link để tổng hợp trends và botClicks
    const analyticsPromises = links.map(l => UserService.getLinkAnalytics(token, l.id).catch(() => null))
    const analyticsResults = await Promise.all(analyticsPromises)

    analyticsResults.forEach(an => {
      if (!an) return
      totalBotClicks += an.botClicks || 0
      
      const linkTrends = an.trends || []
      linkTrends.forEach(t => {
        if (!trendsMap[t.bucket]) {
          trendsMap[t.bucket] = { bucket: t.bucket, totalClicks: 0, uniqueClicks: 0, botClicks: 0 }
        }
        const entry = trendsMap[t.bucket]
        if (entry) {
          entry.totalClicks += t.totalClicks
          entry.uniqueClicks += t.uniqueClicks
          entry.botClicks += t.botClicks
        }
      })
    })

    const aggregatedTrends = Object.values(trendsMap).sort((a, b) => a.bucket.localeCompare(b.bucket))

    // Sắp xếp top links
    const topLinks = [...links]
      .sort((a, b) => b.totalClicks - a.totalClicks)
      .slice(0, 5)
      .map(l => ({
        linkId: l.id,
        shortUrl: `https://sho.rt/${l.slug}`,
        totalClicks: l.totalClicks,
        uniqueClicks: l.uniqueClicks,
        status: l.status
      }))

    data.value = {
      totalClicks: links.reduce((sum, l) => sum + l.totalClicks, 0),
      uniqueClicks: links.reduce((sum, l) => sum + l.uniqueClicks, 0),
      botClicks: totalBotClicks,
      activeLinks: links.filter(l => l.status === 'Active').length,
      trends: aggregatedTrends,
      topLinks: topLinks
    }
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

    <p v-if="error && !loading" class="text-red-500 bg-red-50 p-4 rounded-md">{{ error }}</p>
    <div v-if="loading" class="text-gray-500 font-medium">Đang tải biểu đồ dữ liệu...</div>

    <template v-else-if="data">
      <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
        <WxStatCard title="Tổng lượt click"       :value="data.totalClicks"  :icon="MousePointerClick" />
        <WxStatCard title="Click duy nhất"         :value="data.uniqueClicks" :icon="Users" />
        <WxStatCard title="Click từ Bot"           :value="data.botClicks"    :icon="Bot" />
        <WxStatCard title="Link đang hoạt động"    :value="data.activeLinks"  :icon="LinkIcon" />
      </div>

      <div class="grid grid-cols-1 lg:grid-cols-2 gap-6">
        <WxCard title="Xu hướng click" subtitle="7 ngày gần đây" class="p-4">
          <div v-if="data.trends && data.trends.length > 0" class="mt-4 h-64 w-full">
            <Line :data="chartData" :options="chartOptions" />
          </div>
          <div v-else class="mt-4 h-64 w-full flex items-center justify-center text-gray-400">
            Chưa có xu hướng click nào.
          </div>
        </WxCard>

        <WxCard title="Top links" class="p-4">
          <div class="flex flex-col gap-4 mt-4">
            <div
              v-for="link in (data.topLinks || [])"
              :key="link.linkId"
              class="flex justify-between items-center bg-gray-50 p-3 rounded-lg border border-gray-100"
            >
              <div>
                <strong class="text-blue-600 truncate block max-w-xs"><a :href="link.shortUrl" target="_blank">{{ link.shortUrl }}</a></strong>
                <p class="text-xs text-gray-500 mt-0.5">{{ link.status }}</p>
              </div>
              <div class="text-right">
                <strong class="text-gray-900">{{ link.totalClicks }}</strong>
                <p class="text-xs text-gray-500 mt-0.5">{{ link.uniqueClicks }} duy nhất</p>
              </div>
            </div>
            <div v-if="!data.topLinks || data.topLinks.length === 0" class="text-center text-gray-400 py-8">
              Chưa có dữ liệu Top Links.
            </div>
          </div>
        </WxCard>
      </div>
    </template>
  </div>
</template>
