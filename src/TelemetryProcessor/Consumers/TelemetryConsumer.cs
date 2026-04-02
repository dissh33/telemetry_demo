using TelemetryContracts.Messages;
using TelemetryProcessor.Domain;
using TelemetryProcessor.Repositories;

namespace TelemetryProcessor.Consumers;

public class TelemetryConsumer(ITelemetryRepository repository, ILogger<TelemetryConsumer> logger)
{
    public async Task ProcessAsync(TelemetryMessage message, CancellationToken ct = default)
    {
        logger.LogInformation(
            "Received telemetry {Type} for vehicle {VehicleId}, message {MessageId}",
            message.Type, message.VehicleId, message.MessageId);

        var record = new TelemetryRecord
        {
            Id = message.MessageId,
            VehicleId = message.VehicleId,
            DeviceId = message.DeviceId,
            TelemetryType = message.Type.ToString(),
            PayloadJson = message.PayloadJson,
            RecordedAt = message.RecordedAt,
            ReceivedAt = DateTimeOffset.UtcNow
        };

        await repository.SaveAsync(record, ct);

        logger.LogInformation(
            "Telemetry {Type} saved for vehicle {VehicleId}",
            message.Type, message.VehicleId);
    }
}
