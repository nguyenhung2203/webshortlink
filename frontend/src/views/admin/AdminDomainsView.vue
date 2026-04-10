<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useAuthStore } from '@/stores/auth'
import { AdminService } from '@/api/services'
import { Globe, CheckCircle, RefreshCw, AlertCircle, Clock, Trash2, ShieldCheck, Plus, X, Search } from 'lucide-vue-next'

const authStore = useAuthStore()

const domains = ref<any[]>([])
const users = ref<any[]>([])
const loading = ref(true)
const error = ref('')
const actionLoading = ref<Record<string, boolean>>({})

const isModalOpen = ref(false)
const newDomainForm = ref({ userId: '', host: '', isGlobal: false })
const modalError = ref('')

const loadData = async () => {
  loading.value = true
  error.value = ''
  try {
    if (!authStore.accessToken) return
    const [fetchedDomains, fetchedUsers] = await Promise.all([
      AdminService.getDomains(authStore.accessToken),
      AdminService.getUsers(authStore.accessToken)
    ])
    domains.value = fetchedDomains
    users.value = fetchedUsers
  } catch (err) {
    error.value = err instanceof Error ? err.message : 'Không thể tải dữ liệu'
  } finally {
    loading.value = false
  }
}

const createDomainForUser = async () => {
  if ((!newDomainForm.value.userId && !newDomainForm.value.isGlobal) || !newDomainForm.value.host) {
    modalError.value = 'Vui lòng chọn khách hàng (hoặc đánh dấu Global) và nhập tên miền đầy đủ.'
    return
  }
  
  actionLoading.value['create'] = true
  modalError.value = ''
  try {
    await AdminService.createDomain(authStore.accessToken!, newDomainForm.value.userId || '', newDomainForm.value.host, newDomainForm.value.isGlobal)
    alert('Thêm domain thành công!')
    isModalOpen.value = false
    newDomainForm.value = { userId: '', host: '', isGlobal: false }
    await loadData()
  } catch (err) {
    modalError.value = err instanceof Error ? err.message : 'Lỗi thêm domain'
  } finally {
    actionLoading.value['create'] = false
  }
}

const checkDns = async (domainId: string) => {
  if (!authStore.accessToken) return
  actionLoading.value[domainId + '-check'] = true
  try {
    const res = await AdminService.checkDomainDns(authStore.accessToken, domainId)
    alert(res.message || 'Kết quả Check DNS: Hoàn tất')
    await loadData()
  } catch (err) {
    alert(err instanceof Error ? err.message : 'Lỗi kiểm tra DNS')
  } finally {
    actionLoading.value[domainId + '-check'] = false
  }
}

const verifyDomainId = ref<string | null>(null)
const adminFeedback = ref('')

const openVerifyModal = (domainId: string) => {
  verifyDomainId.value = domainId
  adminFeedback.value = 'Tên miền đã được cấu hình DNS và SSL thành công. Bạn có thể sử dụng ngay.'
}

const verifyDomain = async () => {
  if (!authStore.accessToken || !verifyDomainId.value) return
  
  const domainId = verifyDomainId.value
  actionLoading.value[domainId + '-verify'] = true
  try {
    const res = await AdminService.verifyDomain(authStore.accessToken, domainId, adminFeedback.value)
    alert(res.message || 'Đã xác minh domain và gửi phản hồi cho người dùng')
    verifyDomainId.value = null
    await loadData()
  } catch (err) {
    alert(err instanceof Error ? err.message : 'Lỗi xác minh domain')
  } finally {
    actionLoading.value[domainId + '-verify'] = false
  }
}

const deleteDomain = async (domainId: string) => {
  if (!authStore.accessToken) return
  if (!confirm('Bạn có chắc chắn muốn xóa domain này không? Các link thuộc domain này có thể ngừng hoạt động.')) return
  
  actionLoading.value[domainId + '-delete'] = true
  try {
    const res = await AdminService.deleteDomain(authStore.accessToken, domainId)
    alert(res.message || 'Đã xóa domain')
    await loadData()
  } catch (err) {
    alert(err instanceof Error ? err.message : 'Lỗi xóa domain')
  } finally {
    actionLoading.value[domainId + '-delete'] = false
  }
}

