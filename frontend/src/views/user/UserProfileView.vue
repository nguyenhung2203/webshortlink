<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { apiRequest } from '@/api/client'
import { useAuthStore } from '@/stores/auth'

const authStore = useAuthStore()
const profile = ref<any>(null)
const fullName = ref('')
const currentPassword = ref('')
const newPassword = ref('')
const message = ref('')
const error = ref('')

async function load() {
  try {
    profile.value = await apiRequest('/api/me/profile', { token: authStore.token })
    fullName.value = profile.value.fullName
  } catch (err) {
    error.value = err instanceof Error ? err.message : 'Không thể tải hồ sơ.'
  }
}

async function saveProfile() {
  try {
    profile.value = await apiRequest('/api/me/profile', {
      method: 'PUT',
      token: authStore.token,
      body: { fullName: fullName.value },
    })
    message.value = 'Đã cập nhật hồ sơ.'
  } catch (err) {
    error.value = err instanceof Error ? err.message : 'Không thể cập nhật hồ sơ.'
  }
}

async function changePassword() {
  try {
    await apiRequest('/api/me/password', {
      method: 'PUT',
      token: authStore.token,
      body: {
        currentPassword: currentPassword.value,
        newPassword: newPassword.value,
      },
    })
    message.value = 'Đổi mật khẩu thành công.'
    currentPassword.value = ''
    newPassword.value = ''
  } catch (err) {
    error.value = err instanceof Error ? err.message : 'Không thể đổi mật khẩu.'
  }
}

onMounted(load)
</script>

<template>
  <section class="stack-lg">
    <div class="page-header">
      <div>
        <h2>Profile</h2>
        <p class="muted">Quản lý thông tin cá nhân và bảo mật tài khoản.</p>
      </div>
    </div>

    <p v-if="error" class="error-text">{{ error }}</p>
    <p v-if="message" class="success-text">{{ message }}</p>

    <div class="two-columns">
      <form class="panel stack-sm" @submit.prevent="saveProfile">
        <h3>Thông tin cá nhân</h3>
        <label class="field">
          <span>Họ tên</span>
          <input v-model="fullName" type="text" />
        </label>
        <button class="primary-button" type="submit">Lưu hồ sơ</button>
      </form>

      <form class="panel stack-sm" @submit.prevent="changePassword">
        <h3>Đổi mật khẩu</h3>
        <label class="field">
          <span>Mật khẩu hiện tại</span>
          <input v-model="currentPassword" type="password" />
        </label>
        <label class="field">
          <span>Mật khẩu mới</span>
          <input v-model="newPassword" type="password" />
        </label>
        <button class="primary-button" type="submit">Cập nhật mật khẩu</button>
      </form>
    </div>
  </section>
</template>
