namespace HomeInventory.WebApi.Configuration;

public sealed record ApplicationOptions
{
    public required OpenTelemetryOptions OpenTelemetry { get; init; }
}

public sealed record OpenTelemetryOptions
{
    public required string ServiceName { get; init; }

    public required string ServiceVersion { get; init; }

    public required bool EnableLogging { get; init; }

    public required bool EnableTracing { get; init; }

    public required bool EnableMetrics { get; init; }

    public required ExportersOptions Exporters { get; init; }
}

public sealed record ExportersOptions
{
    public required ConsoleExporterOptions Console { get; init; }

    public required OtlpExporterOptions Otlp { get; init; }
}

public sealed record ConsoleExporterOptions
{
    public bool Enabled { get; init; }
}

public sealed record OtlpExporterOptions
{
    public bool Enabled { get; init; }

    public string? Endpoint { get; init; }
}