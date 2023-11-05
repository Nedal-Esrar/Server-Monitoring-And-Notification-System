using Microsoft.Extensions.DependencyInjection;
using ServerStatisticsCollectionService.CollectorStrategies;
using ServerStatisticsCollectionService.Interfaces;
using ServerStatisticsCollectionService.Services;

namespace ServerStatisticsCollectionService.DependencyInjection;

public static class StatisticsCollectorExtensions
{
  public static IServiceCollection AddStatisticsCollector(this IServiceCollection serviceCollection)
  {
    return serviceCollection
      .AddSingleton<IStatisticsCollectorStrategy>(_ =>
        StatisticsCollectorStrategyFactory.CreateCollector(Environment.OSVersion.Platform))
      .AddSingleton<IStatisticsCollector, StatisticsCollector>();
  }
}