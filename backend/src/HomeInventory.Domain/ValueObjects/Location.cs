using HomeInventory.Domain.Common;

namespace HomeInventory.Domain.ValueObjects;

public class Location : ValueObject
{
    public string? RoomName { get; private set; }
    public QrCode? RoomQrCode { get; private set; }

    public string? ShelfName { get; private set; }
    public QrCode? ShelfQrCode { get; private set; }

    public string? BoxName { get; private set; }
    public QrCode? BoxQrCode { get; private set; }

    // Private parameterless constructor for EF Core
    private Location()
    {
    }

    private Location(
        string? roomName, QrCode? roomQrCode,
        string? shelfName, QrCode? shelfQrCode,
        string? boxName, QrCode? boxQrCode)
    {
        RoomName = string.IsNullOrWhiteSpace(roomName) ? null : roomName.Trim();
        RoomQrCode = roomQrCode;

        ShelfName = string.IsNullOrWhiteSpace(shelfName) ? null : shelfName.Trim();
        ShelfQrCode = shelfQrCode;

        BoxName = string.IsNullOrWhiteSpace(boxName) ? null : boxName.Trim();
        BoxQrCode = boxQrCode;
    }

    public static Location Create(
        string? roomName = null, QrCode? roomQrCode = null,
        string? shelfName = null, QrCode? shelfQrCode = null,
        string? boxName = null, QrCode? boxQrCode = null)
    {
        return new Location(roomName, roomQrCode, shelfName, shelfQrCode, boxName, boxQrCode);
    }

    public static Location Empty()
    {
        return new Location(null, null, null, null, null, null);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return RoomName;
        yield return RoomQrCode;
        yield return ShelfName;
        yield return ShelfQrCode;
        yield return BoxName;
        yield return BoxQrCode;
    }

    public override string ToString()
    {
        var parts = new List<string>();

        if (!string.IsNullOrWhiteSpace(RoomName))
            parts.Add($"Room: {RoomName}");

        if (!string.IsNullOrWhiteSpace(ShelfName))
            parts.Add($"Shelf: {ShelfName}");

        if (!string.IsNullOrWhiteSpace(BoxName))
            parts.Add($"Box: {BoxName}");

        return parts.Count > 0 ? string.Join(" > ", parts) : "No location";
    }
}
