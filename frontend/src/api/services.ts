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
  AdminAuditLog,
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
  forgotPassword: (email: string) =>
    apiRequest<MessageResponseDto>('/api/public/auth/forgot-password', {
      method: 'POST',
      body: { email, turnstileToken: null },
    }),
  resetPassword: (email: string, token: string, newPassword: string, confirmPassword: string) =>
    apiRequest<MessageResponseDto>('/api/public/auth/reset-password', {
      method: 'POST',
      body: { email, token, newPassword, confirmPassword },
    }),
  verifyEmail: (userId: string, token: string) =>
    apiRequest<MessageResponseDto>('/api/public/auth/verify-email', {
      method: 'POST',
      body: { userId, token },
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

export const LinkRuleService = {
  list: (token: string, linkId: string) =>
    apiRequest<any[]>(`/api/user/links/${linkId}/rules`, { token }),
  create: (token: string, linkId: string, data: {
    ruleType: string
    ruleValue: string
    targetUrl: string
    priority: number
    isActive: boolean
  }) =>
    apiRequest<any>(`/api/user/links/${linkId}/rules`, { method: 'POST', body: data, token }),
  update: (token: string, linkId: string, ruleId: string, data: {
    ruleType: string
    ruleValue: string
    targetUrl: string
    priority: number
    isActive: boolean
  }) =>
    apiRequest<any>(`/api/user/links/${linkId}/rules/${ruleId}`, { method: 'PUT', body: data, token }),
  delete: (token: string, linkId: string, ruleId: string) =>
    apiRequest<ApiMessageResponse>(`/api/user/links/${linkId}/rules/${ruleId}`, { method: 'DELETE', token }),
}

// ─── User / Analytics / Profile / Plans ────────────────────────────────────────
export const UserService = {
  getDashboard: (token: string) =>
    apiRequest<DashboardMetrics>('/api/user/analytics/overview', { token }),
  getLinkAnalytics: (token: string, id: string) =>
    apiRequest<LinkAnalytics>(`/api/user/analytics/links/${id}`, { token }),
  /** FIX-019: Download CSV export (Pro/Plus plans only) */
  exportLinkAnalyticsCsv: async (token: string, id: string): Promise<void> => {
    const API_BASE_URL = import.meta.env.VITE_API_BASE_URL ?? 'http://localhost:5130'
    const response = await fetch(`${API_BASE_URL}/api/user/analytics/links/${id}/export-csv`, {
      headers: { Authorization: `Bearer ${token}` },
    })
    if (!response.ok) {
      const err = await response.json().catch(() => ({ message: 'Lỗi xuất dữ liệu.' }))
      throw new Error(err.message || 'Lỗi xuất dữ liệu.')
    }
    const blob = await response.blob()
    const fileName = response.headers.get('Content-Disposition')?.match(/filename="?(.+)"?/)?.[1] ?? 'analytics.csv'
    const url = URL.createObjectURL(blob)
    const a = document.createElement('a')
    a.href = url
    a.download = fileName
    a.click()
    URL.revokeObjectURL(url)
  },
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
  getPaymentDetail: (token: string, paymentId: string) =>
    apiRequest<any>('/api/user/billing/payments/' + paymentId, { token }),
  upgradeSubscription: (token: string, planId: number) =>
    apiRequest<UpgradeSubscriptionResponse>('/api/user/billing/upgrade', {
      method: 'POST',
      body: { planId },
      token,
    }),
  getDomains: (token: string) =>
    apiRequest<any[]>('/api/user/domains', { token }),
}

export const DomainService = {
  list: (token: string) =>
    apiRequest<any[]>('/api/user/domains', { token }),
  create: (token: string, host: string) =>
    apiRequest<any>('/api/user/domains', { method: 'POST', body: { host }, token }),
  verify: (token: string, domainId: string, verificationToken: string) =>
    apiRequest<any>(`/api/user/domains/${domainId}/verify`, { method: 'POST', body: { verificationToken }, token }),
  delete: (token: string, domainId: string) =>
    apiRequest<ApiMessageResponse>(`/api/user/domains/${domainId}`, { method: 'DELETE', token }),
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
  getAuditLogs: (token: string) =>
    apiRequest<AdminAuditLog[]>('/api/admin/audit-logs', { token }),
  getUserDetail: (token: string, userId: string) =>
    apiRequest<any>(`/api/admin/users/${userId}`, { token }),
  changeUserPlan: (token: string, userId: string, planId: number) =>
    apiRequest<ApiMessageResponse>(`/api/admin/users/${userId}/plan`, {
      method: 'PATCH',
      body: { planId },
      token,
    }),
  getLinkDetail: (token: string, linkId: string) =>
    apiRequest<any>(`/api/admin/links/${linkId}`, { token }),
  getReports: (token: string) =>
    apiRequest<any>('/api/admin/reports', { token }),
  getSecurity: (token: string) =>
    apiRequest<any>('/api/admin/security', { token }),
  getPayments: (token: string) =>
    apiRequest<any[]>('/api/admin/payments', { token }),
  approvePayment: (token: string, paymentId: string) =>
    apiRequest<ApiMessageResponse>('/api/admin/payments/' + paymentId + '/approve', { method: 'PATCH', token }),
} as const
