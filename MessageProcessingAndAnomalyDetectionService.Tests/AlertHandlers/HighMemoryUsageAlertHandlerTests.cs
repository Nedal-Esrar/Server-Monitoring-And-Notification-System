using MessageProcessingAndAnomalyDetectionService.AlertHandlers;
using MessageProcessingAndAnomalyDetectionService.Configurations;
using MessageProcessingAndAnomalyDetectionService.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shared.Models;

namespace MessageProcessingAndAnomalyDetectionService.Tests.AlertHandlers;

public class HighMemoryUsageAlertHandlerTests
{
  private readonly Mock<IOptions<AnomalyDetectionConfig>> _anomalyDetectionConfigMock;

  private readonly Fixture _fixture;

  private readonly Mock<ISignalRClientSender> _signalRClientSenderMock;

  private readonly HighMemoryUsageAlertHandler _sut;

  public HighMemoryUsageAlertHandlerTests()
  {
    _signalRClientSenderMock = new Mock<ISignalRClientSender>();

    _anomalyDetectionConfigMock = new Mock<IOptions<AnomalyDetectionConfig>>();

    var loggerMock = new Mock<ILogger<HighMemoryUsageAlertHandler>>();

    _fixture = new Fixture();

    _sut = new HighMemoryUsageAlertHandler(
      _signalRClientSenderMock.Object,
      _anomalyDetectionConfigMock.Object,
      loggerMock.Object);
  }

  [Fact]
  public async Task CheckCondition_PassedObjectIsNull_ThrowsArgumentNullException()
  {
    var act = () => _sut.CheckCondition(null);

    await act.Should().ThrowAsync<ArgumentNullException>();
  }

  [Theory]
  [InlineData(100.0, 101.0, 0.25)]
  [InlineData(50.0, 200.0, 0.1)]
  public async Task CheckCondition_ConditionIsMet_ShouldReturnTrue(double currentMemoryUsage, double availableMemory,
    double memoryUsageThresholdPercentage)
  {
    SetupConfig(memoryUsageThresholdPercentage);

    var currentStatistics = GetCurrentStatistics(currentMemoryUsage, availableMemory);

    var actual = await _sut.CheckCondition(currentStatistics);

    actual.Should().BeTrue();
  }

  private ServerStatistics GetCurrentStatistics(double currentMemoryUsage, double availableMemory)
  {
    return _fixture.Build<ServerStatistics>()
      .With(x => x.MemoryUsage, currentMemoryUsage)
      .With(x => x.AvailableMemory, availableMemory)
      .Create();
  }

  private void SetupConfig(double memoryUsageThresholdPercentage)
  {
    var config = _fixture.Build<AnomalyDetectionConfig>()
      .With(x => x.MemoryUsageThresholdPercentage, memoryUsageThresholdPercentage)
      .Create();

    _anomalyDetectionConfigMock
      .SetupGet(x => x.Value)
      .Returns(config);
  }

  [Theory]
  [InlineData(100.0, 50.0, 0.8)]
  [InlineData(50.0, 200.0, 0.3)]
  public async Task CheckCondition_ConditionIsNotMet_ShouldReturnFalse(double currentMemoryUsage,
    double availableMemory,
    double memoryUsageThresholdPercentage)
  {
    SetupConfig(memoryUsageThresholdPercentage);

    var currentStatistics = GetCurrentStatistics(currentMemoryUsage, availableMemory);

    var actual = await _sut.CheckCondition(currentStatistics);

    actual.Should().BeFalse();
  }

  [Theory]
  [InlineData(100.0, 101.0, 0.25)]
  [InlineData(50.0, 200.0, 0.1)]
  public async Task CheckCondition_ConditionIsMet_ShouldSendAlertMessage(double currentMemoryUsage,
    double availableMemory,
    double memoryUsageThresholdPercentage)
  {
    SetupConfig(memoryUsageThresholdPercentage);

    var currentStatistics = GetCurrentStatistics(currentMemoryUsage, availableMemory);

    await _sut.Handle(currentStatistics);

    _signalRClientSenderMock.Verify(
      x => x.SendAlertAsync(currentStatistics.ServerIdentifier, AlertMessages.HighMemoryUsage, currentStatistics.Timestamp), Times.Once);
  }

  [Theory]
  [InlineData(100.0, 50.0, 0.8)]
  [InlineData(50.0, 200.0, 0.3)]
  public async Task CheckCondition_ConditionIsNotMet_ShouldNotSendAlertMessage(double currentMemoryUsage,
    double availableMemory,
    double memoryUsageThresholdPercentage)
  {
    SetupConfig(memoryUsageThresholdPercentage);

    var currentStatistics = GetCurrentStatistics(currentMemoryUsage, availableMemory);

    await _sut.Handle(currentStatistics);

    _signalRClientSenderMock.Verify(
      x => x.SendAlertAsync(currentStatistics.ServerIdentifier, AlertMessages.HighMemoryUsage, currentStatistics.Timestamp), Times.Never);
  }
}