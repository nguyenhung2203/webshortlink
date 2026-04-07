# Phase 1: Audit Current React UI

## Source
- File: `system 001/src/App.tsx` (4,323 lines, 262KB)
- Styles: `system 001/src/index.css` (184 lines)
- Stack: React 19, TypeScript, Vite, Tailwind CSS v4, Lucide React, Framer Motion, Recharts

## Structure
Single monolithic component `App()` with all state, logic, and UI in one file.
5 tab pages: `primitives`, `patterns`, `advanced`, `dialogs`, `tokens`.

## Components Identified

### Primitives (line 562-1321)
| Component | Lines | Features |
|-----------|-------|----------|
| WxButton | 564-614 | 7 variants (primary, cta, secondary, ghost, danger, success, text), 3 sizes, icon/loading/disabled states |
| Inputs | 616-695 | Text, search, password (with eye toggle), textarea, number input |
| Select & MultiSelect | 697-728 | Custom dropdown with chevron, multi-select with tags |
| WxDropdown | 730-798 | Popover menu with sections and dividers |
| Controls | 800-873 | Toggle/switch, radio group, checkbox, slider with value display |
| WxTabs | 875-950 | Tab bar with active indicator, content panels |
| WxAvatar | 952-1016 | Multiple sizes, group stacking, status indicators |
| WxTag | 1018-1066 | Removable tags, multiple variants |
| Badges | 1069-1086 | Color variants (teal, amber, rose, sky, slate, emerald) |
| Spinner & ProgressBar | 1088-1160 | Animated spinner, determinate progress bar |
| WxPagination | 1162-1252 | Page numbers, prev/next, page size selector, item count display |
| WxSkeleton | 1255-1304 | Loading skeleton cards with animated pulse |
| Cards | 1273-1306 | Basic card container pattern |
| WxEmptyState | 1307-1320 | Centered empty state with icon and message |

### Patterns (line 1323-2687)
| Pattern | Lines | Features |
|---------|-------|----------|
| WxDataTable (3 variants) | 1348-1854 | TKQC table, BM table with row selection + filter, Activity log table. All have sort, filter, status badges |
| WxStatCard | 1855-1888 | Metric cards with icon, value, trend indicator |
| WxAlert | 1889-1930 | 4 types: info, success, warning, error with icons |
| WxSettingSection + WxToggleRow | 1931-1970 | Settings groups with toggle switches |
| WxOptionCard | 1971-2008 | Radio-style selection cards |
| WxCollapsiblePanel | 2009-2066 | Accordion panels with expand/collapse |
| WxDashboardSection + WxFeatureCard | 2067-2095 | Dashboard grid with feature cards |
| WxActionPopoverPanel | 2096-2144 | Action buttons with popover |
| WxFilterBar | 2145-2188 | Tag-based removable filters |
| WxFormLayout | 2189-2244 | Form sections with labels, inputs, selects |
| WxAuthPatterns | 2245-2282 | Login form pattern |
| WxFeedbackPatterns | 2283-2332 | Feedback/rating pattern |
| WxDataVisualization | 2333-2409 | Recharts line/bar/pie charts |
| WxEmptyStatePatterns | 2410-2448 | Various empty state variants |
| WxRemovablePatterns | 2449-2523 | Removable tags, filters, user cards |
| WxNotificationPatterns | 2524-2574 | Notification list items |
| WxCardPatterns | 2575-2631 | Various card layouts |
| WxLayoutPatterns | 2632-2686 | Layout structure patterns |

### Advanced (line 2688-3162)
| Component | Lines | Features |
|-----------|-------|----------|
| WxBreadcrumb | 2690-2714 | Route-based breadcrumb with home icon |
| WxStepper | 2716-2841 | Horizontal step wizard with progress, completion animation |
| WxPageHeader | 2842-2886 | Page title, description, action buttons, breadcrumb |
| WxFileUpload | 2887-2907 | Drag-and-drop upload area |
| WxDatePicker & WxSlider | 2908-2960 | Date input + range slider |
| WxDrawer | 2961-2977 | Side panel with form fields, implemented as overlay |
| WxTimeline | 2978-3011 | Vertical timeline with colored dots |
| WxTreeView | 3012-3068 | Expandable tree with nested nodes |
| WxToast | 3069-3108 | Toast notification triggers (info/success/warning/error) |
| WxSideNav | 3109-3161 | Side navigation preview with icons, active state, collapse |

### Dialogs (line 3163-3264)
| Component | Lines | Features |
|-----------|-------|----------|
| Modal triggers | 3163-3264 | 12 modal types: info, success, error, question, result, delete, warning, sidebar, default, config (tabbed), campaign-form, system-info |

### Tokens (line 3265-4323)
| Section | Content |
|---------|---------|
| Brand Colors | Primary (#00f2ff), semantic colors with copy-to-clipboard |
| Gradients | Header gradient, button gradient, card gradients |
| Typography | Inter (sans), JetBrains Mono (mono), size scale |
| Foundation Surfaces | 5-level surface system, light/dark |
| Glass Effects | 3 levels: heavy, medium, light |
| Semantic Colors | Danger, success, warning, info |

## Design Tokens (from index.css)
- Tailwind v4 @theme syntax + CSS variables in :root/.dark
- Fonts: Inter, JetBrains Mono
- Colors: primary (#00f2ff), surfaces (5 levels), outlines
- Glass effects: 3 levels via @layer components
- Button styles: primary, secondary, danger, success via @layer components
- Custom scrollbar styling

## Stateful Demo Logic (to NOT migrate)
- Table sort state (3 separate implementations)
- Table filter state
- Pagination state
- Modal config state
- Drawer form state
- Campaign form state
- Config form state
- Stepper completion animation
- Toast queue management
- Tree node expand/collapse
- Popover open/close

## Key Observations
1. All 4,323 lines in one component = all state is local, no routing, no store
2. Framer Motion used for: stepper animation, modal/drawer transitions, toast enter/exit
3. Recharts used for: line chart, bar chart, pie chart (Dashboard section)
4. ~50 lucide-react icons imported
5. Vietnamese text throughout (Wemake brand context)
6. Dark mode via class toggle on root div
7. Glass morphism effects used in several patterns
8. Responsive: basic (hidden sm:flex patterns), mostly desktop
