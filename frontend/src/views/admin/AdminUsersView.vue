<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { AdminService } from '@/api/services'
import { useAuthStore } from '@/stores/auth'
import type { AdminUser } from '@/types/api'
import WxPageHeader from '@/components/ui/WxPageHeader.vue'

const authStore = useAuthStore()
const data = ref<AdminUser[]>([])
const error = ref('')

async function load() {
  try {
    if (!authStore.accessToken) {
      throw new Error('Chưa xác thực.')
    }
    data.value = await AdminService.getUsers(authStore.accessToken)
  } catch (err) {
    error.value = err instanceof Error ? err.message : 'Không thể tải danh sách khách hàng.'
  }
}

onMounted(load)
</script>

<template>
  <div class="flex flex-col gap-6">
    <WxPageHeader
      title="Khách hàng"
      description="Quản lý toàn bộ người dùng trên hệ thống."
    />

    <div v-if="error" class="text-red-500 bg-red-50 p-4 rounded-md">
      {{ error }}
    </div>

    <div v-else class="bg-white rounded-xl border border-gray-200 overflow-x-auto">
      <table class="w-full text-left text-sm whitespace-nowrap">
        <thead class="bg-gray-50 text-gray-600 border-b border-gray-200">
          <tr>
            <th class="p-4 font-medium">Email</th>
            <th class="p-4 font-medium">Họ tên</th>
            <th class="p-4 font-medium">Gói dịch vụ</th>
            <th class="p-4 font-medium">Trạng thái</th>
            <th class="p-4 font-medium">Lượng links</th>
            <th class="p-4 font-medium">Ngày tạo</th>
          </tr>
        </thead>
        <tbody class="divide-y divide-gray-100 text-gray-700">
          <tr v-for="user in data" :key="user.id" class="hover:bg-gray-50">
            <td class="p-4 font-medium">{{ user.email }}</td>
            <td class="p-4">{{ user.fullName }}</td>
            <td class="p-4"><span class="px-2 py-1 bg-blue-100 text-blue-700 rounded-md text-xs font-semibold">{{ user.planName }}</span></td>
            <td class="p-4">
              <span :class="user.status === 'Active' ? 'text-green-600 bg-green-50' : 'text-red-600 bg-red-50'" class="px-2 py-1 rounded-md text-xs font-semibold">
                {{ user.status }}
              </span>
            </td>
            <td class="p-4">{{ user.totalLinks }}</td>
            <td class="p-4">{{ new Date(user.createdAtUtc).toLocaleDateString('vi-VN') }}</td>
          </tr>
          <tr v-if="data.length === 0">
            <td colspan="6" class="p-8 text-center text-gray-500">Chưa có người dùng nào.</td>
          </tr>
        </tbody>
      </table>
    </div>
  </div>
</template>
