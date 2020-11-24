using System;
using Todo.Domain.EventBus;

namespace Todo.Domain.EventBus
{
    public interface IEventBus : IEventPublisher, IEventSubscriber, IDisposable { }
}
