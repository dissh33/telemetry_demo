using Microsoft.Extensions.Options;
using OpenTelemetry.Logs;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using TelemetryApi.Options;
using TelemetryApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
    c.SwaggerDoc("v1", new() { Title = "Telemetry API", Version = "v1" }));

builder.Services.Configure<RabbitMqOptions>(builder.Configuration.GetSection("RabbitMQ"));

builder.Services.AddScoped<ITelemetryService, TelemetryService>();
builder.Services.AddSingleton<ITelemetryPublisher>(sp =>
    new TelemetryPublisher(sp.GetRequiredService<IOptions<RabbitMqOptions>>().Value));

builder.Logging.AddOpenTelemetry(logging =>
{
    logging.IncludeScopes = true;
    logging.AddConsoleExporter();
});
builder.Services.AddOpenTelemetry()
    .ConfigureResource(r => r.AddService("TelemetryApi"))
    .WithTracing(tracing => tracing
        .AddAspNetCoreInstrumentation()
        .AddConsoleExporter());

var app = builder.Build();

app.UseExceptionHandler(errApp => errApp.Run(async ctx =>
{
    var ex = ctx.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>()?.Error;
    ctx.Response.StatusCode = StatusCodes.Status500InternalServerError;
    await ctx.Response.WriteAsJsonAsync(new { error = ex?.Message });
}));

app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();

app.Run();
