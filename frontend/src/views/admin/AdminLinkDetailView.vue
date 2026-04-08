<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { AdminService } from '@/api/services'
import { useAuthStore } from '@/stores/auth'
import { ArrowLeft, Link2, Ban, CheckCircle, ExternalLink, ShieldAlert, MousePointerClick, RefreshCw, CalendarDays, Key, Globe } from 'lucide-vue-next'

const route = useRoute()
const router = useRouter()
const authStore = useAuthStore()

const linkId = route.params.id as string
const link = ref<any>(null)
const error = ref('')
const loading = ref(true)
const actionLoading = ref(false)

async function load() {
  loading.value = true
  error.value = ''
  try {
    if (!authStore.accessToken) throw new Error('Chưa xác thực.')
    link.value = await AdminService.getLinkDetail(authStore.accessToken, linkId)
  } catch (err) {
    error.value = err instanceof Error ? err.message : 'Không thể tải thông tin chi tiết.'
  } finally {
    loading.value = false
  }
}

async function toggleBlock() {
  if (!authStore.accessToken || !link.value) return
  const action: 'block' | 'unblock' = link.value.status === 'DisabledByAdmin' ? 'unblock' : 'block'
  if (!confirm(`Hệ thống sẽ ${action === 'block' ? 'CHẶN' : 'KHÔI PHỤC'} truy cập vào liên kết này. Đồng ý thực thi?`)) return
  actionLoading.value = true
  try {
    await AdminService.toggleLinkBlock(authStore.accessToken, linkId, action)
    await load()
  } catch (err) {
    alert(err instanceof Error ? err.message : 'Thao tác bảo mật thất bại.')
  } finally {
    actionLoading.value = false
  }
}

onMounted(load)
</script>

