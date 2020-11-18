namespace Todo.Application
{
    public interface IApplicationService
    {
        void Execute(ICommand cmd);
    }
}
