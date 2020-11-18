using Todo.Domain.TodoItem;

namespace Todo.Application.TodoList.Commands
{
    public sealed class CreateTodoItem : ICommand
    {
        public TodoItemId Id { get; set; }

        public string Description { get; set; } = string.Empty;
    }
}
