import { computed, ref } from 'vue'
import { defineStore } from 'pinia'
import { apiRequest } from '@/api/client'

interface AuthPayload {
  token: string
  user: {
    id: string
    fullName: string
    email: string
    role: 'User' | 'Admin'
    status: string
    currentPlanId: number
  }
}

const STORAGE_KEY = 'webshortlink.auth'

export const useAuthStore = defineStore('auth', () => {
  const token = ref<string | null>(null)
  const user = ref<AuthPayload['user'] | null>(null)

  const isAuthenticated = computed(() => Boolean(token.value && user.value))
  const role = computed(() => user.value?.role ?? null)

  function restore() {
    const raw = localStorage.getItem(STORAGE_KEY)
    if (!raw) {
      return
    }

    const parsed = JSON.parse(raw) as AuthPayload
    token.value = parsed.token
    user.value = parsed.user
  }

  function persist() {
    if (!token.value || !user.value) {
      localStorage.removeItem(STORAGE_KEY)
      return
    }

    localStorage.setItem(STORAGE_KEY, JSON.stringify({ token: token.value, user: user.value }))
  }

  async function login(email: string, password: string) {
    const payload = await apiRequest<AuthPayload>('/api/auth/login', {
      method: 'POST',
      body: { email, password },
    })

    token.value = payload.token
    user.value = payload.user
    persist()
  }

  async function register(fullName: string, email: string, password: string) {
    const payload = await apiRequest<AuthPayload>('/api/auth/register', {
      method: 'POST',
      body: { fullName, email, password },
    })

    token.value = payload.token
    user.value = payload.user
    persist()
  }

  function logout() {
    token.value = null
    user.value = null
    persist()
  }

  return {
    token,
    user,
    role,
    isAuthenticated,
    restore,
    login,
    register,
    logout,
  }
})
