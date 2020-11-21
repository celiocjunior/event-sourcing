using System.Collections.Generic;
using System.Linq;
using Todo.Infra.EventStore;

namespace Todo.Infra.InMemoryEventStore
{
    public record MemoryRecord(
        string Name,
        int Version,
        string EventType,
        byte[] Data
    );

    public class InMemoryEventStore : IAppendOnlyStore
    {
        private readonly ICollection<MemoryRecord> records = new List<MemoryRecord>();

        public void Append(string eventType, string streamName, byte[] data, long expectedStreamVersion)
        {
            var version = records
                .Where(_ => _.Name == streamName)
                .Select(_ => _.Version)
                .OrderBy(_ => _)
                .LastOrDefault();

            if (version != expectedStreamVersion)
                throw new AppendOnlyStoreConcurrencyException(version, expectedStreamVersion, streamName);

            records.Add(new MemoryRecord(
                streamName,
                version + 1,
                eventType,
                data
            ));
        }

        public IEnumerable<DataWithVersion> ReadRecords(string streamName, long afterVersion, int maxCount) =>
            records
                .Where(_ => _.Name == streamName && _.Version > afterVersion)
                .OrderBy(_ => _.Version)
                .Take(maxCount)
                .Select(_ => new DataWithVersion(_.Version, _.Data, _.EventType));

        public IEnumerable<DataWithName> ReadRecords(long afterVersion, int maxCount) =>
            records
                .Where(_ => _.Version > afterVersion)
                .OrderBy(_ => _.Version)
                .Take(maxCount)
                .Select(_ => new DataWithName(_.Name, _.Data, _.EventType));

        public void Close()
        {
        }

        public void Dispose()
        {
        }
    }
}
