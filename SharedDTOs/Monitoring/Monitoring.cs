using Microsoft.Extensions.Logging;
using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using Serilog.Enrichers.Span;
using Serilog.Sinks.Loki;
using System.Diagnostics;
using System.Reflection;

namespace SharedDTOs.Monitoring;

public class Monitoring
{
    public static Microsoft.Extensions.Logging.ILogger Log { get; set; }
    private static readonly TracerProvider TracerProvider;
    public static readonly ActivitySource ActivitySource = new("ChessGame_" + Assembly.GetCallingAssembly()?.GetName().Name, "1.0.0");

    static Monitoring()
    {
        // Configure tracing
        var serviceName = Assembly.GetCallingAssembly().GetName().Name;
        var version = "1.0.0";

        TracerProvider = Sdk.CreateTracerProviderBuilder()
            .AddConsoleExporter()
            .AddJaegerExporter(
            options =>
            {
                options.AgentHost = "jaeger";
                options.AgentPort = 6831;
            })
            .AddSource(ActivitySource.Name)
            .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(serviceName: serviceName, serviceVersion: version))
            .Build()!;

        Serilog.Core.Logger serilog = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .Enrich.WithSpan()
            .WriteTo.LokiHttp("http://loki:3100")
            .WriteTo.Console()
            .CreateLogger();

        var loggerFactory = new LoggerFactory().AddSerilog(serilog);
        Log = loggerFactory.CreateLogger("Logger");

        Log.LogDebug("Hello, Loki!");
    }
}
