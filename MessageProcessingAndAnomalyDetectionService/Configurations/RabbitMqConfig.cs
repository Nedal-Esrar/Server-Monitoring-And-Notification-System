namespace MessageProcessingAndAnomalyDetectionService.Configurations;

public class RabbitMqConfig
{
  public const string SectionName = "RabbitMqConfig";

  public string ConnectionString { get; set; } = string.Empty;
  
  public string QueueToReceiveFrom { get; set; } = string.Empty;
}