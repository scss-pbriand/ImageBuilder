// Copyright (c) SC Strategic Solutions. All rights reserved.

namespace Domain.Identity.Models;

public class RecoveryCode : IEquatable<RecoveryCode>
{

    public string Code { get; set; }

    public RecoveryCode()
    {
        Code = string.Empty;
    }

    public RecoveryCode(string code)
    {
        Code = code;
    }

    public bool Equals(RecoveryCode? other)
    {
        if (ReferenceEquals(null, other))
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return Code == other.Code;
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

        return Equals((RecoveryCode)obj);
    }

}
