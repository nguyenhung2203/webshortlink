<script setup lang="ts">
import { ref } from 'vue'
import { RouterLink, useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth'

const fullName = ref('Nguyen Demo')
const email = ref('new-user@demo.local')
const password = ref('Demo123!')
const error = ref('')
const loading = ref(false)

const authStore = useAuthStore()
const router = useRouter()

async function submit() {
  error.value = ''
  loading.value = true

  try {
    await authStore.register(fullName.value, email.value, password.value)
    router.push('/app/dashboard')
  } catch (err) {
    error.value = err instanceof Error ? err.message : 'Không thể đăng ký.'
  } finally {
    loading.value = false
  }
}
</script>

<template>
  <div class="auth-shell">
    <form class="auth-card" @submit.prevent="submit">
      <p class="eyebrow">Đăng ký</p>
      <h1>Tạo tài khoản mới</h1>

      <label class="field">
        <span>Họ tên</span>
        <input v-model="fullName" type="text" required />
      </label>

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
        {{ loading ? 'Đang tạo...' : 'Tạo tài khoản' }}
      </button>

      <p class="muted">
        Đã có tài khoản?
        <RouterLink to="/login">Đăng nhập</RouterLink>
      </p>
    </form>
  </div>
</template>
