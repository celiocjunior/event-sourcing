namespace Todo.Infra.EventStore
{
    public sealed class DataWithVersion
    {
        public readonly long Version;
        public readonly byte[] Data;

        public DataWithVersion(long version, byte[] data)
        {
            Version = version;
            Data = data;
        }
    }
}
