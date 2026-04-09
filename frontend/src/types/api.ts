// ─── Shared ────────────────────────────────────────────────────────────────────
export interface ApiError {
  message: string
}

export interface ApiMessageResponse {
  message: string
}

export interface MessageResponseDto {
  message: string
}

export interface RegisterResponseDto {
  message: string
  code?: string | null
  userId: string
  email: string
}

// ─── Auth ─────────────────────────────────────────────────────────────────────
export interface AuthResponseDto {
  accessToken: string
  refreshToken: string
  expiresAtUtc: string
  user: {
    id: string
    email: string
    fullName: string
    role: string
    currentPlanId: number
    accountStatus: string
  }
}

export interface CurrentSessionDto {
  userId: string
  email: string
  fullName: string
  role: string
  currentPlanId: number
  accountStatus: string
}

// ─── Profile ──────────────────────────────────────────────────────────────────
// Source: AppDataStore.GetCurrentUserProfile
// { Id, FullName, Email, Role, Status, EmailConfirmed, CreatedAtUtc, LastLoginAtUtc,
//   plan: { Id, Code, Name, LinkLimit, AnalyticsRetentionDays },
//   usage: { linksUsed, linksLimit } }
export interface UserProfileProfile {
  userId: string
  email: string
  fullName: string
  role: string
  currentPlanId: number
  accountStatus: string
}

// ─── Plans & Billing ──────────────────────────────────────────────────────────
export interface PlanFeature {
  title: string
  hint?: string
  isEnabled: boolean
}

export interface Plan {
  id: number
  code: string
  name: string
  monthlyPrice: number
  isActive: boolean
  features: PlanFeature[]
}

export interface Subscription {
  id: string
  planId: number
  planName: string
  status: string
  startAtUtc: string
  endAtUtc: string
}

export interface UpgradeSubscriptionResponse {
  subscriptionId: string
  planId: number
  planCode: string
  planName: string
  amountCharged: number
  currency: string
  startAtUtc: string
  endAtUtc: string
  message: string
}

// ─── Links ────────────────────────────────────────────────────────────────────
export interface ShortLink {
  id: string
  shortUrl: string
  slug: string
  host: string
  originalUrl: string
  status: string
  description?: string
  tag: string | null
  totalClicks: number
  uniqueClicks: number
  botClicks: number
  clicksToday: number
  createdAtUtc: string
  updatedAtUtc: string | null
}

export interface LinkDetail {
  id: string
  shortUrl: string
  slug: string
  host: string
  originalUrl: string
  status: string
  description: string | null
  tag: string | null
  expiresAtUtc: string | null
  clickLimit: number | null
  hasPassword: boolean
  totalClicks: number
  uniqueClicks: number
  createdAtUtc: string
  updatedAtUtc: string | null
  ogTitle?: string | null
  ogDescription?: string | null
  ogImageUrl?: string | null
}

export interface CreateLinkRequest {
  originalUrl: string
  customSlug?: string | null
  domainId?: number | null
  description?: string | null
  tag?: string | null
  expiresAtUtc?: string | null
  clickLimit?: number | null
  password?: string | null
  ogTitle?: string | null
  ogDescription?: string | null
  ogImageUrl?: string | null
}

// ─── Analytics ────────────────────────────────────────────────────────────────
export interface TrendPoint {
  bucket: string
  totalClicks: number
  uniqueClicks: number
  botClicks: number
}

export interface TopLink {
  linkId: string
  shortUrl: string
  totalClicks: number
  uniqueClicks: number
  status: string
}

export interface DashboardMetrics {
  totalClicks: number
  uniqueClicks: number
  botClicks: number
  activeLinks: number
  trends: TrendPoint[]
  topLinks: TopLink[]
}

export interface AnalyticsBreakdownItem {
  label: string
  value: number
}

export interface LinkAnalytics {
  linkId: string
  totalClicks: number
  uniqueClicks: number
  botClicks: number
  trends: TrendPoint[]
  topCountries: AnalyticsBreakdownItem[]
  topDevices: AnalyticsBreakdownItem[]
  topReferrers: AnalyticsBreakdownItem[]
  topCampaigns: AnalyticsBreakdownItem[]
}

// ─── Admin ──────────────────────────────────────────────────────────────────────
export interface AdminDashboardDashboard {
  business: {
    totalUsers: number
    paidUsers: number
    conversionRate: number
    monthlyRevenue: number
  }
  product: {
    totalLinks: number
    activeLinks: number
    totalClicks: number
    uniqueClicks: number
  }
  operations: {
    botClicks: number
    suspiciousClicks: number
    errorRate: number
    queueLagSeconds: number
    redirectLatencyP95Ms: number
  }
}

export interface AdminUser {
  id: string
  email: string
  fullName: string
  role: string
  status: string
  planName: string
  totalLinks: number
  totalClicks: number
  createdAtUtc: string
  lastLoginAtUtc: string | null
}

export interface AdminLink {
  id: string
  shortUrl: string
  slug: string
  originalUrl: string
  status: string
  userEmail: string
  totalClicks: number
  botClicks: number
  highestRiskScore: number | null
  createdAtUtc: string
}

export interface AdminAuditLog {
  id: string
  action: string
  resourceType: string | null
  resourceId: string | null
  actorType: string
  createdAtUtc: string
}
