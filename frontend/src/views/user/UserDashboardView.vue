<script setup lang="ts">
import { onMounted, ref, computed } from 'vue'
import { UserService } from '@/api/services'
import { useAuthStore } from '@/stores/auth'
import type { DashboardMetrics } from '@/types/api'
import {
  MousePointerClick,
  Users,
  Bot,
  Link as LinkIcon,
  TrendingUp,
  ArrowUpRight,
  BarChart3,
  RefreshCw,
  ExternalLink,
  Activity,
} from 'lucide-vue-next'
import {
  Chart as ChartJS,
  Title,
  Tooltip,
  Legend,
  CategoryScale,
  LinearScale,
  LineElement,
  PointElement,
  Filler,
} from 'chart.js'
import { Line } from 'vue-chartjs'
import { useRouter } from 'vue-router'

ChartJS.register(CategoryScale, LinearScale, PointElement, LineElement, Title, Tooltip, Legend, Filler)

const authStore = useAuthStore()
const router = useRouter()
const loading = ref(true)
const error = ref('')
const data = ref<DashboardMetrics | null>(null)

// Tính tỉ lệ click bot so với tổng
const botRate = computed(() => {
  if (!data.value || data.value.totalClicks === 0) return 0
  return Math.round((data.value.botClicks / data.value.totalClicks) * 100)
})

// Tỉ lệ click unique
const uniqueRate = computed(() => {
  if (!data.value || data.value.totalClicks === 0) return 0
  return Math.round((data.value.uniqueClicks / data.value.totalClicks) * 100)
})

const chartData = computed(() => {
  if (!data.value) return { labels: [], datasets: [] }
  const trends = data.value.trends || []
  return {
    labels: trends.map((p) => {
      const d = new Date(p.bucket)
      return `${d.getDate()}/${d.getMonth() + 1}`
    }),
    datasets: [
      {
        label: 'Tổng click',
        data: trends.map((p) => p.totalClicks),
        borderColor: '#3b82f6',
        backgroundColor: 'rgba(59,130,246,0.12)',
        tension: 0.4,
        fill: true,
        pointBackgroundColor: '#3b82f6',
        pointRadius: 4,
        pointHoverRadius: 6,
        borderWidth: 2,
      },
      {
        label: 'Click duy nhất',
        data: trends.map((p) => p.uniqueClicks),
        borderColor: '#10b981',
        backgroundColor: 'rgba(16,185,129,0.08)',
        tension: 0.4,
        fill: true,
        pointBackgroundColor: '#10b981',
        pointRadius: 3,
        pointHoverRadius: 5,
        borderWidth: 2,
        borderDash: [],
      },
    ],
  }
})

const heatmapDays = computed(() => {
  if (!data.value) return []
  const days = []
  const today = new Date()
  today.setHours(0,0,0,0) // Reset time to midnight

  const trendsMap = new Map((data.value.trends || []).map(t => [new Date(t.bucket).toISOString().split('T')[0], t.totalClicks]))
  
  const numDays = 52 * 7
  for(let i = numDays - 1; i >= 0; i--) {
     const d = new Date(today)
     d.setDate(today.getDate() - i)
     const key = d.toISOString().split('T')[0]
     const clicks = trendsMap.get(key) || 0
     
     let level = 0
     if (clicks > 0) level = 1
     if (clicks >= 10) level = 2
     if (clicks >= 50) level = 3
     if (clicks >= 100) level = 4

     days.push({
        date: key,
        clicks: clicks,
        level: level,
        isToday: i === 0
     })
  }
  return days
})

