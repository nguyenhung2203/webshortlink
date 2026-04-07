<script setup lang="ts">
import { ref } from 'vue'
import { RouterLink, useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth'
import WxButton from '@/components/ui/WxButton.vue'
import WxInput from '@/components/ui/WxInput.vue'
import WxPasswordInput from '@/components/ui/WxPasswordInput.vue'
import { Shield } from 'lucide-vue-next'

const email = ref('admin@demo.local')
const password = ref('Admin123!')
const error = ref('')
const loading = ref(false)

const authStore = useAuthStore()
const router = useRouter()

async function submit() {
  error.value = ''

  if (!email.value || !email.value.includes('@')) {
    error.value = 'Vui lòng nhập định dạng email hợp lệ.'
    return
  }
  if (!password.value || password.value.length < 6) {
    error.value = 'Mật khẩu quản trị viên không hợp lệ.'
    return
  }

  loading.value = true

  try {
    await authStore.login(email.value, password.value)
    if (authStore.role !== 'Admin') {
      authStore.logout()
      throw new Error('Tài khoản không có quyền Admin.')
    }
    router.push('/admin')
  } catch (err) {
    error.value = err instanceof Error ? err.message : 'Không thể đăng nhập.'
  } finally {
    loading.value = false
  }
}
</script>

<template>
  <div>
    <form @submit.prevent="submit" class="flex flex-col gap-4">
      <div class="mb-4">
        <div class="flex items-center gap-2 mb-2 text-danger">
          <Shield :size="20" />
          <p class="font-bold uppercase tracking-wider text-sm">Quản trị viên</p>
        </div>
        <h1 class="text-2xl font-bold text-on-surface">Cổng Admin Portal</h1>
        <p class="text-on-surface-variant mt-2 text-sm">Đăng nhập tài khoản hệ thống để quản lý data.</p>
      </div>

      <WxInput 
        v-model="email" 
        label="Tài khoản Admin" 
        type="email" 
        required 
      />

      <WxPasswordInput 
        v-model="password" 
        label="Mật khẩu" 
        required 
      />

      <p v-if="error" class="text-danger mt-1 text-sm font-medium">{{ error }}</p>

      <WxButton :loading="loading" variant="danger" class="w-full mt-2" type="submit">
        {{ loading ? 'Đang xác thực...' : 'Vào hệ thống Admin' }}
      </WxButton>

      <p class="text-on-surface-variant mt-4 text-center text-sm">
        <RouterLink to="/auth/login" class="text-primary hover:underline font-medium">Quay về cổng Khách hàng</RouterLink>
      </p>
    </form>
  </div>
</template>
