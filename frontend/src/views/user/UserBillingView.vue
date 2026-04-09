<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { useRouter } from 'vue-router'
import { UserService } from '@/api/services'
import { useAuthStore } from '@/stores/auth'
import type { Plan, Subscription } from '@/types/api'
import { Check, Zap, ArrowRight, History, Crown, CheckCircle2, AlertCircle } from 'lucide-vue-next'

const authStore = useAuthStore()
const router = useRouter()
const plans = ref<Plan[]>([])
const subscription = ref<Subscription | null>(null)
const error = ref('')
const loading = ref(true)
const actionMessage = ref('')

const planDetails: Record<string, { desc: string }> = {
  regular: {
    desc: 'Bắt đầu hành trình của bạn với các công cụ cơ bản hoàn toàn miễn phí.',
  },
  pro: {
    desc: 'Sức mạnh tùy chỉnh tối đa dành cho marketer và creator chuyên nghiệp.',
  },
  plus: {
    desc: 'Giải pháp gần như không giới hạn với dữ liệu phân tích chuyên sâu cho doanh nghiệp.',
  },
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
    error.value = err instanceof Error ? err.message : 'Dịch vụ nâng cấp tạm thời không phản hồi.'
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
    error.value = err instanceof Error ? err.message : 'Khởi tạo thanh toán thất bại.'
    loading.value = false
  }
}

onMounted(load)
</script>

<template>
  <div class="pricing-container">
    <div class="pricing-hero">
      <div class="pricing-hero-badge">NÂNG CẤP DỊCH VỤ</div>
      <h1 class="pricing-hero-title">Chọn Gói Phù Hợp Với Bạn</h1>
      <p class="pricing-hero-subtitle">
        Mở khóa tên miền riêng, phân tích chuyên sâu và nhiều quyền lợi khác. Nâng cấp ngay hôm nay để đẩy nhanh hiệu suất chiến dịch.
      </p>

      <div class="pricing-actions">
        <button class="pricing-history-btn" @click="router.push('/app/payments')">
          <History :size="16" /> Lịch sử thanh toán
        </button>
      </div>
    </div>

    <div v-if="error && !loading" class="pricing-alert error">
      <AlertCircle :size="18" /> {{ error }}
    </div>
    <div v-if="actionMessage" class="pricing-alert success">
      <CheckCircle2 :size="18" /> {{ actionMessage }}
    </div>

    <div v-if="subscription && !loading" class="active-plan-banner">
      <div class="banner-content">
        <div class="banner-icon">
          <CheckCircle2 :size="24" stroke-width="2.5" />
        </div>
        <div class="banner-text">
          <span class="banner-label">TRẠNG THÁI TÀI KHOẢN HIỆN TẠI</span>
          <h2 class="banner-plan-name">
            Gói {{ subscription.planName.toUpperCase() }}
            <span class="status-pill">{{ subscription.status === 'Active' ? 'ĐANG HOẠT ĐỘNG' : subscription.status.toUpperCase() }}</span>
          </h2>
        </div>
      </div>
      <div class="banner-end">
        <span class="banner-cycle">Chu kỳ gia hạn: <strong>30 ngày</strong></span>
      </div>
    </div>

    <div v-if="loading" class="pricing-grid loading-state">
      <div v-for="i in 3" :key="i" class="skeleton-card"></div>
    </div>

    <div v-else class="pricing-grid">
      <div
        v-for="plan in plans"
        :key="plan.id"
        :class="['pricing-card', `plan-${plan.code.toLowerCase()}`, { 'is-current': subscription?.planId === plan.id }]"
      >
        <div v-if="plan.code === 'pro'" class="popular-label">
          <Zap :size="14" fill="currentColor" /> ĐƯỢC ƯA CHUỘNG NHẤT
        </div>
        <div v-if="plan.code === 'plus'" class="enterprise-label">
          <Crown :size="14" /> DÀNH CHO DOANH NGHIỆP
        </div>

        <div class="card-header">
          <h3 class="plan-name">{{ plan.name }}</h3>
          <p class="plan-desc">{{ planDetails[plan.code]?.desc || 'Mở khóa thêm tính năng phù hợp với nhu cầu của bạn.' }}</p>

          <div class="plan-price-box">
            <span class="price-val">{{ plan.monthlyPrice === 0 ? 'Miễn phí' : (plan.monthlyPrice / 1000).toLocaleString() + 'k' }}</span>
            <span v-if="plan.monthlyPrice > 0" class="price-cycle">/tháng</span>
            <span v-if="plan.monthlyPrice > 0" class="price-currency">VND</span>
          </div>
        </div>

        <div class="card-features">
          <div class="feature-title">Bao gồm các quyền lợi:</div>
          <ul class="feature-list">
            <li v-for="(feat, idx) in plan.features" :key="idx" class="feature-item">
              <Check :size="18" class="check-icon" stroke-width="3" />
              <div class="feat-text">
                <span class="feat-main">{{ feat.title }}</span>
                <span v-if="feat.hint" class="feat-hint">{{ feat.hint }}</span>
              </div>
            </li>
          </ul>
        </div>

        <div class="card-action">
          <button
            class="cta-btn"
            :disabled="subscription?.planId === plan.id || !plan.isActive || (subscription?.planId !== 1 && subscription?.planId !== plan.id)"
            @click="upgrade(plan.id)"
          >
            <span v-if="subscription?.planId === plan.id">Gói hiện tại của bạn</span>
            <span v-else-if="!plan.isActive">Tạm khóa nâng cấp</span>
            <span v-else-if="subscription?.planId !== 1 && subscription?.planId !== plan.id">Đang sử dụng gói khác</span>
            <span v-else-if="plan.monthlyPrice === 0">Bắt đầu miễn phí</span>
            <span v-else class="flex items-center justify-center gap-2">
              Nâng cấp lên {{ plan.name }} <ArrowRight :size="16" />
            </span>
          </button>
        </div>
      </div>
    </div>

    <div v-if="!loading" class="pricing-footer">
      <p>Thanh toán an toàn qua cổng chuyển khoản quốc gia (VietQR). Gói sẽ được kích hoạt ngay sau khi thanh toán được xác nhận.</p>
    </div>
  </div>
