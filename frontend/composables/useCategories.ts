import type { CategoryDto, CreateCategoryDto, CategoryTreeNode } from '~/types/category'

export const useCategories = () => {
  const { apiFetch } = useApi()
  const toast = useToast()

  // Fetch all categories (not paginated)
  const fetchCategories = async () => {
    const { isOnline } = useOffline()
    const { getCategories } = useOfflineStorage()

    // If offline, get from IndexedDB
    if (!isOnline.value) {
      const categories = await getCategories()

      return {
        data: ref(categories),
        error: ref(null),
        pending: ref(false),
        refresh: async () => {}
      }
    }

    // Online - normal API fetch
    const { data, error, pending, refresh } = await apiFetch<CategoryDto[]>(
      '/categories',
      {
        key: 'categories'
      }
    )

    return { data, error, pending, refresh }
  }

  // Create category
  const createCategory = async (category: CreateCategoryDto) => {
    try {
      const result = await apiFetch<{ id: string }>('/categories', {
        method: 'POST',
        body: category,
      })

      if (result.error?.value) {
        toast.error(result.error.value.data?.message || 'Failed to create category')
        return { success: false, error: result.error.value }
      }

      // Check if this was an offline operation
      if ((result as any).offline) {
        toast.info('Category saved offline. Will sync when online.')
        return { success: true, data: result.data.value, offline: true }
      }

      toast.success('Category created successfully')
      return { success: true, data: result.data.value }
    } catch (err) {
      toast.error('Failed to create category')
      return { success: false, error: err }
    }
  }

  // Helper: Build category tree for hierarchical display
  const buildCategoryTree = (categories: CategoryDto[]): CategoryTreeNode[] => {
    console.log('Building category tree from categories:', categories)
    const categoryMap = new Map(
      categories.map(cat => [cat.id, { ...cat, children: [] as CategoryTreeNode[] }])
    )
    const rootCategories: CategoryTreeNode[] = []

    categories.forEach(category => {
      const categoryWithChildren = categoryMap.get(category.id)!

      if (category.parentCategoryId) {
        const parent = categoryMap.get(category.parentCategoryId)
        if (parent) {
          parent.children.push(categoryWithChildren)
        } else {
          rootCategories.push(categoryWithChildren)
        }
      } else {
        rootCategories.push(categoryWithChildren)
      }
    })

    return rootCategories
  }

  return {
    fetchCategories,
    createCategory,
    buildCategoryTree,
  }
}
