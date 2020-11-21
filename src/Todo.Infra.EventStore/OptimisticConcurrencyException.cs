using System;
using System.Collections.Generic;
using Todo.Domain;

namespace Todo.Infra.EventStore
{
    public class OptimisticConcurrencyException : Exception
    {
        public long ActualVersion { get; }
        public long ExpectedVersion { get; }
        public IIdentity Id { get; }
        public IEnumerable<IEvent> ActualEvents { get; }

        OptimisticConcurrencyException(
            string message,
            long actualVersion,
            long expectedVersion,
            IIdentity id,
            IEnumerable<IEvent> actualEvents)
            : base(message)
        {
            ActualVersion = actualVersion;
            ExpectedVersion = expectedVersion;
            Id = id;
            ActualEvents = actualEvents;
        }

        public static OptimisticConcurrencyException Create(
            long actual,
            long expected,
            IIdentity id,
            IEnumerable<IEvent> actualEvents)
        {
            var message = $"Expected v{expected} but found v{actual} in stream '{id}'";
            return new OptimisticConcurrencyException(message, actual, expected, id, actualEvents);
        }
    }
}
