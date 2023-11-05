namespace MessageProcessingAndAnomalyDetectionService.Configurations;

public class MongoDbConfig
{
  public const string SectionName = "MongoDbConfig";

  public string ConnectionString { get; set; } = string.Empty;
  
  public string DatabaseName { get; set; } = string.Empty;

  public string CollectionName { get; set; } = string.Empty;
}