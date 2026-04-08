<script setup lang="ts">
import { onMounted, ref, computed } from 'vue'
import { LinkService } from '@/api/services'
import { useAuthStore } from '@/stores/auth'
import type { ShortLink } from '@/types/api'
import { useRouter } from 'vue-router'
import {
  Search,
  ArrowUpDown,
  MousePointerClick,
  Copy,
  ExternalLink,
  Plus,
  Link as LinkIcon,
  Calendar,
  MoreVertical,
  Activity,
  Pause,
  AlertCircle
} from 'lucide-vue-next'

const router = useRouter()
const authStore = useAuthStore()

const links = ref<ShortLink[]>([])
const loading = ref(true)
const error = ref('')
const search = ref('')
const sortKey = ref<'createdAt' | 'clicks'>('createdAt')
const copySuccessId = ref<string | null>(null)

// Stats for toolbar
const summary = computed(() => {
  return {
    total: links.value.length,
    active: links.value.filter(l => l.status === 'Active').length,
    paused: links.value.filter(l => l.status === 'Paused').length,
  }
})

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
    // Cập nhật local state nhanh thay vì reload lại toàn bộ
    link.status = link.status === 'Active' ? 'Paused' : 'Active'
  } catch (err) {
    error.value = err instanceof Error ? err.message : 'Không thể cập nhật trạng thái.'
  }
}

async function copyUrl(link: ShortLink) {
  try {
    await navigator.clipboard.writeText(link.shortUrl)
    copySuccessId.value = link.id
    setTimeout(() => (copySuccessId.value = null), 1500)
  } catch {
    // ignore
  }
}

function formatDate(dateStr: string | null) {
  if (!dateStr) return 'N/A'
  const d = new Date(dateStr)
  return d.toLocaleDateString('vi-VN', { day: '2-digit', month: '2-digit', year: 'numeric' })
}

function formatNumber(n: number): string {
  if (n >= 1000000) return (n / 1000000).toFixed(1) + 'M'
  if (n >= 1000) return (n / 1000).toFixed(1) + 'K'
  return String(n)
}

const filtered = computed(() => {
  let list = links.value
  if (search.value.trim()) {
    const q = search.value.trim().toLowerCase()
    list = list.filter(
      (l) => l.slug.toLowerCase().includes(q) || l.originalUrl.toLowerCase().includes(q) || (l.shortUrl || '').toLowerCase().includes(q)
    )
  }
  if (sortKey.value === 'clicks') {
    list = [...list].sort((a, b) => (b.totalClicks ?? 0) - (a.totalClicks ?? 0))
  } else {
    list = [...list].sort(
      (a, b) => new Date(b.createdAtUtc ?? 0).getTime() - new Date(a.createdAtUtc ?? 0).getTime()
    )
  }
  return list
})

function toggleSort() {
  sortKey.value = sortKey.value === 'createdAt' ? 'clicks' : 'createdAt'
}

onMounted(load)
</script>

