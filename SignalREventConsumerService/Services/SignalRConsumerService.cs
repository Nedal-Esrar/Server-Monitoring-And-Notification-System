using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SignalREventConsumerService.Configurations;
using SignalREventConsumerService.Interfaces;

namespace SignalREventConsumerService.Services;

public class SignalRConsumerService : ISignalRConsumerService
{
  private readonly SignalRConfig _config;

  private readonly ILogger<ISignalRConsumerService> _logger;

  public SignalRConsumerService(IOptions<SignalRConfig> config, ILogger<ISignalRConsumerService> logger)
  {
    _logger = logger;

    _config = config.Value;
  }

  public async Task RunAsync()
  {
    _logger.LogInformation($"Service Started Successfully at {DateTime.Now}");
    
    try
    {
      _logger.LogInformation("Establishing SignalR connection...");
      
      await using var hubConnection = new HubConnectionBuilder()
        .WithUrl(_config.SignalRUrl)
        .WithAutomaticReconnect()
        .Build();

      hubConnection.On<string, string, DateTime>(_config.SignalRReceiveMethod,
        (serverIdentifier, message, date) => { Console.WriteLine($"From Server {serverIdentifier}: {message} at {date}"); });

      await hubConnection.StartAsync();
      
      _logger.LogInformation($"Connection established successfully at {DateTime.Now}");

      Console.ReadLine();
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, $"An error occurred while establishing at {DateTime.Now}");
    }
  }
}