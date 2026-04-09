<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { RouterLink, useRouter } from 'vue-router'
import { UserService } from '@/api/services'
import { useAuthStore } from '@/stores/auth'
import { CreditCard, CheckCircle, XCircle, Clock, ArrowLeft, ArrowUpRight, FileText } from 'lucide-vue-next'

const router = useRouter()
const authStore = useAuthStore()

const payments = ref<any[]>([])
const loading = ref(true)
const error = ref('')

function statusIcon(status: string) {
  if (['Paid', 'Succeeded', 'Completed'].includes(status)) return CheckCircle
  if (status === 'Failed') return XCircle
  return Clock
}

function statusBadgeClass(status: string) {
  if (['Paid', 'Succeeded', 'Completed'].includes(status)) return 'ui-badge-success'
  if (status === 'Failed') return 'ui-badge-error'
  return 'ui-badge-warning'
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
  <div class="ui-root" style="max-width: 900px; margin: 0 auto;">
    
    <!-- Back nav -->
    <div class="ui-action-bar">
      <button 
        @click="router.push('/app/billing')"
        class="ui-back-btn"
      >
        <ArrowLeft :size="14" /> Quay lại Gói cước
      </button>
    </div>

    <div v-if="error && !loading" class="ui-alert ui-alert-error">{{ error }}</div>

    <!-- Loading -->
    <div v-if="loading" class="ui-skeleton" style="height: 350px;" />

    <template v-else>
      <div v-if="payments.length === 0" class="ui-empty">
        <div class="ui-empty-icon" style="opacity: 0.4;"><CreditCard :size="48" /></div>
        <h3 class="ui-empty-title">Chưa có giao dịch nào</h3>
        <p class="ui-empty-desc">Lịch sử thanh toán sẽ xuất hiện ở đây sau khi bạn đăng ký gói Pro hoặc Plus.</p>
        <button class="ui-btn ui-btn-primary" style="margin-top: 1.5rem;" @click="router.push('/app/billing')">
          Khám phá các gói dịch vụ
        </button>
      </div>

      <div v-else style="display: flex; flex-direction: column; gap: 0.75rem;">
        
        <div 
          v-for="payment in payments" 
          :key="payment.id" 
          class="ui-panel"
        >
          <div class="ui-panel-body" style="display: flex; flex-wrap: wrap; justify-content: space-between; align-items: center; gap: 1.5rem;">
            
            <div style="display: flex; gap: 1rem; align-items: center; flex: 1; min-width: 250px;">
              <div style="width: 44px; height: 44px; border-radius: 12px; background: #f1f5f9; color: #64748b; display: grid; place-items: center; flex-shrink: 0;">
                <FileText :size="20"/>
              </div>
              <div>
                <h4 style="margin: 0 0 0.2rem; font-size: 1rem; font-weight: 700; color: #0f172a;">
                  {{ payment.description || payment.planName || 'Thanh toán dịch vụ' }}
                </h4>
                <p style="margin: 0; font-size: 0.8rem; color: #64748b; font-family: monospace;">
                  Mã GD: {{ payment.id.substring(0,8).toUpperCase() }}
                  <span style="margin: 0 0.5rem; color: #cbd5e1;">|</span>
                  {{ payment.createdAtUtc ? new Date(payment.createdAtUtc).toLocaleDateString('vi-VN', { year: 'numeric', month: 'long', day: 'numeric', hour: '2-digit', minute: '2-digit' }) : '—' }}
                </p>
              </div>
            </div>

            <div style="display: flex; align-items: center; gap: 1.5rem; justify-content: flex-end;">
              <span class="ui-badge" :class="statusBadgeClass(payment.status)" style="display: flex; align-items: center; gap: 0.2rem;">
                <component :is="statusIcon(payment.status)" :size="12" />
                {{ payment.status }}
              </span>
              
              <div style="text-align: right; min-width: 120px;">
                <p style="margin: 0; font-size: 1.15rem; font-weight: 800; color: #0f172a;">{{ formatAmount(payment.amount, payment.currency) }}</p>
                <a 
                  v-if="payment.invoiceUrl && !isPending(payment.status)" 
                  :href="payment.invoiceUrl" 
                  target="_blank" 
                  style="display: inline-flex; align-items: center; gap: 0.1rem; font-size: 0.75rem; font-weight: 600; color: #3b82f6; text-decoration: none; margin-top: 0.25rem;"
                >
                  Tải hóa đơn <ArrowUpRight :size="12" />
                </a>
                <RouterLink 
                  v-else-if="isPending(payment.status)"
                  :to="`/app/checkout/${payment.id}`"
                  style="display: inline-flex; align-items: center; gap: 0.1rem; font-size: 0.75rem; font-weight: 700; color: #f59e0b; text-decoration: none; margin-top: 0.25rem;"
                >
                  Hoàn tất thanh toán →
                </RouterLink>
              </div>
            </div>

          </div>
        </div>

      </div>
    </template>
  </div>
</template>
