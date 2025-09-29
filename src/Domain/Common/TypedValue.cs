// Copyright (c) SC Strategic Solutions. All rights reserved.

namespace Domain.Common;

public record TypedValue<T> where T : IComparable<T>
{
    public T Value { get; init; }

    public TypedValue(T value)
    {
        Value = value;
    }

    public virtual bool Equals(TypedValue<T>? other)
    {
        if (ReferenceEquals(null, other))
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return EqualityComparer<T>.Default.Equals(Value, other.Value);
    }

    public override int GetHashCode() => EqualityComparer<T>.Default.GetHashCode(Value);
    public override string ToString() => Value?.ToString() ?? "";
}
