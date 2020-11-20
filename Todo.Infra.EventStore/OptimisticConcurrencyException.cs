using System;
using System.Collections.Generic;
using Todo.Domain;

namespace Todo.Infra.EventStore
{
    /// <summary>
    /// Is thrown by event store if there were changes since our last version
    /// </summary>
    public class OptimisticConcurrencyException : Exception
    {
        public long ActualVersion { get; private set; }
        public long ExpectedVersion { get; private set; }
        public IIdentity Id { get; private set; }
        public IEnumerable<IEvent> ActualEvents { get; private set; }

        OptimisticConcurrencyException(
            string message,
            long actualVersion,
            long expectedVersion,
            IIdentity id,
            IEnumerable<IEvent> serverEvents)
            : base(message)
        {
            ActualVersion = actualVersion;
            ExpectedVersion = expectedVersion;
            Id = id;
            ActualEvents = serverEvents;
        }

        public static OptimisticConcurrencyException Create(
            long actual,
            long expected,
            IIdentity id,
            IEnumerable<IEvent> serverEvents)
        {
            var message = $"Expected v{expected} but found v{actual} in stream '{id}'";
            return new OptimisticConcurrencyException(message, actual, expected, id, serverEvents);
        }
    }
}
