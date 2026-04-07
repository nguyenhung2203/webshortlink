<script setup lang="ts">
import { ref } from 'vue'
import { Book, Download, Component, Layout, Palette, Code } from 'lucide-vue-next'

const activeSection = ref('getting-started')

const navItems = [
  { key: 'getting-started', label: 'Getting Started', icon: Book },
  { key: 'installation', label: 'Installation', icon: Download },
  { key: 'components', label: 'Components', icon: Component },
  { key: 'layouts', label: 'Layouts', icon: Layout },
  { key: 'theming', label: 'Theming', icon: Palette },
  { key: 'api-reference', label: 'API Reference', icon: Code },
]

const components = [
  { name: 'WxAlert', description: 'Contextual feedback messages with variants' },
  { name: 'WxAvatar', description: 'User profile image or initials with status' },
  { name: 'WxBadge', description: 'Small count or label indicator' },
  { name: 'WxBreadcrumb', description: 'Navigation hierarchy trail' },
  { name: 'WxButton', description: 'Action trigger with multiple variants' },
  { name: 'WxCard', description: 'Content container with header/footer slots' },
  { name: 'WxCheckbox', description: 'Boolean toggle input with label' },
  { name: 'WxConfirmDialog', description: 'Confirmation modal for destructive actions' },
  { name: 'WxDataTable', description: 'Full-featured data table with sorting and selection' },
  { name: 'WxDatePicker', description: 'Calendar-based date selection input' },
  { name: 'WxDrawer', description: 'Slide-out side panel overlay' },
  { name: 'WxDropdown', description: 'Contextual menu triggered by click' },
  { name: 'WxEmptyState', description: 'Placeholder for empty content areas' },
  { name: 'WxFileUpload', description: 'Drag-and-drop file upload with preview' },
  { name: 'WxFilterBar', description: 'Composable filter controls row' },
  { name: 'WxInput', description: 'Text input with prefix/suffix slots' },
  { name: 'WxInputNumber', description: 'Numeric input with stepper controls' },
  { name: 'WxModal', description: 'Centered dialog overlay with backdrop' },
  { name: 'WxMultiSelect', description: 'Multi-value selection dropdown' },
  { name: 'WxPageHeader', description: 'Page title with description and actions' },
  { name: 'WxPagination', description: 'Page navigation with size selector' },
  { name: 'WxPasswordInput', description: 'Password field with visibility toggle' },
  { name: 'WxProgressBar', description: 'Linear progress indicator' },
  { name: 'WxRadio', description: 'Single-choice radio button group' },
  { name: 'WxSelect', description: 'Single-value selection dropdown' },
  { name: 'WxSkeleton', description: 'Loading placeholder animation' },
  { name: 'WxSlider', description: 'Range slider with value display' },
  { name: 'WxSpinner', description: 'Circular loading indicator' },
  { name: 'WxStatCard', description: 'Metric display card with trend' },
  { name: 'WxStepper', description: 'Multi-step progress indicator' },
  { name: 'WxSwitch', description: 'Toggle switch for on/off states' },
  { name: 'WxTabs', description: 'Tabbed navigation container' },
  { name: 'WxTag', description: 'Categorization label chip' },
  { name: 'WxTextarea', description: 'Multi-line text input' },
  { name: 'WxTimeline', description: 'Chronological event list' },
  { name: 'WxToast', description: 'Non-blocking notification popup' },
  { name: 'WxTreeView', description: 'Hierarchical expandable tree' },
]

function scrollTo(key: string) {
  activeSection.value = key
  document.getElementById(key)?.scrollIntoView({ behavior: 'smooth' })
}
</script>

