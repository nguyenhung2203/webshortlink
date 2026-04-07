<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { AdminService } from '@/api/services'
import { useAuthStore } from '@/stores/auth'
import type { AdminLink } from '@/types/api'
import WxPageHeader from '@/components/ui/WxPageHeader.vue'

const authStore = useAuthStore()
const data = ref<AdminLink[]>([])
const error = ref('')

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

    <div v-else class="bg-white rounded-xl border border-gray-200 overflow-x-auto">
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
          </tr>
          <tr v-if="data.length === 0">
            <td colspan="7" class="p-8 text-center text-gray-500">Chưa có dữ liệu link.</td>
          </tr>
        </tbody>
      </table>
    </div>
  </div>
</template>
