using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Todo.Domain;
using Todo.Domain.EventBus;

namespace Todo.Infra.InMemoryEventBus
{
    public class Subscription : IDisposable
    {
        private readonly List<Delegate> _subs;
        private readonly Delegate _sub;

        internal Subscription(List<Delegate> subs, Delegate sub)
        {
            _subs = subs;
            _sub = sub;
        }

        public void Dispose()
        {
            _subs.Remove(_sub);
            GC.SuppressFinalize(this);
        }
    }

    public class InMemoryEventBus : IEventBus
    {
        ConcurrentDictionary<Type, List<Delegate>> subscriptions = new ConcurrentDictionary<Type, List<Delegate>>();

        public void Publish(IEvent @event)
        {
            var subs = subscriptions.GetOrAdd(@event.GetType(), new List<Delegate>());

            foreach (var sub in subs)
                sub.DynamicInvoke(Convert.ChangeType(@event, @event.GetType()));
        }

        public IDisposable Subscribe<T>(Action<T> callback) where T : IEvent
        {
            var subs = subscriptions.GetOrAdd(typeof(T), new List<Delegate>());
            subs.Add(callback);

            return new Subscription(subs, callback);
        }

        public void Dispose()
        {
            subscriptions.Clear();
            GC.SuppressFinalize(this);
        }
    }
}