<template>
  <div class="ui-root">
    
    <div style="margin-bottom: 1rem;">
      <button @click="router.back()" style="display: flex; align-items: center; gap: 0.5rem; font-size: 0.85rem; font-weight: 600; color: #64748b; background: transparent; border: none; cursor: pointer;">
        <ArrowLeft :size="16" /> Trở về danh sách Liên kết
      </button>
    </div>

    <div class="ui-header">
      <div class="ui-header-left">
        <div class="ui-eyebrow"><Link2 :size="13" /> Phân giải Dữ liệu Liên kết</div>
        <h1 class="ui-title">Cấu hình & Giám sát</h1>
        <p class="ui-subtitle">Xem thông tin định tuyến, đánh giá rủi ro và thực thi quản lý tập trung.</p>
      </div>
      <div>
        <button class="ui-btn ui-btn-outline" @click="load" :disabled="loading" style="color: #0f172a;">
          <RefreshCw :size="14" :class="{'animate-spin': loading}" /> Khôi phục Logs
        </button>
      </div>
    </div>

    <div v-if="error" class="ui-alert ui-alert-error" style="background: white;">
      <ShieldAlert :size="16" /> {{ error }}
    </div>

    <div v-if="loading" style="display: flex; flex-direction: column; gap: 1.5rem;">
      <div class="ui-skeleton" style="height: 150px; border-radius: 12px;" />
      <div class="ui-skeleton" style="height: 250px; border-radius: 12px;" />
    </div>

    <template v-else-if="link">
      
      <div style="display: flex; flex-direction: column; gap: 1.5rem;">
        
        <!-- Header Panel -->
        <div class="ui-panel" style="background: white; border-top: 4px solid #10b981;">
          <div class="ui-panel-body" style="padding: 2rem; display: flex; flex-direction: column; gap: 2rem;">
            
            <div style="display: flex; align-items: flex-start; gap: 1.5rem;">
              <div style="width: 64px; height: 64px; border-radius: 16px; background: #ecfdf5; border: 1px solid #10b981; display: grid; place-items: center; color: #10b981;">
                <Link2 :size="32" />
              </div>
              <div style="flex: 1; min-width: 0;">
                <h2 style="margin: 0; font-size: 1.5rem; font-weight: 800; color: #0f172a; display: flex; align-items: center; gap: 0.5rem; flex-wrap: wrap;">
                  {{ link.shortUrl }}
                  <span class="ui-badge" :class="link.status === 'Active' ? 'ui-badge-success' : 'ui-badge-error'">
                    {{ link.status === 'Active' ? 'HOẠT ĐỘNG' : 'ĐANG BỊ CHẶN' }}
                  </span>
                </h2>
                
                <a :href="link.originalUrl" target="_blank" style="display: flex; align-items: center; gap: 0.5rem; font-size: 0.95rem; color: #3b82f6; text-decoration: none; margin-top: 0.5rem; margin-bottom: 1rem; word-break: break-all;">
                  {{ link.originalUrl }} <ExternalLink :size="14" />
                </a>

                <div style="display: flex; gap: 0.75rem; align-items: center; flex-wrap: wrap;">
                  <span v-if="link.hasPassword" class="ui-badge" style="background: #fffbeb; color: #b45309; border: 1px solid #fde68a;">
                    <Key :size="12" style="margin-right: 0.25rem;" /> Protect
                  </span>
                  <span class="ui-badge" style="background: #f1f5f9; color: #475569; border: 1px solid #e2e8f0;">
                    <Globe :size="12" style="margin-right: 0.25rem;" /> Domain {{ link.domain || 'Nội bộ' }}
                  </span>
                  <span v-if="link.expiresAtUtc" class="ui-badge" style="background: #f1f5f9; color: #475569; border: 1px solid #e2e8f0;">
                    <CalendarDays :size="12" style="margin-right: 0.25rem;" /> Exp {{ new Date(link.expiresAtUtc).toLocaleDateString('vi-VN') }}
                  </span>
                </div>
              </div>
            </div>

            <div style="display: grid; grid-template-columns: repeat(auto-fit, minmax(200px, 1fr)); gap: 1rem; border-top: 1px solid #f1f5f9; padding-top: 2rem;">
              <div>
                <div style="font-size: 0.75rem; font-weight: 700; color: #94a3b8; text-transform: uppercase; margin-bottom: 0.5rem; display: flex; align-items: center; gap: 0.3rem;"><MousePointerClick :size="14" /> Clicks Ingest</div>
                <div style="font-size: 1.5rem; font-weight: 800; color: #0f172a;">{{ link.totalClicks ?? 0 }}</div>
              </div>
              <div>
                <div style="font-size: 0.75rem; font-weight: 700; color: #94a3b8; text-transform: uppercase; margin-bottom: 0.5rem; display: flex; align-items: center; gap: 0.3rem;"><MousePointerClick :size="14" /> Unique</div>
                <div style="font-size: 1.5rem; font-weight: 800; color: #0f172a;">{{ link.uniqueClicks ?? 0 }}</div>
              </div>
              <div>
                <div style="font-size: 0.75rem; font-weight: 700; color: #94a3b8; text-transform: uppercase; margin-bottom: 0.5rem; display: flex; align-items: center; gap: 0.3rem;"><ShieldAlert :size="14" /> Bot Request</div>
                <div style="font-size: 1.5rem; font-weight: 800;" :style="link.botClicks > 0 ? 'color: #ef4444;' : 'color: #0f172a;'">{{ link.botClicks ?? 0 }}</div>
              </div>
            </div>

          </div>
        </div>

        <div style="display: grid; grid-template-columns: repeat(auto-fit, minmax(300px, 1fr)); gap: 1.5rem;">
          
          <!-- System Metadata -->
          <div class="ui-panel" style="background: white;">
            <div class="ui-panel-header" style="border-bottom: 1px solid #f1f5f9; padding: 1.25rem 1.5rem;">
              <h3 class="ui-panel-title">Metadata Cấu hình</h3>
            </div>
            <div class="ui-panel-body" style="padding: 1.5rem;">
              <table style="width: 100%; border-collapse: collapse; text-align: left;">
                <tbody>
                  <tr style="border-bottom: 1px solid #f1f5f9;">
                    <td style="padding: 0.75rem 0; font-size: 0.85rem; color: #64748b; font-weight: 600;">Sở hữu bởi</td>
                    <td style="padding: 0.75rem 0; font-size: 0.85rem; color: #0f172a; font-weight: 700;">{{ link.userEmail || '-' }}</td>
                  </tr>
                  <tr style="border-bottom: 1px solid #f1f5f9;">
                    <td style="padding: 0.75rem 0; font-size: 0.85rem; color: #64748b; font-weight: 600;">Slug định danh</td>
                    <td style="padding: 0.75rem 0; font-size: 0.85rem; color: #0f172a; font-family: monospace;">{{ link.slug }}</td>
                  </tr>
                  <tr style="border-bottom: 1px solid #f1f5f9;">
                    <td style="padding: 0.75rem 0; font-size: 0.85rem; color: #64748b; font-weight: 600;">Giới hạn Click</td>
                    <td style="padding: 0.75rem 0; font-size: 0.85rem; color: #0f172a; font-weight: 700;">{{ link.clickLimit ?? 'Vô hạn (Không giới hạn)' }}</td>
                  </tr>
                  <tr>
                    <td style="padding: 0.75rem 0; font-size: 0.85rem; color: #64748b; font-weight: 600;">Cập nhật lần cuối</td>
                    <td style="padding: 0.75rem 0; font-size: 0.85rem; color: #0f172a; font-weight: 600;">{{ link.updatedAtUtc ? new Date(link.updatedAtUtc).toLocaleDateString('vi-VN') : '-' }}</td>
                  </tr>
                </tbody>
              </table>
            </div>
          </div>

          <!-- Ops Control -->
          <div class="ui-panel" style="background: white; border-color: #fecdd3;">
            <div class="ui-panel-header" style="background: #fff1f2; border-bottom: 1px solid #ffe4e6; padding: 1.25rem 1.5rem;">
              <h3 class="ui-panel-title" style="color: #e11d48; display: flex; align-items: center; gap: 0.5rem;"><ShieldAlert :size="16" /> Trung tâm Cảnh trị</h3>
            </div>
            <div class="ui-panel-body" style="padding: 1.5rem; display: flex; flex-direction: column; gap: 1rem;">
              
              <p style="margin: 0; font-size: 0.85rem; color: #64748b; line-height: 1.5;">Hệ thống phát hiện link này có nguy cơ trỏ đến các nội dung độc hại? Thực thi quyền quản trị tối cao để khóa truy cập công khai vào liên kết này.</p>
              
              <div style="margin-top: 1rem;">
                <button
                  @click="toggleBlock"
                  :disabled="actionLoading"
                  class="ui-btn"
                  :style="link.status === 'Active'
                    ? 'background: #e11d48; color: white; border: none; font-weight: 800;'
                    : 'background: #10b981; color: white; border: none; font-weight: 800;'"
                >
                  <Ban v-if="link.status === 'Active'" :size="16" />
                  <CheckCircle v-else :size="16" />
                  {{ link.status === 'Active' ? 'VÔ HIỆU HÓA LIÊN KẾT (BLOCK PATH)' : 'KHÔI PHỤC VẬN HÀNH (UNBLOCK)' }}
                </button>
              </div>

            </div>
          </div>

        </div>

      </div>
    </template>
  </div>
</template>
