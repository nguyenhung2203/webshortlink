<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { AdminService } from '@/api/services'
import { useAuthStore } from '@/stores/auth'
import type { AdminLink } from '@/types/api'
import WxPageHeader from '@/components/ui/WxPageHeader.vue'

const authStore = useAuthStore()
const data = ref<AdminLink[]>([])
const error = ref('')
const isActionLoading = ref(false)

async function load() {
  try {
    if (!authStore.accessToken) {
      throw new Error('Chưa xác thực.')
    }
    data.value = await AdminService.getLinks(authStore.accessToken)
  } catch (err) {
    error.value = err instanceof Error ? err.message : 'Không thể tải danh sách links.'
  }
}

async function toggleLinkStatus(link: AdminLink) {
  if (isActionLoading.value || !authStore.accessToken) return
  
  const action = link.status === 'Active' ? 'block' : 'unblock'
  if (!confirm(`Bạn có chắc chắn muốn ${action === 'block' ? 'vô hiệu hóa' : 'kích hoạt lại'} link /${link.slug}?`)) return
  
  try {
    isActionLoading.value = true
    await AdminService.toggleLinkBlock(authStore.accessToken, link.id, action)
    await load()
  } catch (err) {
    alert(err instanceof Error ? err.message : 'Lỗi khi thay đổi trạng thái link.')
  } finally {
    isActionLoading.value = false
  }
}

onMounted(load)
</script>

<template>
  <div class="flex flex-col gap-6">
    <WxPageHeader
      title="Quản lý Links"
      description="Giám sát và quản lý toàn bộ link của hệ thống."
    />

    <div v-if="error" class="text-red-500 bg-red-50 p-4 rounded-md">
      {{ error }}
    </div>

    <div v-else class="bg-white rounded-xl border border-gray-200 overflow-x-auto relative">
      <div v-if="isActionLoading" class="absolute inset-0 bg-white/50 z-10 flex items-center justify-center">
        <span class="text-gray-500 font-medium">Đang xử lý...</span>
      </div>
      <table class="w-full text-left text-sm whitespace-nowrap">
        <thead class="bg-gray-50 text-gray-600 border-b border-gray-200">
          <tr>
            <th class="p-4 font-medium">Đường dẫn</th>
            <th class="p-4 font-medium">Chủ sở hữu</th>
            <th class="p-4 font-medium">Lượt Click</th>
            <th class="p-4 font-medium">Lượt Bot</th>
            <th class="p-4 font-medium">Risk Score</th>
            <th class="p-4 font-medium">Trạng thái</th>
            <th class="p-4 font-medium">Ngày tạo</th>
            <th class="p-4 font-medium">Thao tác</th>
          </tr>
        </thead>
        <tbody class="divide-y divide-gray-100 text-gray-700">
          <tr v-for="link in data" :key="link.id" class="hover:bg-gray-50">
            <td class="p-4 text-blue-600 hover:underline"><a :href="link.shortUrl" target="_blank">{{ link.slug }}</a></td>
            <td class="p-4">{{ link.userEmail }}</td>
            <td class="p-4">{{ link.totalClicks }}</td>
            <td class="p-4">{{ link.botClicks }}</td>
            <td class="p-4 text-red-600 font-medium">{{ link.highestRiskScore ?? '-' }}</td>
            <td class="p-4">
              <span :class="link.status === 'Active' ? 'text-green-600 bg-green-50' : 'text-gray-600 bg-gray-100'" class="px-2 py-1 rounded-md text-xs font-semibold">
                {{ link.status }}
              </span>
            </td>
            <td class="p-4">{{ new Date(link.createdAtUtc).toLocaleDateString('vi-VN') }}</td>
            <td class="p-4">
              <button 
                @click="toggleLinkStatus(link)"
                :disabled="isActionLoading"
                :class="link.status === 'Active' ? 'text-red-600 hover:text-red-800' : 'text-green-600 hover:text-green-800'"
                class="font-semibold transition-colors focus:outline-none"
              >
                {{ link.status === 'Active' ? 'Khóa' : 'Mở khóa' }}
              </button>
            </td>
          </tr>
          <tr v-if="data.length === 0">
            <td colspan="8" class="p-8 text-center text-gray-500">Chưa có dữ liệu link.</td>
          </tr>
        </tbody>
      </table>
    </div>
  </div>
</template>
