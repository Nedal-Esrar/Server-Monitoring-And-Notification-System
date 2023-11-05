using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ServerStatisticsCollectionService.DependencyInjection;
using ServerStatisticsCollectionService.Interfaces;

var configuration = new ConfigurationBuilder()
  .AddEnvironmentVariables()
  .Build();

var serviceProvider = new ServiceCollection()
  .ConfigureDependencies(configuration)
  .BuildServiceProvider();

var app = serviceProvider.GetRequiredService<IPeriodicStatisticsPublisherService>();

app.Run();