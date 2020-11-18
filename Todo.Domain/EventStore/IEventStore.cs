using System.Collections.Generic;

namespace Todo.Domain.EventStore
{
    public interface IEventStore
    {
        EventStream LoadEventStream(IIdentity id);
        EventStream LoadEventStream(IIdentity id, long skipEvents, int maxCount);
        void AppendToStream(IIdentity id, long expectedVersion, IEnumerable<IEvent> events);
    }
}
