using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text.Json;
using TelemetryContracts.Messages;
using TelemetryProcessor.Consumers;
using TelemetryProcessor.Options;

namespace TelemetryProcessor.Listeners;

public sealed class TelemetryListener : BackgroundService
{
    private readonly IServiceProvider _services;
    private readonly RabbitMqOptions _options;
    private readonly ILogger<TelemetryListener> _logger;
    private IConnection? _connection;
    private IModel? _channel;

    public TelemetryListener(IServiceProvider services, RabbitMqOptions options, ILogger<TelemetryListener> logger)
    {
        _services = services;
        _options  = options;
        _logger   = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _connection = new ConnectionFactory
        {
            HostName               = _options.Host,
            VirtualHost            = _options.VirtualHost,
            UserName               = _options.Username,
            Password               = _options.Password,
            DispatchConsumersAsync = true
        }.CreateConnection();

        _channel = _connection.CreateModel();
        _channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.Received += async (_, ea) =>
        {
            try
            {
                var message = JsonSerializer.Deserialize<TelemetryMessage>(ea.Body.Span)!;

                using var scope = _services.CreateScope();
                await scope.ServiceProvider
                    .GetRequiredService<TelemetryConsumer>()
                    .ProcessAsync(message, stoppingToken);

                _channel.BasicAck(ea.DeliveryTag, multiple: false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process telemetry message");
                _channel.BasicNack(ea.DeliveryTag, multiple: false, requeue: true);
            }
        };

        _channel.BasicConsume(_options.Queue, autoAck: false, consumer: consumer);
        _logger.LogInformation("TelemetryListener started on queue '{Queue}'", _options.Queue);

        var tcs = new TaskCompletionSource();
        await using var _ = stoppingToken.Register(() => tcs.TrySetResult());
        await tcs.Task;
    }

    public override void Dispose()
    {
        _channel?.Close();
        _connection?.Close();
        base.Dispose();
    }
}
