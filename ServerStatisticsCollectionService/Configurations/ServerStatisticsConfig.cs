using Microsoft.Extensions.Configuration;

namespace ServerStatisticsCollectionService.Configurations;

public class ServerStatisticsConfig
{
  public const string SectionName = "SERVER_STATISTICS_CONFIG";
  
  [ConfigurationKeyName("SAMPLING_INTERVAL_SECONDS")]
  public int SamplingIntervalSeconds { get; set; }

  [ConfigurationKeyName("SERVER_IDENTIFIER")]
  public string ServerIdentifier { get; set; } = string.Empty;
}