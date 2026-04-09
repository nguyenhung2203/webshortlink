<script setup lang="ts">
import { onMounted, ref, computed, watch } from 'vue'
import { AdminService } from '@/api/services'
import { useAuthStore } from '@/stores/auth'
import type { AdminUser } from '@/types/api'
import { Search, User, Lock, Unlock, ChevronRight, RefreshCw, AlertCircle, Shield } from 'lucide-vue-next'
import { RouterLink } from 'vue-router'

const authStore = useAuthStore()
const data = ref<AdminUser[]>([])
const error = ref('')
const isActionLoading = ref(false)
const search = ref('')
const loading = ref(true)

const filter = ref({
  pageIndex: 1,
  pageSize: 10
})

const plans = [
  { id: 1, name: 'Thường' },
  { id: 2, name: 'Pro' },
  { id: 3, name: 'Plus' },
]

const planBadgeClass = (planName: string) => {
  if (planName === 'Plus') return 'color: #b45309; background: #fffbeb; border: 1px solid #fde68a;'
  if (planName === 'Pro') return 'color: #1d4ed8; background: #eff6ff; border: 1px solid #bfdbfe;'
  return 'color: #475569; background: #f1f5f9; border: 1px solid #e2e8f0;'
}

async function load() {
  loading.value = true
  error.value = ''
  try {
    if (!authStore.accessToken) throw new Error('Chưa xác thực.')
    data.value = await AdminService.getUsers(authStore.accessToken)
  } catch (err) {
    error.value = err instanceof Error ? err.message : 'Không thể tải danh sách khách hàng.'
  } finally {
    loading.value = false
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
  isActionLoading.value = true
  try {
    await AdminService.changeUserPlan(authStore.accessToken, userId, planId)
    await load()
  } catch (err) {
    alert(err instanceof Error ? err.message : 'Lỗi khi đổi gói.')
  } finally {
    isActionLoading.value = false
  }
}

const filtered = computed(() => {
  if (!search.value.trim()) return data.value
  const q = search.value.trim().toLowerCase()
  return data.value.filter(u =>
    u.email.toLowerCase().includes(q) || (u.fullName || '').toLowerCase().includes(q)
  )
})

const totalCount = computed(() => filtered.value.length)
const totalPages = computed(() => Math.max(1, Math.ceil(totalCount.value / filter.value.pageSize)))

const paginated = computed(() => {
  const start = (filter.value.pageIndex - 1) * filter.value.pageSize
  return filtered.value.slice(start, start + filter.value.pageSize)
})

watch(search, () => { filter.value.pageIndex = 1 })

onMounted(load)
</script>

<template>
  <div class="ui-root">
    
    <div class="ui-header">
      <div class="ui-header-left">
        <div class="ui-eyebrow"><Users :size="13" /> Quản lý Khách hàng</div>
        <h1 class="ui-title">Danh sách Tài khoản</h1>
        <p class="ui-subtitle">Quản lý định danh, gói dịch vụ và trạng thái bảo mật của toàn bộ người dùng.</p>
      </div>
      <div>
        <button class="ui-btn ui-btn-outline" @click="load" :disabled="loading">
          <RefreshCw :size="14" :class="{'animate-spin': loading}" /> Làm mới
        </button>
      </div>
    </div>

    <div v-if="error" class="ui-alert ui-alert-error">
      <AlertCircle :size="16" /> {{ error }}
    </div>

    <!-- Toolbar -->
    <div class="ui-panel" style="padding: 1rem 1.5rem; display: flex; gap: 1rem; flex-wrap: wrap; align-items: center;">
      <div style="flex: 1; min-width: 250px; position: relative;">
        <Search :size="16" style="position: absolute; left: 1rem; top: 50%; transform: translateY(-50%); color: #94a3b8;" />
        <input
          v-model="search"
          type="text"
          placeholder="Tìm kiếm bằng địa chỉ email hoặc họ tên..."
          class="ui-form-input"
          style="padding-left: 2.5rem; margin: 0; box-shadow: none;"
        />
      </div>
    </div>

    <div v-if="loading" class="ui-skeleton" style="height: 400px; border-radius: 12px;" />

    <div v-else-if="filtered.length === 0" class="ui-empty" style="background: white;">
      <div class="ui-empty-icon" style="opacity: 0.5;"><Users :size="48" /></div>
      <h3 class="ui-empty-title">{{ search ? 'Không tìm thấy tài khoản tương thích' : 'Chưa có tài khoản nào' }}</h3>
      <p class="ui-empty-desc">Không có dữ liệu khớp với bộ lọc hiện tại của bạn.</p>
    </div>

    <!-- User Table -->
    <div v-else class="ui-panel" style="overflow: hidden; padding: 0; position: relative;">
      
      <div v-if="isActionLoading" style="position: absolute; inset: 0; z-index: 10; background: rgba(255,255,255,0.7); backdrop-filter: blur(2px); display: flex; justify-content: center; align-items: center;">
        <span style="font-weight: 600; font-size: 0.9rem; color: #3b82f6; display: flex; align-items: center; gap: 0.5rem;">
          <RefreshCw :size="16" class="animate-spin" /> Hệ thống đang xử lý...
        </span>
      </div>

      <div style="overflow-x: auto;">
        <table style="width: 100%; border-collapse: collapse; min-width: 900px; text-align: left;">
          <thead style="background: #f8fafc; border-bottom: 1px solid #e2e8f0;">
            <tr>
              <th style="padding: 1rem 1.5rem; font-size: 0.75rem; font-weight: 700; color: #64748b; text-transform: uppercase; width: 60px;">STT</th>
              <th style="padding: 1rem 1.5rem; font-size: 0.75rem; font-weight: 700; color: #64748b; text-transform: uppercase;">Định danh</th>
              <th style="padding: 1rem 1.5rem; font-size: 0.75rem; font-weight: 700; color: #64748b; text-transform: uppercase;">Gói Dịch vụ</th>
              <th style="padding: 1rem 1.5rem; font-size: 0.75rem; font-weight: 700; color: #64748b; text-transform: uppercase;">Trạng thái</th>
              <th style="padding: 1rem 1.5rem; font-size: 0.75rem; font-weight: 700; color: #64748b; text-transform: uppercase;">Tổng Links</th>
              <th style="padding: 1rem 1.5rem; font-size: 0.75rem; font-weight: 700; color: #64748b; text-transform: uppercase;">Ngày tạo</th>
              <th style="padding: 1rem 1.5rem; font-size: 0.75rem; font-weight: 700; color: #64748b; text-transform: uppercase; text-align: right;">Quyền kiểm soát</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="(user, index) in paginated" :key="user.id" style="border-bottom: 1px solid #f1f5f9; transition: background 0.15s;" class="hover:bg-slate-50">
              <td style="padding: 1rem 1.5rem; font-weight: 600; color: #94a3b8;">
                {{ (filter.pageIndex - 1) * filter.pageSize + index + 1 }}
              </td>
              <td style="padding: 1rem 1.5rem;">
                <div style="display: flex; align-items: center; gap: 0.75rem;">
                  <div style="width: 32px; height: 32px; border-radius: 50%; background: #e0f2fe; color: #0284c7; display: grid; place-items: center; font-weight: 800; font-size: 0.85rem; flex-shrink: 0;">
                    {{ user.email.charAt(0).toUpperCase() }}
                  </div>
                  <div style="min-width: 0;">
                    <p style="margin: 0; font-weight: 700; color: #0f172a; font-size: 0.95rem; line-height: 1.2;">
                      {{ user.email }}
                      <span v-if="user.role === 'Admin'" style="margin-left: 0.5rem; padding: 0.1rem 0.4rem; background: #fee2e2; color: #991b1b; font-size: 0.6rem; font-weight: 800; border-radius: 4px; border: 1px solid #fecaca; vertical-align: middle;">ADMIN</span>
                    </p>
                    <p style="margin: 0; font-size: 0.75rem; color: #64748b; margin-top: 0.1rem;">{{ user.fullName || 'Chưa thiết lập tên' }}</p>
                  </div>
                </div>
              </td>
              
              <td style="padding: 1rem 1.5rem;">
                <div style="display: flex; align-items: center; gap: 0.5rem;">
                  <span style="padding: 0.2rem 0.6rem; border-radius: 999px; font-size: 0.7rem; font-weight: 700; letter-spacing: 0.5px; text-transform: uppercase;" :style="planBadgeClass(user.planName)">
                    {{ user.planName }}
                  </span>
                  <select
                    v-model.number="(user as any).__newPlanId"
                    @change="changePlan(user.id, (user as any).__newPlanId)"
                    style="height: 24px; font-size: 0.7rem; border-radius: 4px; border: 1px solid #cbd5e1; background: transparent; cursor: pointer; color: #475569;"
                  >
                    <option value="" disabled selected>Đổi gói ▼</option>
                    <option v-for="p in plans" :key="p.id" :value="p.id">{{ p.name }}</option>
                  </select>
                </div>
              </td>
              
              <td style="padding: 1rem 1.5rem;">
                <div style="display: flex; align-items: center; gap: 0.35rem;">
                  <Shield :size="14" :style="user.status === 'Active' ? 'color: #10b981;' : 'color: #ef4444;'" />
                  <span class="ui-badge" :class="user.status === 'Active' ? 'ui-badge-success' : 'ui-badge-error'">
                    {{ user.status === 'Active' ? 'HIỆU LỰC' : 'ĐÃ BỊ KHÓA' }}
                  </span>
                </div>
              </td>
              
              <td style="padding: 1rem 1.5rem;">
                <span style="font-weight: 700; color: #0f172a; font-size: 0.95rem;">{{ user.totalLinks.toLocaleString() }}</span>
              </td>
              
              <td style="padding: 1rem 1.5rem;">
                <span style="font-size: 0.85rem; color: #64748b; font-variant-numeric: tabular-nums;">
                  {{ new Date(user.createdAtUtc).toLocaleDateString('vi-VN') }}
                </span>
              </td>

              <td style="padding: 1rem 1.5rem; text-align: right;">
                <div style="display: flex; align-items: center; justify-content: flex-end; gap: 0.75rem;">
                  <RouterLink :to="{ name: 'admin-user-detail', params: { id: user.id } }" style="font-size: 0.85rem; font-weight: 600; color: #3b82f6; text-decoration: none;">
                    Kiểm tra
                  </RouterLink>
                  <span style="color: #cbd5e1;">|</span>
                  <button
                    v-if="user.role !== 'Admin'"
                    @click="toggleUserStatus(user)"
                    :disabled="isActionLoading"
                    style="background: transparent; border: 0; font-size: 0.85rem; font-weight: 600; cursor: pointer; display: flex; align-items: center; gap: 0.25rem;"
                    :style="user.status === 'Active' ? 'color: #ef4444;' : 'color: #10b981;'"
                  >
                    <Lock v-if="user.status === 'Active'" :size="14" />
                    <Unlock v-else :size="14" />
                    {{ user.status === 'Active' ? 'Khóa User' : 'Mở khóa' }}
                  </button>
                  <span v-else style="font-size: 0.75rem; color: #94a3b8; font-style: italic;">Không thể khóa Admin</span>
                </div>
              </td>

            </tr>
          </tbody>
        </table>
      </div>
      
      <!-- Pagination Controls -->
      <div v-if="totalPages > 1" style="padding: 1rem 1.5rem; display: flex; align-items: center; justify-content: space-between; border-top: 1px solid #e2e8f0; background: #fff;">
        <span style="font-size: 0.85rem; color: #64748b;">
          Trang <strong>{{ filter.pageIndex }}</strong> / {{ totalPages }} (Tổng {{ totalCount }} mục)
        </span>
        <div style="display: flex; gap: 0.5rem;">
          <button class="ui-btn ui-btn-outline" style="padding: 0.25rem 0.5rem; height: 32px; font-size: 0.8rem; background: white;" :disabled="filter.pageIndex === 1" @click="filter.pageIndex--">
            <ChevronLeft :size="14" /> Trước
          </button>
          <button class="ui-btn ui-btn-outline" style="padding: 0.25rem 0.5rem; height: 32px; font-size: 0.8rem; background: white;" :disabled="filter.pageIndex >= totalPages" @click="filter.pageIndex++">
            Sau <ChevronRight :size="14" />
          </button>
        </div>
      </div>

    </div>
  </div>
</template>