<template>
  <div class="lk-root">
    
    <!-- ── Header ── -->
    <div class="lk-header">
      <div class="lk-header-left">
        <h1 class="lk-title">Quản lý Link</h1>
        <p class="lk-subtitle">Tạo, theo dõi hiệu suất và cấu hình các shortlink của bạn.</p>
      </div>
      <button class="lk-btn lk-btn-primary" @click="router.push('/app/links/create')">
        <Plus :size="16" />
        Tạo Link mới
      </button>
    </div>

    <!-- ── Error Message ── -->
    <div v-if="error && !loading" class="lk-alert lk-alert-error">
      <AlertCircle :size="16" />
      <span>{{ error }}</span>
    </div>

    <!-- ── Loading State ── -->
    <template v-if="loading">
      <div class="lk-toolbar-skeleton" />
      <div class="lk-list">
        <div v-for="i in 3" :key="i" class="lk-card-skeleton" />
      </div>
    </template>

    <!-- ── Content ── -->
    <template v-else>
      
      <!-- Toolbar -->
      <div v-if="links.length > 0" class="lk-toolbar">
        <div class="lk-toolbar-search">
          <Search :size="15" class="lk-search-icon" />
          <input
            v-model="search"
            type="text"
            placeholder="Tìm theo slug, đích đến, URL..."
            class="lk-search-input"
          />
        </div>
        
        <div class="lk-toolbar-actions">
          <div class="lk-summary">
            <span class="lk-summary-item" title="Tổng số link">Trang này: <strong>{{ summary.total }}</strong></span>
            <span class="lk-summary-item text-green" title="Đang hoạt động"><Activity :size="13"/> {{ summary.active }}</span>
            <span class="lk-summary-item text-amber" title="Tạm dừng"><Pause :size="13"/> {{ summary.paused }}</span>
          </div>
          <div class="lk-divider" />
          <button class="lk-btn lk-btn-outline" @click="toggleSort">
            <ArrowUpDown :size="14" />
            Sắp xếp: {{ sortKey === 'createdAt' ? 'Mới nhất' : 'Clicks' }}
          </button>
        </div>
      </div>

      <!-- Links List -->
      <div v-if="links.length > 0 && filtered.length > 0" class="lk-list">
        <div v-for="link in filtered" :key="link.id" class="lk-card">
          
          <div class="lk-card-main">
            <!-- Icon -->
            <div class="lk-card-icon" :class="link.status === 'Active' ? 'bg-green-soft text-green' : 'bg-amber-soft text-amber'">
              <LinkIcon :size="18" />
            </div>
            
            <!-- Info -->
            <div class="lk-card-info">
              <div class="lk-card-title-row">
                <a :href="link.shortUrl" target="_blank" class="lk-short-url">{{ link.shortUrl }}</a>
                <span class="lk-badge" :class="link.status === 'Active' ? 'lk-badge-active' : 'lk-badge-paused'">
                  {{ link.status === 'Active' ? 'Hoạt động' : 'Tạm dừng' }}
                </span>
              </div>
              <a :href="link.originalUrl" target="_blank" class="lk-original-url" :title="link.originalUrl">
                <svg width="12" height="12" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="m9 18 6-6-6-6"/></svg>
                {{ link.originalUrl }}
              </a>
            </div>
          </div>

          <!-- Stats -->
          <div class="lk-card-stats">
            <div class="lk-stat" title="Tổng số click">
              <MousePointerClick :size="14" class="lk-stat-icon" />
              <span><strong>{{ formatNumber(link.totalClicks ?? 0) }}</strong></span>
            </div>
            <div class="lk-stat" title="Ngày tạo">
              <Calendar :size="14" class="lk-stat-icon" />
              <span>{{ formatDate(link.createdAtUtc) }}</span>
            </div>
          </div>

          <!-- Actions -->
          <div class="lk-card-actions">
            <button 
              class="lk-action-btn" 
              :class="{ 'lk-action-copied': copySuccessId === link.id }"
              title="Sao chép link" 
              @click="copyUrl(link)"
            >
              <Copy :size="15" />
              <span class="lk-action-text">{{ copySuccessId === link.id ? 'Đã copy' : 'Copy' }}</span>
            </button>
            <a :href="link.shortUrl" target="_blank" class="lk-action-btn" title="Mở thử link">
              <ExternalLink :size="15" />
              <span class="lk-action-text">Mở</span>
            </a>
            <button class="lk-action-btn" title="Xem chi tiết & Thống kê" @click="router.push(`/app/links/${link.id}`)">
              <Activity :size="15" />
              <span class="lk-action-text">Chi tiết</span>
            </button>
            
            <!-- Context Menu for Status -->
            <button 
              class="lk-action-btn" 
              :class="link.status === 'Active' ? 'lk-btn-danger-soft' : 'lk-btn-success-soft'" 
              @click="toggleStatus(link)"
              :title="link.status === 'Active' ? 'Tạm dừng link này' : 'Kích hoạt lại'"
            >
              <Pause v-if="link.status === 'Active'" :size="15" />
              <Activity v-else :size="15" />
              <span class="lk-action-text">{{ link.status === 'Active' ? 'Dừng' : 'Bật' }}</span>
            </button>
          </div>

        </div>
      </div>

      <!-- No Results from Search -->
      <div v-else-if="links.length > 0 && filtered.length === 0" class="lk-empty lk-empty-search">
        <div class="lk-empty-icon"><Search :size="32" /></div>
        <h3 class="lk-empty-title">Không tìm thấy kết quả</h3>
        <p class="lk-empty-desc">Không có link nào khớp với từ khóa "<strong>{{ search }}</strong>".</p>
      </div>

      <!-- Completely Empty State -->
      <div v-else-if="links.length === 0" class="lk-empty lk-empty-main">
        <div class="lk-empty-illus">🔗</div>
        <h3 class="lk-empty-title">Chưa có shortlink nào</h3>
        <p class="lk-empty-desc">Tạo link rút gọn đầu tiên của bạn để chia sẻ và theo dõi hiệu suất dễ dàng.</p>
        <button class="lk-btn lk-btn-primary mt-4" @click="router.push('/app/links/create')">
          <Plus :size="16" />
          Tạo Shortlink ngay
        </button>
      </div>

    </template>
  </div>
</template>

