using Microsoft.Extensions.Configuration;

namespace MessageProcessingAndAnomalyDetectionService.Configurations;

public class MongoDbConfig
{
  public const string SectionName = "MONGODB_CONFIG";
  
  [ConfigurationKeyName("CONNECTION_STRING")]
  public string ConnectionString { get; set; }
  
  [ConfigurationKeyName("DATABASE_NAME")]
  public string DatabaseName { get; set; } = string.Empty;

  [ConfigurationKeyName("COLLECTION_NAME")]
  public string CollectionName { get; set; } = string.Empty;
}