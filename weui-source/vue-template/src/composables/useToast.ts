import { useToastStore } from '@/stores/toast'
import { storeToRefs } from 'pinia'

export function useToast() {
  const store = useToastStore()
  const { toasts } = storeToRefs(store)

  return {
    toasts,
    add: store.add,
    remove: store.remove,
    success: (title: string, message: string) => store.add('success', title, message),
    error: (title: string, message: string) => store.add('error', title, message),
    info: (title: string, message: string) => store.add('info', title, message),
    warning: (title: string, message: string) => store.add('warning', title, message),
  }
}
