using System;
using Todo.Domain.TodoItem.Events;

namespace Todo.Domain.TodoItem
{
    public class TodoItemState : State,
        IEventHandler<TodoItemCreated>,
        IEventHandler<TodoItemMarkedAsDone>,
        IEventHandler<TodoItemMarkedAsPending>,
        IEventHandler<TodoItemDescriptionUpdated>
    {
        public TodoItemId Id { get; private set; } = new TodoItemId(Guid.Empty);
        public bool Done { get; private set; } = false;
        public bool Pending => !Done;
        public string Description { get; private set; } = string.Empty;

        public void When(TodoItemCreated e)
        {
            Id = e.TodoItemId;
            Done = false;
            Description = e.Description;
        }

        public void When(TodoItemMarkedAsDone e)
        {
            Done = true;
        }

        public void When(TodoItemMarkedAsPending e)
        {
            Done = false;
        }

        public void When(TodoItemDescriptionUpdated e)
        {
            Description = e.NewDescription;
        }
    }
}
