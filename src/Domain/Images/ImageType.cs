using FluentValidation;

namespace Domain.Images;

public class ImageType
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public List<ImageCategory> Categories { get; set; } = [];
}

public class ImageTypeValidator : AbstractValidator<ImageType>
{
    public ImageTypeValidator()
    {
        RuleFor(it => it.Name)
            .NotEmpty().WithMessage("ImageType name is required.")
            .MaximumLength(100).WithMessage("ImageType name must not exceed 100 characters.");

        RuleFor(it => it.Description)
            .MaximumLength(500).WithMessage("ImageType description must not exceed 500 characters.");

        RuleForEach(it => it.Categories)
            .SetValidator(new ImageCategoryValidator());

        RuleFor(it => it.Categories)
            .Must(cats =>
            {
                if (cats is null) return true;
                var names = cats.Select(c => (c.Name ?? string.Empty).Trim().ToLowerInvariant());
                return names.Distinct().Count() == cats.Count;
            })
            .WithMessage("Category names must be unique within the ImageType (case-insensitive).");
    }
}
