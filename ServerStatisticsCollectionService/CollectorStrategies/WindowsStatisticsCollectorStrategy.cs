using System.Diagnostics;
using ServerStatisticsCollectionService.Interfaces;

namespace ServerStatisticsCollectionService.CollectorStrategies;

public class WindowsStatisticsCollectorStrategy : IStatisticsCollectorStrategy
{
  public double GetMemoryUsage()
  {
    var memoryUsageCounter = new PerformanceCounter("Memory", "Committed Bytes");

    return memoryUsageCounter.NextValue() / 1048576;
  }

  public double GetAvailableMemory()
  {
    using var availableMemoryCounter = new PerformanceCounter("Memory", "Available MBytes");

    return availableMemoryCounter.NextValue();
  }

  public double GetCpuUsage()
  {
    using var cpuUsageCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");

    return cpuUsageCounter.NextValue() / 100;
  }
}