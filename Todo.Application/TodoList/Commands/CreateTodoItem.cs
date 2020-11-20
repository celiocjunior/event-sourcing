using System;
using Todo.Domain.TodoItem;

namespace Todo.Application.TodoList.Commands
{
    public sealed class CreateTodoItem : ICommand
    {
        public TodoItemId Id { get; set; } = new TodoItemId(Guid.Empty);

        public string Description { get; set; } = string.Empty;
    }
}
