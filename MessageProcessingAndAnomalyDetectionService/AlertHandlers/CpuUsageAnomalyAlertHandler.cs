using MessageProcessingAndAnomalyDetectionService.Configurations;
using MessageProcessingAndAnomalyDetectionService.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shared.Models;

namespace MessageProcessingAndAnomalyDetectionService.AlertHandlers;

public class CpuUsageAnomalyAlertHandler : IAlertHandler
{
  private readonly IOptions<AnomalyDetectionConfig> _anomalyDetectionConfig;

  private readonly ISignalRClientSender _signalRClientSender;

  private readonly ILogger<CpuUsageAnomalyAlertHandler> _logger;

  private readonly IStatisticsRepository _statisticsRepository;

  public CpuUsageAnomalyAlertHandler(
    IStatisticsRepository statisticsRepository,
    IOptions<AnomalyDetectionConfig> anomalyDetectionConfig,
    ISignalRClientSender signalRClientSender,
    ILogger<CpuUsageAnomalyAlertHandler> logger)
  {
    _statisticsRepository = statisticsRepository;

    _anomalyDetectionConfig = anomalyDetectionConfig;

    _signalRClientSender = signalRClientSender;

    _logger = logger;
  }

  public async Task<bool> CheckCondition(ServerStatistics statistics)
  {
    if (statistics is null) throw new ArgumentNullException(nameof(statistics));

    var previousStatistics = await _statisticsRepository.GetLastForAsync(statistics.ServerIdentifier);

    if (previousStatistics is null) return false;

    return statistics.CpuUsage >
           previousStatistics.CpuUsage * (1 + _anomalyDetectionConfig.Value.CpuUsageAnomalyThresholdPercentage);
  }

  public async Task Handle(ServerStatistics statistics)
  {
    if (!await CheckCondition(statistics)) return;

    try
    {
      _logger.LogInformation($"Sending Cpu Anomaly Alert Message at {DateTime.Now}");
      
      await _signalRClientSender.SendAlertAsync(statistics.ServerIdentifier, AlertMessages.CpuUsageAnomaly, statistics.Timestamp);

      _logger.LogInformation($"Sent Cpu Anomaly Alert Message Successfully at {DateTime.Now}");
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, $"Error while Sending cpu alert message at {DateTime.Now}");
    }
  }
}