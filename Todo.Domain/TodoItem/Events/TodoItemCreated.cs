namespace Todo.Domain.TodoItem.Events
{
    public record TodoItemCreated(
        TodoItemId TodoItemId,
        string Description
    ) : IEvent;
}
