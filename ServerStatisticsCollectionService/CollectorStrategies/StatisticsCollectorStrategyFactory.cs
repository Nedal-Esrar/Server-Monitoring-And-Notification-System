using ServerStatisticsCollectionService.Interfaces;

namespace ServerStatisticsCollectionService.CollectorStrategies;

public static class StatisticsCollectorStrategyFactory
{
  public static IStatisticsCollectorStrategy CreateCollector(PlatformID os)
  {
    return os switch
    {
      PlatformID.Win32NT => new WindowsStatisticsCollectorStrategy(),
      PlatformID.Unix => new UnixStatisticsCollectorStrategy(),
      _ => throw new NotSupportedException(nameof(os))
    };
  }
}