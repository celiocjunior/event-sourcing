using System.Collections.Generic;

namespace Todo.Domain.EventStore
{
    public class EventStream
    {
        private readonly List<IEvent> _events = new List<IEvent>();
        public long Version { get; set; }
        public IEnumerable<IEvent> Events { get => _events; }

        public void AppendEvent(IEvent @event) => _events.Add(@event);
    }
}
