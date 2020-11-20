using System;

namespace Todo.Domain.TodoItem.Events
{
    public record TodoItemMarkedAsDone(
        TodoItemId TodoItemId,
        DateTime OcurredOn
    ) : IEvent;
}
