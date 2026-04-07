<script setup lang="ts">
import { ref } from 'vue'
import { useRoute } from 'vue-router'

interface NavChild {
  label: string
  to: string
}

interface NavGroup {
  key: string
  label: string
  children: NavChild[]
}

const route = useRoute()
const openDropdown = ref<string | null>(null)

const navGroups: NavGroup[] = [
  {
    key: 'dashboard',
    label: 'Dashboard',
    children: [
      { label: 'Dashboard v1', to: '/dashboard/v1' },
      { label: 'Dashboard v2', to: '/dashboard/v2' },
      { label: 'Dashboard v3', to: '/dashboard/v3' },
    ],
  },
  {
    key: 'widgets',
    label: 'Widgets',
    children: [{ label: 'Widgets', to: '/widgets' }],
  },
  {
    key: 'layout',
    label: 'Layout',
    children: [
      { label: 'Top Navigation', to: '/layout/top-nav' },
      { label: 'Boxed', to: '/layout/boxed' },
      { label: 'Fixed Sidebar', to: '/layout/fixed-sidebar' },
      { label: 'Collapsed Sidebar', to: '/layout/collapsed-sidebar' },
    ],
  },
  {
    key: 'charts',
    label: 'Charts',
    children: [{ label: 'Charts', to: '/charts' }],
  },
  {
    key: 'ui',
    label: 'UI Elements',
    children: [
      { label: 'General', to: '/ui/general' },
      { label: 'Icons', to: '/ui/icons' },
      { label: 'Buttons', to: '/ui/buttons' },
      { label: 'Sliders', to: '/ui/sliders' },
      { label: 'Modals & Alerts', to: '/ui/modals' },
      { label: 'Navbar & Tabs', to: '/ui/navbar-tabs' },
      { label: 'Timeline', to: '/ui/timeline' },
      { label: 'Ribbons', to: '/ui/ribbons' },
    ],
  },
  {
    key: 'forms',
    label: 'Forms',
    children: [
      { label: 'General', to: '/forms/general' },
      { label: 'Advanced', to: '/forms/advanced' },
      { label: 'Editors', to: '/forms/editors' },
      { label: 'Validation', to: '/forms/validation' },
    ],
  },
  {
    key: 'tables',
    label: 'Tables',
    children: [
      { label: 'Simple', to: '/tables/simple' },
      { label: 'Advanced', to: '/tables/advanced' },
    ],
  },
  {
    key: 'examples',
    label: 'Examples',
    children: [
      { label: 'Calendar', to: '/examples/calendar' },
      { label: 'Gallery', to: '/examples/gallery' },
      { label: 'Kanban Board', to: '/examples/kanban' },
    ],
  },
  {
    key: 'pages',
    label: 'Pages',
    children: [
      { label: 'Invoice', to: '/pages/invoice' },
      { label: 'Profile', to: '/pages/profile' },
      { label: 'E-commerce', to: '/pages/ecommerce' },
      { label: 'Projects', to: '/pages/projects' },
      { label: 'Contacts', to: '/pages/contacts' },
      { label: 'FAQ', to: '/pages/faq' },
    ],
  },
]

function isGroupActive(group: NavGroup): boolean {
  return group.children.some((child) => route.path === child.to)
}

function isChildActive(child: NavChild): boolean {
  return route.path === child.to
}

let closeTimeout: ReturnType<typeof setTimeout> | null = null

function handleMouseEnter(key: string) {
  if (closeTimeout) {
    clearTimeout(closeTimeout)
    closeTimeout = null
  }
  openDropdown.value = key
}

function handleMouseLeave() {
  closeTimeout = setTimeout(() => {
    openDropdown.value = null
  }, 150)
}
</script>

<template>
  <nav class="flex items-center gap-1">
    <div
      v-for="group in navGroups"
      :key="group.key"
      class="relative"
      @mouseenter="handleMouseEnter(group.key)"
      @mouseleave="handleMouseLeave"
    >
      <!-- Top-level item -->
      <button
        class="rounded-lg px-3 py-2 text-sm font-medium transition-colors"
        :class="
          isGroupActive(group)
            ? 'bg-white/20 text-white'
            : 'text-white/80 hover:bg-white/10 hover:text-white'
        "
      >
        {{ group.label }}
      </button>

      <!-- Dropdown -->
      <Transition name="dropdown">
        <div
          v-if="openDropdown === group.key"
          class="absolute left-0 top-full z-50 mt-1 min-w-[180px] overflow-hidden rounded-lg border border-outline bg-surface-container shadow-xl"
        >
          <RouterLink
            v-for="child in group.children"
            :key="child.to"
            :to="child.to"
            class="flex items-center px-4 py-2 text-sm transition-colors"
            :class="
              isChildActive(child)
                ? 'bg-primary/10 font-medium text-primary'
                : 'text-on-surface-variant hover:bg-surface-container-high hover:text-on-surface'
            "
            @click="openDropdown = null"
          >
            {{ child.label }}
          </RouterLink>
        </div>
      </Transition>
    </div>
  </nav>
</template>

<style scoped>
.dropdown-enter-active,
.dropdown-leave-active {
  transition: opacity 0.15s ease, transform 0.15s ease;
}
.dropdown-enter-from,
.dropdown-leave-to {
  opacity: 0;
  transform: translateY(-4px);
}
</style>
