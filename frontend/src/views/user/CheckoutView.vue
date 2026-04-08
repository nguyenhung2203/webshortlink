<script setup lang="ts">
import { ref, onMounted, onUnmounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth'
import { UserService } from '@/api/services'
import { Banknote, ShieldCheck, CreditCard, ArrowLeft, Loader2, Info } from 'lucide-vue-next'

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
  pollInterval = setInterval(loadDetail, 5000)
})

onUnmounted(() => {
  clearInterval(pollInterval)
})
</script>

<template>
  <div class="ui-root" style="max-width: 960px; margin: 0 auto;">
    
    <!-- Header -->
    <div class="ui-header">
      <div class="ui-header-left">
        <button 
          @click="router.push('/app/billing')"
          style="display: inline-flex; align-items: center; gap: 0.25rem; font-size: 0.85rem; font-weight: 600; color: #64748b; background: transparent; border: 0; padding: 0 0 0.5rem 0; cursor: pointer;"
        >
          <ArrowLeft :size="14" /> Chọn gói khác
        </button>
        <h1 class="ui-title">Thanh toán Dịch vụ</h1>
        <p class="ui-subtitle">Sử dụng tính năng quét mã QR của ứng dụng ngân hàng để hoàn tất gia hạn.</p>
      </div>
    </div>

    <!-- Error State -->
    <div v-if="error && !loading" class="ui-empty">
      <div class="ui-empty-icon" style="color: #ef4444;"><Info :size="48" /></div>
      <h3 class="ui-empty-title">Lỗi tải hóa đơn</h3>
      <p class="ui-empty-desc">{{ error }}</p>
      <button class="ui-btn ui-btn-primary" style="margin-top: 1.5rem;" @click="router.push('/app/billing')">Quay Lại Gói Cước</button>
    </div>

    <!-- Loading -->
    <div v-else-if="loading" class="ui-card-grid-2">
      <div class="ui-skeleton" style="height: 450px;" />
      <div class="ui-skeleton" style="height: 450px;" />
    </div>

    <!-- Content -->
    <div v-else-if="paymentDetail" class="ui-card-grid ui-card-grid-2" style="align-items: start;">
      
      <!-- QR Code Panel -->
      <div class="ui-panel" style="display: flex; flex-direction: column; align-items: center; text-align: center; padding: 2rem 1.5rem;">
        <div style="margin-bottom: 1rem; display: flex; flex-direction: column; align-items: center; gap: 0.5rem;">
          <div style="width: 48px; height: 48px; background: #eff6ff; color: #3b82f6; border-radius: 50%; display: grid; place-items: center;">
            <Banknote :size="24" />
          </div>
          <h3 style="margin: 0; font-size: 1.1rem; font-weight: 700; color: #0f172a;">Quét mã VietQR</h3>
          <p style="margin: 0; font-size: 0.85rem; color: #64748b; max-width: 250px;">Mở ứng dụng Ngân hàng của bạn, chọn quét QR và hướng camera vào mã bên dưới.</p>
        </div>
        
        <div style="background: white; padding: 1rem; border-radius: 16px; box-shadow: 0 4px 20px rgba(0,0,0,0.08); border: 1px solid #e2e8f0; display: inline-block;">
          <img
            :src="'https://img.vietqr.io/image/MB-0686868686-compact.jpg?amount=' + paymentDetail.amount + '&addInfo=WEBSHORT ' + paymentDetail.id.substring(0,8).toUpperCase() + '&accountName=NGUYEN%20HUNG'"
            style="width: 240px; height: 240px; border-radius: 8px; object-fit: contain; mix-blend-mode: multiply;"
            alt="VietQR Mã thanh toán"
          />
        </div>

        <p style="margin: 1.5rem 0 0; font-size: 0.8rem; color: #94a3b8; display: flex; align-items: center; gap: 0.5rem; justify-content: center;">
          <ShieldCheck :size="14" /> Giao dịch được mã hóa đầu cuối an toàn.
        </p>
      </div>

      <!-- Detail Panel -->
      <div class="ui-panel">
        <div class="ui-panel-header">
          <h3 class="ui-panel-title" style="display: flex; align-items: center; gap: 0.5rem;">
            <CreditCard :size="16" style="color: #64748b;" /> Chi tiết Giao dịch
          </h3>
        </div>
        <div class="ui-panel-body" style="padding: 0;">
          <div style="display: flex; flex-direction: column;">
            
            <div style="display: flex; justify-content: space-between; padding: 1.25rem 1.5rem; border-bottom: 1px solid #f1f5f9;">
              <span style="font-size: 0.9rem; color: #64748b;">Nội dung thanh toán</span>
              <span style="font-size: 0.9rem; font-weight: 700; color: #0f172a;">Gói {{ paymentDetail.planName }}</span>
            </div>
            
            <div style="display: flex; justify-content: space-between; padding: 1.25rem 1.5rem; border-bottom: 1px solid #f1f5f9;">
              <span style="font-size: 0.9rem; color: #64748b;">Mã ghi chú chuyển khoản</span>
              <span style="font-family: monospace; background: #f8fafc; padding: 0.2rem 0.5rem; border-radius: 6px; border: 1px solid #e2e8f0; font-size: 0.9rem; font-weight: 700; color: #3b82f6;">
                WEBSHORT {{ paymentDetail.id.substring(0,8).toUpperCase() }}
              </span>
            </div>

            <div style="display: flex; justify-content: space-between; align-items: center; padding: 1.25rem 1.5rem; border-bottom: 1px solid #f1f5f9; background: #fafafa;">
              <span style="font-size: 0.9rem; color: #64748b;">Tổng Số Tiền</span>
              <span style="font-size: 1.8rem; font-weight: 900; color: #10b981; letter-spacing: -0.05em;">
                {{ paymentDetail.amount.toLocaleString('vi-VN') }} <span style="font-size: 1rem; font-weight: 700;">{{ paymentDetail.currency }}</span>
              </span>
            </div>

            <div style="padding: 1.5rem; display: flex; flex-direction: column; gap: 1.25rem;">
              <div style="display: flex; justify-content: space-between; align-items: center;">
                <span style="font-size: 0.9rem; color: #64748b;">Trạng thái hệ thống</span>
                <span class="ui-badge ui-badge-warning" style="display: inline-flex; align-items: center; gap: 0.3rem;">
                  <Loader2 :size="12" class="animate-spin" /> Đang chờ đối soát...
                </span>
              </div>
              <div style="background: #eff6ff; border: 1px solid #bfdbfe; border-radius: 10px; padding: 1rem; display: flex; gap: 0.75rem;">
                <Info :size="20" style="color: #3b82f6; flex-shrink: 0;" />
                <p style="margin: 0; font-size: 0.85rem; color: #1d4ed8; line-height: 1.5;">
                  Vui lòng không đóng trang này. Hệ thống tự động kiểm tra mỗi 5s. Khi nhận được thanh toán, màn hình sẽ tự động xử lý gói cước và chuyển hướng.
                </p>
              </div>
            </div>

          </div>
        </div>
      </div>

    </div>
  </div>
</template>