<style scoped>
/* ═══ Root ═══════════════════════════════════════════════════════════════════ */
.lk-root {
  display: flex;
  flex-direction: column;
  gap: 1.5rem;
}

/* ═══ Header ═════════════════════════════════════════════════════════════════ */
.lk-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 1rem;
  flex-wrap: wrap;
  padding-bottom: 0.5rem;
}

.lk-title {
  font-size: 1.65rem;
  font-weight: 800;
  color: #0f172a;
  line-height: 1.2;
  margin: 0;
}

.lk-subtitle {
  margin: 0.25rem 0 0;
  font-size: 0.875rem;
  color: #64748b;
}

/* ═══ Buttons ════════════════════════════════════════════════════════════════ */
.lk-btn {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  gap: 0.4rem;
  padding: 0.6rem 1.1rem;
  border-radius: 10px;
  font-size: 0.85rem;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.2s;
  white-space: nowrap;
  border: 1px solid transparent;
}

.lk-btn-primary {
  background: #3b82f6;
  color: #fff;
  box-shadow: 0 4px 12px rgba(59, 130, 246, 0.25);
}
.lk-btn-primary:hover {
  background: #2563eb;
  transform: translateY(-1px);
}

.lk-btn-outline {
  background: #fff;
  border-color: #e2e8f0;
  color: #475569;
  box-shadow: 0 1px 2px rgba(0,0,0,0.03);
}
.lk-btn-outline:hover {
  background: #f8fafc;
  color: #0f172a;
  border-color: #cbd5e1;
}

/* ═══ Alerts ═════════════════════════════════════════════════════════════════ */
.lk-alert {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  padding: 0.85rem 1rem;
  border-radius: 10px;
  font-size: 0.85rem;
  font-weight: 500;
}
.lk-alert-error {
  background: #fef2f2;
  border: 1px solid #fecaca;
  color: #b91c1c;
}

/* ═══ Toolbar ════════════════════════════════════════════════════════════════ */
.lk-toolbar {
  display: flex;
  flex-direction: column;
  gap: 1rem;
  padding: 1rem;
  background: #fff;
  border: 1px solid #e2e8f0;
  border-radius: 14px;
  box-shadow: 0 1px 3px rgba(0,0,0,0.02);
}
@media (min-width: 768px) {
  .lk-toolbar {
    flex-direction: row;
    align-items: center;
    justify-content: space-between;
  }
}

.lk-toolbar-search {
  position: relative;
  flex: 1;
  max-width: 400px;
}
.lk-search-icon {
  position: absolute;
  left: 0.85rem;
  top: 50%;
  transform: translateY(-50%);
  color: #94a3b8;
}
.lk-search-input {
  width: 100%;
  height: 40px;
  padding: 0 1rem 0 2.4rem;
  background: #f8fafc;
  border: 1px solid #e2e8f0;
  border-radius: 10px;
  font-size: 0.85rem;
  color: #0f172a;
  transition: all 0.2s;
}
.lk-search-input:focus {
  outline: none;
  background: #fff;
  border-color: #3b82f6;
  box-shadow: 0 0 0 3px rgba(59, 130, 246, 0.1);
}

.lk-toolbar-actions {
  display: flex;
  align-items: center;
  gap: 1rem;
  flex-wrap: wrap;
}

