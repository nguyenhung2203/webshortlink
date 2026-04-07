<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { apiRequest } from '@/api/client'
import { useAuthStore } from '@/stores/auth'

const authStore = useAuthStore()
const loading = ref(true)
const error = ref('')
const data = ref<any>(null)

async function load() {
  loading.value = true
  error.value = ''

  try {
    data.value = await apiRequest('/api/analytics/overview', { token: authStore.token })
  } catch (err) {
    error.value = err instanceof Error ? err.message : 'Không thể tải dashboard.'
  } finally {
    loading.value = false
  }
}

onMounted(load)
</script>

<template>
  <section class="stack-lg">
    <div class="page-header">
      <div>
        <h2>Dashboard</h2>
        <p class="muted">Theo dõi tổng quan toàn bộ shortlink của bạn.</p>
      </div>
    </div>

    <p v-if="error" class="error-text">{{ error }}</p>

    <div v-if="loading" class="panel">Đang tải dashboard...</div>

    <template v-else-if="data">
      <div class="stats-grid">
        <article class="stat-card">
          <span>Total Clicks</span>
          <strong>{{ data.metrics.totalClicks }}</strong>
        </article>
        <article class="stat-card">
          <span>Unique Clicks</span>
          <strong>{{ data.metrics.uniqueClicks }}</strong>
        </article>
        <article class="stat-card">
          <span>Bot Clicks</span>
          <strong>{{ data.metrics.botClicks }}</strong>
        </article>
        <article class="stat-card">
          <span>Active Links</span>
          <strong>{{ data.metrics.activeLinks }}</strong>
        </article>
      </div>

      <div class="two-columns">
        <section class="panel">
          <div class="section-title">
            <h3>Xu hướng click</h3>
            <span class="muted">7 ngày gần đây</span>
          </div>
          <div class="mini-bars">
            <div v-for="point in data.trend" :key="point.date" class="mini-bar-item">
              <div class="mini-bar-track">
                <div class="mini-bar-fill" :style="{ width: `${Math.max(16, point.totalClicks * 5)}px` }"></div>
              </div>
              <div class="mini-bar-meta">
                <span>{{ point.date }}</span>
                <strong>{{ point.totalClicks }}</strong>
              </div>
            </div>
          </div>
        </section>

        <section class="panel">
          <div class="section-title">
            <h3>Top links</h3>
          </div>
          <div class="stack-sm">
            <article v-for="link in data.topLinks" :key="link.id" class="list-row">
              <div>
                <strong>{{ link.shortUrl }}</strong>
                <p class="muted">{{ link.status }}</p>
              </div>
              <div class="text-right">
                <strong>{{ link.totalClicks }}</strong>
                <p class="muted">unique {{ link.uniqueClicks }}</p>
              </div>
            </article>
          </div>
        </section>
      </div>
    </template>
  </section>
</template>
