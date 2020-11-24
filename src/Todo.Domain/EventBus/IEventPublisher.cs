namespace Todo.Domain.EventBus
{
    public interface IEventPublisher
    {
        void Publish(IEvent @event);
    }
}
