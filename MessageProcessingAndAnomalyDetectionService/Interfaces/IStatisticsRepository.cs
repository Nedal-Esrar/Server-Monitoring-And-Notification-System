using Shared.Models;

namespace MessageProcessingAndAnomalyDetectionService.Interfaces;

public interface IStatisticsRepository
{
  Task SaveAsync(ServerStatistics statistics);

  Task<ServerStatistics?> GetLastForAsync(string serverIdentifier);
}