</template>

<style scoped>
.pricing-container {
  max-width: 1000px;
  margin: 0 auto;
  padding: 1.5rem;
}

.pricing-hero {
  text-align: center;
  margin-bottom: 2.5rem;
}

.pricing-hero-badge {
  display: inline-block;
  padding: 0.25rem 0.75rem;
  background: #e0e7ff;
  color: #4f46e5;
  font-weight: 800;
  font-size: 0.7rem;
  letter-spacing: 1px;
  border-radius: 999px;
  margin-bottom: 1rem;
  text-transform: uppercase;
}

.pricing-hero-title {
  font-size: 2rem;
  font-weight: 900;
  color: #0f172a;
  letter-spacing: -0.02em;
  margin: 0 0 0.75rem;
  line-height: 1.2;
}

.pricing-hero-subtitle {
  font-size: 0.95rem;
  color: #64748b;
  max-width: 600px;
  margin: 0 auto;
  line-height: 1.5;
}

.pricing-actions {
  margin-top: 1.5rem;
}

.pricing-history-btn {
  display: inline-flex;
  align-items: center;
  gap: 0.5rem;
  background: white;
  border: 1px solid #cbd5e1;
  color: #475569;
  font-weight: 600;
  font-size: 0.85rem;
  padding: 0.4rem 0.85rem;
  border-radius: 8px;
  cursor: pointer;
  transition: all 0.2s;
  box-shadow: 0 1px 2px rgba(0, 0, 0, 0.05);
}

.pricing-history-btn:hover {
  background: #f8fafc;
  border-color: #94a3b8;
  color: #0f172a;
}

.pricing-alert {
  max-width: 900px;
  margin: 0 auto 1.5rem;
  padding: 0.75rem 1rem;
  border-radius: 10px;
  display: flex;
  align-items: center;
  gap: 0.5rem;
  font-weight: 600;
  font-size: 0.85rem;
}

