using MessageProcessingAndAnomalyDetectionService.Configurations;
using MessageProcessingAndAnomalyDetectionService.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shared.Models;

namespace MessageProcessingAndAnomalyDetectionService.AlertHandlers;

public class MemoryUsageAnomalyAlertHandler : IAlertHandler
{
  private readonly IOptions<AnomalyDetectionConfig> _anomalyDetectionConfig;

  private readonly ISignalRClientSender _signalRClientSender;

  private readonly ILogger<MemoryUsageAnomalyAlertHandler> _logger;

  private readonly IStatisticsRepository _statisticsRepository;

  public MemoryUsageAnomalyAlertHandler(
    IOptions<AnomalyDetectionConfig> anomalyDetectionConfig,
    ISignalRClientSender signalRClientSender,
    IStatisticsRepository statisticsRepository,
    ILogger<MemoryUsageAnomalyAlertHandler> logger)
  {
    _anomalyDetectionConfig = anomalyDetectionConfig;
    _signalRClientSender = signalRClientSender;
    _statisticsRepository = statisticsRepository;
    _logger = logger;
  }

  public async Task<bool> CheckCondition(ServerStatistics statistics)
  {
    if (statistics is null) throw new ArgumentNullException(nameof(statistics));

    var previousStatistics = await _statisticsRepository.GetLastForAsync(statistics.ServerIdentifier);

    if (previousStatistics is null) return false;

    return statistics.MemoryUsage > previousStatistics.MemoryUsage *
      (1 + _anomalyDetectionConfig.Value.MemoryUsageAnomalyThresholdPercentage);
  }

  public async Task Handle(ServerStatistics statistics)
  {
    if (!await CheckCondition(statistics)) return;

    try
    {
      _logger.LogInformation($"Sending Memory Anomaly Alert Message at {DateTime.Now}");
      
      await _signalRClientSender.SendAlertAsync(statistics.ServerIdentifier, AlertMessages.MemoryUsageAnomaly, statistics.Timestamp);

      _logger.LogInformation($"Sent Memory Anomaly Alert Message Successfully at {DateTime.Now}");
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, $"Error while Sending memory alert message at {DateTime.Now}");
    }
  }
}