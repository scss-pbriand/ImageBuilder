using FluentValidation;
using ImageBuilder.Server.Models;

namespace ImageBuilder.Server.Validators;

public sealed class CollectionValidator : AbstractValidator<Collection>
{
    public CollectionValidator()
    {
        RuleFor(c => c.Title).NotEmpty();
        RuleForEach(c => c.Categories).SetValidator(new CategoryValidator());
    }
}
