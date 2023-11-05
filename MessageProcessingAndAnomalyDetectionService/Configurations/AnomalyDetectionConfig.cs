using Microsoft.Extensions.Configuration;

namespace MessageProcessingAndAnomalyDetectionService.Configurations;

public class AnomalyDetectionConfig
{
  public const string SectionName = "ANOMALY_DETECTION_CONFIG";
  
  [ConfigurationKeyName("MEMORY_USAGE_ANOMALY_THRESHOLD_PERCENTAGE")]
  public double MemoryUsageAnomalyThresholdPercentage { get; set; }

  [ConfigurationKeyName("CPU_USAGE_ANOMALY_THRESHOLD_PERCENTAGE")]
  public double CpuUsageAnomalyThresholdPercentage { get; set; }

  [ConfigurationKeyName("MEMORY_USAGE_THRESHOLD_PERCENTAGE")]
  public double MemoryUsageThresholdPercentage { get; set; }

  [ConfigurationKeyName("CPU_USAGE_THRESHOLD_PERCENTAGE")]
  public double CpuUsageThresholdPercentage { get; set; }
}