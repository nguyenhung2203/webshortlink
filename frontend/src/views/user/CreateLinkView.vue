<script setup lang="ts">
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { LinkService } from '@/api/services'
import { useAuthStore } from '@/stores/auth'
import type { CreateLinkRequest, ShortLink } from '@/types/api'
import WxPageHeader from '@/components/ui/WxPageHeader.vue'
import WxInput from '@/components/ui/WxInput.vue'
import WxButton from '@/components/ui/WxButton.vue'
import WxCard from '@/components/ui/WxCard.vue'

const router = useRouter()
const authStore = useAuthStore()

const defaultForm = {
  originalUrl: '',
  customSlug: '',
  domainId: null,
  description: '',
  tag: '',
  expiresAtUtc: '',
  clickLimit: null,
  password: '',
}

const form = ref<any>({ ...defaultForm })

const copySuccess = ref(false)

async function copyToClipboard(text: string) {
  try {
    if (navigator.clipboard && navigator.clipboard.writeText) {
      await navigator.clipboard.writeText(text)
      copySuccess.value = true
      setTimeout(() => { copySuccess.value = false }, 2000)
    } else {
      error.value = 'Trình duyệt không hỗ trợ copy tự động.'
    }
  } catch (err) {
    error.value = 'Không thể copy link.'
  }
}

const result = ref<ShortLink | null>(null)
const error = ref('')
const loading = ref(false)

async function submit() {
  error.value = ''

  if (!form.value.originalUrl.startsWith('http://') && !form.value.originalUrl.startsWith('https://')) {
    error.value = 'URL gốc phải bắt đầu bằng http:// hoặc https://'
    return
  }
  if (form.value.customSlug && !/^[a-zA-Z0-9-_]+$/.test(form.value.customSlug)) {
    error.value = 'Slug tùy chỉnh chỉ được chứa chữ cái, số, gạch nối và gạch dưới.'
    return
  }
  if (form.value.clickLimit !== null && (isNaN(Number(form.value.clickLimit)) || Number(form.value.clickLimit) <= 0)) {
    error.value = 'Giới hạn click phải là một số lớn hơn 0.'
    return
  }

  loading.value = true

  try {
    if (!authStore.accessToken) throw new Error('Chưa xác thực')
    result.value = await LinkService.create(authStore.accessToken, {
      ...form.value,
      expiresAtUtc: form.value.expiresAtUtc ? new Date(form.value.expiresAtUtc).toISOString() : null,
      clickLimit: form.value.clickLimit ? Number(form.value.clickLimit) : null,
      customSlug: form.value.customSlug || undefined,
      description: form.value.description || undefined,
      domainId: form.value.domainId || null,
      tag: form.value.tag || undefined,
      password: form.value.password || undefined,
    })
    
    // Clear the form to prevent double submission
    form.value = { ...defaultForm }
  } catch (err) {
    error.value = err instanceof Error ? err.message : 'Không thể tạo link.'
  } finally {
    loading.value = false
  }
}
</script>

<template>
  <div class="flex flex-col gap-6">
    <WxPageHeader
      title="Tạo Link mới"
      subtitle="Thiết lập shortlink mới và cấu hình các giới hạn cơ bản."
    />

    <WxCard title="Thông tin Link" class="p-6 max-w-2xl">
      <form class="flex flex-col gap-4 mt-2" @submit.prevent="submit">
        <WxInput 
          v-model="form.originalUrl" 
          label="URL gốc (Original URL)" 
          type="url" 
          required 
          placeholder="https://example.com/very-long-url-..."
        />
        <WxInput 
          v-model="form.customSlug" 
          label="Đường dẫn tuỳ chỉnh (Custom slug)" 
          type="text" 
          placeholder="VD: san-pham-123"
        />
        <WxInput 
          v-model="form.description" 
          label="Mô tả" 
          type="text" 
          placeholder="VD: Link chiến dịch 2024"
        />
        <WxInput 
          v-model="form.tag" 
          label="Nhãn (Tag)" 
          type="text" 
          placeholder="VD: xuan2024"
        />
        <WxInput 
          v-model="form.expiresAtUtc" 
          label="Thời gian hết hạn (UTC)" 
          type="text" 
        />
        <WxInput 
          v-model="form.clickLimit" 
          label="Giới hạn số lượt click" 
          type="text" 
          placeholder="Để trống nếu không giới hạn"
        />
        <WxInput 
          v-model="form.password" 
          label="Mật khẩu bảo vệ" 
          type="text" 
          placeholder="Để trống nếu không cần mật khẩu"
        />

        <p v-if="error" class="text-danger text-sm font-medium">{{ error }}</p>

        <div class="flex gap-4 mt-4">
          <WxButton variant="primary" :loading="loading" type="submit">
            {{ loading ? 'Đang tạo...' : 'Tạo shortlink' }}
          </WxButton>
          <WxButton variant="ghost" type="button" @click="router.push('/app/links')">
            Hủy và quay lại
          </WxButton>
        </div>
      </form>
    </WxCard>

    <WxCard v-if="result" class="p-6 max-w-2xl bg-success/10 border-success/30 relative">
      <div class="flex items-center justify-between mb-4">
        <h3 class="text-lg font-bold text-success">🎉 Tạo link thành công!</h3>
        <WxButton 
          :variant="copySuccess ? 'primary' : 'secondary'" 
          @click="copyToClipboard(result.shortUrl)"
        >
          <svg v-if="!copySuccess" xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="mr-2"><rect width="14" height="14" x="8" y="8" rx="2" ry="2"/><path d="M4 16c-1.1 0-2-.9-2-2V4c0-1.1.9-2 2-2h10c1.1 0 2 .9 2 2"/></svg>
          <svg v-else xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="mr-2"><path d="M20 6 9 17l-5-5"/></svg>
          {{ copySuccess ? 'Đã sao chép' : 'Copy Link' }}
        </WxButton>
      </div>
      <div class="flex flex-col gap-2">
        <p class="text-sm"><strong>Short URL:</strong> <a :href="result.shortUrl" target="_blank" class="text-primary hover:underline font-medium ml-1">{{ result.shortUrl }}</a></p>
        <p class="text-sm"><strong>Slug:</strong> <span class="ml-1">{{ result.slug }}</span></p>
        <p class="text-sm"><strong>Trạng thái:</strong> <span class="ml-1">{{ result.status === 'Active' ? 'Hoạt động' : result.status }}</span></p>
      </div>
    </WxCard>
  </div>
</template>
