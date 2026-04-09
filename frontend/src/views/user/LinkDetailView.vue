<script setup lang="ts">
import { computed, onMounted, ref } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { LinkService, UserService } from '@/api/services'
import { useAuthStore } from '@/stores/auth'
import type { LinkDetail } from '@/types/api'
import LinkRulesPanel from '@/components/user/LinkRulesPanel.vue'
import {
  ArrowLeft, FileOutput, MousePointerClick, Users, Tag, Calendar,
  Settings, Link as LinkIcon, Info, Pause, Play, Trash2, Activity,
  Lock, Eye, AlertCircle, Link2
} from 'lucide-vue-next'

const route = useRoute()
const router = useRouter()
const authStore = useAuthStore()

const loading = ref(true)
const actionLoading = ref(false)
const error = ref('')
const message = ref('')
const detail = ref<LinkDetail | null>(null)
const currentPlanId = ref(1)
const confirmDelete = ref(false)

const linkId = computed(() => String(route.params.id ?? ''))
const isPro = computed(() => currentPlanId.value >= 2)

async function load() {
  loading.value = true
  error.value = ''
  try {
    if (!authStore.accessToken) throw new Error('Chưa xác thực.')
    const [linkData, subscription] = await Promise.all([
      LinkService.detail(authStore.accessToken, linkId.value),
      UserService.getSubscription(authStore.accessToken),
    ])
    detail.value = linkData
    currentPlanId.value = (subscription as any)?.planId ?? 1
  } catch (err) {
    error.value = err instanceof Error ? err.message : 'Không thể tải chi tiết link.'
  } finally {
    loading.value = false
  }
}

async function toggleStatus() {
  if (!authStore.accessToken || !detail.value) return
  actionLoading.value = true
  error.value = ''
  message.value = ''
  try {
    const newStatus = detail.value.status !== 'Active'
    await LinkService.updateStatus(authStore.accessToken, detail.value.id, newStatus)
    message.value = newStatus ? 'Đã kích hoạt lại link.' : 'Đã tạm dừng link.'
    await load()
  } catch (err) {
    error.value = err instanceof Error ? err.message : 'Không thể cập nhật trạng thái link.'
  } finally {
    actionLoading.value = false
  }
}

async function deleteLink() {
  if (!authStore.accessToken || !detail.value) return

  actionLoading.value = true
  error.value = ''
  message.value = ''
  try {
    const response = await LinkService.delete(authStore.accessToken, detail.value.id)
    message.value = response.message
    router.push('/app/links')
  } catch (err) {
    error.value = err instanceof Error ? err.message : 'Không thể xóa link.'
  } finally {
    actionLoading.value = false
    confirmDelete.value = false
  }
}

const linkHost = computed(() => {
  if (!detail.value) return ''
  try {
    return new URL(detail.value.shortUrl).host
  } catch {
    return 'Liên kết không hợp lệ'
  }
})

const exportLoading = ref(false)
async function exportCsv() {
  if (!authStore.accessToken || !detail.value) return
  exportLoading.value = true
  try {
    await UserService.exportLinkAnalyticsCsv(authStore.accessToken, detail.value.id)
  } catch (err) {
    error.value = err instanceof Error ? err.message : 'Không thể xuất dữ liệu.'
  } finally {
    exportLoading.value = false
  }
}

function formatDate(ds: string | null) {
  if (!ds) return '—'
  return new Date(ds).toLocaleString('vi-VN', {
    hour: '2-digit', minute: '2-digit', day: '2-digit', month: '2-digit', year: 'numeric'
  })
}

onMounted(load)
</script>

