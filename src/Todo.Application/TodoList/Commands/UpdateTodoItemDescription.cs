using Todo.Domain.TodoItem;

namespace Todo.Application.TodoList.Commands
{
    public record UpdateTodoItemDescription(
        TodoItemId Id,
        string Description
    ) : ICommand;
}
