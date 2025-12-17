namespace HomeInventory.Infrastructure.Configuration;

public sealed record PostgresOptions
{
    public required string ConnectionString { get; init; }
}