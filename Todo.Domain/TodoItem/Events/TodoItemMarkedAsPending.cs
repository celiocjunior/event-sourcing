namespace Todo.Domain.TodoItem.Events
{
    public record TodoItemMarkedAsPending(
        TodoItemId TodoItemId
    ) : IEvent;
}
