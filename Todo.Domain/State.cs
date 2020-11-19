using System.Linq;

namespace Todo.Domain
{
    public abstract class State
    {
        public void Mutate<TEvent>(TEvent e) where TEvent : IEvent
        {
            if (typeof(TEvent) == typeof(IEvent))
            {
                GetType()
                    .GetMethods()
                    .Single(_ => _.Name == "When" && _.GetParameters().Single().ParameterType == e.GetType())
                    .Invoke(this, new object[] { e });

                return;
            }

            ((IEventHandler<TEvent>)this).When(e);
        }
    }
}
