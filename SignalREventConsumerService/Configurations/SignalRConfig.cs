using Microsoft.Extensions.Configuration;

namespace SignalREventConsumerService.Configurations;

public class SignalRConfig
{
  public const string SectionName = "SIGNALR_CONFIG";
  
  [ConfigurationKeyName("SIGNALR_URL")]
  public string SignalRUrl { get; set; } = string.Empty;

  [ConfigurationKeyName("SIGNALR_RECEIVE_METHOD")]
  public string SignalRReceiveMethod { get; set; } = string.Empty;
}