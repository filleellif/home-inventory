import type { CategoryDto, CreateCategoryDto, CategoryTreeNode } from '~/types/category'

export const useCategories = () => {
  const { apiFetch } = useApi()
  const toast = useToast()

  // Fetch all categories (not paginated)
  const fetchCategories = async () => {
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
      const { data, error } = await apiFetch<{ id: string }>('/categories', {
        method: 'POST',
        body: category,
      })

      if (error.value) {
        toast.error(error.value.data?.message || 'Failed to create category')
        return { success: false, error: error.value }
      }

      toast.success('Category created successfully')
      return { success: true, data: data.value }
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
