# Phase 4: Implementation Plan

## Build Order

### Step 1: Project Scaffold
- Vite + Vue 3 + TypeScript project
- Tailwind CSS v4 setup
- Design tokens (tokens.css, base.css)
- Router skeleton
- Pinia setup

### Step 2: Layout Shell
- DefaultLayout (sidebar + topbar + content)
- AuthLayout (centered card)
- WxSideNav
- WxThemeToggle
- Theme store + composable
- Basic routing (all pages as stubs)

### Step 3: Primitive Components (foundation)
- WxButton, WxInput, WxPasswordInput, WxTextarea, WxInputNumber
- WxSelect, WxMultiSelect, WxDropdown
- WxSwitch, WxRadio, WxCheckbox, WxSlider
- WxTabs

### Step 4: Display Components
- WxAvatar, WxTag, WxBadge
- WxSpinner, WxProgressBar
- WxSkeleton, WxCard, WxEmptyState
- WxAlert, WxStatCard

### Step 5: Navigation & Feedback Components
- WxBreadcrumb, WxPagination
- WxPageHeader, WxFilterBar
- WxStepper, WxTimeline
- WxToast + useToast composable

### Step 6: Complex Components
- WxDataTable + useDataTable composable
- WxModal, WxConfirmDialog
- WxDrawer
- WxFileUpload
- WxDatePicker
- WxTreeView

### Step 7: Pages
- DashboardView (stat cards, CSS charts, activity)
- FormsView (all form components demo)
- TablesView (data table with sort/filter/pagination)
- FeedbackView (alerts, toasts, empty states)
- DialogsView (modals, drawers, confirm)
- FileUploadView
- SettingsView (toggle rows, option cards)
- TokensView (design system reference)
- Auth pages (login, register, forgot password)
- NotFoundView (404)

### Step 8: State Management & Routing Final
- Sidebar store
- Toast store
- UI store (modal/drawer)
- Route meta, breadcrumb mapping
- Page transitions

## Dependencies Between Steps
- Step 3 depends on Step 1 (tokens must exist for components)
- Step 5 depends on Step 3 (navigation uses buttons/inputs)
- Step 6 depends on Step 3+4 (complex components use primitives)
- Step 7 depends on Step 2+3+4+5+6 (pages use everything)
- Step 8 refines Step 2
