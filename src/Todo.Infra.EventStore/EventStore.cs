using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Todo.Domain;
using Todo.Domain.EventBus;
using Todo.Domain.EventStore;

namespace Todo.Infra.EventStore
{
    public class EventStore : IEventStore
    {
        private readonly IAppendOnlyStore _appendOnlyStore;
        private readonly IEventPublisher _eventPublisher;
        private readonly Dictionary<string, Type> _availableEvents;

        public EventStore(IAppendOnlyStore appendOnlyStore, IEventPublisher eventPublisher)
        {
            _appendOnlyStore = appendOnlyStore;
            _eventPublisher = eventPublisher;

            var eventType = typeof(IEvent);
            _availableEvents = eventType.Assembly
                .GetTypes()
                .Where(_ => _.IsAssignableTo(eventType))
                .ToDictionary(k => k.Name, v => v);
        }

        public void AppendToStream(IIdentity id, long originalVersion, IEvent @event)
        {
            var streamId = id.AsString();
            var data = SerializeEvent(@event);
            var eventType = @event.GetType().Name;

            try
            {
                _appendOnlyStore.Append(eventType, streamId, data, originalVersion);
            }
            catch (AppendOnlyStoreConcurrencyException ex)
            {
                // load server events
                var stream = LoadEventStream(id, 0, int.MaxValue);
                // throw a real problem
                throw OptimisticConcurrencyException.Create(
                    stream.Version,
                    ex.ExpectedStreamVersion,
                    id,
                    stream.Events);
            }

            _eventPublisher.Publish(@event);
        }

        public EventStream LoadEventStream(IIdentity id)
        {
            return LoadEventStream(id, 0, int.MaxValue);
        }

        public EventStream LoadEventStream(IIdentity id, long skip, int take)
        {
            string streamId = id.AsString();
            var records = _appendOnlyStore.ReadRecords(streamId, skip, take);
            var stream = new EventStream();

            foreach (var record in records)
            {
                stream.AppendEvent(GetEventFromRecord(record));
                stream.Version = record.Version;
            }

            return stream;
        }

        private IEvent GetEventFromRecord(DataWithVersion record) =>
            DeserializeEvent(record.Data, _availableEvents[record.EventType]);

        private static byte[] SerializeEvent(IEvent @event) =>
            JsonSerializer.SerializeToUtf8Bytes(Convert.ChangeType(@event, @event.GetType()));

        private static IEvent DeserializeEvent(byte[] data, Type type) =>
            (IEvent)(JsonSerializer.Deserialize(new ReadOnlySpan<byte>(data), type)
            ?? throw new Exception($"Error: {nameof(DeserializeEvent)}"));
    }
}
