<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { LinkService, UserService } from '@/api/services'
import { useAuthStore } from '@/stores/auth'
import type { CreateLinkRequest, ShortLink } from '@/types/api'
import WxPageHeader from '@/components/ui/WxPageHeader.vue'
import WxInput from '@/components/ui/WxInput.vue'
import WxButton from '@/components/ui/WxButton.vue'
import WxCard from '@/components/ui/WxCard.vue'
import { Settings2, Link as LinkIcon, AlertCircle } from 'lucide-vue-next'

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

const domains = ref<any[]>([])
const copySuccess = ref(false)
const showAdvanced = ref(false)

onMounted(async () => {
  if (authStore.user && authStore.user.currentPlanId > 1 && authStore.accessToken) {
    try {
      domains.value = await UserService.getDomains(authStore.accessToken)
    } catch {
      // Bỏ qua lỗi
    }
  }
})

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
    error.value = 'Bạn phải nhập một link hợp lệ bắt đầu bằng http:// hoặc https://'
    return
  }
  if (form.value.customSlug && !/^[a-zA-Z0-9-_]+$/.test(form.value.customSlug)) {
    error.value = 'Hậu tố rút gọn chỉ được chứa chữ cái không dấu, số, gạch nối (-) và gạch dưới (_).'
    return
  }
  if (form.value.clickLimit !== null && (isNaN(Number(form.value.clickLimit)) || Number(form.value.clickLimit) <= 0)) {
    error.value = 'Giới hạn số click phải lớn hơn 0.'
    return
  }

  loading.value = true

  try {
    if (!authStore.accessToken) throw new Error('Chưa xác thực')
    
    // Nếu thiết bị có datetime-local, ta chuyển nó thành Date an toàn:
    let expiresDate = null
    if (form.value.expiresAtUtc) {
      expiresDate = new Date(form.value.expiresAtUtc).toISOString()
    }

    result.value = await LinkService.create(authStore.accessToken, {
      ...form.value,
      expiresAtUtc: expiresDate,
      clickLimit: form.value.clickLimit ? Number(form.value.clickLimit) : null,
      customSlug: form.value.customSlug || undefined,
      description: form.value.description || undefined,
      domainId: form.value.domainId || null,
      tag: form.value.tag || undefined,
      password: form.value.password || undefined,
    })
    
    // Clear form
    form.value = { ...defaultForm }
    showAdvanced.value = false
  } catch (err) {
    error.value = err instanceof Error ? err.message : 'Đã có lỗi từ máy chủ. Không thể tạo link.'
  } finally {
    loading.value = false
  }
}
</script>

