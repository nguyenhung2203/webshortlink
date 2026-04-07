<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { apiRequest } from '@/api/client'
import { useAuthStore } from '@/stores/auth'

const authStore = useAuthStore()
const links = ref<any[]>([])
const error = ref('')

async function load() {
  try {
    links.value = await apiRequest('/api/admin/links', { token: authStore.token })
  } catch (err) {
    error.value = err instanceof Error ? err.message : 'Không thể tải links.'
  }
}

async function toggle(link: any) {
  const action = link.status === 'Blocked' ? 'unblock' : 'block'

  try {
    await apiRequest(`/api/admin/links/${link.id}/${action}`, {
      method: 'PATCH',
      token: authStore.token,
    })
    await load()
  } catch (err) {
    error.value = err instanceof Error ? err.message : 'Không thể cập nhật link.'
  }
}

onMounted(load)
</script>

<template>
  <section class="stack-lg">
    <div class="page-header">
      <div>
        <h2>Links</h2>
        <p class="muted">Giám sát shortlink, owner, traffic và abuse score.</p>
      </div>
    </div>

    <p v-if="error" class="error-text">{{ error }}</p>

    <div class="stack-sm">
      <article v-for="link in links" :key="link.id" class="panel">
        <div class="link-card-header">
          <div>
            <strong>{{ link.shortUrl }}</strong>
            <p class="muted">{{ link.owner }}</p>
          </div>
          <span class="badge">{{ link.status }}</span>
        </div>

        <div class="link-metrics">
          <span>Total {{ link.totalClicks }}</span>
          <span>Bot {{ link.botClicks }}</span>
          <span>Abuse {{ link.abuseScore }}</span>
        </div>

        <button class="ghost-button" @click="toggle(link)">
          {{ link.status === 'Blocked' ? 'Unblock' : 'Block' }}
        </button>
      </article>
    </div>
  </section>
</template>
