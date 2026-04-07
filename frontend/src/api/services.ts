import { apiRequest } from './client'
import type {
  AdminDashboardDashboard,
  AdminLink,
  AdminUser,
  ApiMessageResponse,
  AuthResponseDto,
  MessageResponseDto,
  CurrentSessionDto,
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
    apiRequest<AuthResponseDto>('/api/public/auth/login', {
      method: 'POST',
      body: { email, password },
    }),
  register: (fullName: string, email: string, password: string, confirmPassword: string) =>
    apiRequest<MessageResponseDto>('/api/public/auth/register', {
      method: 'POST',
      body: { fullName, email, password, confirmPassword, turnstileToken: null },
    }),
  refreshToken: (refreshToken: string) =>
    apiRequest<AuthResponseDto>('/api/public/auth/refresh-token', {
      method: 'POST',
      body: { refreshToken },
    }),
  logout: (token: string, refreshToken?: string) =>
    apiRequest<void>('/api/public/auth/logout', {
      method: 'POST',
      body: { refreshToken },
      token,
    }),
  session: (token: string) =>
    apiRequest<CurrentSessionDto>('/api/public/auth/session', {
      method: 'GET',
      token,
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
  update: (token: string, id: string, data: Partial<CreateLinkRequest>) =>
    apiRequest<ShortLink>(`/api/user/links/${id}`, { method: 'PUT', body: data, token }),
  updateStatus: (token: string, id: string, isActive: boolean) =>
    apiRequest<ShortLink>(`/api/user/links/${id}/status`, { method: 'PATCH', body: { isActive }, token }),
  delete: (token: string, id: string) =>
    apiRequest<ApiMessageResponse>(`/api/user/links/${id}`, { method: 'DELETE', token }),
}

// ─── User / Analytics / Profile / Plans ────────────────────────────────────────
export const UserService = {
  getDashboard: (token: string) =>
    apiRequest<DashboardMetrics>('/api/user/analytics/overview', { token }),
  getLinkAnalytics: (token: string, id: string) =>
    apiRequest<LinkAnalytics>(`/api/user/analytics/links/${id}`, { token }),
  getProfile: (token: string) =>
    apiRequest<UserProfileProfile>('/api/user/profile', { token }),
  updateProfile: (token: string, fullName: string) =>
    apiRequest<UserProfileProfile>('/api/user/profile', {
      method: 'PUT',
      body: { fullName },
      token,
    }),
  changePassword: (token: string, currentPassword: string, newPassword: string, confirmPassword: string) =>
    apiRequest<ApiMessageResponse>('/api/user/security/change-password', {
      method: 'PUT',
      body: { currentPassword, newPassword, confirmPassword },
      token,
    }),
  getPlans: (token: string) =>
    apiRequest<Plan[]>('/api/user/plans', { token }),
  getSubscription: (token: string) =>
    apiRequest<Subscription>('/api/user/subscription', { token }),
  getPayments: (token: string) =>
    apiRequest<any[]>('/api/user/payments', { token }),
  upgradeSubscription: (token: string, planId: number) =>
    apiRequest<UpgradeSubscriptionResponse>('/api/user/billing/upgrade', {
      method: 'POST',
      body: { planId },
      token,
    }),
}

// ─── Admin ──────────────────────────────────────────────────────────────────────
export const AdminService = {
  getDashboard: (token: string) =>
    apiRequest<AdminDashboardDashboard>('/api/admin/overview', { token }),
  getUsers: (token: string) =>
    apiRequest<AdminUser[]>('/api/admin/users', { token }),
  getLinks: (token: string) =>
    apiRequest<AdminLink[]>('/api/admin/links', { token }),
  toggleUserLock: (token: string, userId: string, action: 'lock' | 'unlock') =>
    apiRequest<ApiMessageResponse>(`/api/admin/users/${userId}/status`, {
      method: 'PATCH',
      body: { status: action === 'lock' ? 'locked' : 'active' },
      token,
    }),
  toggleLinkBlock: (token: string, linkId: string, action: 'block' | 'unblock') =>
    apiRequest<ApiMessageResponse>(`/api/admin/links/${linkId}/${action === 'block' ? 'disable' : 'enable'}`, {
      method: 'PATCH',
      token,
    }),
} as const
