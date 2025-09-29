// Copyright (c) SC Strategic Solutions. All rights reserved.


// Copyright (c) SC Strategic Solutions. All rights reserved.

using FluentValidation;

namespace Domain.Identity
{
    public class PersonName
    {
        //We can make this an enum if we want to limit the options for title
        public string Title { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string MiddleName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Suffix { get; set; } = string.Empty;
        public string PreferredName { get; set; } = string.Empty;

        public string FullName => $"{FirstName} {LastName}";

        public string FormalName => $"{Title} {FullName} {Suffix}";

        public override string ToString() => $"{LastName}, {FirstName}";
    }

    public class PersonNameValidator : AbstractValidator<PersonName>
    {
        public PersonNameValidator()
        {
            RuleFor(x => x.FirstName).NotEmpty();
            RuleFor(x => x.LastName).NotEmpty();
        }
    }
}
