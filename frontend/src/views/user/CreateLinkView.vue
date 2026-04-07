<script setup lang="ts">
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { apiRequest } from '@/api/client'
import { useAuthStore } from '@/stores/auth'

const router = useRouter()
const authStore = useAuthStore()

const form = ref({
  originalUrl: 'https://example.com/new-campaign',
  customSlug: '',
  tag: 'launch',
  expiredAtUtc: '',
  clickLimit: '',
  password: '',
})

const result = ref<any>(null)
const error = ref('')
const loading = ref(false)

async function submit() {
  error.value = ''
  loading.value = true

  try {
    result.value = await apiRequest('/api/links', {
      method: 'POST',
      token: authStore.token,
      body: {
        ...form.value,
        expiredAtUtc: form.value.expiredAtUtc || null,
        clickLimit: form.value.clickLimit ? Number(form.value.clickLimit) : null,
      },
    })
  } catch (err) {
    error.value = err instanceof Error ? err.message : 'Không thể tạo link.'
  } finally {
    loading.value = false
  }
}
</script>

<template>
  <section class="stack-lg">
    <div class="page-header">
      <div>
        <h2>Create Link</h2>
        <p class="muted">Tạo shortlink mới và cấu hình giới hạn cơ bản.</p>
      </div>
    </div>

    <form class="panel stack-sm" @submit.prevent="submit">
      <label class="field">
        <span>Original URL</span>
        <input v-model="form.originalUrl" type="url" required />
      </label>
      <label class="field">
        <span>Custom slug</span>
        <input v-model="form.customSlug" type="text" />
      </label>
      <label class="field">
        <span>Tag</span>
        <input v-model="form.tag" type="text" />
      </label>
      <label class="field">
        <span>Expiration (UTC)</span>
        <input v-model="form.expiredAtUtc" type="datetime-local" />
      </label>
      <label class="field">
        <span>Click limit</span>
        <input v-model="form.clickLimit" type="number" min="1" />
      </label>
      <label class="field">
        <span>Password</span>
        <input v-model="form.password" type="text" />
      </label>

      <p v-if="error" class="error-text">{{ error }}</p>

      <div class="inline-actions">
        <button class="primary-button" :disabled="loading" type="submit">
          {{ loading ? 'Đang tạo...' : 'Tạo shortlink' }}
        </button>
        <button class="ghost-button" type="button" @click="router.push('/app/links')">Về danh sách</button>
      </div>
    </form>

    <article v-if="result" class="panel success-panel">
      <h3>Tạo link thành công</h3>
      <p><strong>Short URL:</strong> {{ result.shortUrl }}</p>
      <p><strong>Slug:</strong> {{ result.slug }}</p>
      <p><strong>Status:</strong> {{ result.status }}</p>
    </article>
  </section>
</template>
