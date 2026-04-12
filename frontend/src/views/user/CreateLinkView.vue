<script setup lang="ts">
import { computed, ref, onMounted } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { LinkService, UserService } from '@/api/services'
import { useAuthStore } from '@/stores/auth'
import type { ShortLink } from '@/types/api'
import { Settings2, Link as LinkIcon, AlertCircle, Copy, CheckCircle2, ChevronDown, ChevronUp, Lock, Image, ShieldCheck, Info, Sparkles } from 'lucide-vue-next'

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
  isWrapperEnabled: false,
  redirectMode: 'Instant',
  delaySeconds: 3,
  wrapperTitle: '',
  wrapperDescription: '',
  wrapperImageUrl: '',
  continueButtonText: 'Tiếp tục đến trang đích',
  warningText: 'Bạn sắp rời khỏi website hiện tại.',
  wrapperTheme: 'brand',
  brandName: '',
  brandLogoUrl: '',
  ctaTitle: '',
  ctaDescription: '',
  ctaButtonText: '',
  ctaButtonUrl: '',
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
const showWrapperSection = ref(true)

const canUseCustomDomain = computed(() => authStore.hasCapability('domains.custom'))
const canUseCustomSlug = computed(() => authStore.hasCapability('links.custom_slug'))
const canUsePasswordProtection = computed(() => authStore.hasCapability('links.password_protection'))
const canUseExpiration = computed(() => authStore.hasCapability('links.expiration'))
const canUseClickLimit = computed(() => authStore.hasCapability('links.click_limit'))
const canUsePremiumLinkSettings = computed(() =>
  canUsePasswordProtection.value || canUseExpiration.value || canUseClickLimit.value,
)
const canUseSocialPreview = computed(() => authStore.hasCapability('links.social_preview'))
const canUseWrapper = computed(() => authStore.hasCapability('links.wrapper'))
const canUseWrapperLanding = computed(() => authStore.hasCapability('links.wrapper_landing'))
const canUseWrapperCta = computed(() => authStore.hasCapability('links.wrapper_cta'))
const wrapperEnabled = computed(() => canUseWrapper.value && form.value.isWrapperEnabled)
const wrapperModeLabel = computed(() => {
  switch (form.value.redirectMode) {
    case 'Delay':
      return 'Đếm ngược rồi chuyển'
    case 'ManualContinue':
      return 'Người xem bấm mới đi tiếp'
    case 'LandingPage':
      return 'Landing page có CTA'
    default:
      return 'Chuyển hướng ngay'
  }
})
const wrapperStatusLabel = computed(() => {
  if (!canUseWrapper.value) return 'Mở bằng Pro'
  return form.value.isWrapperEnabled ? 'Đang bật' : 'Đang tắt'
})
const wrapperPreviewBadges = computed(() => {
  const badges = ['Branded', 'Tracked']
  if (form.value.warningText?.trim()) badges.push('Safe redirect')
  return badges
})
const wrapperPreviewTheme = computed(() => {
  if (form.value.wrapperTheme === 'dark') {
    return {
      shell: 'linear-gradient(180deg, #0f172a 0%, #111827 100%)',
      hero: 'linear-gradient(135deg, #111827 0%, #1f2937 100%)',
      text: '#f8fafc',
      muted: 'rgba(226,232,240,0.82)',
      accent: '#60a5fa',
      border: 'rgba(148,163,184,0.22)',
    }
  }

  if (form.value.wrapperTheme === 'light') {
    return {
      shell: 'linear-gradient(180deg, #f8fafc 0%, #e2e8f0 100%)',
      hero: 'linear-gradient(135deg, #ffffff 0%, #e2e8f0 100%)',
      text: '#0f172a',
      muted: '#475569',
      accent: '#2563eb',
      border: 'rgba(148,163,184,0.3)',
    }
  }

  return {
    shell: 'linear-gradient(180deg, #0f172a 0%, #111827 100%)',
    hero: 'linear-gradient(135deg, #0ea5e9 0%, #2563eb 55%, #1d4ed8 100%)',
    text: '#eff6ff',
    muted: 'rgba(219,234,254,0.82)',
    accent: '#93c5fd',
    border: 'rgba(96,165,250,0.28)',
  }
})

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

function setWrapperEnabled(enabled: boolean) {
  if (!canUseWrapper.value) return
  form.value.isWrapperEnabled = enabled
  if (!enabled) {
    form.value.redirectMode = 'Instant'
  }
}

