namespace MessageProcessingAndAnomalyDetectionService.Interfaces;

public interface ISignalRClientSender
{
  Task SendAlertAsync(string serverIdentifier, string alertMessage, DateTime date);
}