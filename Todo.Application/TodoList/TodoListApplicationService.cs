using System;
using System.Linq;
using Todo.Application.TodoList.Commands;
using Todo.Domain.EventStore;
using Todo.Domain.TodoItem;

namespace Todo.Application.TodoList
{
    public class TodoListApplicationService : IApplicationService,
        ICommandHandler<CreateTodoItem, TodoItemId>,
        ICommandHandler<MarkTodoItemAsDone, Unit>,
        ICommandHandler<MarkTodoItemAsPending, Unit>,
        ICommandHandler<UpdateTodoItemDescription, Unit>
    {
        private readonly IEventStore _eventStore;

        public TodoListApplicationService(IEventStore eventStore)
        {
            _eventStore = eventStore;
        }

        public TResult Execute<TResult>(ICommand<TResult> cmd) =>
            (TResult)GetType()
                .GetMethod(nameof(When), new[] { cmd.GetType() })
                ?.Invoke(this, new[] { cmd })
                ?? throw new MissingMethodException($"No method found for command ({cmd.GetType().Name})");

        public TodoItemId When(CreateTodoItem command)
        {
            const long INITIAL_VERSION = 0;

            var id = new TodoItemId(Guid.NewGuid());
            var todoItem = new TodoItem(id, command.Description);

            _eventStore.AppendToStream(id, INITIAL_VERSION, todoItem.Changes.Single());

            return id;
        }

        public Unit When(MarkTodoItemAsDone command) =>
            Update(command.Id, t => t.MarkAsDone());

        public Unit When(MarkTodoItemAsPending command) =>
            Update(command.Id, t => t.MarkAsPending());

        public Unit When(UpdateTodoItemDescription command) =>
            Update(command.Id, t => t.UpdateDescription(command.Description));

        private Unit Update(TodoItemId id, Action<TodoItem> execute)
        {
            EventStream stream = _eventStore.LoadEventStream(id);
            TodoItem todoItem = TodoItem.ReplayEvents(stream.Events);
            execute(todoItem);
            foreach (var change in todoItem.Changes)
                _eventStore.AppendToStream(id, stream.Version, change);

            return Unit.Value;
        }
    }
}
