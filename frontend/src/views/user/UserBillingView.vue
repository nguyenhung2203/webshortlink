<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { useRouter } from 'vue-router'
import { UserService } from '@/api/services'
import { useAuthStore } from '@/stores/auth'
import type { Plan, Subscription } from '@/types/api'
import { CreditCard, Check, ShieldCheck, Zap, Diamond, Tag, CheckCircle2, History } from 'lucide-vue-next'

const authStore = useAuthStore()
const router = useRouter()
const plans = ref<Plan[]>([])
const subscription = ref<Subscription | null>(null)
const error = ref('')
const loading = ref(true)
const actionMessage = ref('')

const planFeatures: Record<string, string[]> = {
  'regular': [
    'Tạo tối đa 100 links ngẫu nhiên',
    'Phân tích Analytics giới hạn cơ bản',
    'Không hỗ trợ trỏ Tên miền riêng'
  ],
  'pro': [
    'Tạo tối đa 5,000 links tuỳ chỉnh (Custom Slug)',
    'Phân tích Analytics nâng cao (Quốc gia, Nguồn, KH)',
    'Hỗ trợ gắn tối đa 3 Tên miền độc quyền',
    'Tốc độ Redirect cực nhanh'
  ],
  'plus': [
    'Tạo tối đa 50,000 links (Gần như không giới hạn)',
    'Báo cáo Analytics phân tích đa chiều Export CSV',
    'Hỗ trợ gắn lên đến 20 Tên miền độc quyền',
    'Quyền ưu tiên xử lý Support 24/7',
    'Tùy chỉnh sâu tham số Click'
  ]
}

async function load() {
  loading.value = true
  error.value = ''
  actionMessage.value = ''

  try {
    if (!authStore.accessToken) throw new Error('Chưa xác thực')
    plans.value = await UserService.getPlans(authStore.accessToken)
    subscription.value = await UserService.getSubscription(authStore.accessToken)
  } catch (err) {
    error.value = err instanceof Error ? err.message : 'Không thể tải billing.'
  } finally {
    loading.value = false
  }
}

async function upgrade(planId: number) {
  loading.value = true
  try {
    if (!authStore.accessToken) throw new Error('Chưa xác thực')
    const response = await UserService.upgradeSubscription(authStore.accessToken, planId)
    
    if (response.amountCharged > 0) {
      router.push(`/app/checkout/${response.subscriptionId}`)
    } else {
      actionMessage.value = response.message
      await authStore.refreshSession()
      await load()
    }
  } catch (err) {
    error.value = err instanceof Error ? err.message : 'Không thể nâng cấp gói.'
    loading.value = false
  }
}

onMounted(load)
</script>

