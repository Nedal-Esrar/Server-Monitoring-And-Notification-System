using Shared.Models;

namespace Shared.Utilities;

public static class StandardMessages
{
  public static string FormatStatisticsForLogging(ServerStatistics statistics)
  {
    return $"""
            For {statistics.ServerIdentifier}:
            CPU Usage: {statistics.CpuUsage}
            Memory Usage in MB: {statistics.MemoryUsage}
            Available Memory in MB: {statistics.AvailableMemory}
            """;
  }
}