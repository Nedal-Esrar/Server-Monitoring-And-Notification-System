using MessageBroker.Deserializers;
using MessageBroker.Interfaces;
using MessageBroker.RabbitMq;
using MessageBroker.Utilities;
using MessageProcessingAndAnomalyDetectionService.Configurations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;

namespace MessageProcessingAndAnomalyDetectionService.DependencyInjection;

public static class MessageBrokerExtensions
{
  public static IServiceCollection AddMessageBrokerSubscriber(this IServiceCollection serviceCollection,
    IConfiguration configuration)
  {
    var rabbitMqConfig = configuration.GetSection(RabbitMqConfig.SectionName).Get<RabbitMqConfig>();

    var connectionFactory = new ConnectionFactory
    {
      Uri = new Uri(rabbitMqConfig.ConnectionString)
    };

    var connection = RetryActions.PerformWithRetry(
      5,
      2000,
      connectionFactory.CreateConnection
    );

    var channel = connection.CreateModel();

    ConfigureRabbitMq(channel, rabbitMqConfig);

    return serviceCollection
      .AddSingleton<RabbitMqSubscriberParameters>(_ => new RabbitMqSubscriberParameters
      {
        Queue = rabbitMqConfig.QueueToReceiveFrom
      })
      .AddSingleton<IDeserialize, JsonDeserializer>()
      .AddSingleton(channel)
      .AddSingleton<IQueueSubscriber, RabbitMqQueueSubscriber>();
  }

  private static void ConfigureRabbitMq(IModel channel, RabbitMqConfig rabbitMqConfig)
  {
    channel.QueueDeclare(
      rabbitMqConfig.QueueToReceiveFrom,
      durable: false,
      exclusive: false,
      autoDelete: false
    );
  }
}