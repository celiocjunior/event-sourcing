namespace Todo.Infra.EventStore
{
    public sealed record DataWithVersion(
        long Version,
        byte[] Data,
        string EventType
    );
}
