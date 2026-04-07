<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { apiRequest } from '@/api/client'
import { useAuthStore } from '@/stores/auth'

const authStore = useAuthStore()
const links = ref<any[]>([])
const loading = ref(true)
const error = ref('')

async function load() {
  loading.value = true
  error.value = ''

  try {
    links.value = await apiRequest('/api/links', { token: authStore.token })
  } catch (err) {
    error.value = err instanceof Error ? err.message : 'Không thể tải danh sách link.'
  } finally {
    loading.value = false
  }
}

async function toggleStatus(link: any) {
  const path = link.status === 'Active' ? `/api/links/${link.id}/pause` : `/api/links/${link.id}/resume`

  try {
    await apiRequest(path, { method: 'PATCH', token: authStore.token })
    await load()
  } catch (err) {
    error.value = err instanceof Error ? err.message : 'Không thể cập nhật trạng thái.'
  }
}

onMounted(load)
</script>

<template>
  <section class="stack-lg">
    <div class="page-header">
      <div>
        <h2>Links</h2>
        <p class="muted">Quản lý shortlink và trạng thái redirect.</p>
      </div>
      <RouterLink class="primary-button" to="/app/links/create">Tạo link</RouterLink>
    </div>

    <p v-if="error" class="error-text">{{ error }}</p>

    <div v-if="loading" class="panel">Đang tải links...</div>

    <div v-else class="stack-sm">
      <article v-for="link in links" :key="link.id" class="panel link-card">
        <div class="link-card-header">
          <div>
            <strong>{{ link.shortUrl }}</strong>
            <p class="muted">{{ link.originalUrl }}</p>
          </div>
          <span class="badge">{{ link.status }}</span>
        </div>

        <div class="link-metrics">
          <span>Total {{ link.totalClicks }}</span>
          <span>Unique {{ link.uniqueClicks }}</span>
          <span>Bot {{ link.botClicks }}</span>
        </div>

        <div class="link-actions">
          <a class="secondary-button" :href="`http://localhost:5130/go/${link.slug}`" target="_blank" rel="noreferrer">
            Test Redirect
          </a>
          <button class="ghost-button" @click="toggleStatus(link)">
            {{ link.status === 'Active' ? 'Pause' : 'Resume' }}
          </button>
        </div>
      </article>
    </div>
  </section>
</template>

<script lang="ts">
import { RouterLink } from 'vue-router'
export default { components: { RouterLink } }
</script>
