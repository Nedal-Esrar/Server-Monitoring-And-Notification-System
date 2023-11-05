using System.Timers;
using MessageBroker.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ServerStatisticsCollectionService.Configurations;
using ServerStatisticsCollectionService.Interfaces;
using Shared.Models;
using Shared.Utilities;
using Timer = System.Timers.Timer;

namespace ServerStatisticsCollectionService.Services;

public class PeriodicStatisticsPublisherService : IPeriodicStatisticsPublisherService
{
  private readonly ILogger<IPeriodicStatisticsPublisherService> _logger;

  private readonly IQueuePublisher _queuePublisher;

  private readonly ServerStatisticsConfig _serverStatisticsConfig;

  private readonly IStatisticsCollector _statisticsCollector;

  public PeriodicStatisticsPublisherService(IOptions<ServerStatisticsConfig> serverStatisticsConfig,
    IQueuePublisher queuePublisher,
    IStatisticsCollector statisticsCollector, ILogger<IPeriodicStatisticsPublisherService> logger)
  {
    _serverStatisticsConfig = serverStatisticsConfig.Value;

    _queuePublisher = queuePublisher;

    _statisticsCollector = statisticsCollector;

    _logger = logger;
  }

  public void Run()
  {
    var timer = new Timer(_serverStatisticsConfig.SamplingIntervalSeconds * 1000);

    timer.Elapsed += OnTimerElapsed;

    timer.Start();

    _logger.LogInformation($"Service Started At {DateTime.Now}");

    Console.ReadLine();
  }

  private void OnTimerElapsed(object? sender, ElapsedEventArgs e)
  {
    try
    {
      _logger.LogInformation($"Started Collecting statistics at {DateTime.Now}");
      
      var statisticsToPublish = _statisticsCollector.Collect();

      statisticsToPublish.ServerIdentifier = _serverStatisticsConfig.ServerIdentifier;

      _logger.LogInformation($"""
                              Collected the following statistics successfully at {DateTime.Now}
                              {StandardMessages.FormatStatisticsForLogging(statisticsToPublish)}
                              """);

      Publish(statisticsToPublish);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, $"An error occurred while collecting statistics at {DateTime.Now}");
    }
  }

  private void Publish(ServerStatistics statistics)
  {
    try
    {
      _logger.LogInformation($"publishing the collected statistics...");
      
      _queuePublisher.Publish(statistics);

      _logger.LogInformation($"Collected statistics published successfully at {DateTime.Now}");
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, $"An error occurred while collecting statistics at {DateTime.Now}");
    }
  }
}