using Dapper;
using Npgsql;
using TelemetryProcessor.Domain;

namespace TelemetryProcessor.Repositories;

public class TelemetryRepository(NpgsqlDataSource dataSource) : ITelemetryRepository
{
    public async Task SaveAsync(TelemetryRecord record, CancellationToken ct = default)
    {
        const string sql = """
            INSERT INTO telemetry_records (id, vehicle_id, device_id, telemetry_type, payload, recorded_at, received_at)
            VALUES (@Id, @VehicleId, @DeviceId, @TelemetryType, @PayloadJson::jsonb, @RecordedAt, @ReceivedAt)
            """;

        await using var conn = await dataSource.OpenConnectionAsync(ct);
        await conn.ExecuteAsync(new CommandDefinition(sql, record, cancellationToken: ct));
    }
}
