# Phase 7: State Management Plan

## Pinia Stores

### theme.ts
- `isDark: boolean` - persisted to localStorage, synced with .dark class
- `toggle()` - switches theme

### sidebar.ts
- `isOpen: boolean` (default true) - sidebar visibility
- `isCollapsed: boolean` (default false) - collapsed to icons-only
- `activeItem: string` (default 'dashboard') - current nav item
- `toggle()`, `collapse()`, `setActive(key)`

### toast.ts
- `toasts: ToastItem[]` - queue of active toasts
- `add(type, title, message)` - adds toast, auto-removes after 5s
- `remove(id)` - manual dismiss
- Max 3 visible at once

### ui.ts
- `modalOpen: boolean`, `drawerOpen: boolean`
- Open/close actions for each

## Router Structure
| Path | View | Layout | Meta Title |
|------|------|--------|------------|
| / | DashboardView | Default | Dashboard |
| /forms | FormsView | Default | Forms |
| /tables | TablesView | Default | Tables |
| /feedback | FeedbackView | Default | Feedback |
| /dialogs | DialogsView | Default | Dialogs |
| /file-upload | FileUploadView | Default | File Upload |
| /settings | SettingsView | Default | Settings |
| /tokens | TokensView | Default | Design System |
| /auth/login | LoginView | Auth | Login |
| /auth/register | RegisterView | Auth | Register |
| /auth/forgot-password | ForgotPasswordView | Auth | Forgot Password |
| /:pathMatch(.*)* | NotFoundView | Default | Not Found |

## Composables
- `useTheme()` - wraps theme store
- `useToast()` - wraps toast store with convenience methods
- `useModal()` - wraps ui store for modal/drawer
- `useDataTable()` - generic sort/filter for tables
- `useFormValidation()` - rule-based form validation
