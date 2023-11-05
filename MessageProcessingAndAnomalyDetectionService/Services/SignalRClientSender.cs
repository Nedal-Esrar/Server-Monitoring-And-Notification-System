using MessageProcessingAndAnomalyDetectionService.Configurations;
using MessageProcessingAndAnomalyDetectionService.Interfaces;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Options;

namespace MessageProcessingAndAnomalyDetectionService.Services;

public class SignalRClientSender : ISignalRClientSender
{
  private readonly SignalRConfig _config;

  private readonly HubConnection _hubConnection;

  public SignalRClientSender(IHubConnectionBuilder hubConnectionBuilder, IOptions<SignalRConfig> config)
  {
    _config = config.Value;
    
    _hubConnection = BuildConnection(hubConnectionBuilder);
    
    _hubConnection.StartAsync().Wait();
  }

  public async Task SendAlertAsync(string serverIdentifier, string alertMessage, DateTime date)
  {
    await _hubConnection.InvokeAsync(_config.SignalRSendMethod, serverIdentifier, alertMessage, date);
  }

  private HubConnection BuildConnection(IHubConnectionBuilder hubConnectionBuilder)
  {
    return hubConnectionBuilder
      .WithUrl(_config.SignalRUrl)
      .WithAutomaticReconnect()
      .Build();
  }

  ~SignalRClientSender()
  {
    _hubConnection.StopAsync();
    _hubConnection.DisposeAsync();
  }
}