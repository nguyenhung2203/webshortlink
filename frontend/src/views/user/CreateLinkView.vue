<script setup lang="ts">
import { computed, ref, onMounted } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { LinkService, UserService } from '@/api/services'
import { useAuthStore } from '@/stores/auth'
import type { ShortLink } from '@/types/api'
import { Settings2, Link as LinkIcon, AlertCircle, Copy, CheckCircle2, ChevronDown, ChevronUp, Lock, Image } from 'lucide-vue-next'

const router = useRouter()
const route = useRoute()
const authStore = useAuthStore()

// --- Social Preview State ---
const mockTitle = ref('')
const mockDesc = ref('')
const mockImg = ref('')

const defaultForm = {
  originalUrl: '',
  customSlug: '',
  domainId: null as string | null,
  description: '',
  tag: '',
  expiresAtUtc: '',
  clickLimit: null as number | null,
  password: '',
}

const form = ref<any>({ ...defaultForm })
const domains = ref<any[]>([])

const defaultDomainHost = computed(() => {
  const def = domains.value.find(d => d.isDefault)
  return def ? def.host : 'wemake.link'
})

const selectedDomainHost = computed(() => {
  if (!form.value.domainId) return defaultDomainHost.value;
  const d = domains.value.find(x => x.id === form.value.domainId);
  return d ? d.host : defaultDomainHost.value;
})
const copySuccess = ref(false)
const showAdvanced = ref(false)

const currentPlanId = computed(() => authStore.user?.currentPlanId ?? 1)
const canUseCustomDomain = computed(() => currentPlanId.value >= 2)
const canUseCustomSlug = computed(() => currentPlanId.value >= 2)
const canUsePremiumLinkSettings = computed(() => currentPlanId.value >= 2)
const canUseSocialPreview = computed(() => currentPlanId.value >= 2)

const result = ref<ShortLink | null>(null)
const error = ref('')
const loading = ref(false)

onMounted(async () => {
  if (canUseCustomDomain.value && authStore.accessToken) {
    try {
      domains.value = await UserService.getAvailableDomains(authStore.accessToken)
    } catch {
      // ignore
    }
  }

  // Handle URL passed from Spotlight search
  if (route.query.url) {
    form.value.originalUrl = route.query.url as string
  }
})

