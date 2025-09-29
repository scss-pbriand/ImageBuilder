// Copyright (c) SC Strategic Solutions. All rights reserved.

namespace Domain.Identity.Models;

using SecurityClaim = System.Security.Claims.Claim;

public sealed class Claim
{

    public string Type { get; set; }
    public string Value { get; set; }
    public string Issuer { get; set; }

    public SecurityClaim ToClaim() => new(Type, Value, Issuer);

    private sealed class TypeValueIssuerEqualityComparer : IEqualityComparer<Claim>
    {
        public bool Equals(Claim x, Claim y)
        {
            if (ReferenceEquals(x,
                                y))
            {
                return true;
            }

            if (ReferenceEquals(x,
                                null))
            {
                return false;
            }

            if (ReferenceEquals(y,
                                null))
            {
                return false;
            }

            if (x.GetType() != y.GetType())
            {
                return false;
            }

            return x.Type == y.Type && x.Value == y.Value && x.Issuer == y.Issuer;
        }

        public int GetHashCode(Claim obj) =>
            HashCode.Combine(obj.Type,
                             obj.Value,
                             obj.Issuer);
    }

    public static IEqualityComparer<Claim> TypeValueIssuerComparer { get; } = new TypeValueIssuerEqualityComparer();


}
