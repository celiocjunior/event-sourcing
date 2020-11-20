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
                stream.Events.AddRange(GetEventsFromRecord(tapeRecord));
                stream.Version = tapeRecord.Version;
            }

            return stream;
        }

        private IEnumerable<IEvent> GetEventsFromRecord(DataWithVersion record)
        {
            using var jsonDocument = DeserializeEvent(record.Data);

            foreach (var item in jsonDocument.RootElement.EnumerateArray())
            {
                var json = item.ToString();
                if (json is null) continue;

                var type = GetEventType(item);
                var typedEvent = JsonSerializer.Deserialize(json, type);
                if (typedEvent is null) continue;

                yield return (IEvent)typedEvent;
            }
        }

        private Type GetEventType(JsonElement element)
        {
            var eventType = element
                .GetProperty(nameof(IEvent.EventType))
                .GetString();

            if (eventType is null)
                throw new InvalidOperationException($"No event type defined in json element: {element}");

            return _availableEvents[eventType];
        }

        private static byte[] SerializeEvent(IEvent[] @event) =>
            JsonSerializer.SerializeToUtf8Bytes(@event.Select(_ => Convert.ChangeType(_, _.GetType())));

        private static JsonDocument DeserializeEvent(byte[] data) =>
            JsonDocument.Parse(new ReadOnlyMemory<byte>(data));
    }
}
