<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { apiRequest } from '@/api/client'
import { useAuthStore } from '@/stores/auth'

const authStore = useAuthStore()
const logs = ref<any[]>([])
const error = ref('')

async function load() {
  try {
    logs.value = await apiRequest('/api/admin/audit-logs', { token: authStore.token })
  } catch (err) {
    error.value = err instanceof Error ? err.message : 'Không thể tải audit logs.'
  }
}

onMounted(load)
</script>

<template>
  <section class="stack-lg">
    <div class="page-header">
      <div>
        <h2>Audit Logs</h2>
        <p class="muted">Theo dõi hành động quản trị và thay đổi quan trọng trong hệ thống.</p>
      </div>
    </div>

    <p v-if="error" class="error-text">{{ error }}</p>

    <div class="stack-sm">
      <article v-for="log in logs" :key="log.id" class="panel list-row">
        <div>
          <strong>{{ log.action }}</strong>
          <p class="muted">{{ log.targetType }} • {{ log.targetId }}</p>
        </div>
        <span class="muted">{{ log.createdAtUtc }}</span>
      </article>
    </div>
  </section>
</template>
