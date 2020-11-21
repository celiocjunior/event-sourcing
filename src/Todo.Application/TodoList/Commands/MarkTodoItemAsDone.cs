using Todo.Domain.TodoItem;

namespace Todo.Application.TodoList.Commands
{
    public record MarkTodoItemAsDone(
        TodoItemId Id
    ) : ICommand;
}
