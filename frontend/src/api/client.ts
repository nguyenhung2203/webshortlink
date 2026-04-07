const API_BASE_URL = import.meta.env.VITE_API_BASE_URL ?? 'http://localhost:5130'

type HttpMethod = 'GET' | 'POST' | 'PUT' | 'PATCH' | 'DELETE'

export interface ApiRequestOptions {
  method?: HttpMethod
  body?: unknown
  token?: string | null
  /** Skip the silent refresh retry on 401 (to avoid infinite loops) */
  _skipRefresh?: boolean
}

const STORAGE_KEY = 'webshortlink.auth'

/** 
 * Try to silently refresh the access token using the stored refresh token.
 * Returns the new access token, or null if refresh failed.
 */
async function tryRefreshToken(): Promise<string | null> {
  try {
    const raw = localStorage.getItem(STORAGE_KEY)
    if (!raw) return null

    const stored = JSON.parse(raw)
    const refreshToken = stored?.refreshToken
    if (!refreshToken) return null

    const response = await fetch(`${API_BASE_URL}/api/public/auth/refresh-token`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ refreshToken }),
    })
    if (!response.ok) {
      localStorage.removeItem(STORAGE_KEY)
      return null
    }

    const payload = await response.json()
    // Update stored token
    localStorage.setItem(STORAGE_KEY, JSON.stringify({
      ...stored,
      accessToken: payload.accessToken,
      refreshToken: payload.refreshToken,
      expiresAtUtc: payload.expiresAtUtc,
      user: payload.user ?? stored.user,
    }))
    // Dispatch event so Pinia store can sync
    window.dispatchEvent(new CustomEvent('auth:token-refreshed', { detail: payload }))
    return payload.accessToken as string
  } catch {
    localStorage.removeItem(STORAGE_KEY)
    return null
  }
}

export async function apiRequest<T>(path: string, options: ApiRequestOptions = {}): Promise<T> {
  const response = await fetch(`${API_BASE_URL}${path}`, {
    method: options.method ?? 'GET',
    headers: {
      'Content-Type': 'application/json',
      ...(options.token ? { Authorization: `Bearer ${options.token}` } : {}),
    },
    body: options.body ? JSON.stringify(options.body) : undefined,
  })

  if (!response.ok) {
    // Silent refresh on 401
    if (response.status === 401 && !options._skipRefresh) {
      const newToken = await tryRefreshToken()
      if (newToken) {
        // Retry original request with new token
        return apiRequest<T>(path, { ...options, token: newToken, _skipRefresh: true })
      }
      // Refresh failed — force logout by clearing storage and reloading
      localStorage.removeItem(STORAGE_KEY)
      window.dispatchEvent(new CustomEvent('auth:session-expired'))
      throw new Error('Phiên đăng nhập đã hết hạn. Vui lòng đăng nhập lại.')
    }
    const errorData = await response.json().catch(() => ({ message: 'Đã có lỗi xảy ra.' }))
    throw new Error(errorData.message || errorData.title || 'Đã có lỗi xảy ra.')
  }

  return response.json() as Promise<T>
}
