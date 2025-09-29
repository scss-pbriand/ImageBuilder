// Copyright (c) SC Strategic Solutions. All rights reserved.


// Copyright (c) SC Strategic Solutions. All rights reserved.

using Microsoft.AspNetCore.Identity;

namespace Domain.Identity.Models;

public class UserToken : IEquatable<UserToken>
{

    public virtual string LoginProvider { get; set; } = string.Empty;

    public virtual string Name { get; set; } = string.Empty;

    [ProtectedPersonalData]
    public virtual string? Value { get; set; }

    public bool Equals(UserToken? other)
    {
        if (ReferenceEquals(null, other))
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return LoginProvider == other.LoginProvider
               && Name == other.Name;
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

        if (obj.GetType() != GetType())
        {
            return false;
        }

        return Equals((UserToken)obj);
    }

    public override int GetHashCode() =>
        HashCode.Combine(LoginProvider, Name);

    public static bool operator ==(UserToken? left, UserToken? right) => Equals(left, right);

    public static bool operator !=(UserToken? left, UserToken? right) => !Equals(left, right);



}
