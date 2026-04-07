import { apiRequest } from './client'
import type {
  AdminDashboardDashboard,
  AdminLink,
  AdminUser,
  ApiMessageResponse,
  AuthResult,
  CreateLinkRequest,
  DashboardMetrics,
  LinkAnalytics,
  LinkDetail,
  Plan,
  ShortLink,
  Subscription,
  UpgradeSubscriptionResponse,
  UserProfileProfile,
} from '@/types/api'

// ─── Auth ──────────────────────────────────────────────────────────────────────
export const AuthService = {
  login: (email: string, password: string) =>
    apiRequest<AuthResult>('/api/public/auth/login', {
      method: 'POST',
      body: { email, password },
    }),
  register: (fullName: string, email: string, password: string) =>
    apiRequest<AuthResult>('/api/public/auth/register', {
      method: 'POST',
      body: { fullName, email, password },
    }),
}

// ─── Links ─────────────────────────────────────────────────────────────────────
export const LinkService = {
  list: (token: string) =>
    apiRequest<ShortLink[]>('/api/user/links', { token }),
  detail: (token: string, id: string) =>
    apiRequest<LinkDetail>(`/api/user/links/${id}`, { token }),
  create: (token: string, data: CreateLinkRequest) =>
    apiRequest<ShortLink>('/api/user/links', { method: 'POST', body: data, token }),
  /** pause → PATCH /api/user/links/{id}/status  body: { isActive: false } */
  pause: (token: string, id: string) =>
    apiRequest<ShortLink>(`/api/user/links/${id}/status`, {
      method: 'PATCH',
      body: { isActive: false },
      token,
    }),
  /** resume → PATCH /api/user/links/{id}/status  body: { isActive: true } */
  resume: (token: string, id: string) =>
    apiRequest<ShortLink>(`/api/user/links/${id}/status`, {
      method: 'PATCH',
      body: { isActive: true },
      token,
    }),
  delete: (token: string, id: string) =>
    apiRequest<ApiMessageResponse>(`/api/user/links/${id}`, { method: 'DELETE', token }),
}

// ─── User ──────────────────────────────────────────────────────────────────────
export const UserService = {
  getDashboard: (token: string) =>
    apiRequest<DashboardMetrics>('/api/user/analytics/overview', { token }),
  getLinkAnalytics: (token: string, id: string) =>
    apiRequest<LinkAnalytics>(`/api/user/analytics/links/${id}`, { token }),
  // NOTE: timeseries is now embedded in LinkAnalytics.trends — no separate call needed
  getProfile: (token: string) =>
    apiRequest<UserProfileProfile>('/api/user/profile', { token }),
  updateProfile: (token: string, fullName: string) =>
    apiRequest<UserProfileProfile>('/api/user/profile', {
      method: 'PUT',
      body: { fullName },
      token,
    }),
  changePassword: (token: string, currentPassword: string, newPassword: string) =>
    apiRequest<ApiMessageResponse>('/api/user/security/change-password', {
      method: 'PUT',
      body: { currentPassword, newPassword },
      token,
    }),
  getPlans: () => apiRequest<Plan[]>('/api/user/plans'),
  getSubscription: (token: string) =>
    apiRequest<Subscription>('/api/user/subscription', { token }),
  upgradeSubscription: (token: string, planId: number) =>
    apiRequest<UpgradeSubscriptionResponse>('/api/user/billing/upgrade', {
      method: 'POST',
      body: { planId },
      token,
    }),
}

// ─── Admin ─────────────────────────────────────────────────────────────────────
export const AdminService = {
  getDashboard: (token: string) =>
    apiRequest<AdminDashboardDashboard>('/api/admin/overview', { token }),
  getUsers: (token: string) =>
    apiRequest<AdminUser[]>('/api/admin/users', { token }),
  getLinks: (token: string) =>
    apiRequest<AdminLink[]>('/api/admin/links', { token }),
  /** lock / unlock user via PATCH /api/admin/users/{id}/status */
  toggleUserLock: (token: string, userId: string, action: 'lock' | 'unlock') =>
    apiRequest<ApiMessageResponse>(`/api/admin/users/${userId}/status`, {
      method: 'PATCH',
      body: { status: action === 'lock' ? 'locked' : 'active' },
      token,
    }),
  /** block / unblock link via PATCH /api/admin/links/{id}/disable|enable */
  toggleLinkBlock: (token: string, linkId: string, action: 'block' | 'unblock') =>
    apiRequest<ApiMessageResponse>(
      `/api/admin/links/${linkId}/${action === 'block' ? 'disable' : 'enable'}`,
      { method: 'PATCH', token },
    ),
} as const
