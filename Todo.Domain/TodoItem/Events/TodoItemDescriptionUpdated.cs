using System;

namespace Todo.Domain.TodoItem.Events
{
    public class TodoItemDescriptionUpdated : IEvent
    {
        private const int EVENT_VERSION = 1;

        public int EventVersion { get; }

        public DateTime OcurredOn { get; }

        public TodoItemId TodoItemId { get; }

        public string OldDescription { get; }

        public string NewDescription { get; }

        public TodoItemDescriptionUpdated(TodoItemId todoItemId, string oldDescription, string newDescription)
        {
            EventVersion = EVENT_VERSION;
            OcurredOn = DateTime.UtcNow;
            TodoItemId = todoItemId;
            OldDescription = oldDescription;
            NewDescription = newDescription;
        }
    }
}
