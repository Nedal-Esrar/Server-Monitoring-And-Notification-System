using MessageProcessingAndAnomalyDetectionService.Configurations;
using MessageProcessingAndAnomalyDetectionService.Interfaces;
using MessageProcessingAndAnomalyDetectionService.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using Shared.Models;

namespace MessageProcessingAndAnomalyDetectionService.DependencyInjection;

public static class MongoDbExtensions
{
  public static IServiceCollection AddMongoDb(this IServiceCollection serviceCollection, IConfiguration configuration)
  {
    ConventionRegistry.Register("IgnoreExtraElements", new ConventionPack
    {
      new IgnoreExtraElementsConvention(true)
    }, _ => true);

    var mongoDbConfig = configuration.GetSection(MongoDbConfig.SectionName).Get<MongoDbConfig>();

    return serviceCollection
      .AddSingleton<IStatisticsRepository>(_ =>
      {
        var mongoClient = new MongoClient(mongoDbConfig.ConnectionString);

        var database = mongoClient.GetDatabase(mongoDbConfig.DatabaseName);

        var collection = database.GetCollection<ServerStatistics>(mongoDbConfig.CollectionName);

        return new MongoDbStatisticsRepository(collection);
      });
  }
}