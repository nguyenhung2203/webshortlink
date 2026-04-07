import { apiRequest } from './client'
import type { 
  ShortLink, LinkDetail, CreateLinkRequest, DashboardMetrics, UserProfileProfile, 
  AdminDashboardDashboard, AdminUser, Plan, Subscription, AuthResult 
} from '@/types/api'

export const AuthService = {
  login: (email: string, password: string) => apiRequest<AuthResult>('/api/auth/login', { method: 'POST', body: { email, password } }),
  register: (fullName: string, email: string, password: string) => apiRequest<AuthResult>('/api/auth/register', { method: 'POST', body: { fullName, email, password } }),
}

export const LinkService = {
  list: (token: string) => apiRequest<ShortLink[]>('/api/links', { token }),
  detail: (token: string, id: string) => apiRequest<LinkDetail>(`/api/links/${id}`, { token }),
  create: (token: string, data: CreateLinkRequest) => apiRequest<ShortLink>('/api/links', { method: 'POST', body: data, token }),
  pause: (token: string, id: string) => apiRequest<ShortLink>(`/api/links/${id}/pause`, { method: 'PATCH', token }),
  resume: (token: string, id: string) => apiRequest<ShortLink>(`/api/links/${id}/resume`, { method: 'PATCH', token }),
  delete: (token: string, id: string) => apiRequest<{success: boolean}>(`/api/links/${id}`, { method: 'DELETE', token }),
}

export const UserService = {
  getDashboard: (token: string) => apiRequest<DashboardMetrics>('/api/analytics/overview', { token }),
  getProfile: (token: string) => apiRequest<UserProfileProfile>('/api/me/profile', { token }),
  updateProfile: (token: string, fullName: string) => apiRequest<UserProfileProfile>('/api/me/profile', { method: 'PUT', body: { fullName }, token }),
  getPlans: () => apiRequest<Plan[]>('/api/plans'),
  getSubscription: (token: string) => apiRequest<Subscription>('/api/subscription/current', { token }),
  upgradeSubscription: (token: string, planId: number) => apiRequest<Subscription>('/api/subscription/upgrade', { method: 'POST', body: { planId }, token })
}

export const AdminService = {
  // TODO: BLOCKED BY BACKEND - AdminController.cs is completely empty and exposes no routes.
  // These endpoints are currently stubs waiting for backend implementation.
  getDashboard: async (token: string) => { throw new Error('API bị khóa: AdminController chưa implement route /api/admin/dashboard'); return {} as AdminDashboardDashboard; },
  getUsers: async (token: string) => { throw new Error('API bị khóa: AdminController chưa implement route /api/admin/users'); return [] as AdminUser[]; },
  getLinks: async (token: string) => { throw new Error('API bị khóa: AdminController chưa implement route /api/admin/links'); return [] as ShortLink[]; },
  toggleUserLock: async (token: string, userId: string, action: 'lock'|'unlock') => { throw new Error(`API bị khóa: AdminController chưa implement action ${action} user`); return {} as AdminUser; },
  toggleLinkBlock: async (token: string, linkId: string, action: 'block'|'unblock') => { throw new Error(`API bị khóa: AdminController chưa implement action ${action} link`); return {} as ShortLink; }
}
