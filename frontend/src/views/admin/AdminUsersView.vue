<script setup lang="ts">
import { onMounted, ref, computed } from 'vue'
import { AdminService } from '@/api/services'
import { useAuthStore } from '@/stores/auth'
import type { AdminUser } from '@/types/api'
import WxPageHeader from '@/components/ui/WxPageHeader.vue'
import { Search, User, Lock, Unlock, ChevronRight } from 'lucide-vue-next'
import { RouterLink } from 'vue-router'

const authStore = useAuthStore()
const data = ref<AdminUser[]>([])
const error = ref('')
const isActionLoading = ref(false)
const search = ref('')
const changePlanLoadingId = ref<string | null>(null)

const plans = [
  { id: 1, name: 'Thường' },
  { id: 2, name: 'Pro' },
  { id: 3, name: 'Plus' },
]

const planBadgeClass = (planName: string) => {
  if (planName === 'Plus') return 'bg-amber-100 text-amber-700'
  if (planName === 'Pro') return 'bg-blue-100 text-blue-700'
  return 'bg-surface-container-high text-on-surface-variant'
}

async function load() {
  try {
    if (!authStore.accessToken) throw new Error('Chưa xác thực.')
    data.value = await AdminService.getUsers(authStore.accessToken)
  } catch (err) {
    error.value = err instanceof Error ? err.message : 'Không thể tải danh sách khách hàng.'
  }
}

async function toggleUserStatus(user: AdminUser) {
  if (isActionLoading.value || !authStore.accessToken) return
  const action = user.status === 'Active' ? 'lock' : 'unlock'
  if (!confirm(`${action === 'lock' ? 'Khóa' : 'Mở khóa'} tài khoản ${user.email}?`)) return
  isActionLoading.value = true
  try {
    await AdminService.toggleUserLock(authStore.accessToken, user.id, action)
    await load()
  } catch (err) {
    alert(err instanceof Error ? err.message : 'Lỗi khi thay đổi trạng thái user.')
  } finally {
    isActionLoading.value = false
  }
}

async function changePlan(userId: string, planId: number) {
  if (!authStore.accessToken) return
  changePlanLoadingId.value = userId
  try {
    await AdminService.changeUserPlan(authStore.accessToken, userId, planId)
    await load()
  } catch (err) {
    alert(err instanceof Error ? err.message : 'Lỗi khi đổi gói.')
  } finally {
    changePlanLoadingId.value = null
  }
}

const filtered = computed(() => {
  if (!search.value.trim()) return data.value
  const q = search.value.trim().toLowerCase()
  return data.value.filter(u =>
    u.email.toLowerCase().includes(q) || (u.fullName || '').toLowerCase().includes(q)
  )
})

onMounted(load)
</script>

