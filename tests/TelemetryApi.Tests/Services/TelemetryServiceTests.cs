using FluentAssertions;
using Moq;
using System.Text.Json.Nodes;
using TelemetryApi.Models.Requests;
using TelemetryApi.Services;
using TelemetryContracts.Messages;

namespace TelemetryApi.Tests.Services;

public class TelemetryServiceTests
{
    private readonly Mock<ITelemetryPublisher> _publisher = new();
    private readonly TelemetryService _sut;

    public TelemetryServiceTests() =>
        _sut = new TelemetryService(_publisher.Object);

    [Fact]
    public async Task Submit_ValidRequest_ReturnsMessageId()
    {
        var vehicleId = Guid.NewGuid();

        var messageId = await _sut.SubmitAsync(new SubmitTelemetryRequest
        {
            VehicleId = vehicleId,
            Type = TelemetryType.Speed,
            RecordedAt = DateTimeOffset.UtcNow,
            Payload = new JsonObject { ["speedKmh"] = 90 }
        });

        messageId.Should().NotBeEmpty();
        _publisher.Verify(p => p.PublishAsync(
            It.Is<TelemetryMessage>(m => m.VehicleId == vehicleId && m.Type == TelemetryType.Speed),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory]
    [InlineData(TelemetryType.Coordinates)]
    [InlineData(TelemetryType.Speed)]
    [InlineData(TelemetryType.LiquidSensor)]
    [InlineData(TelemetryType.SubsystemStatus)]
    public async Task Submit_AllTypes_PublishesCorrectType(TelemetryType type)
    {
        await _sut.SubmitAsync(new SubmitTelemetryRequest
        {
            VehicleId = Guid.NewGuid(), Type = type,
            RecordedAt = DateTimeOffset.UtcNow, Payload = new JsonObject()
        });

        _publisher.Verify(p => p.PublishAsync(
            It.Is<TelemetryMessage>(m => m.Type == type),
            It.IsAny<CancellationToken>()), Times.Once);
    }
}
