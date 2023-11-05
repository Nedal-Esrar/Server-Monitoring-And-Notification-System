using MessageProcessingAndAnomalyDetectionService.Interfaces;
using Shared.Models;

namespace MessageProcessingAndAnomalyDetectionService.Services;

public class AlertContext : IAlertContext
{
  private readonly IEnumerable<IAlertHandler> _alertHandlers;

  public AlertContext(IEnumerable<IAlertHandler> alertHandlers)
  {
    _alertHandlers = alertHandlers;
  }

  public async Task Publish(ServerStatistics statistics)
  {
    await Task.WhenAll(_alertHandlers.Select(handler => handler.Handle(statistics)));
  }
}