<script setup lang="ts">
import { onMounted, ref, computed, watch } from 'vue'
import { LinkService } from '@/api/services'
import { useAuthStore } from '@/stores/auth'
import type { ShortLink } from '@/types/api'
import { useRouter } from 'vue-router'
import {
  ArrowUpDown,
  Search,
  MousePointerClick,
  Copy,
  ExternalLink,
  Plus,
  Link as LinkIcon,
  Calendar,
  Activity,
  Pause,
  AlertCircle,
  Filter,
  ChevronLeft,
  ChevronRight,
  Tag,
  FileText
} from 'lucide-vue-next'

const router = useRouter()
const authStore = useAuthStore()

const links = ref<ShortLink[]>([])
const loading = ref(true)
const error = ref('')

const filter = ref({
  search: '',
  status: '',
  tag: '',
  description: '',
  startDate: '',
  endDate: '',
  sortBy: 'createdAt',
  pageIndex: 1,
  pageSize: 5 // Tạm thời để 5 để user test phân trang
})
const totalCount = ref(0)
const copySuccessId = ref<string | null>(null)

const totalPages = computed(() => Math.ceil(totalCount.value / filter.value.pageSize) || 1)

async function load() {
  loading.value = true
  error.value = ''
  try {
    if (!authStore.accessToken) throw new Error('Chưa xác thực')
    // Remove empty values from filter
    const activeFilter: Record<string, any> = { ...filter.value }
    if (!activeFilter.search) delete activeFilter.search
    if (!activeFilter.status) delete activeFilter.status
    if (!activeFilter.tag) delete activeFilter.tag
    if (!activeFilter.description) delete activeFilter.description
    if (!activeFilter.startDate) delete activeFilter.startDate
    if (!activeFilter.endDate) delete activeFilter.endDate

    const res = await LinkService.list(authStore.accessToken, activeFilter)
    
    // Fallback if the backend updates haven't propagated correctly yet
    if (Array.isArray(res)) {
      links.value = res
      totalCount.value = res.length
    } else {
      links.value = res.items || []
      totalCount.value = res.totalCount || 0
      filter.value.pageIndex = res.pageIndex || 1
    }
  } catch (err) {
    error.value = err instanceof Error ? err.message : 'Không thể tải danh sách link.'
  } finally {
    loading.value = false
  }
}

function handleSearch() {
  filter.value.pageIndex = 1
  load()
}

function clearFilters() {
  filter.value.search = ''
  filter.value.status = ''
  filter.value.tag = ''
  filter.value.description = ''
  filter.value.startDate = ''
  filter.value.endDate = ''
  filter.value.sortBy = 'createdAt'
  filter.value.pageIndex = 1
  load()
}

function toggleSort() {
  filter.value.sortBy = filter.value.sortBy === 'createdAt' ? 'clicks' : 'createdAt'
  filter.value.pageIndex = 1
  load()
}

function nextPage() {
  if (filter.value.pageIndex < totalPages.value) {
    filter.value.pageIndex++
    load()
  }
}

function prevPage() {
  if (filter.value.pageIndex > 1) {
    filter.value.pageIndex--
    load()
  }
}

watch(() => [filter.value.status, filter.value.startDate, filter.value.endDate], () => {
    filter.value.pageIndex = 1
    load()
})

