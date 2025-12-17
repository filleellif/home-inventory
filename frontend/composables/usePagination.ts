export const usePagination = (initialPage = 1, initialPageSize = 20) => {
  const currentPage = ref(initialPage)
  const pageSize = ref(initialPageSize)

  const goToPage = (page: number) => {
    currentPage.value = page
  }

  const nextPage = () => {
    currentPage.value++
  }

  const previousPage = () => {
    if (currentPage.value > 1) {
      currentPage.value--
    }
  }

  const reset = () => {
    currentPage.value = initialPage
  }

  return {
    currentPage,
    pageSize,
    goToPage,
    nextPage,
    previousPage,
    reset,
  }
}
