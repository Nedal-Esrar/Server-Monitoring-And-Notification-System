using MessageBroker.Interfaces;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace MessageBroker.RabbitMq;

public class RabbitMqQueueSubscriber : IQueueSubscriber
{
  private readonly IModel _channel;
  private readonly IDeserialize _deserialize;

  private readonly RabbitMqSubscriberParameters _subscriberParameters;

  public RabbitMqQueueSubscriber(IModel channel, IDeserialize deserialize,
    RabbitMqSubscriberParameters subscriberParameters)
  {
    _channel = channel;
    _deserialize = deserialize;
    _subscriberParameters = subscriberParameters;
  }

  public Task Subscribe<TMessage>(Func<TMessage, Task> onMessageReceived)
  {
    var consumer = new EventingBasicConsumer(_channel);

    consumer.Received += async (model, ea) =>
    {
      var message = _deserialize.Deserialize<TMessage>(ea.Body.ToArray());

      await onMessageReceived(message!);
    };

    _channel.BasicConsume(_subscriberParameters.Queue, true, consumer);

    return Task.CompletedTask;
  }
}