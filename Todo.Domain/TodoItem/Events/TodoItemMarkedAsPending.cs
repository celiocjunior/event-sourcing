using System;

namespace Todo.Domain.TodoItem.Events
{
    public record TodoItemMarkedAsPending(
        TodoItemId TodoItemId,
        DateTime OcurredOn
    ) : IEvent;
}
