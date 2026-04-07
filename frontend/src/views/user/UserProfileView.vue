<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { UserService } from '@/api/services'
import { useAuthStore } from '@/stores/auth'

const authStore = useAuthStore()
const fullName = ref('')
const currentPassword = ref('')
const newPassword = ref('')
const message = ref('')
const error = ref('')
const loading = ref(true)
const actionLoading = ref(false)

async function load() {
  loading.value = true
  error.value = ''

  try {
    if (!authStore.token) throw new Error('Chưa xác thực')
    const profile = await UserService.getProfile(authStore.token)
    fullName.value = profile.fullName
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
    if (!authStore.token) throw new Error('Chưa xác thực')
    await UserService.updateProfile(authStore.token, fullName.value)
    message.value = 'Đã cập nhật hồ sơ.'
  } catch (err) {
    error.value = err instanceof Error ? err.message : 'Không thể cập nhật hồ sơ.'
  } finally {
    actionLoading.value = false
  }
}

async function changePassword() {
  actionLoading.value = true
  error.value = ''
  message.value = ''

  try {
    if (!authStore.token) throw new Error('Chưa xác thực')
    await UserService.changePassword(
      authStore.token,
      currentPassword.value,
      newPassword.value
    )
    message.value = 'Đổi mật khẩu thành công.'
    currentPassword.value = ''
    newPassword.value = ''
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
          <WxInput v-model="fullName" label="Họ và tên" type="text" />
          <WxButton variant="primary" type="submit" :loading="actionLoading" class="self-start mt-2">Lưu hồ sơ</WxButton>
        </form>
      </WxCard>

      <WxCard title="Đổi mật khẩu" class="p-6">
        <form class="flex flex-col gap-4 mt-2" @submit.prevent="changePassword">
          <WxPasswordInput v-model="currentPassword" label="Mật khẩu hiện tại" />
          <WxPasswordInput v-model="newPassword" label="Mật khẩu mới" />
          <WxButton variant="cta" type="submit" :loading="actionLoading" class="self-start mt-2">Cập nhật mật khẩu</WxButton>
        </form>
      </WxCard>
    </div>
  </div>
</template>
