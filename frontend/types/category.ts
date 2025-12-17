export interface CategoryDto {
  id: string
  name: string
  description?: string
  parentCategoryId?: string
  createdAt: string
  updatedAt: string
}

export interface CreateCategoryDto {
  name: string
  description?: string
  parentCategoryId?: string
}

export interface CategoryTreeNode extends CategoryDto {
  children: CategoryTreeNode[]
}
