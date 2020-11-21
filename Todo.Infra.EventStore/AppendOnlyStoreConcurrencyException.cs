using System;

namespace Todo.Infra.EventStore
{
    public class AppendOnlyStoreConcurrencyException : Exception
    {
        public long ExpectedStreamVersion { get; }
        public long ActualStreamVersion { get; }
        public string StreamId { get; }

        public AppendOnlyStoreConcurrencyException(long expectedVersion, long actualVersion, string streamId)
            : base($"Expected version v{expectedVersion} in stream '{streamId}' but got v{actualVersion}")
        {
            StreamId = streamId;
            ExpectedStreamVersion = expectedVersion;
            ActualStreamVersion = actualVersion;
        }
    }
}
