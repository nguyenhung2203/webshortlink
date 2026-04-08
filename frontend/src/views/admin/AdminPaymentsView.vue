<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useAuthStore } from '@/stores/auth'
import { AdminService } from '@/api/services'
import { CreditCard, CheckCircle, RefreshCw, AlertCircle, Search, Clock } from 'lucide-vue-next'

const authStore = useAuthStore()

const payments = ref<any[]>([])
const loading = ref(true)
const error = ref('')
const actionLoading = ref<Record<string, boolean>>({})
const search = ref('')

const loadPayments = async () => {
  loading.value = true
  error.value = ''
  try {
    if (!authStore.accessToken) return
    payments.value = await AdminService.getPayments(authStore.accessToken)
  } catch (err) {
    error.value = err instanceof Error ? err.message : 'Không thể tải danh sách thanh toán'
  } finally {
    loading.value = false
  }
}

const approve = async (paymentId: string) => {
  if (!authStore.accessToken) return
  if (!confirm('Bạn có chắc chắn muốn duyệt hóa đơn này? User sẽ được nâng cấp gói dịch vụ tương ứng ngay lập tức.')) return
  
  actionLoading.value[paymentId] = true
  try {
    await AdminService.approvePayment(authStore.accessToken, paymentId)
    await loadPayments()
  } catch (err) {
    alert(err instanceof Error ? err.message : 'Lỗi phê duyệt giao dịch')
  } finally {
    actionLoading.value[paymentId] = false
  }
}

onMounted(() => {
  loadPayments()
})

const formatDate = (dateValue: string | null) => {
  if (!dateValue) return '-'
  return new Date(dateValue).toLocaleString('vi-VN')
}
</script>

