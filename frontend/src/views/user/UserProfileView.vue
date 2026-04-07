<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { UserService } from '@/api/services'
import { useAuthStore } from '@/stores/auth'
import WxPageHeader from '@/components/ui/WxPageHeader.vue'
import WxCard from '@/components/ui/WxCard.vue'
import WxInput from '@/components/ui/WxInput.vue'
import WxPasswordInput from '@/components/ui/WxPasswordInput.vue'
import WxButton from '@/components/ui/WxButton.vue'
import type { UserProfileProfile } from '@/types/api'

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
    message.value = 'Đã cập nhật hồ sơ.'
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
  } catch (err) {
    error.value = err instanceof Error ? err.message : 'Không thể đổi mật khẩu.'
  } finally {
    actionLoading.value = false
  }
}

onMounted(load)
</script>

<template>
  <div class="flex flex-col gap-6">
    <WxPageHeader
      title="Tài khoản cá nhân"
      subtitle="Quản lý thông tin cá nhân và bảo mật tài khoản."
    />

    <p v-if="error && !loading" class="text-danger text-sm">{{ error }}</p>
    <p v-if="message" class="text-success text-sm font-medium p-3 bg-success/10 rounded-lg border border-success/20">{{ message }}</p>

    <div v-if="loading" class="text-sm text-on-surface-variant">Đang tải hồ sơ...</div>

    <div v-else class="grid grid-cols-1 lg:grid-cols-2 gap-6">
      <WxCard title="Thông tin cá nhân" class="p-6">
        <form class="flex flex-col gap-4 mt-2" @submit.prevent="saveProfile">
          <div class="flex flex-col gap-2">
            <span class="text-sm text-on-surface-variant">Email (Không thể đổi): <strong>{{ email }}</strong></span>
            <span class="text-sm text-on-surface-variant">Vai trò: <strong>{{ role }}</strong></span>
            <span class="text-sm text-on-surface-variant">Trạng thái: <strong>{{ status }}</strong></span>
            <span class="text-sm text-on-surface-variant">ID Gói hiện tại: <strong>{{ currentPlanId }}</strong></span>
          </div>
          <WxInput v-model="fullName" label="Họ và tên" type="text" />
          <WxButton variant="primary" type="submit" :loading="actionLoading" class="self-start mt-2">Lưu hồ sơ</WxButton>
        </form>
      </WxCard>

      <WxCard title="Đổi mật khẩu" class="p-6">
        <form class="flex flex-col gap-4 mt-2" @submit.prevent="changePassword">
          <WxPasswordInput v-model="currentPassword" label="Mật khẩu hiện tại" />
          <WxPasswordInput v-model="newPassword" label="Mật khẩu mới" />
          <WxPasswordInput v-model="confirmPassword" label="Xác nhận mật khẩu mới" />
          <WxButton variant="cta" type="submit" :loading="actionLoading" class="self-start mt-2">Cập nhật mật khẩu</WxButton>
        </form>
      </WxCard>
    </div>
  </div>
</template>
