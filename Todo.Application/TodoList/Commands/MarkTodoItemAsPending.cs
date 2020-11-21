using Todo.Domain.TodoItem;

namespace Todo.Application.TodoList.Commands
{
    public record MarkTodoItemAsPending(
        TodoItemId Id
    ) : ICommand;
}