<template>
  <div class="flex gap-8">
    <!-- Side navigation -->
    <nav class="hidden lg:block w-56 shrink-0">
      <div class="sticky top-6 space-y-1">
        <button
          v-for="item in navItems"
          :key="item.key"
          class="w-full flex items-center gap-2.5 px-3 py-2 rounded-lg text-sm font-medium transition-colors cursor-pointer text-left"
          :class="activeSection === item.key
            ? 'bg-primary/10 text-primary'
            : 'text-on-surface-variant hover:bg-surface-container hover:text-on-surface'"
          @click="scrollTo(item.key)"
        >
          <component :is="item.icon" :size="16" />
          {{ item.label }}
        </button>
      </div>
    </nav>

    <!-- Content area -->
    <div class="flex-1 min-w-0 max-w-3xl space-y-12">
      <!-- Getting Started -->
      <section id="getting-started">
        <h2 class="text-2xl font-bold text-on-surface tracking-tight">Getting Started</h2>
        <p class="mt-3 text-on-surface-variant leading-relaxed">
          The Wemake Admin Template is a production-ready Vue 3 dashboard built with TypeScript and Tailwind CSS.
          It provides a comprehensive set of UI components, layouts, and utilities to accelerate admin panel development.
        </p>

        <h3 class="text-lg font-semibold text-on-surface mt-6 mb-3">Prerequisites</h3>
        <ul class="space-y-2 text-sm text-on-surface-variant">
          <li class="flex items-start gap-2">
            <span class="w-1.5 h-1.5 rounded-full bg-primary mt-1.5 shrink-0" />
            Node.js 18.x or higher
          </li>
          <li class="flex items-start gap-2">
            <span class="w-1.5 h-1.5 rounded-full bg-primary mt-1.5 shrink-0" />
            npm 9.x or pnpm 8.x
          </li>
          <li class="flex items-start gap-2">
            <span class="w-1.5 h-1.5 rounded-full bg-primary mt-1.5 shrink-0" />
            Familiarity with Vue 3 Composition API
          </li>
          <li class="flex items-start gap-2">
            <span class="w-1.5 h-1.5 rounded-full bg-primary mt-1.5 shrink-0" />
            Basic knowledge of TypeScript and Tailwind CSS
          </li>
        </ul>
      </section>

      <!-- Installation -->
      <section id="installation">
        <h2 class="text-2xl font-bold text-on-surface tracking-tight">Installation</h2>
        <p class="mt-3 text-on-surface-variant leading-relaxed">
          Clone the repository and install dependencies to get up and running.
        </p>
        <div class="mt-4 rounded-lg bg-surface-container border border-outline-variant overflow-hidden">
          <div class="flex items-center justify-between px-4 py-2 border-b border-outline-variant bg-surface-container-high/50">
            <span class="text-xs font-medium text-on-surface-variant">Terminal</span>
          </div>
          <pre class="p-4 text-sm font-mono text-on-surface overflow-x-auto leading-relaxed"><code># Clone the repository
git clone https://github.com/wemake/vue-admin-template.git

# Navigate into the project
cd vue-admin-template

# Install dependencies
npm install

# Start the development server
npm run dev</code></pre>
        </div>
      </section>

      <!-- Quick Start -->
      <section id="quick-start">
        <h2 class="text-2xl font-bold text-on-surface tracking-tight">Quick Start</h2>
        <p class="mt-3 text-on-surface-variant leading-relaxed">
          Import and use any Wx component in your Vue single-file components.
        </p>
        <div class="mt-4 rounded-lg bg-surface-container border border-outline-variant overflow-hidden">
          <div class="flex items-center justify-between px-4 py-2 border-b border-outline-variant bg-surface-container-high/50">
            <span class="text-xs font-medium text-on-surface-variant">MyPage.vue</span>
          </div>
          <pre class="p-4 text-sm font-mono text-on-surface overflow-x-auto leading-relaxed"><code>&lt;script setup lang="ts"&gt;
import WxButton from '@/components/ui/WxButton.vue'
import WxCard from '@/components/ui/WxCard.vue'
&lt;/script&gt;

&lt;template&gt;
  &lt;WxCard&gt;
    &lt;template #header&gt;My Card&lt;/template&gt;
    &lt;p&gt;Card content goes here.&lt;/p&gt;
    &lt;template #footer&gt;
      &lt;WxButton variant="primary"&gt;Save&lt;/WxButton&gt;
    &lt;/template&gt;
  &lt;/WxCard&gt;
&lt;/template&gt;</code></pre>
        </div>
      </section>

      <!-- Components -->
      <section id="components">
        <h2 class="text-2xl font-bold text-on-surface tracking-tight">Components</h2>
        <p class="mt-3 text-on-surface-variant leading-relaxed">
          The template includes {{ components.length }} pre-built components covering inputs, feedback, navigation, layout, and data display.
        </p>
        <div class="mt-4 rounded-xl border border-outline-variant overflow-hidden">
          <table class="w-full text-sm">
            <thead>
              <tr class="bg-surface-container-high/50">
                <th class="text-left px-4 py-3 font-semibold text-on-surface">Component</th>
                <th class="text-left px-4 py-3 font-semibold text-on-surface">Description</th>
              </tr>
            </thead>
            <tbody>
              <tr
                v-for="(comp, i) in components"
                :key="comp.name"
                class="border-t border-outline-variant"
                :class="i % 2 === 0 ? 'bg-surface-container-lowest' : 'bg-surface-container-lowest/50'"
              >
                <td class="px-4 py-2.5">
                  <code class="text-xs font-mono font-semibold text-primary bg-primary/5 px-1.5 py-0.5 rounded">{{ comp.name }}</code>
                </td>
                <td class="px-4 py-2.5 text-on-surface-variant">{{ comp.description }}</td>
              </tr>
            </tbody>
          </table>
        </div>
      </section>
    </div>
  </div>
</template>
