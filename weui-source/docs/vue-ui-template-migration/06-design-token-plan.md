# Phase 6: Design Token Plan

## Token Architecture
- **Source:** `src/assets/styles/tokens.css`
- **System:** Tailwind CSS v4 `@theme` + CSS variables in `:root` / `.dark`

## Token Categories

### Brand Colors (via @theme)
| Token | Value | Usage |
|-------|-------|-------|
| --color-primary | #00f2ff | Primary actions, active states |
| --color-primary-dim | #00d8e6 | Primary hover |
| --color-danger | #F43F5E | Error, destructive actions |
| --color-success | #10B981 | Success states |
| --color-warning | #F59E0B | Warning states |
| --color-info | #0EA5E9 | Info states |

### Surface Colors (via :root / .dark)
| Token | Light | Dark |
|-------|-------|------|
| --color-surface | #f8fafc | #0a0e1a |
| --color-surface-container | #ffffff | #111827 |
| --color-surface-container-low | #f1f5f9 | #1c243a |
| --color-surface-container-high | #e2e8f0 | #2d3748 |
| --color-surface-container-lowest | #ffffff | #0f172a |
| --color-on-surface | #0f172a | #f8fafc |
| --color-on-surface-variant | #64748b | #94a3b8 |

### Outline Colors
| Token | Light | Dark |
|-------|-------|------|
| --color-outline | rgba(0,0,0,0.1) | rgba(255,255,255,0.1) |
| --color-outline-variant | rgba(0,0,0,0.05) | rgba(255,255,255,0.08) |

### Typography (via @theme)
| Token | Value |
|-------|-------|
| --font-sans | Inter, ui-sans-serif, system-ui |
| --font-mono | JetBrains Mono, ui-monospace |

### Gradients (via :root)
| Token | Value |
|-------|-------|
| --gradient-header | linear-gradient(90deg, #18CFEA, #1E63F1) |
| --gradient-btn-primary | linear-gradient(135deg, #00f2ff, #0088ff) |

### Glass Effects (via @layer components)
| Class | Effect |
|-------|--------|
| .glass-heavy | bg-white/70 backdrop-blur-md |
| .glass-medium | bg-white/40 backdrop-blur-sm |
| .glass-light | bg-white/20 backdrop-blur-[2px] |

## Dark Mode Implementation
- Toggle via `.dark` class on `<html>` element
- Theme store persists to localStorage
- System preference detected on first load
- All surface/outline tokens swap automatically
