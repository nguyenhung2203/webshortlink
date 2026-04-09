<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { UserService } from '@/api/services'
import { useAuthStore } from '@/stores/auth'
import { User, ShieldCheck, Mail, Activity, Package, KeyRound, Save, CheckCircle2 } from 'lucide-vue-next'

const authStore = useAuthStore()
const fullName = ref('')
const email = ref('')
const role = ref('')
const status = ref('')
const currentPlanId = ref(0)

const currentPassword = ref('')
const newPassword = ref('')
const confirmPassword = ref('')

const message = ref('')
const error = ref('')
const loading = ref(true)
const actionLoading = ref(false)

async function load() {
  loading.value = true
  error.value = ''

  try {
    if (!authStore.accessToken) throw new Error('Chưa xác thực')
    const profile = await UserService.getProfile(authStore.accessToken)
    fullName.value = profile.fullName
    email.value = profile.email
    role.value = profile.role
    status.value = profile.accountStatus
    currentPlanId.value = profile.currentPlanId
  } catch (err) {
    error.value = err instanceof Error ? err.message : 'Không thể tải hồ sơ.'
  } finally {
    loading.value = false
  }
}

async function saveProfile() {
  actionLoading.value = true
  error.value = ''
  message.value = ''

  try {
    if (!authStore.accessToken) throw new Error('Chưa xác thực')
    await UserService.updateProfile(authStore.accessToken, fullName.value)
    message.value = 'Đã cập nhật hồ sơ thành công.'
    setTimeout(() => { message.value = '' }, 3000)
  } catch (err) {
    error.value = err instanceof Error ? err.message : 'Không thể cập nhật hồ sơ.'
  } finally {
    actionLoading.value = false
  }
}

async function changePassword() {
  if (newPassword.value !== confirmPassword.value) {
    error.value = 'Mật khẩu xác nhận không khớp.'
    return
  }

  actionLoading.value = true
  error.value = ''
  message.value = ''

  try {
    if (!authStore.accessToken) throw new Error('Chưa xác thực')
    await UserService.changePassword(
      authStore.accessToken,
      currentPassword.value,
      newPassword.value,
      confirmPassword.value
    )
    message.value = 'Đổi mật khẩu thành công.'
    currentPassword.value = ''
    newPassword.value = ''
    confirmPassword.value = ''
    setTimeout(() => { message.value = '' }, 3000)
  } catch (err) {
    error.value = err instanceof Error ? err.message : 'Không thể đổi mật khẩu.'
  } finally {
    actionLoading.value = false
  }
}

onMounted(load)
</script>

