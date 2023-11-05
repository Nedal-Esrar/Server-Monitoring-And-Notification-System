using ServerStatisticsCollectionService.Interfaces;
using Shared.Models;

namespace ServerStatisticsCollectionService.Services;

public class StatisticsCollector : IStatisticsCollector
{
  private readonly IStatisticsCollectorStrategy _statisticsCollectorStrategy;

  public StatisticsCollector(IStatisticsCollectorStrategy statisticsCollectorStrategy)
  {
    _statisticsCollectorStrategy = statisticsCollectorStrategy;
  }

  public ServerStatistics Collect()
  {
    return new ServerStatistics
    {
      MemoryUsage = _statisticsCollectorStrategy.GetMemoryUsage(),
      AvailableMemory = _statisticsCollectorStrategy.GetAvailableMemory(),
      CpuUsage = _statisticsCollectorStrategy.GetCpuUsage(),
      Timestamp = DateTime.Now
    };
  }
}