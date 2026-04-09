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
  pageSize: 10 // Đã cập nhật thành 10 theo yêu cầu
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
    
    <!-- ── Unified Toolbar ── -->
    <div class="lk-toolbar">
      <!-- Row 1: Search + Create -->
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
        <button class="lk-btn lk-btn-ghost-sm" @click="clearFilters">Xóa lọc</button>
        <button class="lk-btn lk-btn-primary" @click="handleSearch">
          <Search :size="14" /> Tìm
        </button>
        <div class="lk-toolbar-divider"></div>
        <button class="lk-btn lk-btn-primary" @click="router.push('/app/links/create')">
          <Plus :size="15" /> Tạo Link mới
        </button>
      </div>

      <!-- Row 2: Filters + Meta -->
      <div class="lk-toolbar-bottom">
        <div class="lk-filter-group">
          <Filter :size="13" class="lk-filter-icon" />
          <select v-model="filter.status" class="lk-select" @change="handleSearch">
            <option value="">Tất cả trạng thái</option>
            <option value="Active">Hoạt động</option>
            <option value="Paused">Tạm dừng</option>
          </select>
          <input v-model="filter.tag" type="text" placeholder="🏷 Tag..." class="lk-select lk-filter-short" @keydown.enter="handleSearch" />
          <input v-model="filter.description" type="text" placeholder="📝 Ghi chú..." class="lk-select lk-filter-med" @keydown.enter="handleSearch" />
          <div class="lk-date-group">
            <span class="lk-date-label">Từ</span>
            <input type="date" v-model="filter.startDate" class="lk-date-input" />
            <span class="lk-date-label">→</span>
            <input type="date" v-model="filter.endDate" class="lk-date-input" />
          </div>
        </div>
        <div class="lk-meta">
          <span class="lk-meta-count"><strong>{{ totalCount }}</strong> link</span>
          
          <div class="lk-sort-toggle">
            <button 
              class="lk-sort-option" 
              :class="{ 'active': filter.sortBy === 'createdAt' }" 
              @click="filter.sortBy = 'createdAt'; filter.pageIndex = 1; load()"
            >Mới nhất</button>
            <button 
              class="lk-sort-option" 
              :class="{ 'active': filter.sortBy === 'clicks' }" 
              @click="filter.sortBy = 'clicks'; filter.pageIndex = 1; load()"
            >Nhiều click</button>
          </div>

        </div>
      </div>
    </div>

    <!-- ── Error Message ── -->
    <div v-if="error && !loading" class="lk-alert lk-alert-error">
      <AlertCircle :size="16" />
      <span>{{ error }}</span>
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
          
          <!-- Status dot -->
          <div class="lk-dot" :class="link.status === 'Active' ? 'lk-dot-active' : 'lk-dot-paused'"></div>

          <!-- Main info: all in one row -->
          <div class="lk-card-info">
            <!-- Row 1: short URL + badge + tag + note -->
            <div class="lk-card-row1">
              <a :href="link.shortUrl" target="_blank" class="lk-short-url">{{ link.shortUrl }}</a>
              <span class="lk-badge" :class="link.status === 'Active' ? 'lk-badge-active' : 'lk-badge-paused'">
                {{ link.status === 'Active' ? 'Hoạt động' : 'Tạm dừng' }}
              </span>
              <span v-if="link.tag" class="lk-tag-chip">
                <Tag :size="9" /> {{ link.tag }}
              </span>
              <span v-if="link.description" class="lk-note-chip" :title="link.description">
                <FileText :size="10" /> {{ link.description }}
              </span>
            </div>
            <!-- Row 2: original URL -->
            <a :href="link.originalUrl" target="_blank" class="lk-original-url" :title="link.originalUrl">
              {{ link.originalUrl }}
            </a>
          </div>

          <!-- Stats (compact) -->
          <div class="lk-card-stats">
            <div class="lk-stat" title="Tổng số click">
              <MousePointerClick :size="13" class="lk-stat-icon" />
              <strong>{{ formatNumber(link.totalClicks ?? 0) }}</strong>
            </div>
            <div class="lk-stat" title="Ngày tạo">
              <Calendar :size="13" class="lk-stat-icon" />
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
              <Copy :size="14" />
              <span class="lk-action-text">{{ copySuccessId === link.id ? 'Đã copy' : 'Copy' }}</span>
            </button>
            <a :href="link.shortUrl" target="_blank" class="lk-action-btn" title="Mở thử link">
              <ExternalLink :size="14" />
              <span class="lk-action-text">Mở</span>
            </a>
            <button class="lk-action-btn" title="Xem chi tiết & Thống kê" @click="router.push(`/app/links/${link.id}`)">
              <Activity :size="14" />
              <span class="lk-action-text">Chi tiết</span>
            </button>
            <button 
              class="lk-action-btn" 
              :class="link.status === 'Active' ? 'lk-btn-danger-soft' : 'lk-btn-success-soft'" 
              @click="toggleStatus(link)"
              :title="link.status === 'Active' ? 'Tạm dừng link này' : 'Kích hoạt lại'"
            >
              <Pause v-if="link.status === 'Active'" :size="14" />
              <Activity v-else :size="14" />
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
  gap: 1.25rem;
}

