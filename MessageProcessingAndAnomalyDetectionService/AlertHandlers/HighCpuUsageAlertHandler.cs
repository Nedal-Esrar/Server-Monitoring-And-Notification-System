using MessageProcessingAndAnomalyDetectionService.Configurations;
using MessageProcessingAndAnomalyDetectionService.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shared.Models;

namespace MessageProcessingAndAnomalyDetectionService.AlertHandlers;

public class HighCpuUsageAlertHandler : IAlertHandler
{
  private readonly IOptions<AnomalyDetectionConfig> _anomalyDetectionConfig;

  private readonly ISignalRClientSender _signalRClientSender;

  private readonly ILogger<HighCpuUsageAlertHandler> _logger;

  public HighCpuUsageAlertHandler(IOptions<AnomalyDetectionConfig> anomalyDetectionConfig,
    ISignalRClientSender signalRClientSender,
    ILogger<HighCpuUsageAlertHandler> logger)
  {
    _anomalyDetectionConfig = anomalyDetectionConfig;
    _signalRClientSender = signalRClientSender;
    _logger = logger;
  }

  public async Task<bool> CheckCondition(ServerStatistics statistics)
  {
    if (statistics is null) throw new ArgumentNullException(nameof(statistics));

    return await Task.FromResult(statistics.CpuUsage > _anomalyDetectionConfig.Value.CpuUsageThresholdPercentage);
  }

  public async Task Handle(ServerStatistics statistics)
  {
    if (!await CheckCondition(statistics)) return;

    try
    {
      _logger.LogInformation($"Sending High Cpu Usage Alert Message at {DateTime.Now}");
      
      await _signalRClientSender.SendAlertAsync(statistics.ServerIdentifier, AlertMessages.HighCpuUsage, statistics.Timestamp);

      _logger.LogInformation($"Sent High Cpu Usage Alert Message Successfully at {DateTime.Now}");
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, $"Error while Sending high cpu usage alert message at {DateTime.Now}");
    }
  }
}