# Phase 2: Target Vue Architecture

## Directory Structure
```
src/
  assets/
    styles/
      tokens.css            # @theme + :root + .dark CSS variables
      base.css              # Global styles, glass effects, scrollbar
  components/
    ui/                     # 36 reusable Wx components
      WxButton.vue
      WxInput.vue
      WxPasswordInput.vue
      WxTextarea.vue
      WxInputNumber.vue
      WxSelect.vue
      WxMultiSelect.vue
      WxDropdown.vue
      WxSwitch.vue
      WxRadio.vue
      WxCheckbox.vue
      WxTabs.vue
      WxAvatar.vue
      WxTag.vue
      WxBadge.vue
      WxSpinner.vue
      WxProgressBar.vue
      WxPagination.vue
      WxSkeleton.vue
      WxCard.vue
      WxEmptyState.vue
      WxAlert.vue
      WxStatCard.vue
      WxFilterBar.vue
      WxBreadcrumb.vue
      WxStepper.vue
      WxPageHeader.vue
      WxFileUpload.vue
      WxDatePicker.vue
      WxSlider.vue
      WxDrawer.vue
      WxTimeline.vue
      WxTreeView.vue
      WxToast.vue
      WxModal.vue
      WxConfirmDialog.vue
      WxDataTable.vue
    layout/
      WxSideNav.vue
      WxThemeToggle.vue
  layouts/
    DefaultLayout.vue       # Sidebar + topbar + main content
    AuthLayout.vue          # Centered card, no sidebar
  views/
    DashboardView.vue
    FormsView.vue
    TablesView.vue
    FeedbackView.vue
    DialogsView.vue
    FileUploadView.vue
    SettingsView.vue
    TokensView.vue
    NotFoundView.vue
    auth/
      LoginView.vue
      RegisterView.vue
      ForgotPasswordView.vue
  composables/
    useToast.ts
    useModal.ts
    useTheme.ts
    useFormValidation.ts
    useDataTable.ts
  stores/
    theme.ts
    sidebar.ts
    toast.ts
    ui.ts
  router/
    index.ts
  types/
    index.ts                # Shared TypeScript types
  App.vue
  main.ts
```

## Layouts
- **DefaultLayout:** Sidebar (WxSideNav) on left, topbar with theme toggle + user avatar, scrollable main content area
- **AuthLayout:** Full-screen centered card, gradient background, no sidebar

## Views
Each view demonstrates the components in a real-product context, not a showcase dump.

## Component Architecture
- Composition API + `<script setup lang="ts">`
- Props via `defineProps<T>()` with TypeScript interfaces
- Emits via `defineEmits<T>()`
- Slots for content projection
- CSS via Tailwind utilities, no scoped CSS unless unavoidable

## State Management (Pinia)
- `theme.ts`: darkMode boolean, persist to localStorage
- `sidebar.ts`: isOpen, isCollapsed, activeItem
- `toast.ts`: toast queue array, add/remove actions
- `ui.ts`: modal/drawer visibility state
