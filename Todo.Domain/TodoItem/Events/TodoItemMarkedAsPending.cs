using System;

namespace Todo.Domain.TodoItem.Events
{
    public class TodoItemMarkedAsPending : IEvent
    {
        private const int EVENT_VERSION = 1;

        public int EventVersion { get; }

        public DateTime OcurredOn { get; }

        public TodoItemId TodoItemId { get; }

        public bool Done { get; }

        public TodoItemMarkedAsPending(TodoItemId todoItemId)
        {
            EventVersion = EVENT_VERSION;
            OcurredOn = DateTime.UtcNow;
            TodoItemId = todoItemId;
        }
    }
}