<template>
  <div class="ui-root">
    <div class="ui-header">
      <div class="ui-header-left">
        <button
          style="display: inline-flex; align-items: center; gap: 0.25rem; font-size: 0.85rem; font-weight: 600; color: #64748b; background: transparent; border: 0; padding: 0 0 0.5rem 0; cursor: pointer;"
          @click="router.push('/app/links')"
        >
          <ArrowLeft :size="14" /> Quay lại danh sách
        </button>
        <h1 class="ui-title">Chi tiết Link</h1>
        <p class="ui-subtitle">Xem cấu hình, thống kê nhanh và quản lý toàn diện link rút gọn.</p>
      </div>
      <div>
        <button v-if="isPro" class="ui-btn ui-btn-outline" style="border-color: #10b981; color: #059669;" :disabled="exportLoading" @click="exportCsv">
          <FileOutput :size="15" /> {{ exportLoading ? 'Đang xuất...' : 'Xuất báo cáo CSV' }}
        </button>
      </div>
    </div>

    <div v-if="message" class="ui-alert ui-alert-success">
      <Info :size="16" /> {{ message }}
    </div>
    <div v-if="error" class="ui-alert ui-alert-error">
      <AlertCircle :size="16" /> {{ error }}
    </div>

    <div v-if="loading" class="ui-skeleton" style="height: 320px;" />

    <template v-else-if="detail">
      <div class="ui-grid-2col" style="align-items: start;">
        <div class="ui-panel">
          <div class="ui-panel-body">
            <div style="display: flex; align-items: flex-start; justify-content: space-between; gap: 1rem;">
              <div style="min-width: 0;">
                <p class="ui-panel-eyebrow">Thông tin link</p>
                <h2 class="ui-panel-title" style="word-break: break-all;">{{ detail.shortUrl }}</h2>
                <p class="ui-panel-subtitle" style="word-break: break-all;">{{ detail.originalUrl }}</p>
              </div>
              <span class="ui-badge" :class="detail.status === 'Active' ? 'ui-badge-success' : 'ui-badge-warning'">
                {{ detail.status }}
              </span>
            </div>

            <div class="ui-stat-grid" style="margin-top: 1.25rem;">
              <div class="ui-stat-card">
                <MousePointerClick :size="18" />
                <div>
                  <div class="ui-stat-label">Tổng click</div>
                  <div class="ui-stat-value">{{ detail.totalClicks }}</div>
                </div>
              </div>
              <div class="ui-stat-card">
                <Users :size="18" />
                <div>
                  <div class="ui-stat-label">Click duy nhất</div>
                  <div class="ui-stat-value">{{ detail.uniqueClicks }}</div>
                </div>
              </div>
              <div class="ui-stat-card">
                <Link2 :size="18" />
                <div>
                  <div class="ui-stat-label">Host</div>
                  <div class="ui-stat-value">{{ linkHost }}</div>
                </div>
              </div>
              <div class="ui-stat-card">
                <Tag :size="18" />
                <div>
                  <div class="ui-stat-label">Tag</div>
                  <div class="ui-stat-value">{{ detail.tag || '—' }}</div>
                </div>
              </div>
            </div>

            <div style="margin-top: 1.25rem; display: flex; flex-direction: column; gap: 0.75rem;">
              <div class="ui-inline-item"><Calendar :size="15" /> Tạo lúc: {{ formatDate(detail.createdAtUtc) }}</div>
              <div class="ui-inline-item"><Eye :size="15" /> Mô tả: {{ detail.description || 'Chưa có mô tả' }}</div>
              <div class="ui-inline-item"><Lock :size="15" /> Mật khẩu: {{ detail.hasPassword ? 'Có bảo vệ' : 'Không' }}</div>
            </div>

            <div style="margin-top: 1.5rem; display: flex; flex-wrap: wrap; gap: 0.75rem;">
              <button class="ui-btn ui-btn-outline" :disabled="actionLoading" @click="toggleStatus">
                <Pause v-if="detail.status === 'Active'" :size="15" />
                <Play v-else :size="15" />
                {{ detail.status === 'Active' ? 'Tạm dừng' : 'Kích hoạt' }}
              </button>

              <template v-if="confirmDelete">
                <span class="ui-inline-item">Xóa link này?</span>
                <button class="ui-btn ui-btn-danger" :disabled="actionLoading" @click="deleteLink">
                  <Trash2 :size="15" /> Xóa
                </button>
                <button class="ui-btn ui-btn-ghost" @click="confirmDelete = false">Hủy</button>
              </template>
              <button v-else class="ui-btn ui-btn-ghost" style="color: #dc2626;" @click="confirmDelete = true">
                <Trash2 :size="15" /> Xóa mềm
              </button>
            </div>
          </div>
        </div>

        <div class="ui-panel">
          <div class="ui-panel-body">
            <div class="ui-panel-title-row">
              <Settings :size="16" />
              <h3 class="ui-panel-title">Điều hướng thông minh</h3>
            </div>
            <p class="ui-panel-subtitle">Quản lý rule theo thiết bị, quốc gia hoặc phân bổ theo trọng số.</p>
            <LinkRulesPanel :link-id="detail.id" :is-pro="isPro" />
          </div>
        </div>
      </div>
    </template>
  </div>
</template>
