<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { apiRequest } from '@/api/client'
import { useAuthStore } from '@/stores/auth'

const authStore = useAuthStore()
const users = ref<any[]>([])
const error = ref('')

async function load() {
  try {
    users.value = await apiRequest('/api/admin/users', { token: authStore.token })
  } catch (err) {
    error.value = err instanceof Error ? err.message : 'Không thể tải users.'
  }
}

async function toggle(user: any) {
  const action = user.status === 'Active' ? 'lock' : 'unlock'

  try {
    await apiRequest(`/api/admin/users/${user.id}/${action}`, {
      method: 'PATCH',
      token: authStore.token,
    })
    await load()
  } catch (err) {
    error.value = err instanceof Error ? err.message : 'Không thể cập nhật user.'
  }
}

onMounted(load)
</script>

<template>
  <section class="stack-lg">
    <div class="page-header">
      <div>
        <h2>Users</h2>
        <p class="muted">Quản lý tài khoản, plan và trạng thái kích hoạt.</p>
      </div>
    </div>

    <p v-if="error" class="error-text">{{ error }}</p>

    <div class="stack-sm">
      <article v-for="user in users" :key="user.id" class="panel list-row list-row-wrap">
        <div>
          <strong>{{ user.email }}</strong>
          <p class="muted">{{ user.fullName }} • {{ user.plan }}</p>
          <p class="muted">Links {{ user.links }} • Clicks {{ user.totalClicks }}</p>
        </div>
        <div class="inline-actions">
          <span class="badge">{{ user.status }}</span>
          <button class="ghost-button" @click="toggle(user)">
            {{ user.status === 'Active' ? 'Lock' : 'Unlock' }}
          </button>
        </div>
      </article>
    </div>
  </section>
</template>
