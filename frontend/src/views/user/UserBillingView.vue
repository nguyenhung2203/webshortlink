<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { apiRequest } from '@/api/client'
import { useAuthStore } from '@/stores/auth'

const authStore = useAuthStore()
const plans = ref<any[]>([])
const subscription = ref<any>(null)
const error = ref('')

async function load() {
  try {
    plans.value = await apiRequest('/api/plans')
    subscription.value = await apiRequest('/api/subscription/current', { token: authStore.token })
  } catch (err) {
    error.value = err instanceof Error ? err.message : 'Không thể tải billing.'
  }
}

async function upgrade(planId: number) {
  try {
    await apiRequest('/api/subscription/upgrade', {
      method: 'POST',
      token: authStore.token,
      body: { planId },
    })
    await load()
  } catch (err) {
    error.value = err instanceof Error ? err.message : 'Không thể nâng cấp gói.'
  }
}

onMounted(load)
</script>

<template>
  <section class="stack-lg">
    <div class="page-header">
      <div>
        <h2>Billing & Plans</h2>
        <p class="muted">Quản lý gói dịch vụ Thường / Pro / Plus.</p>
      </div>
    </div>

    <p v-if="error" class="error-text">{{ error }}</p>

    <article v-if="subscription" class="panel">
      <h3>Gói hiện tại</h3>
      <p><strong>{{ subscription.plan.name }}</strong></p>
      <p class="muted">Hiệu lực đến {{ subscription.endAtUtc }}</p>
    </article>

    <div class="three-columns">
      <article v-for="plan in plans" :key="plan.id" class="panel">
        <h3>{{ plan.name }}</h3>
        <p class="price">{{ plan.monthlyPrice.toLocaleString('vi-VN') }}đ / tháng</p>
        <p class="muted">Link limit: {{ plan.linkLimit }}</p>
        <p class="muted">Analytics retention: {{ plan.analyticsRetentionDays }} ngày</p>
        <button class="primary-button" @click="upgrade(plan.id)">Chọn gói này</button>
      </article>
    </div>
  </section>
</template>
