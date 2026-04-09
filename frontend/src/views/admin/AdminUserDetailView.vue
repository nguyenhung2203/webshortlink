<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { AdminService } from '@/api/services'
import { useAuthStore } from '@/stores/auth'
import { ArrowLeft, User, Lock, Unlock, BadgeCheck, AlertCircle, RefreshCw, Calendar, Link2, MousePointerClick, Shield } from 'lucide-vue-next'

const route = useRoute()
const router = useRouter()
const authStore = useAuthStore()

const userId = route.params.id as string
const user = ref<any>(null)
const error = ref('')
const loading = ref(true)
const planLoading = ref(false)
const lockLoading = ref(false)

const plans = [
  { id: 1, name: 'Thường' },
  { id: 2, name: 'Pro' },
  { id: 3, name: 'Plus' },
]

async function load() {
  loading.value = true
  error.value = ''
  try {
    if (!authStore.accessToken) throw new Error('Chưa xác thực.')
    user.value = await AdminService.getUserDetail(authStore.accessToken, userId)
  } catch (err) {
    error.value = err instanceof Error ? err.message : 'Không thể tải Hồ sơ Khách hàng.'
  } finally {
    loading.value = false
  }
}

async function changePlan(planId: number) {
  if (!authStore.accessToken || !confirm('Nâng cấp/hạ cấp gói dịch vụ cho tài khoản này?')) return
  planLoading.value = true
  try {
    await AdminService.changeUserPlan(authStore.accessToken, userId, planId)
    await load()
  } catch (err) {
    alert(err instanceof Error ? err.message : 'Dịch vụ cấp phép thất bại.')
  } finally {
    planLoading.value = false
  }
}

async function toggleLock() {
  if (!authStore.accessToken || !user.value) return
  const action = user.value.accountStatus === 'Active' ? 'lock' : 'unlock'
  if (!confirm(`Xác nhận ${action === 'lock' ? 'VÔ HIỆU HÓA' : 'MỞ KHÓA'} tài khoản này? Quyền truy cập sẽ bị ảnh hưởng ngay lập tức.`)) return
  lockLoading.value = true
  try {
    await AdminService.toggleUserLock(authStore.accessToken, userId, action)
    await load()
  } catch (err) {
    alert(err instanceof Error ? err.message : 'Thao tác bảo mật thất bại.')
  } finally {
    lockLoading.value = false
  }
}

onMounted(load)
</script>

