using MessageProcessingAndAnomalyDetectionService.Interfaces;
using MessageProcessingAndAnomalyDetectionService.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MessageProcessingAndAnomalyDetectionService.DependencyInjection;

public static class DependenciesConfigurations
{
  public static IServiceCollection ConfigureDependencies(this IServiceCollection serviceCollection,
    IConfiguration configuration)
  {
    serviceCollection
      .AddConfig(configuration)
      .AddDefaultLogger()
      .AddMongoDb(configuration)
      .AddMessageBrokerSubscriber(configuration)
      .AddAnomalyDetection()
      .AddSingleton<IAnomalyMonitorService, AnomalyMonitorService>();

    return serviceCollection;
  }
}