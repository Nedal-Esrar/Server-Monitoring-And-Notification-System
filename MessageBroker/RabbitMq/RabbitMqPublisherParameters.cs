namespace MessageBroker.RabbitMq;

public class RabbitMqPublisherParameters
{
  public string Exchange { get; set; } = string.Empty;

  public string RoutingKey { get; set; } = string.Empty;
  
  public int MaxConnectionRetries { get; set; }
  
  public int DelayBetweenRetries { get; set; } // in ms
}