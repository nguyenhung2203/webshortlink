import { useUiStore } from '@/stores/ui'
import { storeToRefs } from 'pinia'

export function useModal() {
  const store = useUiStore()
  const { modalOpen, drawerOpen } = storeToRefs(store)

  return {
    modalOpen,
    drawerOpen,
    openModal: store.openModal,
    closeModal: store.closeModal,
    openDrawer: store.openDrawer,
    closeDrawer: store.closeDrawer,
  }
}
