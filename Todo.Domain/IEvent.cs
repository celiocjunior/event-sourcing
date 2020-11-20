using System;

namespace Todo.Domain
{
    public interface IEvent
    {
        DateTime OcurredOn { get; }
    }
}
