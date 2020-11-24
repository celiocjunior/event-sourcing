using EasyNetQ;
using System;
using Todo.Domain;

namespace Todo.Infra.RabbitMqEventBus
{
    public class RabbitMqEventBus : Domain.EventBus.IEventBus
    {
        private readonly IBus _bus;

        public RabbitMqEventBus(string connectionString) =>
            _bus = RabbitHutch.CreateBus(connectionString);

        public void Publish(IEvent @event) =>
            _bus.PubSub.Publish(@event, @event.GetType());

        public IDisposable Subscribe<T>(Action<T> callback) where T : IEvent =>
            _bus.PubSub.Subscribe("todo", callback);

        public void Dispose()
        {
            _bus.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
