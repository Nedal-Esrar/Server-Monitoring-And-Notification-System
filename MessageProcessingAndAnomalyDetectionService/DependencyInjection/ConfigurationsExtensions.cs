using MessageProcessingAndAnomalyDetectionService.Configurations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MessageProcessingAndAnomalyDetectionService.DependencyInjection;

public static class ConfigurationsExtensions
{
  public static IServiceCollection AddConfig(this IServiceCollection serviceCollection, IConfiguration configuration)
  {
    return serviceCollection
      .Configure<AnomalyDetectionConfig>(configuration.GetSection(AnomalyDetectionConfig.SectionName))
      .Configure<SignalRConfig>(configuration.GetSection(SignalRConfig.SectionName));
  }
}