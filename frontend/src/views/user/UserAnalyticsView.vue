<script setup lang="ts">
import { computed, onMounted, ref } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { LinkService, UserService } from '@/api/services'
import { useAuthStore } from '@/stores/auth'
import type { LinkAnalytics, ShortLink } from '@/types/api'
import { 
  Bot, 
  MousePointerClick, 
  Users, 
  Activity, 
  ArrowUpRight, 
  AlertCircle,
  Globe,
  MonitorSmartphone,
  Share2,
  Lock
} from 'lucide-vue-next'
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
const isAdvancedAnalyticsEnabled = computed(() => (authStore.user?.currentPlanId ?? 1) >= 2)

const links = ref<ShortLink[]>([])
const selectedLinkId = ref('')
const analytics = ref<LinkAnalytics | null>(null)
const loading = ref(true)
const loadingDetails = ref(false)
const error = ref('')

const selectedLink = computed(() =>
  links.value.find((l) => l.id === selectedLinkId.value) ?? null,
)

const timeseriesChartData = computed(() => {
  if (!analytics.value || analytics.value.trends.length === 0) return { labels: [], datasets: [] }
  const data = analytics.value.trends
  return {
    labels: data.map((p) => {
      const d = new Date(p.bucket)
      return `${d.getDate()}/${d.getMonth() + 1}`
    }),
    datasets: [
      {
        label: 'Tổng click',
        backgroundColor: '#3b82f6',
        hoverBackgroundColor: '#2563eb',
        data: data.map((p) => p.totalClicks),
        borderRadius: 4,
        barPercentage: 0.6,
      },
      {
        label: 'Click duy nhất',
        backgroundColor: '#10b981',
        hoverBackgroundColor: '#059669',
        data: data.map((p) => p.uniqueClicks),
        borderRadius: 4,
        barPercentage: 0.6,
      },
    ],
  }
})

const timeseriesChartOptions = {
  responsive: true,
  maintainAspectRatio: false,
  interaction: { mode: 'index' as const, intersect: false },
  plugins: { 
    legend: { 
      position: 'top' as const,
      align: 'end' as const,
      labels: {
        boxWidth: 12, boxHeight: 12, borderRadius: 6, useBorderRadius: true,
        font: { size: 12, weight: '500' as const }, color: '#64748b', padding: 16,
      }
    },
    tooltip: {
      backgroundColor: 'rgba(15,23,42,0.92)',
      titleColor: '#f8fafc',
      bodyColor: '#cbd5e1',
      padding: 12,
      cornerRadius: 10,
      displayColors: true,
      boxWidth: 10, boxHeight: 10,
      borderColor: 'rgba(255,255,255,0.08)', borderWidth: 1,
    }
  },
  scales: {
    y: { 
      beginAtZero: true, 
      grid: { color: 'rgba(148,163,184,0.1)', drawTicks: false },
      border: { display: false },
      ticks: { color: '#94a3b8', font: { size: 11 }, padding: 8 }
    },
    x: { 
      grid: { display: false },
      border: { display: false },
      ticks: { color: '#94a3b8', font: { size: 11 }, padding: 4 }
    },
  },
}

async function loadLinks() {
  if (!authStore.accessToken) throw new Error('Chưa xác thực.')
  
  // LinkService.list now returns paginated data: { items: ShortLink[], totalCount: number, ... }
  // We request a large page size (e.g. 1000) so we can populate the dropdown.
  const res = await LinkService.list(authStore.accessToken, { pageIndex: 1, pageSize: 1000 })
  links.value = Array.isArray(res) ? res : (res.items || [])
  
  const requestedId = typeof route.query.linkId === 'string' ? route.query.linkId : ''
  selectedLinkId.value = requestedId || links.value[0]?.id || ''
}

