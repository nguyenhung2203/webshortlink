<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { RouterLink } from 'vue-router'
import { UserService } from '@/api/services'
import { useAuthStore } from '@/stores/auth'
import { CreditCard, CheckCircle, XCircle, Clock } from 'lucide-vue-next'
import WxPageHeader from '@/components/ui/WxPageHeader.vue'

const authStore = useAuthStore()
const payments = ref<any[]>([])
const loading = ref(true)
const error = ref('')

function statusIcon(status: string) {
  if (status === 'Paid' || status === 'Succeeded' || status === 'Completed') return CheckCircle
  if (status === 'Failed') return XCircle
  return Clock
}

function statusClass(status: string) {
  if (status === 'Paid' || status === 'Succeeded' || status === 'Completed') return 'text-success'
  if (status === 'Failed') return 'text-danger'
  return 'text-warning'
}

function isPending(status: string) {
  return status === 'Pending'
}

function formatAmount(amount: number, currency: string) {
  try {
    return new Intl.NumberFormat('vi-VN', { style: 'currency', currency: currency || 'VND' }).format(amount)
  } catch {
    return `${amount?.toLocaleString('vi-VN')} ${currency}`
  }
}

async function load() {
  loading.value = true
  try {
    if (!authStore.accessToken) throw new Error('Chưa xác thực.')
    payments.value = await UserService.getPayments(authStore.accessToken)
  } catch (err) {
    error.value = err instanceof Error ? err.message : 'Không thể tải lịch sử thanh toán.'
  } finally {
    loading.value = false
  }
}

onMounted(load)
</script>

<template>
  <div class="flex flex-col gap-6">
    <WxPageHeader
      title="Lịch sử thanh toán"
      description="Xem lại tất cả các giao dịch và hóa đơn của bạn."
    />

    <div v-if="loading" class="text-center py-10 text-on-surface-variant">Đang tải lịch sử...</div>
    <div v-else-if="error" class="bg-danger/10 text-danger rounded-xl p-4 text-sm">{{ error }}</div>

    <div v-else-if="payments.length === 0" class="bg-surface-container rounded-2xl border border-outline p-10 text-center">
      <CreditCard :size="40" class="text-on-surface-variant mx-auto mb-3 opacity-40" />
      <p class="font-semibold text-on-surface mb-1">Chưa có giao dịch nào</p>
      <p class="text-sm text-on-surface-variant">Lịch sử thanh toán sẽ xuất hiện ở đây sau khi bạn nâng cấp gói.</p>
    </div>

    <div v-else class="flex flex-col gap-3">
      <div
        v-for="payment in payments"
        :key="payment.id"
        class="bg-surface-container rounded-2xl border border-outline p-5 flex flex-col sm:flex-row sm:items-center gap-4"
      >
        <!-- Icon -->
        <div class="w-10 h-10 rounded-full bg-primary/10 flex items-center justify-center shrink-0">
          <CreditCard :size="18" class="text-primary" />
        </div>

        <!-- Info -->
        <div class="flex-1 min-w-0">
          <div class="flex items-center gap-2 mb-0.5">
            <p class="font-semibold text-on-surface text-sm">{{ payment.description || payment.planName || 'Thanh toán dịch vụ' }}</p>
            <span class="text-xs font-bold px-2 py-0.5 rounded-full" :class="{
              'bg-success/10 text-success': payment.status === 'Succeeded' || payment.status === 'Completed',
              'bg-danger/10 text-danger': payment.status === 'Failed',
              'bg-warning/10 text-warning': !['Succeeded','Completed','Failed'].includes(payment.status),
            }">
              <component :is="statusIcon(payment.status)" :size="10" class="inline mr-0.5" />
              {{ payment.status }}
            </span>
          </div>
          <p class="text-xs text-on-surface-variant">
            {{ payment.createdAtUtc ? new Date(payment.createdAtUtc).toLocaleDateString('vi-VN', { year: 'numeric', month: 'long', day: 'numeric' }) : '-' }}
          </p>
        </div>

        <!-- Amount -->
        <div class="text-right shrink-0">
          <p class="font-bold text-on-surface">{{ formatAmount(payment.amount, payment.currency) }}</p>
          <p v-if="payment.invoiceUrl" class="text-xs">
            <a :href="payment.invoiceUrl" target="_blank" class="text-primary hover:underline">Xem hóa đơn</a>
          </p>
          <RouterLink
            v-if="isPending(payment.status)"
            :to="'/app/checkout/' + payment.id"
            class="mt-1 inline-block text-xs font-medium text-warning hover:underline"
          >
            Tiếp tục thanh toán →
          </RouterLink>
        </div>
      </div>
    </div>
  </div>
</template>
