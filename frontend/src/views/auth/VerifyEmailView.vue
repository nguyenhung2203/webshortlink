<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { RouterLink, useRoute, useRouter } from 'vue-router'
import { AuthService } from '@/api/services'
import WxButton from '@/components/ui/WxButton.vue'
import WxInput from '@/components/ui/WxInput.vue'

const userId = ref('')
const email = ref('')
const token = ref('')
const error = ref('')
const successMessage = ref('')
const loading = ref(false)
const route = useRoute()
const router = useRouter()

onMounted(() => {
  if (route.query.userId) userId.value = route.query.userId as string
  if (route.query.email) email.value = route.query.email as string
  if (route.query.token) token.value = route.query.token as string
})

async function submit() {
  error.value = ''
  successMessage.value = ''

  if (!userId.value) {
    error.value = 'Mã tài khoản (UserId) không được để trống.'
    return
  }
  if (!token.value) {
    error.value = 'Mã xác thực (Token/OTP) không đủ.'
    return
  }

  loading.value = true

  try {
    const response = await AuthService.verifyEmail(userId.value, token.value)
    successMessage.value = 'Xác thực Email thành công! Kích hoạt tài khoản hoàn tất.'
    setTimeout(() => router.push('/auth/login'), 2500)
  } catch (err) {
    error.value = err instanceof Error ? err.message : 'Dữ liệu không hợp lệ hoặc đã hết hạn.'
  } finally {
    loading.value = false
  }
}
</script>

<template>
  <div>
    <form @submit.prevent="submit" class="flex flex-col gap-4">
      <div class="mb-4">
        <p class="text-success font-bold uppercase tracking-wider text-sm mb-1">Xác minh tài khoản</p>
        <h1 class="text-2xl font-bold text-on-surface">Kích hoạt Email</h1>
        <p class="text-sm text-on-surface-variant mt-2">Dán mã OTP gồm các con số bảo mật trong email bạn nhận được.</p>
      </div>

      <WxInput v-model="userId" label="ID Tài khoản (User ID)" type="text" required placeholder="Chuỗi ID hệ thống cấp..." />
      <WxInput v-model="token" label="Mã xác thực từ Email" type="text" required placeholder="Nhập để kích hoạt" />

      <p v-if="error" class="text-danger text-sm font-medium">{{ error }}</p>
      <div v-if="successMessage" class="bg-success/10 border border-success/30 p-3 rounded-lg text-success text-sm font-medium">
        {{ successMessage }}
      </div>

      <WxButton :loading="loading" variant="primary" class="w-full mt-2" type="submit">
        {{ loading ? 'Đang xử lý...' : 'Kích hoạt ngay' }}
      </WxButton>

      <p class="text-sm text-on-surface-variant mt-4 text-center">
        Quay về cổng chính?
        <RouterLink to="/auth/login" class="text-primary hover:underline font-medium">Đăng nhập</RouterLink>
      </p>
    </form>
  </div>
</template>
