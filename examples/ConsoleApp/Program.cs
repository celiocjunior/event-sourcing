using System;
using System.IO;
using Todo.Application.TodoList;
using Todo.Application.TodoList.Commands;
using Todo.Domain.EventBus;
using Todo.Domain.TodoItem.Events;
using Todo.Infra.EventStore;
using Todo.Infra.RabbitMqEventBus;
using Todo.Infra.SqliteEventStore;

namespace ConsoleApp
{
    internal class Program
    {
        private static void Main()
        {
            const string dbName = "todo.db";
            if (File.Exists(dbName)) File.Delete(dbName);

            var appendOnlyEventStore = new SqliteEventStore($"Data Source={dbName}");
            var eventBus = new RabbitMqEventBus("host=localhost");
            var eventStore = new EventStore(appendOnlyEventStore, eventBus);
            var appService = new TodoListApplicationService(eventStore);

            SubscribeToEvents(eventBus);

            var id = appService.Execute(new CreateTodoItem("Learn E/S"));
            appService.Execute(new MarkTodoItemAsDone(id));
            appService.Execute(new UpdateTodoItemDescription(id, "Learn event sourcing"));
            appService.Execute(new MarkTodoItemAsPending(id));

            // TODO: get projection

            Console.Write("Press any key to exit...");
            Console.ReadKey();
        }

        private static void SubscribeToEvents(IEventSubscriber subscriber)
        {
            subscriber.Subscribe<TodoItemCreated>(Console.WriteLine);
            subscriber.Subscribe<TodoItemMarkedAsDone>(Console.WriteLine);
            subscriber.Subscribe<TodoItemDescriptionUpdated>(Console.WriteLine);
            subscriber.Subscribe<TodoItemMarkedAsPending>(Console.WriteLine);
        }
    }
}
