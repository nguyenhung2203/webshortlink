<script setup lang="ts">
import { ref } from 'vue'
import { RouterLink, useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth'
import WxButton from '@/components/ui/WxButton.vue'
import WxInput from '@/components/ui/WxInput.vue'
import WxPasswordInput from '@/components/ui/WxPasswordInput.vue'

const fullName = ref('Nguyen Demo')
const email = ref('new-user@demo.local')
const password = ref('Demo123!')
const confirmPassword = ref('Demo123!')
const error = ref('')
const successMessage = ref('')
const loading = ref(false)

const authStore = useAuthStore()
const router = useRouter()

async function submit() {
  error.value = ''
  successMessage.value = ''

  if (!fullName.value || fullName.value.length < 3) {
    error.value = 'Họ tên phải có ít nhất 3 ký tự.'
    return
  }
  if (!email.value || !email.value.includes('@')) {
    error.value = 'Vui lòng nhập định dạng email hợp lệ.'
    return
  }
  if (!password.value || password.value.length < 6) {
    error.value = 'Mật khẩu phải có ít nhất 6 ký tự.'
    return
  }
  if (password.value !== confirmPassword.value) {
    error.value = 'Mật khẩu và xác nhận mật khẩu không khớp.'
    return
  }

  loading.value = true

  try {
    const response = await authStore.register(fullName.value, email.value, password.value, confirmPassword.value)
    successMessage.value = 'Đăng ký thành công! Đang chuyển hướng sang trang Đăng nhập...'
    setTimeout(() => {
      router.push('/auth/login')
    }, 3000)
  } catch (err) {
    error.value = err instanceof Error ? err.message : 'Không thể đăng ký.'
  } finally {
    loading.value = false
  }
}
</script>

<template>
  <div>
    <div v-if="successMessage" class="flex flex-col items-center justify-center py-8 text-center">
      <div class="mb-4 rounded-full bg-success/20 p-4">
        <svg class="h-8 w-8 text-success" fill="none" viewBox="0 0 24 24" stroke="currentColor">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M5 13l4 4L19 7" />
        </svg>
      </div>
      <h2 class="mb-2 text-2xl font-bold text-on-surface">Đăng ký thành công!</h2>
      <p class="mb-6 text-on-surface-variant">{{ successMessage }}</p>
      <RouterLink to="/auth/login">
        <WxButton variant="primary">Đến trang Đăng nhập</WxButton>
      </RouterLink>
    </div>

    <form v-else @submit.prevent="submit" class="flex flex-col gap-4">
      <div class="mb-4">
        <p class="text-primary font-bold uppercase tracking-wider text-sm mb-1">Đăng ký thành viên</p>
        <h1 class="text-2xl font-bold text-on-surface">Tạo tài khoản mới</h1>
        <p class="text-sm text-on-surface-variant mt-2">Đăng ký để trải nghiệm toàn bộ tính năng quản lý link chuyên nghiệp.</p>
      </div>

      <WxInput 
        v-model="fullName" 
        label="Họ và tên" 
        type="text" 
        required 
      />

      <WxInput 
        v-model="email" 
        label="Địa chỉ Email" 
        type="email" 
        required 
      />

      <WxPasswordInput 
        v-model="password" 
        label="Mật khẩu tạo mới" 
        required 
      />

      <WxPasswordInput 
        v-model="confirmPassword" 
        label="Xác nhận mật khẩu" 
        required 
      />

      <p v-if="error" class="text-danger text-sm font-medium">{{ error }}</p>

      <WxButton :loading="loading" variant="primary" class="w-full mt-2" type="submit">
        {{ loading ? 'Đang tạo...' : 'Xác nhận Đăng ký' }}
      </WxButton>

      <p class="text-sm text-on-surface-variant mt-4 text-center">
        Đã có tài khoản?
        <RouterLink to="/auth/login" class="text-primary hover:underline font-medium">Đăng nhập</RouterLink>
      </p>
    </form>
  </div>
</template>
