<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { apiRequest } from '@/api/client'
import { useAuthStore } from '@/stores/auth'

const authStore = useAuthStore()
const links = ref<any[]>([])
const selectedLinkId = ref<string>('')
const analytics = ref<any>(null)
const error = ref('')

async function loadLinks() {
  links.value = await apiRequest('/api/links', { token: authStore.token })
  if (!selectedLinkId.value && links.value.length > 0) {
    selectedLinkId.value = links.value[0].id
  }
}

async function loadAnalytics() {
  if (!selectedLinkId.value) return
  analytics.value = await apiRequest(`/api/analytics/links/${selectedLinkId.value}`, { token: authStore.token })
}

async function bootstrap() {
  try {
    await loadLinks()
    await loadAnalytics()
  } catch (err) {
    error.value = err instanceof Error ? err.message : 'Không thể tải analytics.'
  }
}

onMounted(bootstrap)
</script>

<template>
  <section class="stack-lg">
    <div class="page-header">
      <div>
        <h2>Analytics</h2>
        <p class="muted">Xem chi tiết theo link, referrer, device và quốc gia.</p>
      </div>
    </div>

    <p v-if="error" class="error-text">{{ error }}</p>

    <label class="field panel">
      <span>Chọn link</span>
      <select v-model="selectedLinkId" @change="loadAnalytics">
        <option v-for="link in links" :key="link.id" :value="link.id">{{ link.shortUrl }}</option>
      </select>
    </label>

    <template v-if="analytics">
      <div class="stats-grid">
        <article class="stat-card">
          <span>Total Clicks</span>
          <strong>{{ analytics.metrics.totalClicks }}</strong>
        </article>
        <article class="stat-card">
          <span>Unique Clicks</span>
          <strong>{{ analytics.metrics.uniqueClicks }}</strong>
        </article>
        <article class="stat-card">
          <span>Bot Clicks</span>
          <strong>{{ analytics.metrics.botClicks }}</strong>
        </article>
      </div>

      <div class="three-columns">
        <section class="panel">
          <h3>Referrer</h3>
          <div class="stack-sm">
            <div v-for="item in analytics.sources" :key="item.label" class="list-row">
              <span>{{ item.label }}</span>
              <strong>{{ item.value }}</strong>
            </div>
          </div>
        </section>
        <section class="panel">
          <h3>Devices</h3>
          <div class="stack-sm">
            <div v-for="item in analytics.devices" :key="item.label" class="list-row">
              <span>{{ item.label }}</span>
              <strong>{{ item.value }}</strong>
            </div>
          </div>
        </section>
        <section class="panel">
          <h3>Countries</h3>
          <div class="stack-sm">
            <div v-for="item in analytics.countries" :key="item.label" class="list-row">
              <span>{{ item.label }}</span>
              <strong>{{ item.value }}</strong>
            </div>
          </div>
        </section>
      </div>
    </template>
  </section>
</template>
