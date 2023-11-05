using MessageBroker.Interfaces;
using MessageBroker.Utilities;
using RabbitMQ.Client;

namespace MessageBroker.RabbitMq;

public class RabbitMqQueuePublisher : IQueuePublisher
{
  private readonly IConnectionFactory _connectionFactory;

  private readonly RabbitMqPublisherParameters _publisherParameters;
  private readonly ISerializer _serializer;

  public RabbitMqQueuePublisher(IConnectionFactory connectionFactory, ISerializer serializer,
    RabbitMqPublisherParameters publisherParameters)
  {
    _connectionFactory = connectionFactory;
    _serializer = serializer;
    _publisherParameters = publisherParameters;
  }

  public Task Publish<TMessage>(TMessage message)
  {
    using var connection = RetryActions.PerformWithRetry(
      _publisherParameters.MaxConnectionRetries, 
      _publisherParameters.DelayBetweenRetries, 
      _connectionFactory.CreateConnection
    );

    using var channel = connection.CreateModel();

    var serializedMessage = _serializer.Serialize(message);

    channel.BasicPublish(
      _publisherParameters.Exchange,
      _publisherParameters.RoutingKey,
      null,
      serializedMessage
    );

    return Task.CompletedTask;
  }
}