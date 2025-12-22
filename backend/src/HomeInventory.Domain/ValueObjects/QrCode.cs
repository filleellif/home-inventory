using HomeInventory.Domain.Common;

namespace HomeInventory.Domain.ValueObjects;

public class QrCode : ValueObject
{
    public string Code { get; }

    private QrCode(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new DomainException("QR code cannot be empty.");

        if (code.Length > 100)
            throw new DomainException("QR code cannot exceed 100 characters.");

        Code = code.Trim();
    }

    public static QrCode Create(string code)
    {
        return new QrCode(code);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Code;
    }

    public override string ToString()
    {
        return Code;
    }
}
