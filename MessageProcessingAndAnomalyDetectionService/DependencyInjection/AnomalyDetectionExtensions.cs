using MessageProcessingAndAnomalyDetectionService.AlertHandlers;
using MessageProcessingAndAnomalyDetectionService.Interfaces;
using MessageProcessingAndAnomalyDetectionService.Services;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;

namespace MessageProcessingAndAnomalyDetectionService.DependencyInjection;

public static class AnomalyDetectionExtensions
{
  public static IServiceCollection AddAnomalyDetection(this IServiceCollection serviceCollection)
  {
    return serviceCollection
      .AddSingleton<IHubConnectionBuilder, HubConnectionBuilder>()
      .AddSingleton<ISignalRClientSender, SignalRClientSender>()
      .AddSingleton<IAlertHandler, CpuUsageAnomalyAlertHandler>()
      .AddSingleton<IAlertHandler, HighCpuUsageAlertHandler>()
      .AddSingleton<IAlertHandler, HighMemoryUsageAlertHandler>()
      .AddSingleton<IAlertHandler, MemoryUsageAnomalyAlertHandler>()
      .AddSingleton<IAlertContext, AlertContext>()
      .AddSingleton<IAnomalyDetectionService, AnomalyDetectionService>();
  }
}