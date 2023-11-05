namespace MessageBroker.Interfaces;

public interface IDeserialize
{
  TMessage Deserialize<TMessage>(byte[] message);
}