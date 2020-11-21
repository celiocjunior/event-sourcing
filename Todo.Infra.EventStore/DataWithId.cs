namespace Todo.Infra.EventStore
{
    public sealed record DataWithId(
        string Id,
        byte[] Data,
        string EventType
    );
}
