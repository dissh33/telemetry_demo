using System.ComponentModel.DataAnnotations;
using System.Text.Json.Nodes;
using TelemetryContracts.Messages;

namespace TelemetryApi.Models.Requests;

public record SubmitTelemetryRequest
{
    [Required]
    public Guid VehicleId { get; init; }

    public Guid? DeviceId { get; init; }

    [Required]
    public TelemetryType Type { get; init; }

    [Required]
    public DateTimeOffset RecordedAt { get; init; }

    [Required]
    public JsonObject Payload { get; init; } = new();
}
