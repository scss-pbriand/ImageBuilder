using Domain.Images;
using FluentValidation;
using Marten;

namespace ImgGen.Application.Repository;

public class ImageTypeRepository
{
    private readonly IDocumentSession _session;
    private readonly IValidator<ImageType> _imageTypeValidator;
    private readonly IValidator<ImageCategory> _imageCategoryValidator;

    public ImageTypeRepository(IDocumentSession session)
    {
        _session = session;
        _imageTypeValidator = new ImageTypeValidator();
        _imageCategoryValidator = new ImageCategoryValidator();
    }

    public async Task<IEnumerable<ImageType>> GetAllImageTypesAsync() =>
        await _session.Query<ImageType>().ToListAsync();

    public IQueryable<ImageType> QueryImageTypes() => _session.Query<ImageType>();

    public async Task<ImageType?> GetImageTypeByIdAsync(Guid id)
    {
        if (id == Guid.Empty) throw new ArgumentException("Id cannot be empty.", nameof(id));
        return await _session.LoadAsync<ImageType>(id);
    }

    public async Task UpsertImageTypeAsync(ImageType imageType)
    {
        _imageTypeValidator.ValidateAndThrow(imageType);
        _session.Store(imageType);
        await _session.SaveChangesAsync();
    }

    public async Task DeleteImageTypeAsync(Guid id)
    {
        if (id == Guid.Empty) throw new ArgumentException("Id cannot be empty.", nameof(id));
        _session.Delete<ImageType>(id);
        await _session.SaveChangesAsync();
    }

    public async Task AddImageCategory(Guid imageTypeId, string categoryName, double probabilityOdds = 1.0)
    {
        if (imageTypeId == Guid.Empty) throw new ArgumentException("Id cannot be empty.", nameof(imageTypeId));
        if (string.IsNullOrWhiteSpace(categoryName)) throw new ArgumentException("Category name is required.", nameof(categoryName));
        if (probabilityOdds < 0 || probabilityOdds > 1) throw new ArgumentOutOfRangeException(nameof(probabilityOdds));

        var imageType = await GetImageTypeByIdAsync(imageTypeId);
        if (imageType is null) return;

        imageType.Categories ??= new List<ImageCategory>();

        if (!imageType.Categories.Any(c => string.Equals(c.Name, categoryName, StringComparison.OrdinalIgnoreCase)))
        {
            var category = new ImageCategory
            {
                Id = Guid.NewGuid(),
                Name = categoryName,
                ProbabilityWeight = probabilityOdds,
                InsertionOrder = imageType.Categories.Count
            };

            _imageCategoryValidator.ValidateAndThrow(category);
            imageType.Categories.Add(category);
            _imageTypeValidator.ValidateAndThrow(imageType);

            await UpsertImageTypeAsync(imageType);
        }
    }

    public async Task RemoveImageCategory(Guid imageTypeId, string categoryName)
    {
        if (imageTypeId == Guid.Empty) throw new ArgumentException("Id cannot be empty.", nameof(imageTypeId));
        if (string.IsNullOrWhiteSpace(categoryName)) throw new ArgumentException("Category name is required.", nameof(categoryName));

        var imageType = await GetImageTypeByIdAsync(imageTypeId);
        if (imageType?.Categories is null || imageType.Categories.Count == 0) return;

        var category = imageType.Categories.FirstOrDefault(c => string.Equals(c.Name, categoryName, StringComparison.OrdinalIgnoreCase));
        if (category is null) return;

        imageType.Categories.Remove(category);
        _imageTypeValidator.ValidateAndThrow(imageType);

        await UpsertImageTypeAsync(imageType);
    }

    public async Task UpdateImageCategory(Guid imageTypeId, string oldName, string newName)
    {
        if (imageTypeId == Guid.Empty) throw new ArgumentException("Id cannot be empty.", nameof(imageTypeId));
        if (string.IsNullOrWhiteSpace(oldName)) throw new ArgumentException("Old category name is required.", nameof(oldName));
        if (string.IsNullOrWhiteSpace(newName)) throw new ArgumentException("New category name is required.", nameof(newName));

        var imageType = await GetImageTypeByIdAsync(imageTypeId);
        if (imageType?.Categories is null || imageType.Categories.Count == 0) return;

        var category = imageType.Categories.FirstOrDefault(c => string.Equals(c.Name, oldName, StringComparison.OrdinalIgnoreCase));
        if (category is null) return;

        category.Name = newName;
        _imageCategoryValidator.ValidateAndThrow(category);
        _imageTypeValidator.ValidateAndThrow(imageType);

        await UpsertImageTypeAsync(imageType);
    }
}
