namespace MessageProcessingAndAnomalyDetectionService.Configurations;

public class AnomalyDetectionConfig
{
  public const string SectionName = "AnomalyDetectionConfig";
  
  public double MemoryUsageAnomalyThresholdPercentage { get; set; }

  public double CpuUsageAnomalyThresholdPercentage { get; set; }

  public double MemoryUsageThresholdPercentage { get; set; }

  public double CpuUsageThresholdPercentage { get; set; }
}