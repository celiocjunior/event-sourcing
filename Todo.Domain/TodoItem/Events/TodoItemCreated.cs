using System;

namespace Todo.Domain.TodoItem.Events
{
    public class TodoItemCreated : IEvent
    {
        private const int EVENT_VERSION = 1;

        public int EventVersion { get; }

        public DateTime OcurredOn { get; }

        public TodoItemId TodoItemId { get; }

        public string Description { get; private set; }

        public TodoItemCreated(TodoItemId todoItemId, string description)
        {
            EventVersion = EVENT_VERSION;
            OcurredOn = DateTime.UtcNow;
            TodoItemId = todoItemId;
            Description = description;
        }
    }
}
