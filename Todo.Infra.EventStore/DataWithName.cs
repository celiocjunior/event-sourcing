namespace Todo.Infra.EventStore
{
    public sealed record DataWithName(
        string Name,
        byte[] Data,
        string EventType
    );
}
