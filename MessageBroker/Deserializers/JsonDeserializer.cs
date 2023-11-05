using System.Text.Json;
using MessageBroker.Interfaces;

namespace MessageBroker.Deserializers;

public class JsonDeserializer : IDeserialize
{
  public TMessage Deserialize<TMessage>(byte[] message)
  {
    return JsonSerializer.Deserialize<TMessage>(message) ??
           throw new InvalidDataException("Invalid Json");
  }
}