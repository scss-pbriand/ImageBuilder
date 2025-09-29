// Copyright (c) SC Strategic Solutions. All rights reserved.


using Domain.Common;
using Identifier = System.Guid;

namespace SCSM.Domain.SharedKernel;


public record UserId : UniqueId
{
    public UserId() { }
    public UserId(Identifier value) : base(value) { }
    public UserId(string value) : base(Identifier.Parse(value)) { }
    public static UserId Create() => new();

    public static UserId NullUserId => new(Identifier.Empty);
}

public record ImageTypeId : UniqueId
{
    public ImageTypeId() { }
    public ImageTypeId(Identifier value) : base(value) { }
    public ImageTypeId(string value) : base(Identifier.Parse(value)) { }
    public static ImageTypeId Create() => new();
    public static ImageTypeId NullImageTypeId => new(Identifier.Empty);
}

public record GeneratedImageId : UniqueId
{
    public GeneratedImageId() { }
    public GeneratedImageId(Identifier value) : base(value) { }
    public GeneratedImageId(string value) : base(Identifier.Parse(value)) { }
    public static GeneratedImageId Create() => new();
    public static GeneratedImageId NullGeneratedImageId => new(Identifier.Empty);
}
