using Todo.Domain.TodoItem;

namespace Todo.Application.TodoList.Commands
{
    public record CreateTodoItem(
        string Description
    ) : ICommand<TodoItemId>;
}
