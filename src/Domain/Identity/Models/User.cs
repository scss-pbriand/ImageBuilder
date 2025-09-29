// Copyright (c) SC Strategic Solutions. All rights reserved.

using System.Text.Json.Serialization;
using Domain.SharedKernel;
using Microsoft.AspNetCore.Identity;
using SCSM.Domain.SharedKernel;

namespace Domain.Identity.Models;

public class User : IdentityUser<UserId>, IDocumentAggregate<UserId>
{

    public User()

    {
        Id = UserId.Create();
        Roles = new HashSet<UserRole>();
        Claims = new HashSet<Claim>();
        Logins = new HashSet<UserLogin>();
        Tokens = new HashSet<UserToken>();
        RecoveryCodes = new HashSet<RecoveryCode>();
        SecurityStamp = Guid.NewGuid().ToString("N");
    }

    public Guid AggregateId
    {
        get => Id.Value;
    }

    [JsonIgnore]
    public bool IsDeleted { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
    public DateTimeOffset LastModified { get; set; }
    public string? LastModifiedBy { get; set; }
    public string? CorrelationId { get; set; }

    public HashSet<UserRole> Roles { get; set; }
    public HashSet<Claim> Claims { get; set; }
    public HashSet<UserLogin> Logins { get; set; }
    public HashSet<UserToken> Tokens { get; set; }
    public HashSet<RecoveryCode> RecoveryCodes { get; set; }
    public int Version { get; set; }

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

        return obj.GetType() == GetType()
               && Id.Equals(((User)obj).Id);
    }

    public override int GetHashCode() => Id.GetHashCode();
}
