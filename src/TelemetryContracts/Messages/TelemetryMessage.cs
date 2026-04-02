namespace TelemetryContracts.Messages;

public enum TelemetryType
{
    Coordinates = 1,
    Speed = 2,
    LiquidSensor = 3,
    SubsystemStatus = 4
}

public record TelemetryMessage
{
    public Guid MessageId { get; init; } = Guid.NewGuid();
    public Guid VehicleId { get; init; }
    public Guid? DeviceId { get; init; }
    public TelemetryType Type { get; init; }
    public DateTimeOffset RecordedAt { get; init; }
    public string PayloadJson { get; init; } = string.Empty;
}
