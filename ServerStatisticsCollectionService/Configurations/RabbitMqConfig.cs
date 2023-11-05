namespace ServerStatisticsCollectionService.Configurations;

public class RabbitMqConfig
{
  public const string SectionName = "RabbitMqConfig";

  public string ConnectionString { get; set; } = string.Empty;
  
  public string ExchangeName { get; set; } = string.Empty;

  public string QueueToBindWith { get; set; } = string.Empty;
}