using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ServerStatisticsCollectionService.Configurations;

namespace ServerStatisticsCollectionService.DependencyInjection;

public static class ConfigurationsExtensions
{
  public static IServiceCollection AddConfig(this IServiceCollection serviceCollection,
    IConfiguration configuration)
  {
    return serviceCollection
      .Configure<ServerStatisticsConfig>(configuration.GetSection(ServerStatisticsConfig.SectionName));
  }
}