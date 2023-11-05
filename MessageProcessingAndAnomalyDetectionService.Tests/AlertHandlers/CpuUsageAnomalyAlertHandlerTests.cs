using MessageProcessingAndAnomalyDetectionService.AlertHandlers;
using MessageProcessingAndAnomalyDetectionService.Configurations;
using MessageProcessingAndAnomalyDetectionService.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shared.Models;

namespace MessageProcessingAndAnomalyDetectionService.Tests.AlertHandlers;

public class CpuUsageAnomalyAlertHandlerTests
{
  private readonly Mock<IOptions<AnomalyDetectionConfig>> _anomalyDetectionConfigMock;

  private readonly Fixture _fixture;

  private readonly Mock<ISignalRClientSender> _signalRClientSenderMock;

  private readonly Mock<IStatisticsRepository> _statisticsRepositoryMock;

  private readonly CpuUsageAnomalyAlertHandler _sut;

  public CpuUsageAnomalyAlertHandlerTests()
  {
    _signalRClientSenderMock = new Mock<ISignalRClientSender>();

    _statisticsRepositoryMock = new Mock<IStatisticsRepository>();

    _anomalyDetectionConfigMock = new Mock<IOptions<AnomalyDetectionConfig>>();

    var loggerMock = new Mock<ILogger<CpuUsageAnomalyAlertHandler>>();

    _fixture = new Fixture();

    _sut = new CpuUsageAnomalyAlertHandler(
      _statisticsRepositoryMock.Object,
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

  [Fact]
  public async Task CheckCondition_NoPreviousStatistics_ShouldReturnFalse()
  {
    var statistics = _fixture.Create<ServerStatistics>();

    var actual = await _sut.CheckCondition(statistics);

    actual.Should().BeFalse();
  }

  [Theory]
  [InlineData(0.9, 0.3, 0.1)]
  [InlineData(0.8, 0.5, 0.2)]
  public async Task CheckCondition_ConditionIsMet_ShouldReturnTrue(double currentCpuUsage, double previousCpuUsage,
    double cpuUsageAnomalyThresholdPercentage)
  {
    SetupConfigAndStatistics(previousCpuUsage, cpuUsageAnomalyThresholdPercentage);

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

  private void SetupConfigAndStatistics(double previousCpuUsage, double cpuUsageAnomalyThresholdPercentage)
  {
    var config = _fixture.Build<AnomalyDetectionConfig>()
      .With(x => x.CpuUsageAnomalyThresholdPercentage, cpuUsageAnomalyThresholdPercentage)
      .Create();

    _anomalyDetectionConfigMock
      .SetupGet(x => x.Value)
      .Returns(config);

    var previousStatistics = _fixture.Build<ServerStatistics>()
      .With(x => x.CpuUsage, previousCpuUsage)
      .Create();

    _statisticsRepositoryMock
      .Setup(x => x.GetLastForAsync(It.IsAny<string>()))
      .ReturnsAsync(previousStatistics);
  }

  [Theory]
  [InlineData(0.2, 0.3, 0.1)]
  [InlineData(0.2, 0.5, 0.2)]
  public async Task CheckCondition_ConditionIsNotMet_ShouldReturnFalse(double currentCpuUsage,
    double previousCpuUsage,
    double cpuUsageAnomalyThresholdPercentage)
  {
    SetupConfigAndStatistics(previousCpuUsage, cpuUsageAnomalyThresholdPercentage);

    var currentStatistics = GetCurrentStatistics(currentCpuUsage);

    var actual = await _sut.CheckCondition(currentStatistics);

    actual.Should().BeFalse();
  }

  [Theory]
  [InlineData(0.9, 0.3, 0.1)]
  [InlineData(0.8, 0.5, 0.2)]
  public async Task Handle_ConditionIsMet_ShouldSendAlertMessage(double currentCpuUsage,
    double previousCpuUsage,
    double cpuUsageAnomalyThresholdPercentage)
  {
    SetupConfigAndStatistics(previousCpuUsage, cpuUsageAnomalyThresholdPercentage);

    var currentStatistics = GetCurrentStatistics(currentCpuUsage);

    await _sut.Handle(currentStatistics);

    _signalRClientSenderMock.Verify(
      x => x.SendAlertAsync(currentStatistics.ServerIdentifier, AlertMessages.CpuUsageAnomaly, currentStatistics.Timestamp), Times.Once);
  }

  [Theory]
  [InlineData(0.2, 0.3, 0.1)]
  [InlineData(0.2, 0.5, 0.2)]
  public async Task Handle_ConditionIsNotMet_ShouldNotSendAlertMessage(double currentCpuUsage,
    double previousCpuUsage,
    double cpuUsageAnomalyThresholdPercentage)
  {
    SetupConfigAndStatistics(previousCpuUsage, cpuUsageAnomalyThresholdPercentage);

    var currentStatistics = GetCurrentStatistics(currentCpuUsage);

    await _sut.Handle(currentStatistics);

    _signalRClientSenderMock.Verify(
      x => x.SendAlertAsync(currentStatistics.ServerIdentifier, AlertMessages.CpuUsageAnomaly, currentStatistics.Timestamp), Times.Never);
  }
}