namespace TelemetryProcessor.Domain;

public class TelemetryRecord
{
    public Guid Id { get; set; }
    public Guid VehicleId { get; set; }
    public Guid? DeviceId { get; set; }
    public string TelemetryType { get; set; } = string.Empty;
    public string PayloadJson { get; set; } = string.Empty;
    public DateTimeOffset RecordedAt { get; set; }
    public DateTimeOffset ReceivedAt { get; set; }
}
