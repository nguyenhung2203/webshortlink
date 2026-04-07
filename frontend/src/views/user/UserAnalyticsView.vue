<script setup lang="ts">
import { computed, onMounted, ref } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { LinkService, UserService } from '@/api/services'
import { useAuthStore } from '@/stores/auth'
import type { LinkAnalytics, ShortLink } from '@/types/api'
import WxButton from '@/components/ui/WxButton.vue'
import WxCard from '@/components/ui/WxCard.vue'
import WxEmptyState from '@/components/ui/WxEmptyState.vue'
import WxPageHeader from '@/components/ui/WxPageHeader.vue'
import WxSelect from '@/components/ui/WxSelect.vue'
import WxStatCard from '@/components/ui/WxStatCard.vue'
import { Bot, MousePointerClick, Users } from 'lucide-vue-next'
import {
  Chart as ChartJS,
  Title,
  Tooltip,
  Legend,
  BarElement,
  CategoryScale,
  LinearScale,
} from 'chart.js'
import { Bar } from 'vue-chartjs'

ChartJS.register(CategoryScale, LinearScale, BarElement, Title, Tooltip, Legend)

const route = useRoute()
const router = useRouter()
const authStore = useAuthStore()

const links = ref<ShortLink[]>([])
const selectedLinkId = ref('')
const analytics = ref<LinkAnalytics | null>(null)
const loading = ref(true)
const loadingDetails = ref(false)
const error = ref('')

const linkOptions = computed(() =>
  links.value.map((link) => ({ label: link.shortUrl, value: link.id })),
)

const selectedLink = computed(() =>
  links.value.find((l) => l.id === selectedLinkId.value) ?? null,
)

const timeseriesChartData = computed(() => {
  if (!analytics.value || analytics.value.trends.length === 0) return { labels: [], datasets: [] }
  const data = analytics.value.trends
  return {
    labels: data.map((p) => p.bucket),
    datasets: [
      {
        label: 'Tổng click',
        backgroundColor: '#0ea5e9',
        data: data.map((p) => p.totalClicks),
        borderRadius: 4,
      },
      {
        label: 'Click duy nhất',
        backgroundColor: '#10b981',
        data: data.map((p) => p.uniqueClicks),
        borderRadius: 4,
      },
    ],
  }
})

const timeseriesChartOptions = {
  responsive: true,
  maintainAspectRatio: false,
  plugins: { legend: { position: 'bottom' as const } },
  scales: {
    y: { beginAtZero: true, grid: { color: 'rgba(150,150,150,0.1)' } },
    x: { grid: { display: false } },
  },
}

async function loadLinks() {
  if (!authStore.accessToken) throw new Error('Chưa xác thực.')
  links.value = await LinkService.list(authStore.accessToken)
  const requestedId = typeof route.query.linkId === 'string' ? route.query.linkId : ''
  selectedLinkId.value = requestedId || links.value[0]?.id || ''
}

async function loadAnalytics() {
  if (!authStore.accessToken || !selectedLinkId.value) {
    analytics.value = null
    return
  }
  loadingDetails.value = true
  error.value = ''
  try {
    analytics.value = await UserService.getLinkAnalytics(authStore.accessToken, selectedLinkId.value)
  } catch (err) {
    analytics.value = null
    error.value = err instanceof Error ? err.message : 'Không thể tải dữ liệu phân tích.'
  } finally {
    loadingDetails.value = false
  }
}

async function bootstrap() {
  loading.value = true
  error.value = ''
  try {
    await loadLinks()
    await loadAnalytics()
  } catch (err) {
    error.value = err instanceof Error ? err.message : 'Không thể khởi tạo phân tích.'
  } finally {
    loading.value = false
  }
}

async function handleLinkChange(value: string | number) {
  selectedLinkId.value = String(value)
  await router.replace({
    path: '/app/analytics',
    query: selectedLinkId.value ? { linkId: selectedLinkId.value } : {},
  })
  await loadAnalytics()
}

onMounted(bootstrap)
</script>

