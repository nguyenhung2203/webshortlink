import { computed, ref } from 'vue'
import { defineStore } from 'pinia'
import { AuthService } from '@/api/services'
import type { AuthResponseDto } from '@/types/api'

const STORAGE_KEY = 'webshortlink.auth'

export const useAuthStore = defineStore('auth', () => {
  const accessToken = ref<string | null>(null)
  const refreshToken = ref<string | null>(null)
  const expiresAtUtc = ref<string | null>(null)
  const user = ref<AuthResponseDto['user'] | null>(null)

  const isAuthenticated = computed(() => Boolean(accessToken.value && user.value))
  const role = computed(() => user.value?.role ?? null)

  function restore() {
    const raw = localStorage.getItem(STORAGE_KEY)
    if (!raw) {
      return
    }

    try {
      const parsed = JSON.parse(raw) as AuthResponseDto
      accessToken.value = parsed.accessToken
      refreshToken.value = parsed.refreshToken
      expiresAtUtc.value = parsed.expiresAtUtc
      user.value = parsed.user
    } catch {
      localStorage.removeItem(STORAGE_KEY)
    }
  }

  function persist() {
    if (!accessToken.value || !user.value) {
      localStorage.removeItem(STORAGE_KEY)
      return
    }

    localStorage.setItem(STORAGE_KEY, JSON.stringify({
      accessToken: accessToken.value,
      refreshToken: refreshToken.value,
      expiresAtUtc: expiresAtUtc.value,
      user: user.value
    }))
  }

  async function login(email: string, password: string) {
    const payload = await AuthService.login(email, password)

    accessToken.value = payload.accessToken
    refreshToken.value = payload.refreshToken
    expiresAtUtc.value = payload.expiresAtUtc
    user.value = payload.user
    persist()
  }

  async function register(fullName: string, email: string, password: string, confirmPassword: string) {
    const payload = await AuthService.register(fullName, email, password, confirmPassword)
    return payload // returns MessageResponseDto
  }

  async function refreshSession() {
    if (!accessToken.value) return
    try {
      const sessionData = await AuthService.session(accessToken.value)
      if (user.value) {
        user.value = { ...user.value, ...sessionData }
        persist()
      }
    } catch {
      // Ignored
    }
  }

  async function logout() {
    if (accessToken.value) {
      try {
        await AuthService.logout(accessToken.value, refreshToken.value || undefined)
      } catch {
        // Ignore API failures on logout
      }
    }
    accessToken.value = null
    refreshToken.value = null
    expiresAtUtc.value = null
    user.value = null
    persist()
  }

  return {
    accessToken,
    refreshToken,
    expiresAtUtc,
    user,
    role,
    isAuthenticated,
    restore,
    login,
    register,
    logout,
    refreshSession,
  }
})