<template>
  <div class="ui-root">
    
    <div style="margin-bottom: 1rem;">
      <button @click="router.back()" style="display: flex; align-items: center; gap: 0.5rem; font-size: 0.85rem; font-weight: 600; color: #64748b; background: transparent; border: none; cursor: pointer;">
        <ArrowLeft :size="16" /> Trở về danh sách
      </button>
    </div>

    <div class="ui-header">
      <div class="ui-header-left">
        <div class="ui-eyebrow"><User :size="13" /> Định danh Cá nhân</div>
        <h1 class="ui-title">Hồ sơ Cấp phép</h1>
        <p class="ui-subtitle">Tra cứu chi tiết chỉ số và quản lý quyền hạn của người dùng.</p>
      </div>
      <div>
        <button class="ui-btn ui-btn-outline" @click="load" :disabled="loading" style="color: #0f172a;">
          <RefreshCw :size="14" :class="{'animate-spin': loading}" /> Đồng bộ Data
        </button>
      </div>
    </div>

    <div v-if="error" class="ui-alert ui-alert-error" style="background: white;">
      <AlertCircle :size="16" /> {{ error }}
    </div>

    <div v-if="loading" style="display: flex; flex-direction: column; gap: 1.5rem;">
      <div class="ui-skeleton" style="height: 150px; border-radius: 12px;" />
      <div class="ui-skeleton" style="height: 250px; border-radius: 12px;" />
    </div>

    <template v-else-if="user">
      
      <div style="display: flex; flex-direction: column; gap: 1.5rem;">
        
        <!-- Identity Panel -->
        <div class="ui-panel" style="background: white; border-top: 4px solid #0f172a;">
          <div class="ui-panel-body" style="padding: 2rem; display: flex; flex-direction: column; gap: 2rem;">
            
            <div style="display: flex; align-items: center; gap: 1.5rem;">
               <div style="width: 72px; height: 72px; border-radius: 16px; background: #f1f5f9; border: 1px solid #e2e8f0; display: grid; place-items: center; font-size: 2rem; font-weight: 800; color: #94a3b8;">
                 {{ user.email.charAt(0).toUpperCase() }}
               </div>
               <div style="flex: 1;">
                 <h2 style="margin: 0; font-size: 1.5rem; font-weight: 800; color: #0f172a;">{{ user.fullName || 'Chưa định danh tên' }}</h2>
                 <p style="margin: 0.25rem 0 0.75rem; font-size: 0.95rem; font-weight: 500; color: #64748b; font-family: monospace;">{{ user.email }}</p>
                 <div style="display: flex; gap: 0.75rem; align-items: center;">
                   <span class="ui-badge" :class="user.accountStatus === 'Active' ? 'ui-badge-success' : 'ui-badge-error'">
                     <Shield :size="12" style="margin-right: 0.25rem;" /> {{ user.accountStatus === 'Active' ? 'HOẠT ĐỘNG' : 'ĐANG BỊ KHÓA' }}
                   </span>
                   <span class="ui-badge" style="background: #eff6ff; color: #1d4ed8; border: 1px solid #bfdbfe;">
                     GÓI {{ user.currentPlanName ? user.currentPlanName.toUpperCase() : 'THƯỜNG' }}
                   </span>
                   <span v-if="user.emailConfirmed" class="ui-badge" style="background: #f8fafc; color: #475569; border: 1px solid #e2e8f0;">
                     <BadgeCheck :size="12" style="margin-right: 0.25rem; color: #10b981;" /> Đã Verify Email
                   </span>
                 </div>
               </div>
            </div>

            <div style="display: grid; grid-template-columns: repeat(auto-fit, minmax(200px, 1fr)); gap: 1rem; border-top: 1px solid #f1f5f9; padding-top: 2rem;">
              <div>
                <div style="font-size: 0.75rem; font-weight: 700; color: #94a3b8; text-transform: uppercase; margin-bottom: 0.5rem; display: flex; align-items: center; gap: 0.3rem;"><Link2 :size="14" /> Quota Links</div>
                <div style="font-size: 1.5rem; font-weight: 800; color: #0f172a;">{{ user.totalLinks ?? 0 }}</div>
              </div>
              <div>
                <div style="font-size: 0.75rem; font-weight: 700; color: #94a3b8; text-transform: uppercase; margin-bottom: 0.5rem; display: flex; align-items: center; gap: 0.3rem;"><MousePointerClick :size="14" /> Mức tiêu thụ Click</div>
                <div style="font-size: 1.5rem; font-weight: 800; color: #0f172a;">{{ user.totalClicks ?? 0 }}</div>
              </div>
              <div>
                <div style="font-size: 0.75rem; font-weight: 700; color: #94a3b8; text-transform: uppercase; margin-bottom: 0.5rem; display: flex; align-items: center; gap: 0.3rem;"><Calendar :size="14" /> Khởi tạo tài khoản</div>
                <div style="font-size: 1.1rem; font-weight: 700; color: #3b82f6;">{{ user.createdAtUtc ? new Date(user.createdAtUtc).toLocaleDateString('vi-VN') : '-' }}</div>
              </div>
            </div>

          </div>
        </div>

        <!-- Action Controls -->
        <div class="ui-panel" style="background: white;">
          <div class="ui-panel-header" style="border-bottom: 1px solid #f1f5f9; padding: 1.25rem 1.5rem;">
            <h3 class="ui-panel-title">Quyền kiểm soát (Administrative Controls)</h3>
          </div>
          <div class="ui-panel-body" style="padding: 1.5rem;">
            
            <div style="display: flex; flex-direction: column; gap: 2rem;">
              
              <!-- Plan Management -->
              <div>
                <h4 style="font-size: 0.85rem; font-weight: 700; color: #475569; margin: 0 0 1rem;">Cấp phép Gói dịch vụ</h4>
                <div style="display: flex; gap: 0.75rem;">
                  <button
                    v-for="plan in plans" :key="plan.id"
                    @click="changePlan(plan.id)"
                    :disabled="planLoading"
                    class="ui-btn"
                    :style="user.currentPlanId === plan.id 
                      ? 'background: #0f172a; color: white; border: 1px solid #0f172a;' 
                      : 'background: white; border: 1px solid #cbd5e1; color: #475569;'"
                  >
                    Gán gói {{ plan.name }}
                  </button>
                </div>
              </div>

              <hr style="border: 0; border-top: 1px dashed #e2e8f0; margin: 0;" />

              <!-- Security Protocol -->
              <div>
                <h4 style="font-size: 0.85rem; font-weight: 700; color: #475569; margin: 0 0 1rem;">Protocol Bảo mật</h4>
                <button
                  @click="toggleLock"
                  :disabled="lockLoading"
                  class="ui-btn"
                  :style="user.accountStatus === 'Active'
                    ? 'background: #fff1f2; color: #e11d48; border: 1px solid #fecdd3;'
                    : 'background: #ecfdf5; color: #10b981; border: 1px solid #a7f3d0;'"
                >
                  <Lock v-if="user.accountStatus === 'Active'" :size="16" />
                  <Unlock v-else :size="16" />
                  {{ user.accountStatus === 'Active' ? 'Đình chỉ Dịch vụ (Lock Account)' : 'Dỡ bỏ Đình chỉ (Unlock Account)' }}
                </button>
                <p style="margin: 0.75rem 0 0; font-size: 0.75rem; color: #94a3b8;">Hành động khóa sẽ ngay lập tức ngắt toàn bộ quyền truy cập và traffic của khách hàng trên tất cả Link.</p>
              </div>

            </div>

          </div>
        </div>

      </div>
    </template>
  </div>
</template>
