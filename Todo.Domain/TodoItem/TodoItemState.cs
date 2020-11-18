using System;
using Todo.Domain.TodoItem.Events;

namespace Todo.Domain.TodoItem
{
    public class TodoItemState : State
    {
        public TodoItemId Id { get; private set; } = new TodoItemId(Guid.Empty);
        public DateTime CreatedOn { get; private set; } = DateTime.MinValue;
        public DateTime? LastUpdateOn { get; private set; } = null;
        public bool Done { get; private set; } = false;
        public bool Pending => !Done;
        public string Description { get; private set; } = string.Empty;

        public void When(TodoItemCreated e)
        {
            Id = e.TodoItemId;
            CreatedOn = e.OcurredOn;
            Done = false;
            Description = e.Description;
        }

        public void When(TodoItemMarkedAsDone e)
        {
            Done = true;
            LastUpdateOn = e.OcurredOn;
        }

        public void When(TodoItemMarkedAsPending e)
        {
            Done = false;
            LastUpdateOn = e.OcurredOn;
        }

        public void When(TodoItemDescriptionUpdated e)
        {
            Description = e.NewDescription;
            LastUpdateOn = e.OcurredOn;
        }
    }
}