<template>
  <div class="flex flex-col gap-6">
    <WxPageHeader
      title="Rút gọn Link siêu tốc"
      subtitle="Chỉ cần dán link dài của bạn vào đây, WeShort sẽ làm phần còn lại."
    />

    <WxCard title="Tạo link mới" class="p-6 max-w-3xl">
      <form class="flex flex-col gap-6 mt-2" @submit.prevent="submit">
        
        <!-- BƯỚC 1: NHẬP LINK GỐC (Bắt buộc) -->
        <div class="bg-primary/5 p-5 rounded-xl border border-primary/20">
          <div class="flex items-center gap-2 mb-2">
            <LinkIcon class="text-primary" :size="20"/>
            <label class="font-bold text-gray-800 text-lg">Dán link dài vào đây (Bắt buộc)</label>
          </div>
          <p class="text-sm text-gray-600 mb-4 ml-7">Paste vào đây bất kỳ URL dài ngoằng nào bạn cần rút gọn (Shopee, bài viết Facebook, File Drive...)</p>
          <div class="ml-7">
            <WxInput 
              v-model="form.originalUrl" 
              type="url" 
              required 
              placeholder="VD: https://shopee.vn/san-pham-khuyen-mai-10..."
            />
          </div>
        </div>

        <!-- BƯỚC 2: CUSTOM TÊN MIỀN / SLUG -->
        <div class="border-l-4 border-gray-200 pl-5 py-2">
          <label class="font-bold text-gray-800 text-base block mb-1">Cá nhân hoá link (Không bắt buộc)</label>
          <p class="text-sm text-gray-500 mb-4">Tạo một đường dẫn đẹp, có ý nghĩa và dễ nhớ thay vì chữ số lộn xộn.</p>
          <div class="flex flex-col sm:flex-row gap-4">
            <div class="sm:w-1/3">
              <select 
                v-if="domains.length > 0"
                v-model="form.domainId" 
                class="w-full h-10 border border-gray-300 bg-gray-50 rounded-lg text-sm px-3 focus:border-primary focus:ring-1 focus:ring-primary outline-none transition-all"
              >
                <option :value="null">weshort.link (Gốc)</option>
                <option v-for="d in domains" :key="d.id" :value="d.id">{{ d.host }}</option>
              </select>
              <WxInput 
                v-else
                modelValue="weshort.link" 
                disabled 
                placeholder="weshort.link"
              />
              <span class="text-xs mt-1.5 block cursor-pointer" :class="authStore.user?.currentPlanId === 1 ? 'text-amber-600 font-bold hover:underline' : 'text-gray-400'" @click="authStore.user?.currentPlanId === 1 && router.push('/app/billing')">
                <span v-if="authStore.user?.currentPlanId === 1">⚡ Nâng cấp Pro/Plus để dùng Tên miền riêng</span>
                <span v-else>Tên miền hệ thống / Tên miền của bạn</span>
              </span>
            </div>
            <div class="sm:w-2/3">
              <WxInput 
                v-model="form.customSlug" 
                type="text" 
                placeholder="VD: uu-dai-thang-10"
              />
              <span class="text-sm text-primary mt-1.5 block font-medium" v-if="form.customSlug">
                Link sẽ là: <strong class="underline decoration-dashed">weshort.link/{{ form.customSlug }}</strong>
              </span>
            </div>
          </div>
        </div>

        <!-- BƯỚC 3: CÁC TÙY CHỌN NÂNG CAO (ẨN) -->
        <div class="mt-2">
          <button 
            type="button" 
            @click="showAdvanced = !showAdvanced"
            class="flex items-center gap-2 text-sm font-bold text-gray-600 hover:text-primary transition-colors focus:outline-none"
          >
            <Settings2 :size="18" />
            {{ showAdvanced ? 'Ẩn tuỳ chọn nâng cao' : 'Hiện tuỳ chọn nâng cao (Mật khẩu, Hạn sử dụng, Giới hạn...)' }}
          </button>

          <div v-if="showAdvanced" class="mt-5 grid grid-cols-1 md:grid-cols-2 gap-5 bg-gray-50 p-5 rounded-xl border border-gray-100">
            <div>
              <WxInput v-model="form.password" type="text" label="Mật khẩu bảo vệ" placeholder="VD: 123456" />
              <p class="text-xs text-gray-500 mt-1">Người click phải nhập đúng MK này mới vào được web gốc.</p>
            </div>
            <div>
              <WxInput v-model="form.expiresAtUtc" type="datetime-local" label="Thời gian tự huỷ link" />
            </div>
            <div>
              <WxInput v-model="form.clickLimit" type="number" min="1" label="Giới hạn số người click" placeholder="VD: 100" />
            </div>
            <div>
              <WxInput v-model="form.tag" type="text" label="Gắn thẻ (Tag phân loại)" placeholder="VD: quangcao_fb" />
            </div>
            <div class="md:col-span-2">
              <WxInput v-model="form.description" type="text" label="Ghi chú cá nhân" placeholder="Nhớ nội dung link này dùng làm gì..." />
            </div>
          </div>
        </div>

        <div v-if="error" class="flex items-start gap-2 text-danger bg-danger/10 p-3 rounded-lg border border-danger/20 mt-2">
          <AlertCircle :size="20" class="shrink-0 mt-0.5" />
          <p class="text-sm font-medium">{{ error }}</p>
        </div>

        <div class="flex gap-4 mt-6 border-t border-gray-100 pt-6">
          <WxButton variant="primary" :loading="loading" type="submit" class="px-8 py-3 shadow-md hover:shadow-lg focus:ring-2 focus:ring-primary/50 font-bold text-base">
            {{ loading ? 'Đang tạo link, chờ chút...' : '🚀 Tạo Shortlink Ngay!' }}
          </WxButton>
          <WxButton variant="ghost" type="button" @click="router.push('/app/links')" class="py-3">
            Hủy
          </WxButton>
        </div>
      </form>
    </WxCard>

    <WxCard v-if="result" class="p-6 max-w-3xl bg-success/5 border-success/30 relative shadow-sm">
      <div class="flex flex-col gap-5">
        <div class="flex items-center gap-2 text-success font-bold text-xl">
          <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><circle cx="12" cy="12" r="10"/><path d="m9 12 2 2 4-4"/></svg>
          Hoàn tất! Đây là link rút gọn của bạn:
        </div>
        
        <div class="flex flex-col sm:flex-row sm:items-center justify-between bg-white border border-success/30 p-5 rounded-2xl shadow-inner gap-4">
          <a :href="result.shortUrl" target="_blank" class="text-2xl font-black text-primary hover:text-blue-700 tracking-tight break-all">{{ result.shortUrl }}</a>
          
          <WxButton 
            :variant="copySuccess ? 'success' : 'primary'" 
            @click="copyToClipboard(result.shortUrl)"
            class="shadow-sm shrink-0"
          >
            <svg v-if="!copySuccess" xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="mr-2"><rect width="14" height="14" x="8" y="8" rx="2" ry="2"/><path d="M4 16c-1.1 0-2-.9-2-2V4c0-1.1.9-2 2-2h10c1.1 0 2 .9 2 2"/></svg>
            <svg v-else xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="mr-2"><path d="M20 6 9 17l-5-5"/></svg>
            {{ copySuccess ? 'Đã sao chép' : 'Copy Link' }}
          </WxButton>
        </div>
        
        <div class="flex gap-6 text-sm font-medium">
          <button @click="router.push('/app/analytics')" class="text-gray-500 hover:text-primary flex items-center gap-1 group">
            <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="group-hover:stroke-primary"><path d="M3 3v18h18"/><path d="m19 9-5 5-4-4-3 3"/></svg>
            Xem báo cáo lượt nhấn
          </button>
          <button @click="result = null" class="text-gray-500 hover:text-primary flex items-center gap-1 group">
             <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="group-hover:stroke-primary"><path d="M5 12h14"/><path d="M12 5v14"/></svg>
            Rút gọn link khác
          </button>
        </div>
      </div>
    </WxCard>
  </div>
</template>