async function copyToClipboard(text: string) {
  try {
    if (navigator.clipboard?.writeText) {
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

async function submit() {
  error.value = ''

  if (!form.value.originalUrl.trim()) {
    error.value = 'Vui lòng nhập link gốc.'
    return
  }
  
  if (!form.value.originalUrl.startsWith('http://') && !form.value.originalUrl.startsWith('https://')) {
    error.value = 'Bạn phải nhập một link hợp lệ bắt đầu bằng http:// hoặc https://'
    return
  }
  if (form.value.customSlug && !/^[a-zA-Z0-9-_]+$/.test(form.value.customSlug)) {
    error.value = 'Hậu tố rút gọn chỉ được chứa chữ cái không dấu, số, gạch nối (-) và gạch dưới (_).'
    return
  }
  if (form.value.customSlug && !canUseCustomSlug.value) {
    error.value = 'Custom slug yêu cầu gói Pro hoặc Plus.'
    return
  }
  if (form.value.clickLimit !== null && (isNaN(Number(form.value.clickLimit)) || Number(form.value.clickLimit) <= 0)) {
    error.value = 'Giới hạn số click phải lớn hơn 0.'
    return
  }
  if (!canUsePremiumLinkSettings.value && (form.value.password || form.value.expiresAtUtc || form.value.clickLimit)) {
    error.value = 'Mật khẩu, ngày hết hạn và giới hạn click yêu cầu gói Pro hoặc Plus.'
    return
  }
  if (!canUseCustomDomain.value && form.value.domainId) {
    error.value = 'Tên miền riêng yêu cầu gói Pro hoặc Plus.'
    return
  }

  // Validate OG image URL - phải là link public https://, không nhận data: hay http:
  if (canUseSocialPreview.value && mockImg.value) {
    if (!mockImg.value.startsWith('https://')) {
      error.value = 'Ảnh xem trước phải là link ảnh công khai bắt đầu bằng https:// (không nhận data:, http:// hoặc đường dẫn cục bộ).'
      return
    }
  }

  loading.value = true

  try {
    if (!authStore.accessToken) throw new Error('Chưa xác thực')
    
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
      ogTitle: (canUseSocialPreview.value && mockTitle.value) ? mockTitle.value : undefined,
      ogDescription: (canUseSocialPreview.value && mockDesc.value) ? mockDesc.value : undefined,
      ogImageUrl: (canUseSocialPreview.value && mockImg.value) ? mockImg.value : undefined,
    })
    
    form.value = { ...defaultForm }
    showAdvanced.value = false
    // Reset OG fields
    mockTitle.value = ''
    mockDesc.value = ''
    mockImg.value = ''
  } catch (err) {
    error.value = err instanceof Error ? err.message : 'Đã có lỗi từ máy chủ. Không thể tạo link.'
  } finally {
    loading.value = false
  }
}
</script>

<template>
  <div class="ui-root" style="max-width: 1100px;">
    
    <!-- Header -->
    <div class="ui-header">
      <div class="ui-header-left">
        <h1 class="ui-title">Tạo shortlink mới</h1>
        <p class="ui-subtitle">Dán link dài của bạn vào đây, hệ thống sẽ thực hiện phần còn lại ngay lập tức.</p>
      </div>
    </div>

    <!-- Success Result -->
    <div v-if="result" class="ui-panel" style="border: 2px solid #10b981; overflow: visible;">
      <div class="ui-panel-body" style="display: flex; flex-direction: column; gap: 1.5rem; text-align: center; padding: 2.5rem 1.5rem;">
        
        <div style="display: flex; justify-content: center;">
          <div style="width: 64px; height: 64px; background: #ecfdf5; color: #10b981; border-radius: 50%; display: grid; place-items: center;">
            <CheckCircle2 :size="32" />
          </div>
        </div>

        <div>
          <h2 style="margin: 0 0 0.5rem; font-size: 1.5rem; color: #059669; font-weight: 800;">Link rút gọn đã sẵn sàng!</h2>
          <p style="margin: 0; color: #64748b; font-size: 0.9rem;">Bây giờ bạn có thể chia sẻ đường dẫn này ở khắp mọi nơi.</p>
        </div>

        <div style="background: #f8fafc; border: 1px dashed #cbd5e1; border-radius: 12px; padding: 1.25rem; display: flex; flex-direction: column; gap: 1rem; align-items: center; max-width: 500px; margin: 0 auto; width: 100%;">
          <a :href="result.shortUrl" target="_blank" style="font-size: 1.4rem; font-weight: 800; color: #3b82f6; text-decoration: none; word-break: break-all;">
            {{ result.shortUrl }}
          </a>
          <button 
            class="ui-btn" 
            :class="copySuccess ? 'ui-btn-outline' : 'ui-btn-primary'"
            style="width: 100%; border-radius: 8px; justify-content: center; height: 44px;"
            @click="copyToClipboard(result!.shortUrl)"
          >
            <CheckCircle2 v-if="copySuccess" :size="18"/>
            <Copy v-else :size="18"/>
            {{ copySuccess ? 'Đã lưu vào bộ nhớ tạm' : 'Sao chép đường dẫn' }}
          </button>
        </div>

        <div style="display: flex; gap: 1rem; justify-content: center; font-size: 0.85rem; font-weight: 600;">
          <button @click="result = null" style="color: #64748b; background: transparent; border: 0; cursor: pointer;">← Tạo link mới khác</button>
          <span style="color: #cbd5e1;">|</span>
          <button @click="router.push('/app/analytics')" style="color: #3b82f6; background: transparent; border: 0; cursor: pointer;">Xem báo cáo thống kê →</button>
        </div>
      </div>
    </div>

    <!-- Form + Social Preview (2-column layout) -->
    <div v-else class="create-layout lk-grid-responsive">

      <!-- Left: Form -->
      <div class="ui-panel">
        <div class="ui-panel-body" style="padding: 2rem;">
          <form @submit.prevent="submit" style="display: flex; flex-direction: column; gap: 2rem;">
            
            <!-- Step 1: Destination -->
            <div>
              <label class="ui-form-label" style="display: flex; align-items: center; gap: 0.4rem; font-size: 1rem; color: #0f172a; margin-bottom: 0.75rem;">
                <LinkIcon :size="18" style="color: #3b82f6;"/> Đích đến (Destination URL)
              </label>
              <input 
                v-model="form.originalUrl" 
                type="url" 
                required 
                class="ui-form-input" 
                style="padding: 1rem 1.25rem; font-size: 1.05rem; border-color: #cbd5e1; box-shadow: 0 2px 4px rgba(0,0,0,0.02);"
                placeholder="https://example.com/very/long/url..."
              />
            </div>

            <hr style="border: 0; border-top: 1px dotted #cbd5e1; margin: 0;" />

            <!-- Step 2: Customization -->
            <div>
               <label class="ui-form-label" style="display: flex; align-items: center; gap: 0.4rem; font-size: 1rem; color: #0f172a; margin-bottom: 0.75rem;">
                Cá nhân hóa đường dẫn (Tùy chọn)
              </label>
              <p style="font-size: 0.85rem; color: #64748b; margin: 0 0 1rem;">Làm cho shortlink của bạn uy tín hơn bằng cách dùng domain cá nhân và slug thân thiện.</p>
              
              <div style="display: flex; gap: 1rem; flex-wrap: wrap;">
                <div class="ui-form-group" style="flex: 1; min-width: 200px;">
                  <label class="ui-form-label">Tên miền</label>
                  <select 
                    v-if="domains.length > 0"
                    v-model="form.domainId" 
                    class="ui-form-select"
                  >
                    <option :value="null">{{ defaultDomainHost }} (Mặc định)</option>
                    <option v-for="d in domains.filter(d => !d.isDefault)" :key="d.id" :value="d.id">{{ d.host }}</option>
                  </select>
                  <input 
                    v-else
                    :value="defaultDomainHost" 
                    class="ui-form-input" 
                    disabled 
                    style="cursor: not-allowed; background: #f1f5f9; color: #64748b;"
                  />
                  <button v-if="!canUseCustomDomain" type="button" @click="router.push('/app/billing')" style="background: transparent; border: 0; padding: 0; text-align: left; font-size: 0.75rem; color: #f59e0b; margin-top: 0.35rem; font-weight: 600; cursor: pointer;">
                    <Lock :size="10" style="display: inline;" /> Nâng cấp Pro/Plus để sửa tên miền
                  </button>
                </div>

                <div class="ui-form-group" style="flex: 2; min-width: 200px;">
                  <label class="ui-form-label">Hậu tố (Slug)</label>
                  <input 
                    v-model="form.customSlug" 
                    type="text" 
                    class="ui-form-input" 
                    placeholder="Vd: uu-dai-thang-10"
                    :disabled="!canUseCustomSlug"
                  />
                  <p v-if="!canUseCustomSlug" style="font-size: 0.75rem; color: #f59e0b; margin: 0.35rem 0 0; font-weight: 600;">
                    <Lock :size="10" style="display: inline;" /> Yêu cầu gói Pro/Plus
                  </p>
                  <p v-if="form.customSlug" style="font-size: 0.8rem; color: #3b82f6; margin: 0.35rem 0 0; font-weight: 500;">
                    Bản xem trước: {{ selectedDomainHost }}/{{ form.customSlug }}
                  </p>
                </div>
              </div>

              <!-- Tags & Notes -->
              <div style="display: flex; gap: 1rem; flex-wrap: wrap; margin-top: 1.5rem;">
                <div class="ui-form-group" style="flex: 1; min-width: 200px; margin: 0;">
                  <label class="ui-form-label">Tag phân loại</label>
                  <input v-model="form.tag" type="text" class="ui-form-input" placeholder="Vd: marketing_fb" />
                </div>
                <div class="ui-form-group" style="flex: 2; min-width: 200px; margin: 0;">
                  <label class="ui-form-label">Ghi chú cá nhân</label>
                  <input v-model="form.description" type="text" class="ui-form-input" placeholder="Dùng để nhớ link này tạo ra làm gì..." />
                </div>
              </div>
            </div>

            <hr style="border: 0; border-top: 1px dotted #cbd5e1; margin: 0;" />

            <!-- Step 3: Advanced Options -->
            <div>
              <button 
                type="button" 
                @click="showAdvanced = !showAdvanced"
                style="display: flex; align-items: center; gap: 0.5rem; background: transparent; border: 0; padding: 0.5rem 0; font-size: 0.95rem; font-weight: 700; color: #475569; cursor: pointer;"
              >
                <Settings2 :size="18" /> Thiết lập nâng cao 
                <ChevronUp v-if="showAdvanced" :size="16" />
                <ChevronDown v-else :size="16" />
              </button>

              <div v-if="showAdvanced" style="margin-top: 1rem;">
                <div v-if="!canUsePremiumLinkSettings" style="background: #fffbeb; border: 1px solid #fde68a; padding: 1rem; border-radius: 10px; color: #b45309; font-size: 0.85rem; margin-bottom: 1rem;">
                  <Lock :size="14" style="vertical-align: middle; margin-right: 0.25rem;"/> Các tính năng bảo vệ bằng mật khẩu, ngày hết hạn và giới hạn click yêu cầu <a href="/app/billing" style="color: #b45309; font-weight: 700; text-decoration: underline;">nâng cấp tài khoản</a>.
                </div>

                <div style="display: grid; grid-template-columns: repeat(auto-fit, minmax(240px, 1fr)); gap: 1rem; background: #f8fafc; padding: 1.25rem; border-radius: 12px; border: 1px solid #e2e8f0;">
                  <div class="ui-form-group" style="margin: 0;">
                    <label class="ui-form-label">Mật khẩu bảo vệ</label>
                    <input v-model="form.password" type="text" class="ui-form-input" placeholder="Để trống nếu không bảo mật" :disabled="!canUsePremiumLinkSettings" />
                  </div>
                  <div class="ui-form-group" style="margin: 0;">
                    <label class="ui-form-label">Ngày hết hạn</label>
                    <input v-model="form.expiresAtUtc" type="datetime-local" class="ui-form-input" :disabled="!canUsePremiumLinkSettings" />
                  </div>
                  <div class="ui-form-group" style="margin: 0; grid-column: 1 / -1;">
                    <label class="ui-form-label">Giới hạn Click</label>
                    <input v-model="form.clickLimit" type="number" min="1" class="ui-form-input" placeholder="Vd: 100" :disabled="!canUsePremiumLinkSettings" />
                  </div>
                </div>
              </div>
            </div>

            <!-- Error Alert -->
            <div v-if="error" class="ui-alert ui-alert-error" style="margin-top: -1rem;">
              <AlertCircle :size="16" /> {{ error }}
            </div>

            <!-- Actions -->
            <div style="display: flex; gap: 1rem; align-items: center; margin-top: 0.5rem;">
              <button type="submit" class="ui-btn ui-btn-primary" :disabled="loading" style="padding: 0.8rem 2rem; font-size: 1rem;">
                {{ loading ? 'Đang xử lý...' : 'Tạo Shortlink ngay' }}
              </button>
              <button type="button" class="ui-btn ui-btn-ghost" @click="router.push('/app/links')">Hủy</button>
            </div>

          </form>
        </div>
      </div>

      <!-- Right: Social Preview -->
      <div class="ui-panel" style="position: sticky; top: 1.5rem;">
        <div style="padding: 1rem 1.25rem; border-bottom: 1px solid #e2e8f0; display: flex; align-items: center; gap: 0.5rem;">
          <Image :size="16" style="color: #3b82f6;" />
          <h3 style="margin: 0; font-size: 0.875rem; font-weight: 700; color: #0f172a;">Xem trước Mạng xã hội</h3>
        </div>
        <div style="padding: 1.25rem; background: #f8fafc; border-radius: 0 0 12px 12px;">
          <!-- Mock OG Editor -->
          <div style="display: flex; flex-direction: column; gap: 0.75rem; margin-bottom: 1.5rem;">
            <label style="font-size: 11px; font-weight: 700; color: #94a3b8; text-transform: uppercase; letter-spacing: 0.08em;">Điền thông tin hiển thị khi chia sẻ</label>
            <div>
              <input v-model="mockTitle" class="ui-form-input" style="font-size: 0.875rem; padding: 0.5rem 0.75rem;" placeholder="Tiêu đề (vd: Ưu đãi tháng 4 - Giảm 50%)" :disabled="!canUseSocialPreview" maxlength="150" />
            </div>
            <div>
              <textarea v-model="mockDesc" class="ui-form-input" style="font-size: 0.875rem; padding: 0.5rem 0.75rem; resize: none; height: 64px;" placeholder="Mô tả ngắn hiển thị bên dưới tiêu đề..." :disabled="!canUseSocialPreview" maxlength="500"></textarea>
            </div>
            <div>
              <div style="position: relative;">
                <input
                  v-model="mockImg"
                  class="ui-form-input"
                  style="font-size: 0.875rem; padding: 0.5rem 0.75rem 0.5rem 2.25rem;"
                  placeholder="https://i.imgur.com/abc123.jpg"
                  :disabled="!canUseSocialPreview"
                />
                <span style="position: absolute; left: 0.65rem; top: 50%; transform: translateY(-50%); color: #94a3b8; font-size: 13px;">🔗</span>
              </div>
              <p style="font-size: 11px; color: #64748b; margin: 4px 0 0; padding-left: 2px;">
                Nhập link ảnh công khai từ internet (phải bắt đầu bằng <code style="background:#f1f5f9;padding:1px 4px;border-radius:3px;">https://</code>)
              </p>
              <!-- Cảnh báo realtime nếu URL ảnh sai định dạng -->
              <p
                v-if="mockImg && !mockImg.startsWith('https://')"
                style="font-size: 11px; color: #ef4444; font-weight: 500; margin: 4px 0 0; display: flex; align-items: center; gap: 4px;"
              >
                ⚠️ URL ảnh không hợp lệ — phải bắt đầu bằng <code style="background:#fef2f2;padding:1px 4px;border-radius:3px;">https://</code>. Không chấp nhận ảnh base64 hoặc data URL.
              </p>
            </div>
            <p v-if="!canUseSocialPreview" style="font-size: 11px; color: #d97706; font-weight: 500; line-height: 1.5; margin: 0; background: #fffbeb; padding: 0.5rem 0.75rem; border-radius: 6px; border: 1px solid #fde68a;">
              ⚠️ Cần nâng cấp lên gói Pro hoặc Plus để bật tính năng tùy chỉnh Thẻ xem trước mạng xã hội (OpenGraph).
            </p>
          </div>
          
          <!-- Preview Card -->
          <label style="font-size: 11px; font-weight: 700; color: #94a3b8; text-transform: uppercase; letter-spacing: 0.08em; display: block; margin-bottom: 0.5rem;">Xem trước card khi chia sẻ lên MXH:</label>
          <div style="background: white; border-radius: 12px; box-shadow: 0 1px 4px rgba(0,0,0,0.08); border: 1px solid #e2e8f0; overflow: hidden; cursor: pointer; transition: box-shadow 0.15s;">
            <!-- Cover image -->
            <div style="width: 100%; height: 160px; background: #f1f5f9; display: flex; align-items: center; justify-content: center; overflow: hidden; border-bottom: 1px solid #f1f5f9;">
              <img v-if="mockImg" :src="mockImg" style="width: 100%; height: 100%; object-fit: cover;" />
              <div v-else style="display: flex; flex-direction: column; align-items: center; color: #94a3b8;">
                <Image :size="28" style="opacity: 0.5; margin-bottom: 4px;" />
                <span style="font-size: 10px; font-weight: 600; letter-spacing: 0.1em;">CHƯA CÓ ẢNH</span>
              </div>
            </div>
            <!-- Content -->
            <div style="padding: 0.75rem; background: #f8fafc;">
              <p style="font-size: 10px; color: #94a3b8; font-weight: 600; text-transform: uppercase; letter-spacing: 0.06em; margin: 0 0 4px; overflow: hidden; white-space: nowrap; text-overflow: ellipsis;">
                {{ selectedDomainHost.toUpperCase() }}{{ form.customSlug ? '/' + form.customSlug.toUpperCase() : '' }}
              </p>
              <h4 style="font-size: 15px; font-weight: 700; color: #0f172a; line-height: 1.35; margin: 0 0 4px; display: -webkit-box; -webkit-line-clamp: 2; line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; word-break: break-word;">
                {{ mockTitle || form.originalUrl || 'WeShort - Hệ thống chuyển hướng mạnh mẽ' }}
              </h4>
              <p style="font-size: 13px; color: #64748b; margin: 0; overflow: hidden; white-space: nowrap; text-overflow: ellipsis; word-break: break-word;">
                {{ mockDesc || 'Click vào link để xem nội dung chi tiết. Tạo bởi hệ thống quản trị WeShort...' }}
              </p>
            </div>
          </div>
        </div>
      </div>

    </div>

  </div>
</template>

<style scoped>
.lk-grid-responsive {
  display: grid;
  grid-template-columns: 1fr 380px;
  gap: 1.5rem;
  align-items: start;
}
@media (max-width: 900px) {
  .lk-grid-responsive {
    grid-template-columns: 1fr;
  }
}
</style>
