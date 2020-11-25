using System;

namespace Todo.Domain.TodoItem
{
    public class TodoItemId : IIdentity, IEquatable<TodoItemId>
    {
        public Guid Value { get; }

        public TodoItemId(Guid value)
        {
            Value = value;
        }

        public bool Equals(TodoItemId? other)
        {
            if (other is null) return false;
            return Value.Equals(other.Value);
        }

        public override bool Equals(object? obj)
        {
            if (obj is null) return false;
            if (obj.GetType() != GetType()) return false;
            return Equals((TodoItemId)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Value);
        }

        public static bool operator ==(TodoItemId left, TodoItemId right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(TodoItemId left, TodoItemId right)
        {
            return !(left == right);
        }

        public override string ToString() => Value.ToString();

        public string AsString() => ToString();
    }
}
