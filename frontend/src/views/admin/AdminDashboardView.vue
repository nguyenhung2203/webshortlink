<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { apiRequest } from '@/api/client'
import { useAuthStore } from '@/stores/auth'

const authStore = useAuthStore()
const data = ref<any>(null)
const error = ref('')

async function load() {
  try {
    data.value = await apiRequest('/api/admin/dashboard', { token: authStore.token })
  } catch (err) {
    error.value = err instanceof Error ? err.message : 'Không thể tải admin dashboard.'
  }
}

onMounted(load)
</script>

<template>
  <section class="stack-lg">
    <div class="page-header">
      <div>
        <h2>Overview</h2>
        <p class="muted">Theo dõi business, product và operations trong cùng một màn hình.</p>
      </div>
    </div>

    <p v-if="error" class="error-text">{{ error }}</p>

    <template v-if="data">
      <div class="stats-grid">
        <article class="stat-card">
          <span>Total Users</span>
          <strong>{{ data.business.totalUsers }}</strong>
        </article>
        <article class="stat-card">
          <span>Paid Users</span>
          <strong>{{ data.business.paidUsers }}</strong>
        </article>
        <article class="stat-card">
          <span>Total Links</span>
          <strong>{{ data.product.totalLinks }}</strong>
        </article>
        <article class="stat-card">
          <span>Total Clicks</span>
          <strong>{{ data.product.totalClicks }}</strong>
        </article>
      </div>

      <div class="three-columns">
        <section class="panel">
          <h3>Business</h3>
          <p>Conversion: <strong>{{ data.business.conversionRate }}%</strong></p>
          <p>Revenue: <strong>{{ data.business.monthlyRevenue.toLocaleString('vi-VN') }}đ</strong></p>
        </section>
        <section class="panel">
          <h3>Product</h3>
          <p>Active links: <strong>{{ data.product.activeLinks }}</strong></p>
          <p>Unique clicks: <strong>{{ data.product.uniqueClicks }}</strong></p>
        </section>
        <section class="panel">
          <h3>Operations</h3>
          <p>Bot clicks: <strong>{{ data.operations.botClicks }}</strong></p>
          <p>Queue lag: <strong>{{ data.operations.queueLagSeconds }}s</strong></p>
          <p>Error rate: <strong>{{ data.operations.errorRate }}%</strong></p>
        </section>
      </div>
    </template>
  </section>
</template>
