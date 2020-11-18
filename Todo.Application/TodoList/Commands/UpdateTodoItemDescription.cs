using Todo.Domain.TodoItem;

namespace Todo.Application.TodoList.Commands
{
    public class UpdateTodoItemDescription : ICommand
    {
        public TodoItemId Id { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}
