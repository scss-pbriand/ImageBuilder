using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Images;

public class ImageCategory
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public double ProbabilityWeight { get; set; } = 1.0;
    public int InsertionOrder { get; set; } = 0;
    public virtual List<ImageAsset> ImageAssets { get; set; } = [];
}

public class ImageCategoryValidator : AbstractValidator<ImageCategory>
{
    public ImageCategoryValidator()
    {
        RuleFor(it => it.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(100).WithMessage("Name must not exceed 100 characters.");
        RuleFor(it => it.Description)
            .MaximumLength(500).WithMessage("Description must not exceed 500 characters.");
        RuleFor(it => it.ProbabilityWeight)
            .InclusiveBetween(0, 1).WithMessage("Probability weight must be between 0 and 1.");
        RuleForEach(it => it.ImageAssets)
            .SetValidator(new ImageAssetValidator());
    }
}