<template>
  <div class="ui-root">
    

    <!-- Alert Messages -->
    <div v-if="error && !loading" class="ui-alert ui-alert-error">
      {{ error }}
    </div>
    <div v-if="message" class="ui-alert ui-alert-success">
      <CheckCircle2 :size="16" /> {{ message }}
    </div>

    <!-- Loading Skeleton -->
    <div v-if="loading" class="ui-card-grid-2">
      <div class="ui-skeleton" style="height: 250px;" />
      <div class="ui-skeleton" style="height: 350px;" />
    </div>

    <div v-else class="ui-card-grid ui-card-grid-2">
      
      <!-- Col 1: Read-only info & Basic Profile -->
      <div style="display: flex; flex-direction: column; gap: 1.5rem;">
        
        <!-- Summary Info -->
        <div class="ui-panel" style="background: linear-gradient(145deg, #f8fafc 0%, #f1f5f9 100%);">
          <div class="ui-panel-body" style="display: flex; flex-direction: column; gap: 1rem;">
            <div style="display: flex; align-items: center; gap: 1rem; border-bottom: 1px solid #e2e8f0; padding-bottom: 1rem;">
              <div style="width: 48px; height: 48px; border-radius: 50%; background: #3b82f6; color: white; display: grid; place-items: center; font-size: 1.2rem; font-weight: bold;">
                {{ fullName.charAt(0).toUpperCase() }}
              </div>
              <div>
                <h3 style="margin: 0; font-size: 1.1rem; color: #0f172a; font-weight: 700;">{{ fullName }}</h3>
                <p style="margin: 0; font-size: 0.85rem; color: #64748b; display: flex; align-items: center; gap: 0.25rem;">
                  <Mail :size="12" /> {{ email }}
                </p>
              </div>
            </div>
            
            <div style="display: grid; grid-template-columns: 1fr 1fr; gap: 1rem;">
              <div>
                <p style="margin: 0; font-size: 0.75rem; color: #64748b; text-transform: uppercase;">Trạng thái</p>
                <div style="display: flex; align-items: center; gap: 0.35rem; margin-top: 0.2rem; font-size: 0.85rem; font-weight: 600;">
                  <Activity :size="14" style="color: #10b981;" /> 
                  <span style="color: #0f172a;">{{ status }}</span>
                </div>
              </div>
              <div>
                <p style="margin: 0; font-size: 0.75rem; color: #64748b; text-transform: uppercase;">Vai trò</p>
                <div style="display: flex; align-items: center; gap: 0.35rem; margin-top: 0.2rem; font-size: 0.85rem; font-weight: 600;">
                  <ShieldCheck :size="14" style="color: #8b5cf6;" /> 
                  <span style="color: #0f172a;">{{ role }}</span>
                </div>
              </div>
              <div>
                <p style="margin: 0; font-size: 0.75rem; color: #64748b; text-transform: uppercase;">Gói dịch vụ</p>
                <div style="display: flex; align-items: center; gap: 0.35rem; margin-top: 0.2rem; font-size: 0.85rem; font-weight: 600;">
                  <Package :size="14" style="color: #f59e0b;" /> 
                  <span style="color: #0f172a;">Gói số #{{ currentPlanId }}</span>
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- Editable Profile -->
        <div class="ui-panel">
          <div class="ui-panel-header">
            <h3 class="ui-panel-title">Chỉnh sửa hồ sơ</h3>
          </div>
          <div class="ui-panel-body">
            <form @submit.prevent="saveProfile" style="display: flex; flex-direction: column; gap: 1rem;">
              <div class="ui-form-group">
                <label class="ui-form-label">Họ và tên</label>
                <input v-model="fullName" type="text" class="ui-form-input" placeholder="Vd: Nguyễn Văn A" required />
              </div>
              <div class="ui-form-group">
                <label class="ui-form-label">Email <span style="font-weight: 400; color: #94a3b8; font-size: 0.8rem;">(Không thể đổi)</span></label>
                <input :value="email" type="text" class="ui-form-input" disabled style="opacity: 0.6; cursor: not-allowed;" />
              </div>
              <div style="margin-top: 0.5rem;">
                <button type="submit" class="ui-btn ui-btn-primary" :disabled="actionLoading">
                  <Save :size="15" /> Lưu thay đổi
                </button>
              </div>
            </form>
          </div>
        </div>

      </div>

      <!-- Col 2: Security & Password -->
      <div>
        <div class="ui-panel">
          <div class="ui-panel-header">
            <h3 class="ui-panel-title" style="display: flex; align-items: center; gap: 0.4rem;">
              <KeyRound :size="18" style="color: #f59e0b;" /> Bảo mật & Mật khẩu
            </h3>
          </div>
          <div class="ui-panel-body">
            <p style="font-size: 0.85rem; color: #64748b; margin: 0 0 1.5rem;">Cập nhật mật khẩu thường xuyên giúp bảo vệ tài khoản của bạn tốt hơn.</p>
            
            <form @submit.prevent="changePassword" style="display: flex; flex-direction: column; gap: 1rem;">
              <div class="ui-form-group">
                <label class="ui-form-label">Mật khẩu hiện tại</label>
                <input v-model="currentPassword" type="password" class="ui-form-input" required />
              </div>
              <div class="ui-form-group">
                <label class="ui-form-label">Mật khẩu mới</label>
                <input v-model="newPassword" type="password" class="ui-form-input" required minlength="6" />
                <p class="ui-form-hint">Tối thiểu 6 ký tự, kết hợp chữ và số để an toàn hơn.</p>
              </div>
              <div class="ui-form-group">
                <label class="ui-form-label">Xác nhận mật khẩu mới</label>
                <input v-model="confirmPassword" type="password" class="ui-form-input" required />
              </div>
              <div style="margin-top: 1rem; padding-top: 1rem; border-top: 1px solid #f1f5f9;">
                <button type="submit" class="ui-btn ui-btn-outline" style="border-color: #3b82f6; color: #3b82f6;" :disabled="actionLoading">
                  <ShieldCheck :size="15" /> Cập nhật mật khẩu
                </button>
              </div>
            </form>
          </div>
        </div>
      </div>

    </div>
  </div>
</template>
