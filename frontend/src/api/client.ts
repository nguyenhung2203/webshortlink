const API_BASE_URL = import.meta.env.VITE_API_BASE_URL ?? 'http://localhost:5130'

type HttpMethod = 'GET' | 'POST' | 'PUT' | 'PATCH' | 'DELETE'

export interface ApiRequestOptions {
  method?: HttpMethod
  body?: unknown
  token?: string | null
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
    if (response.status === 401) {
      localStorage.removeItem('webshortlink.auth')
      throw new Error('Phiên đăng nhập đã hết hạn. Vui lòng F5 tải lại trang để đăng nhập lại.')
    }
    const errorData = await response.json().catch(() => ({ message: 'Đã có lỗi xảy ra.' }))
    throw new Error(errorData.message || errorData.title || 'Đã có lỗi xảy ra.')
  }

  return response.json() as Promise<T>
}
