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
  Lock, Eye, AlertCircle, Link2, Image
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
  if (!confirm('Bạn có chắc chắn muốn xóa link này không? (Xóa mềm)')) return
  
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
  if(!ds) return '—'
  return new Date(ds).toLocaleString('vi-VN', { 
    hour: '2-digit', minute: '2-digit', day: '2-digit', month: '2-digit', year: 'numeric' 
  })
}

onMounted(load)
</script>

<template>
  <div class="ui-root">
    
    <!-- Header -->
    <div class="ui-header">
      <div class="ui-header-left">
        <button 
          @click="router.push('/app/links')"
          style="display: inline-flex; align-items: center; gap: 0.25rem; font-size: 0.85rem; font-weight: 600; color: #64748b; background: transparent; border: 0; padding: 0 0 0.5rem 0; cursor: pointer;"
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

    <div v-if="message" class="ui-alert ui-alert-info">
      <Info :size="16" /> {{ message }}
    </div>
    <div v-if="error && !loading" class="ui-alert ui-alert-error">
      <AlertCircle :size="16" /> {{ error }}
    </div>

    <!-- Skeleton -->
    <div v-if="loading" class="ui-skeleton" style="height: 250px;" />

    <div v-else-if="!detail" class="ui-empty">
      <div class="ui-empty-icon"><Link2 :size="48"/></div>
      <h3 class="ui-empty-title">Không tìm thấy link</h3>
      <p class="ui-empty-desc">Liên kết không tồn tại, đã bị xóa hoặc bạn không có quyền truy cập.</p>
      <button class="ui-btn ui-btn-primary" style="margin-top: 1.5rem;" @click="router.push('/app/links')">Quay Lại</button>
    </div>

    <template v-else>
      
      <!-- General Box -->
      <div class="ui-panel">
        <div class="ui-panel-body" style="display: flex; flex-wrap: wrap; gap: 2rem; justify-content: space-between;">
          
          <div style="flex: 1; min-width: 300px; display: flex; flex-direction: column; gap: 1rem;">
            <div style="display: flex; align-items: center; gap: 1rem; flex-wrap: wrap;">
              <a :href="detail.shortUrl" target="_blank" style="font-size: 1.4rem; font-weight: 800; color: #3b82f6; text-decoration: none;">
                {{ detail.shortUrl }}
              </a>
              <span class="ui-badge" :class="detail.status === 'Active' ? 'ui-badge-success' : 'ui-badge-warning'">
                {{ detail.status === 'Active' ? 'Hoạt động' : 'Đang tạm dừng' }}
              </span>
            </div>
            
            <div style="display: flex; gap: 0.5rem; align-items: flex-start; max-width: 600px;">
              <div style="margin-top: 0.2rem; color: #94a3b8;"><LinkIcon :size="14" /></div>
              <a :href="detail.originalUrl" target="_blank" style="font-size: 0.9rem; color: #64748b; text-decoration: none; word-break: break-all;">
                {{ detail.originalUrl }}
              </a>
            </div>

            <!-- Stats Mini -->
            <div style="display: flex; gap: 1.5rem; margin-top: 0.5rem;">
               <div style="display: flex; align-items: center; gap: 0.4rem; color: #475569; font-size: 0.85rem;">
                 <MousePointerClick :size="14" />
                 <span>Traffic: <strong>{{ detail.totalClicks }}</strong></span>
               </div>
               <div style="display: flex; align-items: center; gap: 0.4rem; color: #475569; font-size: 0.85rem;">
                 <Users :size="14" />
                 <span>Unique: <strong>{{ detail.uniqueClicks }}</strong></span>
               </div>
               <div style="display: flex; align-items: center; gap: 0.4rem; color: #475569; font-size: 0.85rem;">
                 <Tag :size="14" />
                 <span>Slug: <strong>{{ detail.slug }}</strong></span>
               </div>
            </div>
          </div>

          <div style="display: flex; flex-direction: column; gap: 0.75rem;">
            <button 
              class="ui-btn" 
              :class="detail.status === 'Active' ? 'ui-btn-outline' : 'ui-btn-primary'"
              :style="detail.status === 'Active' ? 'color: #d97706; border-color: #fcd34d;' : ''"
              :disabled="actionLoading"
              @click="toggleStatus"
            >
              <Pause v-if="detail.status === 'Active'" :size="15" />
              <Play v-else :size="15" />
              {{ detail.status === 'Active' ? 'Tạm dừng Link' : 'Kích hoạt lại Link' }}
            </button>
            <button class="ui-btn ui-btn-outline" style="border-color: #3b82f6; color: #3b82f6;" @click="router.push(`/app/analytics?linkId=${detail.id}`)">
              <Activity :size="15" /> Xem Phân Tích
            </button>
            <button class="ui-btn ui-btn-ghost" style="color: #ef4444;" :disabled="actionLoading" @click="deleteLink">
              <Trash2 :size="15" /> Xóa Link này
            </button>
          </div>

        </div>
      </div>

      <!-- Config & Meta Grid -->
      <div class="ui-card-grid ui-card-grid-2">
        <div class="ui-panel">
          <div class="ui-panel-header">
            <h3 class="ui-panel-title" style="display: flex; align-items: center; gap: 0.5rem;">
              <Settings :size="16" style="color: #64748b;" /> Thiết lập & Cấu hình
            </h3>
          </div>
          <div class="ui-panel-body" style="padding: 0;">
            <div style="display: flex; flex-direction: column;">
              <div style="display: flex; justify-content: space-between; padding: 1rem 1.25rem; border-bottom: 1px solid #f1f5f9;">
                <span style="font-size: 0.85rem; color: #64748b;">Mật khẩu</span>
                <span style="font-size: 0.85rem; font-weight: 600; color: #0f172a;">
                  <Lock v-if="detail.hasPassword" :size="12" style="display:inline; margin-right: 0.2rem;"/>
                  {{ detail.hasPassword ? 'Đã thiết lập' : 'Không (Public)' }}
                </span>
              </div>
              <div style="display: flex; justify-content: space-between; padding: 1rem 1.25rem; border-bottom: 1px solid #f1f5f9;">
                <span style="font-size: 0.85rem; color: #64748b;">Giới hạn Click</span>
                <span style="font-size: 0.85rem; font-weight: 600; color: #0f172a;">{{ detail.clickLimit ?? 'Không giới hạn' }}</span>
              </div>
              <div style="display: flex; justify-content: space-between; padding: 1rem 1.25rem; border-bottom: 1px solid #f1f5f9;">
                <span style="font-size: 0.85rem; color: #64748b;">Ngày hết hạn</span>
                <span style="font-size: 0.85rem; font-weight: 600; color: #0f172a;">{{ formatDate(detail.expiresAtUtc) }}</span>
              </div>
              <div style="display: flex; justify-content: space-between; padding: 1rem 1.25rem; border-bottom: 1px solid #f1f5f9;">
                <span style="font-size: 0.85rem; color: #64748b;">Tag</span>
                <span style="font-size: 0.85rem; font-weight: 600; color: #0f172a;" class="ui-badge ui-badge-neutral">{{ detail.tag || '—' }}</span>
              </div>
              <div style="display: flex; justify-content: space-between; padding: 1rem 1.25rem;">
                <span style="font-size: 0.85rem; color: #64748b;">Mô tả</span>
                <span style="font-size: 0.85rem; font-weight: 500; color: #0f172a;">{{ detail.description || '—' }}</span>
              </div>
            </div>
          </div>
        </div>

        <div class="ui-panel">
          <div class="ui-panel-header">
            <h3 class="ui-panel-title" style="display: flex; align-items: center; gap: 0.5rem;">
              <Eye :size="16" style="color: #64748b;" /> Siêu dữ liệu (Meta)
            </h3>
          </div>
          <div class="ui-panel-body" style="padding: 0;">
            <div style="display: flex; flex-direction: column;">
              <div style="display: flex; justify-content: space-between; padding: 1rem 1.25rem; border-bottom: 1px solid #f1f5f9;">
                <span style="font-size: 0.85rem; color: #64748b;">Domain / Host</span>
                <span style="font-size: 0.85rem; font-weight: 600; color: #3b82f6;">{{ linkHost }}</span>
              </div>
              <div style="display: flex; justify-content: space-between; padding: 1rem 1.25rem; border-bottom: 1px solid #f1f5f9;">
                <span style="font-size: 0.85rem; color: #64748b;">Ngày khởi tạo</span>
                <span style="font-size: 0.85rem; font-weight: 600; color: #0f172a;">{{ formatDate(detail.createdAtUtc) }}</span>
              </div>
              <div style="display: flex; justify-content: space-between; padding: 1rem 1.25rem;">
                <span style="font-size: 0.85rem; color: #64748b;">Sửa đổi lần cuối</span>
                <span style="font-size: 0.85rem; font-weight: 600; color: #0f172a;">{{ formatDate(detail.updatedAtUtc) }}</span>
              </div>
            </div>
          </div>
        </div>
      </div>

      <div v-if="detail.isWrapperEnabled" class="ui-panel" style="margin-top: 1.5rem;">
        <div class="ui-panel-header">
          <h3 class="ui-panel-title" style="display: flex; align-items: center; gap: 0.5rem;">
            <Eye :size="16" style="color: #64748b;" /> Bọc link / Smart Redirect
          </h3>
        </div>
        <div class="ui-panel-body" style="display: grid; gap: 1rem; grid-template-columns: repeat(auto-fit, minmax(280px, 1fr));">
          <div style="display: flex; flex-direction: column; gap: 0.75rem;">
            <div style="font-size: 0.85rem; color: #64748b;">Mode: <strong style="color: #0f172a;">{{ detail.redirectMode || 'Instant' }}</strong></div>
            <div v-if="detail.delaySeconds" style="font-size: 0.85rem; color: #64748b;">Delay: <strong style="color: #0f172a;">{{ detail.delaySeconds }} giây</strong></div>
            <div style="font-size: 0.85rem; color: #64748b;">Theme: <strong style="color: #0f172a;">{{ detail.wrapperTheme || 'brand' }}</strong></div>
            <div style="font-size: 0.85rem; color: #64748b;">Thương hiệu: <strong style="color: #0f172a;">{{ detail.brandName || linkHost }}</strong></div>
            <div style="font-size: 0.85rem; color: #64748b;">Nút tiếp tục: <strong style="color: #0f172a;">{{ detail.continueButtonText || 'Tiếp tục đến trang đích' }}</strong></div>
            <div style="font-size: 0.85rem; color: #64748b;">Cảnh báo: <strong style="color: #0f172a;">{{ detail.warningText || 'Bạn sắp rời khỏi website hiện tại.' }}</strong></div>
          </div>
          <div style="background: #0f172a; color: #e2e8f0; border-radius: 16px; overflow: hidden; border: 1px solid rgba(148,163,184,0.2);">
            <div :style="{ padding: '1rem 1.25rem', background: detail.wrapperTheme === 'dark' ? 'linear-gradient(135deg,#111827,#1f2937)' : detail.wrapperTheme === 'light' ? 'linear-gradient(135deg,#e2e8f0,#cbd5e1)' : 'linear-gradient(135deg,#0ea5e9,#2563eb)', color: detail.wrapperTheme === 'light' ? '#0f172a' : '#fff' }">
              <strong>{{ detail.brandName || linkHost }}</strong>
            </div>
            <div style="padding: 1rem 1.25rem; display: flex; flex-direction: column; gap: 0.75rem;">
              <img v-if="detail.wrapperImageUrl" :src="detail.wrapperImageUrl" style="width: 100%; height: 140px; object-fit: cover; border-radius: 12px;" />
              <div style="font-size: 1rem; font-weight: 700;">{{ detail.wrapperTitle || detail.ogTitle || 'Bạn sắp đến trang đích' }}</div>
              <div style="font-size: 0.85rem; color: #cbd5e1;">{{ detail.wrapperDescription || detail.ogDescription || 'Xác nhận trước khi tiếp tục mở liên kết.' }}</div>
              <div style="font-size: 0.75rem; color: #fde68a; background: rgba(245,158,11,0.12); border: 1px solid rgba(245,158,11,0.35); border-radius: 10px; padding: 0.65rem 0.75rem;">
                {{ detail.warningText || 'Bạn sắp rời khỏi website hiện tại.' }}
              </div>
              <div v-if="detail.ctaTitle" style="padding: 0.9rem 1rem; border-radius: 12px; background: rgba(30,41,59,0.8); border: 1px solid rgba(148,163,184,0.16);">
                <div style="font-weight: 700;">{{ detail.ctaTitle }}</div>
                <div style="font-size: 0.8rem; color: #cbd5e1; margin-top: 0.35rem;">{{ detail.ctaDescription }}</div>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Social Preview Meta Grid -->
      <div class="ui-panel" style="margin-top: 1.5rem;">
        <div class="ui-panel-header">
          <h3 class="ui-panel-title" style="display: flex; align-items: center; gap: 0.5rem;">
            <Image :size="16" style="color: #64748b;" /> Xem trước Mạng xã hội (OpenGraph)
          </h3>
        </div>
        <div class="ui-panel-body" style="background: #f8fafc; padding: 1.5rem; display: flex; justify-content: center;">
          <div style="background: white; border-radius: 12px; box-shadow: 0 1px 4px rgba(0,0,0,0.08); border: 1px solid #e2e8f0; overflow: hidden; max-width: 500px; width: 100%;">
            <!-- Cover image -->
            <div style="width: 100%; height: 200px; background: #f1f5f9; display: flex; align-items: center; justify-content: center; overflow: hidden; border-bottom: 1px solid #f1f5f9;">
              <img v-if="detail.ogImageUrl" :src="detail.ogImageUrl" style="width: 100%; height: 100%; object-fit: cover;" />
              <div v-else style="display: flex; flex-direction: column; align-items: center; color: #94a3b8;">
                <Image :size="32" style="opacity: 0.5; margin-bottom: 8px;" />
                <span style="font-size: 11px; font-weight: 600; letter-spacing: 0.1em;">KHÔNG CÓ ẢNH</span>
              </div>
            </div>
            <!-- Content -->
            <div style="padding: 1rem; background: #f8fafc;">
              <p style="font-size: 11px; color: #94a3b8; font-weight: 600; text-transform: uppercase; letter-spacing: 0.06em; margin: 0 0 6px; overflow: hidden; white-space: nowrap; text-overflow: ellipsis;">
                {{ linkHost.toUpperCase() }}/{{ detail.slug.toUpperCase() }}
              </p>
              <h4 style="font-size: 16px; font-weight: 700; color: #0f172a; line-height: 1.35; margin: 0 0 6px; display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; word-break: break-word;">
                {{ detail.ogTitle || detail.originalUrl || 'WeShort - Hệ thống chuyển hướng' }}
              </h4>
              <p style="font-size: 14px; color: #64748b; margin: 0; display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; word-break: break-word;">
                {{ detail.ogDescription || 'Không có mô tả mạng xã hội nào được cung cấp.' }}
              </p>
            </div>
          </div>
        </div>
      </div>

      <!-- Link Rules Panel: Bypassing internal styles by keeping the component but wrapping it in a layout standard if needed.
           Actually we just put it here, the component handles itself. We'll leave it as is. -->
      <LinkRulesPanel
        :link-id="detail.id"
        :is-pro="isPro"
      />

    </template>
  </div>
</template>
