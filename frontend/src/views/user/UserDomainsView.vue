<script setup lang="ts">
import { onMounted, ref, computed } from 'vue'
import { DomainService, UserService } from '@/api/services'
import { useAuthStore } from '@/stores/auth'
import { Globe, Plus, CheckCircle, Clock, Shield, Trash2, RefreshCw, Lock, AlertCircle, Copy } from 'lucide-vue-next'
import { useRouter } from 'vue-router'

const router = useRouter()
const authStore = useAuthStore()

const domains = ref<any[]>([])
const loading = ref(true)
const error = ref('')
const successMessage = ref('')
const showAdd = ref(false)
const newDomainForm = ref({ host: '', expiredAt: '', notes: '' })
const addLoading = ref(false)
const confirmDeleteId = ref<string | null>(null)

const currentPlanId = ref(1)
const isPro = computed(() => currentPlanId.value >= 2)

async function load() {
  loading.value = true
  try {
    if (!authStore.accessToken) return
    const [list, subscription] = await Promise.all([
      DomainService.list(authStore.accessToken),
      UserService.getSubscription(authStore.accessToken),
    ])
    domains.value = list
    currentPlanId.value = (subscription as any)?.planId ?? 1
  } catch (err) {
    error.value = err instanceof Error ? err.message : 'Không thể tải danh sách tên miền.'
  } finally {
    loading.value = false
  }
}

async function addDomain() {
  if (!authStore.accessToken || !newDomainForm.value.host.trim()) return
  addLoading.value = true
  error.value = ''
  try {
    await DomainService.create(authStore.accessToken, 
      newDomainForm.value.host.trim(), 
      newDomainForm.value.expiredAt || undefined,
      newDomainForm.value.notes || undefined
    )
    newDomainForm.value = { host: '', expiredAt: '', notes: '' }
    showAdd.value = false
    await load()
  } catch (err) {
    error.value = err instanceof Error ? err.message : 'Không thể thêm tên miền.'
  } finally {
    addLoading.value = false
  }
}

const formatDate = (date: string | null) => {
  if (!date) return 'Vô thời hạn'
  return new Date(date).toLocaleDateString('vi-VN')
}

async function deleteDomain(domainId: string) {
  if (!authStore.accessToken) return
  try {
    await DomainService.delete(authStore.accessToken, domainId)
    successMessage.value = 'Đã xóa tên miền thành công.'
    confirmDeleteId.value = null
    setTimeout(() => successMessage.value = '', 5000)
    await load()
  } catch (err) {
    error.value = err instanceof Error ? err.message : 'Không thể xóa tên miền.'
  }
}

onMounted(load)
</script>

