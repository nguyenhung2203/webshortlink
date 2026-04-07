// ─── Shared ────────────────────────────────────────────────────────────────────
export interface ApiError {
  message: string
}

export interface ApiMessageResponse {
  message: string
}

// ─── Auth ──────────────────────────────────────────────────────────────────────
export interface AuthResult {
  token: string
  user: {
    id: string
    fullName: string
    email: string
    role: string
    status: string
    currentPlanId: number
  }
}

// ─── User Profile ──────────────────────────────────────────────────────────────
export interface UserProfileProfile {
  id: string
  fullName: string
  email: string
  role: string
  status: string
  emailConfirmed: boolean
  createdAtUtc: string
  lastLoginAtUtc: string | null
  plan: {
    id: number
    code: string
    name: string
    linkLimit: number
    analyticsRetentionDays: number
  }
  usage: {
    linksUsed: number
    linksLimit: number
  }
}

// ─── Links ─────────────────────────────────────────────────────────────────────
/** Maps to LinkListItemDto from C# */
export interface ShortLink {
  id: string
  originalUrl: string
  shortUrl: string
  slug: string
  host: string
  status: string
  tag?: string | null
  description?: string | null
  totalClicks: number
  uniqueClicks: number
  botClicks: number
  createdAtUtc: string
  updatedAtUtc?: string | null
}

/** Maps to LinkDetailDto from C# */
export interface LinkDetail {
  id: string
  shortUrl: string
  slug: string
  host: string
  originalUrl: string
  status: string
  description?: string | null
  tag?: string | null
  expiresAtUtc?: string | null
  clickLimit?: number | null
  hasPassword: boolean
  totalClicks: number
  uniqueClicks: number
  createdAtUtc: string
  updatedAtUtc?: string | null
}

/** Maps to CreateLinkRequestDto from C# */
export interface CreateLinkRequest {
  originalUrl: string
  customSlug?: string | null
  domainId?: string | null
  description?: string | null
  tag?: string | null
  expiresAtUtc?: string | null
  clickLimit?: number | null
  password?: string | null
}

// ─── Analytics ─────────────────────────────────────────────────────────────────
/** Maps to TrendPointDto */
export interface TrendPoint {
  bucket: string
  totalClicks: number
  uniqueClicks: number
  botClicks: number
}

/** Maps to TopLinkDto */
export interface TopLink {
  linkId: string
  shortUrl: string
  totalClicks: number
  uniqueClicks: number
  status: string
}

/** Maps to AnalyticsOverviewDto */
export interface DashboardMetrics {
  totalClicks: number
  uniqueClicks: number
  botClicks: number
  activeLinks: number
  trends: TrendPoint[]
  topLinks: TopLink[]
}

/** Maps to KeyValueMetricDto */
export interface AnalyticsBreakdownItem {
  label: string
  value: number
}

/** Maps to LinkAnalyticsDto */
export interface LinkAnalytics {
  linkId: string
  totalClicks: number
  uniqueClicks: number
  botClicks: number
  trends: TrendPoint[]
  topCountries: AnalyticsBreakdownItem[]
  topDevices: AnalyticsBreakdownItem[]
  topReferrers: AnalyticsBreakdownItem[]
}

// ─── Plans & Billing ───────────────────────────────────────────────────────────
export interface Plan {
  id: number
  code?: string
  name: string
  monthlyPrice: number
  linkLimit: number
  analyticsRetentionDays: number
}

export interface Subscription {
  id: string
  status: string
  startAtUtc: string
  endAtUtc: string
  plan: Plan
}

export interface UpgradeSubscriptionResponse {
  message: string
  plan: Plan
}

// ─── Admin ─────────────────────────────────────────────────────────────────────
/** Maps to AdminOverviewDto */
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

/** Maps to AdminUserListItemDto */
export interface AdminUser {
  id: string
  email: string
  fullName: string
  role: string
  accountStatus: string
  planName: string
  linksCount: number
  totalClicks: number
  createdAtUtc: string
  lastLoginAtUtc?: string | null
}

/** Maps to AdminLinkListItemDto */
export interface AdminLink {
  id: string
  shortUrl: string
  slug: string
  originalUrl: string
  status: string
  ownerEmail: string
  totalClicks: number
  botClicks: number
  abuseScore: number
  createdAtUtc: string
}
