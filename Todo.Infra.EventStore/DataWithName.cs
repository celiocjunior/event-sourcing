namespace Todo.Infra.EventStore
{
    public sealed class DataWithName
    {
        public readonly string Name;
        public readonly byte[] Data;

        public DataWithName(string name, byte[] data)
        {
            Name = name;
            Data = data;
        }
    }
}
