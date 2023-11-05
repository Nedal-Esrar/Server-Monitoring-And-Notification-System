using MessageProcessingAndAnomalyDetectionService.AlertHandlers;
using MessageProcessingAndAnomalyDetectionService.Configurations;
using MessageProcessingAndAnomalyDetectionService.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shared.Models;

namespace MessageProcessingAndAnomalyDetectionService.Tests.AlertHandlers;

public class MemoryUsageAnomalyAlertHandlerTests
{
  private readonly Mock<IOptions<AnomalyDetectionConfig>> _anomalyDetectionConfigMock;

  private readonly Fixture _fixture;

  private readonly Mock<ISignalRClientSender> _signalRClientSenderMock;

  private readonly Mock<IStatisticsRepository> _statisticsRepositoryMock;

  private readonly MemoryUsageAnomalyAlertHandler _sut;

  public MemoryUsageAnomalyAlertHandlerTests()
  {
    _signalRClientSenderMock = new Mock<ISignalRClientSender>();

    _statisticsRepositoryMock = new Mock<IStatisticsRepository>();

    _anomalyDetectionConfigMock = new Mock<IOptions<AnomalyDetectionConfig>>();

    var loggerMock = new Mock<ILogger<MemoryUsageAnomalyAlertHandler>>();

    _fixture = new Fixture();

    _sut = new MemoryUsageAnomalyAlertHandler(
      _anomalyDetectionConfigMock.Object,
      _signalRClientSenderMock.Object,
      _statisticsRepositoryMock.Object,
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
  [InlineData(90.0, 30.0, 0.1)]
  [InlineData(80.0, 50.0, 0.2)]
  public async Task CheckCondition_ConditionIsMet_ShouldReturnTrue(double currentMemoryUsage,
    double previousMemoryUsage,
    double memoryUsageAnomalyThresholdPercentage)
  {
    SetupConfigAndStatistics(previousMemoryUsage, memoryUsageAnomalyThresholdPercentage);

    var currentStatistics = GetCurrentStatistics(currentMemoryUsage);

    var actual = await _sut.CheckCondition(currentStatistics);

    actual.Should().BeTrue();
  }

  private ServerStatistics GetCurrentStatistics(double currentMemoryUsage)
  {
    return _fixture.Build<ServerStatistics>()
      .With(x => x.MemoryUsage, currentMemoryUsage)
      .Create();
  }

  private void SetupConfigAndStatistics(double previousMemoryUsage, double memoryUsageAnomalyThresholdPercentage)
  {
    var config = _fixture.Build<AnomalyDetectionConfig>()
      .With(x => x.MemoryUsageAnomalyThresholdPercentage, memoryUsageAnomalyThresholdPercentage)
      .Create();

    _anomalyDetectionConfigMock
      .SetupGet(x => x.Value)
      .Returns(config);

    var previousStatistics = _fixture.Build<ServerStatistics>()
      .With(x => x.MemoryUsage, previousMemoryUsage)
      .Create();

    _statisticsRepositoryMock
      .Setup(x => x.GetLastForAsync(It.IsAny<string>()))
      .ReturnsAsync(previousStatistics);
  }

  [Theory]
  [InlineData(20.0, 30.0, 0.1)]
  [InlineData(20.0, 50.0, 0.2)]
  public async Task CheckCondition_ConditionIsNotMet_ShouldReturnFalse(double currentMemoryUsage,
    double previousMemoryUsage,
    double memoryUsageAnomalyThresholdPercentage)
  {
    SetupConfigAndStatistics(previousMemoryUsage, memoryUsageAnomalyThresholdPercentage);

    var currentStatistics = GetCurrentStatistics(currentMemoryUsage);

    var actual = await _sut.CheckCondition(currentStatistics);

    actual.Should().BeFalse();
  }

  [Theory]
  [InlineData(90.0, 30.0, 0.1)]
  [InlineData(80.0, 50.0, 0.2)]
  public async Task CheckCondition_ConditionIsMet_ShouldSendAlertMessage(double currentMemoryUsage,
    double previousMemoryUsage,
    double memoryUsageAnomalyThresholdPercentage)
  {
    SetupConfigAndStatistics(previousMemoryUsage, memoryUsageAnomalyThresholdPercentage);

    var currentStatistics = GetCurrentStatistics(currentMemoryUsage);

    await _sut.Handle(currentStatistics);

    _signalRClientSenderMock.Verify(
      x => x.SendAlertAsync(currentStatistics.ServerIdentifier, AlertMessages.MemoryUsageAnomaly, currentStatistics.Timestamp), Times.Once);
  }

  [Theory]
  [InlineData(20.0, 30.0, 0.1)]
  [InlineData(20.0, 50.0, 0.2)]
  public async Task CheckCondition_ConditionIsNotMet_ShouldNotSendAlertMessage(double currentMemoryUsage,
    double previousMemoryUsage,
    double memoryUsageAnomalyThresholdPercentage)
  {
    SetupConfigAndStatistics(previousMemoryUsage, memoryUsageAnomalyThresholdPercentage);

    var currentStatistics = GetCurrentStatistics(currentMemoryUsage);

    await _sut.Handle(currentStatistics);

    _signalRClientSenderMock.Verify(
      x => x.SendAlertAsync(currentStatistics.ServerIdentifier, AlertMessages.MemoryUsageAnomaly, currentStatistics.Timestamp), Times.Never);
  }
}