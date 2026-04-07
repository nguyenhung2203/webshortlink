<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { UserService } from '@/api/services'
import { useAuthStore } from '@/stores/auth'
import type { Plan, Subscription } from '@/types/api'
import WxPageHeader from '@/components/ui/WxPageHeader.vue'
import WxCard from '@/components/ui/WxCard.vue'
import WxButton from '@/components/ui/WxButton.vue'

const authStore = useAuthStore()
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
    actionMessage.value = response.message
    await authStore.refreshSession()
    await load()
  } catch (err) {
    error.value = err instanceof Error ? err.message : 'Không thể nâng cấp gói.'
    loading.value = false
  }
}

onMounted(load)
</script>

<template>
  <div class="flex flex-col gap-6">
    <WxPageHeader
      title="Tiện ích & Gói dịch vụ (Billing)"
      subtitle="Quản lý và Nâng cấp Hạng thành viên để tối đa hoá tiềm năng đường link của bạn."
    />

    <p v-if="error && !loading" class="text-danger text-sm">{{ error }}</p>
    <p v-if="actionMessage" class="rounded-lg border border-success/20 bg-success/10 p-3 text-sm font-medium text-success">{{ actionMessage }}</p>
    
    <div v-if="loading" class="flex justify-center p-12">
      <div class="animate-spin rounded-full h-8 w-8 border-b-2 border-primary"></div>
    </div>

    <!-- Active Subscription Summary -->
    <WxCard v-if="subscription" title="Tình trạng Tài khoản" class="p-6 border-l-4 border-l-primary bg-primary/5">
      <div class="flex flex-col sm:flex-row gap-8">
        <div>
          <p class="text-sm font-medium text-on-surface-variant uppercase tracking-wider mb-1">Gói hiện tại</p>
          <p class="text-2xl font-black text-primary uppercase">{{ subscription.planName }}</p>
        </div>
        <div>
          <p class="text-sm font-medium text-on-surface-variant uppercase tracking-wider mb-1">Trạng thái</p>
          <p class="text-lg font-bold text-success">{{ subscription.status === 'Active' ? 'Đang hoạt động' : subscription.status }}</p>
        </div>
        <div class="flex items-end">
          <div class="sm:ml-auto">
            <RouterLink to="/app/payments" class="text-sm text-primary font-semibold hover:underline">
              Xem lịch sử thanh toán →
            </RouterLink>
          </div>
        </div>
      </div>
    </WxCard>

    <div class="grid grid-cols-1 md:grid-cols-3 gap-6 lg:gap-8 mt-4" v-if="!loading">
      <WxCard 
        v-for="plan in plans" 
        :key="plan.id" 
        :title="plan.name" 
        class="transition-all duration-300 flex flex-col pt-6 px-6 pb-8 border-2"
        :class="subscription?.planId === plan.id ? 'border-primary ring-4 ring-primary/10 scale-[1.02] bg-white' : 'border-gray-100 hover:border-gray-300 hover:-translate-y-1 bg-gray-50/50'"
      >
        <div class="flex-col pb-4 border-b border-gray-100 mb-6">
          <p class="text-4xl font-black text-gray-900 tracking-tight">{{ plan.monthlyPrice === 0 ? 'Miễn phí' : plan.monthlyPrice.toLocaleString('vi-VN') + 'đ' }}<span v-if="plan.monthlyPrice > 0" class="text-sm font-medium text-gray-500 ml-1 tracking-normal">/tháng</span></p>
          <span v-if="plan.code === 'pro'" class="inline-block mt-3 text-xs font-bold bg-primary/10 text-primary px-3 py-1 rounded-full">Phổ biến nhất</span>
          <span v-if="plan.code === 'plus'" class="inline-block mt-3 text-xs font-bold bg-amber-500/10 text-amber-600 px-3 py-1 rounded-full">Cao cấp nhất</span>
        </div>
        
        <ul class="flex flex-col gap-4 mb-8 flex-1">
          <li v-for="(feature, idx) in planFeatures[plan.code] || []" :key="idx" class="flex items-start gap-3 text-sm text-gray-700 font-medium leading-tight">
             <!-- Icon Check -->
            <svg class="w-5 h-5 text-primary shrink-0 drop-shadow-sm" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="3" stroke-linecap="round" stroke-linejoin="round"><path d="M20 6 9 17l-5-5"/></svg>
            <span>{{ feature }}</span>
          </li>
        </ul>

        <WxButton 
          :variant="subscription?.planId === plan.id ? 'secondary' : (plan.code === 'pro' ? 'primary' : 'cta')" 
          class="mt-auto w-full py-3.5 font-bold text-base shadow-sm hover:shadow-md" 
          @click="upgrade(plan.id)" 
          :disabled="subscription?.planId === plan.id || !plan.isActive"
        >
          {{ subscription?.planId === plan.id ? 'Tài khoản gốc' : 'Bắt đầu với ' + plan.name }}
        </WxButton>
      </WxCard>
    </div>
  </div>
</template>