/* ═══ Action Bar ═══════════════════════════════════════════════════════════ */
.lk-action-bar {
  display: flex;
  align-items: center;
  justify-content: flex-end;
  gap: 0.5rem;
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

/* ═══ Toolbar & Filter ═══════════════════════════════════════════════════════════ */
.lk-toolbar {
  display: flex;
  flex-direction: column;
  gap: 0;
  background: #fff;
  border: 1px solid #e2e8f0;
  border-radius: 12px;
  overflow: hidden;
  box-shadow: 0 1px 4px rgba(0,0,0,0.03);
}

.lk-toolbar-top {
  display: flex;
  gap: 0.5rem;
  align-items: center;
  padding: 0.75rem 1rem;
  flex-wrap: nowrap;
}

.lk-toolbar-search {
  position: relative;
  flex: 1;
  min-width: 180px;
}
.lk-search-icon {
  position: absolute;
  left: 0.75rem;
  top: 50%;
  transform: translateY(-50%);
  color: #94a3b8;
  pointer-events: none;
}
.lk-search-input {
  width: 100%;
  height: 36px;
  padding: 0 0.875rem 0 2.25rem;
  background: #f8fafc;
  border: 1px solid #e2e8f0;
  border-radius: 8px;
  font-size: 0.83rem;
  color: #0f172a;
  transition: all 0.2s;
}
.lk-search-input:focus {
  outline: none;
  background: #fff;
  border-color: #3b82f6;
  box-shadow: 0 0 0 3px rgba(59, 130, 246, 0.08);
}

/* Divider between Tìm and Tạo Link */
.lk-toolbar-divider {
  width: 1px;
  height: 22px;
  background: #e2e8f0;
  flex-shrink: 0;
  margin: 0 0.125rem;
}

/* Ghost small button */
.lk-btn-ghost-sm {
  background: transparent;
  border: 1px solid transparent;
  color: #94a3b8;
  font-size: 0.75rem;
  font-weight: 500;
  padding: 0 0.5rem;
  height: 32px;
  border-radius: 6px;
  cursor: pointer;
  transition: all 0.15s;
  white-space: nowrap;
}
.lk-btn-ghost-sm:hover { color: #ef4444; background: #fef2f2; }

/* Filter row */
.lk-toolbar-bottom {
  display: flex;
  justify-content: space-between;
  align-items: center;
  flex-wrap: wrap;
  gap: 0.5rem;
  padding: 0.55rem 1rem;
  background: #f8fafc;
  border-top: 1px solid #f1f5f9;
}

.lk-filter-icon { color: #94a3b8; flex-shrink: 0; }

.lk-filter-group {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  flex-wrap: wrap;
}

.lk-select {
  height: 30px;
  padding: 0 0.6rem;
  border: 1px solid #e2e8f0;
  border-radius: 6px;
  font-size: 0.78rem;
  color: #334155;
  background: #fff;
  outline: none;
  transition: border-color 0.15s;
}
.lk-select:focus { border-color: #3b82f6; }

.lk-filter-short { width: 85px; }
.lk-filter-med   { width: 110px; }

.lk-date-group {
  display: flex;
  align-items: center;
  gap: 0.35rem;
  height: 30px;
  padding: 0 0.5rem;
  background: #fff;
  border: 1px solid #e2e8f0;
  border-radius: 6px;
}
.lk-date-label {
  font-size: 0.72rem;
  font-weight: 500;
  color: #94a3b8;
  white-space: nowrap;
}
.lk-date-input {
  border: none;
  background: transparent;
  font-size: 0.78rem;
  color: #334155;
  outline: none;
  width: 115px;
}

/* Meta & Segmented Control Toggle */
.lk-meta {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  flex-shrink: 0;
}
.lk-meta-count {
  font-size: 0.78rem;
  color: #64748b;
  white-space: nowrap;
}
.lk-meta-count strong { color: #0f172a; }

.lk-sort-toggle {
  display: inline-flex;
  background: #e2e8f0;
  padding: 3px;
  border-radius: 8px;
  align-items: center;
}
.lk-sort-option {
  background: transparent;
  border: none;
  border-radius: 6px;
  font-size: 0.75rem;
  font-weight: 600;
  color: #64748b;
  padding: 0.25rem 0.6rem;
  cursor: pointer;
  transition: all 0.2s cubic-bezier(0.4, 0, 0.2, 1);
  position: relative;
  z-index: 1;
}
.lk-sort-option.active {
  color: #0f172a;
  background: #fff;
  box-shadow: 0 1px 3px rgba(0,0,0,0.1);
}


/* ═══ List & Cards ═══════════════════════════════════════════════════════════ */
.lk-list {
  display: flex;
  flex-direction: column;
  gap: 0;
  border: 1px solid #e2e8f0;
  border-radius: 12px;
  overflow: hidden;
  background: #fff;
}

/* Each card = one horizontal row */
.lk-card {
  display: flex;
  flex-direction: row;
  align-items: center;
  background: #fff;
  border-bottom: 1px solid #f1f5f9;
  padding: 0.55rem 1rem;
  gap: 0.75rem;
  transition: background 0.15s;
  min-height: 0;
}
.lk-card:last-child { border-bottom: none; }
.lk-card:hover { background: #fafafa; }

/* Status dot */
.lk-dot {
  width: 7px;
  height: 7px;
  border-radius: 50%;
  flex-shrink: 0;
}
.lk-dot-active { background: #10b981; }
.lk-dot-paused { background: #f59e0b; }

/* Info block - takes remaining space */
.lk-card-info {
  display: flex;
  flex-direction: column;
  gap: 0.1rem;
  min-width: 0;
  flex: 1;
}

/* Row 1: short URL + badge + tag + note - all inline */
.lk-card-row1 {
  display: flex;
  align-items: center;
  gap: 0.4rem;
  flex-wrap: nowrap;
  min-width: 0;
}

.lk-short-url {
  font-size: 0.875rem;
  font-weight: 700;
  color: #1d4ed8;
  text-decoration: none;
  white-space: nowrap;
  flex-shrink: 0;
}
.lk-short-url:hover { text-decoration: underline; }

.lk-badge {
  font-size: 0.6rem;
  font-weight: 700;
  padding: 0.1rem 0.4rem;
  border-radius: 999px;
  text-transform: uppercase;
  letter-spacing: 0.04em;
  flex-shrink: 0;
  white-space: nowrap;
}
.lk-badge-active { background: #dcfce7; color: #166534; }
.lk-badge-paused { background: #fef3c7; color: #92400e; }

.lk-tag-chip {
  display: inline-flex;
  align-items: center;
  gap: 0.15rem;
  font-size: 0.68rem;
  font-weight: 600;
  color: #3b82f6;
  background: #eff6ff;
  padding: 0.1rem 0.4rem;
  border-radius: 4px;
  white-space: nowrap;
  flex-shrink: 0;
}

.lk-note-chip {
  display: inline-flex;
  align-items: center;
  gap: 0.15rem;
  font-size: 0.68rem;
  color: #94a3b8;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
  max-width: 160px;
  flex-shrink: 1;
}

.lk-original-url {
  font-size: 0.72rem;
  color: #94a3b8;
  text-decoration: none;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
  max-width: 100%;
  display: block;
}
.lk-original-url:hover { color: #3b82f6; }

/* Stats section - compact */
.lk-card-stats {
  display: flex;
  align-items: center;
  gap: 1rem;
  flex-shrink: 0;
}

.lk-stat {
  display: flex;
  align-items: center;
  gap: 0.25rem;
  font-size: 0.75rem;
  color: #64748b;
  white-space: nowrap;
}
.lk-stat-icon { color: #94a3b8; flex-shrink: 0; }
.lk-stat strong { color: #0f172a; font-size: 0.78rem; }

/* Actions section */
.lk-card-actions {
  display: flex;
  align-items: center;
  gap: 0.25rem;
  flex-shrink: 0;
}

.lk-action-btn {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  gap: 0.25rem;
  height: 28px;
  padding: 0 0.5rem;
  border-radius: 6px;
  background: transparent;
  color: #64748b;
  border: 1px solid transparent;
  font-size: 0.72rem;
  font-weight: 600;
  cursor: pointer;
  text-decoration: none;
  transition: all 0.15s;
  white-space: nowrap;
}
.lk-action-btn:hover {
  background: #f1f5f9;
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

@media (max-width: 640px) {
  .lk-action-text { display: none; }
  .lk-action-btn { width: 28px; padding: 0; }
  .lk-card-stats { display: none; }
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
  height: 48px;
  border-bottom: 1px solid #f1f5f9;
  background: linear-gradient(90deg, #f8fafc 25%, #f1f5f9 50%, #f8fafc 75%);
  background-size: 200% 100%;
  animation: shimmer 1.4s infinite;
}
.lk-card-skeleton:first-child { border-radius: 12px 12px 0 0; }
.lk-card-skeleton:last-child { border-radius: 0 0 12px 12px; border-bottom: none; }
@keyframes shimmer {
  0%   { background-position: 200% 0; }
  100% { background-position: -200% 0; }
}
</style>
