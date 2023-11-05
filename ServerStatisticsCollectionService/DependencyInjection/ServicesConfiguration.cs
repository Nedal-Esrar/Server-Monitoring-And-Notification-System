using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ServerStatisticsCollectionService.Interfaces;
using ServerStatisticsCollectionService.Services;

namespace ServerStatisticsCollectionService.DependencyInjection;

public static class ServicesConfiguration
{
  public static IServiceCollection ConfigureDependencies(this IServiceCollection serviceCollection,
    IConfiguration configuration)
  {
    return serviceCollection
      .AddConfig(configuration)
      .AddDefaultLogger()
      .AddStatisticsCollector()
      .AddRabbitMq(configuration)
      .AddSingleton<IPeriodicStatisticsPublisherService, PeriodicStatisticsPublisherService>();
  }
}