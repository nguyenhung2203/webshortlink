<script setup lang="ts">
import { ref } from 'vue'
import { RouterLink, useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth'

const email = ref('user@demo.local')
const password = ref('Demo123!')
const error = ref('')
const loading = ref(false)

const authStore = useAuthStore()
const router = useRouter()

async function submit() {
  error.value = ''
  loading.value = true

  try {
    await authStore.login(email.value, password.value)
    router.push(authStore.role === 'Admin' ? '/admin' : '/app/dashboard')
  } catch (err) {
    error.value = err instanceof Error ? err.message : 'Không thể đăng nhập.'
  } finally {
    loading.value = false
  }
}
</script>

<template>
  <div class="auth-shell">
    <form class="auth-card" @submit.prevent="submit">
      <p class="eyebrow">Đăng nhập</p>
      <h1>Truy cập workspace</h1>
      <p class="muted">Turnstile và Identity sẽ được thay thế ở lớp hạ tầng sau khi chốt flow.</p>

      <label class="field">
        <span>Email</span>
        <input v-model="email" type="email" required />
      </label>

      <label class="field">
        <span>Mật khẩu</span>
        <input v-model="password" type="password" required />
      </label>

      <p v-if="error" class="error-text">{{ error }}</p>

      <button class="primary-button" :disabled="loading" type="submit">
        {{ loading ? 'Đang xử lý...' : 'Đăng nhập' }}
      </button>

      <p class="muted">
        Chưa có tài khoản?
        <RouterLink to="/register">Đăng ký</RouterLink>
      </p>
    </form>
  </div>
</template>
