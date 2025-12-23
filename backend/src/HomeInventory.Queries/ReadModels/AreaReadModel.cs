namespace HomeInventory.Queries.ReadModels;

public class AreaReadModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid? ParentAreaId { get; set; }
    public string? ParentAreaName { get; set; }
    public string FullPath { get; set; } = string.Empty; // e.g., "Room > Shelf > Box"
    public int ItemCount { get; set; }
    public int ChildCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
