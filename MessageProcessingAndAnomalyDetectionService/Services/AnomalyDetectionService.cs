using MessageProcessingAndAnomalyDetectionService.Interfaces;
using Shared.Models;

namespace MessageProcessingAndAnomalyDetectionService.Services;

public class AnomalyDetectionService : IAnomalyDetectionService
{
  private readonly IAlertContext _alertContext;

  public AnomalyDetectionService(IAlertContext alertContext)
  {
    _alertContext = alertContext;
  }

  public async Task CheckForAnomalies(ServerStatistics statistics)
  {
    await _alertContext.Publish(statistics);
  }
}