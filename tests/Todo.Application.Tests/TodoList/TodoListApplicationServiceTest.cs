using FluentAssertions;
using Todo.Application.TodoList;
using Todo.Application.TodoList.Commands;
using Todo.Domain.EventStore;
using Todo.Domain.TodoItem;
using Todo.Infra.EventStore;
using Todo.Infra.InMemoryEventStore;
using Xunit;

namespace Todo.Application.Tests.TodoList
{
    public class TodoListApplicationServiceTest
    {
        private readonly IEventStore _eventStore;
        public TodoListApplicationService Application { get; }

        public TodoListApplicationServiceTest()
        {
            _eventStore = new EventStore(new InMemoryEventStore());
            Application = new TodoListApplicationService(_eventStore);
        }

        [Fact]
        public void TestTodoItemUsage()
        {
            TodoItem todoItem;

            var id = Application.Execute(new CreateTodoItem("Learn E/S"));
            todoItem = GetTodoItem(id);
            todoItem.State.Id.Should().Be(id);
            todoItem.State.Description.Should().Be("Learn E/S");
            todoItem.State.Done.Should().BeFalse();

            Application.Execute(new MarkTodoItemAsDone(id));
            todoItem = GetTodoItem(id);
            todoItem.State.Id.Should().Be(id);
            todoItem.State.Description.Should().Be("Learn E/S");
            todoItem.State.Done.Should().BeTrue();

            Application.Execute(new UpdateTodoItemDescription(id, "Learn event sourcing"));
            todoItem = GetTodoItem(id);
            todoItem.State.Id.Should().Be(id);
            todoItem.State.Description.Should().Be("Learn event sourcing");
            todoItem.State.Done.Should().BeTrue();

            Application.Execute(new MarkTodoItemAsPending(id));
            todoItem = GetTodoItem(id);
            todoItem.State.Id.Should().Be(id);
            todoItem.State.Description.Should().Be("Learn event sourcing");
            todoItem.State.Done.Should().BeFalse();
        }

        private TodoItem GetTodoItem(TodoItemId id)
        {
            var eventStream = _eventStore.LoadEventStream(id);
            return TodoItem.ReplayEvents(eventStream.Events);
        }
    }
}
