using System;
using System.Collections.Generic;

namespace Todo.Infra.EventStore
{
    public interface IAppendOnlyStore : IDisposable
    {
        void Append(string eventType, string streamId, byte[] data, long expectedStreamVersion);

        IEnumerable<DataWithVersion> ReadRecords(string streamId, long afterVersion, int maxCount);

        IEnumerable<DataWithId> ReadRecords(long afterVersion, int maxCount);

        void Close();
    }
}
