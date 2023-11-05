using ServerStatisticsCollectionService.Interfaces;

namespace ServerStatisticsCollectionService.CollectorStrategies;

public class UnixStatisticsCollectorStrategy : IStatisticsCollectorStrategy
{
  public double GetMemoryUsage()
  {
    var memoryInfo = File.ReadLines("/proc/meminfo");

    var totalMemoryInfo = memoryInfo.First();

    var availableMemoryInfo = memoryInfo.Skip(2).First();

    var totalMemoryInKb = double.Parse(totalMemoryInfo.Split(' ', StringSplitOptions.RemoveEmptyEntries)[1]);

    var availableMemoryInKb =
      double.Parse(availableMemoryInfo.Split(' ', StringSplitOptions.RemoveEmptyEntries)[1]);

    return (totalMemoryInKb - availableMemoryInKb) / 1024;
  }

  public double GetAvailableMemory()
  {
    var availableMemoryInfo = File.ReadLines("/proc/meminfo")
      .Skip(2)
      .First();

    var availableMemoryInKb =
      double.Parse(availableMemoryInfo.Split(' ', StringSplitOptions.RemoveEmptyEntries)[1]);

    return availableMemoryInKb / 1024;
  }

  public double GetCpuUsage()
  {
    var cpuInfo = File.ReadLines("/proc/stat")
      .First()
      .Split(' ', StringSplitOptions.RemoveEmptyEntries)
      .Skip(1)
      .ToArray();

    var idleTime = double.Parse(cpuInfo[3]);

    var totalTime = cpuInfo.Aggregate(0.0,
      (total, num) => total + double.Parse(num));

    return 1 - idleTime / totalTime;
  }
}