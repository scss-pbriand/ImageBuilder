// Copyright (c) SC Strategic Solutions. All rights reserved.


// Copyright (c) SC Strategic Solutions. All rights reserved.

using JasperFx.Core;

namespace Domain.Common;

public record UniqueId : TypedValue<Guid>
{
    public UniqueId() : base(CombGuidIdGeneration.NewGuid()) { }

    public UniqueId(Guid value) : base(value) { }

    public static TKey Create<TKey>() => Create<TKey>(CombGuidIdGeneration.NewGuid());

    public static TKey Create<TKey>(Guid id)
    {
        var constructor = typeof(TKey).GetConstructor([typeof(Guid)]);
        return constructor != null
            ? (TKey)constructor.Invoke([id])
            : throw new InvalidOperationException($"Constructor accepting Guid not found for type {typeof(TKey).Name}");
    }

    public static bool TryParse<T>(string? s, out T result)
    {
        if (string.IsNullOrEmpty(s))
        {
            result = default!;
            return false;
        }

        if (Guid.TryParse(s, out var g1))
        {
            result = Create<T>(g1);
            return true;
        }

        if (IdentifierExtensions.TryDecompress(s, out var g2))
        {
            result = Create<T>(g2);
            return true;
        }

        result = default!;
        return false;
    }

    public bool IsNull() => Value == Guid.Empty;
}
