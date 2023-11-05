namespace MessageProcessingAndAnomalyDetectionService.Configurations;

public class SignalRConfig
{
  public const string SectionName = "SignalRConfig";
  
  public string SignalRUrl { get; set; } = string.Empty;

  public string SignalRSendMethod { get; set; } = string.Empty;
}