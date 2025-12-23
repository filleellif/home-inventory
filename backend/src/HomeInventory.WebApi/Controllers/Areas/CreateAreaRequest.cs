using HomeInventory.Application.Commands.Areas.CreateArea;

namespace HomeInventory.WebApi.Controllers.Areas;

public sealed record CreateAreaRequest(
    string Name,
    Guid? ParentAreaId,
    string? Description);

internal static class CreateAreaRequestExtensions
{
    internal static CreateAreaCommand ToCommand(this CreateAreaRequest request, Guid id) => new(
        id,
        request.Name,
        request.ParentAreaId,
        request.Description);
}
