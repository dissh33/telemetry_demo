using TelemetryApi.Models.Requests;

namespace TelemetryApi.Services;

public interface ITelemetryService
{
    Task<Guid> SubmitAsync(SubmitTelemetryRequest request, CancellationToken ct = default);
}
