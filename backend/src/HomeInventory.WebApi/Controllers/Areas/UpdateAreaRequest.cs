using HomeInventory.Application.Commands.Areas.UpdateArea;

namespace HomeInventory.WebApi.Controllers.Areas;

public sealed record UpdateAreaRequest(
    string Name,
    Guid? ParentAreaId,
    string? Description);

internal static class UpdateAreaRequestExtensions
{
    internal static UpdateAreaCommand ToCommand(this UpdateAreaRequest request, Guid id) => new(
        id,
        request.Name,
        request.ParentAreaId,
        request.Description);
}