.pricing-alert.error {
  background: #fef2f2;
  color: #ef4444;
  border: 1px solid #fecaca;
}

.pricing-alert.success {
  background: #ecfdf5;
  color: #10b981;
  border: 1px solid #a7f3d0;
}

.active-plan-banner {
  max-width: 900px;
  margin: 0 auto 2rem;
  background: #f8fafc;
  border: 1px solid #e2e8f0;
  border-radius: 16px;
  padding: 1rem 1.5rem;
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 1.5rem;
  flex-wrap: wrap;
  box-shadow: 0 2px 4px -1px rgba(0, 0, 0, 0.05);
}

.banner-content {
  display: flex;
  align-items: center;
  gap: 1rem;
}

.banner-icon {
  width: 44px;
  height: 44px;
  background: #10b981;
  color: white;
  border-radius: 12px;
  display: grid;
  place-items: center;
  box-shadow: 0 4px 10px rgba(16, 185, 129, 0.3);
}

.banner-label {
  font-size: 0.65rem;
  font-weight: 800;
  color: #64748b;
  letter-spacing: 0.5px;
  display: block;
  margin-bottom: 0.15rem;
}

.banner-plan-name {
  font-size: 1.15rem;
  font-weight: 900;
  color: #0f172a;
  margin: 0;
  display: flex;
  align-items: center;
  gap: 0.5rem;
  flex-wrap: wrap;
}

.status-pill {
  font-size: 0.65rem;
  font-weight: 800;
  padding: 0.15rem 0.5rem;
  border-radius: 4px;
  background: #dcfce7;
  color: #15803d;
  border: 1px solid #bbf7d0;
}

.banner-cycle {
  font-size: 0.85rem;
  color: #475569;
}

.banner-cycle strong {
  color: #0f172a;
}

.pricing-grid {
  display: grid;
  grid-template-columns: repeat(3, 1fr);
  gap: 1.5rem;
  max-width: 900px;
  margin: 0 auto;
  align-items: stretch;
}

.loading-state {
  gap: 1.5rem;
}

.skeleton-card {
  background: #f1f5f9;
  height: 400px;
  border-radius: 16px;
  animation: pulse 2s infinite;
}

@keyframes pulse {
  0%,
  100% {
    opacity: 1;
  }

  50% {
    opacity: 0.5;
  }
}

.pricing-card {
  position: relative;
  background: white;
  border-radius: 16px;
  border: 1px solid #e2e8f0;
  display: flex;
  flex-direction: column;
  padding: 1.5rem;
  transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
}

.pricing-card.is-current {
  box-shadow: inset 0 0 0 2px #10b981;
}

.pricing-card.plan-pro {
  background: #0f172a;
  border: none;
  color: white;
  transform: scale(1.03);
  box-shadow: 0 15px 30px -10px rgba(15, 23, 42, 0.5);
  z-index: 10;
}

.plan-pro .plan-name {
  color: white;
}

.plan-pro .plan-desc {
  color: #94a3b8;
  border-color: #1e293b;
}

.plan-pro .price-val {
  color: white;
}

.plan-pro .price-cycle,
.plan-pro .price-currency {
  color: #94a3b8;
}

.plan-pro .feature-title {
  color: #f8fafc;
}

.plan-pro .feat-main {
  color: #f1f5f9;
}

.plan-pro .feat-hint {
  color: #64748b;
}

.plan-pro .check-icon {
  color: #38bdf8;
}