async function toggleStatus(link: ShortLink) {
  try {
    if (!authStore.accessToken) throw new Error('Chưa xác thực')
    await LinkService.updateStatus(authStore.accessToken, link.id, link.status !== 'Active')
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

onMounted(load)
</script>

<template>
  <div class="lk-root">
    
    <!-- ── Header ── -->
    <div class="lk-header">
      <div class="lk-header-left">
        <h1 class="lk-title">Quản lý Link</h1>
        <p class="lk-subtitle">Tạo, chia sẻ và theo dõi hiệu suất các đường dẫn của bạn.</p>
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

    <!-- ── Filter & Toolbar ── -->
    <div class="lk-toolbar">
      <div class="lk-toolbar-top">
        <div class="lk-toolbar-search">
          <Search :size="15" class="lk-search-icon" />
          <input
            v-model="filter.search"
            type="text"
            placeholder="Tìm kiếm slug, link, mô tả..."
            class="lk-search-input"
            @keydown.enter="handleSearch"
          />
        </div>
        <button class="lk-btn lk-btn-primary" style="padding: 0 1rem;" @click="handleSearch">Tìm</button>
        <button class="lk-btn lk-btn-outline" style="padding: 0 1rem;" @click="clearFilters">Xóa lọc</button>
      </div>

      <div class="lk-toolbar-bottom">
        <div class="lk-filter-group">
          <Filter :size="14" class="text-slate-400" />
          <select v-model="filter.status" class="lk-select" @change="handleSearch">
            <option value="">Tất cả trạng thái</option>
            <option value="Active">Đang hoạt động</option>
            <option value="Paused">Đã tạm dừng</option>
          </select>

          <input v-model="filter.tag" type="text" placeholder="Tag..." class="lk-select" style="width: 100px;" @keydown.enter="handleSearch" />
          <input v-model="filter.description" type="text" placeholder="Ghi chú..." class="lk-select" style="width: 120px;" @keydown.enter="handleSearch" />

          <div class="lk-date-group">
            <span class="text-xs text-slate-500 font-medium">Từ:</span>
            <input type="date" v-model="filter.startDate" class="lk-date-input" />
            <span class="text-xs text-slate-500 font-medium">Đến:</span>
            <input type="date" v-model="filter.endDate" class="lk-date-input" />
          </div>
        </div>
        <div class="lk-summary">
          <span style="margin-right: 0.5rem;">Tổng số: <strong>{{ totalCount }}</strong> link</span>
          |
          <button class="lk-btn lk-btn-outline" style="padding: 0.2rem 0.5rem; height: 28px; line-height: 1; border: none; box-shadow: none;" @click="toggleSort">
            <ArrowUpDown :size="14" />
            Sắp xếp: {{ filter.sortBy === 'createdAt' ? 'Mới nhất' : 'Nhiều Click' }}
          </button>
        </div>
      </div>
    </div>

    <!-- ── Loading State ── -->
    <template v-if="loading">
      <div class="lk-list">
        <div v-for="i in 3" :key="i" class="lk-card-skeleton" />
      </div>
    </template>

    <!-- ── Content ── -->
    <template v-else>
      
      <!-- Links List -->
      <div v-if="links.length > 0" class="lk-list">
        <div v-for="link in links" :key="link.id" class="lk-card">
          
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

              <div v-if="link.tag || link.description" style="display: flex; gap: 0.75rem; align-items: center; margin-top: 0.25rem;">
                <span v-if="link.tag" style="font-size: 0.7rem; font-weight: 600; color: #3b82f6; background: #eff6ff; padding: 0.15rem 0.5rem; border-radius: 4px; display: inline-flex; align-items: center; gap: 0.2rem;">
                  <Tag :size="10" /> {{ link.tag }}
                </span>
                <span v-if="link.description" style="font-size: 0.75rem; color: #94a3b8; display: inline-flex; align-items: center; gap: 0.2rem;">
                  <FileText :size="12" /> {{ link.description }}
                </span>
              </div>
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

      <!-- No Results -->
      <div v-else class="lk-empty lk-empty-search">
        <div class="lk-empty-icon"><Search :size="32" /></div>
        <h3 class="lk-empty-title">Không tìm thấy kết quả</h3>
        <p class="lk-empty-desc">Thử điều chỉnh thanh tìm kiếm hoặc thay đổi bộ lọc ở trên.</p>
      </div>

      <!-- ── Pagination ── -->
      <div v-if="totalPages > 1" class="lk-pagination">
        <span class="lk-page-info">Trang {{ filter.pageIndex }} / {{ totalPages }}</span>
        <div class="lk-page-controls">
          <button class="lk-btn-page" :disabled="filter.pageIndex <= 1" @click="prevPage">
            <ChevronLeft :size="16" /> Trước
          </button>
          <button class="lk-btn-page" :disabled="filter.pageIndex >= totalPages" @click="nextPage">
            Sau <ChevronRight :size="16" />
          </button>
        </div>
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

/* Pagination Buttons */
.lk-pagination {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding-top: 1rem;
  border-top: 1px solid #e2e8f0;
  margin-top: 0.5rem;
}
.lk-page-info {
  font-size: 0.85rem;
  color: #64748b;
  font-weight: 500;
}
.lk-page-controls {
  display: flex;
  gap: 0.5rem;
}
.lk-btn-page {
  display: inline-flex;
  align-items: center;
  gap: 0.2rem;
  padding: 0.4rem 0.8rem;
  background: #fff;
  border: 1px solid #cbd5e1;
  color: #334155;
  border-radius: 8px;
  font-size: 0.85rem;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.2s;
}
.lk-btn-page:hover:not(:disabled) {
  background: #f1f5f9;
  color: #0f172a;
}
.lk-btn-page:disabled {
  opacity: 0.5;
  cursor: not-allowed;
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

/* ═══ Toolbar & Filter ═══════════════════════════════════════════════════════ */
.lk-toolbar {
  display: flex;
  flex-direction: column;
  gap: 1rem;
  padding: 1.25rem;
  background: #fff;
  border: 1px solid #e2e8f0;
  border-radius: 14px;
  box-shadow: 0 2px 6px rgba(0,0,0,0.02);
}
.lk-toolbar-top {
  display: flex;
  gap: 0.5rem;
  align-items: center;
  flex-wrap: wrap;
}
.lk-toolbar-search {
  position: relative;
  flex: 1;
  min-width: 250px;
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
  border: 1px solid #cbd5e1;
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

.lk-toolbar-bottom {
  display: flex;
  justify-content: space-between;
  align-items: center;
  flex-wrap: wrap;
  gap: 1rem;
  padding-top: 1rem;
  border-top: 1px dashed #e2e8f0;
}
.lk-filter-group {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  flex-wrap: wrap;
}
.text-slate-400 { color: #94a3b8; }
.text-slate-500 { color: #64748b; }
.lk-select {
  height: 36px;
  padding: 0 0.75rem;
  border: 1px solid #cbd5e1;
  border-radius: 8px;
  font-size: 0.85rem;
  color: #334155;
  background: #fff;
  outline: none;
}
.lk-date-group {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  background: #f8fafc;
  padding: 0 0.5rem;
  border-radius: 8px;
  border: 1px solid #e2e8f0;
  height: 36px;
}
.lk-date-input {
  border: none;
  background: transparent;
  font-size: 0.85rem;
  color: #334155;
  outline: none;
}
.lk-summary {
  font-size: 0.85rem;
  color: #64748b;
}
.lk-summary strong { color: #0f172a; }

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
.text-green { color: #10b981; }
.text-amber { color: #f59e0b; }

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

.lk-empty-icon { color: #94a3b8; margin-bottom: 1rem; opacity: 0.7; }
.lk-empty-title { font-size: 1.1rem; font-weight: 700; color: #0f172a; margin: 0 0 0.5rem; }
.lk-empty-desc { font-size: 0.85rem; color: #64748b; margin: 0; max-width: 400px; }

/* Skeletons */
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
