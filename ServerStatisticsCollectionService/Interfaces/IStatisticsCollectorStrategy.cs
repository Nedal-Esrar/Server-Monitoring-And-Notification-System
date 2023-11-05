namespace ServerStatisticsCollectionService.Interfaces;

public interface IStatisticsCollectorStrategy
{
  double GetMemoryUsage();

  double GetAvailableMemory();

  double GetCpuUsage();
}