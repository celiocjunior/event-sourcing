namespace Todo.Domain
{
    public abstract class State
    {
        internal void Mutate(IEvent e) =>
            // .NET magic to call one of 'When' handlers with 
            // matching signature 
            ((dynamic)this).When((dynamic)e);
    }
}
