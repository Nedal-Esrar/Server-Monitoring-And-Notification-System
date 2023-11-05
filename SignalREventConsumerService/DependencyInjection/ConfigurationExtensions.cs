using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SignalREventConsumerService.Configurations;

namespace SignalREventConsumerService.DependencyInjection;

public static class ConfigurationExtensions
{
  public static IServiceCollection AddConfig(this IServiceCollection serviceCollection, IConfiguration configuration)
  {
    return serviceCollection
      .Configure<SignalRConfig>(configuration.GetSection(SignalRConfig.SectionName));
  }
}