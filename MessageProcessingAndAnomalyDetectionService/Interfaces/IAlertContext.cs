using Shared.Models;

namespace MessageProcessingAndAnomalyDetectionService.Interfaces;

public interface IAlertContext
{
  Task Publish(ServerStatistics statistics);
}