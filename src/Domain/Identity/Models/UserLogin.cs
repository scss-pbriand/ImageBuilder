// Copyright (c) SC Strategic Solutions. All rights reserved.


// Copyright (c) SC Strategic Solutions. All rights reserved.

using Microsoft.AspNetCore.Identity;

namespace Domain.Identity.Models;

public sealed class UserLogin : UserLoginInfo, IEquatable<UserLogin>
{
    public UserLogin(string loginProvider, string providerKey, string? providerDisplayName)
        : base(loginProvider, providerKey, providerDisplayName) { }

    public override bool Equals(object? obj) => Equals(obj as UserLogin);

    public bool Equals(UserLogin? other)
    {
        if (other == null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return LoginProvider == other.LoginProvider &&
               ProviderKey == other.ProviderKey;
    }

    public override int GetHashCode() => HashCode.Combine(LoginProvider, ProviderKey);
}
