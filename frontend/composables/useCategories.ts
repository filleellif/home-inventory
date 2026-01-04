import type { CategoryDto, CreateCategoryDto, CategoryTreeNode } from '~/types/category'

export const useCategories = () => {
  const store = useCategoriesStore()

  // Backward-compatible wrapper for fetchCategories
  const fetchCategories = async () => {
    // Ensure store is initialized
    if (!store.initialized) {
      await store.initFromIndexedDB()
    }

    return {
      data: computed(() => store.allCategories),
      error: computed(() => store.error),
      pending: computed(() => store.loading),
      refresh: () => store.refreshFromApi(),
    }
  }

  // Create category - delegate to store
  const createCategory = (category: CreateCategoryDto) => store.create(category)

  // Helper: Build category tree - delegate to store
  const buildCategoryTree = (categories: CategoryDto[]): CategoryTreeNode[] => store.buildTree(categories)

  return {
    fetchCategories,
    createCategory,
    buildCategoryTree,
    // Direct store access for components that want it
    store,
  }
}
