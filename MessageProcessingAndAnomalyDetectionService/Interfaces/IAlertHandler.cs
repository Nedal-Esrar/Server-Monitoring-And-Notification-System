using Shared.Models;

namespace MessageProcessingAndAnomalyDetectionService.Interfaces;

public interface IAlertHandler
{
  Task<bool> CheckCondition(ServerStatistics statistics);

  Task Handle(ServerStatistics statistics);
}