<script setup lang="ts">
import { ref } from 'vue'
import { RouterLink } from 'vue-router'
import { AuthService } from '@/api/services'
import WxButton from '@/components/ui/WxButton.vue'
import WxInput from '@/components/ui/WxInput.vue'

const email = ref('')
const error = ref('')
const successMessage = ref('')
const loading = ref(false)

async function submit() {
  error.value = ''
  successMessage.value = ''

  if (!email.value || !email.value.includes('@')) {
    error.value = 'Vui lòng nhập định dạng email hợp lệ.'
    return
  }

  loading.value = true

  try {
    const response = await AuthService.forgotPassword(email.value)
    successMessage.value = response.message || 'Mã OTP đã được gửi đến email của bạn.'
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
        <h1 class="text-2xl font-bold text-on-surface">Quên tài khoản?</h1>
        <p class="text-sm text-on-surface-variant mt-2">Nhập email đăng ký của bạn để hệ thống cấp lại quyền truy cập.</p>
      </div>

      <WxInput 
        v-model="email" 
        label="Địa chỉ Email của bạn" 
        type="email" 
        required 
      />

      <p v-if="error" class="text-danger text-sm font-medium">{{ error }}</p>
      <div v-if="successMessage" class="bg-success/10 border border-success/30 p-3 rounded-lg text-success text-sm font-medium">
        {{ successMessage }}
        <p class="mt-2 text-xs text-on-surface-variant"><RouterLink to="/auth/reset-password" class="text-primary hover:underline font-bold">Bấm vào đây để nhập mã bảo mật (OTP)</RouterLink></p>
      </div>

      <WxButton :loading="loading" variant="primary" class="w-full mt-2" type="submit">
        {{ loading ? 'Đang gửi...' : 'Gửi mã khôi phục (OTP)' }}
      </WxButton>

      <p class="text-sm text-on-surface-variant mt-4 text-center">
        Nhớ mật khẩu rồi?
        <RouterLink to="/auth/login" class="text-primary hover:underline font-medium">Quay lại Đăng nhập</RouterLink>
      </p>
    </form>
  </div>
</template>
