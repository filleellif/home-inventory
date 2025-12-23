using HomeInventory.WebApi.Configuration;
using Microsoft.VisualBasic;
using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace HomeInventory.WebApi;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection ConfigureLogging(this IServiceCollection services, OpenTelemetryOptions options)
    {
        var builder = services
            .AddOpenTelemetry()
            .ConfigureResource(resource => resource
                .AddService(serviceName: options.ServiceName, serviceVersion: options.ServiceVersion));

        if (options.EnableTracing)
        {
            builder = builder
                .WithTracing(tracing =>
                {
                    tracing
                        .AddAspNetCoreInstrumentation()
                        .AddHttpClientInstrumentation();

                    if (options.Exporters.Console.Enabled)
                    {
                        tracing.AddConsoleExporter();
                    }

                    if (options.Exporters.Otlp.Enabled && !string.IsNullOrEmpty(options.Exporters.Otlp.Endpoint))
                    {
                        tracing.AddOtlpExporter(otlp =>
                        {
                            otlp.Endpoint = new Uri($"{options.Exporters.Otlp.Endpoint}/ingest/otlp/v1/traces");
                            otlp.Protocol = OtlpExportProtocol.HttpProtobuf;
                        });
                    }
                });
        }

        if (options.EnableMetrics)
        {
            builder = builder
                .WithMetrics(metrics =>
                {
                    metrics
                        .AddAspNetCoreInstrumentation()
                        .AddHttpClientInstrumentation();

                    if (options.Exporters.Console.Enabled)
                    {
                        metrics.AddConsoleExporter();
                    }

                    if (options.Exporters.Otlp.Enabled && !string.IsNullOrEmpty(options.Exporters.Otlp.Endpoint))
                    {
                        metrics.AddOtlpExporter(otlp =>
                        {
                            otlp.Endpoint = new Uri(options.Exporters.Otlp.Endpoint);
                            otlp.Protocol = OtlpExportProtocol.HttpProtobuf;
                        });
                    }
                });
        }

        if (options.EnableLogging)
        {
            builder = builder
                .WithLogging(logging =>
                {
                    if (options.Exporters.Console.Enabled)
                    {
                        logging.AddConsoleExporter();
                    }

                    if (options.Exporters.Otlp.Enabled && !string.IsNullOrEmpty(options.Exporters.Otlp.Endpoint))
                    {
                        logging.AddOtlpExporter(otlp =>
                        {
                            //otlp.Endpoint = new Uri(options.Exporters.Otlp.Endpoint);
                            otlp.Endpoint = new Uri($"{options.Exporters.Otlp.Endpoint}/ingest/otlp/v1/logs");
                            otlp.Protocol = OtlpExportProtocol.HttpProtobuf;
                        });
                    }
                });
        }

        return services;
    }
}