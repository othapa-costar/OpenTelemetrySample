# OpenTelemetryDemo
 
Steps to setup OpenTelemetry
It basically includes 3 steps.

Step 1: Download the necessary OpenTelemety SDK packages to your project
e.g. instrumenting inbound and output requests
	Install OpenTelemetry Core packages:
	- dotnet add package OpenTelemetry
	- dotnet add package OpenTelemetry.Extensions.Hosting
	- dotnet add package OpenTelemetry.Exporter.Console
	- dotnet add package OpenTelemetry.Exporter.OpenTelemetryProtocol //needed to support collector
	
	Install Instrumentation packages specifigy to your .net version
	- dotnet add package OpenTelemetry.Instrumentation.AspNetCore --prerelease
	- dotnet add package OpenTelemetry.Instrumentation.Http --prerelease

Step 2: Instrument your application to use OpenTelemetry SDK typically done by registering at application start time.
	builder.Services.AddOpenTelemetry()
		.ConfigureResource(resource => resource.AddService(
			serviceName: serviceName,
			serviceVersion: serviceVersion))
		.WithTracing(tracing => tracing
			.AddSource(serviceName
			.AddHttpClientInstrumentation()
			.AddAspNetCoreInstrumentation()
			.AddConsoleExporter())
		.WithMetrics(metrics => metrics
			.AddMeter(serviceName)
			.AddConsoleExporter());

	builder.Logging.AddOpenTelemetry(options => options
		.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(
			serviceName: serviceName,
			serviceVersion: serviceVersion))
		.AddConsoleExporter());

Note: if you instrumenting a library , you don't need to initialize the SDK.

Step 3: Setup and Deploy a collector instance
There are various ways to set it up. The details can be found in this link: https://opentelemetry.io/docs/collector/deployment/

In this project, I have deployed the collector as a side car using the docker compose.
The collector is setup to listen for messages at port 4317
We can achieve desired behavior though configuration file. 
This link (https://opentelemetry.io/docs/collector/configuration/) will give detial on how to configure the yaml file.

The alternate way to do this is by doing a deploying the collector image separately, but they have to be defined withing same network.
Steps

a. Setup a docker network 
	docker network create otel-network

b. Pull the latest collector image
	docker pull otel/opentelemetry-collector:latest

c. Run it in a newly created network
	docker run -d --name otel-collector --network otel-network -p 4317:4317 -v C:\path\to\otel-collector-config.yaml:/otel-config.yaml otel/opentelemetry-collector:latest --config /otel-config.yaml

d. Run .net application
	docker pull <your-docker-registry>/my-dotnet-app-image:latest
	docker run -d --name dotnet-app --network otel-network <your-docker-registry>/my-dotnet-app-image:latest