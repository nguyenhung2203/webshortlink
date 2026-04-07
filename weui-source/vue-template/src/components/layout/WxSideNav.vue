<script setup lang="ts">
import { computed, watch } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useSidebarStore } from '@/stores/sidebar'
import {
  Zap,
  LayoutDashboard,
  Box,
  Layout,
  BarChart3,
  Palette,
  FileText,
  Table,
  Layers,
  Mail,
  File,
  Lock,
  AlertCircle,
  Search,
  BookOpen,
  Rocket,
  FileIcon,
  ChevronDown,
} from 'lucide-vue-next'

defineProps<{
  collapsed?: boolean
}>()

const route = useRoute()
const router = useRouter()
const sidebar = useSidebarStore()

const iconMap: Record<string, any> = {
  LayoutDashboard,
  Box,
  Layout,
  BarChart3,
  Palette,
  FileText,
  Table,
  Layers,
  Mail,
  File,
  Lock,
  AlertCircle,
  Search,
  BookOpen,
  Rocket,
  FileIcon,
}

interface NavChild {
  key: string
  label: string
  to: string
}

interface NavItem {
  key: string
  label: string
  icon: string
  to?: string
  children?: NavChild[]
  badge?: number | string
  separator?: boolean
}

const navItems: NavItem[] = [
  {
    key: 'dashboard',
    label: 'Dashboard',
    icon: 'LayoutDashboard',
    children: [
      { key: 'dashboard-v1', label: 'Dashboard v1', to: '/dashboard/v1' },
      { key: 'dashboard-v2', label: 'Dashboard v2', to: '/dashboard/v2' },
      { key: 'dashboard-v3', label: 'Dashboard v3', to: '/dashboard/v3' },
    ],
  },
  { key: 'widgets', label: 'Widgets', icon: 'Box', to: '/widgets' },
  {
    key: 'layout',
    label: 'Layout Options',
    icon: 'Layout',
    children: [
      { key: 'layout-top-nav', label: 'Top Navigation', to: '/layout/top-nav' },
      { key: 'layout-boxed', label: 'Boxed', to: '/layout/boxed' },
      { key: 'layout-fixed-sidebar', label: 'Fixed Sidebar', to: '/layout/fixed-sidebar' },
      { key: 'layout-collapsed-sidebar', label: 'Collapsed Sidebar', to: '/layout/collapsed-sidebar' },
    ],
  },
  { key: 'charts', label: 'Charts', icon: 'BarChart3', to: '/charts' },
  {
    key: 'ui',
    label: 'UI Elements',
    icon: 'Palette',
    children: [
      { key: 'ui-general', label: 'General', to: '/ui/general' },
      { key: 'ui-icons', label: 'Icons', to: '/ui/icons' },
      { key: 'ui-buttons', label: 'Buttons', to: '/ui/buttons' },
      { key: 'ui-sliders', label: 'Sliders', to: '/ui/sliders' },
      { key: 'ui-modals', label: 'Modals & Alerts', to: '/ui/modals' },
      { key: 'ui-navbar-tabs', label: 'Navbar & Tabs', to: '/ui/navbar-tabs' },
      { key: 'ui-timeline', label: 'Timeline', to: '/ui/timeline' },
      { key: 'ui-ribbons', label: 'Ribbons', to: '/ui/ribbons' },
    ],
  },
  {
    key: 'forms',
    label: 'Forms',
    icon: 'FileText',
    children: [
      { key: 'forms-general', label: 'General', to: '/forms/general' },
      { key: 'forms-advanced', label: 'Advanced', to: '/forms/advanced' },
      { key: 'forms-editors', label: 'Editors', to: '/forms/editors' },
      { key: 'forms-validation', label: 'Validation', to: '/forms/validation' },
    ],
  },
  {
    key: 'tables',
    label: 'Tables',
    icon: 'Table',
    children: [
      { key: 'tables-simple', label: 'Simple', to: '/tables/simple' },
      { key: 'tables-advanced', label: 'Advanced', to: '/tables/advanced' },
    ],
  },
  {
    key: 'examples',
    label: 'Examples',
    icon: 'Layers',
    children: [
      { key: 'examples-calendar', label: 'Calendar', to: '/examples/calendar' },
      { key: 'examples-gallery', label: 'Gallery', to: '/examples/gallery' },
      { key: 'examples-kanban', label: 'Kanban Board', to: '/examples/kanban' },
    ],
  },
  {
    key: 'mailbox',
    label: 'Mailbox',
    icon: 'Mail',
    badge: 3,
    children: [
      { key: 'mailbox-inbox', label: 'Inbox', to: '/mailbox/inbox' },
      { key: 'mailbox-compose', label: 'Compose', to: '/mailbox/compose' },
      { key: 'mailbox-read', label: 'Read', to: '/mailbox/read' },
    ],
  },
  {
    key: 'pages',
    label: 'Pages',
    icon: 'File',
    children: [
      { key: 'pages-invoice', label: 'Invoice', to: '/pages/invoice' },
      { key: 'pages-profile', label: 'Profile', to: '/pages/profile' },
      { key: 'pages-ecommerce', label: 'E-commerce', to: '/pages/ecommerce' },
      { key: 'pages-projects', label: 'Projects', to: '/pages/projects' },
      { key: 'pages-contacts', label: 'Contacts', to: '/pages/contacts' },
      { key: 'pages-faq', label: 'FAQ', to: '/pages/faq' },
    ],
  },
  { key: 'sep-1', label: '', icon: '', separator: true },
  {
    key: 'auth',
    label: 'Auth Pages',
    icon: 'Lock',
    children: [
      { key: 'auth-login', label: 'Login', to: '/auth/login' },
      { key: 'auth-register', label: 'Register', to: '/auth/register' },
      { key: 'auth-forgot', label: 'Forgot Password', to: '/auth/forgot-password' },
      { key: 'auth-recover', label: 'Recover Password', to: '/auth/recover-password' },
      { key: 'auth-lock', label: 'Lock Screen', to: '/auth/lockscreen' },
    ],
  },
  {
    key: 'errors',
    label: 'Errors',
    icon: 'AlertCircle',
    children: [
      { key: 'errors-404', label: '404 Page', to: '/errors/404' },
      { key: 'errors-500', label: '500 Page', to: '/errors/500' },
    ],
  },
  { key: 'sep-2', label: '', icon: '', separator: true },
  { key: 'search', label: 'Search', icon: 'Search', to: '/search' },
  { key: 'docs', label: 'Documentation', icon: 'BookOpen', to: '/docs' },
  { key: 'starter', label: 'Starter Page', icon: 'Rocket', to: '/starter' },
  { key: 'blank-page', label: 'Blank Page', icon: 'FileIcon', to: '/blank' },
]