.popular-label {
  position: absolute;
  top: -12px;
  left: 50%;
  transform: translateX(-50%);
  background: linear-gradient(135deg, #3b82f6, #2563eb);
  color: white;
  padding: 0.25rem 0.85rem;
  border-radius: 999px;
  font-size: 0.65rem;
  font-weight: 800;
  letter-spacing: 0.5px;
  box-shadow: 0 2px 4px rgba(37, 99, 235, 0.4);
  display: flex;
  align-items: center;
  gap: 0.3rem;
  white-space: nowrap;
}

.enterprise-label {
  position: absolute;
  top: 1rem;
  right: 1rem;
  background: #fffbeb;
  color: #b45309;
  padding: 0.2rem 0.5rem;
  border-radius: 4px;
  font-size: 0.65rem;
  font-weight: 800;
  border: 1px solid #fde68a;
  display: flex;
  align-items: center;
  gap: 0.25rem;
}

.card-header {
  margin-bottom: 1.25rem;
}

.plan-name {
  font-size: 1.15rem;
  font-weight: 900;
  color: #0f172a;
  margin: 0 0 0.35rem;
  text-transform: uppercase;
  letter-spacing: 0.5px;
}

.plan-desc {
  font-size: 0.8rem;
  color: #64748b;
  margin: 0;
  line-height: 1.4;
  padding-bottom: 1rem;
  border-bottom: 1px solid #f1f5f9;
}

.plan-price-box {
  margin-top: 1rem;
  display: flex;
  align-items: flex-end;
  gap: 0.2rem;
}

.price-val {
  font-size: 2.25rem;
  font-weight: 900;
  color: #0f172a;
  line-height: 1;
  letter-spacing: -0.03em;
}

.price-cycle {
  font-size: 0.85rem;
  font-weight: 600;
  color: #64748b;
  margin-bottom: 0.25rem;
}

.price-currency {
  font-size: 0.75rem;
  font-weight: 700;
  color: #94a3b8;
  margin-bottom: 0.25rem;
  margin-left: 0.15rem;
}

.card-features {
  flex: 1;
  margin-bottom: 1.5rem;
}

.feature-title {
  font-size: 0.75rem;
  font-weight: 800;
  color: #0f172a;
  text-transform: uppercase;
  letter-spacing: 0.5px;
  margin-bottom: 0.75rem;
}

.feature-list {
  list-style: none;
  padding: 0;
  margin: 0;
  display: flex;
  flex-direction: column;
  gap: 0.65rem;
}

.feature-item {
  display: flex;
  align-items: flex-start;
  gap: 0.5rem;
}

.check-icon {
  flex-shrink: 0;
  color: #10b981;
  margin-top: 0.1rem;
}

.feat-text {
  display: flex;
  flex-direction: column;
}

.feat-main {
  font-size: 0.85rem;
  font-weight: 600;
  color: #1e293b;
  line-height: 1.3;
}

.feat-hint {
  font-size: 0.7rem;
  color: #94a3b8;
  margin-top: 0.1rem;
}

.cta-btn {
  width: 100%;
  padding: 0.75rem 1rem;
  border-radius: 8px;
  font-size: 0.9rem;
  font-weight: 800;
  border: none;
  cursor: pointer;
  transition: all 0.2s;
  background: #f1f5f9;
  color: #0f172a;
}

.cta-btn:hover:not(:disabled) {
  background: #e2e8f0;
}

.pricing-card.is-current .cta-btn {
  background: #ecfdf5 !important;
  color: #10b981 !important;
  cursor: not-allowed;
  border: 1px solid #a7f3d0;
}

.plan-pro .cta-btn {
  background: #3b82f6;
  color: white;
  box-shadow: 0 2px 8px rgba(59, 130, 246, 0.3);
}

.plan-pro .cta-btn:hover:not(:disabled) {
  background: #2563eb;
  transform: translateY(-2px);
}

.pricing-footer {
  text-align: center;
  margin-top: 2rem;
  padding-top: 1.5rem;
  border-top: 1px solid #e2e8f0;
  font-size: 0.8rem;
  color: #64748b;
  font-weight: 500;
}

@media (max-width: 1024px) {
  .pricing-grid {
    grid-template-columns: repeat(2, 1fr);
    gap: 1.5rem;
  }

  .pricing-card.plan-pro {
    transform: none;
  }
}

@media (max-width: 768px) {
  .pricing-hero-title {
    font-size: 1.75rem;
  }

  .pricing-grid {
    grid-template-columns: 1fr;
  }

  .pricing-card {
    padding: 1.5rem;
  }

  .active-plan-banner {
    flex-direction: column;
    align-items: flex-start;
    gap: 1rem;
  }
}
</style>
