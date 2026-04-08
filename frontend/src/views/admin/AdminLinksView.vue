<script setup lang="ts">
import { onMounted, ref, computed } from 'vue'
import { AdminService } from '@/api/services'
import { useAuthStore } from '@/stores/auth'
import type { AdminLink } from '@/types/api'
import { Search, Link2, ChevronRight, Ban, CheckCircle, MousePointerClick, Activity, AlertCircle, RefreshCw } from 'lucide-vue-next'
import { RouterLink } from 'vue-router'

const authStore = useAuthStore()
const data = ref<AdminLink[]>([])
const error = ref('')
const loading = ref(true)
const isActionLoading = ref(false)
const search = ref('')

async function load() {
  loading.value = true
  error.value = ''
  try {
    if (!authStore.accessToken) throw new Error('Chưa xác thực.')
    data.value = await AdminService.getLinks(authStore.accessToken)
  } catch (err) {
    error.value = err instanceof Error ? err.message : 'Không thể tải danh sách links.'
  } finally {
    loading.value = false
  }
}

async function toggleLinkStatus(link: AdminLink) {
  if (isActionLoading.value || !authStore.accessToken) return
  const action = link.status === 'Active' ? 'block' : 'unblock'
  if (!confirm(`${action === 'block' ? 'Vô hiệu hóa' : 'Kích hoạt lại'} link /${link.slug}?`)) return
  isActionLoading.value = true
  try {
    await AdminService.toggleLinkBlock(authStore.accessToken, link.id, action)
    await load()
  } catch (err) {
    alert(err instanceof Error ? err.message : 'Lỗi khi thay đổi trạng thái link.')
  } finally {
    isActionLoading.value = false
  }
}

const filtered = computed(() => {
  if (!search.value.trim()) return data.value
  const q = search.value.trim().toLowerCase()
  return data.value.filter(l =>
    (l.slug || '').toLowerCase().includes(q) ||
    (l.userEmail || '').toLowerCase().includes(q) ||
    (l.shortUrl || '').toLowerCase().includes(q)
  )
})

onMounted(load)
</script>