<template>
  <div class="flex flex-col gap-6">
    <WxPageHeader title="Khách hàng" description="Quản lý toàn bộ người dùng trên hệ thống." />

    <div v-if="error" class="text-danger bg-danger/10 rounded-xl p-4 text-sm">{{ error }}</div>

    <!-- Search -->
    <div class="relative">
      <Search :size="16" class="absolute left-3 top-1/2 -translate-y-1/2 text-on-surface-variant" />
      <input
        v-model="search"
        type="text"
        placeholder="Tìm theo email hoặc tên..."
        class="w-full h-10 pl-9 pr-4 rounded-xl border border-outline bg-surface-container text-sm focus:outline-none focus:border-primary"
      />
    </div>

    <!-- Desktop table (hidden on mobile) -->
    <div class="hidden md:block bg-surface-container rounded-xl border border-outline overflow-hidden relative">
      <div v-if="isActionLoading" class="absolute inset-0 bg-surface/50 z-10 flex items-center justify-center">
        <span class="text-on-surface-variant font-medium text-sm">Đang xử lý...</span>
      </div>
      <table class="w-full text-left text-sm">
        <thead class="bg-surface-container-high text-on-surface-variant border-b border-outline">
          <tr>
            <th class="p-4 font-semibold">Email</th>
            <th class="p-4 font-semibold">Họ tên</th>
            <th class="p-4 font-semibold">Gói</th>
            <th class="p-4 font-semibold">Trạng thái</th>
            <th class="p-4 font-semibold">Links</th>
            <th class="p-4 font-semibold">Ngày tạo</th>
            <th class="p-4 font-semibold">Thao tác</th>
          </tr>
        </thead>
        <tbody class="divide-y divide-outline">
          <tr v-for="user in filtered" :key="user.id" class="hover:bg-surface-container-low">
            <td class="p-4 font-medium text-on-surface">{{ user.email }}</td>
            <td class="p-4 text-on-surface">{{ user.fullName || '—' }}</td>
            <td class="p-4">
              <div class="flex items-center gap-1.5">
                <span class="px-2 py-0.5 rounded-full text-xs font-bold" :class="planBadgeClass(user.planName)">{{ user.planName }}</span>
                <!-- Quick plan switch -->
                <select
                  v-model.number="(user as any).__newPlanId"
                  class="h-6 text-xs rounded border border-outline bg-surface px-1 focus:outline-none focus:border-primary"
                  @change="changePlan(user.id, (user as any).__newPlanId)"
                >
                  <option value="" disabled selected>Đổi...</option>
                  <option v-for="p in plans" :key="p.id" :value="p.id">{{ p.name }}</option>
                </select>
              </div>
            </td>
            <td class="p-4">
              <span :class="user.status === 'Active' ? 'text-success bg-success/10' : 'text-danger bg-danger/10'" class="px-2 py-0.5 rounded-full text-xs font-bold">
                {{ user.status }}
              </span>
            </td>
            <td class="p-4 text-on-surface">{{ user.totalLinks }}</td>
            <td class="p-4 text-on-surface-variant">{{ new Date(user.createdAtUtc).toLocaleDateString('vi-VN') }}</td>
            <td class="p-4">
              <div class="flex items-center gap-3">
                <RouterLink :to="{ name: 'admin-user-detail', params: { id: user.id } }" class="text-primary font-semibold text-sm hover:underline">Chi tiết</RouterLink>
                <button
                  @click="toggleUserStatus(user)"
                  :disabled="isActionLoading"
                  :class="user.status === 'Active' ? 'text-danger hover:text-danger/80' : 'text-success hover:text-success/80'"
                  class="font-semibold text-sm transition-colors"
                >
                  {{ user.status === 'Active' ? 'Khóa' : 'Mở khóa' }}
                </button>
              </div>
            </td>
          </tr>
          <tr v-if="filtered.length === 0">
            <td colspan="7" class="p-8 text-center text-on-surface-variant">{{ search ? 'Không tìm thấy kết quả.' : 'Chưa có người dùng nào.' }}</td>
          </tr>
        </tbody>
      </table>
    </div>

    <!-- Mobile card list -->
    <div class="flex flex-col gap-3 md:hidden">
      <div v-if="filtered.length === 0" class="text-center text-on-surface-variant text-sm py-6">
        {{ search ? 'Không tìm thấy kết quả.' : 'Chưa có người dùng nào.' }}
      </div>
      <div
        v-for="user in filtered" :key="user.id + '-mobile'"
        class="bg-surface-container rounded-2xl border border-outline p-4 flex flex-col gap-3"
      >
        <div class="flex items-start justify-between gap-2">
          <div class="flex items-center gap-2 min-w-0">
            <div class="w-9 h-9 rounded-full bg-primary/10 flex items-center justify-center shrink-0">
              <User :size="16" class="text-primary" />
            </div>
            <div class="min-w-0">
              <p class="font-semibold text-on-surface text-sm truncate">{{ user.email }}</p>
              <p class="text-xs text-on-surface-variant">{{ user.fullName || '—' }}</p>
            </div>
          </div>
          <div class="flex items-center gap-1.5 shrink-0">
            <span class="px-2 py-0.5 rounded-full text-xs font-bold" :class="planBadgeClass(user.planName)">{{ user.planName }}</span>
            <span :class="user.status === 'Active' ? 'text-success bg-success/10' : 'text-danger bg-danger/10'" class="px-2 py-0.5 rounded-full text-xs font-bold">{{ user.status }}</span>
          </div>
        </div>
        <div class="text-xs text-on-surface-variant flex gap-3">
          <span>{{ user.totalLinks }} links</span>
          <span>{{ new Date(user.createdAtUtc).toLocaleDateString('vi-VN') }}</span>
        </div>
        <div class="flex items-center gap-2 pt-1 border-t border-outline">
          <RouterLink :to="{ name: 'admin-user-detail', params: { id: user.id } }" class="flex items-center gap-1 text-primary font-semibold text-sm">
            Chi tiết <ChevronRight :size="14" />
          </RouterLink>
          <button
            @click="toggleUserStatus(user)"
            :disabled="isActionLoading"
            class="flex items-center gap-1 text-sm font-semibold ml-auto transition-colors"
            :class="user.status === 'Active' ? 'text-danger' : 'text-success'"
          >
            <Lock v-if="user.status === 'Active'" :size="13" />
            <Unlock v-else :size="13" />
            {{ user.status === 'Active' ? 'Khóa' : 'Mở khóa' }}
          </button>
        </div>
      </div>
    </div>
  </div>
</template>