<template>
  <div class="flex flex-col gap-6">
    <WxPageHeader
      title="Phân tích"
      description="Xem chi tiết click, nguồn truy cập, thiết bị và quốc gia cho từng link."
    >
      <template #actions>
        <WxButton
          v-if="selectedLinkId"
          variant="ghost"
          @click="router.push(`/app/links/${selectedLinkId}`)"
        >
          Mở chi tiết link
        </WxButton>
      </template>
    </WxPageHeader>

    <p v-if="error && !loading" class="text-sm text-danger">{{ error }}</p>
    <div v-if="loading" class="text-sm text-on-surface-variant">Đang tải danh sách link và phân tích...</div>

    <WxEmptyState
      v-else-if="links.length === 0"
      title="Chưa có link nào để phân tích"
      description="Hãy tạo ít nhất một shortlink để bắt đầu theo dõi lượng truy cập."
    >
      <template #action>
        <WxButton variant="primary" @click="router.push('/app/links/create')">Tạo link mới</WxButton>
      </template>
    </WxEmptyState>

    <template v-else>
      <WxCard padding="md">
        <div class="flex flex-col gap-4 md:flex-row md:items-end md:justify-between">
          <div class="max-w-xl space-y-1">
            <h2 class="text-lg font-semibold text-on-surface">Bộ lọc phân tích</h2>
            <p class="text-sm text-on-surface-variant">
              Chọn một link để xem tổng hợp thống kê và biểu đồ time-series.
            </p>
          </div>
          <div class="w-full md:max-w-sm">
            <WxSelect
              :model-value="selectedLinkId"
              :options="linkOptions"
              placeholder="Chọn một link"
              @update:model-value="handleLinkChange"
            />
          </div>
        </div>
      </WxCard>

      <div v-if="loadingDetails" class="text-sm text-on-surface-variant">Đang tải chi tiết phân tích...</div>

      <template v-else-if="analytics">
        <div class="grid grid-cols-1 gap-4 md:grid-cols-2 xl:grid-cols-3">
          <WxStatCard title="Tổng click"    :value="analytics.totalClicks"  :icon="MousePointerClick" />
          <WxStatCard title="Click duy nhất" :value="analytics.uniqueClicks" :icon="Users" />
          <WxStatCard title="Bot click"      :value="analytics.botClicks"    :icon="Bot" />
        </div>

        <div class="grid gap-6 xl:grid-cols-[1.2fr_0.8fr]">
          <WxCard padding="md">
            <div class="space-y-4">
              <div>
                <h3 class="text-lg font-semibold text-on-surface">Xu hướng click theo ngày</h3>
                <p class="text-sm text-on-surface-variant">Biểu đồ trực quan hóa bằng Chart.js.</p>
              </div>
              <div v-if="analytics.trends.length === 0" class="rounded-lg bg-surface-container-low p-4 text-sm text-on-surface-variant">
                Chưa có dữ liệu time-series cho link này.
              </div>
              <div v-else class="h-72 w-full mt-2">
                <Bar :data="timeseriesChartData" :options="timeseriesChartOptions" />
              </div>
            </div>
          </WxCard>

          <WxCard padding="md">
            <div class="space-y-4">
              <div>
                <h3 class="text-lg font-semibold text-on-surface">Thông tin link</h3>
                <p class="text-sm text-on-surface-variant">Tổng hợp nhanh link đang được phân tích.</p>
              </div>
              <div v-if="selectedLink" class="space-y-2 text-sm">
                <p class="break-all text-primary">{{ selectedLink.shortUrl }}</p>
                <p class="break-all text-on-surface-variant">{{ selectedLink.originalUrl }}</p>
                <p class="text-on-surface-variant">Trạng thái: <strong class="text-on-surface">{{ selectedLink.status }}</strong></p>
              </div>
            </div>
          </WxCard>
        </div>

        <div class="grid gap-6 lg:grid-cols-3">
          <WxCard padding="md">
            <div class="space-y-4">
              <h3 class="text-lg font-semibold text-on-surface">Nguồn truy cập</h3>
              <div v-if="analytics.topReferrers.length === 0" class="text-sm text-on-surface-variant">Chưa có dữ liệu.</div>
              <div v-else class="space-y-3">
                <div v-for="item in analytics.topReferrers" :key="item.label" class="flex items-center justify-between gap-4 text-sm">
                  <span class="truncate text-on-surface-variant">{{ item.label }}</span>
                  <strong>{{ item.value }}</strong>
                </div>
              </div>
            </div>
          </WxCard>

          <WxCard padding="md">
            <div class="space-y-4">
              <h3 class="text-lg font-semibold text-on-surface">Thiết bị</h3>
              <div v-if="analytics.topDevices.length === 0" class="text-sm text-on-surface-variant">Chưa có dữ liệu.</div>
              <div v-else class="space-y-3">
                <div v-for="item in analytics.topDevices" :key="item.label" class="flex items-center justify-between gap-4 text-sm">
                  <span class="truncate text-on-surface-variant">{{ item.label }}</span>
                  <strong>{{ item.value }}</strong>
                </div>
              </div>
            </div>
          </WxCard>

          <WxCard padding="md">
            <div class="space-y-4">
              <h3 class="text-lg font-semibold text-on-surface">Quốc gia</h3>
              <div v-if="analytics.topCountries.length === 0" class="text-sm text-on-surface-variant">Chưa có dữ liệu.</div>
              <div v-else class="space-y-3">
                <div v-for="item in analytics.topCountries" :key="item.label" class="flex items-center justify-between gap-4 text-sm">
                  <span class="truncate text-on-surface-variant">{{ item.label }}</span>
                  <strong>{{ item.value }}</strong>
                </div>
              </div>
            </div>
          </WxCard>
        </div>
      </template>
    </template>
  </div>
</template>
