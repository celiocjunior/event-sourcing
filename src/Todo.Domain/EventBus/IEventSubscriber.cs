using System;

namespace Todo.Domain.EventBus
{
    public interface IEventSubscriber
    {
        IDisposable Subscribe<T>(Action<T> callback) where T : IEvent;
    }
}
