using System.Collections.Generic;

namespace Todo.Domain.EventStore
{
    public class EventStream
    {
        public long Version;
        public List<IEvent> Events = new List<IEvent>();
    }
}
