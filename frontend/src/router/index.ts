import { createRouter, createWebHistory } from 'vue-router'
import { useAuthStore } from '@/stores/auth'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      component: () => import('@/layouts/PublicLayout.vue'),
      children: [
        { path: '', name: 'landing', component: () => import('@/views/LandingView.vue') },
        { path: 'pricing', name: 'pricing', component: () => import('@/views/public/PricingView.vue') },
        { path: 'link/:slug/unlock', name: 'link-unlock', component: () => import('@/views/public/LinkUnlockView.vue') },
        { path: 'link-not-found', name: 'link-not-found', component: () => import('@/views/public/LinkErrorView.vue'), props: { type: 'not-found' } },
        { path: 'link-expired', name: 'link-expired', component: () => import('@/views/public/LinkErrorView.vue'), props: { type: 'expired' } },
        { path: 'link-disabled', name: 'link-disabled', component: () => import('@/views/public/LinkErrorView.vue'), props: { type: 'disabled' } },
        { path: 'link-limit-reached', name: 'link-limit-reached', component: () => import('@/views/public/LinkErrorView.vue'), props: { type: 'limit-reached' } },
      ],
    },
    {
      path: '/auth',
      component: () => import('@/layouts/AuthLayout.vue'),
      children: [
        { path: 'login', name: 'login', component: () => import('@/views/auth/LoginView.vue'), meta: { guestOnly: true } },
        { path: 'admin', name: 'admin-login', component: () => import('@/views/auth/AdminLoginView.vue'), meta: { guestOnly: true } },
        { path: 'register', name: 'register', component: () => import('@/views/auth/RegisterView.vue'), meta: { guestOnly: true } },
        { path: 'verify-email', name: 'verify-email', component: () => import('@/views/auth/VerifyEmailView.vue'), meta: { guestOnly: true } },
        { path: 'forgot-password', name: 'forgot-password', component: () => import('@/views/auth/ForgotPasswordView.vue'), meta: { guestOnly: true } },
        { path: 'reset-password', name: 'reset-password', component: () => import('@/views/auth/ResetPasswordView.vue'), meta: { guestOnly: true } },
      ],
    },
    {
      path: '/app',
      component: () => import('@/layouts/UserLayout.vue'),
      meta: { requiresAuth: true, role: 'User' },
      children: [
        { path: 'dashboard', name: 'user-dashboard', component: () => import('@/views/user/UserDashboardView.vue') },
        { path: 'links', name: 'user-links', component: () => import('@/views/user/UserLinksView.vue') },
        { path: 'links/create', name: 'user-create-link', component: () => import('@/views/user/CreateLinkView.vue') },
        { path: 'links/:id', name: 'user-link-detail', component: () => import('@/views/user/LinkDetailView.vue') },
        { path: 'analytics', name: 'user-analytics', component: () => import('@/views/user/UserAnalyticsView.vue') },
        { path: 'billing', name: 'user-billing', component: () => import('@/views/user/UserBillingView.vue') },
        { path: 'payments', name: 'user-payments', component: () => import('@/views/user/UserPaymentHistoryView.vue') },
        { path: 'domains', name: 'user-domains', component: () => import('@/views/user/UserDomainsView.vue') },
        { path: 'profile', name: 'user-profile', component: () => import('@/views/user/UserProfileView.vue') },
      ],
    },
    {
      path: '/admin',
      component: () => import('@/layouts/AdminLayout.vue'),
      meta: { requiresAuth: true, role: 'Admin' },
      children: [
        { path: '', name: 'admin-dashboard', component: () => import('@/views/admin/AdminDashboardView.vue') },
        { path: 'users', name: 'admin-users', component: () => import('@/views/admin/AdminUsersView.vue') },
        { path: 'users/:id', name: 'admin-user-detail', component: () => import('@/views/admin/AdminUserDetailView.vue') },
        { path: 'links', name: 'admin-links', component: () => import('@/views/admin/AdminLinksView.vue') },
        { path: 'links/:id', name: 'admin-link-detail', component: () => import('@/views/admin/AdminLinkDetailView.vue') },
        { path: 'reports', name: 'admin-reports', component: () => import('@/views/admin/AdminReportsView.vue') },
        { path: 'security', name: 'admin-security', component: () => import('@/views/admin/AdminSecurityView.vue') },
        { path: 'audit', name: 'admin-audit', component: () => import('@/views/admin/AdminAuditView.vue') },
      ],
    },
  ],
})

router.beforeEach((to) => {
  const authStore = useAuthStore()

  if (to.meta.guestOnly && authStore.isAuthenticated) {
    return authStore.role === 'Admin' ? '/admin' : '/app/dashboard'
  }

  if (to.meta.requiresAuth && !authStore.isAuthenticated) {
    return to.path.startsWith('/admin') ? '/auth/admin' : '/auth/login'
  }

  if (to.meta.role && authStore.role !== to.meta.role) {
    return authStore.role === 'Admin' ? '/admin' : '/app/dashboard'
  }

  return true
})

export default router
