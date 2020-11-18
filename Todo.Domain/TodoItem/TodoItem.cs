using System.Collections.Generic;
using Todo.Domain.TodoItem.Events;

namespace Todo.Domain.TodoItem
{
    public class TodoItem : Entity<TodoItemState>
    {
        public TodoItem(TodoItemId id, string description) : base()
        {
            Apply(new TodoItemCreated(id, description));
        }

        private TodoItem(IEnumerable<IEvent> events) : base(events) { }

        public static TodoItem ReplayEvents(IEnumerable<IEvent> events)
        {
            return new TodoItem(events);
        }

        public void MarkAsDone()
        {
            if (State.Done) return;
            Apply(new TodoItemMarkedAsDone(State.Id));
        }

        public void MarkAsPending()
        {
            if (State.Pending) return;
            Apply(new TodoItemMarkedAsPending(State.Id));
        }

        public void UpdateDescription(string newDescription)
        {
            if (State.Description == newDescription) return;
            Apply(new TodoItemDescriptionUpdated(State.Id, State.Description, newDescription));
        }
    }
}
