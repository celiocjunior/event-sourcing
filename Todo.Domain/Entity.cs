using System;
using System.Collections.Generic;

namespace Todo.Domain
{
    public abstract class Entity<TState>
        where TState : State, new()
    {
        private readonly IList<IEvent> _changes;

        public TState State { get; }
        public IEnumerable<IEvent> Changes => _changes;

        protected Entity()
        {
            State = new TState();
            _changes = new List<IEvent>();
        }

        protected Entity(IEnumerable<IEvent> events) : this()
        {
            var isEmpty = true;
            foreach (var e in events)
            {
                isEmpty = false;
                State.Mutate(e);
            }

            if (isEmpty)
                throw new ArgumentException("Event stream is empty", nameof(events));
        }

        protected void Apply<TEvent>(TEvent e)
            where TEvent : IEvent
        {
            State.Mutate(e);
            _changes.Add(e);
        }
    }
}
