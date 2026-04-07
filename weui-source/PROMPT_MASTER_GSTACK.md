Use the gstack workflow you already have installed.

Task:
Migrate the current React + TypeScript UI showcase into a production-ready Vue 3 + TypeScript admin/dashboard template.

Important execution rules:
1. Do not do a mechanical JSX-to-Vue conversion.
2. Treat this as an architecture refactor + UI system standardization task.
3. Use the project requirements file as the source of truth:
   `docs/vue-ui-template-migration/REQUIREMENTS.md`
4. Store all analysis, planning, mapping, and delivery documents inside:
   `docs/vue-ui-template-migration/`
5. Keep documents concise, structured, and useful.
6. Preserve the visual DNA of the original UI where reasonable:
   modern, premium, enterprise, dark-mode capable, gradient accents.
7. This project must be implemented as a Tailwind-first Vue template, not as a plain Vue port with ad-hoc CSS.

Required stack:
- Vue 3
- TypeScript
- Vite
- Vue Router
- Pinia
- Tailwind CSS

Styling rules:
- Tailwind CSS must be the primary styling system.
- Use CSS variables for design tokens and theme values.
- Support both light mode and dark mode.
- Avoid scattered custom CSS unless necessary.
- Avoid hardcoded colors across components.
- Keep utility usage clean, readable, and reusable.
- Prefer a Tailwind + token-based architecture over page-specific styling hacks.

Execution workflow:
- First read and follow:
  - `docs/vue-ui-template-migration/REQUIREMENTS.md`

Then execute in this order:

PHASE 1 — Audit current React UI
- Inspect the current project structure and identify:
  - primitive UI components
  - composed patterns
  - page-level sections
  - design tokens / theme logic
  - stateful demo-only logic
- Write a concise audit document:
  - `docs/vue-ui-template-migration/01-audit-current-react-ui.md`

PHASE 2 — Design target Vue architecture
- Propose a maintainable Vue architecture with:
  - layouts
  - views
  - ui components
  - layout components
  - stores
  - composables
  - router
  - styles/tokens
- Save it to:
  - `docs/vue-ui-template-migration/02-target-architecture-vue.md`

PHASE 3 — Create React-to-Vue mapping
- Map old React showcase structure into new Vue template structure
- Identify:
  - what to keep
  - what to split
  - what to redesign
  - what becomes docs/demo page vs production component
- Save it to:
  - `docs/vue-ui-template-migration/03-component-mapping-react-to-vue.md`

PHASE 4 — Create implementation plan
- Break implementation into logical modules
- Prioritize architecture first, then reusable UI foundation, then pages
- Save the plan to:
  - `docs/vue-ui-template-migration/04-implementation-plan.md`

PHASE 5 — Implement the Vue template
Build a production-ready structure with at least:
- layouts:
  - DefaultLayout
  - AuthLayout
  - DocsLayout or ComponentLibraryLayout if appropriate

- pages:
  - Dashboard Overview
  - Forms
  - Tables
  - Feedback
  - Dialogs
  - File Upload
  - Settings
  - Auth pages
  - Tokens / Design System
  - Not Found

- reusable components:
  - WxButton
  - WxInput
  - WxPasswordInput
  - WxTextarea
  - WxInputNumber
  - WxSelect
  - WxMultiSelect
  - WxDropdown
  - WxSwitch
  - WxRadio
  - WxCheckbox
  - WxTabs
  - WxAvatar
  - WxTag
  - WxBadge
  - WxSpinner
  - WxProgressBar
  - WxPagination
  - WxSkeleton
  - WxCard
  - WxEmptyState
  - WxAlert
  - WxStatCard
  - WxFilterBar
  - WxBreadcrumb
  - WxStepper
  - WxPageHeader
  - WxFileUpload
  - WxDatePicker
  - WxSlider
  - WxDrawer
  - WxTimeline
  - WxTreeView
  - WxToast
  - WxModal
  - WxConfirmDialog
  - WxDataTable
  - WxSideNav

Implementation requirements:
- Strong TypeScript typing
- Clear props / emits / slots
- Basic accessibility
- Dark mode support
- Tailwind-first styling
- No giant monolithic file
- No unnecessary micro-components
- No uncontrolled design drift
- Avoid over-dependence on external UI kits

PHASE 6 — Theme and design tokens
- Standardize:
  - colors
  - semantic colors
  - spacing
  - radius
  - shadows
  - typography
- Use CSS variables as the token layer
- Integrate tokens cleanly with Tailwind CSS
- Support light and dark themes
- Save concise planning notes to:
  - `docs/vue-ui-template-migration/06-design-token-plan.md`

PHASE 7 — State management and routing
- Add Pinia where it makes sense:
  - theme
  - sidebar
  - toast
  - modal/global ui state
- Add router structure and sidebar navigation
- Save concise notes to:
  - `docs/vue-ui-template-migration/05-routing-layout-plan.md`
  - `docs/vue-ui-template-migration/07-state-management-plan.md`

PHASE 8 — Final delivery report
At the end, produce:
- summary of what changed
- key architecture decisions
- file tree
- major implemented files
- known mocks / placeholders
- quick test instructions
- completion checklist

Save to:
- `docs/vue-ui-template-migration/08-final-delivery-report.md`

Output format in the final response:
1. Task checklist
2. Summary
3. Files changed
4. Docs created
5. Commands run
6. Assumptions
7. Risks / follow-ups
8. Quick verification steps

Default behavior:
- If information is missing, make reasonable assumptions and continue.
- Do not ask for confirmation.
- Keep docs concise and organized.
- Prioritize maintainability and template quality over raw speed.
