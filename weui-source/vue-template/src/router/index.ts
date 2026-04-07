import { createRouter, createWebHistory } from 'vue-router'

const DefaultLayout = () => import('@/layouts/DefaultLayout.vue')
const AuthLayout = () => import('@/layouts/AuthLayout.vue')
const BlankLayout = () => import('@/layouts/BlankLayout.vue')
const TopNavLayout = () => import('@/layouts/TopNavLayout.vue')

const router = createRouter({
  history: createWebHistory(),
  routes: [
    // Default Layout routes
    {
      path: '/',
      component: DefaultLayout,
      children: [
        { path: '', redirect: '/dashboard/v1' },
        // Dashboard
        { path: 'dashboard/v1', component: () => import('@/views/dashboard/DashboardV1.vue'), meta: { title: 'Dashboard v1', group: 'Dashboard' } },
        { path: 'dashboard/v2', component: () => import('@/views/dashboard/DashboardV2.vue'), meta: { title: 'Dashboard v2', group: 'Dashboard' } },
        { path: 'dashboard/v3', component: () => import('@/views/dashboard/DashboardV3.vue'), meta: { title: 'Dashboard v3', group: 'Dashboard' } },
        // Widgets
        { path: 'widgets', component: () => import('@/views/widgets/WidgetsView.vue'), meta: { title: 'Widgets' } },
        // Layout demos
        { path: 'layout/top-nav', component: () => import('@/views/layout/TopNavDemo.vue'), meta: { title: 'Top Navigation', group: 'Layout' } },
        { path: 'layout/boxed', component: () => import('@/views/layout/BoxedDemo.vue'), meta: { title: 'Boxed', group: 'Layout' } },
        { path: 'layout/fixed-sidebar', component: () => import('@/views/layout/FixedSidebarDemo.vue'), meta: { title: 'Fixed Sidebar', group: 'Layout' } },
        { path: 'layout/collapsed-sidebar', component: () => import('@/views/layout/CollapsedSidebarDemo.vue'), meta: { title: 'Collapsed Sidebar', group: 'Layout' } },
        // Charts
        { path: 'charts', component: () => import('@/views/charts/ChartsView.vue'), meta: { title: 'Charts' } },
        // UI Elements
        { path: 'ui/general', component: () => import('@/views/ui/GeneralView.vue'), meta: { title: 'General', group: 'UI Elements' } },
        { path: 'ui/icons', component: () => import('@/views/ui/IconsView.vue'), meta: { title: 'Icons', group: 'UI Elements' } },
        { path: 'ui/buttons', component: () => import('@/views/ui/ButtonsView.vue'), meta: { title: 'Buttons', group: 'UI Elements' } },
        { path: 'ui/sliders', component: () => import('@/views/ui/SlidersView.vue'), meta: { title: 'Sliders', group: 'UI Elements' } },
        { path: 'ui/modals', component: () => import('@/views/ui/ModalsView.vue'), meta: { title: 'Modals & Alerts', group: 'UI Elements' } },
        { path: 'ui/navbar-tabs', component: () => import('@/views/ui/NavbarTabsView.vue'), meta: { title: 'Navbar & Tabs', group: 'UI Elements' } },
        { path: 'ui/timeline', component: () => import('@/views/ui/TimelineView.vue'), meta: { title: 'Timeline', group: 'UI Elements' } },
        { path: 'ui/ribbons', component: () => import('@/views/ui/RibbonsView.vue'), meta: { title: 'Ribbons', group: 'UI Elements' } },
        // Forms
        { path: 'forms/general', component: () => import('@/views/forms/GeneralView.vue'), meta: { title: 'General Elements', group: 'Forms' } },
        { path: 'forms/advanced', component: () => import('@/views/forms/AdvancedView.vue'), meta: { title: 'Advanced Elements', group: 'Forms' } },
        { path: 'forms/editors', component: () => import('@/views/forms/EditorsView.vue'), meta: { title: 'Editors', group: 'Forms' } },
        { path: 'forms/validation', component: () => import('@/views/forms/ValidationView.vue'), meta: { title: 'Validation', group: 'Forms' } },
        // Tables
        { path: 'tables/simple', component: () => import('@/views/tables/SimpleView.vue'), meta: { title: 'Simple Tables', group: 'Tables' } },
        { path: 'tables/advanced', component: () => import('@/views/tables/AdvancedView.vue'), meta: { title: 'Advanced Tables', group: 'Tables' } },
        // Examples
        { path: 'examples/calendar', component: () => import('@/views/examples/CalendarView.vue'), meta: { title: 'Calendar', group: 'Examples' } },
        { path: 'examples/gallery', component: () => import('@/views/examples/GalleryView.vue'), meta: { title: 'Gallery', group: 'Examples' } },
        { path: 'examples/kanban', component: () => import('@/views/examples/KanbanView.vue'), meta: { title: 'Kanban Board', group: 'Examples' } },
        // Mailbox
        { path: 'mailbox/inbox', component: () => import('@/views/mailbox/InboxView.vue'), meta: { title: 'Inbox', group: 'Mailbox' } },
        { path: 'mailbox/compose', component: () => import('@/views/mailbox/ComposeView.vue'), meta: { title: 'Compose', group: 'Mailbox' } },
        { path: 'mailbox/read', component: () => import('@/views/mailbox/ReadView.vue'), meta: { title: 'Read Message', group: 'Mailbox' } },
        // Pages
        { path: 'pages/invoice', component: () => import('@/views/pages/InvoiceView.vue'), meta: { title: 'Invoice', group: 'Pages' } },
        { path: 'pages/profile', component: () => import('@/views/pages/ProfileView.vue'), meta: { title: 'Profile', group: 'Pages' } },
        { path: 'pages/ecommerce', component: () => import('@/views/pages/EcommerceView.vue'), meta: { title: 'E-commerce', group: 'Pages' } },
        { path: 'pages/projects', component: () => import('@/views/pages/ProjectsView.vue'), meta: { title: 'Projects', group: 'Pages' } },
        { path: 'pages/project-add', component: () => import('@/views/pages/ProjectAddView.vue'), meta: { title: 'Add Project', group: 'Pages' } },
        { path: 'pages/project-detail', component: () => import('@/views/pages/ProjectDetailView.vue'), meta: { title: 'Project Detail', group: 'Pages' } },
        { path: 'pages/contacts', component: () => import('@/views/pages/ContactsView.vue'), meta: { title: 'Contacts', group: 'Pages' } },
        { path: 'pages/faq', component: () => import('@/views/pages/FaqView.vue'), meta: { title: 'FAQ', group: 'Pages' } },
        // Search
        { path: 'search', component: () => import('@/views/search/SearchView.vue'), meta: { title: 'Search' } },
        // Misc
        { path: 'docs', component: () => import('@/views/misc/DocsView.vue'), meta: { title: 'Documentation' } },
        { path: 'starter', component: () => import('@/views/misc/StarterView.vue'), meta: { title: 'Starter Page' } },
        { path: 'blank', component: () => import('@/views/misc/BlankView.vue'), meta: { title: 'Blank Page' } },
      ],
    },
    // Auth Layout routes
    {
      path: '/auth',
      component: AuthLayout,
      children: [
        { path: 'login', component: () => import('@/views/auth/LoginView.vue'), meta: { title: 'Login' } },
        { path: 'register', component: () => import('@/views/auth/RegisterView.vue'), meta: { title: 'Register' } },
        { path: 'forgot-password', component: () => import('@/views/auth/ForgotPasswordView.vue'), meta: { title: 'Forgot Password' } },
        { path: 'recover-password', component: () => import('@/views/auth/RecoverPasswordView.vue'), meta: { title: 'Recover Password' } },
        { path: 'lockscreen', component: () => import('@/views/auth/LockscreenView.vue'), meta: { title: 'Lock Screen' } },
      ],
    },
    // Blank Layout routes (errors)
    {
      path: '/errors',
      component: BlankLayout,
      children: [
        { path: '404', component: () => import('@/views/errors/Error404View.vue'), meta: { title: '404 Not Found' } },
        { path: '500', component: () => import('@/views/errors/Error500View.vue'), meta: { title: '500 Server Error' } },
      ],
    },
    // Catch-all
    { path: '/:pathMatch(.*)*', redirect: '/errors/404' },
  ],
})

router.beforeEach((to) => {
  document.title = `${to.meta.title || 'Page'} | Wemake Admin`
})

export default router
