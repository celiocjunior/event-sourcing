using FluentAssertions;
using System;
using System.Linq;
using Todo.Domain.TodoItem.Events;
using Xunit;

namespace Todo.Domain.Tests.TodoItem
{
    public class TodoItemTest
    {
        [Fact]
        public void TestCreateNew()
        {
            var id = new Domain.TodoItem.TodoItemId(Guid.NewGuid());
            var description = "Learn event sourcing";
            var todoItem = new Domain.TodoItem.TodoItem(id, description);

            todoItem.Changes
                .Should()
                .HaveCount(1);

            todoItem.Changes.First()
                .Should()
                .BeOfType(typeof(TodoItemCreated));

            todoItem.State.Id
                .Should()
                .Be(id);

            todoItem.State.Description
                .Should()
                .Be(description);

            todoItem.State.Done
                .Should()
                .BeFalse();
        }

        [Fact]
        public void TestCreateFromEvents()
        {
            var id = new Domain.TodoItem.TodoItemId(Guid.NewGuid());
            var description = "Learn event sourcing";
            var events = new IEvent[]
            {
                new TodoItemCreated(id, description, DateTime.UtcNow),
                new TodoItemMarkedAsDone(id, DateTime.UtcNow)
            };

            var todoItem = Domain.TodoItem.TodoItem.ReplayEvents(events);

            todoItem.Changes
                .Should()
                .HaveCount(0);

            todoItem.State.Id
                .Should()
                .Be(id);

            todoItem.State.Description
                .Should()
                .Be(description);

            todoItem.State.Done
                .Should()
                .BeTrue();
        }

        [Fact]
        public void TestCreateAndReplayShouldHaveTheSameResult()
        {
            var id = new Domain.TodoItem.TodoItemId(Guid.NewGuid());
            var description = "Learn event sourcing";

            // Create
            var todoItem = new Domain.TodoItem.TodoItem(id, description);

            todoItem.MarkAsDone();
            todoItem.UpdateDescription("Learn reactive architecture");
            todoItem.MarkAsPending();

            // Replay
            var todoItemReplay = Domain.TodoItem.TodoItem.ReplayEvents(todoItem.Changes);

            // Should have the same result
            todoItemReplay.State.Id
                .Should().Be(todoItem.State.Id);

            todoItemReplay.State.Done
                .Should().Be(todoItem.State.Done);

            todoItemReplay.State.Pending
                .Should().Be(todoItem.State.Pending);

            todoItemReplay.State.Description
                .Should().Be(todoItem.State.Description);
        }
    }
}
