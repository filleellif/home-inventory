using HomeInventory.Domain.Common;

namespace HomeInventory.Domain.ValueObjects;

public class FinancialInfo : ValueObject
{
    public Money? PurchasePrice { get; private set; }
    public Money? CurrentValue { get; private set; }
    public DateTime? PurchaseDate { get; private set; }

    // Private parameterless constructor for EF Core
    private FinancialInfo()
    {
    }

    private FinancialInfo(Money? purchasePrice, Money? currentValue, DateTime? purchaseDate)
    {
        if (purchaseDate.HasValue && purchaseDate.Value > DateTime.UtcNow)
            throw new DomainException("Purchase date cannot be in the future.");

        PurchasePrice = purchasePrice;
        CurrentValue = currentValue;
        PurchaseDate = purchaseDate;
    }

    public static FinancialInfo Create(Money? purchasePrice = null, Money? currentValue = null, DateTime? purchaseDate = null)
    {
        return new FinancialInfo(purchasePrice, currentValue, purchaseDate);
    }

    public static FinancialInfo Empty()
    {
        return new FinancialInfo(null, null, null);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return PurchasePrice;
        yield return CurrentValue;
        yield return PurchaseDate;
    }

    public override string ToString()
    {
        var parts = new List<string>();

        if (PurchasePrice != null)
            parts.Add($"Purchase: {PurchasePrice}");

        if (CurrentValue != null)
            parts.Add($"Current: {CurrentValue}");

        if (PurchaseDate.HasValue)
            parts.Add($"Purchased: {PurchaseDate.Value:yyyy-MM-dd}");

        return parts.Count > 0 ? string.Join(", ", parts) : "No financial info";
    }
}