<template>
  <div class="ui-root">
    
    <!-- Header -->
    <div class="ui-header">
      <div class="ui-header-left">
        <div class="ui-eyebrow"><CreditCard :size="13" /> Thanh toán & Dịch vụ</div>
        <h1 class="ui-title">Gói cước của bạn</h1>
        <p class="ui-subtitle">Nâng cấp tài khoản để mở khóa các tính năng Analytics và Custom Domain mạnh mẽ nhất.</p>
      </div>
      <div>
        <button class="ui-btn ui-btn-outline" @click="router.push('/app/payments')">
          <History :size="16" /> Lịch sử thanh toán
        </button>
      </div>
    </div>

    <!-- Feedback -->
    <div v-if="error && !loading" class="ui-alert ui-alert-error">{{ error }}</div>
    <div v-if="actionMessage" class="ui-alert ui-alert-success"><CheckCircle2 :size="16"/> {{ actionMessage }}</div>

    <!-- Loading -->
    <div v-if="loading" class="ui-skeleton" style="height: 400px;" />

    <template v-else>
      <!-- Current Subscription -->
      <div v-if="subscription" class="ui-panel" style="border-left: 4px solid #3b82f6; background: #f8fafc;">
        <div class="ui-panel-body" style="display: flex; flex-wrap: wrap; gap: 2rem; align-items: center;">
          <div style="width: 54px; height: 54px; border-radius: 16px; background: white; border: 1px solid #e2e8f0; display: grid; place-items: center; box-shadow: 0 4px 6px -1px rgba(0,0,0,0.05);">
            <ShieldCheck v-if="subscription.planName.toLowerCase() === 'regular'" :size="28" style="color: #64748b;" />
            <Zap v-else-if="subscription.planName.toLowerCase() === 'pro'" :size="28" style="color: #3b82f6;" />
            <Diamond v-else :size="28" style="color: #f59e0b;" />
          </div>
          <div style="flex: 1; min-width: 200px;">
            <p style="font-size: 0.75rem; font-weight: 700; color: #64748b; text-transform: uppercase; margin: 0 0 0.25rem;">Trạng thái tài khoản</p>
            <div style="display: flex; align-items: baseline; gap: 0.75rem;">
              <h2 style="margin: 0; font-size: 1.8rem; font-weight: 800; color: #0f172a; text-transform: capitalize;">{{ subscription.planName }}</h2>
              <span class="ui-badge ui-badge-success">{{ subscription.status === 'Active' ? 'Đang hoạt động' : subscription.status }}</span>
            </div>
            <p style="font-size: 0.85rem; color: #64748b; margin: 0.5rem 0 0;">Bạn đang ở gói dịch vụ này. Hãy nâng cấp bên dưới nếu cần thêm tài nguyên.</p>
          </div>
        </div>
      </div>

      <!-- Pricing Cards -->
      <div>
        <h3 style="font-size: 1.25rem; font-weight: 800; color: #0f172a; margin: 2rem 0 1rem;">Nâng cấp hạng thành viên</h3>
        <div class="ui-card-grid-3" style="display: grid; gap: 1.5rem; grid-template-columns: repeat(auto-fit, minmax(300px, 1fr)); align-items: stretch;">
          
          <div 
            v-for="plan in plans" 
            :key="plan.id" 
            class="ui-panel"
            style="display: flex; flex-direction: column; transition: transform 0.2s, box-shadow 0.2s;"
            :style="subscription?.planId === plan.id ? 'border: 2px solid #3b82f6; transform: scale(1.02); box-shadow: 0 10px 25px -5px rgba(59,130,246,0.15);' : ''"
          >
            <!-- Plan Header Info -->
            <div style="padding: 2rem 1.5rem 1rem; border-bottom: 1px solid #f1f5f9; position: relative;">
              <div v-if="subscription?.planId === plan.id" style="position: absolute; top: 1rem; right: 1rem; background: #3b82f6; color: white; font-size: 0.75rem; font-weight: 700; padding: 0.25rem 0.75rem; border-radius: 999px;">
                ĐANG DÙNG
              </div>
              <h3 style="margin: 0 0 0.5rem; font-size: 1.15rem; font-weight: 700; color: #475569; text-transform: uppercase;">{{ plan.name }}</h3>
              <div style="display: flex; align-items: baseline; gap: 0.25rem;">
                <span style="font-size: 2.25rem; font-weight: 900; color: #0f172a; letter-spacing: -0.05em;">
                  {{ plan.monthlyPrice === 0 ? 'Miễn phí' : plan.monthlyPrice.toLocaleString('vi-VN') + 'đ' }}
                </span>
                <span v-if="plan.monthlyPrice > 0" style="font-size: 1rem; color: #64748b; font-weight: 500;">/tháng</span>
              </div>
              <div style="margin-top: 0.75rem; min-height: 24px;">
                <span v-if="plan.code === 'pro'" class="ui-badge" style="background: #eff6ff; color: #1d4ed8;"><Tag :size="10" style="margin-right: 0.2rem;" /> Phổ biến nhất</span>
                <span v-if="plan.code === 'plus'" class="ui-badge" style="background: #fffbeb; color: #b45309;"><Diamond :size="10" style="margin-right: 0.2rem;" /> Dành cho doanh nghiệp</span>
              </div>
            </div>

            <!-- Features -->
            <div class="ui-panel-body" style="flex: 1; padding: 1.5rem;">
              <ul style="list-style: none; padding: 0; margin: 0; display: flex; flex-direction: column; gap: 0.85rem;">
                <li v-for="(feature, idx) in planFeatures[plan.code] || []" :key="idx" style="display: flex; align-items: flex-start; gap: 0.65rem; font-size: 0.9rem; color: #334155; font-weight: 500; line-height: 1.4;">
                  <Check style="color: #3b82f6; stroke-width: 3px; flex-shrink: 0; margin-top: 0.15rem;" :size="16" />
                  <span>{{ feature }}</span>
                </li>
              </ul>
            </div>

            <!-- Action -->
            <div style="padding: 1.5rem; border-top: 1px solid #f1f5f9; background: #fafafa;">
              <button 
                class="ui-btn" 
                :class="subscription?.planId === plan.id ? 'ui-btn-outline' : (plan.code === 'pro' ? 'ui-btn-primary' : 'ui-btn-outline')"
                style="width: 100%; border-radius: 8px; justify-content: center;"
                @click="upgrade(plan.id)" 
                :disabled="subscription?.planId === plan.id || !plan.isActive"
              >
                {{ subscription?.planId === plan.id ? 'Gói hiện tại' : (plan.monthlyPrice === 0 ? 'Bắt đầu Miễn phí' : 'Nâng cấp ' + plan.name) }}
              </button>
            </div>
          </div>

        </div>
      </div>
    </template>
  </div>
</template>
