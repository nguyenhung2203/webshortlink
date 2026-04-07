<script setup lang="ts">
import { ref } from 'vue'
import { RouterLink, useRouter } from 'vue-router'
import { AuthService } from '@/api/services'
import WxButton from '@/components/ui/WxButton.vue'
import WxInput from '@/components/ui/WxInput.vue'
import WxPasswordInput from '@/components/ui/WxPasswordInput.vue'

const email = ref('')
const token = ref('')
const newPassword = ref('')
const confirmPassword = ref('')
const error = ref('')
const successMessage = ref('')
const loading = ref(false)
const router = useRouter()

async function submit() {
  error.value = ''
  successMessage.value = ''

  if (!email.value || !email.value.includes('@')) {
    error.value = 'Vui lòng nhập định dạng email hợp lệ.'
    return
  }
  if (!token.value) {
    error.value = 'Vui lòng nhập mã bảo mật (OTP).'
    return
  }
  if (!newPassword.value || newPassword.value.length < 6) {
    error.value = 'Mật khẩu phải có ít nhất 6 ký tự.'
    return
  }
  if (newPassword.value !== confirmPassword.value) {
    error.value = 'Xác nhận mật khẩu không khớp.'
    return
  }

  loading.value = true

  try {
    const response = await AuthService.resetPassword(email.value, token.value, newPassword.value, confirmPassword.value)
    successMessage.value = 'Đổi mật khẩu thành công! Bạn có thể dùng mật khẩu mới để đăng nhập.'
    setTimeout(() => router.push('/auth/login'), 2500)
  } catch (err) {
    error.value = err instanceof Error ? err.message : 'Không gửi được yêu cầu.'
  } finally {
    loading.value = false
  }
}
</script>

<template>
  <div>
    <form @submit.prevent="submit" class="flex flex-col gap-4">
      <div class="mb-4">
        <p class="text-primary font-bold uppercase tracking-wider text-sm mb-1">Khôi phục mật khẩu</p>
        <h1 class="text-2xl font-bold text-on-surface">Tạo mật khẩu mới</h1>
        <p class="text-sm text-on-surface-variant mt-2">Dùng mã bảo mật (OTP) gửi về email của bạn để đổi mật khẩu.</p>
      </div>

      <WxInput v-model="email" label="Địa chỉ Email" type="email" required />
      <WxInput v-model="token" label="Mã bảo mật (OTP) từ Email" type="text" required placeholder="VD: 123456" />
      <WxPasswordInput v-model="newPassword" label="Mật khẩu mới" required />
      <WxPasswordInput v-model="confirmPassword" label="Xác nhận mật khẩu" required />

      <p v-if="error" class="text-danger text-sm font-medium">{{ error }}</p>
      <div v-if="successMessage" class="bg-success/10 border border-success/30 p-3 rounded-lg text-success text-sm font-medium">
        {{ successMessage }}
      </div>

      <WxButton :loading="loading" variant="primary" class="w-full mt-2" type="submit">
        {{ loading ? 'Đang xác thực...' : 'Lưu mật khẩu mới' }}
      </WxButton>

      <p class="text-sm text-on-surface-variant mt-4 text-center">
        Chưa nhận được mã?
        <RouterLink to="/auth/forgot-password" class="text-primary hover:underline font-medium">Gửi lại yêu cầu khôi phục</RouterLink>
      </p>
    </form>
  </div>
</template>
