using Microsoft.Extensions.Configuration;

namespace MessageProcessingAndAnomalyDetectionService.Configurations;

public class SignalRConfig
{
  public const string SectionName = "SIGNALR_CONFIG";
  
  [ConfigurationKeyName("SIGNALR_URL")]
  public string SignalRUrl { get; set; } = string.Empty;

  [ConfigurationKeyName("SIGNALR_SEND_METHOD")]
  public string SignalRSendMethod { get; set; } = string.Empty;
}