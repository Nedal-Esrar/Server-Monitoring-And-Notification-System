using MessageBroker.Interfaces;
using MessageProcessingAndAnomalyDetectionService.Interfaces;
using Microsoft.Extensions.Logging;
using Shared.Models;
using Shared.Utilities;

namespace MessageProcessingAndAnomalyDetectionService.Services;

public class AnomalyMonitorService : IAnomalyMonitorService
{
  private readonly IAnomalyDetectionService _anomalyDetectionService;

  private readonly ILogger<IAnomalyMonitorService> _logger;

  private readonly IQueueSubscriber _queueSubscriber;

  private readonly IStatisticsRepository _statisticsRepository;

  public AnomalyMonitorService(IAnomalyDetectionService anomalyDetectionService, IQueueSubscriber queueSubscriber,
    IStatisticsRepository statisticsRepository, ILogger<IAnomalyMonitorService> logger)
  {
    _anomalyDetectionService = anomalyDetectionService;
    _queueSubscriber = queueSubscriber;
    _statisticsRepository = statisticsRepository;
    _logger = logger;
  }

  public async Task RunAsync()
  {
    _logger.LogInformation($"Service started at {DateTime.Now}");

    try
    {
      _logger.LogInformation($"Subscribing to the statistics queue at {DateTime.Now}");
        
      await _queueSubscriber.Subscribe<ServerStatistics>(OnStatisticsReceived);
      
      _logger.LogInformation($"Subscribed Successfully to the statistics queue at {DateTime.Now}");
      
      Console.ReadLine();
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, $"An Error occurred while subscribing to the message queue at {DateTime.Now}");
    }
  }

  private async Task OnStatisticsReceived(ServerStatistics statistics)
  {
    _logger.LogInformation($"""
                            Received the following statistics at {DateTime.Now}:
                            {StandardMessages.FormatStatisticsForLogging(statistics)}
                            """);

    try
    {
      _logger.LogInformation($"Checking for anomalies at {DateTime.Now}");
      
      await _anomalyDetectionService.CheckForAnomalies(statistics);

      _logger.LogInformation($"Finished Checking for anomalies at {DateTime.Now}");
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, $"An error occurred while checking for anomalies at {DateTime.Now}");
    }

    try
    {
      _logger.LogInformation($"Saving statistics at {DateTime.Now}");
      
      await _statisticsRepository.SaveAsync(statistics);

      _logger.LogInformation($"Saved statistics successfully at {DateTime.Now}");
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, $"An error occurred while saving statistics at {DateTime.Now}");
    }
  }
}