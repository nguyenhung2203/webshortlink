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

// ─── OG Edit State ─────────────────────────────────────────────────────────────
const ogForm = ref({ ogTitle: '', ogDescription: '', ogImageUrl: '' })
const ogSaving = ref(false)
const ogError = ref('')
const ogSuccess = ref('')

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
    // Populate OG form from saved data
    ogForm.value = {
      ogTitle: linkData.ogTitle ?? '',
      ogDescription: linkData.ogDescription ?? '',
      ogImageUrl: linkData.ogImageUrl ?? '',
    }
  } catch (err) {
    error.value = err instanceof Error ? err.message : 'Không thể tải chi tiết link.'
  } finally {
    loading.value = false
  }
}

async function saveOg() {
  if (!authStore.accessToken || !detail.value) return
  ogError.value = ''
  ogSuccess.value = ''

  if (ogForm.value.ogImageUrl && !ogForm.value.ogImageUrl.startsWith('https://')) {
    ogError.value = 'Ảnh xem trước phải là link https:// công khai.'
    return
  }

  ogSaving.value = true
  try {
    await LinkService.update(authStore.accessToken, detail.value.id, {
      originalUrl: detail.value.originalUrl,
      domainId: (detail.value as any).domainId ?? null,
      description: detail.value.description ?? '',
      tag: detail.value.tag ?? '',
      expiresAtUtc: (detail.value as any).expiresAtUtc ?? null,
      clickLimit: (detail.value as any).clickLimit ?? null,
      password: undefined,
      ogTitle: ogForm.value.ogTitle || undefined,
      ogDescription: ogForm.value.ogDescription || undefined,
      ogImageUrl: ogForm.value.ogImageUrl || undefined,
    })
    ogSuccess.value = 'Đã lưu thẻ xem trước mạng xã hội!'
    await load()
    setTimeout(() => { ogSuccess.value = '' }, 3000)
  } catch (err) {
    ogError.value = err instanceof Error ? err.message : 'Không thể lưu.'
  } finally {
    ogSaving.value = false
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

      <!-- Social Preview: Edit + Live Preview -->
      <div class="ui-panel" style="margin-top: 1.5rem;">
        <div class="ui-panel-header">
          <h3 class="ui-panel-title" style="display: flex; align-items: center; gap: 0.5rem;">
            <Image :size="16" style="color: #64748b;" /> Xem trước Mạng xã hội (OpenGraph)
          </h3>
        </div>
        <div class="ui-panel-body">
          <!-- Pro-only gate -->
          <div v-if="!isPro" style="padding: 1rem; background: #fffbeb; border-radius: 8px; border: 1px solid #fde68a; margin-bottom: 1.25rem; font-size: 0.875rem; color: #92400e;">
            ⚠️ Tính năng tùy chỉnh thẻ xem trước MXH (OpenGraph) yêu cầu gói <strong>Pro</strong> hoặc <strong>Plus</strong>. <a href="/app/billing" style="color:#2563eb;font-weight:600;">Nâng cấp ngay</a>
          </div>

          <div style="display: flex; flex-wrap: wrap; gap: 2rem;">
            <!-- Edit Form -->
            <div style="flex: 1; min-width: 260px;">
              <div style="display: flex; flex-direction: column; gap: 0.75rem;">
                <div>
                  <label style="font-size: 11px; font-weight: 700; color: #94a3b8; text-transform: uppercase; letter-spacing: 0.06em; display: block; margin-bottom: 0.35rem;">Tiêu đề (og:title)</label>
                  <input
                    v-model="ogForm.ogTitle"
                    class="ui-form-input"
                    placeholder="Nhập tiêu đề hiển thị khi chia sẻ..."
                    maxlength="150"
                    :disabled="!isPro"
                    style="font-size: 0.875rem;"
                  />
                </div>
                <div>
                  <label style="font-size: 11px; font-weight: 700; color: #94a3b8; text-transform: uppercase; letter-spacing: 0.06em; display: block; margin-bottom: 0.35rem;">Mô tả (og:description)</label>
                  <textarea
                    v-model="ogForm.ogDescription"
                    class="ui-form-input"
                    placeholder="Mô tả ngắn hiển thị bên dưới tiêu đề..."
                    maxlength="500"
                    :disabled="!isPro"
                    style="font-size: 0.875rem; height: 72px; resize: none;"
                  ></textarea>
                </div>
                <div>
                  <label style="font-size: 11px; font-weight: 700; color: #94a3b8; text-transform: uppercase; letter-spacing: 0.06em; display: block; margin-bottom: 0.35rem;">Link ảnh (og:image)</label>
                  <input
                    v-model="ogForm.ogImageUrl"
                    class="ui-form-input"
                    placeholder="https://example.com/anh.jpg"
                    :disabled="!isPro"
                    style="font-size: 0.875rem;"
                  />
                  <p v-if="ogForm.ogImageUrl && !ogForm.ogImageUrl.startsWith('https://')" style="font-size: 11px; color: #ef4444; margin: 4px 0 0; font-weight: 500;">
                    ⚠️ URL phải bắt đầu bằng https://
                  </p>
                  <p v-else style="font-size: 11px; color: #94a3b8; margin: 4px 0 0;">Link ảnh công khai (bắt đầu bằng https://)</p>
                </div>

                <div v-if="ogError" style="font-size: 12px; color: #ef4444; font-weight: 500; padding: 0.5rem 0.75rem; background: #fef2f2; border-radius: 6px; border: 1px solid #fecaca;">
                  {{ ogError }}
                </div>
                <div v-if="ogSuccess" style="font-size: 12px; color: #059669; font-weight: 500; padding: 0.5rem 0.75rem; background: #ecfdf5; border-radius: 6px; border: 1px solid #a7f3d0;">
                  ✅ {{ ogSuccess }}
                </div>

                <button
                  v-if="isPro"
                  class="ui-btn ui-btn-primary"
                  :disabled="ogSaving"
                  @click="saveOg"
                  style="align-self: flex-start;"
                >
                  {{ ogSaving ? 'Đang lưu...' : 'Lưu thay đổi OG' }}
                </button>
              </div>
            </div>

            <!-- Live Preview -->
            <div style="flex: 1; min-width: 260px;">
              <label style="font-size: 11px; font-weight: 700; color: #94a3b8; text-transform: uppercase; letter-spacing: 0.06em; display: block; margin-bottom: 0.5rem;">Xem trước card MXH:</label>
              <div style="background: white; border-radius: 12px; box-shadow: 0 1px 4px rgba(0,0,0,0.08); border: 1px solid #e2e8f0; overflow: hidden;">
                <div style="width: 100%; height: 180px; background: #f1f5f9; display: flex; align-items: center; justify-content: center; overflow: hidden;">
                  <img v-if="ogForm.ogImageUrl && ogForm.ogImageUrl.startsWith('https://')" :src="ogForm.ogImageUrl" style="width: 100%; height: 100%; object-fit: cover;" />
                  <div v-else style="display: flex; flex-direction: column; align-items: center; color: #94a3b8;">
                    <Image :size="32" style="opacity: 0.4; margin-bottom: 8px;" />
                    <span style="font-size: 11px; font-weight: 600; letter-spacing: 0.1em;">CHƯA CÓ ẢNH</span>
                  </div>
                </div>
                <div style="padding: 0.875rem; background: #f8fafc;">
                  <p style="font-size: 11px; color: #94a3b8; font-weight: 600; text-transform: uppercase; letter-spacing: 0.06em; margin: 0 0 4px;">{{ linkHost }}</p>
                  <h4 style="font-size: 15px; font-weight: 700; color: #0f172a; margin: 0 0 4px; overflow: hidden; display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical;">
                    {{ ogForm.ogTitle || 'WeShort Link' }}
                  </h4>
                  <p style="font-size: 13px; color: #64748b; margin: 0; overflow: hidden; display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical;">
                    {{ ogForm.ogDescription || 'Tiết kiệm thời gian với đường link rút gọn chuyên nghiệp.' }}
                  </p>
                </div>
              </div>
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
