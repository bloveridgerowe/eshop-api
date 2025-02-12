using Application.DataTransferObjects;
using Application.Mappers;
using Domain.Entities;
using Xunit;

namespace Application.Tests.Mappers;

public class CategoryMapperTests
{
    private static readonly Guid ValidId = Guid.NewGuid();
    private static readonly String ValidName = "Test Category";

    [Fact]
    public void ToQueryModel_MapsAllPropertiesCorrectly()
    {
        // Arrange
        Category category = new Category(ValidId, ValidName);

        // Act
        CategoryDetails result = category.ToQueryModel();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(ValidId, result.Id);
        Assert.Equal(ValidName, result.Name);
    }
} 