using System;
using Todo.Application.TodoList.Commands;
using Todo.Domain.EventStore;
using Todo.Domain.TodoItem;

namespace Todo.Application.TodoList
{
    public class TodoListApplicationService : IApplicationService,
        ICommandHandler<CreateTodoItem>,
        ICommandHandler<MarkTodoItemAsDone>,
        ICommandHandler<MarkTodoItemAsPending>,
        ICommandHandler<UpdateTodoItemDescription>
    {
        private readonly IEventStore _eventStore;

        public TodoListApplicationService(IEventStore eventStore)
        {
            _eventStore = eventStore;
        }

        public void When(CreateTodoItem c)
        {
            const long INITIAL_VERSION = 0;

            var todoItem = new TodoItem(c.Id, c.Description);
            _eventStore.AppendToStream(c.Id, INITIAL_VERSION, todoItem.Changes);
        }

        public void When(MarkTodoItemAsDone c)
        {
            Update(c.Id, t => t.MarkAsDone());
        }

        public void When(MarkTodoItemAsPending c)
        {
            Update(c.Id, t => t.MarkAsPending());
        }

        public void When(UpdateTodoItemDescription c)
        {
            Update(c.Id, t => t.UpdateDescription(c.Description));
        }

        public void Execute<TCommand>(TCommand cmd) where TCommand : ICommand =>
            ((ICommandHandler<TCommand>)this).When(cmd);

        private void Update(TodoItemId id, Action<TodoItem> execute)
        {
            EventStream stream = _eventStore.LoadEventStream(id);
            TodoItem customer = TodoItem.ReplayEvents(stream.Events);
            execute(customer);
            _eventStore.AppendToStream(id, stream.Version, customer.Changes);
        }
    }
}
