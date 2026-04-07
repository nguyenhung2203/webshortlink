<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { AdminService } from '@/api/services'
import { useAuthStore } from '@/stores/auth'
import { ArrowLeft, User, Lock, Unlock, BadgeCheck } from 'lucide-vue-next'
import WxPageHeader from '@/components/ui/WxPageHeader.vue'

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
  try {
    if (!authStore.accessToken) throw new Error('Chưa xác thực.')
    user.value = await AdminService.getUserDetail(authStore.accessToken, userId)
  } catch (err) {
    error.value = err instanceof Error ? err.message : 'Không thể tải thông tin người dùng.'
  } finally {
    loading.value = false
  }
}

async function changePlan(planId: number) {
  if (!authStore.accessToken || !confirm('Đổi gói dịch vụ cho người dùng này?')) return
  planLoading.value = true
  try {
    await AdminService.changeUserPlan(authStore.accessToken, userId, planId)
    await load()
  } catch (err) {
    alert(err instanceof Error ? err.message : 'Lỗi khi đổi gói.')
  } finally {
    planLoading.value = false
  }
}

async function toggleLock() {
  if (!authStore.accessToken || !user.value) return
  const action = user.value.status === 'Active' ? 'lock' : 'unlock'
  if (!confirm(`${action === 'lock' ? 'Khóa' : 'Mở khóa'} tài khoản này?`)) return
  lockLoading.value = true
  try {
    await AdminService.toggleUserLock(authStore.accessToken, userId, action)
    await load()
  } catch (err) {
    alert(err instanceof Error ? err.message : 'Lỗi khi thay đổi trạng thái.')
  } finally {
    lockLoading.value = false
  }
}

onMounted(load)
</script>

<template>
  <div class="flex flex-col gap-6">
    <div class="flex items-center gap-3">
      <button @click="router.back()" class="flex items-center gap-1.5 text-sm text-on-surface-variant hover:text-on-surface transition-colors">
        <ArrowLeft :size="16" /> Quay lại
      </button>
    </div>

    <WxPageHeader title="Chi tiết người dùng" description="Xem và quản lý thông tin tài khoản cụ thể." />

    <div v-if="loading" class="text-center py-10 text-on-surface-variant">Đang tải...</div>
    <div v-else-if="error" class="text-danger bg-danger/10 rounded-xl p-4">{{ error }}</div>

    <template v-else-if="user">
      <!-- Profile Card -->
      <div class="bg-surface-container rounded-2xl border border-outline p-6 flex flex-col sm:flex-row gap-6">
        <div class="w-16 h-16 rounded-full bg-primary/10 flex items-center justify-center shrink-0">
          <User :size="30" class="text-primary" />
        </div>
        <div class="flex-1 min-w-0">
          <h2 class="text-xl font-bold text-on-surface mb-1">{{ user.fullName || 'N/A' }}</h2>
          <p class="text-on-surface-variant text-sm mb-3">{{ user.email }}</p>
          <div class="flex flex-wrap gap-2">
            <span class="px-2.5 py-1 rounded-full text-xs font-bold" :class="user.status === 'Active' ? 'bg-success/10 text-success' : 'bg-danger/10 text-danger'">
              {{ user.status }}
            </span>
            <span class="px-2.5 py-1 rounded-full text-xs font-bold bg-primary/10 text-primary">
              {{ user.planName || 'Thường' }}
            </span>
            <span v-if="user.emailConfirmed" class="px-2.5 py-1 rounded-full text-xs font-bold bg-success/10 text-success flex items-center gap-1">
              <BadgeCheck :size="12" /> Email xác thực
            </span>
          </div>
        </div>
      </div>

      <!-- Stats row -->
      <div class="grid grid-cols-2 sm:grid-cols-4 gap-4">
        <div class="bg-surface-container rounded-xl border border-outline p-4 text-center">
          <p class="text-2xl font-extrabold text-on-surface">{{ user.totalLinks ?? 0 }}</p>
          <p class="text-xs text-on-surface-variant mt-1">Tổng Links</p>
        </div>
        <div class="bg-surface-container rounded-xl border border-outline p-4 text-center">
          <p class="text-2xl font-extrabold text-on-surface">{{ user.totalClicks ?? 0 }}</p>
          <p class="text-xs text-on-surface-variant mt-1">Tổng Clicks</p>
        </div>
        <div class="bg-surface-container rounded-xl border border-outline p-4 text-center">
          <p class="text-sm font-bold text-on-surface">{{ user.createdAtUtc ? new Date(user.createdAtUtc).toLocaleDateString('vi-VN') : '-' }}</p>
          <p class="text-xs text-on-surface-variant mt-1">Ngày đăng ký</p>
        </div>
        <div class="bg-surface-container rounded-xl border border-outline p-4 text-center">
          <p class="text-sm font-bold text-on-surface">{{ user.subscriptionEndAtUtc ? new Date(user.subscriptionEndAtUtc).toLocaleDateString('vi-VN') : 'N/A' }}</p>
          <p class="text-xs text-on-surface-variant mt-1">Hết hạn gói</p>
        </div>
      </div>

      <!-- Actions -->
      <div class="bg-surface-container rounded-2xl border border-outline p-6 flex flex-col gap-5">
        <h3 class="font-semibold text-on-surface">Thao tác quản trị</h3>

        <div class="flex flex-col sm:flex-row gap-3 items-start sm:items-center">
          <label class="text-sm font-medium text-on-surface-variant w-28 shrink-0">Đổi gói:</label>
          <div class="flex gap-2 flex-wrap">
            <button
              v-for="plan in plans" :key="plan.id"
              @click="changePlan(plan.id)"
              :disabled="planLoading"
              class="px-4 py-2 text-sm rounded-lg border font-semibold transition-all"
              :class="user.currentPlanId === plan.id
                ? 'bg-primary text-on-primary border-primary'
                : 'border-outline text-on-surface hover:bg-surface-container-high'"
            >{{ plan.name }}</button>
          </div>
        </div>

        <div class="flex flex-col sm:flex-row gap-3 items-start sm:items-center">
          <label class="text-sm font-medium text-on-surface-variant w-28 shrink-0">Trạng thái:</label>
          <button
            @click="toggleLock"
            :disabled="lockLoading"
            class="flex items-center gap-2 px-4 py-2 text-sm rounded-lg border font-semibold transition-all"
            :class="user.status === 'Active'
              ? 'border-danger/50 text-danger hover:bg-danger/10'
              : 'border-success/50 text-success hover:bg-success/10'"
          >
            <Lock v-if="user.status === 'Active'" :size="15" />
            <Unlock v-else :size="15" />
            {{ user.status === 'Active' ? 'Khóa tài khoản' : 'Mở khóa tài khoản' }}
          </button>
        </div>
      </div>
    </template>
  </div>
</template>
