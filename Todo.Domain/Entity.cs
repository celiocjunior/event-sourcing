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

        protected void Apply(IEvent e)
        {
            // pass each event to modify current in-memory state
            State.Mutate(e);
            // append event to change list for further persistence
            _changes.Add(e);
        }
    }
}