async function loadAnalytics() {
  if (!authStore.accessToken || !selectedLinkId.value) {
    analytics.value = null
    return
  }
  if (!isAdvancedAnalyticsEnabled.value) {
    analytics.value = null
    error.value = ''
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

async function handleLinkChange(e: Event) {
  const target = e.target as HTMLSelectElement
  selectedLinkId.value = target.value
  await router.replace({
    path: '/app/analytics',
    query: selectedLinkId.value ? { linkId: selectedLinkId.value } : {},
  })
  await loadAnalytics()
}

onMounted(bootstrap)
</script>

<template>
  <div class="ui-root">
    <!-- Header -->
    <div class="ui-header">
      <div class="ui-header-left">
        <div class="ui-eyebrow"><Activity :size="13" /> Hiệu suất Link</div>
        <h1 class="ui-title">Phân tích</h1>
        <p class="ui-subtitle">Xem chi tiết click, nguồn truy cập, thiết bị và quốc gia cho từng link.</p>
      </div>
      <div>
        <button 
          v-if="selectedLinkId"
          class="ui-btn ui-btn-outline" 
          @click="router.push(`/app/links/${selectedLinkId}`)"
        >
          <Activity :size="15" /> Mở chi tiết
        </button>
      </div>
    </div>

    <!-- Error state -->
    <div v-if="error && !loading" class="ui-alert ui-alert-error">
      <AlertCircle :size="16" /> {{ error }}
    </div>

    <!-- Initial Loading -->
    <div v-if="loading" class="ui-skeleton" style="height: 120px;" />

    <!-- Empty Links -->
    <div v-else-if="links.length === 0" class="ui-empty">
      <div class="ui-empty-icon"><Activity :size="48" /></div>
      <h3 class="ui-empty-title">Chưa có link nào để phân tích</h3>
      <p class="ui-empty-desc">Hãy tạo ít nhất một shortlink và ghi nhận lượt click để xem dữ liệu tại đây.</p>
      <button class="ui-btn ui-btn-primary" style="margin-top: 1rem;" @click="router.push('/app/links/create')">
        Tạo Link Mới
      </button>
    </div>

    <template v-else>
      <!-- Filter Selector -->
      <div class="ui-panel">
        <div class="ui-panel-body" style="display: flex; gap: 1.5rem; flex-wrap: wrap; align-items: center; justify-content: space-between;">
          <div>
            <h3 class="ui-panel-title">Chọn shortlink cần xem</h3>
            <p style="font-size: 0.8rem; color: #64748b; margin: 0.25rem 0 0;">Lựa chọn một link trong kho của bạn để trích xuất báo cáo phân tích chi tiết.</p>
          </div>
          <select 
            class="ui-form-select" 
            style="max-width: 400px; margin: 0;"
            :value="selectedLinkId"
            @change="handleLinkChange"
          >
            <option v-for="l in links" :key="l.id" :value="l.id">{{ l.shortUrl }}</option>
          </select>
        </div>
      </div>

      <!-- Advanced Feature Lock Notice -->
      <div v-if="!isAdvancedAnalyticsEnabled" class="ui-panel" style="border-color: #bfdbfe; background: #eff6ff;">
        <div class="ui-panel-body" style="display: flex; gap: 1.5rem; flex-wrap: wrap; align-items: center; justify-content: space-between;">
          <div style="flex: 1; min-width: 300px;">
            <div class="ui-eyebrow" style="color: #1d4ed8; margin-bottom: 0.5rem;"><Lock :size="13"/> Tính năng nâng cao</div>
            <h3 class="ui-panel-title" style="color: #1e3a8a;">Khu vực Analytics Nâng cao</h3>
            <p style="font-size: 0.85rem; color: #1e40af; margin: 0.4rem 0 0; line-height: 1.5;">
              Chi tiết biểu đồ quốc gia, thiết bị, referrer và time-series ngày/giờ chỉ có sẵn cho gói Pro hoặc Plus.
            </p>
          </div>
          <button class="ui-btn ui-btn-primary" @click="router.push('/app/billing')">
            Nâng cấp gói tài khoản
          </button>
        </div>
      </div>

      <!-- Sub-loading -->
      <div v-else-if="loadingDetails" class="ui-card-grid-3">
        <div v-for="i in 3" :key="i" class="ui-skeleton" style="height: 100px;" />
      </div>

      <!-- Fully Loaded Analytics -->
      <template v-else-if="analytics">
        
        <!-- KPIs -->
        <div class="ui-card-grid ui-card-grid-3">
          <div class="ui-panel">
            <div class="ui-panel-body" style="display: flex; gap: 1rem; align-items: center;">
              <div style="width: 42px; height: 42px; background: #eff6ff; color: #3b82f6; border-radius: 12px; display: grid; place-items: center;">
                <MousePointerClick :size="20"/>
              </div>
              <div>
                <p style="font-size: 0.75rem; font-weight: 700; color: #64748b; text-transform: uppercase; margin: 0;">Tổng Click</p>
                <p style="font-size: 1.6rem; font-weight: 800; color: #0f172a; margin: 0; line-height: 1.1;">{{ analytics.totalClicks }}</p>
              </div>
            </div>
          </div>
          <div class="ui-panel">
            <div class="ui-panel-body" style="display: flex; gap: 1rem; align-items: center;">
              <div style="width: 42px; height: 42px; background: #ecfdf5; color: #10b981; border-radius: 12px; display: grid; place-items: center;">
                <Users :size="20"/>
              </div>
              <div>
                <p style="font-size: 0.75rem; font-weight: 700; color: #64748b; text-transform: uppercase; margin: 0;">Click Duy Nhất</p>
                <p style="font-size: 1.6rem; font-weight: 800; color: #0f172a; margin: 0; line-height: 1.1;">{{ analytics.uniqueClicks }}</p>
              </div>
            </div>
          </div>
          <div class="ui-panel">
            <div class="ui-panel-body" style="display: flex; gap: 1rem; align-items: center;">
              <div style="width: 42px; height: 42px; background: #fffbeb; color: #f59e0b; border-radius: 12px; display: grid; place-items: center;">
                <Bot :size="20"/>
              </div>
              <div>
                <p style="font-size: 0.75rem; font-weight: 700; color: #64748b; text-transform: uppercase; margin: 0;">Click Từ Bot</p>
                <p style="font-size: 1.6rem; font-weight: 800; color: #0f172a; margin: 0; line-height: 1.1;">{{ analytics.botClicks }}</p>
              </div>
            </div>
          </div>
        </div>

        <!-- Chart and Metadata -->
        <div class="ui-card-grid" style="grid-template-columns: 2fr 1fr;">
          <!-- Chart -->
          <div class="ui-panel" style="grid-column: 1 / -1;">
            <div class="ui-panel-header">
              <h3 class="ui-panel-title">Xu hướng click theo ngày</h3>
            </div>
            <div class="ui-panel-body" style="height: 320px;">
              <div v-if="analytics.trends.length === 0" class="ui-empty" style="height: 100%; padding: 0;">
                <p class="ui-empty-title">Chưa có dữ liệu time-series</p>
              </div>
              <Bar v-else :data="timeseriesChartData" :options="timeseriesChartOptions" />
            </div>
          </div>
        </div>

        <!-- Breakdown Block 2x2 Grid -->
        <div class="ui-card-grid" style="grid-template-columns: 1fr 1fr;">
          
          <div class="ui-panel">
            <div class="ui-panel-header">
              <div style="display: flex; gap: 0.5rem; align-items: center;">
                <Share2 :size="16" style="color: #64748b;"/>
                <h3 class="ui-panel-title">Nguồn truy cập (Nền tảng / UTM)</h3>
              </div>
            </div>
            <div class="ui-panel-body" style="padding: 0; max-height: 250px; overflow-y: auto;">
              <div v-if="analytics.topReferrers.length === 0" style="padding: 1.5rem; text-align: center; color: #94a3b8; font-size: 0.85rem;">Chưa biên dịch đủ dữ liệu.</div>
              <div v-else>
                <div v-for="(item, idx) in analytics.topReferrers" :key="item.label" style="display: flex; justify-content: space-between; padding: 0.75rem 1.25rem; border-bottom: 1px solid #f1f5f9; font-size: 0.85rem;">
                  <div style="display: flex; gap: 0.5rem; color: #475569;">
                    <span style="font-weight: 700; color: #cbd5e1; width: 14px;">{{idx + 1}}</span>
                    <span style="overflow: hidden; text-overflow: ellipsis; max-width: 25ch; white-space: nowrap;">{{ item.label || 'Direct' }}</span>
                  </div>
                  <strong style="color: #0f172a;">{{ item.value }}</strong>
                </div>
              </div>
            </div>
          </div>

          <div class="ui-panel">
            <div class="ui-panel-header">
              <div style="display: flex; gap: 0.5rem; align-items: center;">
                <Share2 :size="16" style="color: #64748b;"/>
                <h3 class="ui-panel-title">Chiến dịch (UTM Campaign)</h3>
              </div>
            </div>
            <div class="ui-panel-body" style="padding: 0; max-height: 250px; overflow-y: auto;">
              <div v-if="!analytics.topCampaigns || analytics.topCampaigns.length === 0" style="padding: 1.5rem; text-align: center; color: #94a3b8; font-size: 0.85rem;">Không có dữ liệu UTM Campaign.</div>
              <div v-else>
                <div v-for="(item, idx) in analytics.topCampaigns" :key="item.label" style="display: flex; justify-content: space-between; padding: 0.75rem 1.25rem; border-bottom: 1px solid #f1f5f9; font-size: 0.85rem;">
                  <div style="display: flex; gap: 0.5rem; color: #475569;">
                    <span style="font-weight: 700; color: #cbd5e1; width: 14px;">{{idx + 1}}</span>
                    <span style="overflow: hidden; text-overflow: ellipsis; max-width: 25ch; white-space: nowrap;">{{ item.label }}</span>
                  </div>
                  <strong style="color: #0f172a;">{{ item.value }}</strong>
                </div>
              </div>
            </div>
          </div>

          <div class="ui-panel">
            <div class="ui-panel-header">
              <div style="display: flex; gap: 0.5rem; align-items: center;">
                <MonitorSmartphone :size="16" style="color: #64748b;"/>
                <h3 class="ui-panel-title">Thiết bị (Device)</h3>
              </div>
            </div>
            <div class="ui-panel-body" style="padding: 0; max-height: 250px; overflow-y: auto;">
              <div v-if="analytics.topDevices.length === 0" style="padding: 1.5rem; text-align: center; color: #94a3b8; font-size: 0.85rem;">Chưa biên dịch đủ dữ liệu.</div>
              <div v-else>
                <div v-for="(item, idx) in analytics.topDevices" :key="item.label" style="display: flex; justify-content: space-between; padding: 0.75rem 1.25rem; border-bottom: 1px solid #f1f5f9; font-size: 0.85rem;">
                  <div style="display: flex; gap: 0.5rem; color: #475569;">
                    <span style="font-weight: 700; color: #cbd5e1; width: 14px;">{{idx + 1}}</span>
                    <span>{{ item.label }}</span>
                  </div>
                  <strong style="color: #0f172a;">{{ item.value }}</strong>
                </div>
              </div>
            </div>
          </div>

          <div class="ui-panel">
            <div class="ui-panel-header">
              <div style="display: flex; gap: 0.5rem; align-items: center;">
                <Globe :size="16" style="color: #64748b;"/>
                <h3 class="ui-panel-title">Quốc gia (Country)</h3>
              </div>
            </div>
            <div class="ui-panel-body" style="padding: 0; max-height: 250px; overflow-y: auto;">
              <div v-if="analytics.topCountries.length === 0" style="padding: 1.5rem; text-align: center; color: #94a3b8; font-size: 0.85rem;">Chưa biên dịch đủ dữ liệu.</div>
              <div v-else>
                <div v-for="(item, idx) in analytics.topCountries" :key="item.label" style="display: flex; justify-content: space-between; padding: 0.75rem 1.25rem; border-bottom: 1px solid #f1f5f9; font-size: 0.85rem;">
                  <div style="display: flex; gap: 0.5rem; color: #475569;">
                    <span style="font-weight: 700; color: #cbd5e1; width: 14px;">{{idx + 1}}</span>
                    <span>{{ item.label }}</span>
                  </div>
                  <strong style="color: #0f172a;">{{ item.value }}</strong>
                </div>
              </div>
            </div>
          </div>

        </div>

      </template>
    </template>
  </div>
</template>

<style scoped>
@media (min-width: 1024px) {
  .ui-card-grid-3 .ui-panel:nth-child(n) { grid-column: auto; }
}
</style>
