<script setup lang="ts">
import { ref, onMounted, onUnmounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth'
import { UserService } from '@/api/services'
import WxPageHeader from '@/components/ui/WxPageHeader.vue'
import WxCard from '@/components/ui/WxCard.vue'
import WxButton from '@/components/ui/WxButton.vue'
import WxEmptyState from '@/components/ui/WxEmptyState.vue'

const route = useRoute()
const router = useRouter()
const authStore = useAuthStore()

const paymentId = route.params.id as string
const paymentDetail = ref<any>(null)
const loading = ref(true)
const error = ref('')

let pollInterval: ReturnType<typeof setInterval>

const loadDetail = async () => {
  if (!authStore.accessToken) return
  try {
    paymentDetail.value = await UserService.getPaymentDetail(authStore.accessToken, paymentId)
    // Nếu status là Paid, tự nhảy về trang Payments/Billing
    if (paymentDetail.value.status === 'Paid') {
      clearInterval(pollInterval)
      router.push('/app/payments')
    }
  } catch (err) {
    error.value = err instanceof Error ? err.message : 'Không tìm thấy hóa đơn cần thanh toán.'
    clearInterval(pollInterval)
  } finally {
    loading.value = false
  }
}

onMounted(() => {
  if (!paymentId) {
    error.value = 'Mã giao dịch không hợp lệ'
    loading.value = false
    return
  }
  loadDetail()
  // Poll mỗi 5 giây
  pollInterval = setInterval(loadDetail, 5000)
})

onUnmounted(() => {
  clearInterval(pollInterval)
})
</script>

<template>
  <div class="flex flex-col gap-6">
    <WxPageHeader
      title="Thanh Toán Dịch Vụ"
      description="Quét mã QR dưới đây để nâng cấp hoặc gia hạn gói."
    />

    <div v-if="loading" class="text-sm text-on-surface-variant">Đang tải thông tin...</div>

    <WxEmptyState
      v-else-if="error"
      title="Lỗi tải hóa đơn"
      :description="error"
    >
      <template #action>
        <WxButton variant="primary" @click="router.push('/app/billing')">Quay lại</WxButton>
      </template>
    </WxEmptyState>

    <div v-else-if="paymentDetail" class="grid sm:grid-cols-2 gap-6 items-start">
      <WxCard class="flex flex-col items-center justify-center p-6 bg-white shrink-0">
        <!-- 970422 là BIN của MBBank, STK là 0686868686, tự tạo cấu trúc QR VietQR.io -->
        <h3 class="font-semibold text-lg text-black mb-4">Quét mã VietQR bằng ứng dụng Ngân hàng</h3>
        <img
          :src="'https://img.vietqr.io/image/MB-0686868686-compact.jpg?amount=' + paymentDetail.amount + '&addInfo=WEBSHORT ' + paymentDetail.id.substring(0,8).toUpperCase() + '&accountName=NGUYEN%20HUNG'"
          class="w-full max-w-[280px] rounded object-contain border"
          alt="VietQR Mã thanh toán"
        />
        <p class="mt-4 text-sm text-center text-gray-500">Mã QR tự động nhập số tiền và nội dung.</p>
      </WxCard>

      <WxCard padding="lg" class="flex flex-col gap-4">
        <h3 class="text-lg font-semibold text-on-surface">Thông tin giao dịch</h3>
        <div class="space-y-4 text-sm">
          <div class="flex justify-between items-center pb-2 border-b border-outline-variant">
            <span class="text-on-surface-variant">Gói nâng cấp:</span>
            <span class="font-medium text-primary">{{ paymentDetail.planName }}</span>
          </div>
          <div class="flex justify-between items-center pb-2 border-b border-outline-variant">
            <span class="text-on-surface-variant">Tổng tiền:</span>
            <span class="font-bold text-success text-2xl">{{ paymentDetail.amount.toLocaleString() }} {{ paymentDetail.currency }}</span>
          </div>
          <div class="flex justify-between items-center pb-2 border-b border-outline-variant">
            <span class="text-on-surface-variant">Nội dung chuyển khoản:</span>
            <span class="font-mono bg-surface-container-high px-2 py-1 rounded">WEBSHORT {{ paymentDetail.id.substring(0,8).toUpperCase() }}</span>
          </div>
          <div class="flex justify-between items-center pb-2 border-b border-outline-variant">
            <span class="text-on-surface-variant">Trạng thái:</span>
            <span class="px-2 py-1 bg-warning/10 text-warning text-xs rounded-full font-medium flex items-center gap-2">
              <span class="w-2 h-2 rounded-full bg-warning animate-pulse"></span>
              Đang chờ thanh toán
            </span>
          </div>
        </div>

        <div class="mt-8 bg-primary/10 rounded-lg p-4 text-sm text-on-surface">
          <p>Hệ thống tự động kiểm tra mỗi 5s. Nếu Admin đã duyệt, trang này sẽ tự động chuyển hướng.</p>
        </div>
      </WxCard>
    </div>
  </div>
</template>
