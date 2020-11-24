using FluentAssertions;
using System;
using Todo.Application.TodoList;
using Todo.Application.TodoList.Commands;
using Todo.Domain.EventBus;
using Todo.Domain.EventStore;
using Todo.Domain.TodoItem;
using Todo.Domain.TodoItem.Events;
using Todo.Infra.EventStore;
using Todo.Infra.InMemoryEventBus;
using Todo.Infra.InMemoryEventStore;
using Xunit;

namespace Todo.Application.Tests.TodoList
{
    public class TodoListApplicationServiceTest
    {
        private readonly IEventStore _eventStore;
        private readonly IEventSubscriber _eventSubscriber;
        public TodoListApplicationService Application { get; }

        public TodoListApplicationServiceTest()
        {
            var bus = new InMemoryEventBus();
            _eventSubscriber = bus;
            _eventStore = new EventStore(new InMemoryEventStore(), bus);
            Application = new TodoListApplicationService(_eventStore);
        }

        [Fact]
        public void TestTodoItemUsage()
        {
            TodoItem todoItem;

            var todoItemCreatedSubscriptionCalled1 = false;
            var todoItemCreatedSubscriptionCalled2 = false;
            var todoItemCreatedSubscriptionCalled3 = false;
            var todoItemMarkedAsDoneSubscriptionCalled = false;
            var todoItemDescriptionUpdatedSubscriptionCalled = false;
            var todoItemMarkedAsPendingSubscriptionCalled = false;

            var todoItemCreatedSubscription1 = new Action<TodoItemCreated>(e => todoItemCreatedSubscriptionCalled1 = true);
            var todoItemCreatedSubscription2 = new Action<TodoItemCreated>(e => todoItemCreatedSubscriptionCalled2 = true);
            var todoItemCreatedSubscription3 = new Action<TodoItemCreated>(e => todoItemCreatedSubscriptionCalled3 = true);
            var todoItemMarkedAsDoneSubscription = new Action<TodoItemMarkedAsDone>(e => todoItemMarkedAsDoneSubscriptionCalled = true);
            var todoItemDescriptionUpdatedSubscription = new Action<TodoItemDescriptionUpdated>(e => todoItemDescriptionUpdatedSubscriptionCalled = true);
            var todoItemMarkedAsPendingSubscription = new Action<TodoItemMarkedAsPending>(e => todoItemMarkedAsPendingSubscriptionCalled = true);

            _eventSubscriber.Subscribe(todoItemCreatedSubscription1);
            var sub = _eventSubscriber.Subscribe(todoItemCreatedSubscription2);
            _eventSubscriber.Subscribe(todoItemCreatedSubscription3);
            _eventSubscriber.Subscribe(todoItemMarkedAsDoneSubscription);
            _eventSubscriber.Subscribe(todoItemDescriptionUpdatedSubscription);
            _eventSubscriber.Subscribe(todoItemMarkedAsPendingSubscription);

            sub.Dispose();

            var id = Application.Execute(new CreateTodoItem("Learn E/S"));
            todoItem = GetTodoItem(id);
            todoItem.State.Id.Should().Be(id);
            todoItem.State.Description.Should().Be("Learn E/S");
            todoItem.State.Done.Should().BeFalse();
            todoItemCreatedSubscriptionCalled1.Should().BeTrue();
            todoItemCreatedSubscriptionCalled2.Should().BeFalse();
            todoItemCreatedSubscriptionCalled3.Should().BeTrue();

            Application.Execute(new MarkTodoItemAsDone(id));
            todoItem = GetTodoItem(id);
            todoItem.State.Id.Should().Be(id);
            todoItem.State.Description.Should().Be("Learn E/S");
            todoItem.State.Done.Should().BeTrue();
            todoItemMarkedAsDoneSubscriptionCalled.Should().BeTrue();

            Application.Execute(new UpdateTodoItemDescription(id, "Learn event sourcing"));
            todoItem = GetTodoItem(id);
            todoItem.State.Id.Should().Be(id);
            todoItem.State.Description.Should().Be("Learn event sourcing");
            todoItem.State.Done.Should().BeTrue();
            todoItemDescriptionUpdatedSubscriptionCalled.Should().BeTrue();

            Application.Execute(new MarkTodoItemAsPending(id));
            todoItem = GetTodoItem(id);
            todoItem.State.Id.Should().Be(id);
            todoItem.State.Description.Should().Be("Learn event sourcing");
            todoItem.State.Done.Should().BeFalse();
            todoItemMarkedAsPendingSubscriptionCalled.Should().BeTrue();
        }

        private TodoItem GetTodoItem(TodoItemId id)
        {
            var eventStream = _eventStore.LoadEventStream(id);
            return TodoItem.ReplayEvents(eventStream.Events);
        }
    }
}
