using System;
using System.Collections.Generic;
using System.Linq;
using Todo.Infra.EventStore;

namespace Todo.Infra.InMemoryEventStore
{
    public class Record
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Version { get; set; }
        public string EventType { get; set; } = string.Empty;
        public byte[] Data { get; set; } = Array.Empty<byte>();
    }

    public class InMemoryEventStore : IAppendOnlyStore
    {
        private int currentId = 1;
        private readonly ICollection<Record> records = new List<Record>();

        public void Append(string eventType, string streamName, byte[] data, long expectedStreamVersion = -1)
        {
            var version = records
                .Where(_ => _.Name == streamName)
                .Select(_ => _.Version)
                .OrderBy(_ => _)
                .LastOrDefault();

            if (version != expectedStreamVersion)
                throw new AppendOnlyStoreConcurrencyException(version, expectedStreamVersion, streamName);

            records.Add(new Record
            {
                Id = currentId++,
                Name = streamName,
                Version = version + 1,
                Data = data,
                EventType = eventType
            });
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
