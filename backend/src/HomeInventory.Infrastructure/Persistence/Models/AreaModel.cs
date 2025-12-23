namespace HomeInventory.Infrastructure.Persistence.Models;

public class AreaModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid? ParentAreaId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Navigation property
    public AreaModel? ParentArea { get; set; }
    public ICollection<AreaModel> ChildAreas { get; set; } = new List<AreaModel>();
}
