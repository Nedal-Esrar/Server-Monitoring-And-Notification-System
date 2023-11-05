namespace MessageBroker.Interfaces;

public interface IQueuePublisher
{
  Task Publish<TMessage>(TMessage message);
}