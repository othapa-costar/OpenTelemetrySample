using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var serviceName = "OpenTelemetryDemo";
var serviceVersion = "1.0";

builder.Services.AddControllers();
//builder.Services.AddOpenTelemetry()
//    .ConfigureResource(resource => resource.AddService(
//            serviceName: serviceName,
//            serviceVersion: serviceVersion))
//        .WithTracing(tracing => tracing
//            .AddSource(serviceName)
//            .AddAspNetCoreInstrumentation()
//            //.AddConsoleExporter()
//            )
//        .WithMetrics(metrics => metrics.AddMeter(serviceName)
//            //.AddConsoleExporter()
//            );

builder.Logging.AddOpenTelemetry(options => options
        .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(
            serviceName: serviceName,
            serviceVersion: serviceVersion))
        //.AddConsoleExporter()
        .AddOtlpExporter(
            "demo",
            options =>
            {
                // Note: Options can also be set via code but order is important.
                // In the example here the code will apply after configuration.
                options.Endpoint = new Uri(Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT"));
                options.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.Grpc;
                //options.ExportProcessorType = OpenTelemetry.ExportProcessorType.Simple;
            })
        //.AddConsoleExporter()
        );

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
