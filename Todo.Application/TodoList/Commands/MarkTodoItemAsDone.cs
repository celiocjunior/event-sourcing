using Todo.Domain.TodoItem;

namespace Todo.Application.TodoList.Commands
{
    public class MarkTodoItemAsDone : ICommand
    {
        public TodoItemId Id { get; set; }
    }
}
