using System;

namespace Todo.Domain
{
    public interface IEvent
    {
        public int EventVersion { get; }
        public DateTime OcurredOn { get; }
    }
}
