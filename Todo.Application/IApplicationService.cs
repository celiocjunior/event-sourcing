namespace Todo.Application
{
    public interface IApplicationService
    {
        void Execute<TCommand>(TCommand cmd)
            where TCommand : ICommand;
    }
}
