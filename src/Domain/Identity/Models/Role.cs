// Copyright (c) SC Strategic Solutions. All rights reserved.

using Microsoft.AspNetCore.Identity;

namespace Domain.Identity.Models;

public class Role : IdentityRole<Guid>, IEquatable<Role>
{

    public string AggregateId
    {
        get => Id.ToString();
    }
    public int Version { get; set; }

    public HashSet<Claim> Claims { get; set; }

    public Role(string name) : base(name)
    {
        NormalizedName = name.Normalize().ToUpperInvariant();
        Id = Guid.NewGuid();
        Claims = new HashSet<Claim>();
    }


    public override string ToString()
        => $"{nameof(Id)}: {Id}, {nameof(Version)}: {Version}, {nameof(Name)}: {Name}, {nameof(NormalizedName)}: {NormalizedName}, {nameof(Claims)}: {Claims}";

    public bool Equals(Role? other)
    {
        if (ReferenceEquals(null,
                            other))
        {
            return false;
        }

        if (ReferenceEquals(this,
                            other))
        {
            return true;
        }

        return Id == other.Id;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null,
                            obj))
        {
            return false;
        }

        if (ReferenceEquals(this,
                            obj))
        {
            return true;
        }

        if (obj.GetType() != GetType())
        {
            return false;
        }

        return Equals((Role)obj);
    }

    public override int GetHashCode() => Id.GetHashCode();

    public static bool operator ==(Role? left, Role? right) => Equals(left, right);

    public static bool operator !=(Role? left, Role? right) => !Equals(left, right);


}
