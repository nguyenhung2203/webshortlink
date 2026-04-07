<script setup lang="ts">
import { ref } from 'vue'
import { RouterLink, useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth'
import WxButton from '@/components/ui/WxButton.vue'
import WxInput from '@/components/ui/WxInput.vue'
import WxPasswordInput from '@/components/ui/WxPasswordInput.vue'
import { LockIcon, MailIcon } from 'lucide-vue-next'

const email = ref('')
const password = ref('')
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
    error.value = 'Mật khẩu phải có ít nhất 6 ký tự.'
    return
  }

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
  <div>
    <form @submit.prevent="submit" class="flex flex-col gap-4">
      <div class="mb-4">
        <p class="text-primary font-bold uppercase tracking-wider text-sm mb-1">Đăng nhập Khách hàng</p>
        <h1 class="text-2xl font-bold text-on-surface">Truy cập khu vực quản lý</h1>
        <p class="text-sm text-on-surface-variant mt-2">Đăng nhập để xem danh sách link và biểu đồ truy cập.</p>
      </div>

      <WxInput 
        v-model="email" 
        label="Địa chỉ Email" 
        type="email" 
        required 
      />

      <WxPasswordInput 
        v-model="password" 
        label="Mật khẩu" 
        required 
      />

      <div class="flex justify-end -mt-2">
        <RouterLink to="/auth/forgot-password" class="text-sm text-primary font-medium hover:underline">Quên mật khẩu?</RouterLink>
      </div>

      <p v-if="error" class="text-danger text-sm font-medium">{{ error }}</p>

      <WxButton :loading="loading" variant="primary" class="w-full mt-2" type="submit">
        {{ loading ? 'Đang xử lý...' : 'Đăng nhập vào hệ thống' }}
      </WxButton>

      <p class="text-sm text-on-surface-variant mt-4 text-center">
        Chưa có tài khoản?
        <RouterLink to="/auth/register" class="text-primary hover:underline font-medium">Đăng ký ngay</RouterLink>
      </p>
    </form>
  </div>
</template>
