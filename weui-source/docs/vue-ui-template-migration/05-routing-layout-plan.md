# Phase 5: Routing & Layout Plan

## Layouts
- **DefaultLayout**: Sidebar + topbar + content. Sidebar 256px expanded, 64px collapsed. Responsive overlay on mobile.
- **AuthLayout**: Full-screen centered card, gradient background, Wemake branding.

## Navigation
Sidebar items map 1:1 to routes. Active state via route.path matching.

Items: Dashboard, Forms, Tables, Feedback, Dialogs, File Upload, Settings, Tokens.

## Route Guards
- Auth pages: no guard in template (mock). Production would add `beforeEach` auth check.
- Document title set via `router.beforeEach` using `route.meta.title`.
