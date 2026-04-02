using TelemetryProcessor.Domain;

namespace TelemetryProcessor.Repositories;

public interface ITelemetryRepository
{
    Task SaveAsync(TelemetryRecord record, CancellationToken ct = default);
}
