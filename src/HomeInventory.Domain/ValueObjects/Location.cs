using HomeInventory.Domain.Common;

namespace HomeInventory.Domain.ValueObjects;

public class Location : ValueObject
{
    public string? Room { get; private set; }
    public string? StorageSpot { get; private set; }
    public GpsCoordinates? Coordinates { get; private set; }

    // Private parameterless constructor for EF Core
    private Location()
    {
    }

    private Location(string? room, string? storageSpot, GpsCoordinates? coordinates)
    {
        Room = string.IsNullOrWhiteSpace(room) ? null : room.Trim();
        StorageSpot = string.IsNullOrWhiteSpace(storageSpot) ? null : storageSpot.Trim();
        Coordinates = coordinates;
    }

    public static Location Create(string? room = null, string? storageSpot = null, GpsCoordinates? coordinates = null)
    {
        return new Location(room, storageSpot, coordinates);
    }

    public static Location Empty()
    {
        return new Location(null, null, null);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Room;
        yield return StorageSpot;
        yield return Coordinates;
    }

    public override string ToString()
    {
        var parts = new List<string>();

        if (!string.IsNullOrWhiteSpace(Room))
            parts.Add($"Room: {Room}");

        if (!string.IsNullOrWhiteSpace(StorageSpot))
            parts.Add($"Spot: {StorageSpot}");

        if (Coordinates != null)
            parts.Add($"GPS: {Coordinates}");

        return parts.Count > 0 ? string.Join(", ", parts) : "No location";
    }
}
