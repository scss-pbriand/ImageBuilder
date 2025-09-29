using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Images;

public class ImageAsset
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ImageDataId { get; set; }
    public string? Name { get; set; } = Guid.NewGuid().ToString();
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class ImageAssetValidator : AbstractValidator<ImageAsset>
{
    public ImageAssetValidator()
    {
        RuleFor(it => it.Name)
            .NotEmpty().WithMessage("Name is required.");
    }
}