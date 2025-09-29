// Copyright (c) SC Strategic Solutions. All rights reserved.

using System.Text.Json.Serialization;
using SCSM.Domain.SharedKernel;

namespace Domain.Identity.Models;

public class UserRole : IEquatable<UserRole>
{

    public UserRole() { }
    public UserRole(Guid roleId)
    {
        RoleId = roleId;
    }

    public Guid RoleId { get; init; }

    public bool Equals(UserRole? other)
    {
        if (ReferenceEquals(null, other))
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return RoleId.Equals(other.RoleId);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj))
        {
            return false;
        }

        if (ReferenceEquals(this, obj))
        {
            return true;
        }

        if (obj.GetType() != this.GetType())
        {
            return false;
        }

        return Equals((UserRole)obj);
    }

    public override int GetHashCode() => RoleId.GetHashCode();

    public static bool operator ==(UserRole? left, UserRole? right) => Equals(left, right);

    public static bool operator !=(UserRole? left, UserRole? right) => !Equals(left, right);


}
