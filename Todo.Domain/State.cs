namespace Todo.Domain
{
    public abstract class State
    {
        internal void Mutate(IEvent e) =>
            ((dynamic)this).When((dynamic)e);
    }
}
