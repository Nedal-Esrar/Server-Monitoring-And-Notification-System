using MessageBroker.Interfaces;
using MessageBroker.RabbitMq;
using MessageBroker.Serializers;
using MessageBroker.Utilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using ServerStatisticsCollectionService.Configurations;

namespace ServerStatisticsCollectionService.DependencyInjection;

public static class MessageBrokerExtensions
{
  public static IServiceCollection AddRabbitMq(this IServiceCollection serviceCollection,
    IConfiguration configuration)
  {
    var rabbitMqConfig = configuration.GetSection(RabbitMqConfig.SectionName).Get<RabbitMqConfig>();
    
    var connectionFactory = new ConnectionFactory
    {
      Uri = new Uri(rabbitMqConfig.ConnectionString)
    };

    ConfigureRabbitMq(rabbitMqConfig, connectionFactory);

    var serverStatisticsConfig =
      configuration.GetSection(ServerStatisticsConfig.SectionName).Get<ServerStatisticsConfig>();

    return serviceCollection
      .AddSingleton<RabbitMqPublisherParameters>(_ => new RabbitMqPublisherParameters
      {
        Exchange = rabbitMqConfig.ExchangeName,
        RoutingKey = $"ServerStatistics.{serverStatisticsConfig.ServerIdentifier}",
        MaxConnectionRetries = 5,
        DelayBetweenRetries = 2000
      })
      .AddSingleton<ISerializer, JsonSerializer>()
      .AddSingleton<IConnectionFactory>(connectionFactory)
      .AddSingleton<IQueuePublisher, RabbitMqQueuePublisher>();
  }

  private static void ConfigureRabbitMq(RabbitMqConfig rabbitMqConfig, IConnectionFactory connectionFactory)
  {
    var connection = RetryActions.PerformWithRetry(
      5, 
      2000, 
      connectionFactory.CreateConnection
    );

    var channel = connection.CreateModel();

    channel.ExchangeDeclare(
      rabbitMqConfig.ExchangeName,
      ExchangeType.Topic
    );

    channel.QueueDeclare(
      rabbitMqConfig.QueueToBindWith,
      false,
      false,
      false
    );

    channel.QueueBind(
      rabbitMqConfig.QueueToBindWith,
      rabbitMqConfig.ExchangeName,
      "ServerStatistics.*"
    );
  }
}