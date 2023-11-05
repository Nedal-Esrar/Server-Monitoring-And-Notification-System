namespace MessageBroker.Interfaces;

public interface ISerializer
{
  byte[] Serialize<TMessage>(TMessage message);
}