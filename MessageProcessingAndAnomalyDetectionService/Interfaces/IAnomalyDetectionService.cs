using Shared.Models;

namespace MessageProcessingAndAnomalyDetectionService.Interfaces;

public interface IAnomalyDetectionService
{
  Task CheckForAnomalies(ServerStatistics statistics);
}