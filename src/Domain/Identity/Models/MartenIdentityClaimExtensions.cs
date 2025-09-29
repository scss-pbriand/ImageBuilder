// Copyright (c) SC Strategic Solutions. All rights reserved.

using SecurityClaim = System.Security.Claims.Claim;
using Claim = Domain.Identity.Models.Claim;

namespace Domain.Identity.Models;

/// <summary>
///     Extensions regarding <see cref="Claim" /> and <see cref="System.Security.Claims.Claim" />
/// </summary>
public static class MartenIdentityClaimExtensions
{
    public static SecurityClaim ToClaim(this Claim claim) =>
        new(claim.Type, claim.Value, null, claim.Issuer);

    public static Claim? ToModelClaim(this SecurityClaim? claim) => claim is null
        ? null!
        : new Claim
        {
            Type = claim.Type,
            Value = claim.Value,
            Issuer = claim.Issuer
        };
}
