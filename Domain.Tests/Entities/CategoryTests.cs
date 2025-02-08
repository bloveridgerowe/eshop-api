using Domain.Entities;
using Domain.Exceptions.Categories;
using Xunit;

namespace Domain.Tests.Entities;

public class CategoryTests
{
    private static readonly Guid ValidId = Guid.NewGuid();
    private static readonly String ValidName = "Electronics";
    
    private static Category CreateValidCategory(
        Guid? id = null,
        String? name = null)
    {
        return new Category(
            id ?? ValidId,
            name ?? ValidName
        );
    }

    public class Constructor
    {
        [Fact]
        public void GivenValidParameters_ShouldCreateCategory()
        {
            // Arrange
            Guid id = Guid.NewGuid();
            String name = "Electronics";

            // Act
            Category category = new Category(id, name);

            // Assert
            Assert.Equal(id, category.Id);
            Assert.Equal(name, category.Name);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void GivenEmptyName_ShouldThrowCategoryValidationException(String? invalidName)
        {
            // Act & Assert
            CategoryValidationException exception = Assert.Throws<CategoryValidationException>(() => 
                new Category(ValidId, invalidName!));
            
            Assert.Equal("Category name cannot be empty", exception.Message);
        }

        [Fact]
        public void GivenNameTooLong_ShouldThrowCategoryValidationException()
        {
            // Arrange
            String longName = new String('a', 51);
            
            // Act & Assert
            CategoryValidationException exception = Assert.Throws<CategoryValidationException>(() => 
                new Category(ValidId, longName));
            
            Assert.Equal("Category name cannot be longer than 50 characters", exception.Message);
        }

        [Theory]
        [InlineData("Home and Garden")]
        [InlineData("Gaming 2023")]
        [InlineData("Tech Accessories")]
        public void GivenValidName_ShouldCreateCategory(String validName)
        {
            // Act
            Category category = new Category(ValidId, validName);

            // Assert
            Assert.Equal(validName, category.Name);
        }
    }

    public class UpdateName
    {
        [Fact]
        public void GivenValidName_ShouldUpdateName()
        {
            // Arrange
            Category category = CreateValidCategory();
            String newName = "Books";

            // Act
            category.UpdateName(newName);

            // Assert
            Assert.Equal(newName, category.Name);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void GivenEmptyName_ShouldThrowCategoryValidationException(String? invalidName)
        {
            // Arrange
            Category category = CreateValidCategory();

            // Act & Assert
            CategoryValidationException exception = Assert.Throws<CategoryValidationException>(() => 
                category.UpdateName(invalidName!));
            
            Assert.Equal("Category name cannot be empty", exception.Message);
        }

        [Fact]
        public void GivenNameTooLong_ShouldThrowCategoryValidationException()
        {
            // Arrange
            Category category = CreateValidCategory();
            String longName = new String('a', 51);

            // Act & Assert
            CategoryValidationException exception = Assert.Throws<CategoryValidationException>(() => 
                category.UpdateName(longName));
            
            Assert.Equal("Category name cannot be longer than 50 characters", exception.Message);
        }

        [Theory]
        [InlineData("1Electronics")]
        [InlineData("@Tech")]
        [InlineData("_Gaming")]
        public void GivenNameNotStartingWithLetter_ShouldThrowCategoryValidationException(String invalidName)
        {
            // Arrange
            Category category = CreateValidCategory();

            // Act & Assert
            CategoryValidationException exception = Assert.Throws<CategoryValidationException>(() => 
                category.UpdateName(invalidName));
            
            Assert.Equal("Category name must start with a letter", exception.Message);
        }

        [Theory]
        [InlineData("Home and Garden")]
        [InlineData("Gaming 2023")]
        [InlineData("Tech Accessories")]
        public void GivenValidNameWithSpacesAndNumbers_ShouldUpdateName(String validName)
        {
            // Arrange
            Category category = CreateValidCategory();

            // Act
            category.UpdateName(validName);

            // Assert
            Assert.Equal(validName, category.Name);
        }
    }
} 