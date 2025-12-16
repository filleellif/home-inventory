using HomeInventory.Domain.Common;

namespace HomeInventory.Domain.ValueObjects;

public class GpsCoordinates : ValueObject
{
    public double Latitude { get; private set; }
    public double Longitude { get; private set; }

    // Private parameterless constructor for EF Core
    private GpsCoordinates()
    {
    }

    private GpsCoordinates(double latitude, double longitude)
    {
        if (latitude < -90 || latitude > 90)
            throw new DomainException("Latitude must be between -90 and 90 degrees.");

        if (longitude < -180 || longitude > 180)
            throw new DomainException("Longitude must be between -180 and 180 degrees.");

        Latitude = latitude;
        Longitude = longitude;
    }

    public static GpsCoordinates Create(double latitude, double longitude)
    {
        return new GpsCoordinates(latitude, longitude);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Latitude;
        yield return Longitude;
    }

    public override string ToString()
    {
        return $"{Latitude:F6}, {Longitude:F6}";
    }
}
