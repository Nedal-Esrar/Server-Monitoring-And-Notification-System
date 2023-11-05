using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SignalREventConsumerService.Interfaces;
using SignalREventConsumerService.Services;

namespace SignalREventConsumerService.DependencyInjection;

public static class ServicesConfiguration
{
  public static IServiceCollection ConfigureServices(this IServiceCollection serviceCollection,
    IConfiguration configuration)
  {
    return serviceCollection
      .AddConfig(configuration)
      .AddDefaultLogger()
      .AddSingleton<ISignalRConsumerService, SignalRConsumerService>();
  }
}