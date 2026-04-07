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

const form = ref<any>({
  originalUrl: 'https://example.com/new-campaign',
  customSlug: '',
  domainId: null,
  description: '',
  tag: 'launch',
  expiresAtUtc: '',
  clickLimit: null,
  password: '',
})

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

    <WxCard v-if="result" class="p-6 max-w-2xl bg-success/10 border-success/30">
      <h3 class="text-lg font-bold text-success mb-4">🎉 Tạo link thành công!</h3>
      <div class="flex flex-col gap-2">
        <p class="text-sm"><strong>Short URL:</strong> <a :href="result.shortUrl" target="_blank" class="text-primary hover:underline">{{ result.shortUrl }}</a></p>
        <p class="text-sm"><strong>Slug:</strong> {{ result.slug }}</p>
        <p class="text-sm"><strong>Trạng thái:</strong> {{ result.status === 'Active' ? 'Hoạt động' : result.status }}</p>
      </div>
    </WxCard>
  </div>
</template>
