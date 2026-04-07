export interface ApiError {
  message: string
}

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

export interface UserProfileProfile {
  id: string
  fullName: string
  email: string
  role: string
  status: string
  emailConfirmed: boolean
  createdAtUtc: string
  lastLoginAtUtc: string
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

export interface LinkDetail {
  summary: ShortLink
  settings: {
    password?: string | null
    expiredAtUtc?: string | null
    clickLimit?: number | null
  }
  permissions: {
    canUseCustomDomain: boolean
    canUseAdvancedAnalytics: boolean
    canUseApi: boolean
  }
}

export interface ShortLink {
  id: string
  originalUrl: string
  shortUrl: string
  slug: string
  status: string
  totalClicks: number
  uniqueClicks: number
  botClicks: number
  createdAtUtc: string
  owner?: string
  abuseScore?: number
}

export interface CreateLinkRequest {
  originalUrl: string
  customSlug?: string
  tag?: string
  expiredAtUtc?: string | null
  clickLimit?: number | null
  password?: string
}

export interface DashboardMetrics {
  metrics: {
    totalClicks: number
    uniqueClicks: number
    botClicks: number
    activeLinks: number
  }
  trend: {
    date: string
    totalClicks: number
  }[]
  topLinks: {
    id: string
    shortUrl: string
    status: string
    totalClicks: number
    uniqueClicks: number
  }[]
}

export interface AdminDashboardDashboard {
  business: {
    totalUsers: number
    paidUsers: number
    conversionRate: number
    monthlyRevenue: number
  }
  product: {
    totalLinks: number
    totalClicks: number
    activeLinks: number
    uniqueClicks: number
  }
  operations: {
    botClicks: number
    queueLagSeconds: number
    errorRate: number
  }
}

export interface AdminUser {
  id: string
  email: string
  fullName: string
  plan: string
  links: number
  totalClicks: number
  status: string
}

export interface Plan {
  id: number
  name: string
  monthlyPrice: number
  linkLimit: number
  analyticsRetentionDays: number
}

export interface Subscription {
  planId: number
  endAtUtc: string
  plan: Plan
}
