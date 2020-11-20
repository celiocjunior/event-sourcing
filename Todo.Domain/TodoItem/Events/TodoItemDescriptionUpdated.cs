using System;

namespace Todo.Domain.TodoItem.Events
{
    public record TodoItemDescriptionUpdated(
        TodoItemId TodoItemId,
        string OldDescription,
        string NewDescription,
        DateTime OcurredOn
    ) : IEvent;
}