function isChildActive(item: NavItem): boolean {
  if (!item.children) return false
  return item.children.some((child) => route.path === child.to)
}

function isItemActive(item: NavItem): boolean {
  if (item.to) return route.path === item.to
  return isChildActive(item)
}

function isChildRouteActive(child: NavChild): boolean {
  return route.path === child.to
}

function handleItemClick(item: NavItem) {
  if (item.children) {
    sidebar.toggleExpand(item.key)
  } else if (item.to) {
    router.push(item.to)
    sidebar.setActive(item.key)
  }
}

function handleChildClick(child: NavChild, parentKey: string) {
  router.push(child.to)
  sidebar.setActive(parentKey)
}

// Auto-expand parent of active route on navigation
watch(
  () => route.path,
  () => {
    for (const item of navItems) {
      if (item.children && isChildActive(item)) {
        sidebar.ensureExpanded(item.key)
        break
      }
    }
  },
  { immediate: true },
)

const getIcon = (iconName: string) => iconMap[iconName] || Box
</script>

<template>
  <nav
    class="flex h-full flex-col bg-surface-container transition-all duration-300"
  >
    <!-- Logo -->
    <div
      class="flex h-14 shrink-0 items-center border-b border-outline px-4"
      :class="collapsed ? 'justify-center' : 'gap-3'"
    >
      <div
        class="flex h-8 w-8 shrink-0 items-center justify-center rounded-lg bg-primary/15"
      >
        <Zap :size="18" class="text-primary" />
      </div>
      <Transition name="label">
        <span
          v-if="!collapsed"
          class="text-base font-bold tracking-wider text-on-surface"
        >
          WEMAKE
        </span>
      </Transition>
    </div>

    <!-- Nav items -->
    <div class="flex-1 overflow-y-auto px-3 py-4">
      <ul class="flex flex-col gap-0.5">
        <template v-for="item in navItems" :key="item.key">
          <!-- Separator -->
          <li v-if="item.separator" class="my-2 border-t border-outline/50" />

          <!-- Nav item with children -->
          <li v-else-if="item.children">
            <button
              class="group flex w-full items-center rounded-lg px-3 py-2.5 text-sm font-medium transition-all duration-200"
              :class="[
                collapsed ? 'justify-center' : 'gap-3',
                isItemActive(item)
                  ? 'bg-primary/10 text-primary'
                  : 'text-on-surface-variant hover:bg-surface-container-high hover:text-on-surface',
              ]"
              :title="collapsed ? item.label : undefined"
              @click="handleItemClick(item)"
            >
              <component
                :is="getIcon(item.icon)"
                :size="20"
                class="shrink-0 transition-colors"
                :class="
                  isItemActive(item)
                    ? 'text-primary'
                    : 'text-on-surface-variant group-hover:text-on-surface'
                "
              />
              <Transition name="label">
                <span v-if="!collapsed" class="flex-1 text-left">{{ item.label }}</span>
              </Transition>

              <!-- Badge -->
              <span
                v-if="item.badge && !collapsed"
                class="ml-auto mr-2 rounded-full bg-danger px-2 py-0.5 text-xs font-semibold text-white"
              >
                {{ item.badge }}
              </span>

              <!-- Chevron -->
              <ChevronDown
                v-if="!collapsed"
                :size="16"
                class="shrink-0 transition-transform duration-200"
                :class="sidebar.isExpanded(item.key) ? 'rotate-180' : ''"
              />
            </button>

            <!-- Children -->
            <Transition name="expand">
              <ul
                v-if="!collapsed && sidebar.isExpanded(item.key)"
                class="mt-0.5 flex flex-col gap-0.5 overflow-hidden"
              >
                <li v-for="child in item.children" :key="child.key">
                  <RouterLink
                    :to="child.to"
                    class="flex items-center rounded-lg py-2 pl-11 pr-3 text-sm transition-all duration-200"
                    :class="
                      isChildRouteActive(child)
                        ? 'bg-primary/8 font-medium text-primary'
                        : 'text-on-surface-variant hover:bg-surface-container-high hover:text-on-surface'
                    "
                    @click="handleChildClick(child, item.key)"
                  >
                    <span
                      class="mr-3 h-1.5 w-1.5 rounded-full transition-colors"
                      :class="
                        isChildRouteActive(child)
                          ? 'bg-primary'
                          : 'bg-on-surface-variant/40'
                      "
                    />
                    {{ child.label }}
                  </RouterLink>
                </li>
              </ul>
            </Transition>
          </li>

          <!-- Simple nav item (no children) -->
          <li v-else>
            <RouterLink
              :to="item.to!"
              class="group flex items-center rounded-lg px-3 py-2.5 text-sm font-medium transition-all duration-200"
              :class="[
                collapsed ? 'justify-center' : 'gap-3',
                isItemActive(item)
                  ? 'bg-primary/10 text-primary'
                  : 'text-on-surface-variant hover:bg-surface-container-high hover:text-on-surface',
              ]"
              :title="collapsed ? item.label : undefined"
              @click="sidebar.setActive(item.key)"
            >
              <component
                :is="getIcon(item.icon)"
                :size="20"
                class="shrink-0 transition-colors"
                :class="
                  isItemActive(item)
                    ? 'text-primary'
                    : 'text-on-surface-variant group-hover:text-on-surface'
                "
              />
              <Transition name="label">
                <span v-if="!collapsed">{{ item.label }}</span>
              </Transition>

              <!-- Active indicator -->
              <div
                v-if="isItemActive(item) && !collapsed"
                class="ml-auto h-1.5 w-1.5 rounded-full bg-primary"
              />
            </RouterLink>
          </li>
        </template>
      </ul>
    </div>

    <!-- User section -->
    <div class="shrink-0 border-t border-outline p-3">
      <div
        class="flex items-center rounded-lg px-3 py-2"
        :class="collapsed ? 'justify-center' : 'gap-3'"
      >
        <div
          class="flex h-8 w-8 shrink-0 items-center justify-center rounded-full bg-primary/15 text-sm font-semibold text-primary"
        >
          W
        </div>
        <Transition name="label">
          <div v-if="!collapsed" class="min-w-0">
            <p class="truncate text-sm font-medium text-on-surface">Admin User</p>
            <p class="truncate text-xs text-on-surface-variant">admin@wemake.com</p>
          </div>
        </Transition>
      </div>
    </div>
  </nav>
</template>

<style scoped>
.label-enter-active,
.label-leave-active {
  transition: opacity 0.15s ease, transform 0.15s ease;
}
.label-enter-from,
.label-leave-to {
  opacity: 0;
  transform: translateX(-4px);
}

.expand-enter-active,
.expand-leave-active {
  transition: all 0.2s ease;
  max-height: 500px;
}
.expand-enter-from,
.expand-leave-to {
  max-height: 0;
  opacity: 0;
}
</style>
