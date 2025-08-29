using FluentValidation;
using ImageBuilder.Server.Models;

namespace ImageBuilder.Server.Validators;

public sealed class CategoryValidator : AbstractValidator<Category>
{
    public CategoryValidator()
    {
        RuleFor(c => c.Name).NotEmpty();
        RuleFor(c => c.Probability).InclusiveBetween(0, 100);
    }
}
