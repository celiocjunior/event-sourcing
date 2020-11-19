namespace Todo.Application
{
    public interface ICommandHandler<TCommand>
        where TCommand : ICommand
    {
        void When(TCommand command);
    }
}
