using System.Threading.Tasks;

namespace Shared.EventBus;

public class NullEventBus : IEventBus
{
    public Task PublishAsync<TEvent>(TEvent @event) where TEvent : IEvent
    {
        return Task.CompletedTask;
    }

    public void Subscribe<TEvent>(Func<TEvent, Task> handler) where TEvent : IEvent
    {
        // No hace nada
    }

    public void Unsubscribe<TEvent>(Func<TEvent, Task> handler) where TEvent : IEvent
    {
        // No hace nada
    }
}