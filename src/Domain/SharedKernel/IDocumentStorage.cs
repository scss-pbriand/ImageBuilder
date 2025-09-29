using Domain.Common;

namespace Domain.SharedKernel;

/// <summary>
///     Marker interface indicating aggregate or stream projection is stored as a marten document.
/// </summary>
public interface IDocumentStorage;

public interface IDocumentStorage<TKey> : IDocumentStorage where TKey : UniqueId
{
    TKey Id { get; set; }
}

public interface IIdentifiable
{
    public object GetIdentifier();
}

public interface IStreamAggregate;

public interface IStreamAggregate<TKey> : IStreamAggregate, IDocumentStorage<TKey> where TKey : UniqueId;

public interface IDocumentAggregate<TKey> : IDocumentAggregate, IDocumentStorage<TKey> where TKey : UniqueId
{

    Guid AggregateId => Id.Value;
}

public interface IDocumentAggregate
{

    bool IsDeleted { get; set; }

    DateTimeOffset? DeletedAt { get; set; }

    public DateTimeOffset LastModified { get; set; }
    public string? LastModifiedBy { get; set; }
    public string? CorrelationId { get; set; }

    int Version { get; set; }

}
