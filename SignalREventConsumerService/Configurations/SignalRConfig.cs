namespace SignalREventConsumerService.Configurations;

public class SignalRConfig
{
  public const string SectionName = "SignalRConfig";

  public string SignalRUrl { get; set; } = string.Empty;
  
  public string SignalRReceiveMethod { get; set; } = string.Empty;
}