const chartOptions = {
  responsive: true,
  maintainAspectRatio: false,
  interaction: { mode: 'index' as const, intersect: false },
  plugins: {
    legend: {
      display: true,
      position: 'top' as const,
      align: 'end' as const,
      labels: {
        boxWidth: 12,
        boxHeight: 12,
        borderRadius: 6,
        useBorderRadius: true,
        font: { size: 12, weight: 500 },
        color: '#64748b',
        padding: 16,
      },
    },
    tooltip: {
      backgroundColor: 'rgba(15,23,42,0.92)',
      titleColor: '#f8fafc',
      bodyColor: '#cbd5e1',
      padding: 12,
      cornerRadius: 10,
      displayColors: true,
      boxWidth: 10,
      boxHeight: 10,
      borderColor: 'rgba(255,255,255,0.08)',
      borderWidth: 1,
    },
  },
  scales: {
    y: {
      beginAtZero: true,
      grid: { color: 'rgba(148,163,184,0.1)', drawTicks: false },
      border: { display: false },
      ticks: { color: '#94a3b8', font: { size: 11 }, padding: 8 },
    },
    x: {
      grid: { display: false },
      border: { display: false },
      ticks: { color: '#94a3b8', font: { size: 11 }, padding: 4 },
    },
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

function formatNumber(n: number): string {
  if (n >= 1000000) return (n / 1000000).toFixed(1) + 'M'
  if (n >= 1000) return (n / 1000).toFixed(1) + 'K'
  return String(n)
}

onMounted(load)
</script>

<template>
  <div class="db-root">

    <!-- ── Action Bar ── -->
    <div class="db-action-bar">
      <button class="db-refresh-btn" @click="load" :disabled="loading" title="Làm mới dữ liệu">
        <RefreshCw :size="15" :class="{ 'spin': loading }" />
        <span>Làm mới</span>
      </button>
    </div>

    <!-- ── Error ── -->
    <div v-if="error && !loading" class="db-error">
      <span>⚠ {{ error }}</span>
    </div>

    <!-- ── Loading skeleton ── -->
    <template v-if="loading">
      <div class="db-kpi-grid">
        <div v-for="i in 4" :key="i" class="db-kpi-skeleton" />
      </div>
      <div class="db-grid-two">
        <div class="db-chart-skeleton" />
        <div class="db-chart-skeleton" />
      </div>
    </template>

    <!-- ── Main Content ── -->
    <template v-else-if="data">

      <!-- KPI Cards -->
      <div class="db-kpi-grid">

        <div class="db-kpi-card db-kpi-primary">
          <div class="db-kpi-icon db-kpi-icon--blue">
            <MousePointerClick :size="18" />
          </div>
          <div class="db-kpi-body">
            <p class="db-kpi-label">Tổng lượt click</p>
            <p class="db-kpi-value">{{ formatNumber(data.totalClicks) }}</p>
            <p class="db-kpi-hint">Tất cả shortlinks</p>
          </div>
          <div class="db-kpi-accent" />
        </div>

        <div class="db-kpi-card">
          <div class="db-kpi-icon db-kpi-icon--green">
            <Users :size="18" />
          </div>
          <div class="db-kpi-body">
            <p class="db-kpi-label">Click duy nhất</p>
            <p class="db-kpi-value">{{ formatNumber(data.uniqueClicks) }}</p>
            <p class="db-kpi-hint">
              <span class="db-kpi-rate db-kpi-rate--green">{{ uniqueRate }}%</span>
              tỉ lệ unique
            </p>
          </div>
        </div>

        <div class="db-kpi-card">
          <div class="db-kpi-icon db-kpi-icon--amber">
            <Bot :size="18" />
          </div>
          <div class="db-kpi-body">
            <p class="db-kpi-label">Click từ Bot</p>
            <p class="db-kpi-value">{{ formatNumber(data.botClicks) }}</p>
            <p class="db-kpi-hint">
              <span class="db-kpi-rate" :class="botRate > 20 ? 'db-kpi-rate--red' : 'db-kpi-rate--amber'">{{ botRate }}%</span>
              trong tổng click
            </p>
          </div>
        </div>

        <div class="db-kpi-card">
          <div class="db-kpi-icon db-kpi-icon--violet">
            <LinkIcon :size="18" />
          </div>
          <div class="db-kpi-body">
            <p class="db-kpi-label">Link đang hoạt động</p>
            <p class="db-kpi-value">{{ data.activeLinks }}</p>
            <p class="db-kpi-hint">
              <span class="db-kpi-dot db-kpi-dot--green" />
              Đang chạy
            </p>
          </div>
        </div>

      </div>

      <!-- Chart + Top Links -->
      <div class="db-grid-two">

        <!-- Biểu đồ xu hướng -->
        <div class="db-panel">
          <div class="db-panel-header">
            <div>
              <div class="db-panel-eyebrow">
                <TrendingUp :size="13" />
                Xu hướng click
              </div>
              <p class="db-panel-desc">Dữ liệu theo ngày trong kỳ retention của gói</p>
            </div>
          </div>

          <div v-if="data.trends && data.trends.length > 0" class="db-chart-wrap">
            <Line :data="chartData" :options="chartOptions" />
          </div>
          <div v-else class="db-empty">
            <div class="db-empty-icon">📈</div>
            <p class="db-empty-title">Chưa có dữ liệu xu hướng</p>
            <p class="db-empty-hint">Dữ liệu sẽ xuất hiện sau khi có lượt truy cập vào shortlink.</p>
          </div>
        </div>

        <!-- Top Links -->
        <div class="db-panel">
          <div class="db-panel-header">
            <div>
              <div class="db-panel-eyebrow">
                <BarChart3 :size="13" />
                Top Links
              </div>
              <p class="db-panel-desc">5 shortlink có lượt click cao nhất</p>
            </div>
            <button class="db-panel-link" @click="router.push('/app/links')">
              Xem tất cả
              <ArrowUpRight :size="13" />
            </button>
          </div>

          <div v-if="data.topLinks && data.topLinks.length > 0" class="db-top-links">
            <div class="db-top-link-header">
              <span>Link</span>
              <span>Clicks</span>
            </div>
            <a
              v-for="(link, idx) in data.topLinks"
              :key="link.linkId"
              :href="link.shortUrl"
              target="_blank"
              rel="noopener noreferrer"
              class="db-top-link-row"
            >
              <div class="db-top-link-rank">{{ idx + 1 }}</div>
              <div class="db-top-link-info">
                <span class="db-top-link-url">{{ link.shortUrl }}</span>
                <span
                  class="db-top-link-badge"
                  :class="link.status === 'Active' ? 'db-badge--active' : 'db-badge--paused'"
                >
                  {{ link.status === 'Active' ? 'Hoạt động' : 'Tạm dừng' }}
                </span>
              </div>
              <div class="db-top-link-stats">
                <span class="db-top-link-clicks">{{ formatNumber(link.totalClicks) }}</span>
                <span class="db-top-link-unique">{{ formatNumber(link.uniqueClicks) }} unique</span>
              </div>
              <ExternalLink :size="13" class="db-top-link-icon" />
            </a>
          </div>
          <div v-else class="db-empty">
            <div class="db-empty-icon">🔗</div>
            <p class="db-empty-title">Chưa có shortlink nào</p>
            <p class="db-empty-hint">Tạo shortlink đầu tiên để bắt đầu theo dõi analytics.</p>
          </div>
        </div>

      </div>

      <!-- Github Style Heatmap -->
      <div class="db-panel" style="margin-top: 1.5rem;">
        <div class="db-panel-header">
          <div>
            <div class="db-panel-eyebrow">
              <Activity :size="13" />
              Mật độ Tương tác (Heatmap)
            </div>
            <p class="db-panel-desc">Biểu đồ tần suất click trong 1 năm qua</p>
          </div>
        </div>
        
        <div style="overflow-x: auto; padding: 1rem 0; width: 100%;">
          <!-- Grid wrapper -->
          <div style="display: grid; grid-template-rows: repeat(7, 1fr); grid-auto-flow: column; gap: 4px; width: max-content; margin: 0 auto;">
            <div 
              v-for="day in heatmapDays" 
              :key="day.date" 
              class="heatmap-cell group"
              :class="['level-' + day.level, { 'ring-1 ring-blue-500': day.isToday }]"
            >
              <!-- Tooltip (CSS based) -->
              <div class="hidden group-hover:block absolute z-10 bg-slate-800 text-white text-[10px] py-1 px-2 rounded -top-8 left-1/2 -translate-x-1/2 whitespace-nowrap shadow-lg">
                {{ day.clicks }} clicks - {{ day.date }}
              </div>
            </div>
          </div>
          
          <div style="display: flex; justify-content: flex-end; align-items: center; gap: 0.5rem; margin-top: 1rem; font-size: 0.75rem; color: #64748b;">
            <span>Ít</span>
            <div class="heatmap-cell level-0"></div>
            <div class="heatmap-cell level-1"></div>
            <div class="heatmap-cell level-2"></div>
            <div class="heatmap-cell level-3"></div>
            <div class="heatmap-cell level-4"></div>
            <span>Nhiều</span>
          </div>
        </div>
      </div>

    </template>
  </div>
</template>

<style scoped>
/* ═══ Root ═══════════════════════════════════════════════════════════════════ */
.db-root {
  display: flex;
  flex-direction: column;
  gap: 1.5rem;
}

/* ═══ Action Bar ═══════════════════════════════════════════════════════════ */
.db-action-bar {
  display: flex;
  align-items: center;
  justify-content: flex-end;
  gap: 0.5rem;
  min-height: 36px;
}

.db-refresh-btn {
  display: inline-flex;
  align-items: center;
  gap: 0.4rem;
  padding: 0.55rem 1rem;
  border-radius: 10px;
  background: #fff;
  border: 1px solid #e2e8f0;
  color: #475569;
  font-size: 0.82rem;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.2s;
  box-shadow: 0 1px 3px rgba(0,0,0,0.06);
  white-space: nowrap;
}
.db-refresh-btn:hover {
  background: #f8fafc;
  border-color: #cbd5e1;
  color: #1e293b;
}
.db-refresh-btn:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

/* ═══ Error ══════════════════════════════════════════════════════════════════ */
.db-error {
  padding: 0.85rem 1rem;
  border-radius: 10px;
  background: #fef2f2;
  border: 1px solid #fecaca;
  color: #b91c1c;
  font-size: 0.875rem;
  font-weight: 500;
}

/* ═══ KPI Grid ═══════════════════════════════════════════════════════════════ */
.db-kpi-grid {
  display: grid;
  grid-template-columns: repeat(4, 1fr);
  gap: 1rem;
}

.db-kpi-card {
  position: relative;
  display: flex;
  align-items: flex-start;
  gap: 0.85rem;
  padding: 1.25rem;
  background: #fff;
  border: 1px solid #e2e8f0;
  border-radius: 14px;
  box-shadow: 0 1px 4px rgba(0,0,0,0.05);
  overflow: hidden;
  transition: box-shadow 0.2s, transform 0.2s;
}
.db-kpi-card:hover {
  box-shadow: 0 4px 16px rgba(0,0,0,0.08);
  transform: translateY(-1px);
}

.db-kpi-primary {
  background: linear-gradient(135deg, #1d4ed8 0%, #2563eb 55%, #3b82f6 100%);
  border-color: transparent;
}
.db-kpi-primary .db-kpi-label { color: rgba(255,255,255,0.78); }
.db-kpi-primary .db-kpi-value { color: #fff; }
.db-kpi-primary .db-kpi-hint { color: rgba(255,255,255,0.6); }

.db-kpi-accent {
  position: absolute;
  top: -24px;
  right: -24px;
  width: 80px;
  height: 80px;
  border-radius: 50%;
  background: rgba(255,255,255,0.08);
}

.db-kpi-icon {
  flex-shrink: 0;
  width: 38px;
  height: 38px;
  border-radius: 10px;
  display: flex;
  align-items: center;
  justify-content: center;
  margin-top: 0.1rem;
}
.db-kpi-icon--blue  { background: rgba(59,130,246,0.12);  color: #3b82f6; }
.db-kpi-icon--green { background: rgba(16,185,129,0.12);  color: #10b981; }
.db-kpi-icon--amber { background: rgba(245,158,11,0.12);  color: #d97706; }
.db-kpi-icon--violet{ background: rgba(139,92,246,0.12);  color: #7c3aed; }

.db-kpi-primary .db-kpi-icon--blue {
  background: rgba(255,255,255,0.18);
  color: #fff;
}

.db-kpi-body { flex: 1; min-width: 0; }
.db-kpi-label {
  font-size: 0.78rem;
  font-weight: 600;
  color: #64748b;
  margin: 0 0 0.25rem;
  text-transform: uppercase;
  letter-spacing: 0.05em;
}
.db-kpi-value {
  font-size: 1.75rem;
  font-weight: 800;
  color: #0f172a;
  line-height: 1;
  margin: 0 0 0.4rem;
}
.db-kpi-hint {
  display: flex;
  align-items: center;
  gap: 0.3rem;
  font-size: 0.75rem;
  color: #94a3b8;
  margin: 0;
}

.db-kpi-rate {
  font-weight: 700;
  font-size: 0.78rem;
  padding: 0.1rem 0.4rem;
  border-radius: 4px;
}
.db-kpi-rate--green { background: #dcfce7; color: #15803d; }
.db-kpi-rate--amber { background: #fef3c7; color: #d97706; }
.db-kpi-rate--red   { background: #fee2e2; color: #dc2626; }

.db-kpi-dot {
  display: inline-block;
  width: 6px;
  height: 6px;
  border-radius: 50%;
}
.db-kpi-dot--green { background: #10b981; }

/* ═══ Skeletons ══════════════════════════════════════════════════════════════ */
.db-kpi-skeleton {
  height: 110px;
  border-radius: 14px;
  background: linear-gradient(90deg, #f1f5f9 25%, #e2e8f0 50%, #f1f5f9 75%);
  background-size: 200% 100%;
  animation: shimmer 1.4s infinite;
}
.db-chart-skeleton {
  height: 340px;
  border-radius: 14px;
  background: linear-gradient(90deg, #f1f5f9 25%, #e2e8f0 50%, #f1f5f9 75%);
  background-size: 200% 100%;
  animation: shimmer 1.4s infinite;
}

@keyframes shimmer {
  0%   { background-position: 200% 0; }
  100% { background-position: -200% 0; }
}

/* ═══ Two-col grid ═══════════════════════════════════════════════════════════ */
.db-grid-two {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 1rem;
}

/* ═══ Panel ══════════════════════════════════════════════════════════════════ */
.db-panel {
  background: #fff;
  border: 1px solid #e2e8f0;
  border-radius: 14px;
  box-shadow: 0 1px 4px rgba(0,0,0,0.05);
  overflow: hidden;
}

.db-panel-header {
  display: flex;
  align-items: flex-start;
  justify-content: space-between;
  gap: 1rem;
  padding: 1.25rem 1.25rem 0;
}

.db-panel-eyebrow {
  display: inline-flex;
  align-items: center;
  gap: 0.3rem;
  font-size: 0.72rem;
  font-weight: 700;
  text-transform: uppercase;
  letter-spacing: 0.08em;
  color: #3b82f6;
  margin-bottom: 0.2rem;
}

.db-panel-desc {
  font-size: 0.78rem;
  color: #94a3b8;
  margin: 0;
}

.db-panel-link {
  display: inline-flex;
  align-items: center;
  gap: 0.25rem;
  font-size: 0.78rem;
  font-weight: 600;
  color: #3b82f6;
  background: none;
  border: none;
  cursor: pointer;
  padding: 0.25rem 0.5rem;
  border-radius: 6px;
  transition: background 0.15s;
  white-space: nowrap;
  flex-shrink: 0;
}
.db-panel-link:hover { background: #eff6ff; }

/* Chart */
.db-chart-wrap {
  padding: 1rem 1.25rem 1.25rem;
  height: 280px;
}

/* ═══ Top Links ══════════════════════════════════════════════════════════════ */
.db-top-links {
  display: flex;
  flex-direction: column;
  padding: 0.75rem 0 0;
}

.db-top-link-header {
  display: flex;
  justify-content: space-between;
  padding: 0.4rem 1.25rem;
  font-size: 0.7rem;
  font-weight: 700;
  text-transform: uppercase;
  letter-spacing: 0.08em;
  color: #94a3b8;
  background: #f8fafc;
  border-top: 1px solid #f1f5f9;
  border-bottom: 1px solid #f1f5f9;
}

.db-top-link-row {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  padding: 0.85rem 1.25rem;
  text-decoration: none;
  border-bottom: 1px solid #f8fafc;
  transition: background 0.15s;
}
.db-top-link-row:last-child { border-bottom: none; }
.db-top-link-row:hover { background: #f8fafc; }

.db-top-link-rank {
  width: 22px;
  height: 22px;
  border-radius: 6px;
  background: #f1f5f9;
  color: #64748b;
  font-size: 0.72rem;
  font-weight: 800;
  display: flex;
  align-items: center;
  justify-content: center;
  flex-shrink: 0;
}
.db-top-link-row:first-child .db-top-link-rank {
  background: linear-gradient(135deg, #f59e0b, #ef4444);
  color: #fff;
}

.db-top-link-info {
  flex: 1;
  min-width: 0;
  display: flex;
  align-items: center;
  gap: 0.5rem;
  overflow: hidden;
}

.db-top-link-url {
  font-size: 0.82rem;
  font-weight: 600;
  color: #1d4ed8;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
  max-width: 180px;
}

.db-top-link-badge {
  flex-shrink: 0;
  font-size: 0.65rem;
  font-weight: 700;
  padding: 0.15rem 0.45rem;
  border-radius: 999px;
}
.db-badge--active  { background: #dcfce7; color: #166534; }
.db-badge--paused  { background: #fef3c7; color: #92400e; }

.db-top-link-stats {
  display: flex;
  flex-direction: column;
  align-items: flex-end;
  flex-shrink: 0;
}
.db-top-link-clicks {
  font-size: 0.9rem;
  font-weight: 800;
  color: #0f172a;
}
.db-top-link-unique {
  font-size: 0.7rem;
  color: #94a3b8;
}

.db-top-link-icon {
  color: #cbd5e1;
  flex-shrink: 0;
  transition: color 0.15s;
}
.db-top-link-row:hover .db-top-link-icon { color: #3b82f6; }
.db-top-link-row:hover .db-top-link-clicks {
  color: #3b82f6;
}

/* Heatmap */
.heatmap-cell {
  width: 14px;
  height: 14px;
  border-radius: 2px;
  background-color: #ebedf0;
  position: relative;
  cursor: pointer;
  border: 1px solid rgba(27,31,35,0.06);
}
.heatmap-cell.level-0 { background-color: #ebedf0; }
.heatmap-cell.level-1 { background-color: #9be9a8; }
.heatmap-cell.level-2 { background-color: #40c463; }
.heatmap-cell.level-3 { background-color: #30a14e; }
.heatmap-cell.level-4 { background-color: #216e39; }

/* ═══ Empty State ════════════════════════════════════════════════════════════ */
.db-empty {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 3rem 1.5rem;
  text-align: center;
}
.db-empty-icon {
  font-size: 2rem;
  margin-bottom: 0.75rem;
  opacity: 0.7;
}
.db-empty-title {
  font-size: 0.9rem;
  font-weight: 600;
  color: #475569;
  margin: 0 0 0.35rem;
}
.db-empty-hint {
  font-size: 0.79rem;
  color: #94a3b8;
  margin: 0;
  max-width: 260px;
}

/* ═══ Spin animation ═════════════════════════════════════════════════════════ */
.spin {
  animation: spin 1s linear infinite;
}
@keyframes spin {
  to { transform: rotate(360deg); }
}

/* ═══ Responsive ═════════════════════════════════════════════════════════════ */
@media (max-width: 1200px) {
  .db-kpi-grid {
    grid-template-columns: repeat(2, 1fr);
  }
}

@media (max-width: 900px) {
  .db-grid-two {
    grid-template-columns: 1fr;
  }
}

@media (max-width: 600px) {
  .db-kpi-grid {
    grid-template-columns: 1fr 1fr;
  }
  .db-kpi-value {
    font-size: 1.4rem;
  }
  .db-top-link-url {
    max-width: 100px;
  }
}
</style>
