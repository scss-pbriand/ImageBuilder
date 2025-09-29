// Copyright (c) SC Strategic Solutions. All rights reserved.


// Copyright (c) SC Strategic Solutions. All rights reserved.

using Microsoft.AspNetCore.Identity;

namespace Domain.Identity.Models;

/// <summary>
///     Extensions around the <see cref="UserLogin" /> class
/// </summary>
public static class MartenIdentityUserLoginExtensions
{
    public static UserLogin ToUserLogin(this UserLoginInfo login)
        => new(login.LoginProvider, login.ProviderKey, login.ProviderDisplayName);

    public static UserLoginInfo ToUserLoginInfo(this UserLogin login)
        => new(login.LoginProvider, login.ProviderKey, login.ProviderDisplayName);
}
