using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using TelemetryContracts.Messages;
using TelemetryProcessor.Consumers;
using TelemetryProcessor.Domain;
using TelemetryProcessor.Repositories;

namespace TelemetryProcessor.Tests.Consumers;

public class TelemetryConsumerTests
{
    private readonly Mock<ITelemetryRepository> _repositoryMock = new();
    private readonly TelemetryConsumer _sut;

    public TelemetryConsumerTests()
    {
        _sut = new TelemetryConsumer(_repositoryMock.Object, NullLogger<TelemetryConsumer>.Instance);
    }

    [Fact]
    public async Task ProcessAsync_ValidMessage_SavesRecord()
    {
        var vehicleId = Guid.NewGuid();
        var messageId = Guid.NewGuid();
        var recordedAt = DateTimeOffset.UtcNow.AddMinutes(-1);
        const string payloadJson = """{"speedKmh":95.5,"headingDegrees":180.0}""";

        var message = new TelemetryMessage
        {
            MessageId = messageId,
            VehicleId = vehicleId,
            Type = TelemetryType.Speed,
            RecordedAt = recordedAt,
            PayloadJson = payloadJson
        };

        TelemetryRecord? savedRecord = null;
        _repositoryMock.Setup(r => r.SaveAsync(It.IsAny<TelemetryRecord>(), It.IsAny<CancellationToken>()))
            .Callback<TelemetryRecord, CancellationToken>((r, _) => savedRecord = r)
            .Returns(Task.CompletedTask);

        await _sut.ProcessAsync(message);

        savedRecord.Should().NotBeNull();
        savedRecord!.Id.Should().Be(messageId);
        savedRecord.VehicleId.Should().Be(vehicleId);
        savedRecord.TelemetryType.Should().Be("Speed");
        savedRecord.PayloadJson.Should().Be(payloadJson);
        savedRecord.RecordedAt.Should().Be(recordedAt);
    }

    [Fact]
    public async Task ProcessAsync_WithDeviceId_SavesDeviceId()
    {
        var deviceId = Guid.NewGuid();
        var message = new TelemetryMessage
        {
            MessageId = Guid.NewGuid(),
            VehicleId = Guid.NewGuid(),
            DeviceId = deviceId,
            Type = TelemetryType.SubsystemStatus,
            RecordedAt = DateTimeOffset.UtcNow,
            PayloadJson = """{"subsystemName":"ADAS","isOperational":true}"""
        };

        TelemetryRecord? savedRecord = null;
        _repositoryMock.Setup(r => r.SaveAsync(It.IsAny<TelemetryRecord>(), It.IsAny<CancellationToken>()))
            .Callback<TelemetryRecord, CancellationToken>((r, _) => savedRecord = r)
            .Returns(Task.CompletedTask);

        await _sut.ProcessAsync(message);

        savedRecord!.DeviceId.Should().Be(deviceId);
    }

    [Fact]
    public async Task ProcessAsync_RepositoryThrows_ExceptionPropagates()
    {
        var message = new TelemetryMessage
        {
            MessageId = Guid.NewGuid(),
            VehicleId = Guid.NewGuid(),
            Type = TelemetryType.Coordinates,
            RecordedAt = DateTimeOffset.UtcNow,
            PayloadJson = "{}"
        };

        _repositoryMock.Setup(r => r.SaveAsync(It.IsAny<TelemetryRecord>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("DB error"));

        var act = () => _sut.ProcessAsync(message);
        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("DB error");
    }

    [Theory]
    [InlineData(TelemetryType.Coordinates, "Coordinates")]
    [InlineData(TelemetryType.Speed, "Speed")]
    [InlineData(TelemetryType.LiquidSensor, "LiquidSensor")]
    [InlineData(TelemetryType.SubsystemStatus, "SubsystemStatus")]
    public async Task ProcessAsync_AllTypes_SavesCorrectTypeName(TelemetryType type, string expectedTypeName)
    {
        var message = new TelemetryMessage
        {
            MessageId = Guid.NewGuid(),
            VehicleId = Guid.NewGuid(),
            Type = type,
            RecordedAt = DateTimeOffset.UtcNow,
            PayloadJson = "{}"
        };

        TelemetryRecord? savedRecord = null;
        _repositoryMock.Setup(r => r.SaveAsync(It.IsAny<TelemetryRecord>(), It.IsAny<CancellationToken>()))
            .Callback<TelemetryRecord, CancellationToken>((r, _) => savedRecord = r)
            .Returns(Task.CompletedTask);

        await _sut.ProcessAsync(message);

        savedRecord!.TelemetryType.Should().Be(expectedTypeName);
    }
}
