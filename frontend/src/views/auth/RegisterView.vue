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
const error = ref('')
const loading = ref(false)

const authStore = useAuthStore()
const router = useRouter()

async function submit() {
  error.value = ''

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
  <div>
    <form @submit.prevent="submit" class="flex flex-col gap-4">
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
</template>
