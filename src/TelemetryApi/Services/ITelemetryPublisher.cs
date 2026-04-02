using TelemetryContracts.Messages;

namespace TelemetryApi.Services;

public interface ITelemetryPublisher
{
    Task PublishAsync(TelemetryMessage message, CancellationToken ct = default);
}
