using HomeInventory.Application.Common;
using HomeInventory.Application.DTOs;
using MediatR;

namespace HomeInventory.Application.Queries.Categories.GetAllCategories;

public record GetAllCategoriesQuery : IRequest<Result<List<CategoryDto>>>;
