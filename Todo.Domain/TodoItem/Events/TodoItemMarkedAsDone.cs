namespace Todo.Domain.TodoItem.Events
{
    public record TodoItemMarkedAsDone(
        TodoItemId TodoItemId
    ) : IEvent;
}