function setRedirectMode(mode: string) {
  if (mode === 'LandingPage' && !canUseWrapperLanding.value) return
  form.value.redirectMode = mode
}

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
  if (form.value.password && !canUsePasswordProtection.value) {
    error.value = 'Bảo vệ link bằng mật khẩu chưa sẵn sàng trên gói hiện tại.'
    return
  }
  if (form.value.expiresAtUtc && !canUseExpiration.value) {
    error.value = 'Cài đặt ngày hết hạn chưa sẵn sàng trên gói hiện tại.'
    return
  }
  if (form.value.clickLimit && !canUseClickLimit.value) {
    error.value = 'Giới hạn click chưa sẵn sàng trên gói hiện tại.'
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

  if (form.value.isWrapperEnabled && !canUseWrapper.value) {
    error.value = 'Tính năng bọc link yêu cầu gói Pro hoặc Plus.'
    return
  }

  if (form.value.isWrapperEnabled && form.value.redirectMode === 'LandingPage' && !canUseWrapperLanding.value) {
    error.value = 'Landing wrapper chỉ có trong gói Plus.'
    return
  }

  if (!canUseWrapperCta.value && (form.value.ctaTitle || form.value.ctaDescription || form.value.ctaButtonText || form.value.ctaButtonUrl)) {
    error.value = 'CTA block chỉ có trong gói Plus.'
    return
  }

  if (form.value.isWrapperEnabled && form.value.redirectMode === 'Delay') {
    const delay = Number(form.value.delaySeconds)
    if (Number.isNaN(delay) || delay < 1 || delay > 30) {
      error.value = 'Delay redirect chỉ hỗ trợ từ 1 đến 30 giây.'
      return
    }
  }

  for (const [fieldLabel, value] of [
    ['Ảnh wrapper', form.value.wrapperImageUrl],
    ['Logo thương hiệu', form.value.brandLogoUrl],
    ['CTA URL', form.value.ctaButtonUrl],
  ] as const) {
    if (value && !String(value).startsWith('https://')) {
      error.value = `${fieldLabel} phải là link https:// công khai.`
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
      expiresAtUtc: canUseExpiration.value ? expiresDate : null,
      clickLimit: canUseClickLimit.value && form.value.clickLimit ? Number(form.value.clickLimit) : null,
      customSlug: form.value.customSlug || undefined,
      description: form.value.description || undefined,
      domainId: form.value.domainId || null,
      tag: form.value.tag || undefined,
      password: canUsePasswordProtection.value ? (form.value.password || undefined) : undefined,
      ogTitle: (canUseSocialPreview.value && mockTitle.value) ? mockTitle.value : undefined,
      ogDescription: (canUseSocialPreview.value && mockDesc.value) ? mockDesc.value : undefined,
      ogImageUrl: (canUseSocialPreview.value && mockImg.value) ? mockImg.value : undefined,
      isWrapperEnabled: canUseWrapper.value ? !!form.value.isWrapperEnabled : false,
      redirectMode: form.value.isWrapperEnabled ? form.value.redirectMode : 'Instant',
      delaySeconds: form.value.isWrapperEnabled && form.value.redirectMode === 'Delay' ? Number(form.value.delaySeconds) : null,
      wrapperTitle: form.value.isWrapperEnabled ? (form.value.wrapperTitle || undefined) : undefined,
      wrapperDescription: form.value.isWrapperEnabled ? (form.value.wrapperDescription || undefined) : undefined,
      wrapperImageUrl: form.value.isWrapperEnabled ? (form.value.wrapperImageUrl || undefined) : undefined,
      continueButtonText: form.value.isWrapperEnabled ? (form.value.continueButtonText || undefined) : undefined,
      warningText: form.value.isWrapperEnabled ? (form.value.warningText || undefined) : undefined,
      wrapperTheme: form.value.isWrapperEnabled ? (form.value.wrapperTheme || undefined) : undefined,
      brandName: form.value.isWrapperEnabled ? (form.value.brandName || undefined) : undefined,
      brandLogoUrl: form.value.isWrapperEnabled ? (form.value.brandLogoUrl || undefined) : undefined,
      ctaTitle: form.value.isWrapperEnabled && canUseWrapperCta.value ? (form.value.ctaTitle || undefined) : undefined,
      ctaDescription: form.value.isWrapperEnabled && canUseWrapperCta.value ? (form.value.ctaDescription || undefined) : undefined,
      ctaButtonText: form.value.isWrapperEnabled && canUseWrapperCta.value ? (form.value.ctaButtonText || undefined) : undefined,
      ctaButtonUrl: form.value.isWrapperEnabled && canUseWrapperCta.value ? (form.value.ctaButtonUrl || undefined) : undefined,
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

            <!-- Step 3: Wrapper -->
            <div class="wrapper-section">
              <div class="wrapper-section-header" @click="showWrapperSection = !showWrapperSection">
                <div class="wrapper-section-header__left">
                  <div class="wrapper-section-icon">
                    <ShieldCheck :size="18" />
                  </div>
                  <div>
                    <div class="wrapper-section-title">Bọc link & Smart Redirect</div>
                    <p class="wrapper-section-subtitle">Thêm lớp trung gian có thương hiệu để tăng độ tin cậy, giữ traffic và mở khóa CTA trước khi chuyển đích.</p>
                  </div>
                </div>
                <div class="wrapper-section-header__right">
                  <button type="button" class="wrapper-status-pill" :class="{ 'wrapper-status-pill--active': wrapperEnabled }" @click.stop="setWrapperEnabled(!form.isWrapperEnabled)">
                    {{ wrapperStatusLabel }}
                  </button>
                  <ChevronUp v-if="showWrapperSection" :size="18" />
                  <ChevronDown v-else :size="18" />
                </div>
              </div>

              <div v-if="showWrapperSection" class="wrapper-section-body">
                <div v-if="!canUseWrapper" class="wrapper-upsell">
                  <div class="wrapper-upsell-copy">
                    <span class="wrapper-upsell-badge">Pro / Plus</span>
                    <h3>Biến short link thành một điểm chạm bán hàng</h3>
                    <p>Smart Wrapper giúp link trông đáng tin hơn, giữ người xem trước redirect và tạo thêm chỗ để gắn CTA, cảnh báo hoặc thông điệp thương hiệu.</p>
                  </div>
                  <div class="wrapper-upsell-points">
                    <div class="wrapper-point-card">
                      <strong>Branded page</strong>
                      <span>Hiện tên shop, logo, ảnh chờ và lời nhắc đáng tin cậy.</span>
                    </div>
                    <div class="wrapper-point-card">
                      <strong>Smart redirect</strong>
                      <span>Đếm ngược, bấm thủ công hoặc landing page theo từng chiến dịch.</span>
                    </div>
                    <div class="wrapper-point-card">
                      <strong>Grow conversion</strong>
                      <span>Tăng tỷ lệ tiếp tục và tạo cảm giác chuyên nghiệp trước khi rời trang.</span>
                    </div>
                  </div>
                  <button type="button" class="ui-btn ui-btn-primary" @click="router.push('/app/billing')">
                    Nâng cấp để mở Smart Wrapper
                  </button>
                </div>

                <div v-else class="wrapper-live-config">
                  <div class="wrapper-switch-row">
                    <button type="button" class="wrapper-switch-card" :class="{ 'wrapper-switch-card--active': !form.isWrapperEnabled }" @click="setWrapperEnabled(false)">
                      <span class="wrapper-switch-card__eyebrow">Cơ bản</span>
                      <strong>Redirect thường</strong>
                      <span>Phù hợp khi chỉ cần rút gọn link và chuyển hướng nhanh nhất.</span>
                    </button>
                    <button type="button" class="wrapper-switch-card" :class="{ 'wrapper-switch-card--active': form.isWrapperEnabled }" @click="setWrapperEnabled(true)">
                      <span class="wrapper-switch-card__eyebrow">Upsell tốt hơn</span>
                      <strong>Bật Smart Wrapper</strong>
                      <span>Hiển thị trang trung gian có thương hiệu, warning và CTA trước khi đi tiếp.</span>
                    </button>
                  </div>

                  <div v-if="form.isWrapperEnabled" class="wrapper-config-panel">
                    <div class="wrapper-banner">
                      <Info :size="12" />
                      <span>Wrapper phù hợp cho bán hàng, affiliate, link bio, chiến dịch social và các link cần tăng độ tin cậy.</span>
                    </div>

                    <div class="wrapper-mode-grid">
                      <button type="button" class="wrapper-mode-card" :class="{ 'wrapper-mode-card--active': form.redirectMode === 'Instant' }" @click="setRedirectMode('Instant')">
                        <span class="wrapper-mode-card__label">Nhanh nhất</span>
                        <strong>Chuyển hướng ngay</strong>
                        <span>Vẫn có lớp thương hiệu nhưng ưu tiên tốc độ.</span>
                      </button>
                      <button type="button" class="wrapper-mode-card" :class="{ 'wrapper-mode-card--active': form.redirectMode === 'Delay' }" @click="setRedirectMode('Delay')">
                        <span class="wrapper-mode-card__label">Tăng chú ý</span>
                        <strong>Đếm ngược</strong>
                        <span>Giữ người xem vài giây trước khi chuyển sang đích.</span>
                      </button>
                      <button type="button" class="wrapper-mode-card" :class="{ 'wrapper-mode-card--active': form.redirectMode === 'ManualContinue' }" @click="setRedirectMode('ManualContinue')">
                        <span class="wrapper-mode-card__label">Tin cậy hơn</span>
                        <strong>Bấm thủ công</strong>
                        <span>Người xem chủ động xác nhận trước khi đi tiếp.</span>
                      </button>
                      <button type="button" class="wrapper-mode-card" :class="{ 'wrapper-mode-card--active': form.redirectMode === 'LandingPage', 'wrapper-mode-card--locked': !canUseWrapperLanding }" @click="setRedirectMode('LandingPage')">
                        <span class="wrapper-mode-card__label">Plus</span>
                        <strong>Landing page</strong>
                        <span>{{ canUseWrapperLanding ? 'Hiện trang chuyển hướng có CTA và nội dung bán hàng.' : 'Mở khóa Plus để thêm landing page có CTA.' }}</span>
                      </button>
                    </div>

                    <div class="wrapper-mode-summary">
                      <div>
                        <span class="wrapper-mode-summary__eyebrow">Cách hoạt động</span>
                        <strong>{{ wrapperModeLabel }}</strong>
                      </div>
                      <div v-if="form.redirectMode === 'Delay'" class="wrapper-delay-inline">
                        <label class="ui-form-label">Số giây chờ</label>
                        <input v-model="form.delaySeconds" type="number" min="1" max="30" class="ui-form-input" />
                      </div>
                      <div>
                        <label class="ui-form-label">Chủ đề màu</label>
                        <select v-model="form.wrapperTheme" class="ui-form-select">
                          <option value="brand">Thương hiệu</option>
                          <option value="light">Sáng tối giản</option>
                          <option value="dark">Tối hiện đại</option>
                        </select>
                      </div>
                    </div>

                    <div class="wrapper-fields-card">
                      <div class="wrapper-field-grid">
                        <div class="ui-form-group" style="margin: 0;">
                          <label class="ui-form-label">Tiêu đề chính</label>
                          <input v-model="form.wrapperTitle" class="ui-form-input" placeholder="Ví dụ: Bạn sắp đến trang ưu đãi chính thức" />
                        </div>
                        <div class="ui-form-group" style="margin: 0;">
                          <label class="ui-form-label">Nội dung nút</label>
                          <input v-model="form.continueButtonText" class="ui-form-input" placeholder="Tiếp tục đến trang đích" />
                        </div>
                      </div>

                      <div class="ui-form-group" style="margin: 0;">
                        <label class="ui-form-label">Mô tả ngắn</label>
                        <textarea v-model="form.wrapperDescription" class="ui-form-input" style="resize: none; min-height: 72px;" placeholder="Nói ngắn gọn vì sao người xem nên tiếp tục, ví dụ ưu đãi, thông tin xác thực hoặc lưu ý trước khi rời trang hiện tại."></textarea>
                      </div>

                      <div class="wrapper-field-grid">
                        <div class="ui-form-group" style="margin: 0;">
                          <label class="ui-form-label">Ảnh chờ</label>
                          <input v-model="form.wrapperImageUrl" class="ui-form-input" placeholder="https://..." />
                        </div>
                        <div class="ui-form-group" style="margin: 0;">
                          <label class="ui-form-label">Lời nhắc nhở</label>
                          <input v-model="form.warningText" class="ui-form-input" placeholder="Bạn sắp rời khỏi website hiện tại." />
                        </div>
                      </div>

                      <div class="wrapper-field-grid wrapper-field-grid--brand">
                        <div class="ui-form-group" style="margin: 0;">
                          <label class="ui-form-label">Tên shop / thương hiệu</label>
                          <input v-model="form.brandName" class="ui-form-input" placeholder="Ví dụ: WeShort Team" />
                        </div>
                        <div class="ui-form-group" style="margin: 0;">
                          <label class="ui-form-label">Link logo</label>
                          <input v-model="form.brandLogoUrl" class="ui-form-input" placeholder="https://..." />
                        </div>
                      </div>
                    </div>

                    <div v-if="canUseWrapperCta" class="wrapper-cta-card">
                      <div class="wrapper-cta-card__head">
                        <span class="wrapper-upsell-badge wrapper-upsell-badge--gold">
                          <Sparkles :size="12" />
                          CTA kiếm tiền
                        </span>
                        <p>Thêm lời mời hành động hoặc ưu đãi để tận dụng traffic trước khi người xem đi tiếp.</p>
                      </div>
                      <div class="wrapper-field-grid">
                        <div class="ui-form-group" style="margin: 0;">
                          <label class="ui-form-label">Tiêu đề CTA</label>
                          <input v-model="form.ctaTitle" class="ui-form-input" placeholder="Ví dụ: Mã giảm giá 50%" />
                        </div>
                        <div class="ui-form-group" style="margin: 0;">
                          <label class="ui-form-label">Nút CTA</label>
                          <input v-model="form.ctaButtonText" class="ui-form-input" placeholder="Xem ngay" />
                        </div>
                      </div>
                      <div class="ui-form-group" style="margin: 0;">
                        <label class="ui-form-label">Link CTA</label>
                        <input v-model="form.ctaButtonUrl" class="ui-form-input" placeholder="Dán link affiliate, ưu đãi hoặc trang bán hàng..." />
                      </div>
                    </div>
                    <div v-else class="wrapper-cta-locked">
                      <strong>CTA block là điểm ăn tiền nhất của Plus</strong>
                      <span>Thêm banner khuyến mãi, affiliate hoặc nút bán hàng ngay trong wrapper để tăng conversion trước redirect.</span>
                    </div>
                  </div>
                </div>
              </div>
            </div>
            <!-- Step 4: Advanced Options -->
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

          <div class="wrapper-preview-shell">
            <div class="wrapper-preview-shell__header">
              <div>
                <span class="wrapper-preview-shell__eyebrow">Preview wrapper</span>
                <h4>Trang trung gian cao cấp</h4>
              </div>
              <span class="wrapper-preview-mode-pill">{{ wrapperModeLabel }}</span>
            </div>

            <div
              class="wrapper-preview-device"
              :style="{
                background: wrapperPreviewTheme.shell,
                color: wrapperPreviewTheme.text,
                borderColor: wrapperPreviewTheme.border,
              }"
            >
              <div class="wrapper-preview-hero" :style="{ background: wrapperPreviewTheme.hero, color: wrapperPreviewTheme.text }">
                <div class="wrapper-preview-brandline">
                  <div class="wrapper-preview-brandmark">
                    <img v-if="form.brandLogoUrl" :src="form.brandLogoUrl" alt="Brand logo" />
                    <span v-else>{{ (form.brandName || selectedDomainHost).slice(0, 2).toUpperCase() }}</span>
                  </div>
                  <div>
                    <strong>{{ form.brandName || selectedDomainHost }}</strong>
                    <small>{{ form.isWrapperEnabled ? 'Smart Wrapper đang bật' : 'Preview wrapper demo' }}</small>
                  </div>
                </div>
                <div class="wrapper-preview-proof">
                  <span v-for="badge in wrapperPreviewBadges" :key="badge">{{ badge }}</span>
                </div>
              </div>

              <div class="wrapper-preview-body">
                <img v-if="form.wrapperImageUrl" :src="form.wrapperImageUrl" class="wrapper-preview-cover" alt="Wrapper preview" />

                <div class="wrapper-preview-copy">
                  <h5>{{ form.wrapperTitle || mockTitle || 'Bạn sắp đến trang đích' }}</h5>
                  <p>{{ form.wrapperDescription || mockDesc || 'Hiển thị một lớp trung gian có thương hiệu để người xem yên tâm trước khi tiếp tục.' }}</p>
                </div>

                <div class="wrapper-preview-warning">
                  {{ form.warningText || 'Bạn sắp rời khỏi website hiện tại.' }}
                </div>

                <div v-if="form.redirectMode === 'Delay'" class="wrapper-preview-counter">
                  Tự động chuyển sau {{ form.delaySeconds || 3 }} giây
                </div>

                <button type="button" class="wrapper-preview-primary">
                  {{ form.continueButtonText || 'Tiếp tục đến trang đích' }}
                </button>

                <div v-if="canUseWrapperCta && (form.ctaTitle || form.ctaButtonText || form.ctaButtonUrl)" class="wrapper-preview-offer">
                  <span class="wrapper-preview-offer__badge">CTA block</span>
                  <strong>{{ form.ctaTitle || 'Ưu đãi bổ sung cho traffic này' }}</strong>
                  <p>{{ form.ctaDescription || 'Dùng block này để đặt affiliate, giảm giá hoặc cross-sell trước khi redirect.' }}</p>
                  <button type="button" class="wrapper-preview-secondary">
                    {{ form.ctaButtonText || 'Xem ưu đãi' }}
                  </button>
                </div>

                <div class="wrapper-preview-footer">
                  Trang trung gian này giúp link trông uy tín hơn, giữ traffic tốt hơn và tạo thêm điểm chạm bán hàng.
                </div>
              </div>
            </div>
          </div>

        </div>
      </div>

    </div>
  </div>
</template>

<style scoped>
.create-layout {
  align-items: start;
}

.lk-grid-responsive {
  display: grid;
  grid-template-columns: 1fr 380px;
  gap: 1.5rem;
  align-items: start;
}

.wrapper-section {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.wrapper-section-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 1rem;
  width: 100%;
  padding: 1.05rem 1.15rem;
  border: 1px solid #dbe4f0;
  border-radius: 16px;
  background: linear-gradient(180deg, #ffffff 0%, #f8fbff 100%);
  cursor: pointer;
  text-align: left;
  box-shadow: 0 14px 32px rgba(15, 23, 42, 0.05);
}

.wrapper-section-header__left,
.wrapper-section-header__right {
  display: flex;
  align-items: center;
  gap: 0.9rem;
}

.wrapper-section-icon {
  display: grid;
  place-items: center;
  width: 2.6rem;
  height: 2.6rem;
  border-radius: 14px;
  background: linear-gradient(135deg, #dbeafe 0%, #e0e7ff 100%);
  color: #2563eb;
  flex-shrink: 0;
}

.wrapper-section-title {
  font-size: 1rem;
  font-weight: 800;
  color: #0f172a;
}

.wrapper-section-subtitle {
  margin: 0.25rem 0 0;
  font-size: 0.83rem;
  line-height: 1.5;
  color: #64748b;
}

.wrapper-status-pill {
  display: inline-flex;
  align-items: center;
  padding: 0.35rem 0.7rem;
  border-radius: 999px;
  background: #eef2ff;
  color: #4f46e5;
  font-size: 0.72rem;
  font-weight: 800;
  letter-spacing: 0.03em;
  text-transform: uppercase;
  white-space: nowrap;
  border: none;
  cursor: pointer;
  outline: none;
}

.wrapper-status-pill--active {
  background: #dcfce7;
  color: #15803d;
}

.wrapper-section-body {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.wrapper-upsell,
.wrapper-config-panel {
  display: flex;
  flex-direction: column;
  gap: 1rem;
  padding: 1.2rem;
  border: 1px solid #dbe4f0;
  border-radius: 18px;
  background: linear-gradient(180deg, #ffffff 0%, #f8fbff 100%);
  box-shadow: 0 16px 36px rgba(15, 23, 42, 0.06);
}

.wrapper-upsell-copy h3 {
  margin: 0.35rem 0 0.5rem;
  font-size: 1.15rem;
  color: #0f172a;
}

.wrapper-upsell-copy p,
.wrapper-cta-card__head p {
  margin: 0;
  color: #64748b;
  line-height: 1.6;
}

.wrapper-upsell-badge {
  display: inline-flex;
  align-items: center;
  gap: 0.35rem;
  width: fit-content;
  padding: 0.3rem 0.7rem;
  border-radius: 999px;
  background: #eff6ff;
  color: #2563eb;
  font-size: 0.72rem;
  font-weight: 800;
  text-transform: uppercase;
  letter-spacing: 0.03em;
}

.wrapper-upsell-badge--gold {
  background: #fff7ed;
  color: #c2410c;
}

.wrapper-upsell-points,
.wrapper-mode-grid,
.wrapper-field-grid,
.wrapper-switch-row {
  display: grid;
  gap: 0.9rem;
}

.wrapper-upsell-points,
.wrapper-mode-grid,
.wrapper-switch-row {
  grid-template-columns: repeat(auto-fit, minmax(180px, 1fr));
}

.wrapper-point-card,
.wrapper-switch-card,
.wrapper-mode-card {
  display: flex;
  flex-direction: column;
  gap: 0.4rem;
  padding: 1rem;
  border-radius: 16px;
  border: 1px solid #dbe4f0;
  background: #fff;
}

.wrapper-point-card span,
.wrapper-switch-card span,
.wrapper-mode-card span,
.wrapper-cta-locked span {
  color: #64748b;
  line-height: 1.55;
}

.wrapper-switch-card,
.wrapper-mode-card {
  cursor: pointer;
  text-align: left;
  transition: transform 0.18s ease, border-color 0.18s ease, box-shadow 0.18s ease;
}

.wrapper-switch-card:hover,
.wrapper-mode-card:hover {
  transform: translateY(-1px);
  border-color: #93c5fd;
  box-shadow: 0 12px 24px rgba(37, 99, 235, 0.08);
}

.wrapper-switch-card--active,
.wrapper-mode-card--active {
  border-color: #2563eb;
  background: linear-gradient(180deg, #eff6ff 0%, #ffffff 100%);
  box-shadow: 0 16px 30px rgba(37, 99, 235, 0.12);
}

.wrapper-mode-card--locked {
  opacity: 0.78;
}

.wrapper-switch-card__eyebrow,
.wrapper-mode-card__label,
.wrapper-mode-summary__eyebrow,
.wrapper-preview-shell__eyebrow {
  font-size: 0.72rem;
  font-weight: 800;
  letter-spacing: 0.04em;
  text-transform: uppercase;
  color: #64748b;
}

.wrapper-banner {
  display: flex;
  align-items: center;
  gap: 0.45rem;
  padding: 0.7rem 0.85rem;
  border-radius: 12px;
  background: #eff6ff;
  color: #2563eb;
  font-size: 0.8rem;
  border: 1px solid #dbeafe;
}

.wrapper-mode-summary {
  display: grid;
  grid-template-columns: 1.2fr auto auto;
  gap: 1rem;
  padding: 1rem;
  border-radius: 16px;
  background: #0f172a;
  color: #e2e8f0;
}

.wrapper-mode-summary strong {
  display: block;
  margin-top: 0.3rem;
  color: #fff;
}

.wrapper-delay-inline {
  min-width: 120px;
}

.wrapper-delay-inline :deep(.ui-form-label),
.wrapper-mode-summary > div > .ui-form-label {
  color: #cbd5e1;
  margin-bottom: 0.4rem;
}

.wrapper-fields-card,
.wrapper-cta-card {
  padding: 1rem;
  border-radius: 16px;
  background: #fff;
  border: 1px solid #e2e8f0;
}

.wrapper-field-grid {
  grid-template-columns: repeat(auto-fit, minmax(220px, 1fr));
  margin-bottom: 0.9rem;
}

.wrapper-field-grid:last-child {
  margin-bottom: 0;
}

.wrapper-field-grid--brand {
  padding-top: 0.9rem;
  border-top: 1px solid #e2e8f0;
}

.wrapper-cta-card {
  background: linear-gradient(180deg, #fffbeb 0%, #fff7ed 100%);
  border-color: #fed7aa;
}

.wrapper-cta-card__head {
  display: flex;
  flex-direction: column;
  gap: 0.55rem;
  margin-bottom: 0.9rem;
}

.wrapper-cta-locked {
  display: flex;
  flex-direction: column;
  gap: 0.35rem;
  padding: 0.95rem 1rem;
  border-radius: 16px;
  background: #fff7ed;
  border: 1px dashed #fdba74;
  color: #9a3412;
}

.wrapper-preview-shell {
  margin-top: 1.5rem;
  display: flex;
  flex-direction: column;
  gap: 0.8rem;
}

.wrapper-preview-shell__header {
  display: flex;
  align-items: flex-end;
  justify-content: space-between;
  gap: 1rem;
}

.wrapper-preview-shell__header h4 {
  margin: 0.2rem 0 0;
  font-size: 1rem;
  color: #0f172a;
}

.wrapper-preview-mode-pill {
  display: inline-flex;
  align-items: center;
  padding: 0.4rem 0.75rem;
  border-radius: 999px;
  background: #eff6ff;
  color: #2563eb;
  font-size: 0.72rem;
  font-weight: 800;
}

.wrapper-preview-device {
  border: 1px solid;
  border-radius: 26px;
  overflow: hidden;
  box-shadow: 0 22px 46px rgba(15, 23, 42, 0.18);
}

.wrapper-preview-hero {
  display: flex;
  flex-direction: column;
  gap: 0.85rem;
  padding: 1.15rem 1.2rem 1rem;
}

.wrapper-preview-brandline {
  display: flex;
  align-items: center;
  gap: 0.8rem;
}

.wrapper-preview-brandmark {
  display: grid;
  place-items: center;
  width: 2.7rem;
  height: 2.7rem;
  border-radius: 14px;
  background: rgba(255, 255, 255, 0.18);
  overflow: hidden;
  font-weight: 800;
}

.wrapper-preview-brandmark img {
  width: 100%;
  height: 100%;
  object-fit: cover;
}

.wrapper-preview-brandline strong,
.wrapper-preview-brandline small {
  display: block;
}

.wrapper-preview-brandline small {
  margin-top: 0.2rem;
  opacity: 0.8;
}

.wrapper-preview-proof {
  display: flex;
  flex-wrap: wrap;
  gap: 0.45rem;
}

.wrapper-preview-proof span {
  padding: 0.35rem 0.65rem;
  border-radius: 999px;
  background: rgba(255, 255, 255, 0.16);
  font-size: 0.72rem;
  font-weight: 700;
}

.wrapper-preview-body {
  display: flex;
  flex-direction: column;
  gap: 0.95rem;
  padding: 1rem 1.15rem 1.15rem;
}

.wrapper-preview-cover {
  width: 100%;
  height: 164px;
  object-fit: cover;
  border-radius: 18px;
}

.wrapper-preview-copy h5 {
  margin: 0 0 0.35rem;
  font-size: 1.15rem;
}

.wrapper-preview-copy p,
.wrapper-preview-footer {
  margin: 0;
  line-height: 1.6;
  opacity: 0.88;
}

.wrapper-preview-warning {
  padding: 0.8rem 0.9rem;
  border-radius: 14px;
  background: rgba(245, 158, 11, 0.12);
  border: 1px solid rgba(245, 158, 11, 0.26);
  color: #fcd34d;
  font-size: 0.83rem;
}

.wrapper-preview-counter {
  color: #93c5fd;
  font-size: 0.82rem;
  font-weight: 700;
}

.wrapper-preview-primary,
.wrapper-preview-secondary {
  border: 0;
  border-radius: 14px;
  padding: 0.95rem 1rem;
  font-weight: 800;
  cursor: pointer;
}

.wrapper-preview-primary {
  background: linear-gradient(135deg, #3b82f6 0%, #2563eb 100%);
  color: #fff;
  box-shadow: 0 18px 28px rgba(37, 99, 235, 0.28);
}

.wrapper-preview-offer {
  display: flex;
  flex-direction: column;
  gap: 0.55rem;
  padding: 0.95rem;
  border-radius: 16px;
  background: rgba(255, 255, 255, 0.06);
  border: 1px solid rgba(148, 163, 184, 0.22);
}

.wrapper-preview-offer__badge {
  width: fit-content;
  padding: 0.25rem 0.55rem;
  border-radius: 999px;
  background: rgba(245, 158, 11, 0.16);
  color: #fbbf24;
  font-size: 0.7rem;
  font-weight: 800;
  text-transform: uppercase;
}

.wrapper-preview-secondary {
  background: rgba(255, 255, 255, 0.12);
  color: inherit;
  border: 1px solid rgba(148, 163, 184, 0.22);
}

@media (max-width: 900px) {
  .lk-grid-responsive {
    grid-template-columns: 1fr;
  }

  .wrapper-section-header,
  .wrapper-section-header__left,
  .wrapper-section-header__right,
  .wrapper-preview-shell__header {
    align-items: flex-start;
  }

  .wrapper-section-header,
  .wrapper-preview-shell__header {
    flex-direction: column;
  }

  .wrapper-mode-summary {
    grid-template-columns: 1fr;
  }
}
</style>