<template>
  <div class="ui-root">
    <!-- Action Bar -->
    <div class="ui-action-bar">
      <button v-if="isPro" class="ui-btn ui-btn-primary" @click="showAdd = !showAdd">
        <Plus :size="16" /> Thêm Tên Miền
      </button>
    </div>

    <!-- Notifications -->
    <div v-if="successMessage" class="ui-alert ui-alert-success mt-alert">
      <CheckCircle :size="16"/> {{ successMessage }}
    </div>
    <div v-if="error && !loading" class="ui-alert ui-alert-error mt-alert">
      <AlertCircle :size="16"/> {{ error }}
    </div>

    <!-- Loading -->
    <div v-if="loading" class="ui-skeleton" style="height: 200px;" />

    <template v-else>
      <!-- Paywall for Free Users -->
      <div v-if="!isPro" class="ui-empty">
        <div class="ui-empty-icon locked-icon"><Lock :size="50" /></div>
        <h3 class="ui-empty-title">Tính năng dành cho gói Pro / Plus</h3>
        <p class="ui-empty-desc">Nâng cấp tài khoản để sử dụng tên miền riêng trên các link rút gọn của bạn.</p>
        <button class="ui-btn ui-btn-primary mt-4" @click="router.push('/app/billing')">
          Xem bảng giá nâng cấp
        </button>
      </div>

      <!-- Feature Content -->
      <div v-else class="domain-content">
        
        <!-- Instruction Card -->
        <div class="ui-panel instruction-panel">
          <div class="ui-panel-body flex-gap">
            <Shield :size="24" class="icon-shield" />
            <div>
              <h3 class="instruction-title">Quy trình sử dụng Tên miền riêng</h3>
              <p class="instruction-desc">
                1. Khai báo tên miền bạn sở hữu (ví dụ: link.brand.com). <br>
                2. Cấu hình DNS tại nhà cung cấp tên miền của bạn (trỏ CNAME về <strong>wesortlink.io.vn</strong>). <br>
                3. Đội ngũ kỹ thuật của chúng tôi sẽ thiết lập máy chủ và cấp chứng chỉ SSL cho bạn. <br>
                4. Sau khi Admin phê duyệt, bạn có thể bắt đầu rút gọn link với thương hiệu riêng.
              </p>
            </div>
          </div>
        </div>

        <!-- Add Domain Form Panel -->
        <div v-if="showAdd" class="ui-panel add-panel">
          <div class="ui-panel-body">
            <h3 class="instruction-title mb-4">Kết nối tên miền mới</h3>
            <div style="display: flex; flex-direction: column; gap: 1rem;">
              <div class="form-group-vertical">
                <label class="ui-label">Tên miền (Host)</label>
                <input
                  v-model="newDomainForm.host"
                  type="text"
                  class="ui-form-input"
                  placeholder="Ví dụ: go.yourbrand.com"
                />
              </div>

              <div class="form-row-multi">
                <div class="form-col">
                   <label class="ui-label">Hạn sử dụng (Nếu có)</label>
                   <input v-model="newDomainForm.expiredAt" type="date" class="ui-form-input" style="width: 100%;" />
                </div>
                <div class="form-col">
                   <label class="ui-label">Ghi chú thêm (Đuôi miền, yêu cầu đặc biệt...)</label>
                   <input v-model="newDomainForm.notes" type="text" class="ui-form-input" placeholder="Ví dụ: .vn, dùng cho chiến dịch hè..." style="width: 100%;" />
                </div>
              </div>

              <div style="display: flex; gap: 0.5rem; justify-content: flex-end; margin-top: 0.5rem;">
                <button class="ui-btn ui-btn-primary" :disabled="addLoading" @click="addDomain">
                  <Plus :size="14" /> Gửi yêu cầu Khai báo
                </button>
                <button class="ui-btn ui-btn-ghost" @click="showAdd = false">Hủy</button>
              </div>
            </div>
          </div>
        </div>

        <!-- Domains List -->
        <div v-if="domains.length === 0 && !showAdd" class="ui-empty padding-empty">
          <Globe :size="40" class="ui-empty-icon" />
          <h3 class="ui-empty-title">Chưa có tên miền nào</h3>
          <p class="ui-empty-desc">Nhấn "Thêm Tên Miền" góc trên để bắt đầu thêm custom domain cho hệ thống rút gọn của bạn.</p>
        </div>

        <div v-if="domains.length > 0" class="domains-list">
          <div v-for="domain in domains" :key="domain.id" class="ui-panel">
            <div class="ui-panel-body domain-item">
              
              <!-- Info -->
              <div class="domain-info">
                <div class="domain-icon-box">
                  <Globe :size="20" class="domain-icon" />
                </div>
                <div class="min-w-0">
                  <h4 class="domain-host">{{ domain.host }}</h4>
                  <div class="domain-status mt-1">
                    <span v-if="domain.isVerified" class="ui-badge ui-badge-success flex-center">
                      <CheckCircle :size="11" /> Đã xác minh
                    </span>
                    <span v-else class="ui-badge ui-badge-warning flex-center">
                      <Clock :size="11" /> Đang chờ xác minh
                    </span>
                    <span v-if="domain.expiredAtUtc" class="ui-badge ui-badge-outline flex-center" style="font-size: 0.7rem;">
                      <Clock :size="10" /> Hạn: {{ formatDate(domain.expiredAtUtc) }}
                    </span>
                  </div>
                </div>
              </div>

              <!-- Extra User Info -->
              <div v-if="domain.userNotes" style="font-size: 0.8rem; color: #64748b; margin-top: 0.5rem; background: #f8fafc; padding: 0.5rem; border-radius: 6px; border-left: 3px solid #e2e8f0;">
                <strong>Ghi chú của bạn:</strong> {{ domain.userNotes }}
              </div>

               <!-- Message if pending -->
               <div v-if="!domain.isVerified" class="ui-alert ui-alert-warning" style="margin: 1rem 0 0; padding: 1rem; font-size: 0.85rem; line-height: 1.5; border-radius: 10px;">
                 <div style="font-weight: 700; margin-bottom: 0.25rem; display: flex; align-items: center; gap: 0.5rem;">
                   <Clock :size="16" /> Đang chờ Admin thiết lập
                 </div>
                 Hệ thống đã ghi nhận yêu cầu của bạn. Quản trị viên đang cấu hình máy chủ và chứng chỉ bảo mật (SSL) cho tên miền này. Kết quả sẽ được hiển thị tại đây sớm nhất.
               </div>
 
               <!-- Admin Feedback if verified -->
               <div v-if="domain.isVerified && domain.adminFeedback" class="ui-alert ui-alert-success" style="margin: 1rem 0 0; padding: 1rem; font-size: 0.85rem; line-height: 1.5; border-radius: 10px;">
                 <div style="font-weight: 700; margin-bottom: 0.25rem; display: flex; align-items: center; gap: 0.5rem;">
                   <CheckCircle :size="16" /> Thông báo từ Admin
                 </div>
                 "{{ domain.adminFeedback }}"
               </div>

              <!-- Actions -->
              <div class="actions-group" style="padding-top: 1rem;">
                
                <!-- Inline Delete Confirmation -->
                <div class="delete-group" v-if="confirmDeleteId === domain.id">
                   <span class="delete-confirm-text">Hủy yêu cầu này?</span>
                   <button @click="deleteDomain(domain.id)" class="ui-btn ui-btn-primary btn-delete-yes">Đồng ý</button>
                   <button @click="confirmDeleteId = null" class="ui-btn ui-btn-ghost btn-delete-cancel">Không</button>
                </div>
                <button 
                  v-else
                  class="ui-btn ui-btn-ghost btn-delete"
                  title="Hủy tên miền"
                  @click="confirmDeleteId = domain.id"
                >
                  <Trash2 :size="16" /> Hủy yêu cầu
                </button>
              </div>

            </div>
          </div>
        </div>

      </div>
    </template>
  </div>
