using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Todo.Domain;
using Todo.Domain.EventStore;

namespace Todo.Infra.EventStore
{
    public class EventStore : IEventStore
    {
        private readonly IAppendOnlyStore _appendOnlyStore;

        public EventStore(IAppendOnlyStore appendOnlyStore)
        {
            _appendOnlyStore = appendOnlyStore;
        }

        public void AppendToStream(IIdentity id, long originalVersion, IEnumerable<IEvent> events)
        {
            var eventsArray = events.ToArray();

            if (eventsArray.Length == 0)
                return;

            var name = id.AsString();
            var data = SerializeEvent(eventsArray);

            try
            {
                _appendOnlyStore.Append(name, data, originalVersion);
            }
            catch (AppendOnlyStoreConcurrencyException e)
            {
                // load server events
                var server = LoadEventStream(id, 0, int.MaxValue);
                // throw a real problem
                throw OptimisticConcurrencyException.Create(server.Version, e.ExpectedStreamVersion, id, server.Events);
            }

            // TODO: technically there should be parallel process that queries new changes from 
            // event store and sends them via messages. 
        }

        public EventStream LoadEventStream(IIdentity id)
        {
            return LoadEventStream(id, 0, int.MaxValue);
        }

        public EventStream LoadEventStream(IIdentity id, long skip, int take)
        {
            string name = id.AsString();
            var records = _appendOnlyStore.ReadRecords(name, skip, take);
            var stream = new EventStream();

            foreach (var tapeRecord in records)
            {
                var json = DeserializeEvent(tapeRecord.Data);
                var typedEvents = new List<IEvent>();
                foreach (var item in json.RootElement.EnumerateArray())
                {
                    var eventType = item.GetProperty(nameof(IEvent.EventType)).GetString() ?? throw new InvalidOperationException();
                    var type = typeof(IEvent).Assembly.GetTypes().Where(_ => _.Name == eventType).Single();
                    var typedEvent = JsonSerializer.Deserialize(item.ToString() ?? throw new InvalidOperationException(), type);
                    typedEvents.Add((IEvent)(typedEvent ?? throw new InvalidOperationException()));
                }

                stream.Events.AddRange(typedEvents);
                stream.Version = tapeRecord.Version;
            }
            return stream;
        }

        private static byte[] SerializeEvent(IEvent[] e)
        {
            return JsonSerializer.SerializeToUtf8Bytes(
                e.Select(_ => Convert.ChangeType(_, _.GetType()))
            );
        }

        private static JsonDocument DeserializeEvent(byte[] data) =>
            JsonDocument.Parse(new ReadOnlyMemory<byte>(data));
    }
}
