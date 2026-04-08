<script setup lang="ts">
import { ref } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { Lock } from 'lucide-vue-next'
import WxInput from '@/components/ui/WxInput.vue'
import WxButton from '@/components/ui/WxButton.vue'

const route = useRoute()
const router = useRouter()
const slug = route.params.slug as string
const password = ref('')
const error = ref('')
const loading = ref(false)

async function submit() {
  if (!password.value) {
    error.value = 'Vui lòng nhập mật khẩu.'
    return
  }
  error.value = ''
  loading.value = true
  try {
    const API_BASE_URL = import.meta.env.VITE_API_BASE_URL ?? 'http://localhost:5130'
    const res = await fetch(`${API_BASE_URL}/api/public/resolve/${slug}/password`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ password: password.value }),
    })
    if (!res.ok) {
      const data = await res.json().catch(() => ({}))
      error.value = data.title || data.message || 'Mật khẩu không đúng, vui lòng thử lại.'
      return
    }
    const data = await res.json()
    // Redirect to the original URL
    if (data?.redirectUrl) {
      window.location.href = data.redirectUrl
    } else {
      error.value = 'Không thể lấy đường dẫn gốc.'
    }
  } catch {
    error.value = 'Lỗi kết nối, vui lòng thử lại.'
  } finally {
    loading.value = false
  }
}
</script>

<template>
  <div class="flex min-h-screen items-center justify-center px-4">
    <div class="w-full max-w-sm rounded-2xl border border-outline bg-surface-container p-8 shadow-xl">
      <div class="flex flex-col items-center mb-6">
        <div class="w-14 h-14 rounded-full bg-warning/10 flex items-center justify-center mb-4">
          <Lock :size="26" class="text-warning" />
        </div>
        <h1 class="text-xl font-bold text-on-surface">Link được bảo vệ</h1>
        <p class="text-sm text-on-surface-variant text-center mt-2">
          Nhập mật khẩu để tiếp tục truy cập vào đường dẫn này.
        </p>
      </div>

      <form @submit.prevent="submit" class="flex flex-col gap-4">
        <WxInput
          v-model="password"
          label="Mật khẩu truy cập"
          type="password"
          placeholder="Nhập mật khẩu..."
          required
        />
        <p v-if="error" class="text-danger text-sm font-medium">{{ error }}</p>
        <WxButton variant="primary" type="submit" :loading="loading" class="w-full">
          {{ loading ? 'Đang kiểm tra...' : 'Truy cập ngay' }}
        </WxButton>
      </form>
    </div>
  </div>
</template>
