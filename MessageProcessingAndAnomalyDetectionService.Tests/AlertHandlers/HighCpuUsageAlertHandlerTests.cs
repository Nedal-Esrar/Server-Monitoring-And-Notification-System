using MessageProcessingAndAnomalyDetectionService.AlertHandlers;
using MessageProcessingAndAnomalyDetectionService.Configurations;
using MessageProcessingAndAnomalyDetectionService.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shared.Models;

namespace MessageProcessingAndAnomalyDetectionService.Tests.AlertHandlers;

public class HighCpuUsageAlertHandlerTests
{
  private readonly Mock<IOptions<AnomalyDetectionConfig>> _anomalyDetectionConfigMock;

  private readonly Fixture _fixture;

  private readonly Mock<ISignalRClientSender> _signalRClientSenderMock;

  private readonly HighCpuUsageAlertHandler _sut;

  public HighCpuUsageAlertHandlerTests()
  {
    _signalRClientSenderMock = new Mock<ISignalRClientSender>();

    _anomalyDetectionConfigMock = new Mock<IOptions<AnomalyDetectionConfig>>();

    var loggerMock = new Mock<ILogger<HighCpuUsageAlertHandler>>();

    _fixture = new Fixture();

    _sut = new HighCpuUsageAlertHandler(
      _anomalyDetectionConfigMock.Object,
      _signalRClientSenderMock.Object,
      loggerMock.Object);
  }

  [Fact]
  public async Task CheckCondition_PassedObjectIsNull_ThrowsArgumentNullException()
  {
    var act = () => _sut.CheckCondition(null);

    await act.Should().ThrowAsync<ArgumentNullException>();
  }

  [Theory]
  [InlineData(0.9, 0.1)]
  [InlineData(0.8, 0.2)]
  public async Task CheckCondition_ConditionIsMet_ShouldReturnTrue(double currentCpuUsage,
    double cpuUsageThresholdPercentage)
  {
    SetupConfig(cpuUsageThresholdPercentage);

    var currentStatistics = GetCurrentStatistics(currentCpuUsage);

    var actual = await _sut.CheckCondition(currentStatistics);

    actual.Should().BeTrue();
  }

  private ServerStatistics GetCurrentStatistics(double currentCpuUsage)
  {
    return _fixture.Build<ServerStatistics>()
      .With(x => x.CpuUsage, currentCpuUsage)
      .Create();
  }

  private void SetupConfig(double cpuUsageThresholdPercentage)
  {
    var config = _fixture.Build<AnomalyDetectionConfig>()
      .With(x => x.CpuUsageThresholdPercentage, cpuUsageThresholdPercentage)
      .Create();

    _anomalyDetectionConfigMock
      .SetupGet(x => x.Value)
      .Returns(config);
  }

  [Theory]
  [InlineData(0.1, 0.5)]
  [InlineData(0.2, 0.2)]
  public async Task CheckCondition_ConditionIsNotMet_ShouldReturnFalse(double currentCpuUsage,
    double cpuUsageThresholdPercentage)
  {
    SetupConfig(cpuUsageThresholdPercentage);

    var currentStatistics = GetCurrentStatistics(currentCpuUsage);

    var actual = await _sut.CheckCondition(currentStatistics);

    actual.Should().BeFalse();
  }

  [Theory]
  [InlineData(0.9, 0.1)]
  [InlineData(0.8, 0.2)]
  public async Task CheckCondition_ConditionIsMet_ShouldSendAlertMessage(double currentCpuUsage,
    double cpuUsageThresholdPercentage)
  {
    SetupConfig(cpuUsageThresholdPercentage);

    var currentStatistics = GetCurrentStatistics(currentCpuUsage);

    await _sut.Handle(currentStatistics);

    _signalRClientSenderMock.Verify(
      x => x.SendAlertAsync(currentStatistics.ServerIdentifier, AlertMessages.HighCpuUsage, currentStatistics.Timestamp), Times.Once);
  }

  [Theory]
  [InlineData(0.1, 0.5)]
  [InlineData(0.2, 0.2)]
  public async Task CheckCondition_ConditionIsNotMet_ShouldNotSendAlertMessage(double currentCpuUsage,
    double cpuUsageThresholdPercentage)
  {
    SetupConfig(cpuUsageThresholdPercentage);

    var currentStatistics = GetCurrentStatistics(currentCpuUsage);

    await _sut.Handle(currentStatistics);

    _signalRClientSenderMock.Verify(
      x => x.SendAlertAsync(currentStatistics.ServerIdentifier, AlertMessages.HighCpuUsage, currentStatistics.Timestamp), Times.Never);
  }
}