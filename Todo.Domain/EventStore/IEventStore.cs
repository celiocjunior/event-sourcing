namespace Todo.Domain.EventStore
{
    public interface IEventStore
    {
        EventStream LoadEventStream(IIdentity id);
        EventStream LoadEventStream(IIdentity id, long skip, int take);
        void AppendToStream(IIdentity id, long originalVersion, IEvent @event);
    }
}
