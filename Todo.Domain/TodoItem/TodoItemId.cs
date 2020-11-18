using System;

namespace Todo.Domain.TodoItem
{
    public struct TodoItemId : IIdentity, IEquatable<TodoItemId>
    {
        public Guid Id { get; }

        public TodoItemId(Guid id)
        {
            Id = id;
        }

        public bool Equals(TodoItemId other)
        {
            return Id.Equals(other.Id);
        }

        public override bool Equals(object? obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((TodoItemId)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id);
        }
    }
}
