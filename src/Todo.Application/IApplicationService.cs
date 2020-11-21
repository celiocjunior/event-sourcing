namespace Todo.Application
{
    public interface IApplicationService
    {
        TResult Execute<TResult>(ICommand<TResult> cmd);
    }
}
