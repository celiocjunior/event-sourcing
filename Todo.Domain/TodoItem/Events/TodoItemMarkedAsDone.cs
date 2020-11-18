using System;

namespace Todo.Domain.TodoItem.Events
{
    public class TodoItemMarkedAsDone : IEvent
    {
        private const int EVENT_VERSION = 1;

        public int EventVersion { get; }

        public DateTime OcurredOn { get; }

        public TodoItemId TodoItemId { get; }

        public TodoItemMarkedAsDone(TodoItemId todoItemId)
        {
            EventVersion = EVENT_VERSION;
            OcurredOn = DateTime.UtcNow;
            TodoItemId = todoItemId;
        }
    }
}
