namespace Todo.Application
{
    public interface IBaseCommand { }

    public interface ICommand<out TResult> : IBaseCommand { }

    public interface ICommand : ICommand<Unit> { }
}
