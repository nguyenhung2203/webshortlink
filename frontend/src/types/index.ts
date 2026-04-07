export interface MenuItem {
  key: string
  label: string
  icon: string
  children?: MenuItem[]
}

export interface ToastItem {
  id: number
  type: 'info' | 'success' | 'warning' | 'error'
  title: string
  message: string
}

export interface TableColumn {
  key: string
  label: string
  sortable?: boolean
  width?: string
}

export interface BreadcrumbItem {
  label: string
  to?: string
}

export interface StepItem {
  label: string
  description?: string
}

export interface TimelineItem {
  title: string
  description?: string
  time?: string
  color?: string
}

export interface TreeNode {
  id: string
  label: string
  children?: TreeNode[]
  icon?: string
}

export interface SelectOption {
  label: string
  value: string | number
  disabled?: boolean
}
