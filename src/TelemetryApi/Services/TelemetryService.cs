using TelemetryApi.Models.Requests;
using TelemetryContracts.Messages;

namespace TelemetryApi.Services;

public class TelemetryService(ITelemetryPublisher publisher) : ITelemetryService
{
    public async Task<Guid> SubmitAsync(SubmitTelemetryRequest request, CancellationToken ct = default)
    {
        var message = new TelemetryMessage
        {
            MessageId = Guid.NewGuid(),
            VehicleId = request.VehicleId,
            DeviceId = request.DeviceId,
            Type = request.Type,
            RecordedAt = request.RecordedAt,
            PayloadJson = request.Payload.ToJsonString()
        };

        await publisher.PublishAsync(message, ct);
        return message.MessageId;
    }
}