const setDefaultDomain = async (domainId: string) => {
  if (!authStore.accessToken) return
  if (!confirm('Xác nhận đặt tên miền này làm System Default Domain của toàn hệ thống?')) return

  actionLoading.value[domainId + '-default'] = true
  try {
    const res = await AdminService.setDefaultDomain(authStore.accessToken, domainId)
    alert(res.message || 'Thành công')
    await loadData()
  } catch (err) {
    alert(err instanceof Error ? err.message : 'Lỗi cập nhật tên miền system')
  } finally {
    actionLoading.value[domainId + '-default'] = false
  }
}

onMounted(() => {
  loadData()
})

const formatDate = (dateValue: string | null) => {
  if (!dateValue) return '-'
  return new Date(dateValue).toLocaleString('vi-VN')
}
</script>

<template>
  <div class="ui-root relative">
    
    <div class="ui-header">
      <div class="ui-header-left">
        <div class="ui-eyebrow"><Globe :size="13" /> Quản lý tài nguyên</div>
        <h1 class="ui-title">Quản lý Tên miền (Custom Domains)</h1>
        <p class="ui-subtitle">Xem danh sách, check DNS, và cấp phát tên miền tùy chỉnh cho khách hàng.</p>
      </div>
      <div style="display: flex; gap: 0.5rem;">
        <button class="ui-btn ui-btn-outline" @click="loadData" :disabled="loading">
          <RefreshCw :size="14" :class="{'animate-spin': loading}" /> Làm mới
        </button>
        <button class="ui-btn ui-btn-primary" @click="isModalOpen = true">
          <Plus :size="14" /> Cấp phát Domain
        </button>
      </div>
    </div>

    <!-- Modal Thêm Domain Mới -->
    <div v-if="isModalOpen" class="fixed inset-0 z-50 flex items-center justify-center bg-black bg-opacity-50" style="position: fixed; top: 0; left: 0; right: 0; bottom: 0; background: rgba(0,0,0,0.5); display: flex; align-items: center; justify-content: center; z-index: 100;">
      <div class="ui-panel" style="width: 100%; max-width: 500px; padding: 2rem; position: relative;">
        <button @click="isModalOpen = false" style="position: absolute; right: 1rem; top: 1rem; background: transparent; border: none; cursor: pointer; color: #64748b;">
          <X :size="20" />
        </button>
        
        <h2 style="font-size: 1.25rem; font-weight: 800; color: #0f172a; margin-bottom: 0.5rem; display: flex; align-items: center; gap: 0.5rem;">
          <Globe :size="20" style="color: #38bdf8;" /> Cấp phát Tên miền mới
        </h2>
        <p style="font-size: 0.85rem; color: #64748b; margin-bottom: 1.5rem;">
          Tạo trực tiếp quyền sử dụng domain cho người dùng trong hệ thống (nhưng User vẫn phải cấu hình DNS).
        </p>

        <div v-if="modalError" class="ui-alert ui-alert-error" style="margin-bottom: 1rem; padding: 0.75rem;">
          <AlertCircle :size="14" /> {{ modalError }}
        </div>

        <div style="display: flex; flex-direction: column; gap: 1rem;">
          <div style="background: #f8fafc; padding: 1rem; border-radius: 8px; border: 1px solid #e2e8f0; display: flex; align-items: center; gap: 0.5rem;">
            <input type="checkbox" id="global-domain" v-model="newDomainForm.isGlobal" style="width: 18px; height: 18px; accent-color: #3b82f6; cursor: pointer;">
            <label for="global-domain" style="font-weight: 700; font-size: 0.95rem; color: #0f172a; cursor: pointer;">
              Đây là Tên miền hệ thống (System Global Domain)
              <p style="font-weight: 400; font-size: 0.8rem; color: #64748b; margin: 0.1rem 0 0;">(Toàn bộ user trong hệ thống đều dùng chung được domain này)</p>
            </label>
          </div>

          <div v-if="!newDomainForm.isGlobal">
            <label class="ui-label">Chọn Khách hàng (User)</label>
            <select v-model="newDomainForm.userId" class="ui-input" style="width: 100%;">
              <option value="" disabled>-- Chọn khách hàng --</option>
              <option v-for="u in users" :key="u.id" :value="u.id">
                {{ u.email }} ({{ u.fullName }})
              </option>
            </select>
          </div>

          <div>
            <label class="ui-label">Domain (Host)</label>
            <input 
              v-model="newDomainForm.host" 
              type="text" 
              class="ui-input" 
              placeholder="Ví dụ: go.mycompany.com" 
              style="width: 100%;"
            />
          </div>

          <button 
            @click="createDomainForUser" 
            :disabled="actionLoading['create']"
            class="ui-btn ui-btn-primary" 
            style="width: 100%; justify-content: center; height: 42px;"
          >
            <RefreshCw v-if="actionLoading['create']" :size="16" class="animate-spin" />
            <span v-else>Xác nhận Thêm</span>
          </button>
        </div>
      </div>
    </div>

    <!-- Modal Phê duyệt Domain & Phản hồi -->
    <div v-if="verifyDomainId" class="fixed inset-0 z-50 flex items-center justify-center bg-black bg-opacity-50" style="position: fixed; top: 0; left: 0; right: 0; bottom: 0; background: rgba(0,0,0,0.5); display: flex; align-items: center; justify-content: center; z-index: 100;">
      <div class="ui-panel" style="width: 100%; max-width: 450px; padding: 2rem;">
        <h2 style="font-size: 1.15rem; font-weight: 800; color: #0f172a; margin-bottom: 1rem;">Phê duyệt Tên miền</h2>
        <p style="font-size: 0.85rem; color: #64748b; margin-bottom: 1.25rem;">Xác minh và gửi hướng dẫn/thông báo cho khách hàng.</p>
        
        <div style="margin-bottom: 1.5rem;">
          <label class="ui-label">Phản hồi của Admin (Hiện cho User)</label>
          <textarea 
            v-model="adminFeedback" 
            class="ui-input" 
            rows="4" 
            style="width: 100%; height: auto; padding: 0.75rem; line-height: 1.5; border-radius: 8px;"
            placeholder="Nhập ghi chú hoặc thông báo cho khách..."
          ></textarea>
        </div>

        <div style="display: flex; gap: 0.75rem; justify-content: flex-end;">
          <button @click="verifyDomainId = null" class="ui-btn ui-btn-ghost">Hủy</button>
          <button @click="verifyDomain" :disabled="actionLoading[verifyDomainId + '-verify']" class="ui-btn ui-btn-primary">
            <RefreshCw v-if="actionLoading[verifyDomainId + '-verify']" :size="14" class="animate-spin" />
            <span v-else>Xác nhận & Duyệt</span>
          </button>
        </div>
      </div>
    </div>

    <!-- Main Content -->
    <div v-if="error" class="ui-alert ui-alert-error">
      <AlertCircle :size="16" /> {{ error }}
    </div>

    <div v-if="loading" class="ui-skeleton" style="height: 400px; border-radius: 12px;" />

    <div v-else-if="domains.length === 0" class="ui-empty" style="background: white;">
      <div class="ui-empty-icon" style="opacity: 0.5;"><Globe :size="48" /></div>
      <h3 class="ui-empty-title">Chưa có tên miền nào</h3>
      <p class="ui-empty-desc">Hệ thống chưa ghi nhận tên miền tùy chỉnh nào. Bạn có thể cấp phát mới.</p>
    </div>

    <div v-else class="ui-panel" style="overflow: hidden; padding: 0;">
      
      <div style="overflow-x: auto;">
        <table style="width: 100%; border-collapse: collapse; min-width: 900px; text-align: left;">
          <thead style="background: #f8fafc; border-bottom: 1px solid #e2e8f0;">
            <tr>
              <th style="padding: 1rem 1.5rem; font-size: 0.75rem; font-weight: 700; color: #64748b; text-transform: uppercase;">Domain & Chủ sở hữu</th>
              <th style="padding: 1rem 1.5rem; font-size: 0.75rem; font-weight: 700; color: #64748b; text-transform: uppercase;">Sử dụng</th>
              <th style="padding: 1rem 1.5rem; font-size: 0.75rem; font-weight: 700; color: #64748b; text-transform: uppercase;">Phản hồi / Token</th>
              <th style="padding: 1rem 1.5rem; font-size: 0.75rem; font-weight: 700; color: #64748b; text-transform: uppercase;">Trạng thái</th>
              <th style="padding: 1rem 1.5rem; font-size: 0.75rem; font-weight: 700; color: #64748b; text-transform: uppercase; text-align: right;">Hành động Admin</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="domain in domains" :key="domain.id" style="border-bottom: 1px solid #f1f5f9; transition: background 0.15s;" class="hover:bg-slate-50">
              
              <td style="padding: 1rem 1.5rem;">
                <div style="display: flex; flex-direction: column; gap: 0.2rem;">
                  <span style="font-family: inherit; font-weight: 700; font-size: 0.95rem; color: #0f172a;">
                    {{ domain.host }}
                    <span v-if="domain.isGlobal" class="ui-badge ui-badge-success" style="font-size: 0.6rem; padding: 0.1rem 0.3rem; margin-left: 0.5rem; vertical-align: top;">GLOBAL</span>
                  </span>
                  <span v-if="!domain.isGlobal" style="font-size: 0.8rem; color: #3b82f6; font-weight: 500;">User: {{ domain.userEmail }}</span>
                  <span v-else style="font-size: 0.8rem; color: #059669; font-weight: 500;">By Admin</span>
                </div>
              </td>
              
              <td style="padding: 1rem 1.5rem;">
                <span class="ui-badge" style="background: #f1f5f9; color: #475569; font-weight: 600; border: 1px solid #e2e8f0;">
                  {{ domain.linksCount }} Links
                </span>
              </td>

              <td style="padding: 1rem 1.5rem;">
                <div style="display: flex; flex-direction: column; gap: 0.25rem; font-size: 0.75rem;">
                  <div v-if="domain.adminFeedback" style="color: #059669; font-weight: 600; font-style: italic; max-width: 250px;" class="truncate" :title="domain.adminFeedback">
                    "{{ domain.adminFeedback }}"
                  </div>
                  <div style="display: flex; align-items: center; gap: 0.4rem; color: #64748b;">
                    <Clock :size="12" /> {{ formatDate(domain.createdAtUtc) }}
                  </div>
                </div>
              </td>
              
              <td style="padding: 1rem 1.5rem;">
                <div style="display: flex; align-items: center; flex-direction: column; gap: 0.3rem;">
                  <span v-if="!domain.isVerified" class="ui-badge ui-badge-warning" style="font-weight: 800;"><Clock :size="12" style="margin-right: 0.2rem;" /> UNVERIFIED</span>
                  <span v-else class="ui-badge ui-badge-success" style="font-weight: 800;"><ShieldCheck :size="12" style="margin-right: 0.2rem;" /> SECURE / VERIFIED</span>
                  <span v-if="domain.isDefault" class="ui-badge" style="background: #a855f7; color: white; font-weight: 700; width: fit-content; font-size: 0.6rem;">DEFAULT DOMAIN</span>
                </div>
              </td>

              <td style="padding: 1rem 1.5rem; text-align: right;">
                <div style="display: flex; align-items: center; justify-content: flex-end; gap: 0.5rem;">
                  <button
                    v-if="domain.isGlobal && domain.isVerified && !domain.isDefault"
                    @click="setDefaultDomain(domain.id)"
                    :disabled="actionLoading[domain.id + '-default']"
                    class="ui-btn"
                    title="Đặt làm Mặc định Hệ thống"
                    style="background: #a855f7; color: white; border: none; font-size: 0.8rem; padding: 0.4rem 0.6rem;"
                  >
                    <RefreshCw v-if="actionLoading[domain.id + '-default']" :size="14" class="animate-spin" />
                    <span v-else style="font-weight: bold;">Set Default</span>
                  </button>
                  <button
                    @click="checkDns(domain.id)"
                    :disabled="actionLoading[domain.id + '-check']"
                    class="ui-btn"
                    title="Ping Check DNS"
                    style="background: #3b82f6; color: white; border: none; font-size: 0.8rem; padding: 0.4rem 0.6rem;"
                  >
                    <RefreshCw v-if="actionLoading[domain.id + '-check']" :size="14" class="animate-spin" />
                    <Search v-else :size="14" /> Check
                  </button>
                  <button
                    v-if="!domain.isVerified"
                    @click="openVerifyModal(domain.id)"
                    :disabled="actionLoading[domain.id + '-verify']"
                    class="ui-btn ui-btn-outline"
                    title="Duyệt Domain (Approve)"
                    style="font-size: 0.8rem; padding: 0.4rem 0.6rem; color: #10b981; border-color: #10b981;"
                  >
                    <RefreshCw v-if="actionLoading[domain.id + '-verify']" :size="14" class="animate-spin" />
                    <CheckCircle v-else :size="14" />
                  </button>
                  <button
                    @click="deleteDomain(domain.id)"
                    :disabled="actionLoading[domain.id + '-delete']"
                    class="ui-btn"
                    title="Xóa Domain"
                    style="background: #ef4444; color: white; border: none; font-size: 0.8rem; padding: 0.4rem 0.6rem;"
                  >
                    <RefreshCw v-if="actionLoading[domain.id + '-delete']" :size="14" class="animate-spin" />
                    <Trash2 v-else :size="14" />
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
