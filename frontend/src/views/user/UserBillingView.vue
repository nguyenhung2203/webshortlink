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
  try {
    if (!authStore.accessToken) throw new Error('Chưa xác thực')
    const response = await UserService.upgradeSubscription(authStore.accessToken, planId)
    actionMessage.value = response.message
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
    <p v-if="actionMessage" class="rounded-lg border border-success/20 bg-success/10 p-3 text-sm font-medium text-success">{{ actionMessage }}</p>
    
    <div v-if="loading" class="text-sm text-on-surface-variant">Đang tải gói dịch vụ...</div>

    <WxCard v-if="subscription" title="Gói hiện tại" class="p-6">
      <div class="flex flex-col gap-2">
        <p class="text-lg font-bold text-primary">{{ subscription.planName }}</p>
        <p class="text-sm text-on-surface-variant">Trạng thái: {{ subscription.status }}</p>
        <p class="text-sm text-on-surface-variant">Hiệu lực đến: {{ new Date(subscription.endAtUtc).toLocaleDateString('vi-VN') }}</p>
      </div>
    </WxCard>

    <div class="grid grid-cols-1 md:grid-cols-3 gap-6">
      <WxCard v-for="plan in plans" :key="plan.id" :title="plan.name" class="p-6 flex flex-col gap-4">
        <p class="text-2xl font-bold text-on-surface">{{ plan.monthlyPrice.toLocaleString('vi-VN') }}đ <span class="text-sm font-normal text-on-surface-variant">/ tháng</span></p>
        <WxButton variant="cta" class="mt-auto w-full" @click="upgrade(plan.id)" :disabled="subscription?.planId === plan.id || !plan.isActive">
          {{ subscription?.planId === plan.id ? 'Gói hiện tại' : 'Đăng ký gói này' }}
        </WxButton>
      </WxCard>
    </div>
  </div>
</template>
