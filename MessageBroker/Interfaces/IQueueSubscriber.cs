namespace MessageBroker.Interfaces;

public interface IQueueSubscriber
{
  Task Subscribe<TMessage>(Func<TMessage, Task> onMessageReceived);
}