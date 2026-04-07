# Architecture

## Directory Structure (expanded)
```
src/
  layouts/
    DefaultLayout.vue      # Sidebar + topbar (existing, enhanced)
    TopNavLayout.vue        # Horizontal nav, no sidebar
    AuthLayout.vue          # Centered card (existing)
    BlankLayout.vue         # No chrome
  components/
    layout/
      WxSideNav.vue         # Enhanced with multi-level menu
      WxThemeToggle.vue     # (existing)
      WxFooter.vue          # NEW
      WxRightSidePanel.vue  # NEW - control sidebar
      WxNotificationDropdown.vue  # NEW
      WxMessageDropdown.vue       # NEW
      WxUserMenu.vue              # NEW
      WxTopNav.vue                # NEW - horizontal nav bar
    ui/
      ... (37 existing Wx components)
  views/
    dashboard/
      DashboardV1.vue
      DashboardV2.vue
      DashboardV3.vue
    widgets/
      WidgetsView.vue
    layout/
      TopNavDemo.vue
      BoxedDemo.vue
      FixedSidebarDemo.vue
      CollapsedSidebarDemo.vue
    charts/
      ChartsView.vue
    ui/
      GeneralView.vue
      IconsView.vue
      ButtonsView.vue
      SlidersView.vue
      ModalsView.vue
      NavbarTabsView.vue
      TimelineView.vue
      RibbonsView.vue
    forms/
      GeneralView.vue
      AdvancedView.vue
      EditorsView.vue
      ValidationView.vue
    tables/
      SimpleView.vue
      AdvancedView.vue
    examples/
      CalendarView.vue
      GalleryView.vue
      KanbanView.vue
    mailbox/
      InboxView.vue
      ComposeView.vue
      ReadView.vue
    pages/
      InvoiceView.vue
      ProfileView.vue
      EcommerceView.vue
      ProjectsView.vue
      ProjectAddView.vue
      ProjectDetailView.vue
      ContactsView.vue
      FaqView.vue
    auth/
      LoginView.vue          # (existing)
      RegisterView.vue       # (existing)
      ForgotPasswordView.vue # (existing)
      RecoverPasswordView.vue
      LockscreenView.vue
    errors/
      Error404View.vue
      Error500View.vue
    search/
      SearchView.vue
    misc/
      DocsView.vue
      StarterView.vue
      BlankView.vue
  stores/
    sidebar.ts    # Enhanced with nested menu support
    theme.ts      # (existing)
    toast.ts      # (existing)
    ui.ts         # Enhanced with right panel state
  router/
    index.ts      # Expanded ~55 routes
```

## Sidebar Navigation Structure
```
Dashboard
  ├── Dashboard v1
  ├── Dashboard v2
  └── Dashboard v3
Widgets
Layout Options
  ├── Top Navigation
  ├── Boxed
  ├── Fixed Sidebar
  └── Collapsed Sidebar
Charts
UI Elements
  ├── General
  ├── Icons
  ├── Buttons
  ├── Sliders
  ├── Modals & Alerts
  ├── Navbar & Tabs
  ├── Timeline
  └── Ribbons
Forms
  ├── General Elements
  ├── Advanced Elements
  ├── Editors
  └── Validation
Tables
  ├── Simple Tables
  └── Advanced Tables
Examples
  ├── Calendar
  ├── Gallery
  └── Kanban Board
Mailbox
  ├── Inbox
  ├── Compose
  └── Read
Pages
  ├── Invoice
  ├── Profile
  ├── E-commerce
  ├── Projects
  ├── Contacts
  └── FAQ
Auth Pages
  ├── Login
  ├── Register
  └── More...
Extras
  ├── Error 404
  ├── Error 500
  ├── Starter Page
  └── Blank Page
Search
Documentation
```
