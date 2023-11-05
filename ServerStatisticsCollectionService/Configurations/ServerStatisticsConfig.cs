namespace ServerStatisticsCollectionService.Configurations;

public class ServerStatisticsConfig
{
  public const string SectionName = "ServerStatisticsConfig";
  
  public int SamplingIntervalSeconds { get; set; }

  public string ServerIdentifier { get; set; } = string.Empty;
}