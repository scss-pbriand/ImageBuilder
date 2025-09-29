// Copyright (c) SC Strategic Solutions. All rights reserved.

using System.Text.Json.Serialization;
using Domain.Identity.Models;
using FluentValidation;

namespace Domain.Identity;

public sealed class ApplicationUser : User
{
    public PersonName Name { get; set; } = new();

    /// <summary>
    ///     This is a calculated property to satisfy <c>IUserInfo</c>.  The queryable property is in <see cref="Name" />.
    /// </summary>
    [JsonIgnore]
    public string FullName => Name.FullName;

    public override string ToString() => $"{GetType().Name} {{ Id = {Id}, Name = {Name.FullName} }}";

}

public class ApplicationUserValidator : AbstractValidator<ApplicationUser>
{
    public ApplicationUserValidator()
    {
        RuleFor(x => x.Email).NotEmpty();
        RuleFor(x => x.UserName).NotEmpty();
        RuleFor(x => x.Name).SetValidator(new PersonNameValidator());
    }
}