.lk-summary {
  display: flex;
  align-items: center;
  gap: 1rem;
  font-size: 0.8rem;
  color: #64748b;
}
.lk-summary-item {
  display: flex;
  align-items: center;
  gap: 0.3rem;
}
.text-green { color: #10b981; }
.text-amber { color: #f59e0b; }
.lk-summary strong { color: #0f172a; }

.lk-divider {
  width: 1px;
  height: 20px;
  background: #e2e8f0;
}

/* ═══ List & Cards ═══════════════════════════════════════════════════════════ */
.lk-list {
  display: flex;
  flex-direction: column;
  gap: 0.75rem;
}

.lk-card {
  display: flex;
  flex-direction: column;
  background: #fff;
  border: 1px solid #e2e8f0;
  border-radius: 12px;
  padding: 1rem 1.25rem;
  gap: 1rem;
  transition: box-shadow 0.2s, border-color 0.2s;
}
.lk-card:hover {
  border-color: #cbd5e1;
  box-shadow: 0 4px 12px rgba(0,0,0,0.03);
}

@media (min-width: 900px) {
  .lk-card {
    flex-direction: row;
    align-items: center;
    justify-content: space-between;
    gap: 1.5rem;
  }
}

.lk-card-main {
  display: flex;
  align-items: flex-start;
  gap: 1rem;
  flex: 1;
  min-width: 0; 
}

.lk-card-icon {
  width: 40px;
  height: 40px;
  border-radius: 10px;
  display: flex;
  align-items: center;
  justify-content: center;
  flex-shrink: 0;
}
.bg-green-soft { background: #ecfdf5; }
.bg-amber-soft { background: #fffbeb; }

.lk-card-info {
  display: flex;
  flex-direction: column;
  gap: 0.3rem;
  min-width: 0;
  flex: 1;
}

.lk-card-title-row {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  flex-wrap: wrap;
}

.lk-short-url {
  font-size: 1rem;
  font-weight: 700;
  color: #1d4ed8;
  text-decoration: none;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
  max-width: 250px;
}
.lk-short-url:hover { text-decoration: underline; }

.lk-badge {
  font-size: 0.65rem;
  font-weight: 700;
  padding: 0.15rem 0.5rem;
  border-radius: 999px;
  text-transform: uppercase;
  letter-spacing: 0.05em;
}
.lk-badge-active { background: #dcfce7; color: #166534; }
.lk-badge-paused { background: #fef3c7; color: #92400e; }

.lk-original-url {
  display: inline-flex;
  align-items: center;
  gap: 0.25rem;
  font-size: 0.8rem;
  color: #64748b;
  text-decoration: none;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
  max-width: 350px;
}
.lk-original-url:hover { color: #3b82f6; }

/* Stats section inside card */
.lk-card-stats {
  display: flex;
  align-items: center;
  gap: 1.5rem;
  flex-shrink: 0;
}
@media (max-width: 899px) {
  .lk-card-stats { padding-left: 3.5rem; }
}

.lk-stat {
  display: flex;
  align-items: center;
  gap: 0.35rem;
  font-size: 0.8rem;
  color: #64748b;
}
.lk-stat-icon { color: #94a3b8; }
.lk-stat strong { color: #0f172a; font-size: 0.85rem; }

/* Actions section */
.lk-card-actions {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  flex-shrink: 0;
  flex-wrap: wrap;
}
@media (max-width: 899px) {
  .lk-card-actions { 
    padding-top: 0.75rem;
    border-top: 1px solid #f1f5f9;
  }
}

.lk-action-btn {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  gap: 0.3rem;
  height: 32px;
  padding: 0 0.6rem;
  border-radius: 8px;
  background: transparent;
  color: #64748b;
  border: 1px solid transparent;
  font-size: 0.75rem;
  font-weight: 600;
  cursor: pointer;
  text-decoration: none;
  transition: all 0.15s;
}
.lk-action-btn:hover {
  background: #f8fafc;
  color: #0f172a;
  border-color: #cbd5e1;
}
.lk-action-copied {
  color: #10b981;
  background: #ecfdf5;
  border-color: #a7f3d0;
}
.lk-btn-danger-soft { color: #ef4444; }
.lk-btn-danger-soft:hover { background: #fef2f2; color: #dc2626; border-color: #fecaca; }
.lk-btn-success-soft { color: #10b981; }
.lk-btn-success-soft:hover { background: #ecfdf5; color: #059669; border-color: #a7f3d0; }

@media (max-width: 500px) {
  .lk-action-text { display: none; }
  .lk-action-btn { width: 32px; padding: 0; }
}

/* ═══ Empty / Loading ════════════════════════════════════════════════════════ */
.lk-empty {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 4rem 1.5rem;
  text-align: center;
  background: #fff;
  border-radius: 14px;
  border: 1px dashed #cbd5e1;
}
.lk-empty-search { padding: 3rem 1.5rem; border-style: solid; border-color: #e2e8f0; }

.lk-empty-illus { font-size: 3rem; margin-bottom: 1rem; }
.lk-empty-icon { color: #94a3b8; margin-bottom: 1rem; opacity: 0.7; }
.lk-empty-title { font-size: 1.1rem; font-weight: 700; color: #0f172a; margin: 0 0 0.5rem; }
.lk-empty-desc { font-size: 0.85rem; color: #64748b; margin: 0; max-width: 400px; }

/* Skeletons */
.lk-toolbar-skeleton {
  height: 72px;
  border-radius: 14px;
  background: linear-gradient(90deg, #f1f5f9 25%, #e2e8f0 50%, #f1f5f9 75%);
  background-size: 200% 100%;
  animation: shimmer 1.4s infinite;
}
.lk-card-skeleton {
  height: 80px;
  border-radius: 12px;
  background: linear-gradient(90deg, #f1f5f9 25%, #e2e8f0 50%, #f1f5f9 75%);
  background-size: 200% 100%;
  animation: shimmer 1.4s infinite;
}
@keyframes shimmer {
  0%   { background-position: 200% 0; }
  100% { background-position: -200% 0; }
}
</style>
