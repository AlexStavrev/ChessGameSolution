using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System.Diagnostics;
using System.Reflection;

namespace SharedDTOs.Monitoring;

public class Monitoring
{
    public static readonly ActivitySource ActivitySource = new("ChessGame_" + Assembly.GetCallingAssembly()?.GetName().Name, "1.0.0");
    private static TracerProvider TracerProvider;

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
    }
}
