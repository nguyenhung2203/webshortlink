# Phase 3: React-to-Vue Component Mapping

## Direct Mapping (keep visual, rebuild architecture)
| React Source | Vue Target | Action |
|---|---|---|
| WxButton (line 564) | WxButton.vue | Rebuild as prop-driven component |
| Inputs section (line 616) | WxInput.vue, WxPasswordInput.vue, WxTextarea.vue, WxInputNumber.vue | Split into 4 separate components |
| Select/MultiSelect (line 697) | WxSelect.vue, WxMultiSelect.vue | Rebuild with @floating-ui/vue |
| WxDropdown (line 730) | WxDropdown.vue | Rebuild with Teleport + floating-ui |
| Controls toggle (line 800) | WxSwitch.vue | Extract toggle as standalone |
| Controls radio (line 800) | WxRadio.vue | Extract radio as standalone |
| Controls checkbox (line 800) | WxCheckbox.vue | Extract checkbox as standalone |
| Controls slider (line 800) | WxSlider.vue | Extract slider as standalone |
| WxTabs (line 875) | WxTabs.vue | Rebuild with slots |
| WxAvatar (line 952) | WxAvatar.vue | Keep visual, add size/status props |
| WxTag (line 1018) | WxTag.vue | Keep visual, add removable prop |
| Badges (line 1069) | WxBadge.vue | Standardize variants |
| Spinner (line 1088) | WxSpinner.vue | Simple animated spinner |
| ProgressBar (line 1088) | WxProgressBar.vue | Determinate progress |
| WxPagination (line 1162) | WxPagination.vue | Rebuild with proper events |
| WxSkeleton (line 1255) | WxSkeleton.vue | Keep visual |
| Cards (line 1273) | WxCard.vue | Generic card container |
| WxEmptyState (line 1307) | WxEmptyState.vue | Keep visual |
| WxDataTable (line 1348) | WxDataTable.vue + useDataTable.ts | Rebuild with composable |
| WxStatCard (line 1855) | WxStatCard.vue | Keep visual |
| WxAlert (line 1889) | WxAlert.vue | Keep visual |
| WxFilterBar (line 2145) | WxFilterBar.vue | Keep visual |
| WxBreadcrumb (line 2690) | WxBreadcrumb.vue | Rebuild route-driven |
| WxStepper (line 2716) | WxStepper.vue | Rebuild with Vue Transition |
| WxPageHeader (line 2842) | WxPageHeader.vue | Keep visual |
| WxFileUpload (line 2887) | WxFileUpload.vue | Rebuild with drag-drop |
| WxDatePicker (line 2908) | WxDatePicker.vue | Simplified calendar picker |
| WxDrawer (line 2961) | WxDrawer.vue | Rebuild with Teleport |
| WxTimeline (line 2978) | WxTimeline.vue | Keep visual |
| WxTreeView (line 3012) | WxTreeView.vue | Recursive component |
| WxToast (line 3069) | WxToast.vue + useToast.ts | Rebuild with composable |
| Modals (line 3163) | WxModal.vue, WxConfirmDialog.vue | Split into 2 components |
| WxSideNav (line 3109) | WxSideNav.vue | Rebuild for real router |

## Patterns -> Demo Pages (not separate components)
| React Pattern | Vue Destination |
|---|---|
| WxSettingSection + WxToggleRow | SettingsView.vue |
| WxOptionCard | FormsView.vue |
| WxCollapsiblePanel | FeedbackView.vue (as accordion) |
| WxFormLayout | FormsView.vue |
| WxAuthPatterns | LoginView.vue, RegisterView.vue |
| WxFeedbackPatterns | FeedbackView.vue |
| WxDataVisualization | DashboardView.vue (CSS charts) |
| WxNotificationPatterns | FeedbackView.vue |
| WxCardPatterns | DashboardView.vue |
| WxLayoutPatterns | DefaultLayout.vue |

## Not Migrating
- Recharts (React-only) -> CSS charts or vue-chartjs later
- Framer Motion -> Vue `<Transition>` + CSS animations
- All local demo state -> proper Pinia stores / component state
