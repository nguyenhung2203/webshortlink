<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useAuthStore } from '@/stores/auth'
import { AdminService } from '@/api/services'
import WxPageHeader from '@/components/ui/WxPageHeader.vue'
import WxCard from '@/components/ui/WxCard.vue'
import WxButton from '@/components/ui/WxButton.vue'
import WxEmptyState from '@/components/ui/WxEmptyState.vue'

const authStore = useAuthStore()

const payments = ref<any[]>([])
const loading = ref(true)
const error = ref('')
const actionLoading = ref<Record<string, boolean>>({})

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
  if (!confirm('Bạn có chắc chắn muốn duyệt hóa đơn này? User sẽ được kích hoạt gói tính phí ngay lập tức.')) return
  
  actionLoading.value[paymentId] = true
  try {
    await AdminService.approvePayment(authStore.accessToken, paymentId)
    await loadPayments()
  } catch (err) {
    alert(err instanceof Error ? err.message : 'Lỗi duyệt')
  } finally {
    actionLoading.value[paymentId] = false
  }
}

onMounted(() => {
  loadPayments()
})

const getStatusClass = (status: string) => {
  switch (status) {
    case 'Pending': return 'bg-warning/10 text-warning'
    case 'Paid': return 'bg-success/10 text-success'
    case 'Failed': return 'bg-danger/10 text-danger'
    default: return 'bg-gray-100 text-gray-600'
  }
}

const formatDate = (dateValue: string | null) => {
  if (!dateValue) return '-'
  return new Date(dateValue).toLocaleString('vi-VN')
}
</script>

<template>
  <div class="flex flex-col gap-6">
    <WxPageHeader
      title="Duyệt Giao Dịch (Payments)"
      description="Quản lý và xét duyệt các giao dịch Nâng cấp gói qua VietQR. Giao dịch mới nhất hiển thị trên cùng."
    />

    <WxCard padding="none" class="overflow-hidden">
      <div v-if="loading" class="p-8 flex justify-center">
        <div class="animate-spin rounded-full h-8 w-8 border-b-2 border-primary"></div>
      </div>
      <div v-else-if="error" class="p-8 text-center text-danger">{{ error }}</div>
      <WxEmptyState
        v-else-if="payments.length === 0"
        title="Chưa có giao dịch"
        description="Hiện tại không có thông tin giao dịch thanh toán nào."
        icon="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z"
      />
      
      <div v-else class="overflow-x-auto">
        <table class="w-full text-left border-collapse">
          <thead>
            <tr class="bg-gray-50/50 border-b border-gray-100 text-xs text-on-surface-variant font-medium tracking-wider uppercase">
              <th class="p-4 pl-6">Khách hàng</th>
              <th class="p-4">Gói nâng cấp</th>
              <th class="p-4">Chi tiết</th>
              <th class="p-4">Ngày tạo / Ngày duyệt</th>
              <th class="p-4">Trạng thái</th>
              <th class="p-4 pr-6 text-right">Thao tác</th>
            </tr>
          </thead>
          <tbody class="divide-y divide-gray-100 text-sm">
            <tr v-for="payment in payments" :key="payment.id" class="hover:bg-gray-50/30 transition-colors bg-white">
              <td class="p-4 pl-6">
                <span class="font-medium text-gray-900 block truncate max-w-[200px]" :title="payment.userEmail">{{ payment.userEmail }}</span>
              </td>
              <td class="p-4">
                <span class="inline-block px-2 py-0.5 bg-primary/5 text-primary rounded text-xs font-semibold uppercase">{{ payment.planName }}</span>
              </td>
              <td class="p-4 font-mono text-gray-600">
                <div class="font-bold text-gray-900 font-sans mb-1">{{ payment.amount.toLocaleString() }} VND</div>
                Mã: WEBSHORT {{ payment.id.substring(0,8).toUpperCase() }}
              </td>
              <td class="p-4 whitespace-nowrap text-gray-500">
                <div class="mb-1">Tạo: {{ formatDate(payment.createdAtUtc) }}</div>
                <div v-if="payment.paidAtUtc" class="text-success">Duyệt: {{ formatDate(payment.paidAtUtc) }}</div>
              </td>
              <td class="p-4 whitespace-nowrap">
                <span 
                  class="px-2.5 py-1 text-xs font-semibold rounded-full"
                  :class="getStatusClass(payment.status)"
                >
                  {{ payment.status }}
                </span>
              </td>
              <td class="p-4 pr-6 text-right">
                <WxButton 
                  v-if="payment.status === 'Pending'" 
                  variant="primary" 
                  size="sm"
                  :disabled="actionLoading[payment.id]"
                  @click="approve(payment.id)"
                >
                  {{ actionLoading[payment.id] ? 'Đang duyệt...' : 'Duyệt (Approve)' }}
                </WxButton>
                <div v-else class="text-xs text-gray-400 font-medium">Không thể thao tác</div>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </WxCard>
  </div>
</template>
