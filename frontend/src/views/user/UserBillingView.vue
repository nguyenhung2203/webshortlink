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

async function load() {
  loading.value = true
  error.value = ''

  try {
    if (!authStore.token) throw new Error('Chưa xác thực')
    plans.value = await UserService.getPlans()
    subscription.value = await UserService.getSubscription(authStore.token)
  } catch (err) {
    error.value = err instanceof Error ? err.message : 'Không thể tải billing.'
  } finally {
    loading.value = false
  }
}

async function upgrade(planId: number) {
  try {
    if (!authStore.token) throw new Error('Chưa xác thực')
    await UserService.upgradeSubscription(authStore.token, planId)
    await load()
  } catch (err) {
    error.value = err instanceof Error ? err.message : 'Không thể nâng cấp gói.'
  }
}

onMounted(load)
</script>

<template>
  <div class="flex flex-col gap-6">
    <WxPageHeader
      title="Thanh toán & Gói dịch vụ"
      subtitle="Quản lý gói dịch vụ hiện tại (Basic / Pro / Plus)."
    />

    <p v-if="error && !loading" class="text-danger text-sm">{{ error }}</p>
    
    <div v-if="loading" class="text-sm text-on-surface-variant">Đang tải gói dịch vụ...</div>

    <WxCard v-if="subscription" title="Gói hiện tại" class="p-6">
      <div class="flex flex-col gap-2">
        <p class="text-lg font-bold text-primary">{{ subscription.plan.name }}</p>
        <p class="text-sm text-on-surface-variant">Hiệu lực đến: {{ subscription.endAtUtc }}</p>
      </div>
    </WxCard>

    <div class="grid grid-cols-1 md:grid-cols-3 gap-6">
      <WxCard v-for="plan in plans" :key="plan.id" :title="plan.name" class="p-6 flex flex-col gap-4">
        <p class="text-2xl font-bold text-on-surface">{{ plan.monthlyPrice.toLocaleString('vi-VN') }}đ <span class="text-sm font-normal text-on-surface-variant">/ tháng</span></p>
        <ul class="flex flex-col gap-2 text-sm text-on-surface-variant mt-2 mb-4 list-disc list-inside">
          <li>Giới hạn Link: <strong>{{ plan.linkLimit > 0 ? plan.linkLimit : 'Không giới hạn' }}</strong></li>
          <li>Lưu trữ phân tích: <strong>{{ plan.analyticsRetentionDays }} ngày</strong></li>
        </ul>
        <WxButton variant="cta" class="mt-auto w-full" @click="upgrade(plan.id)">Đăng ký gói này</WxButton>
      </WxCard>
    </div>
  </div>
</template>
