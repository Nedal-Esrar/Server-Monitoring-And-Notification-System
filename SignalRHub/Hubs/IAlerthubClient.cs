namespace SignalRHub.Hubs;

public interface IAlertHubClient
{
  Task ReceiveAlertMessage(string serverIdentifier, string message, DateTime date);
}