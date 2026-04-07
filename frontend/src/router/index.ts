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
        { path: 'login', name: 'login', component: () => import('@/views/auth/LoginView.vue'), meta: { guestOnly: true } },
        { path: 'register', name: 'register', component: () => import('@/views/auth/RegisterView.vue'), meta: { guestOnly: true } },
      ],
    },
    {
      path: '/app',
      component: () => import('@/layouts/UserShellLayout.vue'),
      meta: { requiresAuth: true, role: 'User' },
      children: [
        { path: 'dashboard', name: 'user-dashboard', component: () => import('@/views/user/UserDashboardView.vue') },
        { path: 'links', name: 'user-links', component: () => import('@/views/user/UserLinksView.vue') },
        { path: 'links/create', name: 'user-create-link', component: () => import('@/views/user/CreateLinkView.vue') },
        { path: 'analytics', name: 'user-analytics', component: () => import('@/views/user/UserAnalyticsView.vue') },
        { path: 'billing', name: 'user-billing', component: () => import('@/views/user/UserBillingView.vue') },
        { path: 'profile', name: 'user-profile', component: () => import('@/views/user/UserProfileView.vue') },
      ],
    },
    {
      path: '/admin',
      component: () => import('@/layouts/AdminShellLayout.vue'),
      meta: { requiresAuth: true, role: 'Admin' },
      children: [
        { path: '', name: 'admin-dashboard', component: () => import('@/views/admin/AdminDashboardView.vue') },
        { path: 'users', name: 'admin-users', component: () => import('@/views/admin/AdminUsersView.vue') },
        { path: 'links', name: 'admin-links', component: () => import('@/views/admin/AdminLinksView.vue') },
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
    return '/login'
  }

  if (to.meta.role && authStore.role !== to.meta.role) {
    return authStore.role === 'Admin' ? '/admin' : '/app/dashboard'
  }

  return true
})

export default router
