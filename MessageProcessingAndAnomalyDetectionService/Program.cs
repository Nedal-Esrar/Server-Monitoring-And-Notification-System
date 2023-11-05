using MessageProcessingAndAnomalyDetectionService.DependencyInjection;
using MessageProcessingAndAnomalyDetectionService.Interfaces;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

var configuration = new ConfigurationBuilder()
  .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
  .Build();

var serviceProvider = new ServiceCollection()
  .ConfigureDependencies(configuration)
  .BuildServiceProvider();

var app = serviceProvider.GetRequiredService<IAnomalyMonitorService>();

await app.RunAsync();