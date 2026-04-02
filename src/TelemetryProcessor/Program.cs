using Microsoft.Extensions.Options;
using Npgsql;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using TelemetryProcessor.Consumers;
using TelemetryProcessor.Listeners;
using TelemetryProcessor.Options;
using TelemetryProcessor.Repositories;

var builder = Host.CreateApplicationBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("Postgres")!;
builder.Services.AddSingleton(_ => NpgsqlDataSource.Create(connectionString));
builder.Services.AddScoped<ITelemetryRepository, TelemetryRepository>();
builder.Services.AddScoped<TelemetryConsumer>();

builder.Services.Configure<RabbitMqOptions>(builder.Configuration.GetSection("RabbitMQ"));
builder.Services.AddHostedService(serviceProvider =>
    new TelemetryListener(
        serviceProvider,
        serviceProvider.GetRequiredService<IOptions<RabbitMqOptions>>().Value,
        serviceProvider.GetRequiredService<ILogger<TelemetryListener>>()));

builder.Logging.AddOpenTelemetry(logging =>
{
    logging.IncludeScopes = true;
    logging.AddConsoleExporter();
});
builder.Services.AddOpenTelemetry()
    .ConfigureResource(r => r.AddService("TelemetryProcessor"))
    .WithMetrics(metrics => metrics
        .AddRuntimeInstrumentation()
        .AddConsoleExporter());

var host = builder.Build();
host.Run();
