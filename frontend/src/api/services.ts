import { apiRequest } from './client'
import type {
  AdminDashboardDashboard,
  AdminUser,
  ApiMessageResponse,
  AuthResult,
  CreateLinkRequest,
  DashboardMetrics,
  LinkAnalytics,
  LinkDetail,
  LinkTimeseriesPoint,
  Plan,
  ShortLink,
  Subscription,
  UpgradeSubscriptionResponse,
  UserProfileProfile,
} from '@/types/api'

export const AuthService = {
  login: (email: string, password: string) =>
    apiRequest<AuthResult>('/api/auth/login', {
      method: 'POST',
      body: { email, password },
    }),
  register: (fullName: string, email: string, password: string) =>
    apiRequest<AuthResult>('/api/auth/register', {
      method: 'POST',
      body: { fullName, email, password },
    }),
}

export const LinkService = {
  list: (token: string) => apiRequest<ShortLink[]>('/api/links', { token }),
  detail: (token: string, id: string) =>
    apiRequest<LinkDetail>(`/api/links/${id}`, { token }),
  create: (token: string, data: CreateLinkRequest) =>
    apiRequest<ShortLink>('/api/links', { method: 'POST', body: data, token }),
  pause: (token: string, id: string) =>
    apiRequest<ShortLink>(`/api/links/${id}/pause`, { method: 'PATCH', token }),
  resume: (token: string, id: string) =>
    apiRequest<ShortLink>(`/api/links/${id}/resume`, { method: 'PATCH', token }),
  delete: (token: string, id: string) =>
    apiRequest<ApiMessageResponse>(`/api/links/${id}`, { method: 'DELETE', token }),
}

export const UserService = {
  getDashboard: (token: string) =>
    apiRequest<DashboardMetrics>('/api/analytics/overview', { token }),
  getLinkAnalytics: (token: string, id: string) =>
    apiRequest<LinkAnalytics>(`/api/analytics/links/${id}`, { token }),
  getLinkTimeseries: (token: string, id: string) =>
    apiRequest<LinkTimeseriesPoint[]>(`/api/analytics/links/${id}/timeseries`, { token }),
  getProfile: (token: string) =>
    apiRequest<UserProfileProfile>('/api/me/profile', { token }),
  updateProfile: (token: string, fullName: string) =>
    apiRequest<UserProfileProfile>('/api/me/profile', {
      method: 'PUT',
      body: { fullName },
      token,
    }),
  changePassword: (token: string, currentPassword: string, newPassword: string) =>
    apiRequest<ApiMessageResponse>('/api/me/password', {
      method: 'PUT',
      body: { currentPassword, newPassword },
      token,
    }),
  getPlans: () => apiRequest<Plan[]>('/api/plans'),
  getSubscription: (token: string) =>
    apiRequest<Subscription>('/api/subscription/current', { token }),
  upgradeSubscription: (token: string, planId: number) =>
    apiRequest<UpgradeSubscriptionResponse>('/api/subscription/upgrade', {
      method: 'POST',
      body: { planId },
      token,
    }),
}

function adminBlocked(message: string): never {
  throw new Error(message)
}

export const AdminService = {
  getDashboard: async (_token: string) =>
    adminBlocked('Admin API đang bị khóa: backend chưa implement route /api/admin/dashboard.'),
  getUsers: async (_token: string) =>
    adminBlocked('Admin API đang bị khóa: backend chưa implement route /api/admin/users.'),
  getLinks: async (_token: string) =>
    adminBlocked('Admin API đang bị khóa: backend chưa implement route /api/admin/links.'),
  toggleUserLock: async (_token: string, _userId: string, action: 'lock' | 'unlock') =>
    adminBlocked(`Admin API đang bị khóa: backend chưa implement action ${action} user.`),
  toggleLinkBlock: async (_token: string, _linkId: string, action: 'block' | 'unblock') =>
    adminBlocked(`Admin API đang bị khóa: backend chưa implement action ${action} link.`),
} as const
