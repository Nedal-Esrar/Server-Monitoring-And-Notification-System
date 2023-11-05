using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ServerStatisticsCollectionService.DependencyInjection;

public static class LoggingExtensions
{
  public static IServiceCollection AddDefaultLogger(this IServiceCollection serviceCollection)
  {
    return serviceCollection.AddLogging(builder => builder.ClearProviders().AddConsole());
  }
}