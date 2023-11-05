using Shared.Models;

namespace ServerStatisticsCollectionService.Interfaces;

public interface IStatisticsCollector
{
  ServerStatistics Collect();
}