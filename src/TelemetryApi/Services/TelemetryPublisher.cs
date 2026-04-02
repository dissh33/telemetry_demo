using RabbitMQ.Client;
using System.Text.Json;
using TelemetryApi.Options;
using TelemetryContracts.Messages;

namespace TelemetryApi.Services;

public sealed class TelemetryPublisher : ITelemetryPublisher, IDisposable
{
    private readonly IConnection _connection;
    private readonly string _queue;

    public TelemetryPublisher(RabbitMqOptions options)
    {
        _queue = options.Queue;
        _connection = new ConnectionFactory
        {
            HostName    = options.Host,
            VirtualHost = options.VirtualHost,
            UserName    = options.Username,
            Password    = options.Password
        }.CreateConnection();
    }

    public Task PublishAsync(TelemetryMessage message, CancellationToken ct = default)
    {
        using var channel = _connection.CreateModel();

        var props = channel.CreateBasicProperties();
        props.Persistent = true;

        channel.BasicPublish(
            exchange:        "",
            routingKey:      _queue,
            basicProperties: props,
            body:            JsonSerializer.SerializeToUtf8Bytes(message));

        return Task.CompletedTask;
    }

    public void Dispose() => _connection.Dispose();
}