</template>

<style scoped>
.mt-alert {
  margin-bottom: 1rem;
  padding: 1rem;
  display: flex;
  align-items: center;
  gap: 0.5rem;
  border-radius: 8px;
}
.ui-alert-success { background-color: #ecfdf5; border: 1px solid #10b981; color: #047857; }
.ui-alert-error { background-color: #fef2f2; border: 1px solid #ef4444; color: #b91c1c; }

.locked-icon { color: #64748b; opacity: 0.5; }
.mt-4 { margin-top: 1.5rem; }
.domain-content { display: flex; flex-direction: column; gap: 1.5rem; }

.instruction-panel { border-left: 4px solid #3b82f6; }
.flex-gap { display: flex; gap: 1rem; }
.icon-shield { color: #3b82f6; flex-shrink: 0; }
.instruction-title { margin: 0 0 0.5rem; font-size: 1rem; color: #0f172a; font-weight: 700; }
.instruction-desc { margin: 0; font-size: 0.85rem; color: #475569; line-height: 1.5; }
.code-badge { font-family: monospace; background: #f1f5f9; padding: 0.15rem 0.3rem; border-radius: 4px; }
.code-path { font-family: monospace; color: #0f172a; }
.note-text { opacity: 0.8; font-size: 0.8rem; }

.add-panel { background: #f8fafc; border: 1px dashed #cbd5e1; }
.mb-4 { margin-bottom: 1rem; }
.form-group { display: flex; gap: 0.5rem; flex-wrap: wrap; }
.flex-1 { flex: 1; min-width: 200px; margin: 0; }

.padding-empty { padding: 3rem; }
.domains-list { display: flex; flex-direction: column; gap: 1rem; }
.domain-item { display: flex; flex-wrap: wrap; gap: 1.5rem; justify-content: space-between; align-items: flex-start; }

.domain-info { display: flex; gap: 1rem; flex: 1; min-width: 250px; }
.domain-icon-box { width: 44px; height: 44px; border-radius: 12px; background: #eff6ff; display: grid; place-items: center; flex-shrink: 0; }
.domain-icon { color: #3b82f6; }
.domain-host { margin: 0; font-size: 1.1rem; font-weight: 700; color: #0f172a; line-height: 1.2; }
.domain-status { display: flex; align-items: center; gap: 0.5rem; flex-wrap: wrap; margin-top: 0.5rem; padding-right: 0.5rem; }
.mt-1 { margin-top: 0.35rem; }
.flex-center { display: inline-flex; align-items: center; gap: 0.2rem; }

.token-container { flex: 1; min-width: 280px; background: #f8fafc; padding: 0.8rem 1rem; border-radius: 8px; border: 1px solid #e2e8f0; }
.token-label { margin: 0 0 0.4rem; font-size: 0.75rem; font-weight: 700; color: #475569; text-transform: uppercase; }
.token-box { 
  display: flex; justify-content: space-between; align-items: center;
  cursor: pointer; background: #fff; padding: 0.5rem 0.8rem; 
  border: 1px solid #cbd5e1; border-radius: 6px; color: #0f172a; transition: all 0.2s;
}
.token-box:hover { border-color: #94a3b8; background: #f1f5f9; }
.token-text { font-family: monospace; font-size: 0.85rem; word-break: break-all; }
.copy-icon { color: #64748b; margin-left: 0.5rem; flex-shrink: 0; }

.actions-group { display: flex; align-items: center; gap: 0.5rem; padding-top: 0.5rem; }
.btn-verify { border-color: #f59e0b; color: #b45309; }
.btn-delete { color: #ef4444; padding: 0.6rem; }

.delete-group { display: flex; align-items: center; gap: 0.4rem; background: #fef2f2; padding: 0.4rem 0.6rem; border-radius: 6px; border: 1px solid #fecaca; }
.delete-confirm-text { font-size: 0.8rem; font-weight: 600; color: #b91c1c; margin-right: 0.2rem; }
.btn-delete-yes { background: #ef4444; color: #fff; border: none; padding: 0.2rem 0.6rem; min-height: unset; height: 28px; font-size: 0.8rem; }
.btn-delete-yes:hover { background: #dc2626; }
.btn-delete-cancel { padding: 0.2rem 0.6rem; min-height: unset; height: 28px; font-size: 0.8rem; color: #475569; }

.form-row-multi { display: grid; grid-template-columns: 1fr 2fr; gap: 1rem; }
.form-col { display: flex; flex-direction: column; }

@media (max-width: 640px) {
  .form-row-multi { grid-template-columns: 1fr; }
  .instruction-title { font-size: 0.95rem; }
  .domain-item { flex-direction: column; gap: 1rem; }
  .actions-group { width: 100%; justify-content: flex-end; }
}
</style>
