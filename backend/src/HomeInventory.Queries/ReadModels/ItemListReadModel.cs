namespace HomeInventory.Queries.ReadModels;

public class ItemListReadModel
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public int Quantity { get; init; }
    public decimal? CurrentValueAmount { get; init; }
    public string? CurrentValueCurrency { get; init; }
    public string? RoomName { get; init; }
    public Guid? CategoryId { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
}
