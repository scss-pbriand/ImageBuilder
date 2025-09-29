using Domain.Images;
using FluentValidation;
using Shouldly;

namespace ImgGen.Test.UnitTests.UI;

[TestClass]
public class ManageImageTypesTests
{
    [TestMethod]
    public void ImageType_Creation_Should_Generate_Valid_Guid()
    {
        // Arrange & Act
        var imageType = new ImageType { Id = Guid.NewGuid() };
        
        // Assert
        imageType.Id.ShouldNotBe(Guid.Empty);
        imageType.Name.ShouldBe(string.Empty);
        imageType.Description.ShouldBeNull();
        imageType.Categories.ShouldNotBeNull();
        imageType.Categories.ShouldBeEmpty();
    }

    [TestMethod]
    public void ImageType_Validation_Should_Require_Name()
    {
        // Arrange
        var imageType = new ImageType
        {
            Id = Guid.NewGuid(),
            Name = "", // Invalid - empty name
            Description = "Test description"
        };
        var validator = new ImageTypeValidator();

        // Act
        var result = validator.Validate(imageType);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(error => error.PropertyName == nameof(ImageType.Name));
    }

    [TestMethod]
    public void ImageType_Validation_Should_Accept_Valid_Data()
    {
        // Arrange
        var imageType = new ImageType
        {
            Id = Guid.NewGuid(),
            Name = "Test Image Type",
            Description = "A valid test description",
            Categories = new List<ImageCategory>
            {
                new ImageCategory
                {
                    Id = Guid.NewGuid(),
                    Name = "Category 1",
                    ProbabilityWeight = 0.5
                }
            }
        };
        var validator = new ImageTypeValidator();

        // Act
        var result = validator.Validate(imageType);

        // Assert
        result.IsValid.ShouldBeTrue();
        result.Errors.ShouldBeEmpty();
    }

    [TestMethod]
    public void ImageType_Validation_Should_Reject_Duplicate_Category_Names()
    {
        // Arrange
        var imageType = new ImageType
        {
            Id = Guid.NewGuid(),
            Name = "Test Image Type",
            Categories = new List<ImageCategory>
            {
                new ImageCategory { Id = Guid.NewGuid(), Name = "Duplicate", ProbabilityWeight = 0.5 },
                new ImageCategory { Id = Guid.NewGuid(), Name = "duplicate", ProbabilityWeight = 0.5 } // Case insensitive duplicate
            }
        };
        var validator = new ImageTypeValidator();

        // Act
        var result = validator.Validate(imageType);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(error => error.ErrorMessage.Contains("Category names must be unique"));
    }

    [TestMethod]
    public void ImageCategory_Validation_Should_Require_Name()
    {
        // Arrange
        var category = new ImageCategory
        {
            Id = Guid.NewGuid(),
            Name = "", // Invalid - empty name
            ProbabilityWeight = 0.5
        };
        var validator = new ImageCategoryValidator();

        // Act
        var result = validator.Validate(category);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(error => error.PropertyName == nameof(ImageCategory.Name));
    }

    [TestMethod]
    public void ImageCategory_Validation_Should_Require_Valid_ProbabilityWeight()
    {
        // Arrange
        var category = new ImageCategory
        {
            Id = Guid.NewGuid(),
            Name = "Valid Name",
            ProbabilityWeight = 1.5 // Invalid - greater than 1
        };
        var validator = new ImageCategoryValidator();

        // Act
        var result = validator.Validate(category);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(error => error.PropertyName == nameof(ImageCategory.ProbabilityWeight));
    }
}