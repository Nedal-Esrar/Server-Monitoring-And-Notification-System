using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SignalREventConsumerService.DependencyInjection;
using SignalREventConsumerService.Interfaces;

var configuration = new ConfigurationBuilder()
  .AddEnvironmentVariables()
  .Build();

var serviceProvider = new ServiceCollection()
  .ConfigureServices(configuration)
  .BuildServiceProvider();

var app = serviceProvider.GetRequiredService<ISignalRConsumerService>();

await app.RunAsync();