using Microsoft.Extensions.Configuration;

namespace ServerStatisticsCollectionService.Configurations;

public class RabbitMqConfig
{
  public const string SectionName = "RABBITMQ_CONFIG";

  [ConfigurationKeyName("CONNECTION_STRING")]
  public string ConnectionString { get; set; } = string.Empty;
  
  [ConfigurationKeyName("EXCHANGE_NAME")]
  public string ExchangeName { get; set; } = string.Empty;

  [ConfigurationKeyName("QUEUE_TO_BIND_WITH")]
  public string QueueToBindWith { get; set; } = string.Empty;
}