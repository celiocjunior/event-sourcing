namespace Todo.Application
{
    public interface ICommandHandler<in TCommand, TResult>
        where TCommand : ICommand<TResult>
    {
        TResult When(TCommand command);
    }
}
