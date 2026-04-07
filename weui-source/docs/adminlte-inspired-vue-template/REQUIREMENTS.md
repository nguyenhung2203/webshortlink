# Requirements: AdminLTE-Inspired Vue 3 Template

## Goal
Expand the existing WeUI Vue 3 template to match AdminLTE v3's page coverage and navigation depth, using modern Vue 3 + TypeScript + Tailwind architecture.

## Scope
- ~60+ page views across 16 route groups
- 4 layout variants
- Nested sidebar navigation with multi-level menu
- Full dashboard, app, and utility page shells

## Existing (to keep)
- 37 Wx UI components
- 5 composables, 4 Pinia stores
- Design tokens, dark mode
- DefaultLayout, AuthLayout

## New Requirements

### Layouts (add 2)
- TopNavLayout (horizontal nav, no sidebar)
- BlankLayout (no chrome, for error/misc pages)

### Layout Components (add 4)
- WxFooter
- WxRightSidePanel (control sidebar)
- WxNotificationDropdown
- WxMessageDropdown
- WxUserMenu

### Route Groups (16 total)
/dashboard/*, /widgets, /layout/*, /charts/*, /ui/*, /forms/*, /tables/*, /examples/*, /mailbox/*, /pages/*, /auth/*, /errors/*, /search/*, /docs, /starter, /blank

### Pages (~50+ new)
See ROUTES.md for full list.
