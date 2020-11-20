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
        private readonly Dictionary<string, Type> _availableEvents;

        public EventStore(IAppendOnlyStore appendOnlyStore)
        {
            _appendOnlyStore = appendOnlyStore;

            var eventType = typeof(IEvent);
            _availableEvents = eventType.Assembly
                .GetTypes()
                .Where(_ => _.IsAssignableTo(eventType))
                .ToDictionary(k => k.Name, v => v);
        }

        public void AppendToStream(IIdentity id, long originalVersion, IEvent @event)
        {
            var name = id.AsString();
            var data = SerializeEvent(@event);
            var eventType = @event.GetType().Name;

            try
            {
                _appendOnlyStore.Append(eventType, name, data, originalVersion);
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
                stream.AppendEvent(GetEventFromRecord(tapeRecord));
                stream.Version = tapeRecord.Version;
            }

            return stream;
        }

        private IEvent GetEventFromRecord(DataWithVersion record) =>
            DeserializeEvent(record.Data, _availableEvents[record.EventType]);

        private static byte[] SerializeEvent(IEvent @event) =>
            JsonSerializer.SerializeToUtf8Bytes(Convert.ChangeType(@event, @event.GetType()));

        private static IEvent DeserializeEvent(byte[] data, Type type) =>
            (IEvent)(JsonSerializer.Deserialize(new ReadOnlySpan<byte>(data), type) ?? throw new Exception($"Error: {nameof(DeserializeEvent)}"));
    }
}