<template>
  <div class="ui-root">
    
    <div class="ui-header">
      <div class="ui-header-left">
        <div class="ui-eyebrow"><CreditCard :size="13" /> Hệ thống Tài chính</div>
        <h1 class="ui-title">Đối soát Giao dịch</h1>
        <p class="ui-subtitle">Quản lý và xét duyệt các giao dịch nâng cấp tài khoản của khách hàng.</p>
      </div>
      <div>
        <button class="ui-btn ui-btn-outline" @click="loadPayments" :disabled="loading">
          <RefreshCw :size="14" :class="{'animate-spin': loading}" /> Làm mới
        </button>
      </div>
    </div>

    <div v-if="error" class="ui-alert ui-alert-error">
      <AlertCircle :size="16" /> {{ error }}
    </div>

    <div v-if="loading" class="ui-skeleton" style="height: 400px; border-radius: 12px;" />

    <div v-else-if="payments.length === 0" class="ui-empty" style="background: white;">
      <div class="ui-empty-icon" style="opacity: 0.5;"><CreditCard :size="48" /></div>
      <h3 class="ui-empty-title">Chưa có giao dịch</h3>
      <p class="ui-empty-desc">Hệ thống chưa ghi nhận giao dịch thanh toán nào từ người dùng.</p>
    </div>

    <div v-else class="ui-panel" style="overflow: hidden; padding: 0;">
      
      <div style="overflow-x: auto;">
        <table style="width: 100%; border-collapse: collapse; min-width: 900px; text-align: left;">
          <thead style="background: #f8fafc; border-bottom: 1px solid #e2e8f0;">
            <tr>
              <th style="padding: 1rem 1.5rem; font-size: 0.75rem; font-weight: 700; color: #64748b; text-transform: uppercase;">Mã Giao dịch & Khách hàng</th>
              <th style="padding: 1rem 1.5rem; font-size: 0.75rem; font-weight: 700; color: #64748b; text-transform: uppercase;">Nội dung Nâng cấp</th>
              <th style="padding: 1rem 1.5rem; font-size: 0.75rem; font-weight: 700; color: #64748b; text-transform: uppercase; text-align: right;">Số tiền</th>
              <th style="padding: 1rem 1.5rem; font-size: 0.75rem; font-weight: 700; color: #64748b; text-transform: uppercase;">Timeline</th>
              <th style="padding: 1rem 1.5rem; font-size: 0.75rem; font-weight: 700; color: #64748b; text-transform: uppercase;">Trạng thái</th>
              <th style="padding: 1rem 1.5rem; font-size: 0.75rem; font-weight: 700; color: #64748b; text-transform: uppercase; text-align: right;">Quyết định duyệt</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="payment in payments" :key="payment.id" style="border-bottom: 1px solid #f1f5f9; transition: background 0.15s;" class="hover:bg-slate-50">
              
              <td style="padding: 1rem 1.5rem;">
                <div style="display: flex; flex-direction: column; gap: 0.1rem;">
                  <span style="font-family: monospace; font-weight: 700; font-size: 0.9rem; color: #0f172a;">WEBSHORT {{ payment.id.substring(0,8).toUpperCase() }}</span>
                  <span style="font-size: 0.8rem; color: #3b82f6; font-weight: 500;">{{ payment.userEmail }}</span>
                </div>
              </td>
              
              <td style="padding: 1rem 1.5rem;">
                <span class="ui-badge" style="background: #eff6ff; color: #1d4ed8; font-weight: 800; border: 1px solid #bfdbfe;">
                  GÓI {{ payment.planName.toUpperCase() }}
                </span>
              </td>

              <td style="padding: 1rem 1.5rem; text-align: right;">
                <span style="font-size: 1.05rem; font-weight: 800; color: #0f172a;">{{ payment.amount.toLocaleString() }}</span>
                <span style="font-size: 0.75rem; font-weight: 700; color: #64748b; margin-left: 0.2rem;">VND</span>
              </td>
              
              <td style="padding: 1rem 1.5rem;">
                <div style="display: flex; flex-direction: column; gap: 0.25rem; font-size: 0.75rem;">
                  <div style="display: flex; align-items: center; gap: 0.4rem; color: #64748b;">
                    <Clock :size="12" /> Tạo: {{ formatDate(payment.createdAtUtc) }}
                  </div>
                  <div v-if="payment.paidAtUtc" style="display: flex; align-items: center; gap: 0.4rem; color: #10b981; font-weight: 600;">
                    <CheckCircle :size="12" /> Duyệt: {{ formatDate(payment.paidAtUtc) }}
                  </div>
                </div>
              </td>
              
              <td style="padding: 1rem 1.5rem;">
                <div style="display: flex; align-items: center;">
                  <span v-if="payment.status === 'Pending'" class="ui-badge ui-badge-warning" style="font-weight: 800;"><Clock :size="12" style="margin-right: 0.2rem;" /> CHỜ DUYỆT</span>
                  <span v-else-if="payment.status === 'Paid'" class="ui-badge ui-badge-success" style="font-weight: 800;"><CheckCircle :size="12" style="margin-right: 0.2rem;" /> HOÀN TẤT</span>
                  <span v-else class="ui-badge ui-badge-error" style="font-weight: 800;">THẤT BẠI</span>
                </div>
              </td>

              <td style="padding: 1rem 1.5rem; text-align: right;">
                <button
                  v-if="payment.status === 'Pending'"
                  @click="approve(payment.id)"
                  :disabled="actionLoading[payment.id]"
                  class="ui-btn"
                  style="background: #10b981; color: white; border: none; font-size: 0.8rem; padding: 0.4rem 0.8rem; min-width: 90px; text-transform: uppercase; font-weight: 700; box-shadow: 0 2px 4px rgba(16,185,129,0.2);"
                >
                  <span v-if="actionLoading[payment.id]" style="display: flex; align-items: center; gap: 0.25rem;"><RefreshCw :size="12" class="animate-spin" /> PROCESING</span>
                  <span v-else>Xác nhận</span>
                </button>
                <span v-else style="font-size: 0.8rem; color: #cbd5e1; font-weight: 500;">
                  Locked
                </span>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  </div>
</template>
