namespace HomeInventory.Infrastructure.Persistence.Models;

public class ItemTagModel
{
    public Guid ItemId { get; init; }
    public string TagValue { get; init; } = string.Empty;
}
