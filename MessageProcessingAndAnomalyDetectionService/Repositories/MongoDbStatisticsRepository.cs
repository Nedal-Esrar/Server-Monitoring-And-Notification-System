using MessageProcessingAndAnomalyDetectionService.Interfaces;
using MongoDB.Driver;
using Shared.Models;

namespace MessageProcessingAndAnomalyDetectionService.Repositories;

public class MongoDbStatisticsRepository : IStatisticsRepository
{
  private readonly IMongoCollection<ServerStatistics> _collection;

  public MongoDbStatisticsRepository(IMongoCollection<ServerStatistics> collection)
  {
    _collection = collection;
  }

  public async Task SaveAsync(ServerStatistics statistics)
  {
    if (statistics is null) throw new ArgumentNullException(nameof(statistics));

    await _collection.InsertOneAsync(statistics);
  }

  public async Task<ServerStatistics?> GetLastForAsync(string serverIdentifier)
  {
    if (serverIdentifier is null) throw new ArgumentNullException(nameof(serverIdentifier));

    return await _collection
      .Find(s => s.ServerIdentifier == serverIdentifier)
      .SortByDescending(s => s.Timestamp)
      .FirstOrDefaultAsync();
  }
}