<template>
  <div class="ui-root">
    
    <!-- Header -->
    <div class="ui-header">
      <div class="ui-header-left">
        <div class="ui-eyebrow"><Link2 :size="13" /> Hệ thống Links</div>
        <h1 class="ui-title">Quản lý & Giám sát</h1>
        <p class="ui-subtitle">Giám sát, quản lý và kiểm soát chất lượng toàn bộ đường dẫn trên nền tảng.</p>
      </div>
      <div>
        <button class="ui-btn ui-btn-outline" @click="load" :disabled="loading">
          <RefreshCw :size="14" :class="{'animate-spin': loading}" /> Làm mới
        </button>
      </div>
    </div>

    <!-- Error state -->
    <div v-if="error" class="ui-alert ui-alert-error">
      <AlertCircle :size="16" /> {{ error }}
    </div>

    <!-- Toolbar -->
    <div class="ui-panel" style="padding: 1rem 1.5rem; display: flex; gap: 1rem; flex-wrap: wrap; align-items: center;">
      <div style="flex: 1; min-width: 250px; position: relative;">
        <Search :size="16" style="position: absolute; left: 1rem; top: 50%; transform: translateY(-50%); color: #94a3b8;" />
        <input
          v-model="search"
          type="text"
          placeholder="Tìm theo slug, email chủ sở hữu..."
          class="ui-form-input"
          style="padding-left: 2.5rem; margin: 0; box-shadow: none;"
        />
      </div>
    </div>

    <!-- Skeleton Loading -->
    <div v-if="loading" class="ui-skeleton" style="height: 400px; border-radius: 12px;" />

    <!-- No Data -->
    <div v-else-if="filtered.length === 0" class="ui-empty" style="background: white;">
      <div class="ui-empty-icon" style="opacity: 0.5;"><Link2 :size="48" /></div>
      <h3 class="ui-empty-title">{{ search ? 'Không tìm thấy kết quả' : 'Chưa có dữ liệu link' }}</h3>
      <p class="ui-empty-desc">{{ search ? 'Thử tìm với từ khóa khác xem sao.' : 'Hệ thống hiện tại chưa có link rút gọn nào.' }}</p>
    </div>

    <!-- Content Table -->
    <div v-else class="ui-panel" style="overflow: hidden; padding: 0;">
      
      <div v-if="isActionLoading" style="position: absolute; inset: 0; z-index: 10; background: rgba(255,255,255,0.7); backdrop-filter: blur(2px); display: flex; justify-content: center; align-items: center;">
        <span style="font-weight: 600; font-size: 0.9rem; color: #3b82f6; display: flex; align-items: center; gap: 0.5rem;">
          <RefreshCw :size="16" class="animate-spin" /> Đang xử lý...
        </span>
      </div>

      <div style="overflow-x: auto;">
        <table style="width: 100%; border-collapse: collapse; min-width: 800px; text-align: left;">
          <thead style="background: #f8fafc; border-bottom: 1px solid #e2e8f0;">
            <tr>
              <th style="padding: 1rem 1.25rem; font-size: 0.75rem; font-weight: 700; color: #64748b; text-transform: uppercase;">Shortlink</th>
              <th style="padding: 1rem 1.25rem; font-size: 0.75rem; font-weight: 700; color: #64748b; text-transform: uppercase;">Chủ sở hữu</th>
              <th style="padding: 1rem 1.25rem; font-size: 0.75rem; font-weight: 700; color: #64748b; text-transform: uppercase;">Lượt Click</th>
              <th style="padding: 1rem 1.25rem; font-size: 0.75rem; font-weight: 700; color: #64748b; text-transform: uppercase;">Risk Score</th>
              <th style="padding: 1rem 1.25rem; font-size: 0.75rem; font-weight: 700; color: #64748b; text-transform: uppercase;">Trạng thái</th>
              <th style="padding: 1rem 1.25rem; font-size: 0.75rem; font-weight: 700; color: #64748b; text-transform: uppercase;">Ngày tạo</th>
              <th style="padding: 1rem 1.25rem; font-size: 0.75rem; font-weight: 700; color: #64748b; text-transform: uppercase; text-align: right;">Thao tác</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="link in filtered" :key="link.id" style="border-bottom: 1px solid #f1f5f9; transition: background 0.15s;" class="hover:bg-slate-50">
              
              <td style="padding: 1rem 1.25rem;">
                <a :href="link.shortUrl" target="_blank" style="font-family: monospace; font-weight: 700; font-size: 0.95rem; color: #3b82f6; text-decoration: none;">
                  {{ link.slug }}
                </a>
              </td>
              
              <td style="padding: 1rem 1.25rem;">
                <p style="margin: 0; font-size: 0.85rem; color: #475569; font-weight: 500;">{{ link.userEmail }}</p>
              </td>
              
              <td style="padding: 1rem 1.25rem;">
                <div style="display: flex; gap: 1rem; align-items: center; font-size: 0.85rem;">
                  <span style="font-weight: 600; color: #0f172a;" title="Tổng clicks">{{ link.totalClicks }}</span>
                  <span style="color: #94a3b8; font-size: 0.75rem;" title="Bot clicks">(Bot: {{ link.botClicks }})</span>
                </div>
              </td>
              
              <td style="padding: 1rem 1.25rem;">
                <span v-if="link.highestRiskScore" class="ui-badge" :class="link.highestRiskScore > 70 ? 'ui-badge-error' : 'ui-badge-warning'" style="font-weight: 800;">
                  {{ link.highestRiskScore }}
                </span>
                <span v-else style="color: #cbd5e1; font-weight: 600;">—</span>
              </td>
              
              <td style="padding: 1rem 1.25rem;">
                <span class="ui-badge" :class="link.status === 'Active' ? 'ui-badge-success' : 'ui-badge-neutral'">
                  {{ link.status }}
                </span>
              </td>
              
              <td style="padding: 1rem 1.25rem;">
                <span style="font-size: 0.85rem; color: #64748b; font-variant-numeric: tabular-nums;">
                  {{ new Date(link.createdAtUtc).toLocaleDateString('vi-VN') }}
                </span>
              </td>

              <td style="padding: 1rem 1.25rem; text-align: right;">
                <div style="display: flex; items-center; justify-content: flex-end; gap: 0.75rem;">
                  <RouterLink :to="{ name: 'admin-link-detail', params: { id: link.id } }" style="font-size: 0.85rem; font-weight: 600; color: #3b82f6; text-decoration: none;">
                    Chi tiết
                  </RouterLink>
                  <button
                    @click="toggleLinkStatus(link)"
                    :disabled="isActionLoading"
                    style="background: transparent; border: 0; font-size: 0.85rem; font-weight: 600; cursor: pointer;"
                    :style="link.status === 'Active' ? 'color: #ef4444;' : 'color: #10b981;'"
                  >
                    {{ link.status === 'Active' ? 'Khóa' : 'Mở' }}
                  </button>
                </div>
              </td>
            </tr>
          </tbody>
        </table>
      </div>

    </div>
  </div>
</template>
