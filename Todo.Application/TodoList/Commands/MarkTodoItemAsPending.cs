using Todo.Domain.TodoItem;

namespace Todo.Application.TodoList.Commands
{
    public class MarkTodoItemAsPending : ICommand
    {
        public TodoItemId Id { get; set; }
    }
}
