using MessageProcessingAndAnomalyDetectionService.Configurations;
using MessageProcessingAndAnomalyDetectionService.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shared.Models;

namespace MessageProcessingAndAnomalyDetectionService.AlertHandlers;

public class HighMemoryUsageAlertHandler : IAlertHandler
{
  private readonly IOptions<AnomalyDetectionConfig> _anomalyDetectionConfig;

  private readonly ISignalRClientSender _signalRClientSender;

  private readonly ILogger<HighMemoryUsageAlertHandler> _logger;

  public HighMemoryUsageAlertHandler(ISignalRClientSender signalRClientSender,
    IOptions<AnomalyDetectionConfig> anomalyDetectionConfig,
    ILogger<HighMemoryUsageAlertHandler> logger)
  {
    _signalRClientSender = signalRClientSender;
    _anomalyDetectionConfig = anomalyDetectionConfig;
    _logger = logger;
  }

  public async Task<bool> CheckCondition(ServerStatistics statistics)
  {
    if (statistics is null) throw new ArgumentNullException(nameof(statistics));

    return await Task.FromResult(statistics.MemoryUsage / (statistics.MemoryUsage + statistics.AvailableMemory) >
                                 _anomalyDetectionConfig.Value.MemoryUsageThresholdPercentage);
  }

  public async Task Handle(ServerStatistics statistics)
  {
    if (!await CheckCondition(statistics)) return;

    try
    {
      _logger.LogInformation($"Sending high memory usage Alert Message at {DateTime.Now}");
      
      await _signalRClientSender.SendAlertAsync(statistics.ServerIdentifier, AlertMessages.HighMemoryUsage, statistics.Timestamp);

      _logger.LogInformation($"Sent high memory usage Alert Message Successfully at {DateTime.Now}");
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, $"Error while Sending high memory usage alert message at {DateTime.Now}");
    }
  }
}