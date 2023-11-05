using Microsoft.Extensions.Configuration;

namespace MessageProcessingAndAnomalyDetectionService.Configurations;

public class RabbitMqConfig
{
  public const string SectionName = "RABBITMQ_CONFIG";

  [ConfigurationKeyName("CONNECTION_STRING")]
  public string ConnectionString { get; set; } = string.Empty;
  
  [ConfigurationKeyName("QUEUE_TO_RECEIVE_FROM")]
  public string QueueToReceiveFrom { get; set; } = string.Empty;
}