using Microsoft.AspNetCore.SignalR;

namespace SignalRHub.Hubs;

public class AlertHub : Hub<IAlertHubClient>
{
  public async Task SendAlertMessage(string serverIdentifier, string message, DateTime date)
  {
    await Clients.All.ReceiveAlertMessage(serverIdentifier, message, date);
  }
}